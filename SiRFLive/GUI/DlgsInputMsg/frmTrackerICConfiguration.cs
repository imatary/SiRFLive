﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using SiRFLive.Communication;
    using SiRFLive.MessageHandling;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmTrackerICConfiguration : Form
    {
        private Button button_Default;
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
        private TextBox frmTrackerConfigIOPinConfigTextBox;
        private ComboBox frmTrackerConfigLNASelectComboBox;
        private Label frmTrackerConfigLNASelectLabel;
        private Label frmTrackerConfigMasterAddrLabel;
        private TextBox frmTrackerConfigMasterAddrTextBox;
        private Label frmTrackerConfigModeLabel;
        private ComboBox frmTrackerConfigOffsetKnownComboBox;
        private Label frmTrackerConfigOffsetKnownLabel;
        private Label frmTrackerConfigOffsetLabel;
        private TextBox frmTrackerConfigOffsetTextBox;
        private Button frmTrackerConfigOkBtn;
        private Label frmTrackerConfigRateLabel;
        private TextBox frmTrackerConfigRefFreqTextBox;
        private Label frmTrackerConfigSlaveAddrLabel;
        private TextBox frmTrackerConfigSlaveAddrTextBox;
        private TextBox frmTrackerConfigStartupDelayTextBox;
        private ComboBox frmTrackerConfigUncertaintyKnownComboBox;
        private Label frmTrackerConfigUncertaintyKnownLabel;
        private Label frmTrackerConfigUncertaintyLable;
        private TextBox frmTrackerConfigUncertaintyTextBox;
        private Label frmTrackerConfigWakeupMesgCntLabel;
        private TextBox frmTrackerConfigWakeupMesgCntTextBox;
        private Label frmTrackerConfigWakeupPatternLabel;
        private TextBox frmTrackerConfigWakeupPatternTextBox;
        private Label frmTrackerICConfigRefFreqLabel;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private Label label1;
        private Label label10;
        private Label label11;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;

        public frmTrackerICConfiguration(CommunicationManager commMgr)
        {
            this.InitializeComponent();
            this.comm = commMgr;
        }

        private void button_Default_Click(object sender, EventArgs e)
        {
        }

        private void button_Default_Click_1(object sender, EventArgs e)
        {
            string defaultValue = string.Empty;
            ArrayList list = new ArrayList();
            list = this.comm.m_Protocols.GetDefaultMsgFieldList(false, 0xb2, 2, "Tracker Configuration", "OSP");
            for (int i = 0; i < list.Count; i++)
            {
                defaultValue = ((InputMsg) list[i]).defaultValue;
                switch (((InputMsg) list[i]).fieldName)
                {
                    case "Reference Clock Frequency":
                    {
                        this.frmTrackerConfigRefFreqTextBox.Text = ((InputMsg) list[i]).defaultValue;
                        continue;
                    }
                    case "Reference Start-up Delay":
                    {
                        string str2 = Convert.ToUInt16(defaultValue).ToString("X4");
                        this.frmTrackerConfigStartupDelayTextBox.Text = str2;
                        continue;
                    }
                    case "Reference Initial Uncertainty":
                    {
                        uint num3 = Convert.ToUInt32(defaultValue);
                        string str3 = num3.ToString("X8");
                        this.frmTrackerConfigUncertaintyTextBox.Text = str3;
                        if (num3 != uint.MaxValue)
                        {
                            break;
                        }
                        this.frmTrackerConfigUncertaintyKnownComboBox.SelectedIndex = 0;
                        this.frmTrackerConfigUncertaintyTextBox.ReadOnly = true;
                        continue;
                    }
                    case "Reference Initial Offset":
                    {
                        int num4 = Convert.ToInt32(defaultValue);
                        string str4 = num4.ToString("X8");
                        this.frmTrackerConfigOffsetTextBox.Text = str4;
                        if (num4 != 0x7fffffff)
                        {
                            goto Label_0284;
                        }
                        this.frmTrackerConfigOffsetKnownComboBox.SelectedIndex = 0;
                        this.frmTrackerConfigOffsetTextBox.ReadOnly = true;
                        continue;
                    }
                    case "LNA":
                    {
                        if (Convert.ToByte(defaultValue) != 0)
                        {
                            goto Label_02BE;
                        }
                        this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = 0;
                        continue;
                    }
                    case "IO Pin Configuration Enable":
                    {
                        if (Convert.ToByte(defaultValue) != 0)
                        {
                            goto Label_02EC;
                        }
                        this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex = 0;
                        continue;
                    }
                    case "IO Pin Configuration":
                    {
                        this.frmTrackerConfigIOPinConfigTextBox.Text = defaultValue;
                        if (this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex != 0)
                        {
                            goto Label_0327;
                        }
                        this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = true;
                        continue;
                    }
                    case "UART Baud":
                    {
                        this.frmTrackerConfigBaudComboBox.Text = defaultValue;
                        continue;
                    }
                    case "UART Flow Control":
                    {
                        if (!(defaultValue == "0"))
                        {
                            goto Label_0367;
                        }
                        this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = 0;
                        continue;
                    }
                    case " UART Wake Up Pattern":
                    {
                        this.frmTrackerConfigWakeupPatternTextBox.Text = defaultValue;
                        continue;
                    }
                    case "UART Wake Up Count":
                    {
                        this.frmTrackerConfigWakeupMesgCntTextBox.Text = defaultValue;
                        continue;
                    }
                    case "I2C Master Address":
                    {
                        string str5 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.frmTrackerConfigMasterAddrTextBox.Text = str5;
                        continue;
                    }
                    case "I2C Slave Address":
                    {
                        string str6 = Convert.ToUInt16(defaultValue).ToString("X");
                        this.frmTrackerConfigSlaveAddrTextBox.Text = str6;
                        continue;
                    }
                    case "I2C Mode":
                    {
                        byte num9 = Convert.ToByte(defaultValue);
                        this.frmTrackerConfigI2CModeComboBox.SelectedIndex = num9;
                        continue;
                    }
                    case "I2C Rate":
                    {
                        byte num10 = Convert.ToByte(defaultValue);
                        this.frmTrackerConfigI2CRateComboBox.SelectedIndex = num10;
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                this.frmTrackerConfigUncertaintyKnownComboBox.SelectedIndex = 1;
                this.frmTrackerConfigUncertaintyTextBox.ReadOnly = false;
                continue;
            Label_0284:
                this.frmTrackerConfigOffsetKnownComboBox.SelectedIndex = 1;
                this.frmTrackerConfigOffsetTextBox.ReadOnly = false;
                continue;
            Label_02BE:
                this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = 1;
                continue;
            Label_02EC:
                this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex = 1;
                continue;
            Label_0327:
                this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = false;
                continue;
            Label_0367:
                this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = 1;
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

        private void frmTrackerConfigCancelBtn_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void frmTrackerConfigIOPinConfigEnableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex == 0)
            {
                this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = true;
                this.frmTrackerConfigIOPinConfigTextBox.Text = "1";
            }
            else
            {
                this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = false;
            }
        }

        private void frmTrackerConfigOffsetKnownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.frmTrackerConfigOffsetKnownComboBox.SelectedIndex == 0)
            {
                this.comm.TrackerICCtrl.RefClkOffset = 2147483647.0;
                this.frmTrackerConfigOffsetTextBox.ReadOnly = true;
                this.frmTrackerConfigOffsetTextBox.Text = this.comm.TrackerICCtrl.RefClkOffset.ToString();
            }
            else
            {
                this.frmTrackerConfigOffsetTextBox.ReadOnly = false;
                this.frmTrackerConfigOffsetTextBox.Text = "";
            }
        }

        private void frmTrackerConfigOkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                this.comm.TrackerICCtrl.RefFreq = Convert.ToUInt32(this.frmTrackerConfigRefFreqTextBox.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ref Freq: " + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                this.comm.TrackerICCtrl.StartupDelay = Convert.ToUInt16(this.frmTrackerConfigStartupDelayTextBox.Text, 0x10);
            }
            catch (Exception exception2)
            {
                MessageBox.Show("Startup Delay: " + exception2.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (this.frmTrackerConfigUncertaintyKnownComboBox.SelectedIndex == 0)
            {
                this.comm.TrackerICCtrl.RefClkUncertainty = uint.MaxValue;
                this.frmTrackerConfigUncertaintyTextBox.ReadOnly = true;
                this.frmTrackerConfigUncertaintyTextBox.Text = this.comm.TrackerICCtrl.RefClkUncertainty.ToString();
            }
            else
            {
                try
                {
                    this.frmTrackerConfigUncertaintyTextBox.ReadOnly = false;
                    this.comm.TrackerICCtrl.RefClkUncertainty = Convert.ToUInt32(this.frmTrackerConfigUncertaintyTextBox.Text);
                }
                catch (Exception exception3)
                {
                    MessageBox.Show("Uncertainty: " + exception3.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
            }
            if (this.frmTrackerConfigOffsetKnownComboBox.SelectedIndex == 0)
            {
                this.comm.TrackerICCtrl.RefClkOffset = 2147483647.0;
                this.frmTrackerConfigOffsetTextBox.ReadOnly = true;
                this.frmTrackerConfigOffsetTextBox.Text = this.comm.TrackerICCtrl.RefClkOffset.ToString("F1");
            }
            else
            {
                try
                {
                    this.frmTrackerConfigOffsetTextBox.ReadOnly = false;
                    this.comm.TrackerICCtrl.RefClkOffset = Convert.ToInt32(this.frmTrackerConfigOffsetTextBox.Text);
                }
                catch (Exception exception4)
                {
                    MessageBox.Show("Uncertainty Offset: " + exception4.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
            }
            this.comm.TrackerICCtrl.LNASelect = (byte) this.frmTrackerConfigLNASelectComboBox.SelectedIndex;
            this.comm.TrackerICCtrl.IOPinConfigEnable = (byte) this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex;
            if (this.comm.TrackerICCtrl.IOPinConfigEnable == 0)
            {
                this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = true;
                this.frmTrackerConfigIOPinConfigTextBox.Text = "1";
            }
            else
            {
                this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = false;
                try
                {
                    this.comm.TrackerICCtrl.IOPinConfig = Convert.ToUInt16(this.frmTrackerConfigIOPinConfigTextBox.Text);
                }
                catch (Exception exception5)
                {
                    MessageBox.Show("IO Pin Configuration: " + exception5.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
            }
            try
            {
                this.comm.TrackerICCtrl.UARTBaud = Convert.ToUInt32(this.frmTrackerConfigBaudComboBox.Text);
            }
            catch (Exception exception6)
            {
                MessageBox.Show("Baud: " + exception6.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            this.comm.TrackerICCtrl.UARTFlowControlEnable = (byte) this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex;
            try
            {
                this.comm.TrackerICCtrl.UARTWakeupPattern = Convert.ToByte(this.frmTrackerConfigWakeupPatternTextBox.Text);
            }
            catch (Exception exception7)
            {
                MessageBox.Show("UART Wake Up Pattern: " + exception7.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                this.comm.TrackerICCtrl.UARTWakeupCount = Convert.ToByte(this.frmTrackerConfigWakeupMesgCntTextBox.Text);
            }
            catch (Exception exception8)
            {
                MessageBox.Show("UART Wake Up Pattern: " + exception8.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                this.comm.TrackerICCtrl.I2CMasterAddress = Convert.ToUInt16(this.frmTrackerConfigMasterAddrTextBox.Text, 0x10);
            }
            catch (Exception exception9)
            {
                MessageBox.Show("I2C Master Address: " + exception9.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                this.comm.TrackerICCtrl.I2CSlaveAddress = Convert.ToUInt16(this.frmTrackerConfigSlaveAddrTextBox.Text, 0x10);
            }
            catch (Exception exception10)
            {
                MessageBox.Show("I2C Slave Address: " + exception10.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            this.comm.TrackerICCtrl.I2CMode = (byte) this.frmTrackerConfigI2CModeComboBox.SelectedIndex;
            this.comm.TrackerICCtrl.I2CRate = (byte) this.frmTrackerConfigI2CRateComboBox.SelectedIndex;
            this.comm.RxCtrl.SendTrackerConfig();
            base.Close();
        }

        private void frmTrackerConfigUncertaintyKnownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.frmTrackerConfigUncertaintyKnownComboBox.SelectedIndex == 0)
            {
                this.comm.TrackerICCtrl.RefClkUncertainty = uint.MaxValue;
                this.frmTrackerConfigUncertaintyTextBox.ReadOnly = true;
                this.frmTrackerConfigUncertaintyTextBox.Text = this.comm.TrackerICCtrl.RefClkUncertainty.ToString();
            }
            else
            {
                this.frmTrackerConfigUncertaintyTextBox.ReadOnly = false;
                this.frmTrackerConfigUncertaintyTextBox.Text = "";
            }
        }

        private void frmTrackerICConfiguration_Load(object sender, EventArgs e)
        {
            this.frmTrackerConfigRefFreqTextBox.Text = this.comm.TrackerICCtrl.RefFreq.ToString();
            this.frmTrackerConfigStartupDelayTextBox.Text = this.comm.TrackerICCtrl.StartupDelay.ToString("X4");
            if (this.comm.TrackerICCtrl.RefClkUncertainty == uint.MaxValue)
            {
                this.frmTrackerConfigUncertaintyKnownComboBox.SelectedIndex = 0;
                this.frmTrackerConfigUncertaintyTextBox.ReadOnly = true;
                this.frmTrackerConfigUncertaintyTextBox.Text = "FFFFFFFF";
            }
            else
            {
                this.frmTrackerConfigUncertaintyKnownComboBox.SelectedIndex = 1;
                this.frmTrackerConfigUncertaintyTextBox.ReadOnly = false;
                this.frmTrackerConfigUncertaintyTextBox.Text = this.comm.TrackerICCtrl.RefClkUncertainty.ToString();
            }
            if (this.comm.TrackerICCtrl.RefClkOffset == 2147483647.0)
            {
                this.frmTrackerConfigOffsetKnownComboBox.SelectedIndex = 0;
                this.frmTrackerConfigOffsetTextBox.ReadOnly = true;
                this.frmTrackerConfigOffsetTextBox.Text = "7FFFFFFF";
            }
            else
            {
                this.frmTrackerConfigOffsetKnownComboBox.SelectedIndex = 1;
                this.frmTrackerConfigOffsetTextBox.ReadOnly = false;
                this.frmTrackerConfigOffsetTextBox.Text = this.comm.TrackerICCtrl.RefClkOffset.ToString();
            }
            this.frmTrackerConfigLNASelectComboBox.SelectedIndex = this.comm.TrackerICCtrl.LNASelect;
            this.frmTrackerConfigIOPinConfigEnableComboBox.SelectedIndex = this.comm.TrackerICCtrl.IOPinConfigEnable;
            if (this.comm.TrackerICCtrl.IOPinConfigEnable == 0)
            {
                this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = true;
                this.frmTrackerConfigIOPinConfigTextBox.Text = "1";
            }
            else
            {
                this.frmTrackerConfigIOPinConfigTextBox.ReadOnly = false;
                this.frmTrackerConfigIOPinConfigTextBox.Text = this.comm.TrackerICCtrl.IOPinConfig.ToString();
            }
            this.frmTrackerConfigBaudComboBox.Text = this.comm.TrackerICCtrl.UARTBaud.ToString();
            this.frmTrackerConfigFlowCtrlComboBox.SelectedIndex = this.comm.TrackerICCtrl.UARTFlowControlEnable;
            this.frmTrackerConfigWakeupPatternTextBox.Text = this.comm.TrackerICCtrl.UARTWakeupPattern.ToString();
            this.frmTrackerConfigWakeupMesgCntTextBox.Text = this.comm.TrackerICCtrl.UARTWakeupCount.ToString();
            this.frmTrackerConfigMasterAddrTextBox.Text = this.comm.TrackerICCtrl.I2CMasterAddress.ToString("X");
            this.frmTrackerConfigSlaveAddrTextBox.Text = this.comm.TrackerICCtrl.I2CSlaveAddress.ToString("X");
            this.frmTrackerConfigI2CModeComboBox.SelectedIndex = this.comm.TrackerICCtrl.I2CMode;
            this.frmTrackerConfigI2CRateComboBox.SelectedIndex = this.comm.TrackerICCtrl.I2CRate;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmTrackerICConfiguration));
            this.frmTrackerICConfigRefFreqLabel = new Label();
            this.frmTrackerConfigRefFreqTextBox = new TextBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.frmTrackerConfigStartupDelayTextBox = new TextBox();
            this.label3 = new Label();
            this.groupBox1 = new GroupBox();
            this.frmTrackerConfigUncertaintyKnownComboBox = new ComboBox();
            this.label4 = new Label();
            this.frmTrackerConfigUncertaintyTextBox = new TextBox();
            this.frmTrackerConfigUncertaintyLable = new Label();
            this.frmTrackerConfigUncertaintyKnownLabel = new Label();
            this.frmTrackerConfigLNASelectLabel = new Label();
            this.frmTrackerConfigLNASelectComboBox = new ComboBox();
            this.groupBox2 = new GroupBox();
            this.frmTrackerConfigIOPinConfigTextBox = new TextBox();
            this.frmTrackerConfigIOPinConfigLabel = new Label();
            this.frmTrackerConfigIOPinConfigEnableComboBox = new ComboBox();
            this.frmTrackerConfigIOPinConfigEnableLabel = new Label();
            this.groupBox3 = new GroupBox();
            this.label5 = new Label();
            this.frmTrackerConfigOffsetTextBox = new TextBox();
            this.frmTrackerConfigOffsetLabel = new Label();
            this.frmTrackerConfigOffsetKnownComboBox = new ComboBox();
            this.frmTrackerConfigOffsetKnownLabel = new Label();
            this.groupBox4 = new GroupBox();
            this.frmTrackerConfigFlowCtrlComboBox = new ComboBox();
            this.label6 = new Label();
            this.frmTrackerConfigWakeupMesgCntTextBox = new TextBox();
            this.frmTrackerConfigWakeupMesgCntLabel = new Label();
            this.frmTrackerConfigWakeupPatternTextBox = new TextBox();
            this.frmTrackerConfigWakeupPatternLabel = new Label();
            this.frmTrackerConfigFlowControlLabel = new Label();
            this.frmTrackerConfigBaudComboBox = new ComboBox();
            this.frmTrackerConfigBaudLabel = new Label();
            this.groupBox5 = new GroupBox();
            this.label8 = new Label();
            this.label7 = new Label();
            this.label10 = new Label();
            this.label11 = new Label();
            this.frmTrackerConfigMasterAddrTextBox = new TextBox();
            this.frmTrackerConfigModeLabel = new Label();
            this.frmTrackerConfigRateLabel = new Label();
            this.frmTrackerConfigSlaveAddrTextBox = new TextBox();
            this.frmTrackerConfigSlaveAddrLabel = new Label();
            this.frmTrackerConfigI2CModeComboBox = new ComboBox();
            this.frmTrackerConfigI2CRateComboBox = new ComboBox();
            this.frmTrackerConfigMasterAddrLabel = new Label();
            this.frmTrackerConfigOkBtn = new Button();
            this.frmTrackerConfigCancelBtn = new Button();
            this.label9 = new Label();
            this.button_Default = new Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            base.SuspendLayout();
            this.frmTrackerICConfigRefFreqLabel.AutoSize = true;
            this.frmTrackerICConfigRefFreqLabel.Location = new Point(30, 0x18);
            this.frmTrackerICConfigRefFreqLabel.Name = "frmTrackerICConfigRefFreqLabel";
            this.frmTrackerICConfigRefFreqLabel.Size = new Size(80, 13);
            this.frmTrackerICConfigRefFreqLabel.TabIndex = 0;
            this.frmTrackerICConfigRefFreqLabel.Text = "Ref Frequency:";
            this.frmTrackerConfigRefFreqTextBox.Location = new Point(0x8f, 20);
            this.frmTrackerConfigRefFreqTextBox.Name = "frmTrackerConfigRefFreqTextBox";
            this.frmTrackerConfigRefFreqTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigRefFreqTextBox.TabIndex = 0;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0xf6, 0x18);
            this.label1.Name = "label1";
            this.label1.Size = new Size(20, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hz";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(30, 0x2c);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x4d, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Start-up Delay:";
            this.frmTrackerConfigStartupDelayTextBox.Location = new Point(0x8f, 40);
            this.frmTrackerConfigStartupDelayTextBox.Name = "frmTrackerConfigStartupDelayTextBox";
            this.frmTrackerConfigStartupDelayTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigStartupDelayTextBox.TabIndex = 1;
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0xf6, 0x2c);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x3e, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "RTC cycles";
            this.groupBox1.Controls.Add(this.frmTrackerConfigUncertaintyKnownComboBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.frmTrackerConfigUncertaintyTextBox);
            this.groupBox1.Controls.Add(this.frmTrackerConfigUncertaintyLable);
            this.groupBox1.Controls.Add(this.frmTrackerConfigUncertaintyKnownLabel);
            this.groupBox1.Location = new Point(0x17, 0x4d);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x146, 0x4d);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ref Clock Uncertainty";
            this.frmTrackerConfigUncertaintyKnownComboBox.FormattingEnabled = true;
            this.frmTrackerConfigUncertaintyKnownComboBox.Items.AddRange(new object[] { "Uncertainty Unknown", "Uncertainty Known" });
            this.frmTrackerConfigUncertaintyKnownComboBox.Location = new Point(0x79, 20);
            this.frmTrackerConfigUncertaintyKnownComboBox.Name = "frmTrackerConfigUncertaintyKnownComboBox";
            this.frmTrackerConfigUncertaintyKnownComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigUncertaintyKnownComboBox.TabIndex = 0;
            this.frmTrackerConfigUncertaintyKnownComboBox.SelectedIndexChanged += new EventHandler(this.frmTrackerConfigUncertaintyKnownComboBox_SelectedIndexChanged);
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0xe0, 0x34);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x19, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "ppb";
            this.frmTrackerConfigUncertaintyTextBox.Location = new Point(0x79, 0x30);
            this.frmTrackerConfigUncertaintyTextBox.Name = "frmTrackerConfigUncertaintyTextBox";
            this.frmTrackerConfigUncertaintyTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigUncertaintyTextBox.TabIndex = 1;
            this.frmTrackerConfigUncertaintyLable.AutoSize = true;
            this.frmTrackerConfigUncertaintyLable.Location = new Point(7, 0x34);
            this.frmTrackerConfigUncertaintyLable.Name = "frmTrackerConfigUncertaintyLable";
            this.frmTrackerConfigUncertaintyLable.Size = new Size(0x40, 13);
            this.frmTrackerConfigUncertaintyLable.TabIndex = 2;
            this.frmTrackerConfigUncertaintyLable.Text = "Uncertainty:";
            this.frmTrackerConfigUncertaintyKnownLabel.AutoSize = true;
            this.frmTrackerConfigUncertaintyKnownLabel.Location = new Point(7, 0x18);
            this.frmTrackerConfigUncertaintyKnownLabel.Name = "frmTrackerConfigUncertaintyKnownLabel";
            this.frmTrackerConfigUncertaintyKnownLabel.Size = new Size(0x67, 13);
            this.frmTrackerConfigUncertaintyKnownLabel.TabIndex = 0;
            this.frmTrackerConfigUncertaintyKnownLabel.Text = "Uncertainty Known?";
            this.frmTrackerConfigLNASelectLabel.AutoSize = true;
            this.frmTrackerConfigLNASelectLabel.Location = new Point(30, 0x106);
            this.frmTrackerConfigLNASelectLabel.Name = "frmTrackerConfigLNASelectLabel";
            this.frmTrackerConfigLNASelectLabel.Size = new Size(0x40, 13);
            this.frmTrackerConfigLNASelectLabel.TabIndex = 5;
            this.frmTrackerConfigLNASelectLabel.Text = "LNA Select:";
            this.frmTrackerConfigLNASelectComboBox.FormattingEnabled = true;
            this.frmTrackerConfigLNASelectComboBox.Items.AddRange(new object[] { "Use Internal LNA", "Use External LNA" });
            this.frmTrackerConfigLNASelectComboBox.Location = new Point(0x8f, 0x102);
            this.frmTrackerConfigLNASelectComboBox.Name = "frmTrackerConfigLNASelectComboBox";
            this.frmTrackerConfigLNASelectComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigLNASelectComboBox.TabIndex = 4;
            this.groupBox2.Controls.Add(this.frmTrackerConfigIOPinConfigTextBox);
            this.groupBox2.Controls.Add(this.frmTrackerConfigIOPinConfigLabel);
            this.groupBox2.Controls.Add(this.frmTrackerConfigIOPinConfigEnableComboBox);
            this.groupBox2.Controls.Add(this.frmTrackerConfigIOPinConfigEnableLabel);
            this.groupBox2.Location = new Point(0x17, 290);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(0x146, 0x4d);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "IO Pin Config";
            this.frmTrackerConfigIOPinConfigTextBox.Location = new Point(120, 0x30);
            this.frmTrackerConfigIOPinConfigTextBox.Name = "frmTrackerConfigIOPinConfigTextBox";
            this.frmTrackerConfigIOPinConfigTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigIOPinConfigTextBox.TabIndex = 1;
            this.frmTrackerConfigIOPinConfigLabel.AutoSize = true;
            this.frmTrackerConfigIOPinConfigLabel.Location = new Point(6, 0x34);
            this.frmTrackerConfigIOPinConfigLabel.Name = "frmTrackerConfigIOPinConfigLabel";
            this.frmTrackerConfigIOPinConfigLabel.Size = new Size(0x65, 13);
            this.frmTrackerConfigIOPinConfigLabel.TabIndex = 6;
            this.frmTrackerConfigIOPinConfigLabel.Text = "IO Pin Configuration";
            this.frmTrackerConfigIOPinConfigEnableComboBox.FormattingEnabled = true;
            this.frmTrackerConfigIOPinConfigEnableComboBox.Items.AddRange(new object[] { "Disable IO Pin config", "Enable IO Pin Config" });
            this.frmTrackerConfigIOPinConfigEnableComboBox.Location = new Point(120, 20);
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
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.frmTrackerConfigOffsetTextBox);
            this.groupBox3.Controls.Add(this.frmTrackerConfigOffsetLabel);
            this.groupBox3.Controls.Add(this.frmTrackerConfigOffsetKnownComboBox);
            this.groupBox3.Controls.Add(this.frmTrackerConfigOffsetKnownLabel);
            this.groupBox3.Location = new Point(0x17, 0xa9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(0x146, 0x4d);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Ref Clock Offset";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(0xe0, 0x34);
            this.label5.Name = "label5";
            this.label5.Size = new Size(20, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Hz";
            this.frmTrackerConfigOffsetTextBox.Location = new Point(0x79, 0x30);
            this.frmTrackerConfigOffsetTextBox.Name = "frmTrackerConfigOffsetTextBox";
            this.frmTrackerConfigOffsetTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigOffsetTextBox.TabIndex = 1;
            this.frmTrackerConfigOffsetLabel.AutoSize = true;
            this.frmTrackerConfigOffsetLabel.Location = new Point(7, 0x34);
            this.frmTrackerConfigOffsetLabel.Name = "frmTrackerConfigOffsetLabel";
            this.frmTrackerConfigOffsetLabel.Size = new Size(0x26, 13);
            this.frmTrackerConfigOffsetLabel.TabIndex = 2;
            this.frmTrackerConfigOffsetLabel.Text = "Offset:";
            this.frmTrackerConfigOffsetKnownComboBox.FormattingEnabled = true;
            this.frmTrackerConfigOffsetKnownComboBox.Items.AddRange(new object[] { "Offset Unknown", "Offset Known" });
            this.frmTrackerConfigOffsetKnownComboBox.Location = new Point(0x79, 20);
            this.frmTrackerConfigOffsetKnownComboBox.Name = "frmTrackerConfigOffsetKnownComboBox";
            this.frmTrackerConfigOffsetKnownComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigOffsetKnownComboBox.TabIndex = 0;
            this.frmTrackerConfigOffsetKnownComboBox.SelectedIndexChanged += new EventHandler(this.frmTrackerConfigOffsetKnownComboBox_SelectedIndexChanged);
            this.frmTrackerConfigOffsetKnownLabel.AutoSize = true;
            this.frmTrackerConfigOffsetKnownLabel.Location = new Point(7, 0x18);
            this.frmTrackerConfigOffsetKnownLabel.Name = "frmTrackerConfigOffsetKnownLabel";
            this.frmTrackerConfigOffsetKnownLabel.Size = new Size(0x4d, 13);
            this.frmTrackerConfigOffsetKnownLabel.TabIndex = 0;
            this.frmTrackerConfigOffsetKnownLabel.Text = "Offset Known?";
            this.groupBox4.Controls.Add(this.frmTrackerConfigFlowCtrlComboBox);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupMesgCntTextBox);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupMesgCntLabel);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupPatternTextBox);
            this.groupBox4.Controls.Add(this.frmTrackerConfigWakeupPatternLabel);
            this.groupBox4.Controls.Add(this.frmTrackerConfigFlowControlLabel);
            this.groupBox4.Controls.Add(this.frmTrackerConfigBaudComboBox);
            this.groupBox4.Controls.Add(this.frmTrackerConfigBaudLabel);
            this.groupBox4.Location = new Point(0x17, 0x17f);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new Size(0x146, 0x7e);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "UART Config";
            this.frmTrackerConfigFlowCtrlComboBox.FormattingEnabled = true;
            this.frmTrackerConfigFlowCtrlComboBox.Items.AddRange(new object[] { "Disable", "Enable" });
            this.frmTrackerConfigFlowCtrlComboBox.Location = new Point(120, 0x2d);
            this.frmTrackerConfigFlowCtrlComboBox.Name = "frmTrackerConfigFlowCtrlComboBox";
            this.frmTrackerConfigFlowCtrlComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigFlowCtrlComboBox.TabIndex = 1;
            this.label6.AutoSize = true;
            this.label6.Location = new Point(9, 0x63);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x26, 13);
            this.label6.TabIndex = 0x10;
            this.label6.Text = "Count:";
            this.frmTrackerConfigWakeupMesgCntTextBox.Location = new Point(120, 0x5f);
            this.frmTrackerConfigWakeupMesgCntTextBox.Name = "frmTrackerConfigWakeupMesgCntTextBox";
            this.frmTrackerConfigWakeupMesgCntTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigWakeupMesgCntTextBox.TabIndex = 3;
            this.frmTrackerConfigWakeupMesgCntLabel.AutoSize = true;
            this.frmTrackerConfigWakeupMesgCntLabel.Location = new Point(6, 0x56);
            this.frmTrackerConfigWakeupMesgCntLabel.Name = "frmTrackerConfigWakeupMesgCntLabel";
            this.frmTrackerConfigWakeupMesgCntLabel.Size = new Size(0x63, 13);
            this.frmTrackerConfigWakeupMesgCntLabel.TabIndex = 14;
            this.frmTrackerConfigWakeupMesgCntLabel.Text = "Wake Up Message";
            this.frmTrackerConfigWakeupPatternTextBox.Location = new Point(120, 70);
            this.frmTrackerConfigWakeupPatternTextBox.Name = "frmTrackerConfigWakeupPatternTextBox";
            this.frmTrackerConfigWakeupPatternTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigWakeupPatternTextBox.TabIndex = 2;
            this.frmTrackerConfigWakeupPatternLabel.AutoSize = true;
            this.frmTrackerConfigWakeupPatternLabel.Location = new Point(6, 0x42);
            this.frmTrackerConfigWakeupPatternLabel.Name = "frmTrackerConfigWakeupPatternLabel";
            this.frmTrackerConfigWakeupPatternLabel.Size = new Size(0x5d, 13);
            this.frmTrackerConfigWakeupPatternLabel.TabIndex = 12;
            this.frmTrackerConfigWakeupPatternLabel.Text = "Wake Up Pattern:";
            this.frmTrackerConfigFlowControlLabel.AutoSize = true;
            this.frmTrackerConfigFlowControlLabel.Location = new Point(6, 0x31);
            this.frmTrackerConfigFlowControlLabel.Name = "frmTrackerConfigFlowControlLabel";
            this.frmTrackerConfigFlowControlLabel.Size = new Size(0x44, 13);
            this.frmTrackerConfigFlowControlLabel.TabIndex = 10;
            this.frmTrackerConfigFlowControlLabel.Text = "Flow Control:";
            this.frmTrackerConfigBaudComboBox.FormattingEnabled = true;
            this.frmTrackerConfigBaudComboBox.Items.AddRange(new object[] { 
                "1200", "1800", "2400", "3600", "4800", "7200", "9600", "14400", "19200", "28800", "38400", "57600", "76800", "115200", "153600", "230400", 
                "307200", "460800", "614400", "921600", "1228800", "1843200"
             });
            this.frmTrackerConfigBaudComboBox.Location = new Point(120, 20);
            this.frmTrackerConfigBaudComboBox.Name = "frmTrackerConfigBaudComboBox";
            this.frmTrackerConfigBaudComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigBaudComboBox.TabIndex = 0;
            this.frmTrackerConfigBaudLabel.AutoSize = true;
            this.frmTrackerConfigBaudLabel.Location = new Point(6, 0x18);
            this.frmTrackerConfigBaudLabel.Name = "frmTrackerConfigBaudLabel";
            this.frmTrackerConfigBaudLabel.Size = new Size(0x23, 13);
            this.frmTrackerConfigBaudLabel.TabIndex = 8;
            this.frmTrackerConfigBaudLabel.Text = "Baud:";
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.frmTrackerConfigMasterAddrTextBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigModeLabel);
            this.groupBox5.Controls.Add(this.frmTrackerConfigRateLabel);
            this.groupBox5.Controls.Add(this.frmTrackerConfigSlaveAddrTextBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigSlaveAddrLabel);
            this.groupBox5.Controls.Add(this.frmTrackerConfigI2CModeComboBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigI2CRateComboBox);
            this.groupBox5.Controls.Add(this.frmTrackerConfigMasterAddrLabel);
            this.groupBox5.Location = new Point(0x17, 0x20d);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new Size(0x146, 0x9a);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "I2C Config";
            this.label8.AutoSize = true;
            this.label8.Location = new Point(0x60, 60);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0x12, 13);
            this.label8.TabIndex = 0x13;
            this.label8.Text = "0x";
            this.label7.AutoSize = true;
            this.label7.Location = new Point(0x60, 0x1d);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x12, 13);
            this.label7.TabIndex = 0x13;
            this.label7.Text = "0x";
            this.label10.AutoSize = true;
            this.label10.Location = new Point(5, 0x42);
            this.label10.Name = "label10";
            this.label10.Size = new Size(50, 13);
            this.label10.TabIndex = 0x12;
            this.label10.Text = "(Tracker)";
            this.label11.AutoSize = true;
            this.label11.Location = new Point(7, 0x23);
            this.label11.Name = "label11";
            this.label11.Size = new Size(0x23, 13);
            this.label11.TabIndex = 0x11;
            this.label11.Text = "(Host)";
            this.frmTrackerConfigMasterAddrTextBox.Location = new Point(120, 0x19);
            this.frmTrackerConfigMasterAddrTextBox.Name = "frmTrackerConfigMasterAddrTextBox";
            this.frmTrackerConfigMasterAddrTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigMasterAddrTextBox.TabIndex = 0;
            this.frmTrackerConfigModeLabel.AutoSize = true;
            this.frmTrackerConfigModeLabel.Location = new Point(6, 0x5d);
            this.frmTrackerConfigModeLabel.Name = "frmTrackerConfigModeLabel";
            this.frmTrackerConfigModeLabel.Size = new Size(0x25, 13);
            this.frmTrackerConfigModeLabel.TabIndex = 14;
            this.frmTrackerConfigModeLabel.Text = "Mode:";
            this.frmTrackerConfigRateLabel.AutoSize = true;
            this.frmTrackerConfigRateLabel.Location = new Point(6, 0x7d);
            this.frmTrackerConfigRateLabel.Name = "frmTrackerConfigRateLabel";
            this.frmTrackerConfigRateLabel.Size = new Size(0x21, 13);
            this.frmTrackerConfigRateLabel.TabIndex = 12;
            this.frmTrackerConfigRateLabel.Text = "Rate:";
            this.frmTrackerConfigSlaveAddrTextBox.Location = new Point(120, 0x39);
            this.frmTrackerConfigSlaveAddrTextBox.Name = "frmTrackerConfigSlaveAddrTextBox";
            this.frmTrackerConfigSlaveAddrTextBox.Size = new Size(100, 20);
            this.frmTrackerConfigSlaveAddrTextBox.TabIndex = 1;
            this.frmTrackerConfigSlaveAddrLabel.AutoSize = true;
            this.frmTrackerConfigSlaveAddrLabel.Location = new Point(5, 0x35);
            this.frmTrackerConfigSlaveAddrLabel.Name = "frmTrackerConfigSlaveAddrLabel";
            this.frmTrackerConfigSlaveAddrLabel.Size = new Size(0x4e, 13);
            this.frmTrackerConfigSlaveAddrLabel.TabIndex = 10;
            this.frmTrackerConfigSlaveAddrLabel.Text = "Slave Address:";
            this.frmTrackerConfigI2CModeComboBox.FormattingEnabled = true;
            this.frmTrackerConfigI2CModeComboBox.Items.AddRange(new object[] { "Slave", "Multi-Master" });
            this.frmTrackerConfigI2CModeComboBox.Location = new Point(120, 0x59);
            this.frmTrackerConfigI2CModeComboBox.Name = "frmTrackerConfigI2CModeComboBox";
            this.frmTrackerConfigI2CModeComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigI2CModeComboBox.TabIndex = 2;
            this.frmTrackerConfigI2CRateComboBox.FormattingEnabled = true;
            this.frmTrackerConfigI2CRateComboBox.Items.AddRange(new object[] { "100 Kbps", "400 Kbps", "1 Mbps (not available for GSD4t)", "3.4 Mbps (not available for GSD4t)" });
            this.frmTrackerConfigI2CRateComboBox.Location = new Point(120, 0x79);
            this.frmTrackerConfigI2CRateComboBox.Name = "frmTrackerConfigI2CRateComboBox";
            this.frmTrackerConfigI2CRateComboBox.Size = new Size(100, 0x15);
            this.frmTrackerConfigI2CRateComboBox.TabIndex = 3;
            this.frmTrackerConfigMasterAddrLabel.AutoSize = true;
            this.frmTrackerConfigMasterAddrLabel.Location = new Point(6, 0x16);
            this.frmTrackerConfigMasterAddrLabel.Name = "frmTrackerConfigMasterAddrLabel";
            this.frmTrackerConfigMasterAddrLabel.Size = new Size(0x53, 13);
            this.frmTrackerConfigMasterAddrLabel.TabIndex = 8;
            this.frmTrackerConfigMasterAddrLabel.Text = "Master Address:";
            this.frmTrackerConfigOkBtn.Location = new Point(0x62, 0x2cb);
            this.frmTrackerConfigOkBtn.Name = "frmTrackerConfigOkBtn";
            this.frmTrackerConfigOkBtn.Size = new Size(0x4b, 0x17);
            this.frmTrackerConfigOkBtn.TabIndex = 8;
            this.frmTrackerConfigOkBtn.Text = "&OK";
            this.frmTrackerConfigOkBtn.UseVisualStyleBackColor = true;
            this.frmTrackerConfigOkBtn.Click += new EventHandler(this.frmTrackerConfigOkBtn_Click);
            this.frmTrackerConfigCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.frmTrackerConfigCancelBtn.Location = new Point(0xc5, 0x2cb);
            this.frmTrackerConfigCancelBtn.Name = "frmTrackerConfigCancelBtn";
            this.frmTrackerConfigCancelBtn.Size = new Size(0x4b, 0x17);
            this.frmTrackerConfigCancelBtn.TabIndex = 9;
            this.frmTrackerConfigCancelBtn.Text = "&Cancel";
            this.frmTrackerConfigCancelBtn.UseVisualStyleBackColor = true;
            this.frmTrackerConfigCancelBtn.Click += new EventHandler(this.frmTrackerConfigCancelBtn_Click);
            this.label9.AutoSize = true;
            this.label9.Location = new Point(0x77, 0x2c);
            this.label9.Name = "label9";
            this.label9.Size = new Size(0x12, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "0x";
            this.button_Default.Location = new Point(0x17, 0x2ad);
            this.button_Default.Name = "button_Default";
            this.button_Default.Size = new Size(0x37, 0x17);
            this.button_Default.TabIndex = 0x15;
            this.button_Default.Text = "Default";
            this.button_Default.UseVisualStyleBackColor = true;
            this.button_Default.Click += new EventHandler(this.button_Default_Click_1);
            base.AcceptButton = this.frmTrackerConfigOkBtn;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.frmTrackerConfigCancelBtn;
            base.ClientSize = new Size(0x176, 750);
            base.Controls.Add(this.button_Default);
            base.Controls.Add(this.label9);
            base.Controls.Add(this.frmTrackerConfigCancelBtn);
            base.Controls.Add(this.frmTrackerConfigOkBtn);
            base.Controls.Add(this.groupBox5);
            base.Controls.Add(this.groupBox4);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.frmTrackerConfigLNASelectComboBox);
            base.Controls.Add(this.frmTrackerConfigLNASelectLabel);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.frmTrackerConfigStartupDelayTextBox);
            base.Controls.Add(this.frmTrackerConfigRefFreqTextBox);
            base.Controls.Add(this.frmTrackerICConfigRefFreqLabel);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmTrackerICConfiguration";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Rx IC Configuration";
            base.Load += new EventHandler(this.frmTrackerICConfiguration_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

