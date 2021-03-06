﻿namespace SiRFLive.GUI.Commmunication
{
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.GUI.General;
    using SiRFLive.Utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class frmSatelliteStats : Form
    {
        private List<string> _messageFilterQNames = new List<string>();
        private static int _numberOpen;
        private string _persistedWindowName = "Satellite Satistics Window";
        private DataGridView _SatelliteStatsDataGridView;
        private ObjectInterface _SatelliteStatsGuiCtrl = new ObjectInterface();
        private DataGridViewTextBoxColumn AvgCNoColumn;
        private Button button_exit;
        private Button ClearDataButton;
        private CommunicationManager comm;
        private IContainer components;
        private CSvStats CSS;
        private DataGridViewTextBoxColumn DataPointsColumn;
        private SaveFileDialog ExportCSVsaveFileDialog;
        private Button ExportDataToCSVButton;
        private const int GPS_SVS = 0x21;
        private const int GPS_SVSP1 = 0x22;
        private DataGridViewTextBoxColumn MaxCNoColumn;
        private DataGridViewTextBoxColumn MinCNoColumn;
        private DataGridViewTextBoxColumn RangeColumn;
        private DataGridViewTextBoxColumn RejectedColumn;
        private DataGridViewTextBoxColumn SatelliteIDColumn;
        private DataGridViewTextBoxColumn StdDevColumn;
        private const int TOTALS_AND_AVG_IDX = 0x21;
        private const string WINDOW_TITLE_LABEL = ": Stats";
        public int WinHeight;
        public int WinLeft;
        public int WinTop;
        public int WinWidth;

        public event updateParentEventHandler updateMainWindow;

        public event UpdateWindowEventHandler UpdatePortManager;

        public frmSatelliteStats(CommunicationManager mainComWin)
        {
            this.InitializeComponent();
            _numberOpen++;
            this._persistedWindowName = "Satellite Satistics Window " + _numberOpen.ToString();
            base.MdiParent = clsGlobal.g_objfrmMDIMain;
            this.CommWindow = mainComWin;
            this.CSS = new CSvStats();
            this.CSS.ClearAllData();
            this.CommWindow.DisplayPanelSatelliteStats.Invalidate();
        }

        private void _SatelliteStatsDataGridView_Paint(object sender, PaintEventArgs e)
        {
            if (this.CommWindow.MsgID4Update || this.CSS.ClearDataFlag)
            {
                this.UpdateSatelliteStatistics();
                this.CommWindow.MsgID4Update = false;
                DataGridViewColumn column = new DataGridViewColumn();
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                for (int i = 0; i <= 0x21; i++)
                {
                    if (this._SatelliteStatsDataGridView.RowCount <= 0x21)
                    {
                        this._SatelliteStatsDataGridView.RowTemplate.Height = 15;
                        this._SatelliteStatsDataGridView.Rows.Add();
                    }
                    if ((this.CSS.DataCount[i] == 0) || this.CSS.ClearDataFlag)
                    {
                        this._SatelliteStatsDataGridView["SatelliteIDColumn", i].Value = i.ToString();
                        this._SatelliteStatsDataGridView["AvgCNoColumn", i].Value = "-";
                        this._SatelliteStatsDataGridView["StdDevColumn", i].Value = "-";
                        this._SatelliteStatsDataGridView["MaxCNoColumn", i].Value = "-";
                        this._SatelliteStatsDataGridView["MinCNoColumn", i].Value = "-";
                        this._SatelliteStatsDataGridView["RangeColumn", i].Value = "-";
                        this._SatelliteStatsDataGridView["DataPointsColumn", i].Value = "-";
                        this._SatelliteStatsDataGridView["RejectedColumn", i].Value = "-";
                    }
                    else
                    {
                        this._SatelliteStatsDataGridView["SatelliteIDColumn", i].Value = i.ToString();
                        this._SatelliteStatsDataGridView["AvgCNoColumn", i].Value = this.CSS.GetAvgCNo((uint) i).ToString("F2");
                        this._SatelliteStatsDataGridView["StdDevColumn", i].Value = this.CSS.GetStdDev((uint) i).ToString("F2");
                        this._SatelliteStatsDataGridView["MaxCNoColumn", i].Value = this.CSS.GetMaxCNo((uint) i).ToString("F2");
                        this._SatelliteStatsDataGridView["MinCNoColumn", i].Value = this.CSS.GetMinCNo((uint) i).ToString("F2");
                        this._SatelliteStatsDataGridView["RangeColumn", i].Value = this.CSS.GetRange((uint) i).ToString("F2");
                        this._SatelliteStatsDataGridView["DataPointsColumn", i].Value = this.CSS.GetDataCount((uint) i).ToString();
                        this._SatelliteStatsDataGridView["RejectedColumn", i].Value = this.CSS.GetRejected((uint) i).ToString();
                    }
                    if (i == 0x21)
                    {
                        this._SatelliteStatsDataGridView["SatelliteIDColumn", i].Value = "Totals & Avg";
                    }
                    if (i == 0x21)
                    {
                        this._SatelliteStatsDataGridView.Rows.RemoveAt(0);
                    }
                }
                this.CSS.ClearDataFlag = false;
            }
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void ClearDataButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear SV statistics data?", "Clear Data", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.CSS.ClearAllData();
                this.CommWindow.DisplayPanelSatelliteStats.Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ExportDataToCSVButton_Click(object sender, EventArgs e)
        {
            this.ExportCSVsaveFileDialog.CreatePrompt = true;
            this.ExportCSVsaveFileDialog.OverwritePrompt = true;
            this.ExportCSVsaveFileDialog.DefaultExt = "csv";
            this.ExportCSVsaveFileDialog.FileName = "signalStats.csv";
            this.ExportCSVsaveFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            if (this.ExportCSVsaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string versionNum = SiRFLiveVersion.VersionNum;
                StreamWriter writer = new StreamWriter(this.ExportCSVsaveFileDialog.FileName);
                string str2 = string.Format("SV Statistics Data from SiRFLive Version {0}...", versionNum);
                writer.WriteLine(str2);
                writer.WriteLine();
                str2 = string.Format("Satellite ID,Average CNo,Standard Deviation,Maximum CNo,Minimum CNo,Range,Data Points,Rejected", new object[0]);
                writer.WriteLine(str2);
                for (int i = 0; i < 0x21; i++)
                {
                    if (i == 0x20)
                    {
                        str2 = string.Format("Totals & Avg,", new object[0]);
                        writer.Write(str2);
                    }
                    else
                    {
                        str2 = string.Format("{0},", i + 1);
                        writer.Write(str2);
                    }
                    if ((this.CSS.GetDataCount((uint) (i + 1)) != 0) || (this.CSS.GetRejected((uint) (i + 1)) == 0))
                    {
                        str2 = string.Format("{0:F2},", this.CSS.GetAvgCNo((uint) (i + 1)));
                        writer.Write(str2);
                        str2 = string.Format("{0:F2},", this.CSS.GetStdDev((uint) (i + 1)));
                        writer.Write(str2);
                        str2 = string.Format("{0:F2},", this.CSS.GetMaxCNo((uint) (i + 1)));
                        writer.Write(str2);
                        str2 = string.Format("{0:F2},", this.CSS.GetMinCNo((uint) (i + 1)));
                        writer.Write(str2);
                        str2 = string.Format("{0:F2},", this.CSS.GetRange((uint) (i + 1)));
                        writer.Write(str2);
                        str2 = string.Format("{0},", this.CSS.GetDataCount((uint) (i + 1)));
                        writer.Write(str2);
                        str2 = string.Format("{0}", this.CSS.GetRejected((uint) (i + 1)));
                        writer.WriteLine(str2);
                    }
                    else
                    {
                        str2 = string.Format("-,-,-,-,-,-,-", new object[0]);
                        writer.WriteLine(str2);
                    }
                }
                writer.Close();
            }
        }

        private void frmSatelliteStats_Load(object sender, EventArgs e)
        {
            base.Top = this.WinTop;
            base.Left = this.WinLeft;
            if (this.WinWidth != 0)
            {
                base.Width = this.WinWidth;
            }
            if (this.WinHeight != 0)
            {
                base.Height = this.WinHeight;
            }
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmSatelliteStats));
            this._SatelliteStatsDataGridView = new DataGridView();
            this.SatelliteIDColumn = new DataGridViewTextBoxColumn();
            this.AvgCNoColumn = new DataGridViewTextBoxColumn();
            this.StdDevColumn = new DataGridViewTextBoxColumn();
            this.MaxCNoColumn = new DataGridViewTextBoxColumn();
            this.MinCNoColumn = new DataGridViewTextBoxColumn();
            this.RangeColumn = new DataGridViewTextBoxColumn();
            this.DataPointsColumn = new DataGridViewTextBoxColumn();
            this.RejectedColumn = new DataGridViewTextBoxColumn();
            this.ExportDataToCSVButton = new Button();
            this.ClearDataButton = new Button();
            this.ExportCSVsaveFileDialog = new SaveFileDialog();
            this.button_exit = new Button();
            ((ISupportInitialize) this._SatelliteStatsDataGridView).BeginInit();
            base.SuspendLayout();
            this._SatelliteStatsDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            this._SatelliteStatsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._SatelliteStatsDataGridView.Columns.AddRange(new DataGridViewColumn[] { this.SatelliteIDColumn, this.AvgCNoColumn, this.StdDevColumn, this.MaxCNoColumn, this.MinCNoColumn, this.RangeColumn, this.DataPointsColumn, this.RejectedColumn });
            this._SatelliteStatsDataGridView.Location = new Point(12, 0x17);
            this._SatelliteStatsDataGridView.Name = "_SatelliteStatsDataGridView";
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = SystemColors.Control;
            style.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style.ForeColor = SystemColors.WindowText;
            style.SelectionBackColor = SystemColors.Highlight;
            style.SelectionForeColor = SystemColors.HighlightText;
            style.WrapMode = DataGridViewTriState.True;
            this._SatelliteStatsDataGridView.RowHeadersDefaultCellStyle = style;
            this._SatelliteStatsDataGridView.Size = new Size(0x323, 0x219);
            this._SatelliteStatsDataGridView.TabIndex = 0;
            this._SatelliteStatsDataGridView.Paint += new PaintEventHandler(this._SatelliteStatsDataGridView_Paint);
            this.SatelliteIDColumn.HeaderText = "Satellite ID";
            this.SatelliteIDColumn.Name = "SatelliteIDColumn";
            this.SatelliteIDColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.AvgCNoColumn.HeaderText = "Average CNo";
            this.AvgCNoColumn.Name = "AvgCNoColumn";
            this.AvgCNoColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.StdDevColumn.HeaderText = "Standard Deviation";
            this.StdDevColumn.Name = "StdDevColumn";
            this.StdDevColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.MaxCNoColumn.HeaderText = "Maximum CNo";
            this.MaxCNoColumn.Name = "MaxCNoColumn";
            this.MaxCNoColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.MinCNoColumn.HeaderText = "Minimum CNo";
            this.MinCNoColumn.Name = "MinCNoColumn";
            this.MinCNoColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.RangeColumn.HeaderText = "Range";
            this.RangeColumn.Name = "RangeColumn";
            this.RangeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DataPointsColumn.HeaderText = "Data Points";
            this.DataPointsColumn.Name = "DataPointsColumn";
            this.DataPointsColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.RejectedColumn.HeaderText = "Rejected";
            this.RejectedColumn.Name = "RejectedColumn";
            this.RejectedColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.RejectedColumn.Width = 60;
            this.ExportDataToCSVButton.Location = new Point(0x8f, 0x24d);
            this.ExportDataToCSVButton.Name = "ExportDataToCSVButton";
            this.ExportDataToCSVButton.Size = new Size(130, 0x17);
            this.ExportDataToCSVButton.TabIndex = 1;
            this.ExportDataToCSVButton.Text = "Export Data";
            this.ExportDataToCSVButton.UseVisualStyleBackColor = true;
            this.ExportDataToCSVButton.Click += new EventHandler(this.ExportDataToCSVButton_Click);
            this.ClearDataButton.Location = new Point(0x22e, 0x24d);
            this.ClearDataButton.Name = "ClearDataButton";
            this.ClearDataButton.Size = new Size(130, 0x17);
            this.ClearDataButton.TabIndex = 2;
            this.ClearDataButton.Text = "Clear Data";
            this.ClearDataButton.UseVisualStyleBackColor = true;
            this.ClearDataButton.Click += new EventHandler(this.ClearDataButton_Click);
            this.button_exit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_exit.Location = new Point(0x17a, 0x24d);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new Size(0x4b, 0x17);
            this.button_exit.TabIndex = 3;
            this.button_exit.Text = "E&xit";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new EventHandler(this.button_exit_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.button_exit;
            base.ClientSize = new Size(0x33f, 0x278);
            base.Controls.Add(this.button_exit);
            base.Controls.Add(this.ClearDataButton);
            base.Controls.Add(this.ExportDataToCSVButton);
            base.Controls.Add(this._SatelliteStatsDataGridView);
            base.Icon = (Icon) resources.GetObject("$this.Icon");
            base.Name = "frmSatelliteStats";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.Manual;
            this.Text = "Satellite Statistics";
            base.Load += new EventHandler(this.frmSatelliteStats_Load);
            ((ISupportInitialize) this._SatelliteStatsDataGridView).EndInit();
            base.ResumeLayout(false);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.comm != null)
            {
                this.comm.EnableSatelliteStats = false;
            }
            if (this.updateMainWindow != null)
            {
                this.updateMainWindow(base.Name);
            }
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, false);
            }
            base.OnClosed(e);
        }

        public void UpdateSatelliteStatistics()
        {
            for (uint i = 0; i < 0x21; i++)
            {
                if (this.comm.dataGui.PRN_Arr_ID[i] != 0)
                {
                    this.CSS.AddDataPoint(this.comm.dataGui.PRN_Arr_ID[i], (double) this.comm.dataGui.PRN_Arr_CNO[i], this.comm.dataGui.PRN_Arr_State[i]);
                }
            }
            this.CSS.ComputesTotalsAndAverages();
        }

        public CommunicationManager CommWindow
        {
            get
            {
                return this.comm;
            }
            set
            {
                this.comm = value;
                this.comm.DisplayPanelSatelliteStats = this._SatelliteStatsDataGridView;
                this.comm.EnableSatelliteStats = true;
                this.Text = this.comm.sourceDeviceName + ": Stats";
            }
        }

        public string PersistedWindowName
        {
            get
            {
                return this._persistedWindowName;
            }
            set
            {
                this._persistedWindowName = value;
            }
        }

        public class CSvStats
        {
            public double[] AvgCNo = new double[0x22];
            public bool ClearDataFlag;
            public int[] DataCount = new int[0x22];
            private double[] DiffSum = new double[0x22];
            private const int GPS_SVS = 0x21;
            private const int GPS_SVSP1 = 0x22;
            public double[] MaxCNo = new double[0x22];
            public double[] MinCNo = new double[0x22];
            public double[] Range = new double[0x22];
            public int[] Rejected = new int[0x22];
            public double[] SumCNo = new double[0x22];
            public double[] SvStdDev = new double[0x22];
            private const int TOTALS_AND_AVG_IDX = 0x21;
            public double[] Variance = new double[0x22];

            public void AddDataPoint(uint SVID, double CNo, int SVState)
            {
                if (SVID <= 0x21)
                {
                    if (SVState == 0)
                    {
                        this.Rejected[SVID]++;
                    }
                    else
                    {
                        this.DataCount[SVID]++;
                        this.SumCNo[SVID] += CNo;
                        this.AvgCNo[SVID] = this.SumCNo[SVID] / ((double) this.DataCount[SVID]);
                        this.MaxCNo[SVID] = Math.Max(this.MaxCNo[SVID], CNo);
                        this.MinCNo[SVID] = (this.MinCNo[SVID] != 0.0) ? Math.Min(this.MinCNo[SVID], CNo) : CNo;
                        this.Range[SVID] = this.MaxCNo[SVID] - this.MinCNo[SVID];
                        this.DiffSum[SVID] += Math.Pow(CNo - this.AvgCNo[SVID], 2.0);
                        if ((this.DataCount[SVID] - 1) > 0)
                        {
                            this.Variance[SVID] = Math.Sqrt(this.DiffSum[SVID] / ((double) (this.DataCount[SVID] - 1)));
                        }
                        else
                        {
                            this.Variance[SVID] = 0.0;
                        }
                        this.SvStdDev[SVID] = Math.Sqrt(this.Variance[SVID]);
                    }
                }
            }

            public void ClearAllData()
            {
                for (int i = 0; i <= 0x21; i++)
                {
                    this.MaxCNo[i] = 0.0;
                    this.MinCNo[i] = 0.0;
                    this.SumCNo[i] = 0.0;
                    this.AvgCNo[i] = 0.0;
                    this.Range[i] = 0.0;
                    this.Rejected[i] = 0;
                    this.DataCount[i] = 0;
                    this.SvStdDev[i] = 0.0;
                    this.ClearDataFlag = true;
                }
            }

            public void ComputesTotalsAndAverages()
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                double num4 = 0.0;
                double num5 = 100.0;
                double num6 = 0.0;
                double num7 = 0.0;
                double d = 0.0;
                for (int i = 0; i < 0x21; i++)
                {
                    if (this.DataCount[i] != 0)
                    {
                        num2++;
                        num += this.DataCount[i];
                        num3 += this.Rejected[i];
                        num6 += this.AvgCNo[i];
                        if (this.MaxCNo[i] > num4)
                        {
                            num4 = this.MaxCNo[i];
                        }
                        if (this.MinCNo[i] < num5)
                        {
                            num5 = this.MinCNo[i];
                        }
                    }
                }
                this.DataCount[0x21] = num;
                this.MaxCNo[0x21] = num4;
                this.MinCNo[0x21] = num5;
                this.Range[0x21] = num4 - num5;
                this.AvgCNo[0x21] = num6 / ((double) num2);
                this.Rejected[0x21] = num3;
                for (int j = 0; j < 0x21; j++)
                {
                    if (this.DataCount[j] != 0)
                    {
                        num7 += Math.Pow(this.AvgCNo[j] - this.AvgCNo[0x21], 2.0);
                    }
                }
                d = num7 / (num2 - 1.0);
                this.SvStdDev[0x21] = Math.Sqrt(d);
            }

            public double GetAvgCNo(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0.0;
                }
                return this.AvgCNo[SVID];
            }

            public int GetDataCount(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0;
                }
                return this.DataCount[SVID];
            }

            public double GetMaxCNo(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0.0;
                }
                return this.MaxCNo[SVID];
            }

            public double GetMinCNo(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0.0;
                }
                return this.MinCNo[SVID];
            }

            public double GetRange(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0.0;
                }
                return this.Range[SVID];
            }

            public int GetRejected(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0;
                }
                return this.Rejected[SVID];
            }

            public double GetStdDev(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0.0;
                }
                return this.SvStdDev[SVID];
            }

            public double GetSumCNo(uint SVID)
            {
                if (SVID > 0x21)
                {
                    return 0.0;
                }
                return this.SumCNo[SVID];
            }
        }

        public delegate void updateParentEventHandler(string titleString);

        public delegate void UpdateWindowEventHandler(string titleString, int left, int top, int width, int height, bool state);
    }
}

