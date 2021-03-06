﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using CommMgrClassLibrary;
    using CommonClassLibrary;
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.MessageHandling;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    public class frmTrackerICConfig_Ver2 : Form
    {
        private bool _advancedGUIFlag;
        private bool _errorFound;
        private ComboBox basicRxConfig_comboBox;
        private Button button_Advanced;
        private Button button_Default;
        public double cDrift;
        public double clockFreq;
        private ComboBox comboBox_LDOEnable;
        private ComboBox comboBox_PowCtrlOnOff_Edgetype;
        private ComboBox comboBox_PowCtrlOnOff_Offtype;
        private ComboBox comboBox_PowCtrlOnOff_Usagetype;
        private ComboBox comboBox_RefClkOffset;
        private ComboBox comboBox_RefClkUncertainty;
        private CommunicationManager comm;
        private IContainer components;
        private ComboBox frmTrackerConfigBaudComboBox;
        private Label frmTrackerConfigBaudLabel;
        private Button frmTrackerConfigCancelBtn;
        private Label frmTrackerConfigFlowControlLabel;
        private ComboBox frmTrackerConfigFlowCtrlComboBox;
        private ComboBox frmTrackerConfigI2CModeComboBox;
        private ComboBox frmTrackerConfigI2CRateComboBox;
        private ComboBox frmTrackerConfigIOPinConfigEnableComboBox;
        private Label frmTrackerConfigIOPinConfigEnableLabel;
        private Label frmTrackerConfigIOPinConfigLabel;
        private ComboBox frmTrackerConfigLNASelectComboBox;
        private Label frmTrackerConfigLNASelectLabel;
        private Label frmTrackerConfigMasterAddrLabel;
        private TextBox frmTrackerConfigMasterAddrTextBox;
        private Label frmTrackerConfigModeLabel;
        private Button frmTrackerConfigOkBtn;
        private Label frmTrackerConfigRateLabel;
        private Label frmTrackerConfigSlaveAddrLabel;
        private TextBox frmTrackerConfigSlaveAddrTextBox;
        private Label frmTrackerConfigUncertaintyLable;
        private Label frmTrackerConfigWakeupMesgCntLabel;
        private TextBox frmTrackerConfigWakeupMesgCntTextBox;
        private Label frmTrackerConfigWakeupPatternLabel;
        private TextBox frmTrackerConfigWakeupPatternTextBox;
        private Label frmTrackerICConfigRefFreqLabel;
        private int fromBaseHex = 0x10;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private ComboBox ioPortType_comboBox;
        private Label label1;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label2;
        private Label label20;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label29;
        private Label label3;
        private Label label30;
        private Label label31;
        private Label label32;
        private Label label33;
        private Label label34;
        private Label label35;
        private Label label36;
        private Label label37;
        private Label label38;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private bool needHotStart;
        private CheckBox newConfigCheckBox;
        private Label newConfigDetailLabel;
        public double onePPM;
        private Button PollICConfigBtn;
        private ComboBox refClkFreqComboBox;
        public double refclkOffsetPPM;
        private TextBox textBox_GPIO0;
        private TextBox textBox_GPIO1;
        private TextBox textBox_GPIO10;
        private TextBox textBox_GPIO2;
        private TextBox textBox_GPIO3;
        private TextBox textBox_GPIO4;
        private TextBox textBox_GPIO5;
        private TextBox textBox_GPIO6;
        private TextBox textBox_GPIO7;
        private TextBox textBox_GPIO8;
        private TextBox textBox_GPIO9;
        private TextBox textBox_I2CMaxMsgLength;
        private TextBox textBox_RefClkWarmupDelay;

        public frmTrackerICConfig_Ver2(CommunicationManager commMgr)
        {
            this.InitializeComponent();
            this.LoadRefClockList();
            this.comm = commMgr;
            if (this.comm.ProductFamily == CommonClass.ProductType.GSD4e)
            {
                this.PollICConfigBtn.Visible = true;
            }
        }

        private void basicRxConfig_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.basicRxConfig_comboBox.SelectedIndex)
            {
                case 0:
                    this.textBox_GPIO0.Text = "0";
                    this.textBox_GPIO1.Text = "0";
                    this.textBox_GPIO2.Text = "0";
                    this.textBox_GPIO3.Text = "0";
                    this.textBox_GPIO4.Text = "0";
                    this.textBox_GPIO5.Text = "0";
                    this.textBox_GPIO10.Text = "0";
                    break;

                case 1:
                    this.textBox_GPIO0.Text = "0";
                    this.textBox_GPIO1.Text = "0";
                    this.textBox_GPIO2.Text = "45";
                    this.textBox_GPIO3.Text = "0";
                    this.textBox_GPIO4.Text = "0";
                    this.textBox_GPIO5.Text = "0";
                    this.textBox_GPIO10.Text = "0";
                    break;

                case 2:
                    this.textBox_GPIO0.Text = "3C4";
                    this.textBox_GPIO1.Text = "3C4";
                    this.textBox_GPIO2.Text = "45";
                    this.textBox_GPIO3.Text = "0";
                    this.textBox_GPIO4.Text = "0";
                    this.textBox_GPIO5.Text = "0";
                    this.textBox_GPIO10.Text = "0";
                    break;

                case 3:
                    this.textBox_GPIO0.Text = "3C4";
                    this.textBox_GPIO1.Text = "3C4";
                    this.textBox_GPIO2.Text = "45";
                    this.textBox_GPIO3.Text = "1";
                    this.textBox_GPIO4.Text = "1";
                    this.textBox_GPIO5.Text = "3C5";
                    this.textBox_GPIO10.Text = "3C6";
                    break;
            }
            this.Refresh();
        }

        private void button_Advanced_Click(object sender, EventArgs e)
        {
            this._advancedGUIFlag = !this._advancedGUIFlag;
            if (this._advancedGUIFlag && (MessageBox.Show("Incorrectly configuring these parameters can make the receiver non-operational -- Proceed?", "IC Configuration Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No))
            {
                this._advancedGUIFlag = false;
            }
            else
            {
                this.setWindowSizeforAdvancedOptions();
                this.setControlVisible(this._advancedGUIFlag);
            }
        }

        private void button_Default_Click(object sender, EventArgs e)
        {
            string defaultValue = string.Empty;
            bool isSLCRx = false;
            ArrayList list = new ArrayList();
            list = this.comm.m_Protocols.GetDefaultMsgFieldList(isSLCRx, 0xb2, 2, "Tracker Configuration Version 2.0", "OSP");
            for (int i = 0; i < list.Count; i++)
            {
                string str2;
                string str3;
                defaultValue = ((InputMsg) list[i]).defaultValue;
                switch (((InputMsg) list[i]).fieldName)
                {
                    case "Reference Clock Frequency":
                    {
                        this.refClkFreqComboBox.Text = defaultValue;
                        continue;
                    }
                    case "Reference Clock Warmup Delay":
                    {
                        this.textBox_RefClkWarmupDelay.Text = defaultValue;
                        continue;
                    }
                    case "Reference Clock Uncertainty":
                    {
                        this.comboBox_RefClkUncertainty.Text = defaultValue;
                        continue;
                    }
                    case "Reference Clock Offset":
                    {
                        this.comboBox_RefClkOffset.Text = string.Format("{0:0.0}", Convert.ToSingle(defaultValue));
                        continue;
                    }
                    case "External LNA Enable":
                    {
                        if (Convert.ToByte(defaultValue) != 0)
                        {
                            break;
                        }
                        this.frmTrackerConfigLNASelectComboBox.SelectedIndex = 0;
                        continue;
                    }
                    case "IO Pin Config Enable":
                        if (Convert.ToByte(defaultValue) != 0)
                        {
                            goto Label_0310;
                        }
                        this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex = 0;
                        goto Label_031C;

                    case "IO Pin Config GPIO0":
                    {
                        str2 = Convert.ToUInt16(defaultValue).ToString("X");
                        if (this.comm.ProductFamily != CommonClass.ProductType.GSD4t)
                        {
                            goto Label_0371;
                        }
                        this.textBox_GPIO0.Text = "0";
                        continue;
                    }
                    case "IO Pin Config GPIO1":
                    {
                        str3 = Convert.ToUInt16(defaultValue).ToString("X");
                        if (this.comm.ProductFamily != CommonClass.ProductType.GSD4t)
                        {
                            goto Label_03BB;
                        }
                        this.textBox_GPIO1.Text = "0";
                        continue;
                    }
                    case "IO Pin Config GPIO2":
                    {
                        string str4 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO2.Text = str4;
                        continue;
                    }
                    case "IO Pin Config GPIO3":
                    {
                        string str5 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO3.Text = str5;
                        continue;
                    }
                    case "IO Pin Config GPIO4":
                    {
                        string str6 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO4.Text = str6;
                        continue;
                    }
                    case "IO Pin Config GPIO5":
                    {
                        string str7 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO5.Text = str7;
                        continue;
                    }
                    case "IO Pin Config GPIO6":
                    {
                        string str8 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO6.Text = str8;
                        continue;
                    }
                    case "IO Pin Config GPIO7":
                    {
                        string str9 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO7.Text = str9;
                        continue;
                    }
                    case "IO Pin Config GPIO8":
                    {
                        string str10 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO8.Text = str10;
                        continue;
                    }
                    case "IO Pin Config GPIO9":
                    {
                        string str11 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO9.Text = str11;
                        continue;
                    }
                    case "IO Pin Config GPIO10":
                    {
                        string str12 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.textBox_GPIO10.Text = str12;
                        continue;
                    }
                    case "UART Max Premable":
                    {
                        this.frmTrackerConfigWakeupPatternTextBox.Text = defaultValue;
                        continue;
                    }
                    case "UART Idle Byte Wakeup Delay":
                    {
                        this.frmTrackerConfigWakeupMesgCntTextBox.Text = defaultValue;
                        continue;
                    }
                    case "UART Baud":
                    {
                        this.frmTrackerConfigBaudComboBox.Text = defaultValue;
                        continue;
                    }
                    case "UART HW Flow Control":
                    {
                        if (!(defaultValue == "0"))
                        {
                            goto Label_0586;
                        }
                        this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = 0;
                        continue;
                    }
                    case "I2C Master Address":
                    {
                        string str13 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.frmTrackerConfigMasterAddrTextBox.Text = str13;
                        continue;
                    }
                    case "I2C Slave Address":
                    {
                        string str14 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.frmTrackerConfigSlaveAddrTextBox.Text = str14;
                        continue;
                    }
                    case "I2C Rate":
                    {
                        byte num17 = Convert.ToByte(defaultValue);
                        this.frmTrackerConfigI2CRateComboBox.SelectedIndex = num17;
                        continue;
                    }
                    case "I2C Mode":
                    {
                        byte num18 = Convert.ToByte(defaultValue);
                        this.frmTrackerConfigI2CModeComboBox.SelectedIndex = num18;
                        continue;
                    }
                    case "I2C Max Message length":
                    {
                        this.textBox_I2CMaxMsgLength.Text = Convert.ToUInt16(defaultValue).ToString();
                        continue;
                    }
                    case "PowerControl On Off":
                    {
                        if (defaultValue == "0")
                        {
                            this.comboBox_PowCtrlOnOff_Edgetype.SelectedIndex = GetBits(this.comm.TrackerICCtrl.PwrCtrlOnOff, 0, 3);
                            this.comboBox_PowCtrlOnOff_Usagetype.SelectedIndex = GetBits(this.comm.TrackerICCtrl.PwrCtrlOnOff, 3, 2);
                            this.comboBox_PowCtrlOnOff_Offtype.SelectedIndex = GetBits(this.comm.TrackerICCtrl.PwrCtrlOnOff, 5, 1);
                        }
                        continue;
                    }
                    case "Backup LDO mode enabled":
                    {
                        if (!(defaultValue == "0"))
                        {
                            goto Label_06F4;
                        }
                        if (this.comm.ProductFamily == CommonClass.ProductType.GSD4t)
                        {
                            this.comboBox_LDOEnable.SelectedIndex = 0;
                        }
                        if (this.comm.ProductFamily == CommonClass.ProductType.GSD4e)
                        {
                            this.comboBox_LDOEnable.SelectedIndex = 1;
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                this.frmTrackerConfigLNASelectComboBox.SelectedIndex = 1;
                continue;
            Label_0310:
                this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex = 1;
            Label_031C:
                this.ioPortType_comboBox.SelectedIndex = 0;
                this.basicRxConfig_comboBox.SelectedIndex = 0;
                continue;
            Label_0371:
                this.textBox_GPIO0.Text = str2;
                continue;
            Label_03BB:
                this.textBox_GPIO1.Text = str3;
                continue;
            Label_0586:
                this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = 1;
                continue;
            Label_06F4:
                this.comboBox_LDOEnable.SelectedIndex = 1;
            }
        }

        private void comboBox_RefClkOffset_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    float num;
                    if (this.comboBox_RefClkOffset.Text == "Unknown - use default")
                    {
                        num = 2.147484E+09f;
                    }
                    else
                    {
                        num = Convert.ToSingle(this.comboBox_RefClkOffset.Text);
                        if ((num < 0f) || (num > 1362973f))
                        {
                            MessageBox.Show("The Ref Clock Offset value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._errorFound = true;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("The Ref Clock Offset value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void comboBox_RefClkUncertainty_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    uint maxValue;
                    if (this.comboBox_RefClkUncertainty.Text == "Unknown - use default")
                    {
                        maxValue = uint.MaxValue;
                    }
                    else
                    {
                        maxValue = Convert.ToUInt32(this.comboBox_RefClkUncertainty.Text);
                    }
                    if ((maxValue < 0) || (maxValue > uint.MaxValue))
                    {
                        MessageBox.Show("The Ref Clock Uncertainty value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._errorFound = true;
                    }
                }
                catch
                {
                    MessageBox.Show("The Ref Clock Uncertainty value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private static double convertHzToPPM(double refFreq, double clkOffset)
        {
            double num = refFreq;
            double num2 = ((num * Math.Pow(10.0, -6.0)) * 1540.0) / 16.0;
            double num3 = ((num * 1540.0) / 16.0) - clsGlobal.GPSL1Frequency;
            double num4 = (clkOffset - num3) / num2;
            if (num4 < 0.0)
            {
                num4 = 0.0;
            }
            return num4;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void enableDisableAllCtrls(bool enable)
        {
            this.textBox_RefClkWarmupDelay.Enabled = enable;
            this.comboBox_RefClkUncertainty.Enabled = enable;
            this.comboBox_RefClkOffset.Enabled = enable;
            this.frmTrackerConfigLNASelectComboBox.Enabled = enable;
            this.frmTrackerConfigIOPinConfigEnableComboBox.Enabled = enable;
            this.textBox_GPIO0.Enabled = enable;
            if (enable && (this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex == 1))
            {
                this.enableDisableIOPinConfig(true);
            }
            else
            {
                this.enableDisableIOPinConfig(false);
            }
            this.frmTrackerConfigWakeupPatternTextBox.Enabled = enable;
            this.frmTrackerConfigWakeupMesgCntTextBox.Enabled = enable;
            this.frmTrackerConfigBaudComboBox.Enabled = enable;
            this.frmTrackerConfigFlowCtrlComboBox.Enabled = enable;
            this.frmTrackerConfigMasterAddrTextBox.Enabled = enable;
            this.frmTrackerConfigSlaveAddrTextBox.Enabled = enable;
            this.frmTrackerConfigI2CRateComboBox.Enabled = enable;
            this.frmTrackerConfigI2CModeComboBox.Enabled = enable;
            this.textBox_I2CMaxMsgLength.Enabled = enable;
            this.comboBox_PowCtrlOnOff_Edgetype.Enabled = enable;
            this.comboBox_PowCtrlOnOff_Usagetype.Enabled = enable;
            this.comboBox_RefClkOffset.Enabled = enable;
            this.comboBox_LDOEnable.Enabled = enable;
        }

        private void enableDisableIOPinConfig(bool enable)
        {
            this.textBox_GPIO0.Enabled = enable;
            this.textBox_GPIO1.Enabled = enable;
            this.textBox_GPIO2.Enabled = enable;
            this.textBox_GPIO3.Enabled = enable;
            this.textBox_GPIO4.Enabled = enable;
            this.textBox_GPIO5.Enabled = enable;
            this.textBox_GPIO6.Enabled = enable;
            this.textBox_GPIO7.Enabled = enable;
            this.textBox_GPIO8.Enabled = enable;
            this.textBox_GPIO9.Enabled = enable;
            this.groupBox7.Enabled = enable;
            this.ioPortType_comboBox.SelectedIndex = 0;
            this.basicRxConfig_comboBox.SelectedIndex = 0;
            int num = 0;
            this.textBox_GPIO0.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO1.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO2.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO3.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO4.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO5.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO6.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO7.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO8.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO9.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            if (this.newConfigCheckBox.Checked)
            {
                this.textBox_GPIO10.Enabled = enable;
                this.textBox_GPIO10.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
                this.comm.TrackerICCtrl.Version = 2.0;
            }
            else
            {
                this.textBox_GPIO10.Enabled = false;
                this.textBox_GPIO10.Text = "0";
            }
            if (this.comm.ProductFamily == CommonClass.ProductType.GSD4t)
            {
                this.textBox_GPIO0.Text = "0";
                this.textBox_GPIO1.Text = "0";
            }
            if (this.comm.ProductFamily == CommonClass.ProductType.GSD4e)
            {
                this.textBox_GPIO0.Text = "3FC";
                this.textBox_GPIO1.Text = "3FC";
            }
        }

        private void frmTrackerConfigCancelBtn_Click(object sender, EventArgs e)
        {
            base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._errorFound = false;
            base.Close();
        }

        private void frmTrackerConfigI2CRateComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (((base.DialogResult != DialogResult.Cancel) && ((this.frmTrackerConfigI2CRateComboBox.SelectedIndex == 2) || (this.frmTrackerConfigI2CRateComboBox.SelectedIndex == 3))) && ((this.comm.ProductFamily == CommonClass.ProductType.GSD4e) || (this.comm.ProductFamily == CommonClass.ProductType.GSD4t)))
            {
                MessageBox.Show(string.Format("The selected I2C Rate value doesn't work with {0}", this.comm.ProductFamily), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                this._errorFound = true;
            }
        }

        private void frmTrackerConfigIOPinConfigEnableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex == 1) && ((this.comm.ProductFamily == CommonClass.ProductType.GSD4t) || (this.comm.ProductFamily == CommonClass.ProductType.GSD4e)))
            {
                this.enableDisableIOPinConfig(true);
            }
            else
            {
                this.enableDisableIOPinConfig(false);
            }
        }

        private void frmTrackerConfigMasterAddrTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    ushort num = Convert.ToUInt16(this.frmTrackerConfigMasterAddrTextBox.Text);
                    if (((ushort) (num & 0xf000)) >= 0xf000)
                    {
                        if ((num > 0xf3ff) || (num < 0xf000))
                        {
                            MessageBox.Show("The I2C Master Address value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._errorFound = true;
                        }
                    }
                    else if ((num > 0x7f) || (num < 8))
                    {
                        MessageBox.Show("The I2C Master Address value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._errorFound = true;
                    }
                }
                catch
                {
                    MessageBox.Show("The I2C Master Address value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void frmTrackerConfigOkBtn_Click(object sender, EventArgs e)
        {
            bool sendHotStart = false;
            if (this.needHotStart)
            {
                string text = "Send a Hot Start to enable Tracker IC changes?";
                string caption = "Tracker IC Changed";
                switch (MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        sendHotStart = true;
                        break;

                    case DialogResult.No:
                        sendHotStart = false;
                        break;

                    case DialogResult.Cancel:
                        return;
                }
            }
            this._errorFound = false;
            if (clsGlobal.PerformOnAll)
            {
                foreach (string str3 in clsGlobal.g_objfrmMDIMain.PortManagerHash.Keys)
                {
                    if (!(str3 == clsGlobal.FilePlayBackPortName))
                    {
                        PortManager manager = (PortManager) clsGlobal.g_objfrmMDIMain.PortManagerHash[str3];
                        if ((manager != null) && manager.comm.IsSourceDeviceOpen())
                        {
                            this.saveTrackerConfigParams(ref manager.comm, sendHotStart);
                        }
                    }
                }
                clsGlobal.PerformOnAll = false;
            }
            else
            {
                this.saveTrackerConfigParams(ref this.comm, sendHotStart);
            }
            base.DialogResult = DialogResult.OK;
            base.Close();
        }

        private void frmTrackerConfigSlaveAddrTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    ushort num = Convert.ToUInt16(this.frmTrackerConfigSlaveAddrTextBox.Text);
                    if (((ushort) (num & 0xf000)) >= 0xf000)
                    {
                        if ((num > 0xf3ff) || (num < 0xf000))
                        {
                            MessageBox.Show("The I2C Master Address value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this._errorFound = true;
                        }
                    }
                    else if ((num > 0x7f) || (num < 8))
                    {
                        MessageBox.Show("The I2C Master Address value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._errorFound = true;
                    }
                }
                catch
                {
                    MessageBox.Show("The I2C Slave Address value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void frmTrackerConfigWakeupMesgCntTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToByte(this.frmTrackerConfigWakeupMesgCntTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("The UART Idle Byte Wakeup delay value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void frmTrackerConfigWakeupPatternTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToByte(this.frmTrackerConfigWakeupPatternTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("The UART Max Preamble value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void frmTrackerICConfig_Ver2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = this._errorFound;
        }

        private void frmTrackerICConfig_Ver2_Load(object sender, EventArgs e)
        {
            this._errorFound = false;
            this.refClkFreqComboBox.Text = this.comm.TrackerICCtrl.RefFreq.ToString();
            this.textBox_RefClkWarmupDelay.Text = this.comm.TrackerICCtrl.StartupDelay.ToString();
            if (this.comm.TrackerICCtrl.RefClkUncertainty >= 0xffffe380)
            {
                this.comboBox_RefClkUncertainty.Text = "Unknown - use default";
            }
            else
            {
                this.comboBox_RefClkUncertainty.Text = this.comm.TrackerICCtrl.RefClkUncertainty.ToString();
            }
            if (this.comm.TrackerICCtrl.RefClkOffset >= 2147483647.0)
            {
                this.comboBox_RefClkOffset.Text = "Unknown - use default";
            }
            else
            {
                this.comboBox_RefClkOffset.Text = convertHzToPPM((double) this.comm.TrackerICCtrl.RefFreq, this.comm.TrackerICCtrl.RefClkOffset).ToString("F1");
            }
            this.frmTrackerConfigLNASelectComboBox.SelectedIndex = this.comm.TrackerICCtrl.LNASelect;
            this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex = this.comm.TrackerICCtrl.IOPinConfigEnable;
            if (this.comm.TrackerICCtrl.IOPinConfigEnable == 0)
            {
                this.enableDisableIOPinConfig(false);
            }
            else
            {
                this.enableDisableIOPinConfig(true);
            }
            this.frmTrackerConfigWakeupPatternTextBox.Text = this.comm.TrackerICCtrl.UARTPreambleMax.ToString();
            this.frmTrackerConfigWakeupMesgCntTextBox.Text = this.comm.TrackerICCtrl.UARTWakeupDelay.ToString();
            this.frmTrackerConfigBaudComboBox.Text = this.comm.TrackerICCtrl.UARTBaud.ToString();
            this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = this.comm.TrackerICCtrl.UARTFlowControlEnable;
            this.textBox_I2CMaxMsgLength.Text = this.comm.TrackerICCtrl.I2CMaxMsgLength.ToString();
            this.frmTrackerConfigMasterAddrTextBox.Text = this.comm.TrackerICCtrl.I2CMasterAddress.ToString("X");
            this.frmTrackerConfigSlaveAddrTextBox.Text = this.comm.TrackerICCtrl.I2CSlaveAddress.ToString("X");
            this.frmTrackerConfigI2CRateComboBox.SelectedIndex = this.comm.TrackerICCtrl.I2CRate;
            this.frmTrackerConfigI2CModeComboBox.SelectedIndex = this.comm.TrackerICCtrl.I2CMode;
            this.comboBox_PowCtrlOnOff_Edgetype.SelectedIndex = GetBits(this.comm.TrackerICCtrl.PwrCtrlOnOff, 0, 3);
            this.comboBox_PowCtrlOnOff_Usagetype.SelectedIndex = GetBits(this.comm.TrackerICCtrl.PwrCtrlOnOff, 3, 2);
            this.comboBox_PowCtrlOnOff_Offtype.SelectedIndex = GetBits(this.comm.TrackerICCtrl.PwrCtrlOnOff, 5, 1);
            this.comboBox_LDOEnable.SelectedIndex = this.comm.TrackerICCtrl.LDOModeEnabled;
            this.setWindowSizeforAdvancedOptions();
            this.setControlVisible(false);
            if (this.comm.ProductFamily != CommonClass.ProductType.GSD4e)
            {
                this.label24.Visible = false;
                this.label34.Visible = false;
                this.textBox_GPIO10.Visible = false;
            }
        }

        private static int GetBits(byte b, int offset, int count)
        {
            return ((b >> offset) & ((((int) 1) << count) - 1));
        }

        public ArrayList GetRefClockFreqs()
        {
            ArrayList list = new ArrayList();
            list.Add("13000000");
            list.Add("16369000");
            list.Add("16800000");
            list.Add("19200000");
            list.Add("19800000");
            list.Add("26000000");
            list.Add("33600000");
            list.Add("38400000");
            list.Add("40000000");
            return list;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmTrackerICConfig_Ver2));
            this.button_Default = new Button();
            this.frmTrackerConfigCancelBtn = new Button();
            this.frmTrackerConfigOkBtn = new Button();
            this.groupBox5 = new GroupBox();
            this.textBox_I2CMaxMsgLength = new TextBox();
            this.label8 = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.frmTrackerConfigMasterAddrTextBox = new TextBox();
            this.frmTrackerConfigModeLabel = new Label();
            this.frmTrackerConfigRateLabel = new Label();
            this.frmTrackerConfigSlaveAddrTextBox = new TextBox();
            this.frmTrackerConfigSlaveAddrLabel = new Label();
            this.frmTrackerConfigI2CModeComboBox = new ComboBox();
            this.frmTrackerConfigI2CRateComboBox = new ComboBox();
            this.frmTrackerConfigMasterAddrLabel = new Label();
            this.groupBox4 = new GroupBox();
            this.frmTrackerConfigFlowCtrlComboBox = new ComboBox();
            this.frmTrackerConfigWakeupMesgCntTextBox = new TextBox();
            this.frmTrackerConfigWakeupMesgCntLabel = new Label();
            this.frmTrackerConfigWakeupPatternTextBox = new TextBox();
            this.frmTrackerConfigWakeupPatternLabel = new Label();
            this.frmTrackerConfigFlowControlLabel = new Label();
            this.frmTrackerConfigBaudComboBox = new ComboBox();
            this.frmTrackerConfigBaudLabel = new Label();
            this.groupBox2 = new GroupBox();
            this.groupBox7 = new GroupBox();
            this.label36 = new Label();
            this.label35 = new Label();
            this.ioPortType_comboBox = new ComboBox();
            this.basicRxConfig_comboBox = new ComboBox();
            this.label34 = new Label();
            this.label33 = new Label();
            this.label32 = new Label();
            this.label31 = new Label();
            this.label30 = new Label();
            this.label29 = new Label();
            this.label28 = new Label();
            this.label27 = new Label();
            this.label23 = new Label();
            this.label22 = new Label();
            this.label9 = new Label();
            this.textBox_GPIO10 = new TextBox();
            this.label24 = new Label();
            this.textBox_GPIO9 = new TextBox();
            this.label20 = new Label();
            this.textBox_GPIO8 = new TextBox();
            this.label17 = new Label();
            this.textBox_GPIO7 = new TextBox();
            this.label16 = new Label();
            this.textBox_GPIO4 = new TextBox();
            this.label18 = new Label();
            this.textBox_GPIO6 = new TextBox();
            this.label15 = new Label();
            this.textBox_GPIO3 = new TextBox();
            this.label19 = new Label();
            this.textBox_GPIO5 = new TextBox();
            this.label14 = new Label();
            this.textBox_GPIO2 = new TextBox();
            this.label13 = new Label();
            this.textBox_GPIO1 = new TextBox();
            this.label5 = new Label();
            this.textBox_GPIO0 = new TextBox();
            this.frmTrackerConfigIOPinConfigLabel = new Label();
            this.frmTrackerConfigIOPinConfigEnableComboBox = new ComboBox();
            this.frmTrackerConfigIOPinConfigEnableLabel = new Label();
            this.frmTrackerConfigLNASelectComboBox = new ComboBox();
            this.frmTrackerConfigLNASelectLabel = new Label();
            this.groupBox1 = new GroupBox();
            this.comboBox_RefClkOffset = new ComboBox();
            this.comboBox_RefClkUncertainty = new ComboBox();
            this.label21 = new Label();
            this.label4 = new Label();
            this.label12 = new Label();
            this.frmTrackerConfigUncertaintyLable = new Label();
            this.textBox_RefClkWarmupDelay = new TextBox();
            this.label3 = new Label();
            this.label2 = new Label();
            this.frmTrackerICConfigRefFreqLabel = new Label();
            this.label1 = new Label();
            this.label11 = new Label();
            this.comboBox_LDOEnable = new ComboBox();
            this.button_Advanced = new Button();
            this.newConfigCheckBox = new CheckBox();
            this.newConfigDetailLabel = new Label();
            this.refClkFreqComboBox = new ComboBox();
            this.groupBox3 = new GroupBox();
            this.comboBox_PowCtrlOnOff_Offtype = new ComboBox();
            this.comboBox_PowCtrlOnOff_Usagetype = new ComboBox();
            this.comboBox_PowCtrlOnOff_Edgetype = new ComboBox();
            this.label26 = new Label();
            this.label25 = new Label();
            this.label10 = new Label();
            this.PollICConfigBtn = new Button();
            this.groupBox6 = new GroupBox();
            this.label37 = new Label();
            this.label38 = new Label();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            base.SuspendLayout();
            this.button_Default.Location = new Point(0x38, 0x15);
            this.button_Default.Name = "button_Default";
            this.button_Default.Size = new Size(0x4b, 0x17);
            this.button_Default.TabIndex = 0x26;
            this.button_Default.Text = "&Default";
            this.button_Default.UseVisualStyleBackColor = true;
            this.button_Default.Click += new EventHandler(this.button_Default_Click);
            this.frmTrackerConfigCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.frmTrackerConfigCancelBtn.Location = new Point(0x1da, 0x35);
            this.frmTrackerConfigCancelBtn.Name = "frmTrackerConfigCancelBtn";
            this.frmTrackerConfigCancelBtn.Size = new Size(0x4b, 0x17);
            this.frmTrackerConfigCancelBtn.TabIndex = 0x24;
            this.frmTrackerConfigCancelBtn.Text = "&Cancel";
            this.frmTrackerConfigCancelBtn.UseVisualStyleBackColor = true;
            this.frmTrackerConfigCancelBtn.Click += new EventHandler(this.frmTrackerConfigCancelBtn_Click);
            this.frmTrackerConfigOkBtn.Location = new Point(0x1da, 0x18);
            this.frmTrackerConfigOkBtn.Name = "frmTrackerConfigOkBtn";
            this.frmTrackerConfigOkBtn.Size = new Size(0x4b, 0x17);
            this.frmTrackerConfigOkBtn.TabIndex = 0x23;
            this.frmTrackerConfigOkBtn.Text = "&OK";
            this.frmTrackerConfigOkBtn.UseVisualStyleBackColor = true;
            this.frmTrackerConfigOkBtn.Click += new EventHandler(this.frmTrackerConfigOkBtn_Click);
            this.groupBox5.Controls.Add(this.textBox_I2CMaxMsgLength);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.frmTrackerConfigMasterAddrTextBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigModeLabel);
            this.groupBox5.Controls.Add(this.frmTrackerConfigRateLabel);
            this.groupBox5.Controls.Add(this.frmTrackerConfigSlaveAddrTextBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigSlaveAddrLabel);
            this.groupBox5.Controls.Add(this.frmTrackerConfigI2CModeComboBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigI2CRateComboBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigMasterAddrLabel);
            this.groupBox5.Location = new Point(0x119, 0x11c);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new Size(0x10c, 0x81);
            this.groupBox5.TabIndex = 0x22;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "I2C Config";
            this.textBox_I2CMaxMsgLength.Location = new Point(0x98, 0x62);
            this.textBox_I2CMaxMsgLength.Name = "textBox_I2CMaxMsgLength";
            this.textBox_I2CMaxMsgLength.Size = new Size(100, 20);
            this.textBox_I2CMaxMsgLength.TabIndex = 40;
            this.textBox_I2CMaxMsgLength.Validating += new CancelEventHandler(this.textBox_I2CMaxMsgLength_Validating);
            this.label8.AutoSize = true;
            this.label8.Location = new Point(0x83, 40);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0x12, 13);
            this.label8.TabIndex = 0x13;
            this.label8.Text = "0x";
            this.label6.AutoSize = true;
            this.label6.Location = new Point(5, 0x66);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x59, 13);
            this.label6.TabIndex = 0x27;
            this.label6.Text = "Max Msg Length:";
            this.label7.AutoSize = true;
            this.label7.Location = new Point(0x83, 20);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x12, 13);
            this.label7.TabIndex = 0x13;
            this.label7.Text = "0x";
            this.frmTrackerConfigMasterAddrTextBox.Location = new Point(0x98, 0x10);
            this.frmTrackerConfigMasterAddrTextBox.Name = "frmTrackerConfigMasterAddrTextBox";
            this.frmTrackerConfigMasterAddrTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigMasterAddrTextBox.TabIndex = 0;
            this.frmTrackerConfigMasterAddrTextBox.Validating += new CancelEventHandler(this.frmTrackerConfigMasterAddrTextBox_Validating);
            this.frmTrackerConfigModeLabel.AutoSize = true;
            this.frmTrackerConfigModeLabel.Location = new Point(6, 60);
            this.frmTrackerConfigModeLabel.Name = "frmTrackerConfigModeLabel";
            this.frmTrackerConfigModeLabel.Size = new Size(0x25, 13);
            this.frmTrackerConfigModeLabel.TabIndex = 14;
            this.frmTrackerConfigModeLabel.Text = "Mode:";
            this.frmTrackerConfigRateLabel.AutoSize = true;
            this.frmTrackerConfigRateLabel.Location = new Point(6, 0x51);
            this.frmTrackerConfigRateLabel.Name = "frmTrackerConfigRateLabel";
            this.frmTrackerConfigRateLabel.Size = new Size(0x21, 13);
            this.frmTrackerConfigRateLabel.TabIndex = 12;
            this.frmTrackerConfigRateLabel.Text = "Rate:";
            this.frmTrackerConfigSlaveAddrTextBox.Location = new Point(0x98, 0x24);
            this.frmTrackerConfigSlaveAddrTextBox.Name = "frmTrackerConfigSlaveAddrTextBox";
            this.frmTrackerConfigSlaveAddrTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigSlaveAddrTextBox.TabIndex = 1;
            this.frmTrackerConfigSlaveAddrTextBox.Validating += new CancelEventHandler(this.frmTrackerConfigSlaveAddrTextBox_Validating);
            this.frmTrackerConfigSlaveAddrLabel.AutoSize = true;
            this.frmTrackerConfigSlaveAddrLabel.Location = new Point(5, 40);
            this.frmTrackerConfigSlaveAddrLabel.Name = "frmTrackerConfigSlaveAddrLabel";
            this.frmTrackerConfigSlaveAddrLabel.Size = new Size(0x79, 13);
            this.frmTrackerConfigSlaveAddrLabel.TabIndex = 10;
            this.frmTrackerConfigSlaveAddrLabel.Text = "Slave Address(Tracker):";
            this.frmTrackerConfigI2CModeComboBox.FormattingEnabled = true;
            this.frmTrackerConfigI2CModeComboBox.Items.AddRange(new object[] { "Slave", "Multi-Master" });
            this.frmTrackerConfigI2CModeComboBox.Location = new Point(0x98, 0x38);
            this.frmTrackerConfigI2CModeComboBox.Name = "frmTrackerConfigI2CModeComboBox";
            this.frmTrackerConfigI2CModeComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigI2CModeComboBox.TabIndex = 2;
            this.frmTrackerConfigI2CRateComboBox.FormattingEnabled = true;
            this.frmTrackerConfigI2CRateComboBox.Items.AddRange(new object[] { "100 Kbps", "400 Kbps", "1 Mbps (not available for 4t/4e)", "3.4 Mbps (not available for 4t/4e)" });
            this.frmTrackerConfigI2CRateComboBox.Location = new Point(0x98, 0x4d);
            this.frmTrackerConfigI2CRateComboBox.Name = "frmTrackerConfigI2CRateComboBox";
            this.frmTrackerConfigI2CRateComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigI2CRateComboBox.TabIndex = 3;
            this.frmTrackerConfigI2CRateComboBox.Validating += new CancelEventHandler(this.frmTrackerConfigI2CRateComboBox_Validating);
            this.frmTrackerConfigMasterAddrLabel.AutoSize = true;
            this.frmTrackerConfigMasterAddrLabel.Location = new Point(6, 20);
            this.frmTrackerConfigMasterAddrLabel.Name = "frmTrackerConfigMasterAddrLabel";
            this.frmTrackerConfigMasterAddrLabel.Size = new Size(0x6f, 13);
            this.frmTrackerConfigMasterAddrLabel.TabIndex = 8;
            this.frmTrackerConfigMasterAddrLabel.Text = "Master Address(Host):";
            this.groupBox4.Controls.Add(this.frmTrackerConfigFlowCtrlComboBox);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupMesgCntTextBox);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupMesgCntLabel);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupPatternTextBox);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupPatternLabel);
            this.groupBox4.Controls.Add(this.frmTrackerConfigFlowControlLabel);
            this.groupBox4.Controls.Add(this.frmTrackerConfigBaudComboBox);
            this.groupBox4.Controls.Add(this.frmTrackerConfigBaudLabel);
            this.groupBox4.Location = new Point(0x11, 0x11c);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new Size(0xfc, 0x81);
            this.groupBox4.TabIndex = 0x21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "UART Config";
            this.frmTrackerConfigFlowCtrlComboBox.FormattingEnabled = true;
            this.frmTrackerConfigFlowCtrlComboBox.Items.AddRange(new object[] { "Disable", "Enable" });
            this.frmTrackerConfigFlowCtrlComboBox.Location = new Point(0x86, 0x24);
            this.frmTrackerConfigFlowCtrlComboBox.Name = "frmTrackerConfigFlowCtrlComboBox";
            this.frmTrackerConfigFlowCtrlComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigFlowCtrlComboBox.TabIndex = 1;
            this.frmTrackerConfigWakeupMesgCntTextBox.Location = new Point(0x86, 0x4d);
            this.frmTrackerConfigWakeupMesgCntTextBox.Name = "frmTrackerConfigWakeupMesgCntTextBox";
            this.frmTrackerConfigWakeupMesgCntTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigWakeupMesgCntTextBox.TabIndex = 3;
            this.frmTrackerConfigWakeupMesgCntTextBox.Validating += new CancelEventHandler(this.frmTrackerConfigWakeupMesgCntTextBox_Validating);
            this.frmTrackerConfigWakeupMesgCntLabel.AutoSize = true;
            this.frmTrackerConfigWakeupMesgCntLabel.Location = new Point(6, 0x51);
            this.frmTrackerConfigWakeupMesgCntLabel.Name = "frmTrackerConfigWakeupMesgCntLabel";
            this.frmTrackerConfigWakeupMesgCntLabel.Size = new Size(0x7d, 13);
            this.frmTrackerConfigWakeupMesgCntLabel.TabIndex = 14;
            this.frmTrackerConfigWakeupMesgCntLabel.Text = "Idle Byte Wakeup Delay:";
            this.frmTrackerConfigWakeupPatternTextBox.Location = new Point(0x86, 0x38);
            this.frmTrackerConfigWakeupPatternTextBox.Name = "frmTrackerConfigWakeupPatternTextBox";
            this.frmTrackerConfigWakeupPatternTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigWakeupPatternTextBox.TabIndex = 2;
            this.frmTrackerConfigWakeupPatternTextBox.Validating += new CancelEventHandler(this.frmTrackerConfigWakeupPatternTextBox_Validating);
            this.frmTrackerConfigWakeupPatternLabel.AutoSize = true;
            this.frmTrackerConfigWakeupPatternLabel.Location = new Point(5, 60);
            this.frmTrackerConfigWakeupPatternLabel.Name = "frmTrackerConfigWakeupPatternLabel";
            this.frmTrackerConfigWakeupPatternLabel.Size = new Size(0x7e, 13);
            this.frmTrackerConfigWakeupPatternLabel.TabIndex = 12;
            this.frmTrackerConfigWakeupPatternLabel.Text = "Wake Up Max Preamble:";
            this.frmTrackerConfigFlowControlLabel.AutoSize = true;
            this.frmTrackerConfigFlowControlLabel.Location = new Point(6, 40);
            this.frmTrackerConfigFlowControlLabel.Name = "frmTrackerConfigFlowControlLabel";
            this.frmTrackerConfigFlowControlLabel.Size = new Size(90, 13);
            this.frmTrackerConfigFlowControlLabel.TabIndex = 10;
            this.frmTrackerConfigFlowControlLabel.Text = "HW Flow Control:";
            this.frmTrackerConfigBaudComboBox.FormattingEnabled = true;
            this.frmTrackerConfigBaudComboBox.Items.AddRange(new object[] { 
                "900", "1200", "1800", "2400", "3600", "4800", "7200", "9600", "14400", "19200", "28800", "38400", "57600", "76800", "115200", "153600", 
                "230400", "307200", "460800", "614400", "921600", "1228800", "1843200"
             });
            this.frmTrackerConfigBaudComboBox.Location = new Point(0x86, 0x10);
            this.frmTrackerConfigBaudComboBox.Name = "frmTrackerConfigBaudComboBox";
            this.frmTrackerConfigBaudComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigBaudComboBox.TabIndex = 0;
            this.frmTrackerConfigBaudLabel.AutoSize = true;
            this.frmTrackerConfigBaudLabel.Location = new Point(6, 20);
            this.frmTrackerConfigBaudLabel.Name = "frmTrackerConfigBaudLabel";
            this.frmTrackerConfigBaudLabel.Size = new Size(0x3d, 13);
            this.frmTrackerConfigBaudLabel.TabIndex = 8;
            this.frmTrackerConfigBaudLabel.Text = "Baud Rate:";
            this.groupBox2.Controls.Add(this.groupBox7);
            this.groupBox2.Controls.Add(this.label34);
            this.groupBox2.Controls.Add(this.label33);
            this.groupBox2.Controls.Add(this.label32);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Controls.Add(this.label30);
            this.groupBox2.Controls.Add(this.label29);
            this.groupBox2.Controls.Add(this.label28);
            this.groupBox2.Controls.Add(this.label27);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.textBox_GPIO10);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.textBox_GPIO9);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.textBox_GPIO8);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.textBox_GPIO7);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.textBox_GPIO4);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.textBox_GPIO6);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.textBox_GPIO3);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.textBox_GPIO5);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.textBox_GPIO2);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textBox_GPIO1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_GPIO0);
            this.groupBox2.Controls.Add(this.frmTrackerConfigIOPinConfigLabel);
            this.groupBox2.Controls.Add(this.frmTrackerConfigIOPinConfigEnableComboBox);
            this.groupBox2.Controls.Add(this.frmTrackerConfigIOPinConfigEnableLabel);
            this.groupBox2.Location = new Point(0x11, 0x1a3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(0x214, 0xcd);
            this.groupBox2.TabIndex = 0x1f;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "IO Pin Config";
            this.groupBox7.Controls.Add(this.label38);
            this.groupBox7.Controls.Add(this.label37);
            this.groupBox7.Controls.Add(this.label36);
            this.groupBox7.Controls.Add(this.label35);
            this.groupBox7.Controls.Add(this.ioPortType_comboBox);
            this.groupBox7.Controls.Add(this.basicRxConfig_comboBox);
            this.groupBox7.Location = new Point(0x108, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new Size(0xfc, 0x51);
            this.groupBox7.TabIndex = 0x3d;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Predefined Settings";
            this.label36.AutoSize = true;
            this.label36.Location = new Point(0x22, 0x12);
            this.label36.Name = "label36";
            this.label36.Size = new Size(0x52, 13);
            this.label36.TabIndex = 3;
            this.label36.Text = "Basic Rx Config";
            this.label35.AutoSize = true;
            this.label35.Location = new Point(0x9b, 0x12);
            this.label35.Name = "label35";
            this.label35.Size = new Size(0x48, 13);
            this.label35.TabIndex = 2;
            this.label35.Text = "I/O Port Type";
            this.ioPortType_comboBox.FormattingEnabled = true;
            this.ioPortType_comboBox.Items.AddRange(new object[] { "None", "UART", "S-SPI", "I2C" });
            this.ioPortType_comboBox.Location = new Point(0x90, 0x23);
            this.ioPortType_comboBox.Name = "ioPortType_comboBox";
            this.ioPortType_comboBox.Size = new Size(0x5e, 0x15);
            this.ioPortType_comboBox.TabIndex = 1;
            this.ioPortType_comboBox.SelectedIndexChanged += new EventHandler(this.ioPortType_comboBox_SelectedIndexChanged);
            this.basicRxConfig_comboBox.FormattingEnabled = true;
            this.basicRxConfig_comboBox.Items.AddRange(new object[] { "None", "Normal PVT", "PVT + MEMS", "DR Automotive" });
            this.basicRxConfig_comboBox.Location = new Point(15, 0x23);
            this.basicRxConfig_comboBox.Name = "basicRxConfig_comboBox";
            this.basicRxConfig_comboBox.Size = new Size(0x79, 0x15);
            this.basicRxConfig_comboBox.TabIndex = 0;
            this.basicRxConfig_comboBox.SelectedIndexChanged += new EventHandler(this.basicRxConfig_comboBox_SelectedIndexChanged);
            this.label34.AutoSize = true;
            this.label34.Location = new Point(0x170, 0xb7);
            this.label34.Name = "label34";
            this.label34.Size = new Size(0x12, 13);
            this.label34.TabIndex = 60;
            this.label34.Text = "0x";
            this.label33.AutoSize = true;
            this.label33.Location = new Point(0x170, 0xa3);
            this.label33.Name = "label33";
            this.label33.Size = new Size(0x12, 13);
            this.label33.TabIndex = 0x3b;
            this.label33.Text = "0x";
            this.label32.AutoSize = true;
            this.label32.Location = new Point(0x170, 0x8f);
            this.label32.Name = "label32";
            this.label32.Size = new Size(0x12, 13);
            this.label32.TabIndex = 0x3a;
            this.label32.Text = "0x";
            this.label31.AutoSize = true;
            this.label31.Location = new Point(0x170, 0x7b);
            this.label31.Name = "label31";
            this.label31.Size = new Size(0x12, 13);
            this.label31.TabIndex = 0x39;
            this.label31.Text = "0x";
            this.label30.AutoSize = true;
            this.label30.Location = new Point(0x170, 0x67);
            this.label30.Name = "label30";
            this.label30.Size = new Size(0x12, 13);
            this.label30.TabIndex = 0x38;
            this.label30.Text = "0x";
            this.label29.AutoSize = true;
            this.label29.Location = new Point(0x7f, 0xb7);
            this.label29.Name = "label29";
            this.label29.Size = new Size(0x12, 13);
            this.label29.TabIndex = 0x37;
            this.label29.Text = "0x";
            this.label28.AutoSize = true;
            this.label28.Location = new Point(0x7f, 0xa3);
            this.label28.Name = "label28";
            this.label28.Size = new Size(0x12, 13);
            this.label28.TabIndex = 0x36;
            this.label28.Text = "0x";
            this.label27.AutoSize = true;
            this.label27.Location = new Point(0x7f, 0x8f);
            this.label27.Name = "label27";
            this.label27.Size = new Size(0x12, 13);
            this.label27.TabIndex = 0x35;
            this.label27.Text = "0x";
            this.label23.AutoSize = true;
            this.label23.Location = new Point(0x7f, 0x7b);
            this.label23.Name = "label23";
            this.label23.Size = new Size(0x12, 13);
            this.label23.TabIndex = 0x34;
            this.label23.Text = "0x";
            this.label22.AutoSize = true;
            this.label22.Location = new Point(0x7f, 0x67);
            this.label22.Name = "label22";
            this.label22.Size = new Size(0x12, 13);
            this.label22.TabIndex = 0x33;
            this.label22.Text = "0x";
            this.label9.AutoSize = true;
            this.label9.Location = new Point(0x7f, 0x53);
            this.label9.Name = "label9";
            this.label9.Size = new Size(0x12, 13);
            this.label9.TabIndex = 50;
            this.label9.Text = "0x";
            this.textBox_GPIO10.Location = new Point(0x185, 0xb3);
            this.textBox_GPIO10.Name = "textBox_GPIO10";
            this.textBox_GPIO10.Size = new Size(100, 20);
            this.textBox_GPIO10.TabIndex = 0x30;
            this.textBox_GPIO10.Validating += new CancelEventHandler(this.textBox_GPIO10_Validating);
            this.label24.AutoSize = true;
            this.label24.Location = new Point(0x10a, 0xb7);
            this.label24.Name = "label24";
            this.label24.Size = new Size(80, 13);
            this.label24.TabIndex = 0x31;
            this.label24.Text = "GPIO-8 (EIT 2):";
            this.textBox_GPIO9.Location = new Point(0x185, 0x9f);
            this.textBox_GPIO9.Name = "textBox_GPIO9";
            this.textBox_GPIO9.Size = new Size(100, 20);
            this.textBox_GPIO9.TabIndex = 0x2e;
            this.textBox_GPIO9.Validating += new CancelEventHandler(this.textBox_GPIO9_Validating);
            this.label20.AutoSize = true;
            this.label20.Location = new Point(0x10a, 0xa3);
            this.label20.Name = "label20";
            this.label20.Size = new Size(0x18, 13);
            this.label20.TabIndex = 0x2f;
            this.label20.Text = "TX:";
            this.textBox_GPIO8.Location = new Point(0x185, 0x8b);
            this.textBox_GPIO8.Name = "textBox_GPIO8";
            this.textBox_GPIO8.Size = new Size(100, 20);
            this.textBox_GPIO8.TabIndex = 0x2b;
            this.textBox_GPIO8.Validating += new CancelEventHandler(this.textBox_GPIO8_Validating);
            this.label17.AutoSize = true;
            this.label17.Location = new Point(0x10a, 0x8f);
            this.label17.Name = "label17";
            this.label17.Size = new Size(0x19, 13);
            this.label17.TabIndex = 0x2d;
            this.label17.Text = "RX:";
            this.textBox_GPIO7.Location = new Point(0x185, 0x77);
            this.textBox_GPIO7.Name = "textBox_GPIO7";
            this.textBox_GPIO7.Size = new Size(100, 20);
            this.textBox_GPIO7.TabIndex = 0x27;
            this.textBox_GPIO7.Validating += new CancelEventHandler(this.textBox_GPIO7_Validating);
            this.label16.AutoSize = true;
            this.label16.Location = new Point(0x10a, 0x7b);
            this.label16.Name = "label16";
            this.label16.Size = new Size(90, 13);
            this.label16.TabIndex = 40;
            this.label16.Text = "GPIO-7 (RTS_N):";
            this.textBox_GPIO4.Location = new Point(0x94, 0x9f);
            this.textBox_GPIO4.Name = "textBox_GPIO4";
            this.textBox_GPIO4.Size = new Size(100, 20);
            this.textBox_GPIO4.TabIndex = 0x29;
            this.textBox_GPIO4.Validating += new CancelEventHandler(this.textBox_GPIO4_Validating);
            this.label18.AutoSize = true;
            this.label18.Location = new Point(6, 0xa3);
            this.label18.Name = "label18";
            this.label18.Size = new Size(0x70, 13);
            this.label18.TabIndex = 0x2e;
            this.label18.Text = "GPIO-4 (EIT/RF_ON):";
            this.textBox_GPIO6.Location = new Point(0x185, 0x63);
            this.textBox_GPIO6.Name = "textBox_GPIO6";
            this.textBox_GPIO6.Size = new Size(100, 20);
            this.textBox_GPIO6.TabIndex = 0x27;
            this.textBox_GPIO6.Validating += new CancelEventHandler(this.textBox_GPIO6_Validating);
            this.label15.AutoSize = true;
            this.label15.Location = new Point(0x10a, 0x67);
            this.label15.Name = "label15";
            this.label15.Size = new Size(0x59, 13);
            this.label15.TabIndex = 40;
            this.label15.Text = "GPIO-6 (CTS_N):";
            this.textBox_GPIO3.Location = new Point(0x94, 0x8b);
            this.textBox_GPIO3.Name = "textBox_GPIO3";
            this.textBox_GPIO3.Size = new Size(100, 20);
            this.textBox_GPIO3.TabIndex = 0x2a;
            this.textBox_GPIO3.Validating += new CancelEventHandler(this.textBox_GPIO3_Validating);
            this.label19.AutoSize = true;
            this.label19.Location = new Point(6, 0x8f);
            this.label19.Name = "label19";
            this.label19.Size = new Size(0x51, 13);
            this.label19.TabIndex = 0x2c;
            this.label19.Text = "GPIO-3 (ECLK):";
            this.textBox_GPIO5.Location = new Point(0x94, 0xb3);
            this.textBox_GPIO5.Name = "textBox_GPIO5";
            this.textBox_GPIO5.Size = new Size(100, 20);
            this.textBox_GPIO5.TabIndex = 0x27;
            this.textBox_GPIO5.Validating += new CancelEventHandler(this.textBox_GPIO5_Validating);
            this.label14.AutoSize = true;
            this.label14.Location = new Point(6, 0xb7);
            this.label14.Name = "label14";
            this.label14.Size = new Size(70, 13);
            this.label14.TabIndex = 40;
            this.label14.Text = "GPIO-5 (TM):";
            this.textBox_GPIO2.Location = new Point(0x94, 0x77);
            this.textBox_GPIO2.Name = "textBox_GPIO2";
            this.textBox_GPIO2.Size = new Size(100, 20);
            this.textBox_GPIO2.TabIndex = 0x27;
            this.textBox_GPIO2.Validating += new CancelEventHandler(this.textBox_GPIO2_Validating);
            this.label13.AutoSize = true;
            this.label13.Location = new Point(6, 0x7b);
            this.label13.Name = "label13";
            this.label13.Size = new Size(90, 13);
            this.label13.TabIndex = 40;
            this.label13.Text = "GPIO-2 (TSYNC):";
            this.textBox_GPIO1.Location = new Point(0x94, 0x63);
            this.textBox_GPIO1.Name = "textBox_GPIO1";
            this.textBox_GPIO1.Size = new Size(100, 20);
            this.textBox_GPIO1.TabIndex = 0x27;
            this.textBox_GPIO1.Validating += new CancelEventHandler(this.textBox_GPIO1_Validating);
            this.label5.AutoSize = true;
            this.label5.Location = new Point(6, 0x67);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x73, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "GPIO-1 (FT_I2C_CLK):";
            this.textBox_GPIO0.Location = new Point(0x94, 0x4f);
            this.textBox_GPIO0.Name = "textBox_GPIO0";
            this.textBox_GPIO0.Size = new Size(100, 20);
            this.textBox_GPIO0.TabIndex = 1;
            this.textBox_GPIO0.Validating += new CancelEventHandler(this.textBox_GPIO0_Validating);
            this.frmTrackerConfigIOPinConfigLabel.AutoSize = true;
            this.frmTrackerConfigIOPinConfigLabel.Location = new Point(6, 0x53);
            this.frmTrackerConfigIOPinConfigLabel.Name = "frmTrackerConfigIOPinConfigLabel";
            this.frmTrackerConfigIOPinConfigLabel.Size = new Size(0x75, 13);
            this.frmTrackerConfigIOPinConfigLabel.TabIndex = 6;
            this.frmTrackerConfigIOPinConfigLabel.Text = "GPIO-0 (DR_I2C_DIO):";
            this.frmTrackerConfigIOPinConfigEnableComboBox.FormattingEnabled = true;
            this.frmTrackerConfigIOPinConfigEnableComboBox.Items.AddRange(new object[] { "Disable", "Enable" });
            this.frmTrackerConfigIOPinConfigEnableComboBox.Location = new Point(0x86, 20);
            this.frmTrackerConfigIOPinConfigEnableComboBox.Name = "frmTrackerConfigIOPinConfigEnableComboBox";
            this.frmTrackerConfigIOPinConfigEnableComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigIOPinConfigEnableComboBox.TabIndex = 0;
            this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndexChanged += new EventHandler(this.frmTrackerConfigIOPinConfigEnableComboBox_SelectedIndexChanged);
            this.frmTrackerConfigIOPinConfigEnableLabel.AutoSize = true;
            this.frmTrackerConfigIOPinConfigEnableLabel.Location = new Point(6, 0x18);
            this.frmTrackerConfigIOPinConfigEnableLabel.Name = "frmTrackerConfigIOPinConfigEnableLabel";
            this.frmTrackerConfigIOPinConfigEnableLabel.Size = new Size(0x69, 13);
            this.frmTrackerConfigIOPinConfigEnableLabel.TabIndex = 4;
            this.frmTrackerConfigIOPinConfigEnableLabel.Text = "IO Pin Config Enable";
            this.frmTrackerConfigLNASelectComboBox.FormattingEnabled = true;
            this.frmTrackerConfigLNASelectComboBox.Items.AddRange(new object[] { "High", "Low" });
            this.frmTrackerConfigLNASelectComboBox.Location = new Point(0x97, 0x81);
            this.frmTrackerConfigLNASelectComboBox.Name = "frmTrackerConfigLNASelectComboBox";
            this.frmTrackerConfigLNASelectComboBox.Size = new Size(0x76, 0x15);
            this.frmTrackerConfigLNASelectComboBox.TabIndex = 30;
            this.frmTrackerConfigLNASelectLabel.AutoSize = true;
            this.frmTrackerConfigLNASelectLabel.Location = new Point(0x13, 0x85);
            this.frmTrackerConfigLNASelectLabel.Name = "frmTrackerConfigLNASelectLabel";
            this.frmTrackerConfigLNASelectLabel.Size = new Size(0x56, 13);
            this.frmTrackerConfigLNASelectLabel.TabIndex = 0x20;
            this.frmTrackerConfigLNASelectLabel.Text = "LNA Gain Mode:";
            this.groupBox1.Controls.Add(this.comboBox_RefClkOffset);
            this.groupBox1.Controls.Add(this.comboBox_RefClkUncertainty);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.frmTrackerConfigUncertaintyLable);
            this.groupBox1.Controls.Add(this.textBox_RefClkWarmupDelay);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new Point(0x119, 0xbd);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x10c, 0x59);
            this.groupBox1.TabIndex = 0x19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ref Clock ";
            this.comboBox_RefClkOffset.FormattingEnabled = true;
            this.comboBox_RefClkOffset.Items.AddRange(new object[] { "Unknown - use default" });
            this.comboBox_RefClkOffset.Location = new Point(0x59, 0x3a);
            this.comboBox_RefClkOffset.Name = "comboBox_RefClkOffset";
            this.comboBox_RefClkOffset.Size = new Size(0x8f, 0x15);
            this.comboBox_RefClkOffset.TabIndex = 0x2a;
            this.comboBox_RefClkOffset.Validating += new CancelEventHandler(this.comboBox_RefClkOffset_Validating);
            this.comboBox_RefClkUncertainty.FormattingEnabled = true;
            this.comboBox_RefClkUncertainty.Items.AddRange(new object[] { "Unknown - use default" });
            this.comboBox_RefClkUncertainty.Location = new Point(0x59, 0x26);
            this.comboBox_RefClkUncertainty.Name = "comboBox_RefClkUncertainty";
            this.comboBox_RefClkUncertainty.Size = new Size(0x8f, 0x15);
            this.comboBox_RefClkUncertainty.TabIndex = 0x29;
            this.comboBox_RefClkUncertainty.Validating += new CancelEventHandler(this.comboBox_RefClkUncertainty_Validating);
            this.label21.AutoSize = true;
            this.label21.Location = new Point(0xeb, 0x3e);
            this.label21.Name = "label21";
            this.label21.Size = new Size(0x1b, 13);
            this.label21.TabIndex = 40;
            this.label21.Text = "ppm";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0xeb, 0x2a);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x19, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "ppb";
            this.label12.AutoSize = true;
            this.label12.Location = new Point(7, 0x3e);
            this.label12.Name = "label12";
            this.label12.Size = new Size(0x26, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Offset:";
            this.frmTrackerConfigUncertaintyLable.AutoSize = true;
            this.frmTrackerConfigUncertaintyLable.Location = new Point(7, 0x2a);
            this.frmTrackerConfigUncertaintyLable.Name = "frmTrackerConfigUncertaintyLable";
            this.frmTrackerConfigUncertaintyLable.Size = new Size(0x40, 13);
            this.frmTrackerConfigUncertaintyLable.TabIndex = 2;
            this.frmTrackerConfigUncertaintyLable.Text = "Uncertainty:";
            this.textBox_RefClkWarmupDelay.Location = new Point(0x59, 0x12);
            this.textBox_RefClkWarmupDelay.Name = "textBox_RefClkWarmupDelay";
            this.textBox_RefClkWarmupDelay.Size = new Size(0x55, 20);
            this.textBox_RefClkWarmupDelay.TabIndex = 0x18;
            this.textBox_RefClkWarmupDelay.Validating += new CancelEventHandler(this.textBox_RefClkWarmupDelay_Validating);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0xb2, 0x15);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x3e, 13);
            this.label3.TabIndex = 0x1a;
            this.label3.Text = "RTC cycles";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(7, 0x16);
            this.label2.Name = "label2";
            this.label2.Size = new Size(80, 13);
            this.label2.TabIndex = 0x1c;
            this.label2.Text = "Warmup Delay:";
            this.frmTrackerICConfigRefFreqLabel.AutoSize = true;
            this.frmTrackerICConfigRefFreqLabel.Location = new Point(0x13, 0x1d);
            this.frmTrackerICConfigRefFreqLabel.Name = "frmTrackerICConfigRefFreqLabel";
            this.frmTrackerICConfigRefFreqLabel.Size = new Size(0x65, 13);
            this.frmTrackerICConfigRefFreqLabel.TabIndex = 0x17;
            this.frmTrackerICConfigRefFreqLabel.Text = "Ref Clk. Frequency:";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x10f, 0x1d);
            this.label1.Name = "label1";
            this.label1.Size = new Size(20, 13);
            this.label1.TabIndex = 0x1b;
            this.label1.Text = "Hz";
            this.label11.AutoSize = true;
            this.label11.Location = new Point(0x13, 0x9b);
            this.label11.Name = "label11";
            this.label11.Size = new Size(0x69, 13);
            this.label11.TabIndex = 0x29;
            this.label11.Text = "Power Supply Mode:";
            this.comboBox_LDOEnable.FormattingEnabled = true;
            this.comboBox_LDOEnable.Items.AddRange(new object[] { "Switching regulator", "Internal LDO" });
            this.comboBox_LDOEnable.Location = new Point(0x97, 0x97);
            this.comboBox_LDOEnable.Name = "comboBox_LDOEnable";
            this.comboBox_LDOEnable.Size = new Size(0x76, 0x15);
            this.comboBox_LDOEnable.TabIndex = 40;
            this.button_Advanced.Location = new Point(0x133, 0x18);
            this.button_Advanced.Name = "button_Advanced";
            this.button_Advanced.Size = new Size(0x4b, 0x17);
            this.button_Advanced.TabIndex = 0x2b;
            this.button_Advanced.Text = "&Advanced...";
            this.button_Advanced.UseVisualStyleBackColor = true;
            this.button_Advanced.Click += new EventHandler(this.button_Advanced_Click);
            this.newConfigCheckBox.AutoSize = true;
            this.newConfigCheckBox.Checked = true;
            this.newConfigCheckBox.CheckState = CheckState.Checked;
            this.newConfigCheckBox.Location = new Point(0x17, 0x35);
            this.newConfigCheckBox.Name = "newConfigCheckBox";
            this.newConfigCheckBox.Size = new Size(0x77, 0x11);
            this.newConfigCheckBox.TabIndex = 0x2c;
            this.newConfigCheckBox.Text = "New Configuration?";
            this.newConfigCheckBox.UseVisualStyleBackColor = true;
            this.newConfigCheckBox.CheckedChanged += new EventHandler(this.newConfigCheckBox_CheckedChanged);
            this.newConfigDetailLabel.AutoSize = true;
            this.newConfigDetailLabel.Location = new Point(0x10, 70);
            this.newConfigDetailLabel.Name = "newConfigDetailLabel";
            this.newConfigDetailLabel.Size = new Size(410, 13);
            this.newConfigDetailLabel.TabIndex = 0x2d;
            this.newConfigDetailLabel.Text = " (22 bytes IO Pin Config. Applicable for Version GSD4t >= 4.1.0 and GSD4e >= 4.0.1) ";
            this.newConfigDetailLabel.TextAlign = ContentAlignment.TopCenter;
            this.refClkFreqComboBox.FormattingEnabled = true;
            this.refClkFreqComboBox.Location = new Point(0xa9, 0x19);
            this.refClkFreqComboBox.Name = "refClkFreqComboBox";
            this.refClkFreqComboBox.Size = new Size(100, 0x15);
            this.refClkFreqComboBox.TabIndex = 0x2e;
            this.groupBox3.Controls.Add(this.comboBox_PowCtrlOnOff_Offtype);
            this.groupBox3.Controls.Add(this.comboBox_PowCtrlOnOff_Usagetype);
            this.groupBox3.Controls.Add(this.comboBox_PowCtrlOnOff_Edgetype);
            this.groupBox3.Controls.Add(this.label26);
            this.groupBox3.Controls.Add(this.label25);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new Point(0x11, 0xbd);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(0xfc, 0x59);
            this.groupBox3.TabIndex = 0x2f;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Power Control on/off";
            this.comboBox_PowCtrlOnOff_Offtype.FormattingEnabled = true;
            this.comboBox_PowCtrlOnOff_Offtype.Items.AddRange(new object[] { "OFF disabled", "OFF enabled" });
            this.comboBox_PowCtrlOnOff_Offtype.Location = new Point(0x86, 0x3a);
            this.comboBox_PowCtrlOnOff_Offtype.Name = "comboBox_PowCtrlOnOff_Offtype";
            this.comboBox_PowCtrlOnOff_Offtype.Size = new Size(0x70, 0x15);
            this.comboBox_PowCtrlOnOff_Offtype.TabIndex = 5;
            this.comboBox_PowCtrlOnOff_Usagetype.FormattingEnabled = true;
            this.comboBox_PowCtrlOnOff_Usagetype.Items.AddRange(new object[] { "No On/Off used", "GPIO controlled On/Off", "UartA Rx controlled On/Off", "UartB CTS controlled On/Off" });
            this.comboBox_PowCtrlOnOff_Usagetype.Location = new Point(70, 0x26);
            this.comboBox_PowCtrlOnOff_Usagetype.Name = "comboBox_PowCtrlOnOff_Usagetype";
            this.comboBox_PowCtrlOnOff_Usagetype.Size = new Size(0xb0, 0x15);
            this.comboBox_PowCtrlOnOff_Usagetype.TabIndex = 4;
            this.comboBox_PowCtrlOnOff_Edgetype.FormattingEnabled = true;
            this.comboBox_PowCtrlOnOff_Edgetype.Items.AddRange(new object[] { "On/Off disabled/not detected", "Edge: Falling On/Off IRQ", "Edge: Rising On/Off IRQ", "Edge: Rising On, Falling Off IRQ", "Edge: Falling On, Rising Off IRQ" });
            this.comboBox_PowCtrlOnOff_Edgetype.Location = new Point(70, 0x12);
            this.comboBox_PowCtrlOnOff_Edgetype.Name = "comboBox_PowCtrlOnOff_Edgetype";
            this.comboBox_PowCtrlOnOff_Edgetype.Size = new Size(0xb0, 0x15);
            this.comboBox_PowCtrlOnOff_Edgetype.TabIndex = 3;
            this.label26.AutoSize = true;
            this.label26.Location = new Point(6, 0x3e);
            this.label26.Name = "label26";
            this.label26.Size = new Size(0x35, 13);
            this.label26.TabIndex = 2;
            this.label26.Text = "OFF type:";
            this.label25.AutoSize = true;
            this.label25.Location = new Point(6, 0x2a);
            this.label25.Name = "label25";
            this.label25.Size = new Size(0x40, 13);
            this.label25.TabIndex = 1;
            this.label25.Text = "Usage type:";
            this.label10.AutoSize = true;
            this.label10.Location = new Point(6, 0x16);
            this.label10.Name = "label10";
            this.label10.Size = new Size(0x3a, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Edge type:";
            this.PollICConfigBtn.Location = new Point(0x8a, 0x15);
            this.PollICConfigBtn.Name = "PollICConfigBtn";
            this.PollICConfigBtn.Size = new Size(0x4b, 0x17);
            this.PollICConfigBtn.TabIndex = 0x30;
            this.PollICConfigBtn.Text = "&Poll";
            this.PollICConfigBtn.UseVisualStyleBackColor = true;
            this.PollICConfigBtn.Visible = false;
            this.PollICConfigBtn.Click += new EventHandler(this.PollICConfigBtn_Click);
            this.groupBox6.Controls.Add(this.button_Default);
            this.groupBox6.Controls.Add(this.PollICConfigBtn);
            this.groupBox6.Location = new Point(0x119, 0x79);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new Size(0x10c, 0x38);
            this.groupBox6.TabIndex = 0x31;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Settings";
            this.label37.AutoSize = true;
            this.label37.Location = new Point(0x2d, 60);
            this.label37.Name = "label37";
            this.label37.Size = new Size(60, 13);
            this.label37.TabIndex = 4;
            this.label37.Text = "GPIO 0-5,8";
            this.label38.AutoSize = true;
            this.label38.Location = new Point(0x94, 0x3b);
            this.label38.Name = "label38";
            this.label38.Size = new Size(0x56, 13);
            this.label38.TabIndex = 5;
            this.label38.Text = "GPIO 6-7,RX,TX";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = AutoValidate.EnablePreventFocusChange;
            base.ClientSize = new Size(0x237, 0x281);
            base.Controls.Add(this.groupBox6);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.refClkFreqComboBox);
            base.Controls.Add(this.newConfigDetailLabel);
            base.Controls.Add(this.newConfigCheckBox);
            base.Controls.Add(this.button_Advanced);
            base.Controls.Add(this.label11);
            base.Controls.Add(this.comboBox_LDOEnable);
            base.Controls.Add(this.frmTrackerConfigCancelBtn);
            base.Controls.Add(this.frmTrackerConfigOkBtn);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.frmTrackerICConfigRefFreqLabel);
            base.Controls.Add(this.groupBox5);
            base.Controls.Add(this.groupBox4);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.frmTrackerConfigLNASelectComboBox);
            base.Controls.Add(this.frmTrackerConfigLNASelectLabel);
            base.Controls.Add(this.groupBox1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmTrackerICConfig_Ver2";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Rx IC Config";
            base.FormClosing += new FormClosingEventHandler(this.frmTrackerICConfig_Ver2_FormClosing);
            base.Load += new EventHandler(this.frmTrackerICConfig_Ver2_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void ioPortType_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.ioPortType_comboBox.SelectedIndex)
            {
                case 0:
                    this.textBox_GPIO6.Text = "0";
                    this.textBox_GPIO7.Text = "0";
                    this.textBox_GPIO8.Text = "0";
                    this.textBox_GPIO9.Text = "0";
                    break;

                case 1:
                    this.textBox_GPIO6.Text = "3C4";
                    this.textBox_GPIO7.Text = "3C4";
                    this.textBox_GPIO8.Text = "3C4";
                    this.textBox_GPIO9.Text = "3C4";
                    break;

                case 2:
                    this.textBox_GPIO6.Text = "5";
                    this.textBox_GPIO7.Text = "3C5";
                    this.textBox_GPIO8.Text = "3C5";
                    this.textBox_GPIO9.Text = "3C5";
                    break;

                case 3:
                    this.textBox_GPIO6.Text = "0";
                    this.textBox_GPIO7.Text = "0";
                    this.textBox_GPIO8.Text = "3C6";
                    this.textBox_GPIO9.Text = "3C6";
                    break;
            }
            this.Refresh();
        }

        private void LoadRefClockList()
        {
            ArrayList refClockFreqs = new ArrayList();
            refClockFreqs = this.GetRefClockFreqs();
            for (int i = 0; i < refClockFreqs.Count; i++)
            {
                if (!this.refClkFreqComboBox.Items.Contains(refClockFreqs[i]))
                {
                    if (clsGlobal.IsMarketingUser())
                    {
                        if (((((string) refClockFreqs[i]) == "16369000") || (((string) refClockFreqs[i]) == "19200000")) || (((string) refClockFreqs[i]) == "26000000"))
                        {
                            this.refClkFreqComboBox.Items.Add(refClockFreqs[i]);
                        }
                    }
                    else
                    {
                        this.refClkFreqComboBox.Items.Add(refClockFreqs[i]);
                    }
                }
            }
            refClockFreqs.Clear();
        }

        private void newConfigCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.newConfigCheckBox.Checked)
            {
                if (this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex == 1)
                {
                    this.textBox_GPIO10.Enabled = true;
                }
            }
            else
            {
                this.textBox_GPIO10.Enabled = false;
            }
        }

        private void PollICConfigBtn_Click(object sender, EventArgs e)
        {
			base.BeginInvoke((MethodInvoker)delegate
			{
                this.pollTrackerConfig(this.comm);
                Thread.Sleep(100);
                this.settingsToPolledValues();
            });
        }

        private void pollTrackerConfig(CommunicationManager comm)
        {
            bool isSLCRx = false;
            int mid = 0xb2;
            int sid = 9;
            comm.CMC.TxCurrentTransmissionType = (CommonClass.TransmissionType) comm.TxCurrentTransmissionType;
            string messageProtocol = comm.MessageProtocol;
            string msg = comm.m_Protocols.GetDefaultMsgtoSend(isSLCRx, mid, sid, "SW Toolbox Message", messageProtocol);
            comm.WriteData(msg);
        }

        private void saveTrackerConfigParams(ref CommunicationManager targetComm, bool sendHotStart)
        {
            if (this.newConfigCheckBox.Checked)
            {
                targetComm.TrackerICCtrl.Version = 2.0;
            }
            else
            {
                targetComm.TrackerICCtrl.Version = 1.0;
            }
            try
            {
                targetComm.TrackerICCtrl.RefFreq = Convert.ToUInt32(this.refClkFreqComboBox.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ref Freq: " + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                targetComm.TrackerICCtrl.StartupDelay = Convert.ToUInt16(this.textBox_RefClkWarmupDelay.Text);
            }
            catch (Exception exception2)
            {
                MessageBox.Show("Startup Delay: " + exception2.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                if (this.comboBox_RefClkUncertainty.Text == "Unknown - use default")
                {
                    targetComm.TrackerICCtrl.RefClkUncertainty = uint.MaxValue;
                }
                else
                {
                    targetComm.TrackerICCtrl.RefClkUncertainty = Convert.ToUInt32(this.comboBox_RefClkUncertainty.Text);
                }
            }
            catch (Exception exception3)
            {
                MessageBox.Show("Uncertainty: " + exception3.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                if (this.comboBox_RefClkOffset.Text == "Unknown - use default")
                {
                    targetComm.TrackerICCtrl.RefClkOffset = 2147483647.0;
                }
                else
                {
                    this.clockFreq = Convert.ToSingle(this.refClkFreqComboBox.Text);
                    this.onePPM = ((this.clockFreq * Math.Pow(10.0, -6.0)) * 1540.0) / 16.0;
                    this.cDrift = ((this.clockFreq * 1540.0) / 16.0) - clsGlobal.GPSL1Frequency;
                    this.refclkOffsetPPM = this.cDrift + (Convert.ToSingle(this.comboBox_RefClkOffset.Text) * this.onePPM);
                    targetComm.TrackerICCtrl.RefClkOffset = Convert.ToInt32(this.refclkOffsetPPM);
                }
            }
            catch (Exception exception4)
            {
                MessageBox.Show("Uncertainty Offset: " + exception4.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            targetComm.TrackerICCtrl.LNASelect = (byte) this.frmTrackerConfigLNASelectComboBox.SelectedIndex;
            targetComm.TrackerICCtrl.IOPinConfigEnable = (byte) this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex;
            if (targetComm.TrackerICCtrl.IOPinConfigEnable == 0)
            {
                this.enableDisableIOPinConfig(false);
            }
            else
            {
                this.textBox_GPIO0.Enabled = true;
                try
                {
                    int num = 0;
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO0.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO1.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO2.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO3.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO4.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO5.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO6.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO7.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO8.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO9.Text, this.fromBaseHex);
                    targetComm.TrackerICCtrl.IOPinConfigs[num++] = Convert.ToUInt16(this.textBox_GPIO10.Text, this.fromBaseHex);
                }
                catch (Exception exception5)
                {
                    MessageBox.Show("IO Pin Configuration: " + exception5.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
            }
            try
            {
                targetComm.TrackerICCtrl.UARTBaud = Convert.ToUInt32(this.frmTrackerConfigBaudComboBox.Text);
            }
            catch (Exception exception6)
            {
                MessageBox.Show("Baud: " + exception6.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            targetComm.TrackerICCtrl.UARTFlowControlEnable = (byte) this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex;
            try
            {
                targetComm.TrackerICCtrl.UARTPreambleMax = Convert.ToByte(this.frmTrackerConfigWakeupPatternTextBox.Text);
            }
            catch (Exception exception7)
            {
                MessageBox.Show("UART Wake Up Pattern: " + exception7.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                targetComm.TrackerICCtrl.UARTWakeupDelay = Convert.ToByte(this.frmTrackerConfigWakeupMesgCntTextBox.Text);
            }
            catch (Exception exception8)
            {
                MessageBox.Show("UART Wake Up Pattern: " + exception8.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                targetComm.TrackerICCtrl.I2CMasterAddress = Convert.ToUInt16(this.frmTrackerConfigMasterAddrTextBox.Text, this.fromBaseHex);
            }
            catch (Exception exception9)
            {
                MessageBox.Show("I2C Master Address: " + exception9.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                targetComm.TrackerICCtrl.I2CSlaveAddress = Convert.ToUInt16(this.frmTrackerConfigSlaveAddrTextBox.Text, this.fromBaseHex);
            }
            catch (Exception exception10)
            {
                MessageBox.Show("I2C Slave Address: " + exception10.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            targetComm.TrackerICCtrl.I2CMode = (byte) this.frmTrackerConfigI2CModeComboBox.SelectedIndex;
            targetComm.TrackerICCtrl.I2CRate = (byte) this.frmTrackerConfigI2CRateComboBox.SelectedIndex;
            targetComm.TrackerICCtrl.I2CMaxMsgLength = Convert.ToUInt16(this.textBox_I2CMaxMsgLength.Text);
            byte a = Convert.ToByte(this.comboBox_PowCtrlOnOff_Edgetype.SelectedIndex);
            byte b = Convert.ToByte(this.comboBox_PowCtrlOnOff_Usagetype.SelectedIndex);
            byte c = Convert.ToByte(this.comboBox_PowCtrlOnOff_Offtype.SelectedIndex);
            targetComm.TrackerICCtrl.PwrCtrlOnOff = SetPowerControlBits(a, b, c);
            targetComm.TrackerICCtrl.LDOModeEnabled = (byte) this.comboBox_LDOEnable.SelectedIndex;
            targetComm.I2CModeSwitchDone = false;
            Thread.Sleep(50);
            targetComm.RxCtrl.SendTrackerConfig();
            Thread.Sleep(0x3e8);
            if (sendHotStart)
            {
                targetComm.RxCtrl.ResetCtrl.Reset("HOT");
            }
            targetComm.CMC.HostAppI2CSlave.I2CTalkMode = CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE;
            Thread.Sleep(50);
            targetComm.I2CModeSwitchDone = true;
        }

        private void setControlVisible(bool state)
        {
            this.frmTrackerConfigLNASelectLabel.Visible = state;
            this.frmTrackerConfigLNASelectComboBox.Visible = state;
            this.label11.Visible = state;
            this.comboBox_LDOEnable.Visible = state;
            this.groupBox1.Visible = state;
            this.groupBox2.Visible = state;
            this.groupBox3.Visible = state;
            this.groupBox4.Visible = state;
            this.groupBox5.Visible = state;
            this.groupBox6.Visible = state;
            this.groupBox7.Visible = state;
        }

        private static byte SetPowerControlBits(byte a, byte b, byte c)
        {
            return Convert.ToByte((int) ((a | (b << 3)) | (c << 5)));
        }

        private void settingsToPolledValues()
        {
            this.refClkFreqComboBox.Text = this.comm.dataICTrack.RefFreq.ToString();
            this.textBox_RefClkWarmupDelay.Text = this.comm.dataICTrack.StartupDelay.ToString();
            this.comboBox_RefClkUncertainty.Text = this.comm.dataICTrack.RefClkUncertainty.ToString();
            this.comboBox_RefClkOffset.Text = this.comm.dataICTrack.RefClkOffset.ToString("F1");
            this.frmTrackerConfigLNASelectComboBox.SelectedIndex = this.comm.dataICTrack.LNASelect;
            this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex = this.comm.dataICTrack.IOPinConfigEnable;
            int num = 0;
            this.textBox_GPIO0.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO1.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO2.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO3.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO4.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO5.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO6.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO7.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO8.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            this.textBox_GPIO9.Text = this.comm.dataICTrack.IOPinConfigs[num++].ToString("X");
            if (this.newConfigCheckBox.Checked)
            {
                this.textBox_GPIO10.Text = this.comm.TrackerICCtrl.IOPinConfigs[num++].ToString("X");
            }
            this.frmTrackerConfigWakeupPatternTextBox.Text = this.comm.dataICTrack.UARTPreambleMax.ToString();
            this.frmTrackerConfigWakeupMesgCntTextBox.Text = this.comm.dataICTrack.UARTWakeupDelay.ToString();
            this.frmTrackerConfigBaudComboBox.Text = this.comm.dataICTrack.UARTBaud.ToString();
            this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = this.comm.dataICTrack.UARTFlowControlEnable;
            this.frmTrackerConfigMasterAddrTextBox.Text = this.comm.dataICTrack.I2CMasterAddress.ToString("X");
            this.frmTrackerConfigSlaveAddrTextBox.Text = this.comm.dataICTrack.I2CSlaveAddress.ToString("X");
            this.frmTrackerConfigI2CRateComboBox.SelectedIndex = this.comm.dataICTrack.I2CRate;
            this.frmTrackerConfigI2CModeComboBox.SelectedIndex = this.comm.dataICTrack.I2CMode;
            this.textBox_I2CMaxMsgLength.Text = this.comm.dataICTrack.I2CMaxMsgLength.ToString();
            this.comboBox_PowCtrlOnOff_Edgetype.SelectedIndex = GetBits(this.comm.dataICTrack.PwrCtrlOnOff, 0, 3);
            this.comboBox_PowCtrlOnOff_Usagetype.SelectedIndex = GetBits(this.comm.dataICTrack.PwrCtrlOnOff, 3, 2);
            this.comboBox_PowCtrlOnOff_Offtype.SelectedIndex = GetBits(this.comm.dataICTrack.PwrCtrlOnOff, 5, 1);
            this.comboBox_LDOEnable.SelectedIndex = this.comm.dataICTrack.LDOModeEnabled;
            this.Refresh();
        }

        private void setWindowSizeforAdvancedOptions()
        {
            base.Width = (this.frmTrackerConfigOkBtn.Location.X + this.frmTrackerConfigOkBtn.Width) + 0x18;
            if (this._advancedGUIFlag)
            {
                base.Height = (this.groupBox2.Location.Y + this.groupBox2.Height) + 0x2b;
            }
            else
            {
                base.Height = (this.frmTrackerConfigCancelBtn.Location.Y + this.frmTrackerConfigCancelBtn.Height) + 70;
            }
            this.enableDisableAllCtrls(true);
        }

        private void textBox_GPIO0_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO0.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 0 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO1_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO1.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 1 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO10_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO10.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 8 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO2_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO2.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 2 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO3_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO3.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 3 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO4_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO4.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 4 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO5_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO5.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 5 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO6_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO6.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 6 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO7_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO7.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The GPIO 7 value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO8_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO0.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The RX value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_GPIO9_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    Convert.ToUInt16(this.textBox_GPIO9.Text, this.fromBaseHex);
                }
                catch
                {
                    MessageBox.Show("The TX value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_I2CMaxMsgLength_Validating(object sender, CancelEventArgs e)
        {
            if (base.DialogResult != DialogResult.Cancel)
            {
                try
                {
                    ushort num = Convert.ToUInt16(this.textBox_I2CMaxMsgLength.Text);
                    if ((num < 0) || (num > 0x400))
                    {
                        MessageBox.Show("The Max message length value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this._errorFound = true;
                    }
                }
                catch
                {
                    MessageBox.Show("The Max message length value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }

        private void textBox_RefClkWarmupDelay_Validating(object sender, CancelEventArgs e)
        {
            if ((base.DialogResult == DialogResult.None) || (base.DialogResult == DialogResult.OK))
            {
                try
                {
                    Convert.ToUInt16(this.textBox_RefClkWarmupDelay.Text);
                }
                catch
                {
                    MessageBox.Show("The Ref Clock Warmup Delay value is out of range", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this._errorFound = true;
                }
            }
        }
    }
}

