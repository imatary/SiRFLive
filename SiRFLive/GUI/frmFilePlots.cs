﻿namespace SiRFLive.GUI
{
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.MessageHandling;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
	using System.Configuration;

    public class frmFilePlots : Form
    {
        private bool _abort;
        private bool _extractStatus;
        private string _inputFilePath = string.Empty;
        private string _nameOnly = string.Empty;
        private string _outFilePath_msg4 = string.Empty;
        private string _outFilePath_msg41 = string.Empty;
        internal frmFileLocationMap _SIRFFileLocMap;
        internal frmCommNavAccVsTime _SiRFNavAccVsTime;
        internal frmCommSVAvgCNo _SIRFSVAvgCNo;
        internal frmCommSVTrackedVsTime _SIRFSVTrackedVsTime;
        internal frmCommSVTrajectory _SIRFSVTraj;
        private Button btn_Abort;
        private Button btn_exit;
        private Button btn_start;
        private Button buton_Analize;
        private Button Button_Browse;
        private IContainer components;
        public DataForPlotting dataPlot = new DataForPlotting();
        private int extractThreadID;
        private Label label_status;
        private Label label1;
        public MsgFactory m_Protocols = new MsgFactory(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols.xml");
        private static frmFilePlots m_SChildform;
        private ProgressBar progressBar1;
        private TextBox TxtBox_FilePath;

        public frmFilePlots()
        {
            this.InitializeComponent();
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            this._abort = false;
            if (!this._extractStatus)
            {
                this._inputFilePath = this.TxtBox_FilePath.Text;
                int num = this._inputFilePath.LastIndexOf(@"\");
                this._nameOnly = this._inputFilePath.Substring(num + 1, (this._inputFilePath.Length - num) - 1);
                if (!File.Exists(this._inputFilePath))
                {
                    MessageBox.Show("File does not exist, please enter a valid file path", "Error");
                }
                else
                {
                    string str = this._inputFilePath.Substring(0, this._inputFilePath.Length - 4) + "_";
                    this._outFilePath_msg4 = str.Replace(",", "_") + "msg4.par";
                    this._outFilePath_msg41 = str.Replace(",", "_") + "msg41.par";
                    this.progressBar1.Value = 0;
                    Thread.Sleep(100);
                    try
                    {
                        this._extractStatus = true;
                        Thread thread = new Thread(new ThreadStart(this.extractFile));
                        this.extractThreadID = thread.ManagedThreadId;
                        thread.IsBackground = true;
                        thread.Start();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void buton_Analize_Click(object sender, EventArgs e)
        {
            if (this.dataPlot.GetNumSample_nvplot() >= 1)
            {
                this.CreateSVAverageCNoWindow();
                this.CreateSVTrackedVsTimeWindow();
                this.CreateSVTrajWindow();
                this.CreateNavAccuracyPlotWindow();
            }
        }

        private void Button_Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "gps";
            dialog.Filter = "gps files (*.gps)|*.gps";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                this.TxtBox_FilePath.Text = fileName;
            }
        }

        internal frmFileLocationMap CreateLocationMapWindow()
        {
            EventHandler method = null;
            if (!base.IsDisposed)
            {
                if (method == null)
                {
                    method = delegate {
                        string str = this._nameOnly + ": Horizontal Trajectory";
                        if ((this._SIRFFileLocMap == null) || this._SIRFFileLocMap.IsDisposed)
                        {
                            this._SIRFFileLocMap = new frmFileLocationMap(this.dataPlot.GetNumSample_nvplot(), this.dataPlot.Lat_nvplot, this.dataPlot.Lon_nvplot);
                            this._SIRFFileLocMap.MdiParent = base.MdiParent;
                            this._SIRFFileLocMap.Show();
                        }
                        this._SIRFFileLocMap.Text = str;
                        this._SIRFFileLocMap.BringToFront();
                    };
                }
                base.Invoke(method);
            }
            return this._SIRFFileLocMap;
        }

        internal frmCommNavAccVsTime CreateNavAccuracyPlotWindow()
        {
            EventHandler method = null;
            if (!base.IsDisposed)
            {
                if (method == null)
                {
                    method = delegate {
                        string str = this._nameOnly + ": Nav Accuracy vs Time";
                        if ((this._SiRFNavAccVsTime == null) || this._SiRFNavAccVsTime.IsDisposed)
                        {
                            this._SiRFNavAccVsTime = new frmCommNavAccVsTime(this.dataPlot.GetNumSample_nvplot(), this.dataPlot.Lat_nvplot, this.dataPlot.Lon_nvplot, this.dataPlot.Alt_nvplot, this.dataPlot.tows_nvplot);
                            this._SiRFNavAccVsTime.MdiParent = base.MdiParent;
                            this._SiRFNavAccVsTime.Show();
                        }
                        this._SiRFNavAccVsTime.Text = str;
                        this._SiRFNavAccVsTime.BringToFront();
                    };
                }
                base.Invoke(method);
            }
            return this._SiRFNavAccVsTime;
        }

        internal frmCommSVAvgCNo CreateSVAverageCNoWindow()
        {
            EventHandler method = null;
            if (!base.IsDisposed)
            {
                if (method == null)
                {
                    method = delegate {
                        string str = this._nameOnly + ": SV Average CNo";
                        if ((this._SIRFSVAvgCNo == null) || this._SIRFSVAvgCNo.IsDisposed)
                        {
                            this._SIRFSVAvgCNo = new frmCommSVAvgCNo(this.dataPlot.Avg_CNo);
                            this._SIRFSVAvgCNo.MdiParent = base.MdiParent;
                            this._SIRFSVAvgCNo.Show();
                        }
                        this._SIRFSVAvgCNo.Text = str;
                        this._SIRFSVAvgCNo.BringToFront();
                    };
                }
                base.Invoke(method);
            }
            return this._SIRFSVAvgCNo;
        }

        internal frmCommSVTrackedVsTime CreateSVTrackedVsTimeWindow()
        {
            EventHandler method = null;
            if (!base.IsDisposed)
            {
                if (method == null)
                {
                    method = delegate {
                        string str = this._nameOnly + ": SV Tracked vs Time";
                        if ((this._SIRFSVTrackedVsTime == null) || this._SIRFSVTrackedVsTime.IsDisposed)
                        {
                            this._SIRFSVTrackedVsTime = new frmCommSVTrackedVsTime(this.dataPlot.GetNumTrackedSVs(), this.dataPlot.SVTrkr.SVIDs, this.dataPlot.GetNumSamplesTrackVsTimePlot(), this.dataPlot.tows, this.dataPlot.cnos);
                            this._SIRFSVTrackedVsTime.MdiParent = base.MdiParent;
                            this._SIRFSVTrackedVsTime.Show();
                        }
                        this._SIRFSVTrackedVsTime.Text = str;
                        this._SIRFSVTrackedVsTime.BringToFront();
                    };
                }
                base.Invoke(method);
            }
            return this._SIRFSVTrackedVsTime;
        }

        internal frmCommSVTrajectory CreateSVTrajWindow()
        {
            EventHandler method = null;
            if (!base.IsDisposed)
            {
                if (method == null)
                {
                    method = delegate {
                        string str = this._nameOnly + ": SV Trajectory";
                        if ((this._SIRFSVTraj == null) || this._SIRFSVTraj.IsDisposed)
                        {
                            this._SIRFSVTraj = new frmCommSVTrajectory(this.dataPlot.idx_P, this.dataPlot.SVTrkr.SVIDs, this.dataPlot.elevation, this.dataPlot.azimuth);
                            this._SIRFSVTraj.MdiParent = base.MdiParent;
                            this._SIRFSVTraj.Show();
                        }
                        this._SIRFSVTraj.Text = str;
                        this._SIRFSVTraj.BringToFront();
                    };
                }
                base.Invoke(method);
            }
            return this._SIRFSVTraj;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void extractFile()
        {
            EventHandler method = null;
            Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
            StreamWriter writer = null;
            StreamWriter writer2 = null;
            StreamReader reader = null;
            new StringBuilder();
			this.label_status.BeginInvoke((MethodInvoker)delegate
			{
                this.label_status.Text = "Status: Extracting in progress, please wait...";
            });
            double lineNo = 0.0;
            try
            {
                FileInfo info = new FileInfo(this._inputFilePath);
                double length = info.Length;
                writer = new StreamWriter(this._outFilePath_msg4);
                writer2 = new StreamWriter(this._outFilePath_msg41);
                reader = new StreamReader(this._inputFilePath);
                string str = string.Empty;
                int num2 = 0;
                double num3 = 0.0;
                lineNo = 1.0;
                str = reader.ReadLine();
                while (str != null)
                {
                    if (this._abort)
                    {
                        break;
                    }
                    num3 = (num3 + str.Length) + 2.0;
                    num2++;
                    if (num2 > 100)
                    {
                        int percent = (int) ((num3 / length) * 100.0);
                        if (percent > 100)
                        {
                            percent = 100;
                        }
						this.progressBar1.BeginInvoke((MethodInvoker)delegate
						{
                            this.progressBar1.Value = percent;
                        });
                        num2 = 0;
                    }
                    if (str.StartsWith("4,"))
                    {
                        writer.WriteLine(str);
                        string csvString = str.Replace(" ", "");
                        Hashtable msgH = this.m_Protocols.ConvertCSVToHash(4, 0, "SSB", csvString);
                        this.getMsg4DataFromHash(msgH);
                    }
                    if (str.StartsWith("41,"))
                    {
                        writer2.WriteLine(str);
                        string str3 = str.Replace(" ", "");
                        Hashtable hashtable2 = this.m_Protocols.ConvertCSVToHash(0x29, 0, "SSB", str3);
                        this.getMsg41DataFromHash(hashtable2);
                    }
                    str = reader.ReadLine();
                    lineNo++;
                }
                if (writer != null)
                {
                    writer.Close();
                }
                if (writer2 != null)
                {
                    writer2.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                this._extractStatus = false;
            }
            catch (Exception exception)
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (writer2 != null)
                {
                    writer2.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                this._extractStatus = false;
                this._abort = false;
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
			this.label_status.BeginInvoke((MethodInvoker)delegate
			{
                if (this._abort)
                {
                    this.label_status.Text = "Parsing aborted.";
                }
                else
                {
                    this.label_status.Text = "Parsing completed. Total lines processed: " + ((lineNo - 1.0)).ToString();
                }
            });
            if (!this._abort)
            {
                if (this.dataPlot.GetNumSample_nvplot() >= 1)
                {
                    this.CreateSVAverageCNoWindow();
                    this.CreateSVTrackedVsTimeWindow();
                    this.CreateSVTrajWindow();
                    this.CreateNavAccuracyPlotWindow();
                    this.CreateLocationMapWindow();
                }
                if (method == null)
                {
                    method = delegate {
                        base.Close();
                    };
                }
                base.BeginInvoke(method);
            }
        }

        private void frmFilePlots_Load(object sender, EventArgs e)
        {
        }

        public static frmFilePlots GetChildInstance()
        {
            if (m_SChildform == null)
            {
                m_SChildform = new frmFilePlots();
            }
            return m_SChildform;
        }

        private void getMsg41DataFromHash(Hashtable msgH)
        {
            ushort navValid = 0xffff;
            PositionInfo.PositionStruct struct2 = new PositionInfo.PositionStruct();
            if (msgH.ContainsKey("Nav Valid"))
            {
                struct2.NavValid = Convert.ToUInt16((string) msgH["Nav Valid"]);
                navValid = struct2.NavValid;
            }
            if (msgH.ContainsKey("TOW"))
            {
                struct2.TOW = Convert.ToDouble((string) msgH["TOW"]);
                if (navValid == 0)
                {
                    this.dataPlot.InsertTow_nvplot(struct2.TOW / 1000.0);
                }
            }
            if (msgH.ContainsKey("Latitude"))
            {
                struct2.Latitude = Convert.ToDouble((string) msgH["Latitude"]) / 10000000.0;
                if (navValid == 0)
                {
                    this.dataPlot.InsertLat(struct2.Latitude);
                }
            }
            if (msgH.ContainsKey("Longitude"))
            {
                struct2.Longitude = Convert.ToDouble((string) msgH["Longitude"]) / 10000000.0;
                if (navValid == 0)
                {
                    this.dataPlot.InsertLon(struct2.Longitude);
                }
            }
            if (msgH.ContainsKey("Altitude from Ellipsoid"))
            {
                struct2.Altitude = Convert.ToDouble((string) msgH["Altitude from Ellipsoid"]) / 100.0;
                if (navValid == 0)
                {
                    this.dataPlot.InsertAlt(struct2.Altitude);
                    this.dataPlot.UpdateIdx_nvplot();
                }
            }
        }

        private void getMsg4DataFromHash(Hashtable msgH)
        {
            bool flag = false;
            if (msgH.ContainsKey("GPS TOW"))
            {
                double num = Convert.ToDouble((string) msgH["GPS TOW"]);
                this.dataPlot.InsertTOW(num / 100.0);
            }
            for (int i = 0; i < 12; i++)
            {
                string key = "SVID" + ((i + 1)).ToString();
                string text1 = "State " + key;
                string str3 = "Elev " + key;
                string str4 = "Azimuth " + key;
                if (msgH.ContainsKey(key))
                {
                    byte prn = Convert.ToByte((string) msgH[key]);
                    if ((prn > 0) && (prn < DataForGUI.MAX_PRN))
                    {
                        double num4 = 0.0;
                        double num5 = 0.0;
                        for (int j = 1; j < 11; j++)
                        {
                            string str2 = "C/NO" + j.ToString() + " " + key;
                            if (msgH.ContainsKey(str2))
                            {
                                double num7 = Convert.ToDouble((string) msgH[str2]);
                                num4 += num7;
                                if (num7 != 0.0)
                                {
                                    num5++;
                                }
                            }
                        }
                        if (num5 == 0.0)
                        {
                            num5 = 1.0;
                        }
                        float cno = (float) (num4 / num5);
                        int index = -1;
                        if (num4 > 10.0)
                        {
                            index = this.dataPlot.SVTrkr.getIdx(prn);
                            this.dataPlot.InsertCNo(prn, (double) cno);
                        }
                        if (((msgH.ContainsKey(str3) && (num4 > 10.0)) && ((index >= 0) && (index < TrackSVRec.MAX_SVT))) && (this.dataPlot.idx_P[index] < DataForPlotting.MAX_P))
                        {
                            this.dataPlot.elevation[index, this.dataPlot.idx_P[index]] = Convert.ToSingle((string) msgH[str3]);
                        }
                        if (((msgH.ContainsKey(str4) && (num4 > 10.0)) && ((index >= 0) && (index < TrackSVRec.MAX_SVT))) && (this.dataPlot.idx_P[index] < DataForPlotting.MAX_P))
                        {
                            this.dataPlot.azimuth[index, this.dataPlot.idx_P[index]] = Convert.ToSingle((string) msgH[str4]);
                            flag = true;
                            this.dataPlot.idx_P[index]++;
                            if (this.dataPlot.idx_P[index] >= DataForPlotting.MAX_P)
                            {
                                this.dataPlot.idx_P[index] = 0;
                            }
                        }
                        if (((num4 > 10.0) && flag) && ((index >= 0) && (index < TrackSVRec.MAX_SVT)))
                        {
                            int num10 = this.dataPlot.idx_P[index];
                            if (((num10 >= 2) && (Math.Abs((float) (this.dataPlot.elevation[index, num10 - 1] - this.dataPlot.elevation[index, num10 - 2])) < 0.001)) && (Math.Abs((float) (this.dataPlot.azimuth[index, num10 - 1] - this.dataPlot.azimuth[index, num10 - 2])) < 0.001))
                            {
                                this.dataPlot.idx_P[index]--;
                            }
                            flag = false;
                        }
                        this.dataPlot.UpdateAvgCNoTable(Convert.ToSingle((string) msgH[str3]), Convert.ToSingle((string) msgH[str4]), cno);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmFilePlots));
            this.progressBar1 = new ProgressBar();
            this.label_status = new Label();
            this.Button_Browse = new Button();
            this.TxtBox_FilePath = new TextBox();
            this.btn_start = new Button();
            this.btn_Abort = new Button();
            this.btn_exit = new Button();
            this.buton_Analize = new Button();
            this.label1 = new Label();
            base.SuspendLayout();
            this.progressBar1.Location = new Point(0x38, 0x7e);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(0x21f, 0x11);
            this.progressBar1.TabIndex = 0x40;
            this.label_status.AutoSize = true;
            this.label_status.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label_status.ForeColor = Color.RoyalBlue;
            this.label_status.Location = new Point(0x47, 0xa2);
            this.label_status.Name = "label_status";
            this.label_status.Size = new Size(0x3b, 13);
            this.label_status.TabIndex = 0x3f;
            this.label_status.Text = "Status: idle";
            this.Button_Browse.Location = new Point(0x234, 0x22);
            this.Button_Browse.Name = "Button_Browse";
            this.Button_Browse.Size = new Size(0x22, 20);
            this.Button_Browse.TabIndex = 0x42;
            this.Button_Browse.Text = "&...";
            this.Button_Browse.UseVisualStyleBackColor = true;
            this.Button_Browse.Click += new EventHandler(this.Button_Browse_Click);
            this.TxtBox_FilePath.Location = new Point(0x38, 0x22);
            this.TxtBox_FilePath.Name = "TxtBox_FilePath";
            this.TxtBox_FilePath.Size = new Size(0x1f6, 20);
            this.TxtBox_FilePath.TabIndex = 0x41;
            this.btn_start.Location = new Point(0x20b, 0x48);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new Size(0x4b, 0x17);
            this.btn_start.TabIndex = 0x43;
            this.btn_start.Text = "&Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new EventHandler(this.btn_start_Click);
            this.btn_Abort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Abort.Location = new Point(0x7b, 0xf3);
            this.btn_Abort.Name = "btn_Abort";
            this.btn_Abort.Size = new Size(0x4b, 0x17);
            this.btn_Abort.TabIndex = 0x44;
            this.btn_Abort.Text = "A&bort";
            this.btn_Abort.UseVisualStyleBackColor = true;
            this.btn_Abort.Visible = false;
            this.btn_Abort.Click += new EventHandler(this.btn_Abort_Click);
            this.btn_exit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_exit.Location = new Point(0x20b, 0xf3);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new Size(0x4b, 0x17);
            this.btn_exit.TabIndex = 0x45;
            this.btn_exit.Text = "E&xit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new EventHandler(this.btn_exit_Click);
            this.buton_Analize.Location = new Point(0x19, 0xf3);
            this.buton_Analize.Name = "buton_Analize";
            this.buton_Analize.Size = new Size(0x4b, 0x17);
            this.buton_Analize.TabIndex = 70;
            this.buton_Analize.Text = "&Analyze";
            this.buton_Analize.UseVisualStyleBackColor = true;
            this.buton_Analize.Visible = false;
            this.buton_Analize.Click += new EventHandler(this.buton_Analize_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x38, 15);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x34, 13);
            this.label1.TabIndex = 0x47;
            this.label1.Text = "File name";
            base.AcceptButton = this.btn_start;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.btn_exit;
            base.ClientSize = new Size(0x26f, 0x11d);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.buton_Analize);
            base.Controls.Add(this.btn_exit);
            base.Controls.Add(this.btn_Abort);
            base.Controls.Add(this.btn_start);
            base.Controls.Add(this.Button_Browse);
            base.Controls.Add(this.TxtBox_FilePath);
            base.Controls.Add(this.progressBar1);
            base.Controls.Add(this.label_status);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmFilePlots";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "File Analysis/Plotting";
            base.Load += new EventHandler(this.frmFilePlots_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

