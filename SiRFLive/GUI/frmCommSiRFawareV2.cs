﻿namespace SiRFLive.GUI
{
    using CommonClassLibrary;
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.GUI.DlgsInputMsg;
    using SiRFLive.Properties;
    using SiRFLive.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    public class frmCommSiRFawareV2 : Form
    {
        private uint _cumulateTimeInFullPower;
        private Color _currentCurveColor = Color.Blue;
        private DateTime _currTime = new DateTime();
        private List<Color> _curvePlotColorsList = new List<Color>();
        private List<int> _curvePlotIndicesList = new List<int>();
        private List<string> _curvePlotTitlesList = new List<string>();
        private bool _gotTTFF = true;
        private bool _inAwareMode;
        private DateTime _lastUpdateTime = new DateTime();
        private frmMPMStatsPlots _MPMPlotWin;
        private DateTime _resetTime = new DateTime();
        private List<string> _SiRFAwareScanResultList = new List<string>();
        private DateTime _startTimeInSec;
        private double _totalMPMTime;
        private double _ttff;
        private CommonClass.BinarycheckBox checkBox1;
        private CommonClass.BinarycheckBox checkBox10;
        private CommonClass.BinarycheckBox checkBox11;
        private CommonClass.BinarycheckBox checkBox12;
        private CommonClass.BinarycheckBox checkBox13;
        private CommonClass.BinarycheckBox checkBox14;
        private CommonClass.BinarycheckBox checkBox15;
        private CommonClass.BinarycheckBox checkBox16;
        private CommonClass.BinarycheckBox checkBox17;
        private CommonClass.BinarycheckBox checkBox18;
        private CommonClass.BinarycheckBox checkBox19;
        private CommonClass.BinarycheckBox checkBox2;
        private CommonClass.BinarycheckBox checkBox20;
        private CommonClass.BinarycheckBox checkBox21;
        private CommonClass.BinarycheckBox checkBox22;
        private CommonClass.BinarycheckBox checkBox23;
        private CommonClass.BinarycheckBox checkBox24;
        private CommonClass.BinarycheckBox checkBox25;
        private CommonClass.BinarycheckBox checkBox26;
        private CommonClass.BinarycheckBox checkBox27;
        private CommonClass.BinarycheckBox checkBox28;
        private CommonClass.BinarycheckBox checkBox29;
        private CommonClass.BinarycheckBox checkBox3;
        private CommonClass.BinarycheckBox checkBox30;
        private CommonClass.BinarycheckBox checkBox31;
        private CommonClass.BinarycheckBox checkBox32;
        private CommonClass.BinarycheckBox checkBox4;
        private CommonClass.BinarycheckBox checkBox5;
        private CommonClass.BinarycheckBox checkBox6;
        private CommonClass.BinarycheckBox checkBox7;
        private CommonClass.BinarycheckBox checkBox8;
        private CommonClass.BinarycheckBox checkBox9;
        private CommunicationManager comm;
        private IContainer components;
        private ColorDialog CurveColorDialog;
        public SiRFawareStatsParams DisplayStatsData = new SiRFawareStatsParams();
        public List<SiRFawareStatsParams> DisplayStatsDataList = new List<SiRFawareStatsParams>();
        private ToolTip ephCollectionStatusDesc;
        private System.Windows.Forms.Timer OneSecondTimer;
        private Label sirfawareCollectedEphLabel;
        private Label sirfawareCumulativeTimeInFullPowerLabel;
        private Label sirfawareCurrentTimeLabel;
        private Button sirfawareExitBtn;
        private Button sirfawareGetPositionBtn;
        private CommonClass.BinarycheckBox sirfawareIsNavCheckBox;
        private Label sirfawareLastUpdateTimeLabel;
        private Button sirfawareStartBtn;
        private RichTextBox sirfawareStatusTxtBox1;
        private RichTextBox sirfawareStatusTxtBox2;
        private CommonClass.BinarycheckBox sirfawareSuccessfulALMCollectCheckBox;
        private Label sirfawareSVIDsLabel;
        private ToolStripButton sirfawareToolStripAddPlotBtn;
        private ToolStripTextBox sirfawareToolStripColorTxtBox;
        private ToolStripButton sirfawareToolStripConfigBtn;
        private ToolStripComboBox sirfawareToolStripDataSetBtn;
        private ToolStripButton sirfawareToolStripHelpBtn;
        private ToolStripButton sirfawareToolStripPlotBtn;
        private ToolStripButton sirfawareToolStripPlotColorBtn;
        private ToolStripTextBox sirfawareToolStripPlotTitleTxtBox;
        private Label sirfawareTotalTimeInMPMLabel;
        private Label sirfawareTTFFLabel;
        private Label sirfawareUpdateTypeLabel;
        private SplitContainer splitContainer1;
        public List<CommonClass.BinarycheckBox> SVList = new List<CommonClass.BinarycheckBox>();
        private ToolStrip toolStrip1;
        private const string WINDOW_TITLE_LABEL = ": SiRFaware";
        public int WinHeight;
        public int WinLeft;
        public int WinTop;
        public int WinWidth;

        public event updateParentEventHandler updateMainWindow;

        public event UpdateWindowEventHandler UpdatePortManager;

        public frmCommSiRFawareV2(CommunicationManager mainComWin)
        {
            this.InitializeComponent();
            base.MdiParent = clsGlobal.g_objfrmMDIMain;
            this.CommWindow = mainComWin;
            this.SVList.Add(this.checkBox1);
            this.SVList.Add(this.checkBox2);
            this.SVList.Add(this.checkBox3);
            this.SVList.Add(this.checkBox4);
            this.SVList.Add(this.checkBox5);
            this.SVList.Add(this.checkBox6);
            this.SVList.Add(this.checkBox7);
            this.SVList.Add(this.checkBox8);
            this.SVList.Add(this.checkBox9);
            this.SVList.Add(this.checkBox10);
            this.SVList.Add(this.checkBox11);
            this.SVList.Add(this.checkBox12);
            this.SVList.Add(this.checkBox13);
            this.SVList.Add(this.checkBox14);
            this.SVList.Add(this.checkBox15);
            this.SVList.Add(this.checkBox16);
            this.SVList.Add(this.checkBox17);
            this.SVList.Add(this.checkBox18);
            this.SVList.Add(this.checkBox19);
            this.SVList.Add(this.checkBox20);
            this.SVList.Add(this.checkBox21);
            this.SVList.Add(this.checkBox22);
            this.SVList.Add(this.checkBox23);
            this.SVList.Add(this.checkBox24);
            this.SVList.Add(this.checkBox25);
            this.SVList.Add(this.checkBox26);
            this.SVList.Add(this.checkBox27);
            this.SVList.Add(this.checkBox28);
            this.SVList.Add(this.checkBox29);
            this.SVList.Add(this.checkBox30);
            this.SVList.Add(this.checkBox31);
            this.SVList.Add(this.checkBox32);
        }

        private void cleanupData()
        {
            this.updateSVEphCheckboxColor(-1, Color.Gray);
            this.DisplayStatsDataList.Clear();
            this._curvePlotIndicesList.Clear();
            this._curvePlotColorsList.Clear();
            this._curvePlotTitlesList.Clear();
            this._cumulateTimeInFullPower = 0;
            this._totalMPMTime = 0.0;
            this._inAwareMode = false;
        }

        public void ClearLastUpdatedTime()
        {
            this.sirfawareLastUpdateTimeLabel.Text = "Last Update: ";
        }

        public void ClearSiRFAwareWindowValues()
        {
        }

        public void ClearTTFFTime()
        {
            this.sirfawareTTFFLabel.Text = "TTFF(s): ";
        }

        public void DisableLastUpdate()
        {
            DateTime time = new DateTime();
            this._lastUpdateTime = time;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmCommSiRFawareV2_DoubleClick(object sender, EventArgs e)
        {
            base.Height = 0x1af;
            base.Width = 710;
        }

        private void frmCommSiRFawareV2_Load(object sender, EventArgs e)
        {
            this.SirfawareGUIInit();
            if (this._inAwareMode && (this.comm.ProductFamily == CommonClass.ProductType.GSD4e))
            {
                this.sirfawareTTFFLabel.Visible = false;
            }
            else
            {
                this.sirfawareTTFFLabel.Visible = true;
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
            if (this.comm.PortName == clsGlobal.FilePlayBackPortName)
            {
                this.sirfawareCurrentTimeLabel.Visible = false;
                this.sirfawareLastUpdateTimeLabel.Visible = false;
            }
            else
            {
                this.sirfawareCurrentTimeLabel.Visible = true;
                this.sirfawareLastUpdateTimeLabel.Visible = true;
                this.sirfawareTotalTimeInMPMLabel.Visible = true;
                this.OneSecondTimer.Start();
            }
            this.sirfawareToolStripPlotBtn.Enabled = true;
            this.sirfawareToolStripAddPlotBtn.Enabled = false;
            this.sirfawareToolStripPlotTitleTxtBox.Text = "Plot Title";
            this.sirfawareToolStripColorTxtBox.Text = Color.Blue.ToString();
            this.sirfawareToolStripDataSetBtn.Items.AddRange(new object[] { 
                "Time In Full Power", "Initial RTC Time Uncertainty", "Total RTC Correction", "Available Maintenance Power Time", "Temperature(C)", "Temperature(T)", "ALM ID", "Total SVs with BE", "Total SVs with EE", "Total SVs with ALM", "uNav Correction", "Mean Code Phase Correction", "Pseudo Range Std", "Mean Doppler residual", "Delta Ranges Std", "Bit Synch achieved", 
                "Frame Synch achieved", "Total Time Correction", "SV mask before Eph collection", "SV mask after Eph collection", "Successful Nav", "Successful ALM collection", "Cumulate Time in Full Power", "Update Type"
             });
            this.sirfawareToolStripDataSetBtn.SelectedIndex = 0;
        }

        private void frmCommSiRFawareV2_LocationChanged(object sender, EventArgs e)
        {
            this.WinTop = base.Top;
            this.WinLeft = base.Left;
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, true);
            }
        }

        private void frmCommSiRFawareV2_ResizeEnd(object sender, EventArgs e)
        {
            this.WinWidth = base.Width;
            this.WinHeight = base.Height;
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, true);
            }
        }

        private void frmSiRFawareDisplayQueueHandler(object sender, DoWorkEventArgs myQContent)
        {
            if (this.comm.MessageProtocol == "OSP")
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
                    MessageQData argument = (MessageQData) myQContent.Argument;
                    if (argument.MessageText != string.Empty)
                    {
                        try
                        {
                            Hashtable siRFAwareScanResHash = this.comm.m_Protocols.ConvertRawToHash(HelperFunctions.HexToByte(argument.MessageText), "OSP");
                            this.UpdateSiRFawareGUI(siRFAwareScanResHash);
                        }
                        catch (Exception exception)
                        {
                            string msg = string.Format("### SiRFaware GUI handler error: {0}\n{1} ", exception.Message, argument.MessageText);
                            this.comm.WriteApp(msg);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    string str2 = string.Format("### SiRFaware GUI handler error: {0}", exception2.Message);
                    this.comm.WriteApp(str2);
                }
            }
        }

        private void frmSiRFawareUpdateGUI()
        {
			base.BeginInvoke((MethodInvoker)delegate
			{
                this.UpdateLastUpdatedTime();
                this.UpdateLastUpdatedTimeGUI();
                this.sirfawareUpdateTypeLabel.Text = string.Format("Last Update Type: {0}", (SiRFawareStatsType) this.DisplayStatsData.UpdateType);
                int num = 0;
                if ((this.DisplayStatsData.UpdateType == 3) || (this.DisplayStatsData.UpdateType == 2))
                {
                    if (this.DisplayStatsData.IsNav == 1)
                    {
                        this.sirfawareIsNavCheckBox.BackgroundBrush = new SolidBrush(Color.Green);
                        this.sirfawareIsNavCheckBox.Checked = !this.sirfawareIsNavCheckBox.Checked;
                    }
                    else
                    {
                        this.sirfawareIsNavCheckBox.BackgroundBrush = new SolidBrush(Color.Red);
                        this.sirfawareIsNavCheckBox.Checked = !this.sirfawareIsNavCheckBox.Checked;
                    }
                    if (this.DisplayStatsData.UpdateType == 3)
                    {
                        if (this.DisplayStatsData.IsSuccessAlmCollection == 1)
                        {
                            this.sirfawareSuccessfulALMCollectCheckBox.BackgroundBrush = new SolidBrush(Color.Green);
                            this.sirfawareSuccessfulALMCollectCheckBox.Checked = !this.sirfawareSuccessfulALMCollectCheckBox.Checked;
                        }
                        else
                        {
                            this.sirfawareSuccessfulALMCollectCheckBox.BackgroundBrush = new SolidBrush(Color.Red);
                            this.sirfawareSuccessfulALMCollectCheckBox.Checked = !this.sirfawareSuccessfulALMCollectCheckBox.Checked;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 0x20; j++)
                        {
                            int num3 = ((int) (this.DisplayStatsData.SVBeforeEphCollection >> j)) & 1;
                            int num4 = ((int) (this.DisplayStatsData.SVAfterEphCollection >> j)) & 1;
                            if (num3 != num4)
                            {
                                if (num4 == 0)
                                {
                                    num |= num4 << j;
                                    this.SVList[j].BackgroundBrush = new SolidBrush(Color.Green);
                                }
                                else
                                {
                                    this.SVList[j].BackgroundBrush = new SolidBrush(Color.Yellow);
                                }
                            }
                            else if (num4 == 1)
                            {
                                this.SVList[j].BackgroundBrush = new SolidBrush(Color.Red);
                            }
                            else
                            {
                                this.SVList[j].BackgroundBrush = new SolidBrush(Color.Gray);
                            }
                            this.SVList[j].Checked = !this.SVList[j].Checked;
                        }
                    }
                }
                string[] strArray = this.comm.RxCtrl.FormatMPMStatsMessage(this.DisplayStatsData);
                this.sirfawareStatusTxtBox1.Text = strArray[0];
                this.sirfawareStatusTxtBox2.Text = strArray[1];
                this.sirfawareCumulativeTimeInFullPowerLabel.Text = string.Format("Cumulate Time in Full Power(s): {0}", this._cumulateTimeInFullPower);
            });
        }

        private void getPlotData(int idx, string myTitle, Color myColor)
        {
            List<double> list = new List<double>();
            List<double> list2 = new List<double>();
            switch (idx)
            {
                case 0:
                    foreach (SiRFawareStatsParams @params in this.DisplayStatsDataList)
                    {
                        if (@params.isValid_TimeSpentInFullPowerSec)
                        {
                            list.Add((double) @params.TOW);
                            list2.Add((double) @params.TimeSpentInFullPowerSec);
                        }
                    }
                    break;

                case 1:
                    foreach (SiRFawareStatsParams params2 in this.DisplayStatsDataList)
                    {
                        if (params2.isValid_RTCWakeupUncUs)
                        {
                            list.Add((double) params2.TOW);
                            list2.Add((double) params2.RTCWakeupUncUs);
                        }
                    }
                    break;

                case 2:
                    foreach (SiRFawareStatsParams params3 in this.DisplayStatsDataList)
                    {
                        if (params3.isValid_RTCCorrectionPerform)
                        {
                            list.Add((double) params3.TOW);
                            list2.Add((double) params3.RTCCorrectionPerform);
                        }
                    }
                    break;

                case 3:
                    foreach (SiRFawareStatsParams params4 in this.DisplayStatsDataList)
                    {
                        if (params4.isValid_UnusedTokenLeft)
                        {
                            list.Add((double) params4.TOW);
                            list2.Add((double) params4.UnusedTokenLeft);
                        }
                    }
                    break;

                case 4:
                    foreach (SiRFawareStatsParams params5 in this.DisplayStatsDataList)
                    {
                        if (params5.isValid_TempRecord)
                        {
                            list.Add((double) params5.TOW);
                            list2.Add(params5.TempRecordC);
                        }
                    }
                    break;

                case 5:
                    foreach (SiRFawareStatsParams params6 in this.DisplayStatsDataList)
                    {
                        if (params6.isValid_TempRecord)
                        {
                            list.Add((double) params6.TOW);
                            list2.Add((double) params6.TempRecordT);
                        }
                    }
                    break;

                case 6:
                    foreach (SiRFawareStatsParams params7 in this.DisplayStatsDataList)
                    {
                        if (params7.isValid_AlmID)
                        {
                            list.Add((double) params7.TOW);
                            list2.Add((double) params7.AlmID);
                        }
                    }
                    break;

                case 7:
                    foreach (SiRFawareStatsParams params8 in this.DisplayStatsDataList)
                    {
                        if (params8.isValid_TotalSVMeasureWithBE)
                        {
                            list.Add((double) params8.TOW);
                            list2.Add((double) params8.TotalSVMeasureWithBE);
                        }
                    }
                    break;

                case 8:
                    foreach (SiRFawareStatsParams params9 in this.DisplayStatsDataList)
                    {
                        if (params9.isValid_TotalSVMeasureWithEE)
                        {
                            list.Add((double) params9.TOW);
                            list2.Add((double) params9.TotalSVMeasureWithEE);
                        }
                    }
                    break;

                case 9:
                    foreach (SiRFawareStatsParams params10 in this.DisplayStatsDataList)
                    {
                        if (params10.isValid_TotalSVMeasureWithAlm)
                        {
                            list.Add((double) params10.TOW);
                            list2.Add((double) params10.TotalSVMeasureWithAlm);
                        }
                    }
                    break;

                case 10:
                    foreach (SiRFawareStatsParams params11 in this.DisplayStatsDataList)
                    {
                        if (params11.isValid_uNavTimeCorrection)
                        {
                            list.Add((double) params11.TOW);
                            list2.Add((double) params11.uNavTimeCorrection);
                        }
                    }
                    break;

                case 11:
                    foreach (SiRFawareStatsParams params12 in this.DisplayStatsDataList)
                    {
                        if (params12.isValid_MeanCodePhaseCorrection)
                        {
                            list.Add((double) params12.TOW);
                            list2.Add((double) params12.MeanCodePhaseCorrection);
                        }
                    }
                    break;

                case 12:
                    foreach (SiRFawareStatsParams params13 in this.DisplayStatsDataList)
                    {
                        if (params13.isValid_StdPseudoRanges)
                        {
                            list.Add((double) params13.TOW);
                            list2.Add((double) params13.StdPseudoRanges);
                        }
                    }
                    break;

                case 13:
                    foreach (SiRFawareStatsParams params14 in this.DisplayStatsDataList)
                    {
                        if (params14.isValid_MeanDopplerResidual)
                        {
                            list.Add((double) params14.TOW);
                            list2.Add((double) params14.MeanDopplerResidual);
                        }
                    }
                    break;

                case 14:
                    foreach (SiRFawareStatsParams params15 in this.DisplayStatsDataList)
                    {
                        if (params15.isValid_StdDeltaRanges)
                        {
                            list.Add((double) params15.TOW);
                            list2.Add((double) params15.StdDeltaRanges);
                        }
                    }
                    break;

                case 15:
                    foreach (SiRFawareStatsParams params16 in this.DisplayStatsDataList)
                    {
                        if (params16.isValid_IsBitSynch)
                        {
                            list.Add((double) params16.TOW);
                            list2.Add((double) params16.IsBitSynch);
                        }
                    }
                    break;

                case 0x10:
                    foreach (SiRFawareStatsParams params17 in this.DisplayStatsDataList)
                    {
                        if (params17.isValid_IsFrameSynch)
                        {
                            list.Add((double) params17.TOW);
                            list2.Add((double) params17.IsFrameSynch);
                        }
                    }
                    break;

                case 0x11:
                    foreach (SiRFawareStatsParams params18 in this.DisplayStatsDataList)
                    {
                        if (params18.isValid_TotalTimeCorrection)
                        {
                            list.Add((double) params18.TOW);
                            list2.Add((double) params18.TotalTimeCorrection);
                        }
                    }
                    break;

                case 0x12:
                    foreach (SiRFawareStatsParams params19 in this.DisplayStatsDataList)
                    {
                        if (params19.isValid_SVBeforeEphCollection)
                        {
                            list.Add((double) params19.TOW);
                            list2.Add((double) params19.SVBeforeEphCollection);
                        }
                    }
                    break;

                case 0x13:
                    foreach (SiRFawareStatsParams params20 in this.DisplayStatsDataList)
                    {
                        if (params20.isValid_SVAfterEphCollection)
                        {
                            list.Add((double) params20.TOW);
                            list2.Add((double) params20.SVAfterEphCollection);
                        }
                    }
                    break;

                case 20:
                    foreach (SiRFawareStatsParams params21 in this.DisplayStatsDataList)
                    {
                        if (params21.isValid_IsNav)
                        {
                            list.Add((double) params21.TOW);
                            list2.Add((double) params21.IsNav);
                        }
                    }
                    break;

                case 0x15:
                    foreach (SiRFawareStatsParams params22 in this.DisplayStatsDataList)
                    {
                        if (params22.isValid_IsSuccessAlmCollection)
                        {
                            list.Add((double) params22.TOW);
                            list2.Add((double) params22.IsSuccessAlmCollection);
                        }
                    }
                    break;

                case 0x16:
                    foreach (SiRFawareStatsParams params23 in this.DisplayStatsDataList)
                    {
                        list.Add((double) params23.TOW);
                        list2.Add((double) params23.CumulateTimeInFullPower);
                    }
                    break;

                case 0x17:
                    foreach (SiRFawareStatsParams params24 in this.DisplayStatsDataList)
                    {
                        list.Add((double) params24.TOW);
                        list2.Add((double) params24.UpdateType);
                    }
                    break;
            }
            this._MPMPlotWin.SetPlotData(list.ToArray(), list2.ToArray(), "SiRFaware Plot(s)", myTitle, myColor);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmCommSiRFawareV2));
            this.toolStrip1 = new ToolStrip();
            this.sirfawareToolStripConfigBtn = new ToolStripButton();
            this.sirfawareToolStripPlotBtn = new ToolStripButton();
            this.sirfawareToolStripPlotTitleTxtBox = new ToolStripTextBox();
            this.sirfawareToolStripAddPlotBtn = new ToolStripButton();
            this.sirfawareToolStripDataSetBtn = new ToolStripComboBox();
            this.sirfawareToolStripPlotColorBtn = new ToolStripButton();
            this.sirfawareToolStripColorTxtBox = new ToolStripTextBox();
            this.sirfawareToolStripHelpBtn = new ToolStripButton();
            this.sirfawareStartBtn = new Button();
            this.sirfawareGetPositionBtn = new Button();
            this.sirfawareExitBtn = new Button();
            this.sirfawareCurrentTimeLabel = new Label();
            this.sirfawareLastUpdateTimeLabel = new Label();
            this.sirfawareUpdateTypeLabel = new Label();
            this.sirfawareCollectedEphLabel = new Label();
            this.sirfawareStatusTxtBox1 = new RichTextBox();
            this.sirfawareTTFFLabel = new Label();
            this.OneSecondTimer = new System.Windows.Forms.Timer(this.components);
            this.sirfawareSuccessfulALMCollectCheckBox = new CommonClass.BinarycheckBox();
            this.sirfawareIsNavCheckBox = new CommonClass.BinarycheckBox();
            this.checkBox32 = new CommonClass.BinarycheckBox();
            this.checkBox31 = new CommonClass.BinarycheckBox();
            this.checkBox30 = new CommonClass.BinarycheckBox();
            this.checkBox29 = new CommonClass.BinarycheckBox();
            this.checkBox28 = new CommonClass.BinarycheckBox();
            this.checkBox27 = new CommonClass.BinarycheckBox();
            this.checkBox26 = new CommonClass.BinarycheckBox();
            this.checkBox25 = new CommonClass.BinarycheckBox();
            this.checkBox24 = new CommonClass.BinarycheckBox();
            this.checkBox23 = new CommonClass.BinarycheckBox();
            this.checkBox22 = new CommonClass.BinarycheckBox();
            this.checkBox21 = new CommonClass.BinarycheckBox();
            this.checkBox20 = new CommonClass.BinarycheckBox();
            this.checkBox19 = new CommonClass.BinarycheckBox();
            this.checkBox18 = new CommonClass.BinarycheckBox();
            this.checkBox17 = new CommonClass.BinarycheckBox();
            this.checkBox16 = new CommonClass.BinarycheckBox();
            this.checkBox15 = new CommonClass.BinarycheckBox();
            this.checkBox14 = new CommonClass.BinarycheckBox();
            this.checkBox13 = new CommonClass.BinarycheckBox();
            this.checkBox12 = new CommonClass.BinarycheckBox();
            this.checkBox11 = new CommonClass.BinarycheckBox();
            this.checkBox10 = new CommonClass.BinarycheckBox();
            this.checkBox9 = new CommonClass.BinarycheckBox();
            this.checkBox8 = new CommonClass.BinarycheckBox();
            this.checkBox7 = new CommonClass.BinarycheckBox();
            this.checkBox6 = new CommonClass.BinarycheckBox();
            this.checkBox5 = new CommonClass.BinarycheckBox();
            this.checkBox4 = new CommonClass.BinarycheckBox();
            this.checkBox3 = new CommonClass.BinarycheckBox();
            this.checkBox2 = new CommonClass.BinarycheckBox();
            this.checkBox1 = new CommonClass.BinarycheckBox();
            this.ephCollectionStatusDesc = new ToolTip(this.components);
            this.splitContainer1 = new SplitContainer();
            this.sirfawareStatusTxtBox2 = new RichTextBox();
            this.sirfawareCumulativeTimeInFullPowerLabel = new Label();
            this.CurveColorDialog = new ColorDialog();
            this.sirfawareSVIDsLabel = new Label();
            this.sirfawareTotalTimeInMPMLabel = new Label();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            base.SuspendLayout();
            this.toolStrip1.Items.AddRange(new ToolStripItem[] { this.sirfawareToolStripConfigBtn, this.sirfawareToolStripPlotBtn, this.sirfawareToolStripPlotTitleTxtBox, this.sirfawareToolStripAddPlotBtn, this.sirfawareToolStripDataSetBtn, this.sirfawareToolStripPlotColorBtn, this.sirfawareToolStripColorTxtBox, this.sirfawareToolStripHelpBtn });
            this.toolStrip1.Location = new Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new Size(0x2be, 0x19);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.sirfawareToolStripConfigBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.sirfawareToolStripConfigBtn.Image = Resources.Config;
            this.sirfawareToolStripConfigBtn.ImageTransparentColor = Color.Magenta;
            this.sirfawareToolStripConfigBtn.Name = "sirfawareToolStripConfigBtn";
            this.sirfawareToolStripConfigBtn.Size = new Size(0x17, 0x16);
            this.sirfawareToolStripConfigBtn.Text = "Configuration";
            this.sirfawareToolStripConfigBtn.Click += new EventHandler(this.sirfawareToolStripConfigBtn_Click);
            this.sirfawareToolStripPlotBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.sirfawareToolStripPlotBtn.Image = Resources.ttff;
            this.sirfawareToolStripPlotBtn.ImageTransparentColor = Color.Magenta;
            this.sirfawareToolStripPlotBtn.Name = "sirfawareToolStripPlotBtn";
            this.sirfawareToolStripPlotBtn.Size = new Size(0x17, 0x16);
            this.sirfawareToolStripPlotBtn.Text = "Plot";
            this.sirfawareToolStripPlotBtn.Click += new EventHandler(this.sirfawareToolStripPlotBtn_Click);
            this.sirfawareToolStripPlotTitleTxtBox.Name = "sirfawareToolStripPlotTitleTxtBox";
            this.sirfawareToolStripPlotTitleTxtBox.Size = new Size(150, 0x19);
            this.sirfawareToolStripPlotTitleTxtBox.Text = "Plot Title";
            this.sirfawareToolStripAddPlotBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.sirfawareToolStripAddPlotBtn.Image = Resources.addCurve;
            this.sirfawareToolStripAddPlotBtn.ImageTransparentColor = Color.Magenta;
            this.sirfawareToolStripAddPlotBtn.Name = "sirfawareToolStripAddPlotBtn";
            this.sirfawareToolStripAddPlotBtn.Size = new Size(0x17, 0x16);
            this.sirfawareToolStripAddPlotBtn.Text = "Add Plot";
            this.sirfawareToolStripAddPlotBtn.Click += new EventHandler(this.sirfawareToolStripAddPlotBtn_Click);
            this.sirfawareToolStripDataSetBtn.Name = "sirfawareToolStripDataSetBtn";
            this.sirfawareToolStripDataSetBtn.Size = new Size(210, 0x19);
            this.sirfawareToolStripDataSetBtn.Text = "Data Set";
            this.sirfawareToolStripDataSetBtn.ToolTipText = "Data Set";
            this.sirfawareToolStripPlotColorBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.sirfawareToolStripPlotColorBtn.Image = Resources.ColorHS;
            this.sirfawareToolStripPlotColorBtn.ImageTransparentColor = Color.Magenta;
            this.sirfawareToolStripPlotColorBtn.Name = "sirfawareToolStripPlotColorBtn";
            this.sirfawareToolStripPlotColorBtn.Size = new Size(0x17, 0x16);
            this.sirfawareToolStripPlotColorBtn.Text = "Plot Color";
            this.sirfawareToolStripPlotColorBtn.Click += new EventHandler(this.sirfawareToolStripPlotColorBtn_Click);
            this.sirfawareToolStripColorTxtBox.Name = "sirfawareToolStripColorTxtBox";
            this.sirfawareToolStripColorTxtBox.Size = new Size(100, 0x19);
            this.sirfawareToolStripColorTxtBox.Text = "Plot Color";
            this.sirfawareToolStripHelpBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.sirfawareToolStripHelpBtn.Image = Resources.Help;
            this.sirfawareToolStripHelpBtn.ImageTransparentColor = Color.Magenta;
            this.sirfawareToolStripHelpBtn.Name = "sirfawareToolStripHelpBtn";
            this.sirfawareToolStripHelpBtn.Size = new Size(0x17, 0x16);
            this.sirfawareToolStripHelpBtn.Text = "Help";
            this.sirfawareToolStripHelpBtn.Click += new EventHandler(this.sirfawareToolStripHelpBtn_Click);
            this.sirfawareStartBtn.Anchor = AnchorStyles.Bottom;
            this.sirfawareStartBtn.Location = new Point(0xda, 0x16e);
            this.sirfawareStartBtn.Name = "sirfawareStartBtn";
            this.sirfawareStartBtn.Size = new Size(0x4b, 0x17);
            this.sirfawareStartBtn.TabIndex = 1;
            this.sirfawareStartBtn.Text = "Start";
            this.sirfawareStartBtn.UseVisualStyleBackColor = true;
            this.sirfawareStartBtn.Click += new EventHandler(this.sirfawareStartBtnt_Click);
            this.sirfawareGetPositionBtn.Anchor = AnchorStyles.Bottom;
            this.sirfawareGetPositionBtn.Location = new Point(0x13a, 0x16e);
            this.sirfawareGetPositionBtn.Name = "sirfawareGetPositionBtn";
            this.sirfawareGetPositionBtn.Size = new Size(0x4b, 0x17);
            this.sirfawareGetPositionBtn.TabIndex = 1;
            this.sirfawareGetPositionBtn.Text = "Get Position";
            this.sirfawareGetPositionBtn.UseVisualStyleBackColor = true;
            this.sirfawareGetPositionBtn.Click += new EventHandler(this.sirfawareGetPositionBtn_Click);
            this.sirfawareExitBtn.Anchor = AnchorStyles.Bottom;
            this.sirfawareExitBtn.Location = new Point(410, 0x16d);
            this.sirfawareExitBtn.Name = "sirfawareExitBtn";
            this.sirfawareExitBtn.Size = new Size(0x4b, 0x17);
            this.sirfawareExitBtn.TabIndex = 1;
            this.sirfawareExitBtn.Text = "Exit";
            this.sirfawareExitBtn.UseVisualStyleBackColor = true;
            this.sirfawareExitBtn.Click += new EventHandler(this.sirfawareExitBtn_Click);
            this.sirfawareCurrentTimeLabel.AutoSize = true;
            this.sirfawareCurrentTimeLabel.Location = new Point(0x8a, 0x2b);
            this.sirfawareCurrentTimeLabel.Name = "sirfawareCurrentTimeLabel";
            this.sirfawareCurrentTimeLabel.Size = new Size(70, 13);
            this.sirfawareCurrentTimeLabel.TabIndex = 0x1f;
            this.sirfawareCurrentTimeLabel.Text = "Current Time:";
            this.sirfawareLastUpdateTimeLabel.AutoSize = true;
            this.sirfawareLastUpdateTimeLabel.Location = new Point(0x147, 0x2b);
            this.sirfawareLastUpdateTimeLabel.Name = "sirfawareLastUpdateTimeLabel";
            this.sirfawareLastUpdateTimeLabel.Size = new Size(0x44, 13);
            this.sirfawareLastUpdateTimeLabel.TabIndex = 0x1f;
            this.sirfawareLastUpdateTimeLabel.Text = "Last Update:";
            this.sirfawareUpdateTypeLabel.AutoSize = true;
            this.sirfawareUpdateTypeLabel.Location = new Point(0x1e5, 0x2b);
            this.sirfawareUpdateTypeLabel.Name = "sirfawareUpdateTypeLabel";
            this.sirfawareUpdateTypeLabel.Size = new Size(0x5f, 13);
            this.sirfawareUpdateTypeLabel.TabIndex = 0x1f;
            this.sirfawareUpdateTypeLabel.Text = "Last Update Type:";
            this.sirfawareCollectedEphLabel.AutoSize = true;
            this.sirfawareCollectedEphLabel.Location = new Point(0x11, 80);
            this.sirfawareCollectedEphLabel.Name = "sirfawareCollectedEphLabel";
            this.sirfawareCollectedEphLabel.Size = new Size(0x72, 13);
            this.sirfawareCollectedEphLabel.TabIndex = 0x20;
            this.sirfawareCollectedEphLabel.Text = "EPH Collection Status:";
            this.sirfawareCollectedEphLabel.MouseHover += new EventHandler(this.sirfawareCollectedEphLabel_MouseHover);
            this.sirfawareStatusTxtBox1.BorderStyle = BorderStyle.None;
            this.sirfawareStatusTxtBox1.Dock = DockStyle.Fill;
            this.sirfawareStatusTxtBox1.Location = new Point(0, 0);
            this.sirfawareStatusTxtBox1.Name = "sirfawareStatusTxtBox1";
            this.sirfawareStatusTxtBox1.ReadOnly = true;
            this.sirfawareStatusTxtBox1.Size = new Size(0x156, 0xc7);
            this.sirfawareStatusTxtBox1.TabIndex = 0x21;
            this.sirfawareStatusTxtBox1.Text = "";
            this.sirfawareTTFFLabel.AutoSize = true;
            this.sirfawareTTFFLabel.Location = new Point(0x11, 0x2b);
            this.sirfawareTTFFLabel.Name = "sirfawareTTFFLabel";
            this.sirfawareTTFFLabel.Size = new Size(0x2f, 13);
            this.sirfawareTTFFLabel.TabIndex = 0x22;
            this.sirfawareTTFFLabel.Text = "TTFF(s):";
            this.OneSecondTimer.Interval = 0x3e8;
            this.OneSecondTimer.Tick += new EventHandler(this.OneSecondTimer_Tick);
            this.sirfawareSuccessfulALMCollectCheckBox.AutoSize = true;
            this.sirfawareSuccessfulALMCollectCheckBox.CheckAlign = ContentAlignment.MiddleRight;
            this.sirfawareSuccessfulALMCollectCheckBox.FlatAppearance.BorderColor = Color.Green;
            this.sirfawareSuccessfulALMCollectCheckBox.FlatAppearance.CheckedBackColor = Color.FromArgb(0, 0xc0, 0);
            this.sirfawareSuccessfulALMCollectCheckBox.FlatStyle = FlatStyle.Flat;
            this.sirfawareSuccessfulALMCollectCheckBox.Location = new Point(0x89, 0x67);
            this.sirfawareSuccessfulALMCollectCheckBox.Name = "sirfawareSuccessfulALMCollectCheckBox";
            this.sirfawareSuccessfulALMCollectCheckBox.Size = new Size(0x9b, 0x11);
            this.sirfawareSuccessfulALMCollectCheckBox.TabIndex = 0x24;
            this.sirfawareSuccessfulALMCollectCheckBox.Text = "Successful ALM Collection?";
            this.sirfawareSuccessfulALMCollectCheckBox.UseVisualStyleBackColor = true;
            this.sirfawareIsNavCheckBox.AutoSize = true;
            this.sirfawareIsNavCheckBox.CheckAlign = ContentAlignment.MiddleRight;
            this.sirfawareIsNavCheckBox.FlatStyle = FlatStyle.Flat;
            this.sirfawareIsNavCheckBox.Location = new Point(0x11, 0x67);
            this.sirfawareIsNavCheckBox.Name = "sirfawareIsNavCheckBox";
            this.sirfawareIsNavCheckBox.Size = new Size(0x68, 0x11);
            this.sirfawareIsNavCheckBox.TabIndex = 0x24;
            this.sirfawareIsNavCheckBox.Text = "Successful Nav?";
            this.sirfawareIsNavCheckBox.UseVisualStyleBackColor = true;
            this.checkBox32.AutoSize = true;
            this.checkBox32.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox32.FlatStyle = FlatStyle.Flat;
            this.checkBox32.Location = new Point(0x299, 0x3e);
            this.checkBox32.Name = "checkBox32";
            this.checkBox32.Size = new Size(0x17, 0x1c);
            this.checkBox32.TabIndex = 0x23;
            this.checkBox32.Text = "32";
            this.checkBox32.UseVisualStyleBackColor = true;
            this.checkBox31.AutoSize = true;
            this.checkBox31.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox31.FlatStyle = FlatStyle.Flat;
            this.checkBox31.Location = new Point(0x288, 0x3e);
            this.checkBox31.Name = "checkBox31";
            this.checkBox31.Size = new Size(0x17, 0x1c);
            this.checkBox31.TabIndex = 0x23;
            this.checkBox31.Text = "31";
            this.checkBox31.UseVisualStyleBackColor = true;
            this.checkBox30.AutoSize = true;
            this.checkBox30.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox30.FlatStyle = FlatStyle.Flat;
            this.checkBox30.Location = new Point(0x277, 0x3e);
            this.checkBox30.Name = "checkBox30";
            this.checkBox30.Size = new Size(0x17, 0x1c);
            this.checkBox30.TabIndex = 0x23;
            this.checkBox30.Text = "30";
            this.checkBox30.UseVisualStyleBackColor = true;
            this.checkBox29.AutoSize = true;
            this.checkBox29.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox29.FlatStyle = FlatStyle.Flat;
            this.checkBox29.Location = new Point(0x266, 0x3e);
            this.checkBox29.Name = "checkBox29";
            this.checkBox29.Size = new Size(0x17, 0x1c);
            this.checkBox29.TabIndex = 0x23;
            this.checkBox29.Text = "29";
            this.checkBox29.UseVisualStyleBackColor = true;
            this.checkBox28.AutoSize = true;
            this.checkBox28.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox28.FlatStyle = FlatStyle.Flat;
            this.checkBox28.Location = new Point(0x255, 0x3e);
            this.checkBox28.Name = "checkBox28";
            this.checkBox28.Size = new Size(0x17, 0x1c);
            this.checkBox28.TabIndex = 0x23;
            this.checkBox28.Text = "28";
            this.checkBox28.UseVisualStyleBackColor = true;
            this.checkBox27.AutoSize = true;
            this.checkBox27.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox27.FlatStyle = FlatStyle.Flat;
            this.checkBox27.Location = new Point(580, 0x3e);
            this.checkBox27.Name = "checkBox27";
            this.checkBox27.Size = new Size(0x17, 0x1c);
            this.checkBox27.TabIndex = 0x23;
            this.checkBox27.Text = "27";
            this.checkBox27.UseVisualStyleBackColor = true;
            this.checkBox26.AutoSize = true;
            this.checkBox26.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox26.FlatStyle = FlatStyle.Flat;
            this.checkBox26.Location = new Point(0x233, 0x3e);
            this.checkBox26.Name = "checkBox26";
            this.checkBox26.Size = new Size(0x17, 0x1c);
            this.checkBox26.TabIndex = 0x23;
            this.checkBox26.Text = "26";
            this.checkBox26.UseVisualStyleBackColor = true;
            this.checkBox25.AutoSize = true;
            this.checkBox25.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox25.FlatStyle = FlatStyle.Flat;
            this.checkBox25.Location = new Point(0x222, 0x3e);
            this.checkBox25.Name = "checkBox25";
            this.checkBox25.Size = new Size(0x17, 0x1c);
            this.checkBox25.TabIndex = 0x23;
            this.checkBox25.Text = "25";
            this.checkBox25.UseVisualStyleBackColor = true;
            this.checkBox24.AutoSize = true;
            this.checkBox24.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox24.FlatStyle = FlatStyle.Flat;
            this.checkBox24.Location = new Point(0x211, 0x3e);
            this.checkBox24.Name = "checkBox24";
            this.checkBox24.Size = new Size(0x17, 0x1c);
            this.checkBox24.TabIndex = 0x23;
            this.checkBox24.Text = "24";
            this.checkBox24.UseVisualStyleBackColor = true;
            this.checkBox23.AutoSize = true;
            this.checkBox23.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox23.FlatStyle = FlatStyle.Flat;
            this.checkBox23.Location = new Point(0x200, 0x3e);
            this.checkBox23.Name = "checkBox23";
            this.checkBox23.Size = new Size(0x17, 0x1c);
            this.checkBox23.TabIndex = 0x23;
            this.checkBox23.Text = "23";
            this.checkBox23.UseVisualStyleBackColor = true;
            this.checkBox22.AutoSize = true;
            this.checkBox22.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox22.FlatStyle = FlatStyle.Flat;
            this.checkBox22.Location = new Point(0x1ef, 0x3e);
            this.checkBox22.Name = "checkBox22";
            this.checkBox22.Size = new Size(0x17, 0x1c);
            this.checkBox22.TabIndex = 0x23;
            this.checkBox22.Text = "22";
            this.checkBox22.UseVisualStyleBackColor = true;
            this.checkBox21.AutoSize = true;
            this.checkBox21.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox21.FlatStyle = FlatStyle.Flat;
            this.checkBox21.Location = new Point(0x1de, 0x3e);
            this.checkBox21.Name = "checkBox21";
            this.checkBox21.Size = new Size(0x17, 0x1c);
            this.checkBox21.TabIndex = 0x23;
            this.checkBox21.Text = "21";
            this.checkBox21.UseVisualStyleBackColor = true;
            this.checkBox20.AutoSize = true;
            this.checkBox20.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox20.FlatStyle = FlatStyle.Flat;
            this.checkBox20.Location = new Point(0x1cd, 0x3e);
            this.checkBox20.Name = "checkBox20";
            this.checkBox20.Size = new Size(0x17, 0x1c);
            this.checkBox20.TabIndex = 0x23;
            this.checkBox20.Text = "20";
            this.checkBox20.UseVisualStyleBackColor = true;
            this.checkBox19.AutoSize = true;
            this.checkBox19.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox19.FlatStyle = FlatStyle.Flat;
            this.checkBox19.Location = new Point(0x1bc, 0x3e);
            this.checkBox19.Name = "checkBox19";
            this.checkBox19.Size = new Size(0x17, 0x1c);
            this.checkBox19.TabIndex = 0x23;
            this.checkBox19.Text = "19";
            this.checkBox19.UseVisualStyleBackColor = true;
            this.checkBox18.AutoSize = true;
            this.checkBox18.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox18.FlatStyle = FlatStyle.Flat;
            this.checkBox18.Location = new Point(0x1ab, 0x3e);
            this.checkBox18.Name = "checkBox18";
            this.checkBox18.Size = new Size(0x17, 0x1c);
            this.checkBox18.TabIndex = 0x23;
            this.checkBox18.Text = "18";
            this.checkBox18.UseVisualStyleBackColor = true;
            this.checkBox17.AutoSize = true;
            this.checkBox17.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox17.FlatStyle = FlatStyle.Flat;
            this.checkBox17.Location = new Point(410, 0x3e);
            this.checkBox17.Name = "checkBox17";
            this.checkBox17.Size = new Size(0x17, 0x1c);
            this.checkBox17.TabIndex = 0x23;
            this.checkBox17.Text = "17";
            this.checkBox17.UseVisualStyleBackColor = true;
            this.checkBox16.AutoSize = true;
            this.checkBox16.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox16.FlatStyle = FlatStyle.Flat;
            this.checkBox16.Location = new Point(0x189, 0x3e);
            this.checkBox16.Name = "checkBox16";
            this.checkBox16.Size = new Size(0x17, 0x1c);
            this.checkBox16.TabIndex = 0x23;
            this.checkBox16.Text = "16";
            this.checkBox16.UseVisualStyleBackColor = true;
            this.checkBox15.AutoSize = true;
            this.checkBox15.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox15.FlatStyle = FlatStyle.Flat;
            this.checkBox15.Location = new Point(0x178, 0x3e);
            this.checkBox15.Name = "checkBox15";
            this.checkBox15.Size = new Size(0x17, 0x1c);
            this.checkBox15.TabIndex = 0x23;
            this.checkBox15.Text = "15";
            this.checkBox15.UseVisualStyleBackColor = true;
            this.checkBox14.AutoSize = true;
            this.checkBox14.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox14.FlatStyle = FlatStyle.Flat;
            this.checkBox14.Location = new Point(0x167, 0x3e);
            this.checkBox14.Name = "checkBox14";
            this.checkBox14.Size = new Size(0x17, 0x1c);
            this.checkBox14.TabIndex = 0x23;
            this.checkBox14.Text = "14";
            this.checkBox14.UseVisualStyleBackColor = true;
            this.checkBox13.AutoSize = true;
            this.checkBox13.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox13.FlatStyle = FlatStyle.Flat;
            this.checkBox13.Location = new Point(0x156, 0x3e);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new Size(0x17, 0x1c);
            this.checkBox13.TabIndex = 0x23;
            this.checkBox13.Text = "13";
            this.checkBox13.UseVisualStyleBackColor = true;
            this.checkBox12.AutoSize = true;
            this.checkBox12.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox12.FlatStyle = FlatStyle.Flat;
            this.checkBox12.Location = new Point(0x145, 0x3e);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new Size(0x17, 0x1c);
            this.checkBox12.TabIndex = 0x23;
            this.checkBox12.Text = "12";
            this.checkBox12.UseVisualStyleBackColor = true;
            this.checkBox11.AutoSize = true;
            this.checkBox11.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox11.FlatStyle = FlatStyle.Flat;
            this.checkBox11.Location = new Point(0x134, 0x3e);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new Size(0x17, 0x1c);
            this.checkBox11.TabIndex = 0x23;
            this.checkBox11.Text = "11";
            this.checkBox11.UseVisualStyleBackColor = true;
            this.checkBox10.AutoSize = true;
            this.checkBox10.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox10.FlatStyle = FlatStyle.Flat;
            this.checkBox10.Location = new Point(0x123, 0x3e);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new Size(0x17, 0x1c);
            this.checkBox10.TabIndex = 0x23;
            this.checkBox10.Text = "10";
            this.checkBox10.UseVisualStyleBackColor = true;
            this.checkBox9.AutoSize = true;
            this.checkBox9.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox9.FlatStyle = FlatStyle.Flat;
            this.checkBox9.Location = new Point(0x112, 0x3e);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new Size(0x17, 0x1c);
            this.checkBox9.TabIndex = 0x23;
            this.checkBox9.Text = "09";
            this.checkBox9.UseVisualStyleBackColor = true;
            this.checkBox8.AutoSize = true;
            this.checkBox8.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox8.FlatStyle = FlatStyle.Flat;
            this.checkBox8.Location = new Point(0x101, 0x3e);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new Size(0x17, 0x1c);
            this.checkBox8.TabIndex = 0x23;
            this.checkBox8.Text = "08";
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox7.AutoSize = true;
            this.checkBox7.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox7.FlatStyle = FlatStyle.Flat;
            this.checkBox7.Location = new Point(240, 0x3e);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new Size(0x17, 0x1c);
            this.checkBox7.TabIndex = 0x23;
            this.checkBox7.Text = "07";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox6.AutoSize = true;
            this.checkBox6.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox6.FlatStyle = FlatStyle.Flat;
            this.checkBox6.Location = new Point(0xdf, 0x3e);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new Size(0x17, 0x1c);
            this.checkBox6.TabIndex = 0x23;
            this.checkBox6.Text = "06";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox5.AutoSize = true;
            this.checkBox5.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox5.FlatStyle = FlatStyle.Flat;
            this.checkBox5.Location = new Point(0xce, 0x3e);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new Size(0x17, 0x1c);
            this.checkBox5.TabIndex = 0x23;
            this.checkBox5.Text = "05";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox4.AutoSize = true;
            this.checkBox4.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox4.FlatStyle = FlatStyle.Flat;
            this.checkBox4.Location = new Point(0xbd, 0x3e);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new Size(0x17, 0x1c);
            this.checkBox4.TabIndex = 0x23;
            this.checkBox4.Text = "04";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox3.AutoSize = true;
            this.checkBox3.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox3.FlatStyle = FlatStyle.Flat;
            this.checkBox3.Location = new Point(0xac, 0x3e);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new Size(0x17, 0x1c);
            this.checkBox3.TabIndex = 0x23;
            this.checkBox3.Text = "03";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox2.AutoSize = true;
            this.checkBox2.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox2.FlatStyle = FlatStyle.Flat;
            this.checkBox2.Location = new Point(0x9b, 0x3e);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new Size(0x17, 0x1c);
            this.checkBox2.TabIndex = 0x23;
            this.checkBox2.Text = "02";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox1.AutoSize = true;
            this.checkBox1.CheckAlign = ContentAlignment.BottomCenter;
            this.checkBox1.FlatStyle = FlatStyle.Flat;
            this.checkBox1.Location = new Point(0x8a, 0x3e);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new Size(0x17, 0x1c);
            this.checkBox1.TabIndex = 0x23;
            this.checkBox1.Text = "01";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.splitContainer1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            this.splitContainer1.Location = new Point(20, 150);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.sirfawareStatusTxtBox1);
            this.splitContainer1.Panel2.Controls.Add(this.sirfawareStatusTxtBox2);
            this.splitContainer1.Size = new Size(670, 0xc9);
            this.splitContainer1.SplitterDistance = 0x158;
            this.splitContainer1.TabIndex = 0x25;
            this.sirfawareStatusTxtBox2.BorderStyle = BorderStyle.None;
            this.sirfawareStatusTxtBox2.Dock = DockStyle.Fill;
            this.sirfawareStatusTxtBox2.Location = new Point(0, 0);
            this.sirfawareStatusTxtBox2.Name = "sirfawareStatusTxtBox2";
            this.sirfawareStatusTxtBox2.ReadOnly = true;
            this.sirfawareStatusTxtBox2.Size = new Size(320, 0xc7);
            this.sirfawareStatusTxtBox2.TabIndex = 0x22;
            this.sirfawareStatusTxtBox2.Text = "";
            this.sirfawareCumulativeTimeInFullPowerLabel.AutoSize = true;
            this.sirfawareCumulativeTimeInFullPowerLabel.Location = new Point(0x11, 0x7e);
            this.sirfawareCumulativeTimeInFullPowerLabel.Name = "sirfawareCumulativeTimeInFullPowerLabel";
            this.sirfawareCumulativeTimeInFullPowerLabel.Size = new Size(0x97, 13);
            this.sirfawareCumulativeTimeInFullPowerLabel.TabIndex = 0x26;
            this.sirfawareCumulativeTimeInFullPowerLabel.Text = "CumulateTime in Full Power(s):";
            this.sirfawareSVIDsLabel.AutoSize = true;
            this.sirfawareSVIDsLabel.Location = new Point(0x11, 0x3e);
            this.sirfawareSVIDsLabel.Name = "sirfawareSVIDsLabel";
            this.sirfawareSVIDsLabel.Size = new Size(0x26, 13);
            this.sirfawareSVIDsLabel.TabIndex = 0x27;
            this.sirfawareSVIDsLabel.Text = "SV ID:";
            this.sirfawareTotalTimeInMPMLabel.AutoSize = true;
            this.sirfawareTotalTimeInMPMLabel.Location = new Point(0x16e, 0x7e);
            this.sirfawareTotalTimeInMPMLabel.Name = "sirfawareTotalTimeInMPMLabel";
            this.sirfawareTotalTimeInMPMLabel.Size = new Size(0x89, 13);
            this.sirfawareTotalTimeInMPMLabel.TabIndex = 40;
            this.sirfawareTotalTimeInMPMLabel.Text = "Total Time in SiRFaware(s):";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            base.ClientSize = new Size(0x2be, 0x194);
            base.Controls.Add(this.sirfawareTotalTimeInMPMLabel);
            base.Controls.Add(this.sirfawareSVIDsLabel);
            base.Controls.Add(this.sirfawareCumulativeTimeInFullPowerLabel);
            base.Controls.Add(this.splitContainer1);
            base.Controls.Add(this.sirfawareSuccessfulALMCollectCheckBox);
            base.Controls.Add(this.sirfawareIsNavCheckBox);
            base.Controls.Add(this.checkBox32);
            base.Controls.Add(this.checkBox31);
            base.Controls.Add(this.checkBox30);
            base.Controls.Add(this.checkBox29);
            base.Controls.Add(this.checkBox28);
            base.Controls.Add(this.checkBox27);
            base.Controls.Add(this.checkBox26);
            base.Controls.Add(this.checkBox25);
            base.Controls.Add(this.checkBox24);
            base.Controls.Add(this.checkBox23);
            base.Controls.Add(this.checkBox22);
            base.Controls.Add(this.checkBox21);
            base.Controls.Add(this.checkBox20);
            base.Controls.Add(this.checkBox19);
            base.Controls.Add(this.checkBox18);
            base.Controls.Add(this.checkBox17);
            base.Controls.Add(this.checkBox16);
            base.Controls.Add(this.checkBox15);
            base.Controls.Add(this.checkBox14);
            base.Controls.Add(this.checkBox13);
            base.Controls.Add(this.checkBox12);
            base.Controls.Add(this.checkBox11);
            base.Controls.Add(this.checkBox10);
            base.Controls.Add(this.checkBox9);
            base.Controls.Add(this.checkBox8);
            base.Controls.Add(this.checkBox7);
            base.Controls.Add(this.checkBox6);
            base.Controls.Add(this.checkBox5);
            base.Controls.Add(this.checkBox4);
            base.Controls.Add(this.checkBox3);
            base.Controls.Add(this.checkBox2);
            base.Controls.Add(this.checkBox1);
            base.Controls.Add(this.sirfawareTTFFLabel);
            base.Controls.Add(this.sirfawareCollectedEphLabel);
            base.Controls.Add(this.sirfawareUpdateTypeLabel);
            base.Controls.Add(this.sirfawareLastUpdateTimeLabel);
            base.Controls.Add(this.sirfawareCurrentTimeLabel);
            base.Controls.Add(this.sirfawareExitBtn);
            base.Controls.Add(this.sirfawareGetPositionBtn);
            base.Controls.Add(this.sirfawareStartBtn);
            base.Controls.Add(this.toolStrip1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmCommSiRFawareV2";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SiRFaware";
            base.Load += new EventHandler(this.frmCommSiRFawareV2_Load);
            base.DoubleClick += new EventHandler(this.frmCommSiRFawareV2_DoubleClick);
            base.LocationChanged += new EventHandler(this.frmCommSiRFawareV2_LocationChanged);
            base.ResizeEnd += new EventHandler(this.frmCommSiRFawareV2_ResizeEnd);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.comm != null)
            {
                this.StopListeners();
            }
            if (this._MPMPlotWin != null)
            {
                this._MPMPlotWin.Close();
            }
            if (this.updateMainWindow != null)
            {
                this.updateMainWindow(base.Name);
            }
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, false);
            }
            this.cleanupData();
            this.SVList.Clear();
            this._SiRFAwareScanResultList.Clear();
        }

        private void OneSecondTimer_Tick(object sender, EventArgs e)
        {
            this.UpdateCurrentTime();
        }

        private void RefreshMPMPlot()
        {
            if ((this._MPMPlotWin != null) && (this._curvePlotIndicesList.Count > 0))
            {
                for (int i = 0; i < this._curvePlotIndicesList.Count; i++)
                {
                    this.getPlotData(this._curvePlotIndicesList[i], this._curvePlotTitlesList[i], this._curvePlotColorsList[i]);
                    if (i == 0)
                    {
                        this._MPMPlotWin.RefreshChart();
                    }
                    else
                    {
                        this._MPMPlotWin.AddCurveToPlot();
                    }
                }
            }
        }

        private void SendInputMessage(string payload, string hexString)
        {
            string msg = "A0A2 " + payload + " " + hexString + " " + this.comm.m_Protocols.GetChecksum(hexString, true) + " B0B3";
            this.comm.WriteData(msg);
        }

        private void sirfawareCollectedEphLabel_MouseHover(object sender, EventArgs e)
        {
            this.ephCollectionStatusDesc.Show(string.Format("Successful Collection = {0}\nFail Collection = {1}\nTo Collect = {2}\nUnchange = {3}", new object[] { "Green", "Red", "Yellow", "Gray" }), this, 0x1388);
        }

        private void sirfawareExitBtn_Click(object sender, EventArgs e)
        {
            this.StopListeners();
            base.Close();
        }

        private void sirfawareGetPositionBtn_Click(object sender, EventArgs e)
        {
            if ((this.comm != null) && !this.comm.IsSourceDeviceOpen())
            {
                MessageBox.Show("Port is not connected!", "SiRFaware Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                this.sirfawareStartBtn.Enabled = true;
                this.SendInputMessage("0002", "DA 00");
                this.ClearSiRFAwareWindowValues();
                if (this._inAwareMode && (this.comm.ProductFamily == CommonClass.ProductType.GSD4e))
                {
                    new frmCommSiRFAwarePic().ShowDialog();
                }
                this.comm.Toggle4eWakeupPort();
                this.sirfawareTTFFLabel.Visible = true;
                this.DisableLastUpdate();
                this.comm.dataGui._PMODE = 0;
                this.ClearTTFFTime();
                this.ClearLastUpdatedTime();
                this._resetTime = DateTime.Now;
                this._gotTTFF = false;
                this._inAwareMode = false;
                this.comm.WriteApp(string.Format("Total Time in SiRFaware(s) = {0:F2}", this._totalMPMTime));
            }
        }

        public void SirfawareGUIInit()
        {
			base.Invoke((MethodInvoker)delegate
			{
                this.ClearTTFFTime();
                this.updateSVEphCheckboxColor(-1, Color.Gray);
                this.sirfawareIsNavCheckBox.BackgroundBrush = new SolidBrush(Color.Gray);
                this.sirfawareSuccessfulALMCollectCheckBox.BackgroundBrush = new SolidBrush(Color.Gray);
                this.sirfawareIsNavCheckBox.Checked = false;
                this.sirfawareSuccessfulALMCollectCheckBox.Checked = false;
                this.sirfawareStatusTxtBox1.Text = string.Empty;
                this.sirfawareStatusTxtBox2.Text = string.Empty;
                this.sirfawareCumulativeTimeInFullPowerLabel.Text = "Cumulate Time in Full Power(s):";
                this.UpdateLastUpdatedTime();
                this.UpdateLastUpdatedTimeGUI();
                this.cleanupData();
            });
        }

        private void sirfawareStartBtnt_Click(object sender, EventArgs e)
        {
            if ((this.comm != null) && !this.comm.IsSourceDeviceOpen())
            {
                MessageBox.Show("Port is not connected!", "SiRFaware Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                this.SirfawareGUIInit();
                this.SendInputMessage("000A", "DD 00 10 10 00 00 00 00 00 00");
                this.comm.RxCtrl.SendMPM_V2(this.comm.LowPowerParams.MPM_Timeout, this.comm.LowPowerParams.MPM_Control);
                this.sirfawareStartBtn.Enabled = false;
                this.sirfawareTTFFLabel.Visible = false;
                this.StartListen();
                this._startTimeInSec = DateTime.Now;
                this._inAwareMode = true;
            }
        }

        private void sirfawareToolStripAddPlotBtn_Click(object sender, EventArgs e)
        {
            if (this._MPMPlotWin != null)
            {
                if (this.sirfawareToolStripPlotTitleTxtBox.Text == string.Empty)
                {
                    this.sirfawareToolStripPlotTitleTxtBox.Text = this.sirfawareToolStripDataSetBtn.Text + " vs TOW";
                }
                this.getPlotData(this.sirfawareToolStripDataSetBtn.SelectedIndex, this.sirfawareToolStripPlotTitleTxtBox.Text, this._currentCurveColor);
                this._MPMPlotWin.AddCurveToPlot();
                this._curvePlotIndicesList.Add(this.sirfawareToolStripDataSetBtn.SelectedIndex);
                this._curvePlotTitlesList.Add(this.sirfawareToolStripPlotTitleTxtBox.Text);
                this._curvePlotColorsList.Add(this._currentCurveColor);
            }
        }

        private void sirfawareToolStripConfigBtn_Click(object sender, EventArgs e)
        {
            if (this.comm != null)
            {
                new frmMPMConfigure(ref this.comm._lowPowerParams).ShowDialog();
            }
        }

        private void sirfawareToolStripHelpBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("Ephemeris Collection Status:\n    {0} = Successful collection\n    {1} = Fail collection\n    {2} = Set to collect\n    {3} = Unchange\n\nAlmanac Collection Status:\n    {4} = Successful collection\n    {5} = Fail collection\n    {6} = Unchange\n\nNav Status:\n    {7} = Successful Nav\n    {8} = Fail collection\n    {9} = Unknown\n\n", new object[] { Color.Green, Color.Red, Color.Yellow, Color.Gray, Color.Green, Color.Red, Color.Gray, Color.Green, Color.Red, Color.Gray }), "SiRFaware Help", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void sirfawareToolStripPlotBtn_Click(object sender, EventArgs e)
        {
            if (this._MPMPlotWin == null)
            {
                this._MPMPlotWin = new frmMPMStatsPlots();
                this._MPMPlotWin.MdiParent = base.MdiParent;
                this._MPMPlotWin.UpdateStatusToParentWin += new frmMPMStatsPlots.UpdateParentEventHandler(this.updatePlotStatus);
                this._MPMPlotWin.UpdateStatsData += new frmMPMStatsPlots.UpdateParentEventHandler(this.RefreshMPMPlot);
            }
            this.sirfawareToolStripPlotBtn.Enabled = false;
            this.sirfawareToolStripAddPlotBtn.Enabled = true;
            if (this.sirfawareToolStripPlotTitleTxtBox.Text == string.Empty)
            {
                this.sirfawareToolStripPlotTitleTxtBox.Text = this.sirfawareToolStripDataSetBtn.Text + " vs TOW";
            }
            this.getPlotData(this.sirfawareToolStripDataSetBtn.SelectedIndex, this.sirfawareToolStripPlotTitleTxtBox.Text, this._currentCurveColor);
            this._curvePlotIndicesList.Add(this.sirfawareToolStripDataSetBtn.SelectedIndex);
            this._curvePlotTitlesList.Add(this.sirfawareToolStripPlotTitleTxtBox.Text);
            this._curvePlotColorsList.Add(this._currentCurveColor);
            this._MPMPlotWin.Text = string.Format("{0}: SiRFaware Plots", this.comm.PortName);
            this._MPMPlotWin.Show();
        }

        private void sirfawareToolStripPlotColorBtn_Click(object sender, EventArgs e)
        {
            if (this.CurveColorDialog.ShowDialog() == DialogResult.OK)
            {
                this.sirfawareToolStripColorTxtBox.Text = this.CurveColorDialog.Color.ToString();
                this._currentCurveColor = this.CurveColorDialog.Color;
            }
        }

        public void StartListen()
        {
            if ((this.comm.ListenersCtrl != null) && (this.comm.MessageProtocol == "OSP"))
            {
                string listenerName = "MPM_STATS_SV_DATA_UPDATE_GUI";
                if (!this.comm.ListenersCtrl.Exists(listenerName, this.comm.PortName))
                {
                    ListenerContent content = this.comm.ListenersCtrl.Create(listenerName, this.comm.PortName);
                    if (content != null)
                    {
                        content.DoUserWork.DoWork += new DoWorkEventHandler(this.frmSiRFawareDisplayQueueHandler);
                        this._SiRFAwareScanResultList.Add(content.ListenerName);
                        this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                    }
                }
                else
                {
                    this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                }
                listenerName = "MPM_STATS_EPH_COLLECTION_GUI";
                if (!this.comm.ListenersCtrl.Exists(listenerName, this.comm.PortName))
                {
                    ListenerContent content2 = this.comm.ListenersCtrl.Create(listenerName, this.comm.PortName);
                    if (content2 != null)
                    {
                        content2.DoUserWork.DoWork += new DoWorkEventHandler(this.frmSiRFawareDisplayQueueHandler);
                        this._SiRFAwareScanResultList.Add(content2.ListenerName);
                        this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                    }
                }
                else
                {
                    this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                }
                listenerName = "MPM_STATS_ALM_COLLECTION_GUI";
                if (!this.comm.ListenersCtrl.Exists(listenerName, this.comm.PortName))
                {
                    ListenerContent content3 = this.comm.ListenersCtrl.Create(listenerName, this.comm.PortName);
                    if (content3 != null)
                    {
                        content3.DoUserWork.DoWork += new DoWorkEventHandler(this.frmSiRFawareDisplayQueueHandler);
                        this._SiRFAwareScanResultList.Add(content3.ListenerName);
                        this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                    }
                }
                else
                {
                    this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                }
                listenerName = "MPM_STATS_GPS_UPDATE_GUI";
                if (!this.comm.ListenersCtrl.Exists(listenerName, this.comm.PortName))
                {
                    ListenerContent content4 = this.comm.ListenersCtrl.Create(listenerName, this.comm.PortName);
                    if (content4 != null)
                    {
                        content4.DoUserWork.DoWork += new DoWorkEventHandler(this.frmSiRFawareDisplayQueueHandler);
                        this._SiRFAwareScanResultList.Add(content4.ListenerName);
                        this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                    }
                }
                else
                {
                    this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                }
                listenerName = "MPM_STATS_RECOVERY_MODE_GUI";
                if (!this.comm.ListenersCtrl.Exists(listenerName, this.comm.PortName))
                {
                    ListenerContent content5 = this.comm.ListenersCtrl.Create(listenerName, this.comm.PortName);
                    if (content5 != null)
                    {
                        content5.DoUserWork.DoWork += new DoWorkEventHandler(this.frmSiRFawareDisplayQueueHandler);
                        this._SiRFAwareScanResultList.Add(content5.ListenerName);
                        this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                    }
                }
                else
                {
                    this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                }
                listenerName = "MPM_STATS_FULLPOWER_SV_DATA_UPDATE_GUI";
                if (!this.comm.ListenersCtrl.Exists(listenerName, this.comm.PortName))
                {
                    ListenerContent content6 = this.comm.ListenersCtrl.Create(listenerName, this.comm.PortName);
                    if (content6 != null)
                    {
                        content6.DoUserWork.DoWork += new DoWorkEventHandler(this.frmSiRFawareDisplayQueueHandler);
                        this._SiRFAwareScanResultList.Add(content6.ListenerName);
                        this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                    }
                }
                else
                {
                    this.comm.ListenersCtrl.Start(listenerName, this.comm.PortName);
                }
            }
        }

        internal void StopListeners()
        {
            foreach (string str in this._SiRFAwareScanResultList)
            {
                try
                {
                    if (this.comm.ListenersCtrl != null)
                    {
                        this.comm.ListenersCtrl.Stop(str);
                        this.comm.ListenersCtrl.Delete(str);
                    }
                }
                catch
                {
                }
            }
            this._SiRFAwareScanResultList.Clear();
        }

        public void UpdateCurrentTime()
        {
            this._currTime = DateTime.Now;
            string str = string.Format("{0:D2}:{1:D2}:{2:D2}", this._currTime.Hour, this._currTime.Minute, this._currTime.Second);
            this.sirfawareCurrentTimeLabel.Text = "Current Time: " + str;
            if (this._inAwareMode)
            {
                TimeSpan span = (TimeSpan) (this._currTime - this._startTimeInSec);
                this._totalMPMTime = span.TotalSeconds;
                this.sirfawareTotalTimeInMPMLabel.Text = string.Format("Total Time in SiRFaware(s): {0:F1}", this._totalMPMTime);
            }
            this.UpdateLastUpdatedTimeGUI();
            this.UpdateTTFF();
        }

        public void UpdateLastUpdatedTime()
        {
            this._lastUpdateTime = DateTime.Now;
        }

        public void UpdateLastUpdatedTimeGUI()
        {
            DateTime time = new DateTime();
            if (!this._lastUpdateTime.Equals(time))
            {
                string str = string.Format("{0:D2}:{1:D2}:{2:D2}", this._lastUpdateTime.Hour, this._lastUpdateTime.Minute, this._lastUpdateTime.Second);
                this.sirfawareLastUpdateTimeLabel.Text = "Last Update: " + str;
            }
            else
            {
                this.sirfawareLastUpdateTimeLabel.Text = "Last Update: ";
            }
        }

        private void updatePlotStatus()
        {
            this.sirfawareToolStripPlotBtn.Enabled = true;
            this.sirfawareToolStripAddPlotBtn.Enabled = false;
            this._MPMPlotWin = null;
            this._curvePlotIndicesList.Clear();
            this._curvePlotTitlesList.Clear();
            this._curvePlotColorsList.Clear();
        }

        public void UpdateSiRFawareGUI(Hashtable SiRFAwareScanResHash)
        {
            try
            {
                this.DisplayStatsData = this.comm.RxCtrl.DecodeMPMStats(SiRFAwareScanResHash);
                if ((this.DisplayStatsDataList != null) && (this.DisplayStatsData != null))
                {
                    this._cumulateTimeInFullPower += this.DisplayStatsData.TimeSpentInFullPowerSec;
                    this.DisplayStatsData.CumulateTimeInFullPower = this._cumulateTimeInFullPower;
                    if (this.DisplayStatsDataList.Count > 0x2710)
                    {
                        this.DisplayStatsDataList.RemoveAt(0);
                    }
                    this.DisplayStatsDataList.Add(this.DisplayStatsData);
                    this.frmSiRFawareUpdateGUI();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void UpdateSiRFawareGUITime()
        {
			base.BeginInvoke((MethodInvoker)delegate
			{
                this.sirfawareTTFFLabel.Visible = true;
                this.sirfawareTTFFLabel.Text = string.Format("TTFF(s): {0:F1}", this._ttff);
                this.sirfawareTotalTimeInMPMLabel.Text = string.Format("Total Time in SiRFaware(s): {0:F2}", this._totalMPMTime);
            });
        }

        private void updateSVEphCheckboxColor(int myIdx, Color myColor)
        {
            if (myIdx == -1)
            {
                foreach (CommonClass.BinarycheckBox box in this.SVList)
                {
                    box.BackgroundBrush = new SolidBrush(myColor);
                    box.Checked = !box.Checked;
                }
            }
            else if (myIdx < this.SVList.Count)
            {
                this.SVList[myIdx].BackgroundBrush = new SolidBrush(myColor);
                this.SVList[myIdx].Checked = !this.SVList[myIdx].Checked;
            }
        }

        public void UpdateTTFF()
        {
            try
            {
                DateTime time = new DateTime();
                if (!this._resetTime.Equals(time))
                {
                    TimeSpan span = (TimeSpan) (this._currTime - this._resetTime);
                    if (!this._gotTTFF)
                    {
                        if (this.comm.dataGui._PMODE > 0)
                        {
                            if (span.TotalSeconds >= 1.0)
                            {
                                string str = string.Format("TTFF(s): {0:F1}", span.TotalSeconds);
                                this.sirfawareTTFFLabel.Text = str;
                                this._gotTTFF = true;
                                this.comm.WriteApp(str.Replace(":", "="));
                            }
                        }
                        else
                        {
                            this.ClearLastUpdatedTime();
                            this.ClearTTFFTime();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
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
                this.Text = this.comm.sourceDeviceName + ": SiRFaware";
            }
        }

        public double TotalMPMTime
        {
            get
            {
                return this._totalMPMTime;
            }
            set
            {
                this._totalMPMTime = value;
            }
        }

        public double TTFF
        {
            get
            {
                return this._ttff;
            }
            set
            {
                this._ttff = value;
            }
        }

        public delegate void updateParentEventHandler(string titleString);

        public delegate void UpdateWindowEventHandler(string titleString, int left, int top, int width, int height, bool state);
    }
}

