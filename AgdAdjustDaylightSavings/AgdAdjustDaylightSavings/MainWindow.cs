using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ServiceStack.OrmLite;

namespace AgdAdjustDaylightSavings
{
    public partial class MainWindow : Form
    {
        private List<AgdFile> _loadedFiles;
        private BindingSource _source;

        public MainWindow()
        {
            InitializeComponent();

            buttonAddFiles.Click += (sender, args) => ButtonAddFilesClicked();
            buttonAdjustFiles.Click += (sender, args) => AdjustFiles();
            richTextBox1.TextChanged += (sender, args) =>
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length; //Set the current caret position at the end
                richTextBox1.ScrollToCaret(); //Now scroll it automatically
            };
            
            HandleCreated += (sender, args) =>
            {
                this.Text = string.Format("{0} v{1}", this.Text,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
                    comboBoxTimeZone.Items.Add(z);

                comboBoxTimeZone.SelectedItem =
                    TimeZoneInfo.GetSystemTimeZones()
                        .First(x => x.StandardName.Equals(TimeZone.CurrentTimeZone.StandardName));
            };

            comboBoxTimeZone.SelectedIndexChanged += (sender, args) =>
            {
                var timeZoneInfo = comboBoxTimeZone.SelectedItem as TimeZoneInfo;
                if (timeZoneInfo == null)
                    return;

                var daylightTime = TimeZoneHelpers.GetDaylightChanges(timeZoneInfo.Id, DateTime.UtcNow.Year);
                if (daylightTime != null)
                {
                    labelDayLightStart.Text = string.Format("Daylight Start: {0:G}", daylightTime.Start);
                    labelDayLightEnd.Text = string.Format("Daylight End: {0:G}", daylightTime.End);
                }
                else
                {
                    labelDayLightStart.Text = string.Format("Daylight Start: unknown");
                    labelDayLightEnd.Text = string.Format("Daylight End: unknown");
                }
            };
        }

        private void AdjustFiles()
        {
            if (_loadedFiles == null || !_loadedFiles.Any())
            {
                MessageBox.Show(this, "Please Load Some Files Before Adjusting", "Load Files First",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var timeZoneInfo = comboBoxTimeZone.SelectedItem as TimeZoneInfo;
            if (timeZoneInfo == null)
                return;

            buttonAddFiles.Enabled = false;
            buttonAdjustFiles.Enabled = false;

            foreach (var file in _loadedFiles)
                AdjustFile(file, timeZoneInfo.Id);

            richTextBox1.AppendText("ALL DONE!\r\n");

            buttonAddFiles.Enabled = true;
            buttonAdjustFiles.Enabled = true;
        }

        private void AdjustFile(AgdFile agdFile, string timeZoneInfoId)
        {
            if (agdFile == null)
                throw new NullReferenceException("agdFile can't be null");

            if (string.IsNullOrEmpty(timeZoneInfoId))
                throw new NullReferenceException("timeZoneInfoId can't be null");

            var daylightTime = TimeZoneHelpers.GetDaylightChanges(timeZoneInfoId, agdFile.LastEpoch.Year);

            richTextBox1.AppendText(string.Format("{0}: started adjusting\r\n", agdFile));

            using (var db = new OrmLiteConnectionFactory(agdFile.GetSQLiteConnectionString(), SqliteDialect.Provider).OpenDbConnection())
            {
                AdjustFileForFallDst(agdFile, db, daylightTime);
                AdjustFileForSpringDst(agdFile, db, daylightTime);

                var oldFirstEpoch = agdFile.FirstEpoch;
                var oldLastEpoch = agdFile.LastEpoch;
                var totalEpochs = agdFile.EpochCount;
                agdFile.GetFirstAndLastEpoch();
                richTextBox1.AppendText(string.Format("{0}: original first epoch - {1:G} | new first epoch - {2:G}\r\n", agdFile, oldFirstEpoch, agdFile.FirstEpoch));
                richTextBox1.AppendText(string.Format("{0}: original last epoch - {1:G} | new last epoch - {2:G}\r\n", agdFile, oldLastEpoch, agdFile.LastEpoch));
                richTextBox1.AppendText(string.Format("{0}: original totalEpochs - {1:G} | new total epochs - {2:G}\r\n", agdFile, totalEpochs, agdFile.EpochCount));
                richTextBox1.AppendText(string.Format("{0}: finished adjusting file\r\n", agdFile));
            }
        }

        private void AdjustFileForSpringDst(AgdFile agdFile, IDbConnection db, DaylightTime daylightTime)
        {
            //see if there's any epochs before DST
            int epochsBeforeSpringDst =
                db.Scalar<int>(
                    db.From<AgdTableTimestampAxis1>()
                        .Select(Sql.Count("*"))
                        .Where(q => q.TimestampTicks < daylightTime.Start.Ticks));
            bool dataBeforeSpringDst = epochsBeforeSpringDst > 0;

            int epochsAfterSpringDst =
                db.Scalar<int>(
                    db.From<AgdTableTimestampAxis1>()
                        .Select(Sql.Count("*"))
                        .Where(q => q.TimestampTicks > daylightTime.Start.Ticks));
            
            bool dataAfterSpringDst = epochsAfterSpringDst > 0;

            if (!dataBeforeSpringDst && dataAfterSpringDst)
            {
                //no need to do anything
                richTextBox1.AppendText(string.Format("{0}: NOT adjusting file for spring DST\r\n", agdFile));
                return;
            }

            richTextBox1.AppendText(string.Format("{0}: adjusting file for spring DST\r\n", agdFile));

            //adjust timestamps for data after DST by subtracting an hour
            string sql =
                string.Format(
                    "UPDATE data SET dataTimestamp = dataTimestamp + {0} WHERE dataTimestamp >= {1}",
                    daylightTime.Delta.Ticks, daylightTime.Start.Ticks);
            db.ExecuteSql(sql);

            //add the extra hour of data (October 25th 0200) 
            var totalEpochsToInsert = daylightTime.Delta.TotalSeconds / agdFile.EpochLengthInSeconds;
            var ticksPerEpoch = TimeSpan.FromSeconds(agdFile.EpochLengthInSeconds).Ticks;
            for (int i = 0; i < totalEpochsToInsert; i++)
            {
                sql = string.Format("insert into data(dataTimestamp) values ({0});",
                    (daylightTime.Start.Ticks + (i * ticksPerEpoch)));
                db.ExecuteSql(sql);
            }

            var columnNames = db.GetColumnNames("data");
            foreach (var columnName in columnNames)
            {
                sql = string.Format("UPDATE data SET {0} = 0 WHERE {0} IS NULL", columnName);
                db.ExecuteSql(sql);
            }
            //adjust WTV
            if (db.TableExists("filters"))
            {
                sql =
                    string.Format(
                        "UPDATE filters SET filterStartTimestamp = filterStartTimestamp + {0} WHERE filterStartTimestamp >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.Start.Ticks);
                db.ExecuteSql(sql);

                sql =
                    string.Format(
                        "UPDATE filters SET filterStopTimestamp = filterStopTimestamp + {0} WHERE filterStopTimestamp >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.Start.Ticks);
                db.ExecuteSql(sql);
            }

            if (db.TableExists("wtvBouts"))
            {
                sql =
                    string.Format(
                        "UPDATE wtvBouts SET startTicks = startTicks + {0} WHERE startTicks >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.Start.Ticks);
                db.ExecuteSql(sql);

                sql =
                    string.Format(
                        "UPDATE wtvBouts SET stopTicks = stopTicks + {0} WHERE stopTicks >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.Start.Ticks);
                db.ExecuteSql(sql);
            }

            //adjust capsense
            if (db.TableExists("capsense"))
            {
                //adjust timestamps for data after DST by subtracting an hour
                sql = string.Format(
                        "UPDATE capsense SET timeStamp = timeStamp + {0} WHERE timeStamp >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.Start.Ticks);
                db.ExecuteSql(sql);
            }
        }

        private void AdjustFileForFallDst(AgdFile agdFile, IDbConnection db, DaylightTime daylightTime)
        {
            //see if there's any epochs before DST
            int epochsBeforeFallDst =
                db.Scalar<int>(
                    db.From<AgdTableTimestampAxis1>()
                        .Select(Sql.Count("*"))
                        .Where(q => q.TimestampTicks < daylightTime.End.Ticks));
            bool dataBeforeFallDst = epochsBeforeFallDst > 0;

            int epochsAfterFallDst =
                db.Scalar<int>(
                    db.From<AgdTableTimestampAxis1>()
                        .Select(Sql.Count("*"))
                        .Where(q => q.TimestampTicks > daylightTime.End.Ticks));
            bool dataAfterFallDst = epochsAfterFallDst > 0;
            
            if (dataBeforeFallDst && !dataAfterFallDst)
            {
                //there's only data before
                //no need to do anything
                richTextBox1.AppendText(string.Format("{0}: not adjusting file for fall DST\r\n", agdFile));
                return;
            }

            richTextBox1.AppendText(string.Format("{0}: adjusting file for fall DST\r\n", agdFile));

            //delete the extra hour of data (October 25th 0200) 
            string sql = string.Format("DELETE FROM data WHERE dataTimestamp >= {0} AND dataTimeStamp < {1}",
                daylightTime.End.Ticks, daylightTime.End.Add(daylightTime.Delta).Ticks);
            db.ExecuteSql(sql);

            //adjust timestamps for data after DST by subtracting an hour
            sql =
                string.Format(
                    "UPDATE data SET dataTimestamp = dataTimestamp - {0} WHERE dataTimestamp >= {1}",
                    daylightTime.Delta.Ticks, daylightTime.End.Ticks);
            db.ExecuteSql(sql);

            //adjust WTV
            if (db.TableExists("filters"))
            {
                sql =
                    string.Format(
                        "UPDATE filters SET filterStartTimestamp = filterStartTimestamp - {0} WHERE filterStartTimestamp >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.End.Ticks);
                db.ExecuteSql(sql);

                sql =
                    string.Format(
                        "UPDATE filters SET filterStopTimestamp = filterStopTimestamp - {0} WHERE filterStopTimestamp >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.End.Ticks);
                db.ExecuteSql(sql);
            }

            if (db.TableExists("wtvBouts"))
            {
                sql =
                    string.Format(
                        "UPDATE wtvBouts SET startTicks = startTicks - {0} WHERE startTicks >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.End.Ticks);
                db.ExecuteSql(sql);

                sql =
                    string.Format(
                        "UPDATE wtvBouts SET stopTicks = stopTicks - {0} WHERE stopTicks >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.End.Ticks);
                db.ExecuteSql(sql);
            }

            //adjust capsense
            if (db.TableExists("capsense"))
            {
                //delete the extra hour of data (October 25th 0200) 
                sql = string.Format("DELETE FROM capsense WHERE timeStamp  >= {0} AND timeStamp < {1}",
                    daylightTime.End.Ticks, daylightTime.End.Add(daylightTime.Delta).Ticks);
                db.ExecuteSql(sql);

                //adjust timestamps for data after DST by subtracting an hour
                sql =
                    string.Format(
                        "UPDATE capsense SET timeStamp = timeStamp - {0} WHERE timeStamp >= {1}",
                        daylightTime.Delta.Ticks, daylightTime.End.Ticks);
                db.ExecuteSql(sql);
            }
        }

        private void ButtonAddFilesClicked()
        {
            using (var openFile = new OpenFileDialog())
            {
                openFile.Multiselect = true;
                const string FILE_EXTENSION = "*.agd";
                openFile.Filter = "Files (" + FILE_EXTENSION + ")|" + FILE_EXTENSION;
                openFile.Title = "Select File(s)";
                openFile.FileName = "";
                if (openFile.ShowDialog() == DialogResult.OK)
                    AddFiles(openFile.FileNames);
            }
        }

        private void AddFiles(IEnumerable<string> fileNames)
        {
            if (fileNames == null || !fileNames.Any())
                return;

            if (_loadedFiles == null)
            {
                _loadedFiles = new List<AgdFile>(fileNames.Count());
                _source = new BindingSource();
                _source.DataSource = _loadedFiles;
                dataGridView1.DataSource = _source;
            }

            var filesToAdd =
                fileNames.Where(
                    x => !_loadedFiles.Any(y => y.FileName.Equals(x, StringComparison.CurrentCultureIgnoreCase)));

            if (filesToAdd.Any())
            {
                richTextBox1.AppendText("adding " + filesToAdd.Count() + " files\r\n");
                _loadedFiles.AddRange(filesToAdd.Select(file => new AgdFile(file)));
                _source.ResetBindings(false);
                labelTotalFilesLoaded.Text = string.Format("{0} Files Loaded", _loadedFiles.Count);
            }
        }
    }

    public static class DbConnectionExtensions
    {
        public static List<string> GetColumnNames(this IDbConnection db, string tableName)
        {
            var columnNames = new List<string>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = GetCommandText(tableName);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        columnNames.Add(rdr["name"].ToString());
                }
            }
            return columnNames;
        }

        private static string GetCommandText(string tableName)
        {
            return string.Format("PRAGMA table_info({0})", tableName);
        }
    }
}
