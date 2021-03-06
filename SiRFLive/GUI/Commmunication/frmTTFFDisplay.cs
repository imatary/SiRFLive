﻿namespace SiRFLive.GUI.Commmunication
{
    using SiRFLive.Analysis;
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.GUI;
    using SiRFLive.Properties;
    using SiRFLive.Utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows.Forms;

    public class frmTTFFDisplay : Form
    {
        private frmCDFPlots _CDFPlotWin;
        private Color _currentCurveColor = Color.Red;
        private List<Color> _curvePlotColorsList = new List<Color>();
        private List<int> _curvePlotIndicesList = new List<int>();
        private List<string> _curvePlotTitlesList = new List<string>();
        private int _displayBuffer = 500;
        private Stats _hrzErrorStats = new Stats();
        private bool _isAidingSession;
        private List<string> _messageFilterQNames = new List<string>();
        private int _startTTFFIndex = 2;
        private Stats _ttffAidedStats = new Stats();
        private Stats _ttffFirstNavStats = new Stats();
        private ObjectInterface _ttffGuiCtrl = new ObjectInterface();
        private Stats _ttffResetStats = new Stats();
        private Stats _ttffSiRFLiveStats = new Stats();
        private DataGridView _ttffViewGrid;
        private BackgroundWorker _updateTTFFBG;
        private Stats _vrtErrorStats = new Stats();
        private const string AIDING_FLAGS = "Aiding Flags";
        private CommunicationManager comm;
        private IContainer components;
        private ColorDialog CurveColorDialog;
        private const string FREQ_UNC = "Freq Unc. (ppm)";
        private const string FRQ_ERROR = "Freq Error (ppm)";
        private const string HORIZONTAL_ACC_LABEL = "Horz Acc. (m)";
        private StatusStrip statusStrip1;
        private const string TIME_ERROR = "Time Error (ms)";
        private const string TIME_UNC = "Time Unc. (ms)";
        private const string TTFF_SINCE_AIDED_RECEIVED_LABEL = "TTFF-Aiding (s)";
        private const string TTFF_SINCE_FIRST_NAV_LABEL = "TTFF-First Nav (s)";
        private const string TTFF_SINCE_RESET_LABEL = "TTFF-Reset (s)";
        private const string TTFF_SiRFLive_LABEL = "TTFF-SiRFLive (s)";
        private ToolStripButton ttffDisplayToolbarAddCurveBtn;
        private ToolStripTextBox ttffDisplayToolbarBufferTxtBox;
        private ToolStripButton ttffDisplayToolbarColumnDescriptionBtn;
        private ToolStripButton ttffDisplayToolbarCurveColorBtn;
        private ToolStripTextBox ttffDisplayToolbarCurveColorTxtBox;
        private ToolStripTextBox ttffDisplayToolbarCurveTitleTxtBox;
        private ToolStripComboBox ttffDisplayToolbarCurveTypeComboBox;
        private ToolStripButton ttffDisplayToolbarDeleteBtn;
        private ToolStripButton ttffDisplayToolbarPlotCDFBtn;
        private ToolStripButton ttffDisplayToolbarSetRefPositionBtn;
        private ToolStrip ttffDisplayToolStrip1;
        private ToolStripStatusLabel ttffDisplayToolStripStatusLabel;
        private const string VERTICAL_ACC_LABEL = "Vert Acc. (m)";
        private const string WINDOW_TITLE_LABEL = ": TTFF/Nav Accuracy";
        public int WinHeight;
        public int WinLeft;
        public int WinTop;
        public int WinWidth;

        public event updateParentEventHandler updateMainWindow;

        public event UpdateWindowEventHandler UpdatePortManager;

        public frmTTFFDisplay(CommunicationManager mainComWin)
        {
            this.InitializeComponent();
            base.MdiParent = clsGlobal.g_objfrmMDIMain;
            this.CommWindow = mainComWin;
        }

        private void AddCurveToPlotButton_Click(object sender, EventArgs e)
        {
            if (this._CDFPlotWin != null)
            {
                Stats dataClass = this.getPlotData(this.ttffDisplayToolbarCurveTypeComboBox.SelectedIndex);
                string label = this.getPlotTitle(this.ttffDisplayToolbarCurveTypeComboBox.SelectedIndex);
                if (dataClass != null)
                {
                    this._CDFPlotWin.AddCurveToPlot(dataClass, dataClass.Samples, label, this._currentCurveColor);
                    this._curvePlotIndicesList.Add(this.ttffDisplayToolbarCurveTypeComboBox.SelectedIndex);
                    this._curvePlotTitlesList.Add(label);
                    this._curvePlotColorsList.Add(this._currentCurveColor);
                    this.ttffDisplayToolbarCurveTitleTxtBox.Text = label;
                }
            }
        }

        public void ClearTTFFData()
        {
            EventHandler method = null;
            if (base.InvokeRequired)
            {
                if (method == null)
                {
                    method = delegate {
                        this.localClearTTFFData();
                    };
                }
                base.BeginInvoke(method);
            }
            else
            {
                this.localClearTTFFData();
            }
        }

        private string convertByteToBinaryString(byte num)
        {
            string str = Convert.ToString(num, 2);
            for (int i = str.Length; i < 8; i++)
            {
                str = "0" + str;
            }
            return str;
        }

        private void CurveColorButton_Click(object sender, EventArgs e)
        {
            if (this.CurveColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.ttffDisplayToolbarCurveColorTxtBox.Text = this.CurveColorDialog.Color.ToString();
                this._currentCurveColor = this.CurveColorDialog.Color;
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

        private void frmTTFFAndNavAccuracyClearBtn_Click(object sender, EventArgs e)
        {
            this.ClearTTFFData();
        }

        private void frmTTFFAndNavAccuracyColumnDescriptionBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("\n{0}:\t\tSession number\n{1}:\tCommanded reset type\n{2}:\tTTFF since reset received\n{3}:\tTTFF since all aidings received and QoS met(applicable only in aided reset)\n{4}:\tTTFF since first Nav received regardless of QoS(applicable only in aided reset)\n{5}:\tHorizontal error in meter(applicable only when correct reference location set)\n{6}:\tVertical error in meter(applicable only when correct reference location set)\n{7}:\tTime uncertainty(applicable only in aided reset)\n{8}:\tTime error(applicable only in aided reset)\n{9}:\tFrequency uncertainty(applicable only in aided reset)\n{10}:\tFrequency error(applicable only in aided reset)\n{11}:\tBit 1  (0x01):       Precise Time\n\t\tBit 2  (0x02):       Coarse Time\n\t\tBit 3  (0x04):       External Position Aiding received and Used\n\t\tBit 4  (0x08):       External Position Aiding received but Not Used\n\t\tBit 5  (0x10):       External Time Aiding received and Used\n\t\tBit 6  (0x20):       External Time Aiding received but Not Used\n\t\tBit 7  (0x40):       External Frequency Aiding received and Used\n\t\tBit 8  (0x80):       External Frequency Aiding received but Not Used\n\n\tNote: -9999 indicates data not available", new object[] { "Reset #", "Reset Type", "TTFF-Reset (s)", "TTFF-Aiding (s)", "TTFF-First Nav (s)", "Horz Acc. (m)", "Vert Acc. (m)", "Time Unc. (ms)", "Time Error (ms)", "Freq Unc. (ppm)", "Freq Error (ppm)", "Aiding Flags" }), "TTFF Column Meanings", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void frmTTFFDisplay_Load(object sender, EventArgs e)
        {
            this.ttffDisplayToolbarBufferTxtBox.Text = this._displayBuffer.ToString();
            this.loadDisplayColumnHeader();
            this.ttffDisplayToolbarCurveColorTxtBox.Text = this._currentCurveColor.ToString();
            this.ttffDisplayToolbarAddCurveBtn.Enabled = false;
            this.ttffDisplayToolbarCurveTypeComboBox.SelectedIndex = 0;
            this.ttffDisplayToolbarBufferTxtBox.Visible = false;
            if (this.comm != null)
            {
                if (this.comm.comPort.IsOpen || this.comm.CMC.HostAppClient.IsOpen())
                {
                    this.SetTTFFMsgIndication();
                }
                else
                {
                    this.SetReferenceLocation();
                }
            }
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

        private void frmTTFFDisplay_LocationChanged(object sender, EventArgs e)
        {
            this.WinTop = base.Top;
            this.WinLeft = base.Left;
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, true);
            }
        }

        private void frmTTFFDisplay_Resize(object sender, EventArgs e)
        {
            this.WinWidth = base.Width;
            this.WinHeight = base.Height;
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, true);
            }
        }

        private void frmTTFFDisplayBufferSizeTxtBox_TextChanged(object sender, EventArgs e)
        {
            int num = 0;
            try
            {
                num = Convert.ToInt32(this.ttffDisplayToolbarBufferTxtBox.Text);
                if (num > 0x3e8)
                {
                    MessageBox.Show("Max Buffer is 1000", "Information!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    num = 0x3e8;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            this._displayBuffer = num;
        }

        private Stats getPlotData(int idx)
        {
            switch (idx)
            {
                case 0:
                    return this._ttffResetStats;

                case 1:
                    return this._ttffAidedStats;

                case 2:
                    return this._ttffFirstNavStats;

                case 3:
                    return this._hrzErrorStats;

                case 4:
                    return this._vrtErrorStats;

                case 5:
                    return this._ttffSiRFLiveStats;
            }
            return null;
        }

        private string getPlotTitle(int idx)
        {
            string text = this.ttffDisplayToolbarCurveTitleTxtBox.Text;
            if (text == string.Empty)
            {
                text = this.ttffDisplayToolbarCurveTypeComboBox.Text;
            }
            switch (idx)
            {
                case 0:
                    if (!text.Contains("_TTFF_Reset(s)"))
                    {
                        text = text + "_TTFF_Reset(s)";
                    }
                    return text;

                case 1:
                    if (!text.Contains("_TTFF_AidingRecvd(s)"))
                    {
                        text = text + "_TTFF_AidingRecvd(s)";
                    }
                    return text;

                case 2:
                    if (!text.Contains("_TTFF_1stNav(s)"))
                    {
                        text = text + "_TTFF_1stNav(s)";
                    }
                    return text;

                case 3:
                    if (!text.Contains("_Horz_Error(m)"))
                    {
                        text = text + "_Horz_Error(m)";
                    }
                    return text;

                case 4:
                    if (!text.Contains("_Vert_Error(m)"))
                    {
                        text = text + "_Vert_Error(m)";
                    }
                    return text;

                case 5:
                    if (!text.Contains("_TTFF_SiRFLive(s)"))
                    {
                        text = text + "_TTFF_SiRFLive(s)";
                    }
                    return text;
            }
            return text;
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            DataGridViewCellStyle style2 = new DataGridViewCellStyle();
            DataGridViewCellStyle style3 = new DataGridViewCellStyle();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmTTFFDisplay));
            this._ttffViewGrid = new DataGridView();
            this.CurveColorDialog = new ColorDialog();
            this._updateTTFFBG = new BackgroundWorker();
            this.statusStrip1 = new StatusStrip();
            this.ttffDisplayToolStripStatusLabel = new ToolStripStatusLabel();
            this.ttffDisplayToolStrip1 = new ToolStrip();
            this.ttffDisplayToolbarPlotCDFBtn = new ToolStripButton();
            this.ttffDisplayToolbarCurveTitleTxtBox = new ToolStripTextBox();
            this.ttffDisplayToolbarAddCurveBtn = new ToolStripButton();
            this.ttffDisplayToolbarCurveTypeComboBox = new ToolStripComboBox();
            this.ttffDisplayToolbarCurveColorBtn = new ToolStripButton();
            this.ttffDisplayToolbarCurveColorTxtBox = new ToolStripTextBox();
            this.ttffDisplayToolbarSetRefPositionBtn = new ToolStripButton();
            this.ttffDisplayToolbarDeleteBtn = new ToolStripButton();
            this.ttffDisplayToolbarColumnDescriptionBtn = new ToolStripButton();
            this.ttffDisplayToolbarBufferTxtBox = new ToolStripTextBox();
            ((ISupportInitialize) this._ttffViewGrid).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.ttffDisplayToolStrip1.SuspendLayout();
            base.SuspendLayout();
            this._ttffViewGrid.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = SystemColors.Control;
            style.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style.ForeColor = SystemColors.WindowText;
            style.SelectionBackColor = SystemColors.Highlight;
            style.SelectionForeColor = SystemColors.HighlightText;
            style.WrapMode = DataGridViewTriState.True;
            this._ttffViewGrid.ColumnHeadersDefaultCellStyle = style;
            this._ttffViewGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            style2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style2.BackColor = SystemColors.Window;
            style2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style2.ForeColor = SystemColors.ControlText;
            style2.SelectionBackColor = SystemColors.Highlight;
            style2.SelectionForeColor = SystemColors.HighlightText;
            style2.WrapMode = DataGridViewTriState.False;
            this._ttffViewGrid.DefaultCellStyle = style2;
            this._ttffViewGrid.Location = new Point(12, 0x1c);
            this._ttffViewGrid.Name = "_ttffViewGrid";
            this._ttffViewGrid.ReadOnly = true;
            style3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style3.BackColor = SystemColors.Control;
            style3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style3.ForeColor = SystemColors.WindowText;
            style3.SelectionBackColor = SystemColors.Highlight;
            style3.SelectionForeColor = SystemColors.HighlightText;
            style3.WrapMode = DataGridViewTriState.True;
            this._ttffViewGrid.RowHeadersDefaultCellStyle = style3;
            this._ttffViewGrid.Size = new Size(0x2c0, 0x15c);
            this._ttffViewGrid.TabIndex = 8;
            this.CurveColorDialog.AnyColor = true;
            this._updateTTFFBG.WorkerReportsProgress = true;
            this._updateTTFFBG.WorkerSupportsCancellation = true;
            this._updateTTFFBG.DoWork += new DoWorkEventHandler(this.updateTTFFBG_DoWork);
            this._updateTTFFBG.ProgressChanged += new ProgressChangedEventHandler(this.updateTTFFBG_ProgressChanged);
            this.statusStrip1.Items.AddRange(new ToolStripItem[] { this.ttffDisplayToolStripStatusLabel });
            this.statusStrip1.Location = new Point(0, 0x17a);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new Size(0x2d8, 0x16);
            this.statusStrip1.TabIndex = 0x1d;
            this.statusStrip1.Text = "Status";
            this.ttffDisplayToolStripStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.ttffDisplayToolStripStatusLabel.Name = "ttffDisplayToolStripStatusLabel";
            this.ttffDisplayToolStripStatusLabel.Size = new Size(0x26, 0x11);
            this.ttffDisplayToolStripStatusLabel.Text = "Status";
            this.ttffDisplayToolStrip1.Items.AddRange(new ToolStripItem[] { this.ttffDisplayToolbarPlotCDFBtn, this.ttffDisplayToolbarCurveTitleTxtBox, this.ttffDisplayToolbarAddCurveBtn, this.ttffDisplayToolbarCurveTypeComboBox, this.ttffDisplayToolbarCurveColorBtn, this.ttffDisplayToolbarCurveColorTxtBox, this.ttffDisplayToolbarSetRefPositionBtn, this.ttffDisplayToolbarDeleteBtn, this.ttffDisplayToolbarColumnDescriptionBtn, this.ttffDisplayToolbarBufferTxtBox });
            this.ttffDisplayToolStrip1.Location = new Point(0, 0);
            this.ttffDisplayToolStrip1.Name = "ttffDisplayToolStrip1";
            this.ttffDisplayToolStrip1.Size = new Size(0x2d8, 0x19);
            this.ttffDisplayToolStrip1.TabIndex = 0x1f;
            this.ttffDisplayToolStrip1.Text = "TTFF Display Toolbar";
            this.ttffDisplayToolbarPlotCDFBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ttffDisplayToolbarPlotCDFBtn.Image = Resources.ttff;
            this.ttffDisplayToolbarPlotCDFBtn.ImageTransparentColor = Color.Magenta;
            this.ttffDisplayToolbarPlotCDFBtn.Name = "ttffDisplayToolbarPlotCDFBtn";
            this.ttffDisplayToolbarPlotCDFBtn.Size = new Size(0x17, 0x16);
            this.ttffDisplayToolbarPlotCDFBtn.Text = "Plot CDF";
            this.ttffDisplayToolbarPlotCDFBtn.Click += new EventHandler(this.PlotCDFCurvesButton_Click);
            this.ttffDisplayToolbarCurveTitleTxtBox.Name = "ttffDisplayToolbarCurveTitleTxtBox";
            this.ttffDisplayToolbarCurveTitleTxtBox.Size = new Size(150, 0x19);
            this.ttffDisplayToolbarCurveTitleTxtBox.Text = "Curve Label";
            this.ttffDisplayToolbarCurveTitleTxtBox.ToolTipText = "Curve Label";
            this.ttffDisplayToolbarAddCurveBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ttffDisplayToolbarAddCurveBtn.Image = Resources.addCurve;
            this.ttffDisplayToolbarAddCurveBtn.ImageTransparentColor = Color.Magenta;
            this.ttffDisplayToolbarAddCurveBtn.Name = "ttffDisplayToolbarAddCurveBtn";
            this.ttffDisplayToolbarAddCurveBtn.Size = new Size(0x17, 0x16);
            this.ttffDisplayToolbarAddCurveBtn.Text = "Add Curve";
            this.ttffDisplayToolbarAddCurveBtn.Click += new EventHandler(this.AddCurveToPlotButton_Click);
            this.ttffDisplayToolbarCurveTypeComboBox.Items.AddRange(new object[] { "TTFF Since Reset", "TTFF Since Aiding Received", "TTFF Since First Nav", "Horz. Error", "Vert. Error" });
            this.ttffDisplayToolbarCurveTypeComboBox.Name = "ttffDisplayToolbarCurveTypeComboBox";
            this.ttffDisplayToolbarCurveTypeComboBox.Size = new Size(0x9b, 0x19);
            this.ttffDisplayToolbarCurveTypeComboBox.Text = "Curve Data";
            this.ttffDisplayToolbarCurveTypeComboBox.ToolTipText = "Curve Data";
            this.ttffDisplayToolbarCurveColorBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ttffDisplayToolbarCurveColorBtn.Image = Resources.ColorHS;
            this.ttffDisplayToolbarCurveColorBtn.ImageTransparentColor = Color.Magenta;
            this.ttffDisplayToolbarCurveColorBtn.Name = "ttffDisplayToolbarCurveColorBtn";
            this.ttffDisplayToolbarCurveColorBtn.Size = new Size(0x17, 0x16);
            this.ttffDisplayToolbarCurveColorBtn.Text = "Curve Color";
            this.ttffDisplayToolbarCurveColorBtn.Click += new EventHandler(this.CurveColorButton_Click);
            this.ttffDisplayToolbarCurveColorTxtBox.Name = "ttffDisplayToolbarCurveColorTxtBox";
            this.ttffDisplayToolbarCurveColorTxtBox.Size = new Size(100, 0x19);
            this.ttffDisplayToolbarCurveColorTxtBox.Text = "Curve Color";
            this.ttffDisplayToolbarCurveColorTxtBox.ToolTipText = "Curve Color";
            this.ttffDisplayToolbarSetRefPositionBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ttffDisplayToolbarSetRefPositionBtn.Image = Resources.RefLocation;
            this.ttffDisplayToolbarSetRefPositionBtn.ImageTransparentColor = Color.Magenta;
            this.ttffDisplayToolbarSetRefPositionBtn.Name = "ttffDisplayToolbarSetRefPositionBtn";
            this.ttffDisplayToolbarSetRefPositionBtn.Size = new Size(0x17, 0x16);
            this.ttffDisplayToolbarSetRefPositionBtn.Text = "Set Reference Position";
            this.ttffDisplayToolbarSetRefPositionBtn.Click += new EventHandler(this.setRefLocationBtn_Click);
            this.ttffDisplayToolbarDeleteBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ttffDisplayToolbarDeleteBtn.Image = Resources.clearTableHS;
            this.ttffDisplayToolbarDeleteBtn.ImageTransparentColor = Color.Magenta;
            this.ttffDisplayToolbarDeleteBtn.Name = "ttffDisplayToolbarDeleteBtn";
            this.ttffDisplayToolbarDeleteBtn.Size = new Size(0x17, 0x16);
            this.ttffDisplayToolbarDeleteBtn.Text = "Clear";
            this.ttffDisplayToolbarDeleteBtn.Click += new EventHandler(this.frmTTFFAndNavAccuracyClearBtn_Click);
            this.ttffDisplayToolbarColumnDescriptionBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ttffDisplayToolbarColumnDescriptionBtn.Image = Resources.ColumnDescription;
            this.ttffDisplayToolbarColumnDescriptionBtn.ImageTransparentColor = Color.Magenta;
            this.ttffDisplayToolbarColumnDescriptionBtn.Name = "ttffDisplayToolbarColumnDescriptionBtn";
            this.ttffDisplayToolbarColumnDescriptionBtn.Size = new Size(0x17, 0x16);
            this.ttffDisplayToolbarColumnDescriptionBtn.Text = "Column Description";
            this.ttffDisplayToolbarColumnDescriptionBtn.Click += new EventHandler(this.frmTTFFAndNavAccuracyColumnDescriptionBtn_Click);
            this.ttffDisplayToolbarBufferTxtBox.Name = "ttffDisplayToolbarBufferTxtBox";
            this.ttffDisplayToolbarBufferTxtBox.Size = new Size(100, 0x19);
            this.ttffDisplayToolbarBufferTxtBox.Text = "500";
            this.ttffDisplayToolbarBufferTxtBox.Visible = false;
            this.ttffDisplayToolbarBufferTxtBox.TextChanged += new EventHandler(this.frmTTFFDisplayBufferSizeTxtBox_TextChanged);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = SystemColors.Control;
            base.ClientSize = new Size(0x2d8, 400);
            base.Controls.Add(this.ttffDisplayToolStrip1);
            base.Controls.Add(this.statusStrip1);
            base.Controls.Add(this._ttffViewGrid);
            base.Icon = (Icon) resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            this.MinimumSize = new Size(0xc2, 120);
            base.Name = "frmTTFFDisplay";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.Manual;
            this.Text = "TTFF/Nav Accuracy";
            base.Load += new EventHandler(this.frmTTFFDisplay_Load);
            base.Resize += new EventHandler(this.frmTTFFDisplay_Resize);
            base.LocationChanged += new EventHandler(this.frmTTFFDisplay_LocationChanged);
            ((ISupportInitialize) this._ttffViewGrid).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ttffDisplayToolStrip1.ResumeLayout(false);
            this.ttffDisplayToolStrip1.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void loadDisplayColumnHeader()
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column4 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column5 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column6 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column7 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column8 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column9 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column10 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column11 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column12 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column13 = new DataGridViewTextBoxColumn();
            column.HeaderText = "Reset#";
            column.Name = "ResetNumber";
            column.ReadOnly = true;
            column.Width = 0x30;
            column2.HeaderText = "Reset Type";
            column2.Name = "ResetType";
            column2.ReadOnly = true;
            column2.Width = 0x69;
            column3.HeaderText = "TTFF-SiRFLive (s)";
            column3.Name = "TTFFSiRFLive";
            column3.ReadOnly = true;
            column3.Width = 120;
            column4.HeaderText = "TTFF-Reset (s)";
            column4.Name = "TTFFReset";
            column4.ReadOnly = true;
            column4.Width = 0x69;
            column5.HeaderText = "TTFF-Aiding (s)";
            column5.Name = "TTFFAided";
            column5.ReadOnly = true;
            column5.Width = 0x69;
            column6.HeaderText = "TTFF-First Nav (s)";
            column6.Name = "TTFFFirstNav";
            column6.ReadOnly = true;
            column6.Width = 120;
            column7.HeaderText = "Horz Acc. (m)";
            column7.Name = "HrzAcc";
            column7.ReadOnly = true;
            column7.Width = 100;
            column8.HeaderText = "Vert Acc. (m)";
            column8.Name = "VrtAcc";
            column8.ReadOnly = true;
            column8.Width = 100;
            column10.HeaderText = "Time Error (ms)";
            column10.Name = "TimeError";
            column10.ReadOnly = true;
            column10.Width = 100;
            column9.HeaderText = "Time Unc. (ms)";
            column9.Name = "TimeUnc";
            column9.ReadOnly = true;
            column9.Width = 100;
            column12.HeaderText = "Freq Error (ppm)";
            column12.Name = "FreqError";
            column12.ReadOnly = true;
            column12.Width = 100;
            column11.HeaderText = "Freq Unc. (ppm)";
            column11.Name = "FreqUnc";
            column11.ReadOnly = true;
            column11.Width = 100;
            column13.HeaderText = "Aiding Flags";
            column13.Name = "AidingFlags";
            column13.ReadOnly = true;
            column13.Width = 150;
            this._ttffViewGrid.Columns.AddRange(new DataGridViewColumn[] { column, column2, column3, column4, column5, column6, column7, column8, column10, column9, column12, column11, column13 });
            column3.Visible = false;
        }

        private void localClearTTFFData()
        {
            if ((this.comm != null) && (this.comm.MessageProtocol != "NMEA"))
            {
                if (MessageBox.Show("Are you sure you want to clear all data from the TTFF view?", "Clear Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    return;
                }
                if (((this.comm.RxCtrl != null) && (this.comm.RxCtrl.ResetCtrl != null)) && !this.comm.RxCtrl.ResetCtrl.LoopitInprogress)
                {
                    this.comm.RxCtrl.ResetCtrl.ResetCount = 0;
                }
            }
            this._ttffViewGrid.Rows.Clear();
            this._ttffSiRFLiveStats.Clear();
            this._ttffAidedStats.Clear();
            this._ttffResetStats.Clear();
            this._ttffFirstNavStats.Clear();
            this._hrzErrorStats.Clear();
            this._vrtErrorStats.Clear();
            for (int i = this._startTTFFIndex; i < (this._ttffViewGrid.ColumnCount - 7); i++)
            {
                this._ttffViewGrid.Columns[i++].HeaderText = "TTFF-SiRFLive (s)";
                this._ttffViewGrid.Columns[i++].HeaderText = "TTFF-Reset (s)";
                this._ttffViewGrid.Columns[i++].HeaderText = "TTFF-Aiding (s)";
                this._ttffViewGrid.Columns[i++].HeaderText = "TTFF-First Nav (s)";
                this._ttffViewGrid.Columns[i++].HeaderText = "Horz Acc. (m)";
                this._ttffViewGrid.Columns[i++].HeaderText = "Vert Acc. (m)";
            }
        }

        private void localUpdateTTFFDisplay()
        {
            if (this._ttffViewGrid.Rows.Count > this._displayBuffer)
            {
                this._ttffViewGrid.Rows.RemoveAt(0);
            }
            int num = this._ttffViewGrid.Rows.Add();
            int num2 = 0;
            string str = string.Empty;
            string str2 = string.Format("{0}{1}{2}{3}{4:F1}{5}{6:F1}{7}{8:F1}{9}{10:F1}{11}{12:F2}{13}{14:F1}{15}{16}{17}{18}{19}{20}{21}{22}", new object[] { 
                this.comm.RxCtrl.ResetCtrl.ResetCount, clsGlobal.SiRFLiveDelimeter, this.comm.RxCtrl.ResetCtrl.DisplayResetType, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.TTFFSiRFLive, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.TTFFReset, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.TTFFAided, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.TTFFFirstNav, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.FirstFix2DPositionError, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.FirstFixVerticalPositionError, clsGlobal.SiRFLiveDelimeter, 
                this.comm.m_NavData.TimeErr, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.TimeUncer, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.FreqErr, clsGlobal.SiRFLiveDelimeter, this.comm.m_NavData.FreqUncer
             });
            byte num3 = Convert.ToByte(this.comm.m_NavData.AidingFlags);
            string str3 = string.Format("0x{0:X2}", num3);
            string[] strArray = string.Format("{0}{1}{2} ({3})", new object[] { str2, clsGlobal.SiRFLiveDelimeter, this.convertByteToBinaryString(num3), str3 }).Split(new char[] { clsGlobal.SiRFLiveDelimeter });
            for (int i = 0; i < strArray.Length; i++)
            {
                str = strArray[i];
                this._ttffViewGrid.Rows[num].Cells[num2++].Value = str;
                if (str != "-9999")
                {
                    switch (i)
                    {
                        case 2:
                            this._ttffSiRFLiveStats.InsertSample(Convert.ToDouble(str));
                            break;

                        case 3:
                            this._ttffResetStats.InsertSample(Convert.ToDouble(str));
                            break;

                        case 4:
                            this._ttffAidedStats.InsertSample(Convert.ToDouble(str));
                            break;

                        case 5:
                            this._ttffFirstNavStats.InsertSample(Convert.ToDouble(str));
                            break;

                        case 6:
                            this._hrzErrorStats.InsertSample(Convert.ToDouble(str));
                            break;

                        case 7:
                            this._vrtErrorStats.InsertSample(Convert.ToDouble(str));
                            break;
                    }
                }
            }
            if (this._ttffViewGrid.Rows.Count >= 2)
            {
                this._ttffViewGrid.FirstDisplayedScrollingRowIndex = this._ttffViewGrid.Rows.Count - 2;
            }
            else
            {
                this._ttffViewGrid.FirstDisplayedScrollingRowIndex = 0;
            }
            double num5 = this._ttffSiRFLiveStats.Stats_Mean();
            double num6 = this._ttffResetStats.Stats_Mean();
            double num7 = this._ttffAidedStats.Stats_Mean();
            double num8 = this._ttffFirstNavStats.Stats_Mean();
            double num9 = this._hrzErrorStats.Stats_Mean();
            double num10 = this._vrtErrorStats.Stats_Mean();
            for (int j = this._startTTFFIndex; j < (this._ttffViewGrid.ColumnCount - 7); j++)
            {
                this._ttffViewGrid.Columns[j++].HeaderText = string.Format("{0} (avg: {1:F2})", "TTFF-SiRFLive (s)", num5);
                this._ttffViewGrid.Columns[j++].HeaderText = string.Format("{0} (avg: {1:F2})", "TTFF-Reset (s)", num6);
                this._ttffViewGrid.Columns[j++].HeaderText = string.Format("{0} (avg: {1:F2})", "TTFF-Aiding (s)", num7);
                this._ttffViewGrid.Columns[j++].HeaderText = string.Format("{0} (avg: {1:F2})", "TTFF-First Nav (s)", num8);
                this._ttffViewGrid.Columns[j++].HeaderText = string.Format("{0} (avg: {1:F2})", "Horz Acc. (m)", num9);
                this._ttffViewGrid.Columns[j++].HeaderText = string.Format("{0} (avg: {1:F2})", "Vert Acc. (m)", num10);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            this._updateTTFFBG.CancelAsync();
            if (this.comm != null)
            {
                if (this.updateMainWindow != null)
                {
                    this.updateMainWindow(base.Name);
                }
                if (this.UpdatePortManager != null)
                {
                    this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, false);
                }
            }
            if (this._CDFPlotWin != null)
            {
                this._CDFPlotWin.Close();
            }
        }

        private void PlotCDFCurvesButton_Click(object sender, EventArgs e)
        {
            this.ttffDisplayToolbarPlotCDFBtn.Enabled = false;
            this.ttffDisplayToolbarAddCurveBtn.Enabled = true;
            if (this._CDFPlotWin == null)
            {
                this._CDFPlotWin = new frmCDFPlots();
                this._CDFPlotWin.MdiParent = base.MdiParent;
                this._CDFPlotWin.UpdateStatusToParentWin += new frmCDFPlots.UpdateParentEventHandler(this.updatePlotStatus);
                this._CDFPlotWin.UpdateStatsData += new frmCDFPlots.UpdateParentEventHandler(this.RefreshCDFPlot);
            }
            Stats dataClass = this.getPlotData(this.ttffDisplayToolbarCurveTypeComboBox.SelectedIndex);
            string label = this.getPlotTitle(this.ttffDisplayToolbarCurveTypeComboBox.SelectedIndex);
            if (dataClass != null)
            {
                this._CDFPlotWin.SetPlotData(dataClass, dataClass.Samples, "CDF Plot", label, this._currentCurveColor);
                this._curvePlotIndicesList.Add(this.ttffDisplayToolbarCurveTypeComboBox.SelectedIndex);
                this._curvePlotTitlesList.Add(label);
                this._curvePlotColorsList.Add(this._currentCurveColor);
                this._CDFPlotWin.Show();
                this.ttffDisplayToolbarCurveTitleTxtBox.Text = label;
            }
        }

        private void RefreshCDFPlot()
        {
            if ((this._CDFPlotWin != null) && (this._curvePlotIndicesList.Count > 0))
            {
                for (int i = 0; i < this._curvePlotIndicesList.Count; i++)
                {
                    Stats dataClass = this.getPlotData(this._curvePlotIndicesList[i]);
                    if (dataClass != null)
                    {
                        if (i == 0)
                        {
                            this._CDFPlotWin.SetPlotData(dataClass, dataClass.Samples, "CDF Plot", this._curvePlotTitlesList[i], this._curvePlotColorsList[i]);
                            this._CDFPlotWin.RefreshPlot();
                        }
                        else
                        {
                            this._CDFPlotWin.AddCurveToPlot(dataClass, dataClass.Samples, this._curvePlotTitlesList[i], this._curvePlotColorsList[i]);
                        }
                    }
                }
            }
        }

        public void SetReferenceLocation()
        {
            if ((this.CommWindow.RxCtrl != null) && (this.CommWindow.RxCtrl.RxNavData != null))
            {
                this.SetTTFFMsgIndication();
            }
        }

        private void setRefLocationBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.comm.IsSourceDeviceOpen())
                {
                    frmSetReferenceLocation location = new frmSetReferenceLocation();
                    location.CommWindow = this.comm;
                    location.ShowDialog();
                    location.Dispose();
                    this.SetReferenceLocation();
                }
                else
                {
                    MessageBox.Show("Port not open -- action failed", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public void SetTTFFMsgIndication()
        {
            StringBuilder statsMesg;
            if (CommunicationManager.ValidateCommManager(this.comm))
            {
                statsMesg = new StringBuilder();
				base.BeginInvoke((MethodInvoker)delegate
				{
                    statsMesg.Append(string.Format("Ref Location:{0} ({1},{2},{3}) -- Session Type: ", new object[] { this.CommWindow.RxCtrl.RxNavData.RefLocationName, this.CommWindow.RxCtrl.RxNavData.RefLat, this.CommWindow.RxCtrl.RxNavData.RefLon, this.CommWindow.RxCtrl.RxNavData.RefAlt }));
                    if ((this.comm.MessageProtocol == "OSP") && this.comm.AutoReplyCtrl.AutoReplyParams.AutoReply)
                    {
                        if (!this.comm.RxCtrl.ResetCtrl.ResetType.Contains("FACTORY"))
                        {
                            statsMesg.Append("Aided(225,7) -- ");
                            this._isAidingSession = true;
                        }
                        else if (this.comm.RxCtrl.ResetCtrl.IsAidingPerformedOnFactory)
                        {
                            statsMesg.Append("Aided(225,7) -- ");
                            this._isAidingSession = true;
                        }
                        else
                        {
                            statsMesg.Append("(225,6) -- ");
                            this._isAidingSession = false;
                        }
                    }
                    else
                    {
                        if (!this.comm.AutoReplyCtrl.AutoReplyParams.AutoPosReq)
                        {
                            statsMesg.Append("Autonomous ");
                        }
                        else
                        {
                            statsMesg.Append("Aided ");
                            this._isAidingSession = true;
                        }
                        statsMesg.Append("(225,6) -- ");
                    }
                    if (this.comm.m_NavData.ValidatePosition)
                    {
                        statsMesg.Append("Position accuracy computed");
                    }
                    else
                    {
                        statsMesg.Append("Position accuracy not computed");
                    }
                    this.ttffDisplayToolStripStatusLabel.Text = statsMesg.ToString();
                });
            }
        }

        private void updatePlotStatus()
        {
            this.ttffDisplayToolbarPlotCDFBtn.Enabled = true;
            this.ttffDisplayToolbarAddCurveBtn.Enabled = false;
            this._CDFPlotWin = null;
            this._curvePlotIndicesList.Clear();
            this._curvePlotTitlesList.Clear();
            this._curvePlotColorsList.Clear();
        }

        private void updateTTFFBG_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this._updateTTFFBG.CancellationPending)
            {
            }
            e.Cancel = true;
        }

        private void updateTTFFBG_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            EventHandler method = null;
            try
            {
                if (base.WindowState != FormWindowState.Minimized)
                {
                    this.SetTTFFMsgIndication();
                    if (this._ttffViewGrid.InvokeRequired)
                    {
                        if (method == null)
                        {
                            method = delegate {
                                this.localUpdateTTFFDisplay();
                            };
                        }
                        this._ttffViewGrid.BeginInvoke(method);
                    }
                    else
                    {
                        this.localUpdateTTFFDisplay();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("TTFF Window: " + exception.Message);
            }
        }

        public void updateTTFFNow()
        {
            this._updateTTFFBG.ReportProgress(1);
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
                this.Text = this.comm.sourceDeviceName + ": TTFF/Nav Accuracy";
            }
        }

        public delegate void updateParentEventHandler(string titleString);

        public delegate void UpdateWindowEventHandler(string titleString, int left, int top, int width, int height, bool state);
    }
}

