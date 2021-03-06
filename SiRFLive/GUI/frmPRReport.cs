﻿namespace SiRFLive.GUI
{
    using SiRFLive.Analysis;
    using SiRFLive.Reporting;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmPRReport : Form
    {
        private Button Cancel;
        private IContainer components;
        private Button Generate;
        private TextBox GPS_FILE_EDIT_BOX;
        private Button GPSFileDirBrowser;
        private TextBox IMU_FILE_EDIT_BOX;
        private Button IMUFileDirBrowser;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox OUT_FILE_EDIT_BOX;
        private Button OutFileDirBrowser;
        private TextBox RINEX_FILE_EDIT_BOX;
        private Button RinexFileDirBrowser;

        public frmPRReport()
        {
            this.InitializeComponent();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void FileBrowser(TextBox anything, string initialDir)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "File Locator";
            dialog.InitialDirectory = initialDir;
            dialog.Filter = "All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.SetTextBoxText(anything, dialog.FileName);
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            new ComputePRerror().ComputePRerrorOneFile(this.GPS_FILE_EDIT_BOX.Text, this.RINEX_FILE_EDIT_BOX.Text, this.IMU_FILE_EDIT_BOX.Text, this.OUT_FILE_EDIT_BOX.Text);
            new Report().PseudoRangeError_Summary(this.OUT_FILE_EDIT_BOX.Text);
            MessageBox.Show("Done computing PR error.");
            this.ShowWebPage(this.OUT_FILE_EDIT_BOX.Text);
            base.Close();
        }

        private void GPSFileDirBrowser_Click(object sender, EventArgs e)
        {
            this.FileBrowser(this.GPS_FILE_EDIT_BOX, "Select .gps file...");
        }

        private void IMUFileDirBrowser_Click(object sender, EventArgs e)
        {
            this.FileBrowser(this.IMU_FILE_EDIT_BOX, "Select IMU true nav position file...");
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmPRReport));
            this.Generate = new Button();
            this.Cancel = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.GPS_FILE_EDIT_BOX = new TextBox();
            this.RINEX_FILE_EDIT_BOX = new TextBox();
            this.IMU_FILE_EDIT_BOX = new TextBox();
            this.OUT_FILE_EDIT_BOX = new TextBox();
            this.GPSFileDirBrowser = new Button();
            this.RinexFileDirBrowser = new Button();
            this.IMUFileDirBrowser = new Button();
            this.OutFileDirBrowser = new Button();
            base.SuspendLayout();
            this.Generate.Location = new Point(0xd5, 0xa3);
            this.Generate.Name = "Generate";
            this.Generate.Size = new Size(0x4b, 0x17);
            this.Generate.TabIndex = 0;
            this.Generate.Text = "&Generate";
            this.Generate.UseVisualStyleBackColor = true;
            this.Generate.Click += new EventHandler(this.Generate_Click);
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new Point(0x139, 0xa3);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new Size(0x4b, 0x17);
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "&Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new EventHandler(this.Cancel_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x1d, 0x1f);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x30, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "GPS File";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x1d, 60);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Rinex File";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x1d, 90);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x2e, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "IMU File";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0x1d, 0x79);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x3a, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Output File";
            this.GPS_FILE_EDIT_BOX.Location = new Point(0x60, 0x1b);
            this.GPS_FILE_EDIT_BOX.Name = "GPS_FILE_EDIT_BOX";
            this.GPS_FILE_EDIT_BOX.Size = new Size(0x1c5, 20);
            this.GPS_FILE_EDIT_BOX.TabIndex = 6;
            this.RINEX_FILE_EDIT_BOX.Location = new Point(0x60, 0x38);
            this.RINEX_FILE_EDIT_BOX.Name = "RINEX_FILE_EDIT_BOX";
            this.RINEX_FILE_EDIT_BOX.Size = new Size(0x1c5, 20);
            this.RINEX_FILE_EDIT_BOX.TabIndex = 7;
            this.IMU_FILE_EDIT_BOX.Location = new Point(0x60, 0x56);
            this.IMU_FILE_EDIT_BOX.Name = "IMU_FILE_EDIT_BOX";
            this.IMU_FILE_EDIT_BOX.Size = new Size(0x1c5, 20);
            this.IMU_FILE_EDIT_BOX.TabIndex = 8;
            this.OUT_FILE_EDIT_BOX.Location = new Point(0x60, 0x75);
            this.OUT_FILE_EDIT_BOX.Name = "OUT_FILE_EDIT_BOX";
            this.OUT_FILE_EDIT_BOX.Size = new Size(0x1c5, 20);
            this.OUT_FILE_EDIT_BOX.TabIndex = 9;
            this.GPSFileDirBrowser.Location = new Point(0x22b, 0x1a);
            this.GPSFileDirBrowser.Name = "GPSFileDirBrowser";
            this.GPSFileDirBrowser.Size = new Size(30, 0x17);
            this.GPSFileDirBrowser.TabIndex = 10;
            this.GPSFileDirBrowser.Text = "...";
            this.GPSFileDirBrowser.UseVisualStyleBackColor = true;
            this.GPSFileDirBrowser.Click += new EventHandler(this.GPSFileDirBrowser_Click);
            this.RinexFileDirBrowser.Location = new Point(0x22b, 0x37);
            this.RinexFileDirBrowser.Name = "RinexFileDirBrowser";
            this.RinexFileDirBrowser.Size = new Size(30, 0x17);
            this.RinexFileDirBrowser.TabIndex = 11;
            this.RinexFileDirBrowser.Text = "...";
            this.RinexFileDirBrowser.UseVisualStyleBackColor = true;
            this.RinexFileDirBrowser.Click += new EventHandler(this.RinexFileDirBrowser_Click);
            this.IMUFileDirBrowser.Location = new Point(0x22b, 0x55);
            this.IMUFileDirBrowser.Name = "IMUFileDirBrowser";
            this.IMUFileDirBrowser.Size = new Size(30, 0x17);
            this.IMUFileDirBrowser.TabIndex = 12;
            this.IMUFileDirBrowser.Text = "...";
            this.IMUFileDirBrowser.UseVisualStyleBackColor = true;
            this.IMUFileDirBrowser.Click += new EventHandler(this.IMUFileDirBrowser_Click);
            this.OutFileDirBrowser.Location = new Point(0x22b, 0x74);
            this.OutFileDirBrowser.Name = "OutFileDirBrowser";
            this.OutFileDirBrowser.Size = new Size(30, 0x17);
            this.OutFileDirBrowser.TabIndex = 13;
            this.OutFileDirBrowser.Text = "...";
            this.OutFileDirBrowser.UseVisualStyleBackColor = true;
            this.OutFileDirBrowser.Click += new EventHandler(this.OutFileDirBrowser_Click);
            base.AcceptButton = this.Generate;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.Cancel;
            base.ClientSize = new Size(600, 0xdf);
            base.Controls.Add(this.OutFileDirBrowser);
            base.Controls.Add(this.IMUFileDirBrowser);
            base.Controls.Add(this.RinexFileDirBrowser);
            base.Controls.Add(this.GPSFileDirBrowser);
            base.Controls.Add(this.OUT_FILE_EDIT_BOX);
            base.Controls.Add(this.IMU_FILE_EDIT_BOX);
            base.Controls.Add(this.RINEX_FILE_EDIT_BOX);
            base.Controls.Add(this.GPS_FILE_EDIT_BOX);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.Cancel);
            base.Controls.Add(this.Generate);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmPRReport";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "PseudoRange Report";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void OutFileDirBrowser_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.OUT_FILE_EDIT_BOX.Text = dialog.SelectedPath + @"\out.csv";
            }
        }

        private void RinexFileDirBrowser_Click(object sender, EventArgs e)
        {
            this.FileBrowser(this.RINEX_FILE_EDIT_BOX, "Select Rinex true sv position file...");
        }

        public void SetTextBoxText(TextBox anything, string text)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                anything.Text = text;
            });
        }

        private void ShowWebPage(string address)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = @"C:\Program Files\Internet Explorer\iexplore.exe";
                process.StartInfo.Arguments = address;
                process.EnableRaisingEvents = false;
                process.Start();
            }
            catch (Win32Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}

