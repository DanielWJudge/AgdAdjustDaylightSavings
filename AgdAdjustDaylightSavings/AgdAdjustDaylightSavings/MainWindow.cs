using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ServiceStack.DataAnnotations;

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


            this.HandleCreated += (sender, args) =>
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
                    labelDayLightStart.Text = string.Format("Daylight Start: {0:g}", daylightTime.Start);
                    labelDayLightEnd.Text = string.Format("Daylight End: {0:g}", daylightTime.End);
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

            foreach (var file in _loadedFiles)
            {
                richTextBox1.AppendText("adjusting file: " + file + "\r\n");
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
