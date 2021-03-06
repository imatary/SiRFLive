﻿namespace SiRFLive.GUI
{
    using SiRFLive.Communication;
    using SiRFLive.General;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public class frmErrorLogConfig : Form
    {
        private int _logType;
        private Button btn_Add;
        private Button btn_clear;
        private Button btn_clearAll;
        private CheckBox checkBox_SubStr;
        private CommunicationManager comm;
        private IContainer components;
        private DataGridView dataGridView1;
        private Button frmErrorLogConfigCancelBtn;
        private TextBox frmErrorLogConfigErrorStrTextBox;
        private Button frmErrorLogConfigOkBtn;
        private DataGridViewTextBoxColumn isSubString;
        private Label label_searchstr;
        private Label label1;
        private Label label2;
        private Button logConfigCancelBtn;
        private Button logConfigOkBtn;
        private DataGridViewTextBoxColumn searchString;
        private TextBox txtBox_searchStr;

        public frmErrorLogConfig(CommunicationManager parentComm, int logType)
        {
            this.InitializeComponent();
            this.comm = parentComm;
            this._logType = logType;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (this.txtBox_searchStr.Text == "")
            {
                MessageBox.Show("Please enter string to add", "Need more information");
            }
            else
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                int rowIndex = this.dataGridView1.RowCount - 1;
                this.dataGridView1.Rows.Insert(rowIndex, dataGridViewRow);
                this.dataGridView1.Rows[rowIndex].Cells[0].Value = this.txtBox_searchStr.Text;
                this.dataGridView1.Rows[rowIndex].Cells[1].Value = this.checkBox_SubStr.Checked ? "Y" : "N";
                this.txtBox_searchStr.Text = "";
                this.checkBox_SubStr.Checked = false;
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            try
            {
                if ((this.dataGridView1.SelectedRows.Count > 0) || (this.dataGridView1.SelectedCells.Count > 0))
                {
                    if (this.dataGridView1.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
                        {
                            this.dataGridView1.Rows.Remove(row);
                        }
                    }
                    else
                    {
                        foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
                        {
                            this.dataGridView1.Rows.RemoveAt(cell.RowIndex);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btn_clearAll_Click(object sender, EventArgs e)
        {
            int num = this.dataGridView1.RowCount - 1;
            if (num >= 1)
            {
                for (int i = 0; i < num; i++)
                {
                    this.dataGridView1.Rows.RemoveAt(0);
                }
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

        private void frmErrorLogConfig_Load(object sender, EventArgs e)
        {
            if (this.comm != null)
            {
                string[] strArray;
                if (this._logType == 0)
                {
                    strArray = this.comm.ErrorStringList.ToArray();
                    this.checkBox_SubStr.Checked = true;
                    this.checkBox_SubStr.Enabled = false;
                }
                else
                {
                    strArray = this.comm.UserSpecifiedMsgList.ToArray();
                    this.checkBox_SubStr.Checked = false;
                    this.checkBox_SubStr.Enabled = true;
                }
                string[] strArray2 = this.comm.UserSpecifiedSubStringList.ToArray();
                for (int i = 0; i < strArray.Length; i++)
                {
                    DataGridViewRow dataGridViewRow = new DataGridViewRow();
                    int rowIndex = this.dataGridView1.RowCount - 1;
                    this.dataGridView1.Rows.Insert(rowIndex, dataGridViewRow);
                    this.dataGridView1.Rows[rowIndex].Cells[0].Value = strArray[i];
                    if (this._logType == 0)
                    {
                        this.dataGridView1.Rows[rowIndex].Cells[1].Value = "Y";
                    }
                    else
                    {
                        this.dataGridView1.Rows[rowIndex].Cells[1].Value = strArray2[i];
                    }
                }
            }
        }

        private void frmErrorLogConfigCancelBtn_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void frmErrorLogConfigOkBtn_Click(object sender, EventArgs e)
        {
            StreamWriter writer;
            string path = string.Empty;
            if (this._logType == 0)
            {
                path = this.comm.ErrorCfgFilePath;
            }
            else
            {
                path = this.comm.UserSpecifiedLogCfgFilePath;
            }
            if ((this.comm == null) || !File.Exists(path))
            {
                goto Label_02F5;
            }
            if ((File.GetAttributes(this.comm.ErrorCfgFilePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                MessageBox.Show(string.Format("Readonly file\n{0}\nPlease change file property and retry", path), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int num = this.dataGridView1.RowCount - 1;
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            if (num < 1)
            {
                return;
            }
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            for (int i = 0; i < num; i++)
            {
                list.Add(this.dataGridView1.Rows[i].Cells[0].Value.ToString());
                list2.Add(this.dataGridView1.Rows[i].Cells[1].Value.ToString());
                builder.Append(this.dataGridView1.Rows[i].Cells[0].Value.ToString());
                builder.Append('%');
                builder2.Append(this.dataGridView1.Rows[i].Cells[1].Value.ToString());
                builder2.Append("%");
            }
            if (clsGlobal.PerformOnAll)
            {
                foreach (string str3 in clsGlobal.g_objfrmMDIMain.PortManagerHash.Keys)
                {
                    PortManager manager = (PortManager) clsGlobal.g_objfrmMDIMain.PortManagerHash[str3];
                    if ((manager == null) || (manager.comm == null))
                    {
                        continue;
                    }
                    if (this._logType == 0)
                    {
                        lock (manager.comm.LockErrorLog)
                        {
                            manager.comm.ErrorStringList = list;
                            goto Label_021C;
                        }
                    }
                    manager.comm.UserSpecifiedMsgList = list;
                Label_021C:
                    manager.comm.UserSpecifiedSubStringList = list2;
                }
                goto Label_0299;
            }
            if (this._logType == 0)
            {
                lock (this.comm.LockErrorLog)
                {
                    this.comm.ErrorStringList = list;
                    goto Label_028C;
                }
            }
            this.comm.UserSpecifiedMsgList = list;
        Label_028C:
            this.comm.UserSpecifiedSubStringList = list2;
        Label_0299:
            writer = new StreamWriter(path, false);
            writer.WriteLine(builder.ToString().TrimEnd(new char[] { '%' }));
            writer.WriteLine(builder2.ToString().TrimEnd(new char[] { '%' }));
            writer.Close();
            writer.Dispose();
        Label_02F5:
            base.Close();
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            DataGridViewCellStyle style2 = new DataGridViewCellStyle();
            DataGridViewCellStyle style3 = new DataGridViewCellStyle();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmErrorLogConfig));
            this.frmErrorLogConfigErrorStrTextBox = new TextBox();
            this.label1 = new Label();
            this.frmErrorLogConfigOkBtn = new Button();
            this.frmErrorLogConfigCancelBtn = new Button();
            this.label2 = new Label();
            this.btn_clear = new Button();
            this.btn_clearAll = new Button();
            this.btn_Add = new Button();
            this.checkBox_SubStr = new CheckBox();
            this.dataGridView1 = new DataGridView();
            this.searchString = new DataGridViewTextBoxColumn();
            this.isSubString = new DataGridViewTextBoxColumn();
            this.label_searchstr = new Label();
            this.txtBox_searchStr = new TextBox();
            this.logConfigOkBtn = new Button();
            this.logConfigCancelBtn = new Button();
            ((ISupportInitialize) this.dataGridView1).BeginInit();
            base.SuspendLayout();
            this.frmErrorLogConfigErrorStrTextBox.Location = new Point(0x63, 0x11);
            this.frmErrorLogConfigErrorStrTextBox.Name = "frmErrorLogConfigErrorStrTextBox";
            this.frmErrorLogConfigErrorStrTextBox.Size = new Size(0x18d, 20);
            this.frmErrorLogConfigErrorStrTextBox.TabIndex = 0;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x17, 0x15);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x40, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Error Strings";
            this.frmErrorLogConfigOkBtn.Location = new Point(0x93, 0x39);
            this.frmErrorLogConfigOkBtn.Name = "frmErrorLogConfigOkBtn";
            this.frmErrorLogConfigOkBtn.Size = new Size(0x4b, 0x17);
            this.frmErrorLogConfigOkBtn.TabIndex = 2;
            this.frmErrorLogConfigOkBtn.Text = "&OK";
            this.frmErrorLogConfigOkBtn.UseVisualStyleBackColor = true;
            this.frmErrorLogConfigCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.frmErrorLogConfigCancelBtn.Location = new Point(0xf9, 0x39);
            this.frmErrorLogConfigCancelBtn.Name = "frmErrorLogConfigCancelBtn";
            this.frmErrorLogConfigCancelBtn.Size = new Size(0x4b, 0x17);
            this.frmErrorLogConfigCancelBtn.TabIndex = 2;
            this.frmErrorLogConfigCancelBtn.Text = "&Cancel";
            this.frmErrorLogConfigCancelBtn.UseVisualStyleBackColor = true;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x15, 0x29);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "(separated by commas)";
            this.btn_clear.Location = new Point(0x215, 0x75);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new Size(0x39, 0x17);
            this.btn_clear.TabIndex = 70;
            this.btn_clear.Text = "Clea&r";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new EventHandler(this.btn_clear_Click);
            this.btn_clearAll.Location = new Point(0x215, 0x92);
            this.btn_clearAll.Name = "btn_clearAll";
            this.btn_clearAll.Size = new Size(0x39, 0x17);
            this.btn_clearAll.TabIndex = 0x47;
            this.btn_clearAll.Text = "Clear A&ll";
            this.btn_clearAll.UseVisualStyleBackColor = true;
            this.btn_clearAll.Click += new EventHandler(this.btn_clearAll_Click);
            this.btn_Add.Location = new Point(0x215, 0x2d);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new Size(0x39, 0x17);
            this.btn_Add.TabIndex = 0x45;
            this.btn_Add.Text = "&Add";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new EventHandler(this.btn_Add_Click);
            this.checkBox_SubStr.AutoSize = true;
            this.checkBox_SubStr.Location = new Point(0x1b2, 0x30);
            this.checkBox_SubStr.Name = "checkBox_SubStr";
            this.checkBox_SubStr.Size = new Size(0x4e, 0x11);
            this.checkBox_SubStr.TabIndex = 0x44;
            this.checkBox_SubStr.Text = "SubString?";
            this.checkBox_SubStr.UseVisualStyleBackColor = true;
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = SystemColors.Control;
            style.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style.ForeColor = SystemColors.WindowText;
            style.SelectionBackColor = SystemColors.Highlight;
            style.SelectionForeColor = SystemColors.HighlightText;
            style.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = style;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new DataGridViewColumn[] { this.searchString, this.isSubString });
            style2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style2.BackColor = SystemColors.Window;
            style2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style2.ForeColor = SystemColors.ControlText;
            style2.SelectionBackColor = SystemColors.Highlight;
            style2.SelectionForeColor = SystemColors.HighlightText;
            style2.WrapMode = DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = style2;
            this.dataGridView1.Location = new Point(0x20, 0x57);
            this.dataGridView1.Name = "dataGridView1";
            style3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style3.BackColor = SystemColors.Control;
            style3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style3.ForeColor = SystemColors.WindowText;
            style3.SelectionBackColor = SystemColors.Highlight;
            style3.SelectionForeColor = SystemColors.HighlightText;
            style3.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = style3;
            this.dataGridView1.Size = new Size(480, 0xb9);
            this.dataGridView1.TabIndex = 0x49;
            this.searchString.FillWeight = 340f;
            this.searchString.HeaderText = "String";
            this.searchString.Name = "searchString";
            this.searchString.ReadOnly = true;
            this.searchString.Width = 340;
            this.isSubString.HeaderText = "SubString? (Y/N)";
            this.isSubString.Name = "isSubString";
            this.isSubString.ReadOnly = true;
            this.label_searchstr.AutoSize = true;
            this.label_searchstr.Location = new Point(0x1d, 0x1d);
            this.label_searchstr.Name = "label_searchstr";
            this.label_searchstr.Size = new Size(160, 13);
            this.label_searchstr.TabIndex = 0x48;
            this.label_searchstr.Text = "String (example: 225,6 [id,subid])";
            this.txtBox_searchStr.Location = new Point(0x20, 0x2e);
            this.txtBox_searchStr.Name = "txtBox_searchStr";
            this.txtBox_searchStr.Size = new Size(0x185, 20);
            this.txtBox_searchStr.TabIndex = 0x43;
            this.logConfigOkBtn.Location = new Point(0x215, 0xaf);
            this.logConfigOkBtn.Name = "logConfigOkBtn";
            this.logConfigOkBtn.Size = new Size(0x39, 0x17);
            this.logConfigOkBtn.TabIndex = 0x4a;
            this.logConfigOkBtn.Text = "&Done";
            this.logConfigOkBtn.UseVisualStyleBackColor = true;
            this.logConfigOkBtn.Click += new EventHandler(this.frmErrorLogConfigOkBtn_Click);
            this.logConfigCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.logConfigCancelBtn.Location = new Point(0x215, 0xcc);
            this.logConfigCancelBtn.Name = "logConfigCancelBtn";
            this.logConfigCancelBtn.Size = new Size(0x39, 0x17);
            this.logConfigCancelBtn.TabIndex = 0x4a;
            this.logConfigCancelBtn.Text = "&Cancel";
            this.logConfigCancelBtn.UseVisualStyleBackColor = true;
            this.logConfigCancelBtn.Click += new EventHandler(this.frmErrorLogConfigCancelBtn_Click);
            base.CancelButton = this.logConfigCancelBtn;
            base.ClientSize = new Size(0x266, 0x133);
            base.Controls.Add(this.logConfigCancelBtn);
            base.Controls.Add(this.logConfigOkBtn);
            base.Controls.Add(this.btn_clear);
            base.Controls.Add(this.btn_clearAll);
            base.Controls.Add(this.btn_Add);
            base.Controls.Add(this.checkBox_SubStr);
            base.Controls.Add(this.dataGridView1);
            base.Controls.Add(this.label_searchstr);
            base.Controls.Add(this.txtBox_searchStr);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmErrorLogConfig";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.Load += new EventHandler(this.frmErrorLogConfig_Load);
            ((ISupportInitialize) this.dataGridView1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

