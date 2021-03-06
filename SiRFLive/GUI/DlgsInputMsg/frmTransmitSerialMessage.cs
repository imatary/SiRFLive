﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.MessageHandling;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows.Forms;

    public class frmTransmitSerialMessage : Form
    {
        private CommunicationManager comm;
        private IContainer components;
        private Button frmTransmitSerialCancelBtn;
        private RadioButton frmTransmitSerialMessageAi3RadioBtn;
        private RadioButton frmTransmitSerialMessageFRadioBtn;
        private TextBox frmTransmitSerialMessageInputText;
        private RadioButton frmTransmitSerialMessageNmeaRadioBtn;
        private RadioButton frmTransmitSerialMessageOspRadioBtn;
        private TextBox frmTransmitSerialMessageOutputText;
        private GroupBox frmTransmitSerialMessageProtocolWrapperGroupBox;
        private RadioButton frmTransmitSerialMessageRawRadioBtn;
        private Button frmTransmitSerialMessageSendBtn;
        private RadioButton frmTransmitSerialMessageSsbRadioBtn;
        private Label frmTransmitSerialMessageTextLabel;
        private RadioButton frmTransmitSerialMessageTTBRadioBtn;
        public int WinHeight;
        public int WinLeft;
        public int WinTop;
        public int WinWidth;

        public event UpdateWindowEventHandler UpdatePortManager;

        public frmTransmitSerialMessage(CommunicationManager controlComm)
        {
            this.InitializeComponent();
            this.comm = controlComm;
            base.MdiParent = clsGlobal.g_objfrmMDIMain;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmTransmitSerialCancelBtn_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void frmTransmitSerialMessage_Load(object sender, EventArgs e)
        {
            if (clsGlobal.IsMarketingUser())
            {
                this.frmTransmitSerialMessageSsbRadioBtn.Visible = false;
                this.frmTransmitSerialMessageFRadioBtn.Visible = false;
                this.frmTransmitSerialMessageAi3RadioBtn.Visible = false;
                this.frmTransmitSerialMessageTTBRadioBtn.Visible = false;
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

        private void frmTransmitSerialMessageAi3RadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
        }

        private void frmTransmitSerialMessageFRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
        }

        private void frmTransmitSerialMessageInputText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                this.UpdateRawDataToSendBox();
            }
        }

        private void frmTransmitSerialMessageNmeaRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
        }

        private void frmTransmitSerialMessageOspRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
        }

        private void frmTransmitSerialMessageRawRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
        }

        private void frmTransmitSerialMessageSendBtn_Click(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
            if (clsGlobal.PerformOnAll)
            {
                foreach (string str in clsGlobal.g_objfrmMDIMain.PortManagerHash.Keys)
                {
                    if (!(str == clsGlobal.FilePlayBackPortName))
                    {
                        PortManager manager = (PortManager) clsGlobal.g_objfrmMDIMain.PortManagerHash[str];
                        if ((manager.comm != null) && manager.comm.IsSourceDeviceOpen())
                        {
                            manager.comm.WriteData(this.frmTransmitSerialMessageOutputText.Text);
                        }
                    }
                }
            }
            else
            {
                this.comm.WriteData(this.frmTransmitSerialMessageOutputText.Text);
            }
        }

        private void frmTransmitSerialMessageSsbRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
        }

        private void frmTransmitSerialMessageTTBRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateRawDataToSendBox();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmTransmitSerialMessage));
            this.frmTransmitSerialMessageTextLabel = new Label();
            this.frmTransmitSerialMessageInputText = new TextBox();
            this.frmTransmitSerialMessageProtocolWrapperGroupBox = new GroupBox();
            this.frmTransmitSerialMessageRawRadioBtn = new RadioButton();
            this.frmTransmitSerialMessageNmeaRadioBtn = new RadioButton();
            this.frmTransmitSerialMessageTTBRadioBtn = new RadioButton();
            this.frmTransmitSerialMessageAi3RadioBtn = new RadioButton();
            this.frmTransmitSerialMessageFRadioBtn = new RadioButton();
            this.frmTransmitSerialMessageOspRadioBtn = new RadioButton();
            this.frmTransmitSerialMessageSsbRadioBtn = new RadioButton();
            this.frmTransmitSerialMessageOutputText = new TextBox();
            this.frmTransmitSerialMessageSendBtn = new Button();
            this.frmTransmitSerialCancelBtn = new Button();
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.SuspendLayout();
            base.SuspendLayout();
            this.frmTransmitSerialMessageTextLabel.AutoSize = true;
            this.frmTransmitSerialMessageTextLabel.Location = new Point(0x1f, 0x1c);
            this.frmTransmitSerialMessageTextLabel.Name = "frmTransmitSerialMessageTextLabel";
            this.frmTransmitSerialMessageTextLabel.Size = new Size(0x18f, 13);
            this.frmTransmitSerialMessageTextLabel.TabIndex = 0;
            this.frmTransmitSerialMessageTextLabel.Text = "Specify in Hex (eg. 55 AB 6D ...) or Text depending on 'Protocol Wrapper' selection";
            this.frmTransmitSerialMessageInputText.Location = new Point(0x22, 50);
            this.frmTransmitSerialMessageInputText.Multiline = true;
            this.frmTransmitSerialMessageInputText.Name = "frmTransmitSerialMessageInputText";
            this.frmTransmitSerialMessageInputText.Size = new Size(0x1dc, 0x1c);
            this.frmTransmitSerialMessageInputText.TabIndex = 0;
            this.frmTransmitSerialMessageInputText.PreviewKeyDown += new PreviewKeyDownEventHandler(this.frmTransmitSerialMessageInputText_PreviewKeyDown);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Controls.Add(this.frmTransmitSerialMessageRawRadioBtn);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Controls.Add(this.frmTransmitSerialMessageNmeaRadioBtn);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Controls.Add(this.frmTransmitSerialMessageTTBRadioBtn);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Controls.Add(this.frmTransmitSerialMessageAi3RadioBtn);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Controls.Add(this.frmTransmitSerialMessageFRadioBtn);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Controls.Add(this.frmTransmitSerialMessageOspRadioBtn);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Controls.Add(this.frmTransmitSerialMessageSsbRadioBtn);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Location = new Point(0x22, 0x61);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Name = "frmTransmitSerialMessageProtocolWrapperGroupBox";
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Size = new Size(0xdf, 0x75);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.TabIndex = 1;
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.TabStop = false;
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.Text = "Protocol Wrapper";
            this.frmTransmitSerialMessageRawRadioBtn.AutoSize = true;
            this.frmTransmitSerialMessageRawRadioBtn.Location = new Point(30, 0x4c);
            this.frmTransmitSerialMessageRawRadioBtn.Name = "frmTransmitSerialMessageRawRadioBtn";
            this.frmTransmitSerialMessageRawRadioBtn.Size = new Size(0x2f, 0x11);
            this.frmTransmitSerialMessageRawRadioBtn.TabIndex = 6;
            this.frmTransmitSerialMessageRawRadioBtn.TabStop = true;
            this.frmTransmitSerialMessageRawRadioBtn.Text = "&Raw";
            this.frmTransmitSerialMessageRawRadioBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageRawRadioBtn.CheckedChanged += new EventHandler(this.frmTransmitSerialMessageRawRadioBtn_CheckedChanged);
            this.frmTransmitSerialMessageNmeaRadioBtn.AutoSize = true;
            this.frmTransmitSerialMessageNmeaRadioBtn.Location = new Point(30, 0x35);
            this.frmTransmitSerialMessageNmeaRadioBtn.Name = "frmTransmitSerialMessageNmeaRadioBtn";
            this.frmTransmitSerialMessageNmeaRadioBtn.Size = new Size(0x38, 0x11);
            this.frmTransmitSerialMessageNmeaRadioBtn.TabIndex = 5;
            this.frmTransmitSerialMessageNmeaRadioBtn.TabStop = true;
            this.frmTransmitSerialMessageNmeaRadioBtn.Text = "&NMEA";
            this.frmTransmitSerialMessageNmeaRadioBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageNmeaRadioBtn.CheckedChanged += new EventHandler(this.frmTransmitSerialMessageNmeaRadioBtn_CheckedChanged);
            this.frmTransmitSerialMessageTTBRadioBtn.AutoSize = true;
            this.frmTransmitSerialMessageTTBRadioBtn.Location = new Point(0x9d, 0x19);
            this.frmTransmitSerialMessageTTBRadioBtn.Name = "frmTransmitSerialMessageTTBRadioBtn";
            this.frmTransmitSerialMessageTTBRadioBtn.Size = new Size(0x2e, 0x11);
            this.frmTransmitSerialMessageTTBRadioBtn.TabIndex = 4;
            this.frmTransmitSerialMessageTTBRadioBtn.TabStop = true;
            this.frmTransmitSerialMessageTTBRadioBtn.Text = "&TTB";
            this.frmTransmitSerialMessageTTBRadioBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageTTBRadioBtn.CheckedChanged += new EventHandler(this.frmTransmitSerialMessageTTBRadioBtn_CheckedChanged);
            this.frmTransmitSerialMessageAi3RadioBtn.AutoSize = true;
            this.frmTransmitSerialMessageAi3RadioBtn.Location = new Point(0x5c, 0x4c);
            this.frmTransmitSerialMessageAi3RadioBtn.Name = "frmTransmitSerialMessageAi3RadioBtn";
            this.frmTransmitSerialMessageAi3RadioBtn.Size = new Size(40, 0x11);
            this.frmTransmitSerialMessageAi3RadioBtn.TabIndex = 3;
            this.frmTransmitSerialMessageAi3RadioBtn.TabStop = true;
            this.frmTransmitSerialMessageAi3RadioBtn.Text = "&Ai3";
            this.frmTransmitSerialMessageAi3RadioBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageAi3RadioBtn.CheckedChanged += new EventHandler(this.frmTransmitSerialMessageAi3RadioBtn_CheckedChanged);
            this.frmTransmitSerialMessageFRadioBtn.AutoSize = true;
            this.frmTransmitSerialMessageFRadioBtn.Location = new Point(0x5c, 0x35);
            this.frmTransmitSerialMessageFRadioBtn.Name = "frmTransmitSerialMessageFRadioBtn";
            this.frmTransmitSerialMessageFRadioBtn.Size = new Size(0x1f, 0x11);
            this.frmTransmitSerialMessageFRadioBtn.TabIndex = 2;
            this.frmTransmitSerialMessageFRadioBtn.TabStop = true;
            this.frmTransmitSerialMessageFRadioBtn.Text = "&F";
            this.frmTransmitSerialMessageFRadioBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageFRadioBtn.CheckedChanged += new EventHandler(this.frmTransmitSerialMessageFRadioBtn_CheckedChanged);
            this.frmTransmitSerialMessageOspRadioBtn.AutoSize = true;
            this.frmTransmitSerialMessageOspRadioBtn.Location = new Point(30, 0x19);
            this.frmTransmitSerialMessageOspRadioBtn.Name = "frmTransmitSerialMessageOspRadioBtn";
            this.frmTransmitSerialMessageOspRadioBtn.Size = new Size(0x2f, 0x11);
            this.frmTransmitSerialMessageOspRadioBtn.TabIndex = 1;
            this.frmTransmitSerialMessageOspRadioBtn.TabStop = true;
            this.frmTransmitSerialMessageOspRadioBtn.Text = "&OSP";
            this.frmTransmitSerialMessageOspRadioBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageOspRadioBtn.CheckedChanged += new EventHandler(this.frmTransmitSerialMessageOspRadioBtn_CheckedChanged);
            this.frmTransmitSerialMessageSsbRadioBtn.AutoSize = true;
            this.frmTransmitSerialMessageSsbRadioBtn.Location = new Point(0x5c, 0x19);
            this.frmTransmitSerialMessageSsbRadioBtn.Name = "frmTransmitSerialMessageSsbRadioBtn";
            this.frmTransmitSerialMessageSsbRadioBtn.Size = new Size(0x31, 0x11);
            this.frmTransmitSerialMessageSsbRadioBtn.TabIndex = 0;
            this.frmTransmitSerialMessageSsbRadioBtn.TabStop = true;
            this.frmTransmitSerialMessageSsbRadioBtn.Text = "&SSB ";
            this.frmTransmitSerialMessageSsbRadioBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageSsbRadioBtn.CheckedChanged += new EventHandler(this.frmTransmitSerialMessageSsbRadioBtn_CheckedChanged);
            this.frmTransmitSerialMessageOutputText.Location = new Point(0x22, 0xf4);
            this.frmTransmitSerialMessageOutputText.Multiline = true;
            this.frmTransmitSerialMessageOutputText.Name = "frmTransmitSerialMessageOutputText";
            this.frmTransmitSerialMessageOutputText.ReadOnly = true;
            this.frmTransmitSerialMessageOutputText.Size = new Size(0x1dc, 0x23);
            this.frmTransmitSerialMessageOutputText.TabIndex = 1;
            this.frmTransmitSerialMessageSendBtn.Location = new Point(0x163, 0x74);
            this.frmTransmitSerialMessageSendBtn.Name = "frmTransmitSerialMessageSendBtn";
            this.frmTransmitSerialMessageSendBtn.Size = new Size(0x4b, 0x17);
            this.frmTransmitSerialMessageSendBtn.TabIndex = 2;
            this.frmTransmitSerialMessageSendBtn.Text = "&Send";
            this.frmTransmitSerialMessageSendBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialMessageSendBtn.Click += new EventHandler(this.frmTransmitSerialMessageSendBtn_Click);
            this.frmTransmitSerialCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.frmTransmitSerialCancelBtn.Location = new Point(0x163, 0x9a);
            this.frmTransmitSerialCancelBtn.Name = "frmTransmitSerialCancelBtn";
            this.frmTransmitSerialCancelBtn.Size = new Size(0x4b, 0x17);
            this.frmTransmitSerialCancelBtn.TabIndex = 3;
            this.frmTransmitSerialCancelBtn.Text = "&Cancel";
            this.frmTransmitSerialCancelBtn.UseVisualStyleBackColor = true;
            this.frmTransmitSerialCancelBtn.Click += new EventHandler(this.frmTransmitSerialCancelBtn_Click);
            base.AcceptButton = this.frmTransmitSerialMessageSendBtn;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.frmTransmitSerialCancelBtn;
            base.ClientSize = new Size(0x218, 0x139);
            base.Controls.Add(this.frmTransmitSerialCancelBtn);
            base.Controls.Add(this.frmTransmitSerialMessageSendBtn);
            base.Controls.Add(this.frmTransmitSerialMessageProtocolWrapperGroupBox);
            base.Controls.Add(this.frmTransmitSerialMessageOutputText);
            base.Controls.Add(this.frmTransmitSerialMessageInputText);
            base.Controls.Add(this.frmTransmitSerialMessageTextLabel);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmTransmitSerialMessage";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Transmit Serial Message";
            base.Load += new EventHandler(this.frmTransmitSerialMessage_Load);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.ResumeLayout(false);
            this.frmTransmitSerialMessageProtocolWrapperGroupBox.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        protected override void OnClosed(EventArgs e)
        {
            if ((this.comm != null) && (this.UpdatePortManager != null))
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, false);
            }
            base.OnClosed(e);
        }

        private void UpdateRawDataToSendBox()
        {
            if (this.frmTransmitSerialMessageInputText.Text.Length != 0)
            {
                this.frmTransmitSerialMessageInputText.Text = this.frmTransmitSerialMessageInputText.Text.Replace("\r\n", "");
                StringBuilder builder = new StringBuilder();
                if (this.frmTransmitSerialMessageNmeaRadioBtn.Checked)
                {
                    this.frmTransmitSerialMessageOutputText.Text = NMEAReceiver.NMEA_AddCheckSum(this.frmTransmitSerialMessageInputText.Text);
                }
                else if (this.frmTransmitSerialMessageRawRadioBtn.Checked)
                {
                    this.frmTransmitSerialMessageOutputText.Text = this.frmTransmitSerialMessageInputText.Text;
                }
                else
                {
                    builder.Append("A0A2");
                    string str = this.frmTransmitSerialMessageInputText.Text.Replace(" ", "").Replace("\r", "").Replace("\n", "");
                    int num = str.Length / 2;
                    bool slcRx = false;
                    try
                    {
                        if (((this.comm._rxType == CommunicationManager.ReceiverType.SLC) || (this.comm._rxType == CommunicationManager.ReceiverType.TTB)) && (this.comm.MessageProtocol != "OSP"))
                        {
                            num++;
                            slcRx = true;
                        }
                        if (this.frmTransmitSerialMessageSsbRadioBtn.Checked)
                        {
                            if (this.comm._rxType == CommunicationManager.ReceiverType.SLC)
                            {
                                str = "EE" + str;
                            }
                        }
                        else if (!this.frmTransmitSerialMessageOspRadioBtn.Checked)
                        {
                            if (this.frmTransmitSerialMessageFRadioBtn.Checked)
                            {
                                str = "02" + str;
                                num++;
                            }
                            else if (this.frmTransmitSerialMessageAi3RadioBtn.Checked)
                            {
                                str = "01" + str;
                                num++;
                            }
                            else if (this.frmTransmitSerialMessageTTBRadioBtn.Checked)
                            {
                                str = "CC" + str;
                                num++;
                            }
                        }
                        builder.Append(num.ToString("X").PadLeft(4, '0'));
                        builder.Append(str);
                        builder.Append(this.comm.m_Protocols.GetChecksum(str, slcRx));
                        builder.Append("B0B3");
                        this.frmTransmitSerialMessageOutputText.Text = builder.ToString();
                    }
                    catch
                    {
                        MessageBox.Show("Error in building message", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        public delegate void UpdateWindowEventHandler(string titleString, int left, int top, int width, int height, bool state);
    }
}

