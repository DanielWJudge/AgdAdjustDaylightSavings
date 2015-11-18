using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ServiceStack.DataAnnotations;
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

                var daylightTime = TimeZoneHelpers.GetDaylightChanges(timeZoneInfo.Id, 2015);
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

            var daylightTime = TimeZoneHelpers.GetDaylightChanges(timeZoneInfo.Id, 2015);
            foreach (var file in _loadedFiles)
                AdjustFile(file, daylightTime);

            richTextBox1.AppendText("ALL DONE!\r\n");

            buttonAddFiles.Enabled = true;
            buttonAdjustFiles.Enabled = true;
        }

        private void AdjustFile(AgdFile file, DaylightTime daylightTime)
        {
            if (file == null)
                throw new NullReferenceException("file can't be null");

            if (daylightTime == null)
                throw new ArgumentNullException("");

            richTextBox1.AppendText(string.Format("{0}: started adjusting\r\n", file));

            using (var db = new OrmLiteConnectionFactory(file.GetSQLiteConnectionString(), SqliteDialect.Provider).OpenDbConnection())
            {
                //see if there's any epochs before DST
                int epochsBeforeDST =
                    db.Scalar<int>(
                        db.From<AgdTableTimestampAxis1>()
                            .Select(Sql.Count("*"))
                            .Where(q => q.TimestampTicks < daylightTime.End.Ticks));
                bool dataBeforeDST = epochsBeforeDST > 0;

                int epochsAfterDST =
                    db.Scalar<int>(
                        db.From<AgdTableTimestampAxis1>()
                            .Select(Sql.Count("*"))
                            .Where(q => q.TimestampTicks > daylightTime.End.Ticks));
                bool dataAfterDST = epochsAfterDST > 0;

                
                if (dataBeforeDST && !dataAfterDST)
                {
                    //there's only data before
                    //no need to do anything
                    richTextBox1.AppendText(string.Format("{0}: only has data BEFORE daylight savings time ended\r\n", file));
                    return;
                }

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

                //adjust sleep??


                var oldFirstEpoch = file.FirstEpoch;
                var oldLastEpoch = file.LastEpoch;
                file.GetFirstAndLastEpoch();
                richTextBox1.AppendText(string.Format("{0}: original first epoch - {1:G} | new first epoch - {2:G}\r\n", file, oldFirstEpoch, file.FirstEpoch));
                richTextBox1.AppendText(string.Format("{0}: original last epoch - {1:G} | new last epoch - {2:G}\r\n", file, oldLastEpoch, file.LastEpoch));
                richTextBox1.AppendText(string.Format("{0}: finished adjusting file\r\n", file));
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

    [Alias("settings")]
    public class AgdSettings
    {
        [Alias("settingID")]
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        [Alias("settingName")]
        [StringLength(64)]
        public string Name { get; set; }

        [Alias("settingValue")]
        [StringLength(8192)]
        public string Value { get; set; }
    }

    [Alias("data")]
    public class AgdTableTimestampAxis1
    {
        [Alias("dataTimestamp")]
        public long TimestampTicks { get; set; }

        [Alias("axis1")]
        public double Axis1Counts { get; set; }
    }
}
