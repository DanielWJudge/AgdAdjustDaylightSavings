namespace AgdAdjustDaylightSavings
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.buttonAddFiles = new System.Windows.Forms.Button();
            this.comboBoxTimeZone = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelDayLightStart = new System.Windows.Forms.Label();
            this.labelDayLightEnd = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.labelTotalFilesLoaded = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAdjustFiles = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAddFiles
            // 
            this.buttonAddFiles.AutoSize = true;
            this.buttonAddFiles.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddFiles.Image")));
            this.buttonAddFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddFiles.Location = new System.Drawing.Point(15, 18);
            this.buttonAddFiles.Margin = new System.Windows.Forms.Padding(5);
            this.buttonAddFiles.Name = "buttonAddFiles";
            this.buttonAddFiles.Size = new System.Drawing.Size(134, 31);
            this.buttonAddFiles.TabIndex = 1;
            this.buttonAddFiles.Text = "Add File(s)...";
            this.buttonAddFiles.UseVisualStyleBackColor = true;
            // 
            // comboBoxTimeZone
            // 
            this.comboBoxTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTimeZone.FormattingEnabled = true;
            this.comboBoxTimeZone.Location = new System.Drawing.Point(115, 4);
            this.comboBoxTimeZone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxTimeZone.Name = "comboBoxTimeZone";
            this.comboBoxTimeZone.Size = new System.Drawing.Size(287, 25);
            this.comboBoxTimeZone.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 33);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select TimeZone:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.comboBoxTimeZone);
            this.flowLayoutPanel1.Controls.Add(this.labelDayLightStart);
            this.flowLayoutPanel1.Controls.Add(this.labelDayLightEnd);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(15, 346);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(949, 38);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // labelDayLightStart
            // 
            this.labelDayLightStart.AutoSize = true;
            this.labelDayLightStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDayLightStart.Location = new System.Drawing.Point(408, 0);
            this.labelDayLightStart.Name = "labelDayLightStart";
            this.labelDayLightStart.Size = new System.Drawing.Size(0, 33);
            this.labelDayLightStart.TabIndex = 4;
            this.labelDayLightStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelDayLightEnd
            // 
            this.labelDayLightEnd.AutoSize = true;
            this.labelDayLightEnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDayLightEnd.Location = new System.Drawing.Point(414, 0);
            this.labelDayLightEnd.Name = "labelDayLightEnd";
            this.labelDayLightEnd.Size = new System.Drawing.Size(0, 33);
            this.labelDayLightEnd.TabIndex = 5;
            this.labelDayLightEnd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(15, 479);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(949, 216);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // labelTotalFilesLoaded
            // 
            this.labelTotalFilesLoaded.AutoSize = true;
            this.labelTotalFilesLoaded.Location = new System.Drawing.Point(157, 25);
            this.labelTotalFilesLoaded.Name = "labelTotalFilesLoaded";
            this.labelTotalFilesLoaded.Size = new System.Drawing.Size(0, 17);
            this.labelTotalFilesLoaded.TabIndex = 6;
            this.labelTotalFilesLoaded.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 57);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(949, 282);
            this.dataGridView1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 459);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Status:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonAdjustFiles
            // 
            this.buttonAdjustFiles.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAdjustFiles.AutoSize = true;
            this.buttonAdjustFiles.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAdjustFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAdjustFiles.Location = new System.Drawing.Point(422, 408);
            this.buttonAdjustFiles.Margin = new System.Windows.Forms.Padding(5);
            this.buttonAdjustFiles.Name = "buttonAdjustFiles";
            this.buttonAdjustFiles.Size = new System.Drawing.Size(134, 35);
            this.buttonAdjustFiles.TabIndex = 9;
            this.buttonAdjustFiles.Text = "Adjust Files!";
            this.buttonAdjustFiles.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 707);
            this.Controls.Add(this.buttonAdjustFiles);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.labelTotalFilesLoaded);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.buttonAddFiles);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainWindow";
            this.Text = "Adjust AGD Timestamps For Daylight Savings";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAddFiles;
        private System.Windows.Forms.ComboBox comboBoxTimeZone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelDayLightStart;
        private System.Windows.Forms.Label labelDayLightEnd;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label labelTotalFilesLoaded;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonAdjustFiles;
    }
}

