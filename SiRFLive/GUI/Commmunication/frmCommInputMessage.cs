﻿namespace SiRFLive.GUI.Commmunication
{
    using CommonClassLibrary;
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.GUI.DlgsInputMsg;
    using SiRFLive.MessageHandling;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    public class frmCommInputMessage : Form
    {
        private int _defaultWidth;
        private int _maxItemWidth;
        private static int _numberOpen;
        private string _persistedWindowName = "Input Command Window";
        private Button btnDefaultValues;
        private Button btnExit;
        private Button btnImport;
        private Button btnReload;
        private Button btnSendInput;
        private ComboBox cboMessages;
        private ComboBox cboProtocols;
        private CheckBox chkboxSLC;
        private CommunicationManager comm;
        private IContainer components;
        private int currentSelectCellIdx;
        private int currentSelectRowIdx;
        private DataGridView dataGridViewMsgStructure;
        private bool editInputStrFlag;
        private DataGridViewTextBoxColumn fieldExample;
        private DataGridViewTextBoxColumn fieldName;
        private DataGridViewTextBoxColumn fieldValue;
        private Label label_MessageIDs;
        private Label label6;
        private RichTextBox txtConvertedString;
        public int WinHeight;
        public int WinLeft;
        public int WinTop;
        public int WinWidth;

        public event updateParentEventHandler updateMainWindow;

        public event UpdateWindowEventHandler UpdatePortManager;

        public frmCommInputMessage()
        {
            this.InitializeComponent();
            this._defaultWidth = this.cboMessages.DropDownWidth;
            this._maxItemWidth = this.cboMessages.DropDownWidth;
            _numberOpen++;
            this._persistedWindowName = "Input command window " + _numberOpen.ToString();
            this.chkboxSLC.Enabled = false;
            this.btnSendInput.Enabled = false;
            base.MdiParent = clsGlobal.g_objfrmMDIMain;
        }

        private void AddChannelIDRow()
        {
            if ((this.cboProtocols.SelectedIndex != -1) && (this.cboMessages.SelectedIndex != -1))
            {
                if (this.cboProtocols.SelectedItem.ToString() == "SSB")
                {
                    DataGridViewRow dataGridViewRow = new DataGridViewRow();
                    this.dataGridViewMsgStructure.Rows.Insert(0, dataGridViewRow);
                    this.dataGridViewMsgStructure.Rows[0].Cells[0].Value = "Channel ID";
                    this.dataGridViewMsgStructure.Rows[0].Cells[1].Value = "238";
                    this.dataGridViewMsgStructure.Rows[0].Cells[2].Value = "238";
                }
                else if (this.cboProtocols.SelectedItem.ToString() == "F")
                {
                    DataGridViewRow row2 = new DataGridViewRow();
                    this.dataGridViewMsgStructure.Rows.Insert(0, row2);
                    this.dataGridViewMsgStructure.Rows[0].Cells[0].Value = "Channel ID";
                    this.dataGridViewMsgStructure.Rows[0].Cells[1].Value = "2";
                    this.dataGridViewMsgStructure.Rows[0].Cells[2].Value = "2";
                }
                else if (this.cboProtocols.SelectedItem.ToString() == "AI3")
                {
                    DataGridViewRow row3 = new DataGridViewRow();
                    this.dataGridViewMsgStructure.Rows.Insert(0, row3);
                    this.dataGridViewMsgStructure.Rows[0].Cells[0].Value = "Channel ID";
                    this.dataGridViewMsgStructure.Rows[0].Cells[1].Value = "1";
                    this.dataGridViewMsgStructure.Rows[0].Cells[2].Value = "1";
                }
            }
        }

        private void btnDefaultValues_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewMsgStructure.Rows.GetRowCount(DataGridViewElementStates.None) > 0)
            {
                for (int i = 0; i < this.dataGridViewMsgStructure.Rows.GetRowCount(DataGridViewElementStates.None); i++)
                {
                    this.dataGridViewMsgStructure.Rows[i].Cells[1].Value = this.dataGridViewMsgStructure.Rows[i].Cells[2].Value;
                }
                this.ConvertInputDataToSend(this.cboProtocols.SelectedItem.ToString());
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select xml input file";
            dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                XmlDocument document = new XmlDocument();
                document.Load(dialog.FileName);
                XmlNodeList list = document.SelectNodes("/message/field");
                this.dataGridViewMsgStructure.Rows.Clear();
                try
                {
                    XmlNode node = list.Item(0);
                    if (node != null)
                    {
                        this.cboProtocols.SelectedIndex = this.cboProtocols.FindString(node.ParentNode.Attributes["protocol"].Value);
                        this.cboMessages.Items.Clear();
                        this.cboMessages.Text = node.ParentNode.Attributes["mid"].Value + " - " + node.ParentNode.Attributes["name"].Value;
                        foreach (XmlNode node2 in list)
                        {
                            int num = this.dataGridViewMsgStructure.Rows.Add();
                            this.dataGridViewMsgStructure.Rows[num].Cells[0].Value = node2.Attributes["name"].Value;
                            this.dataGridViewMsgStructure.Rows[num].Cells[1].Value = node2.Attributes["value"].Value;
                        }
                        this.btnSendInput.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("The XML document failed to load because it contains no /message/field nodes.");
                    }
                }
                catch
                {
                    MessageBox.Show("The XML document failed to load because some fields do not contain name or value attributes.");
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.comm != null)
                {
                    this.comm.m_Protocols.MsgFactoryInit(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols.xml");
                    this.updateMessage();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btnSendInput_Click(object sender, EventArgs e)
        {
            string protocol = this.cboProtocols.SelectedItem.ToString();
            string msg = string.Empty;
            if (!this.editInputStrFlag)
            {
                this.ConvertInputDataToSend(protocol);
                switch (protocol)
                {
                    case "NMEA":
                        this.comm.TxCurrentTransmissionType = CommunicationManager.TransmissionType.Text;
                        this.comm.CMC.TxCurrentTransmissionType = CommonClass.TransmissionType.Text;
                        msg = this.txtConvertedString.Text;
                        break;

                    case "SSB":
                    case "OSP":
                    case "LPL":
                    case "F":
                    case "AI3":
                        this.comm.TxCurrentTransmissionType = CommunicationManager.TransmissionType.Hex;
                        this.comm.CMC.TxCurrentTransmissionType = CommonClass.TransmissionType.Hex;
                        msg = this.txtConvertedString.Text.Replace(" ", "");
                        break;
                }
            }
            else
            {
                this.comm.TxCurrentTransmissionType = CommunicationManager.TransmissionType.Hex;
                this.comm.CMC.TxCurrentTransmissionType = CommonClass.TransmissionType.Hex;
                msg = this.txtConvertedString.Text.Replace(" ", "");
            }
            if (clsGlobal.PerformOnAll)
            {
                foreach (string str3 in clsGlobal.g_objfrmMDIMain.PortManagerHash.Keys)
                {
                    if (!(str3 == clsGlobal.FilePlayBackPortName))
                    {
                        PortManager manager = (PortManager) clsGlobal.g_objfrmMDIMain.PortManagerHash[str3];
                        if ((manager != null) && manager.comm.IsSourceDeviceOpen())
                        {
                            manager.comm.WriteData(msg);
                        }
                    }
                }
                clsGlobal.PerformOnAll = false;
            }
            else
            {
                this.comm.WriteData(msg);
            }
            this.SaveInputData(protocol);
            try
            {
                if (this.cboMessages.Text.Contains("SiRFNav Start"))
                {
                    switch (Convert.ToInt32(this.dataGridViewMsgStructure.Rows[2].Cells[1].Value))
                    {
                        case 0:
                            this.comm.RxCtrl.ResetCtrl.ResetType = "AUTO";
                            return;

                        case 1:
                            this.comm.RxCtrl.ResetCtrl.ResetType = "HOT";
                            return;

                        case 2:
                            this.comm.RxCtrl.ResetCtrl.ResetType = "WARM_NO_INIT";
                            return;

                        case 3:
                            this.comm.RxCtrl.ResetCtrl.ResetType = "COLD";
                            return;

                        case 4:
                            this.comm.RxCtrl.ResetCtrl.ResetType = "FACTORY";
                            return;

                        case 5:
                            this.comm.RxCtrl.ResetCtrl.ResetType = "TEST";
                            return;

                        case 6:
                            this.comm.RxCtrl.ResetCtrl.ResetType = "FACTORY_XO";
                            return;
                    }
                    this.comm.RxCtrl.ResetCtrl.ResetType = "UNKNOWN";
                }
            }
            catch
            {
            }
        }

        private void btnSendUserString_Click(object sender, EventArgs e)
        {
            if (this.txtConvertedString.Text.Length != 0)
            {
                this.comm.WriteData(this.txtConvertedString.Text);
            }
        }

        private void cboMessages_DropDown(object sender, EventArgs e)
        {
            this.cboMessages.Width = this._maxItemWidth;
        }

        private void cboMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.updateMessage();
        }

        private void cboProtocols_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadInputMessageList(this.cboProtocols.SelectedItem.ToString());
            this.cboMessages.Text = "";
            this.dataGridViewMsgStructure.Rows.Clear();
            this.txtConvertedString.Text = "";
        }

        private void chkboxSLC_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkboxSLC.Checked)
            {
                if (this.comm.MessageProtocol != "OSP")
                {
                    this.AddChannelIDRow();
                }
                this.comm.RxType = CommunicationManager.ReceiverType.SLC;
            }
            else
            {
                this.RemoveChannelIDRow();
                this.comm.RxType = CommunicationManager.ReceiverType.GSW;
            }
            if ((this.cboProtocols.SelectedItem != null) && (this.cboMessages.SelectedItem != null))
            {
                this.ConvertInputDataToSend(this.cboProtocols.SelectedItem.ToString());
            }
        }

        private void ConvertInputDataToSend(string protocol)
        {
            string str = this.cboMessages.Text.ToString();
            string messageName = str.Substring(str.IndexOf(" - ") + 3);
            StringBuilder builder = new StringBuilder();
            builder.Append(this.dataGridViewMsgStructure.Rows[0].Cells[1].FormattedValue.ToString());
            for (int i = 1; i < this.dataGridViewMsgStructure.Rows.GetRowCount(DataGridViewElementStates.None); i++)
            {
                if (string.IsNullOrEmpty(this.dataGridViewMsgStructure.Rows[i].Cells[1].FormattedValue.ToString()))
                {
                    builder.Append(",");
                }
                else
                {
                    builder.Append(",");
                    builder.Append(this.dataGridViewMsgStructure.Rows[i].Cells[1].FormattedValue.ToString());
                }
            }
            string csvMessage = builder.ToString();
            this.txtConvertedString.Text = this.comm.m_Protocols.ConvertFieldsToRaw(csvMessage, messageName, protocol);
        }

        private void dataGridViewMsgStructure_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string str2;
                int rowIndex = e.RowIndex;
                if (((str2 = this.dataGridViewMsgStructure.Rows[rowIndex].Cells[0].Value.ToString()) != null) && (str2 == "Reset Config Bitmap"))
                {
                    frmRXInit childInstance = frmRXInit.GetChildInstance();
                    if (childInstance.IsDisposed)
                    {
                        childInstance = new frmRXInit();
                    }
                    this.currentSelectCellIdx = 1;
                    this.currentSelectRowIdx = rowIndex;
                    childInstance.updateParent += new frmRXInit.updateParentEventHandler(this.updateConfigList);
                    childInstance.MdiParent = base.MdiParent;
                    childInstance.Show();
                }
            }
            catch
            {
            }
        }

        private void dataGridViewMsgStructure_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.ConvertInputDataToSend(this.cboProtocols.SelectedItem.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmCommInputMessage_Load(object sender, EventArgs e)
        {
            if (this.comm != null)
            {
                this.comm.m_Protocols.MsgFactoryInit(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols.xml");
            }
            if (this.WinTop != 0)
            {
                base.Top = this.WinTop;
            }
            if (this.WinLeft != 0)
            {
                base.Left = this.WinLeft;
            }
            if (this.WinWidth != 0)
            {
                base.Width = this.WinWidth;
            }
            if (this.WinHeight != 0)
            {
                base.Height = this.WinHeight;
            }
        }

        private void frmCommInputMessage_LocationChanged(object sender, EventArgs e)
        {
            this.WinTop = base.Top;
            this.WinLeft = base.Left;
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, true);
            }
        }

        private void frmCommInputMessage_ResizeEnd(object sender, EventArgs e)
        {
            this.WinWidth = base.Width;
            this.WinHeight = base.Height;
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, true);
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmCommInputMessage));
            this.btnImport = new Button();
            this.btnDefaultValues = new Button();
            this.cboProtocols = new ComboBox();
            this.label6 = new Label();
            this.txtConvertedString = new RichTextBox();
            this.btnSendInput = new Button();
            this.dataGridViewMsgStructure = new DataGridView();
            this.fieldName = new DataGridViewTextBoxColumn();
            this.fieldValue = new DataGridViewTextBoxColumn();
            this.fieldExample = new DataGridViewTextBoxColumn();
            this.label_MessageIDs = new Label();
            this.cboMessages = new ComboBox();
            this.chkboxSLC = new CheckBox();
            this.btnReload = new Button();
            this.btnExit = new Button();
            ((ISupportInitialize) this.dataGridViewMsgStructure).BeginInit();
            base.SuspendLayout();
            this.btnImport.Location = new Point(0x238, 0x5c);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new Size(0x65, 0x17);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "&Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new EventHandler(this.btnImport_Click);
            this.btnDefaultValues.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.btnDefaultValues.Location = new Point(0x238, 0x7e);
            this.btnDefaultValues.Name = "btnDefaultValues";
            this.btnDefaultValues.Size = new Size(0x65, 0x17);
            this.btnDefaultValues.TabIndex = 6;
            this.btnDefaultValues.Text = "&Default Values";
            this.btnDefaultValues.UseVisualStyleBackColor = true;
            this.btnDefaultValues.Click += new EventHandler(this.btnDefaultValues_Click);
            this.cboProtocols.FormattingEnabled = true;
            this.cboProtocols.Location = new Point(80, 10);
            this.cboProtocols.Name = "cboProtocols";
            this.cboProtocols.Size = new Size(0x54, 0x15);
            this.cboProtocols.TabIndex = 0;
            this.cboProtocols.SelectedIndexChanged += new EventHandler(this.cboProtocols_SelectedIndexChanged);
            this.label6.AutoSize = true;
            this.label6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label6.Location = new Point(15, 14);
            this.label6.Name = "label6";
            this.label6.Size = new Size(60, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Protocols";
            this.txtConvertedString.Location = new Point(0x11, 0x143);
            this.txtConvertedString.Name = "txtConvertedString";
            this.txtConvertedString.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.txtConvertedString.Size = new Size(0x210, 0x17);
            this.txtConvertedString.TabIndex = 7;
            this.txtConvertedString.TabStop = false;
            this.txtConvertedString.Text = "";
            this.txtConvertedString.TextChanged += new EventHandler(this.txtConvertedString_TextChanged);
            this.btnSendInput.Enabled = false;
            this.btnSendInput.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.btnSendInput.Location = new Point(0x238, 0x143);
            this.btnSendInput.Name = "btnSendInput";
            this.btnSendInput.Size = new Size(0x65, 0x17);
            this.btnSendInput.TabIndex = 8;
            this.btnSendInput.Text = "&Send";
            this.btnSendInput.UseVisualStyleBackColor = true;
            this.btnSendInput.Click += new EventHandler(this.btnSendInput_Click);
            this.dataGridViewMsgStructure.AllowUserToAddRows = false;
            this.dataGridViewMsgStructure.AllowUserToDeleteRows = false;
            this.dataGridViewMsgStructure.AllowUserToResizeRows = false;
            this.dataGridViewMsgStructure.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMsgStructure.Columns.AddRange(new DataGridViewColumn[] { this.fieldName, this.fieldValue, this.fieldExample });
            this.dataGridViewMsgStructure.Location = new Point(0x11, 0x26);
            this.dataGridViewMsgStructure.Name = "dataGridViewMsgStructure";
            this.dataGridViewMsgStructure.RowHeadersVisible = false;
            this.dataGridViewMsgStructure.Size = new Size(0x210, 0x10d);
            this.dataGridViewMsgStructure.TabIndex = 3;
            this.dataGridViewMsgStructure.CellContentDoubleClick += new DataGridViewCellEventHandler(this.dataGridViewMsgStructure_CellDoubleClick);
            this.dataGridViewMsgStructure.CellEndEdit += new DataGridViewCellEventHandler(this.dataGridViewMsgStructure_CellEndEdit);
            this.fieldName.DividerWidth = 2;
            this.fieldName.HeaderText = "Field Name";
            this.fieldName.Name = "fieldName";
            this.fieldName.ReadOnly = true;
            this.fieldName.Width = 0x7d;
            this.fieldValue.DividerWidth = 2;
            this.fieldValue.HeaderText = "Field Value";
            this.fieldValue.Name = "fieldValue";
            this.fieldValue.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.fieldValue.Width = 200;
            this.fieldExample.HeaderText = "Example";
            this.fieldExample.Name = "fieldExample";
            this.fieldExample.ReadOnly = true;
            this.fieldExample.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.fieldExample.Width = 200;
            this.label_MessageIDs.AutoSize = true;
            this.label_MessageIDs.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label_MessageIDs.Location = new Point(0xea, 14);
            this.label_MessageIDs.Name = "label_MessageIDs";
            this.label_MessageIDs.Size = new Size(0x3f, 13);
            this.label_MessageIDs.TabIndex = 2;
            this.label_MessageIDs.Text = "Messages";
            this.cboMessages.FormattingEnabled = true;
            this.cboMessages.ImeMode = ImeMode.NoControl;
            this.cboMessages.Location = new Point(0x12e, 10);
            this.cboMessages.Name = "cboMessages";
            this.cboMessages.Size = new Size(0x16f, 0x15);
            this.cboMessages.TabIndex = 2;
            this.cboMessages.SelectedIndexChanged += new EventHandler(this.cboMessages_SelectedIndexChanged);
            this.cboMessages.DropDown += new EventHandler(this.cboMessages_DropDown);
            this.chkboxSLC.AutoSize = true;
            this.chkboxSLC.Location = new Point(170, 12);
            this.chkboxSLC.Name = "chkboxSLC";
            this.chkboxSLC.Size = new Size(0x3e, 0x11);
            this.chkboxSLC.TabIndex = 1;
            this.chkboxSLC.Text = "SLC Rx";
            this.chkboxSLC.UseVisualStyleBackColor = true;
            this.chkboxSLC.CheckedChanged += new EventHandler(this.chkboxSLC_CheckedChanged);
            this.btnReload.Location = new Point(0x238, 0x3a);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new Size(0x65, 0x17);
            this.btnReload.TabIndex = 4;
            this.btnReload.Text = "&Reload Definition";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new EventHandler(this.btnReload_Click);
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new Point(0x238, 0xb5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new Size(0x65, 0x17);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "E&xit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new EventHandler(this.btnExit_Click);
            base.AcceptButton = this.btnSendInput;
            this.AllowDrop = true;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            base.CancelButton = this.btnExit;
            base.ClientSize = new Size(690, 0x16c);
            base.Controls.Add(this.btnExit);
            base.Controls.Add(this.btnReload);
            base.Controls.Add(this.chkboxSLC);
            base.Controls.Add(this.btnImport);
            base.Controls.Add(this.btnDefaultValues);
            base.Controls.Add(this.cboProtocols);
            base.Controls.Add(this.label6);
            base.Controls.Add(this.txtConvertedString);
            base.Controls.Add(this.btnSendInput);
            base.Controls.Add(this.dataGridViewMsgStructure);
            base.Controls.Add(this.label_MessageIDs);
            base.Controls.Add(this.cboMessages);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmCommInputMessage";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.Manual;
            this.Text = "Input Message";
            base.Load += new EventHandler(this.frmCommInputMessage_Load);
            base.LocationChanged += new EventHandler(this.frmCommInputMessage_LocationChanged);
            base.ResizeEnd += new EventHandler(this.frmCommInputMessage_ResizeEnd);
            ((ISupportInitialize) this.dataGridViewMsgStructure).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadInputMessageList(string protocol)
        {
            this.cboMessages.Items.Clear();
            List<InputMsg> inputMessages = this.comm.m_Protocols.GetInputMessages(protocol);
            this._maxItemWidth = this._defaultWidth;
            for (int i = 0; i < inputMessages.Count; i++)
            {
                string str;
                if (inputMessages[i].subID == -1)
                {
                    str = inputMessages[i].messageID + " - " + inputMessages[i].messageName;
                }
                else
                {
                    str = string.Concat(new object[] { inputMessages[i].messageID, ",", inputMessages[i].subID, " - ", inputMessages[i].messageName });
                }
                this.cboMessages.Items.Add(str);
                if (this._maxItemWidth < (str.Length * 6))
                {
                    this._maxItemWidth = str.Length * 6;
                }
            }
            inputMessages.Clear();
        }

        private void LoadProtocolList()
        {
            ArrayList protocols = new ArrayList();
            protocols = this.comm.m_Protocols.GetProtocols();
            for (int i = 0; i < protocols.Count; i++)
            {
                if (!this.cboProtocols.Items.Contains(protocols[i]))
                {
                    if (this.comm.MessageProtocol == "NMEA")
                    {
                        if (!(((string) protocols[i]) == "NMEA"))
                        {
                            continue;
                        }
                        this.cboProtocols.Items.Add(protocols[i]);
                        break;
                    }
                    this.cboProtocols.Items.Add(protocols[i]);
                }
            }
            protocols.Clear();
        }

        protected override void OnClosed(EventArgs e)
        {
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

        private void RemoveChannelIDRow()
        {
            if (((this.cboProtocols.SelectedIndex != -1) && (this.cboMessages.SelectedIndex != -1)) && (((this.cboProtocols.SelectedItem.ToString() == "SSB") || (this.cboProtocols.SelectedItem.ToString() == "F")) || (this.cboProtocols.SelectedItem.ToString() == "AI3")))
            {
                this.dataGridViewMsgStructure.Rows.RemoveAt(0);
            }
        }

        private void SaveInputData(string protocol)
        {
            string text = this.cboMessages.Text;
            string messageName = text.Substring(text.IndexOf(" - ") + 3);
            StringBuilder builder = new StringBuilder();
            builder.Append(this.dataGridViewMsgStructure.Rows[0].Cells[1].FormattedValue.ToString());
            for (int i = 1; i < this.dataGridViewMsgStructure.Rows.GetRowCount(DataGridViewElementStates.None); i++)
            {
                if (string.IsNullOrEmpty(this.dataGridViewMsgStructure.Rows[i].Cells[1].FormattedValue.ToString()))
                {
                    builder.Append(",");
                    builder.Append("0");
                }
                else
                {
                    builder.Append(",");
                    builder.Append(this.dataGridViewMsgStructure.Rows[i].Cells[1].FormattedValue.ToString());
                }
            }
            string message = builder.ToString();
            this.comm.m_Protocols.SaveInputDataToFile(message, messageName, protocol);
        }

        private void txtConvertedString_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtConvertedString.Text.ToString()))
            {
                this.btnSendInput.Enabled = false;
            }
            else
            {
                this.btnSendInput.Enabled = true;
            }
        }

        internal void updateConfigList(string updatedData)
        {
            this.dataGridViewMsgStructure.Rows[this.currentSelectRowIdx].Cells[this.currentSelectCellIdx].Value = updatedData;
        }

        private void updateMessage()
        {
            this.cboMessages.Width = this._defaultWidth;
            string str = this.cboMessages.Text.ToString();
            int sid = -1;
            int mid = 0;
            if (str.IndexOf(',') >= 0)
            {
                string[] strArray = str.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                mid = Convert.ToInt32(strArray[0]);
                sid = Convert.ToInt32(strArray[1]);
            }
            else
            {
                mid = int.Parse(str.Remove(str.IndexOf(' ')));
            }
            string messageName = str.Substring(str.IndexOf(" - ") + 3);
            ArrayList list = new ArrayList();
            list = this.comm.m_Protocols.GetInputMessageStructure(mid, sid, messageName, this.cboProtocols.SelectedItem.ToString());
            if (list.Count == 0)
            {
                MessageBox.Show("Can't find message structure in protocol.xml file!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                this.dataGridViewMsgStructure.Rows.Clear();
                if (this.comm.RxType == CommunicationManager.ReceiverType.SLC)
                {
                    if (this.comm.MessageProtocol != "OSP")
                    {
                        this.AddChannelIDRow();
                    }
                    this.chkboxSLC.Checked = true;
                }
                this.txtConvertedString.Text = "";
                for (int i = 0; i < list.Count; i++)
                {
                    int num4 = this.dataGridViewMsgStructure.Rows.Add();
                    this.dataGridViewMsgStructure.Rows[num4].Cells[0].Value = ((InputMsg) list[i]).fieldName;
                    this.dataGridViewMsgStructure.Rows[num4].Cells[1].Value = ((InputMsg) list[i]).savedValue;
                    this.dataGridViewMsgStructure.Rows[num4].Cells[2].Value = ((InputMsg) list[i]).defaultValue;
                    if ((((InputMsg) list[i]).fieldName == "Message ID") || (((InputMsg) list[i]).fieldName == "Message Sub ID"))
                    {
                        this.dataGridViewMsgStructure.Rows[num4].Cells[1].ReadOnly = true;
                    }
                    if (((InputMsg) list[i]).fieldName == "Reset Config Bitmap")
                    {
                        this.dataGridViewMsgStructure.Rows[num4].Cells[0].Style.Font = new Font("Arial", 8f, FontStyle.Bold);
                        this.dataGridViewMsgStructure.Rows[num4].Cells[0].Style.ForeColor = Color.Red;
                        this.dataGridViewMsgStructure.Rows[num4].Cells[0].Style.BackColor = Color.Yellow;
                    }
                    if (((InputMsg) list[i]).datatype == "RAW_HEX")
                    {
                        this.dataGridViewMsgStructure.Rows[num4].Cells[0].Style.ForeColor = Color.Blue;
                        DataGridViewCell cell1 = this.dataGridViewMsgStructure.Rows[num4].Cells[0];
                        cell1.Value = cell1.Value + " (Hex String)";
                    }
                }
                this.ConvertInputDataToSend(this.cboProtocols.SelectedItem.ToString());
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
                this.LoadProtocolList();
                if (this.comm.RxType == CommunicationManager.ReceiverType.SLC)
                {
                    this.chkboxSLC.Checked = true;
                }
                else
                {
                    this.chkboxSLC.Checked = false;
                }
                this.Text = this.comm.sourceDeviceName + ": Input Message";
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

        public delegate void updateParentEventHandler(string titleString);

        public delegate void UpdateWindowEventHandler(string titleString, int left, int top, int width, int height, bool state);
    }
}

