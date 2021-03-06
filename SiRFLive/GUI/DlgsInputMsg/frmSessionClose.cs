﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using SiRFLive.Communication;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmSessionClose : Form
    {
        private Button button_Cancel;
        private Button button_OK;
        private ComboBox comboBox_SessionCloseOrSuspend;
        private CommunicationManager comm;
        private IContainer components;

        public frmSessionClose()
        {
            this.InitializeComponent();
            this.comboBox_SessionCloseOrSuspend.SelectedIndex = 0;
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            byte sessionCloseReqInfo = 0;
            if (this.comboBox_SessionCloseOrSuspend.SelectedIndex == 1)
            {
                sessionCloseReqInfo = 0x80;
            }
            this.comm.RxCtrl.CloseSession(sessionCloseReqInfo);
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

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmSessionClose));
            this.button_OK = new Button();
            this.button_Cancel = new Button();
            this.comboBox_SessionCloseOrSuspend = new ComboBox();
            base.SuspendLayout();
            this.button_OK.Location = new Point(60, 0x3d);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new Size(0x4b, 0x17);
            this.button_OK.TabIndex = 1;
            this.button_OK.Text = "&OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new EventHandler(this.button_OK_Click);
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new Point(0x98, 0x3d);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new Size(0x4b, 0x17);
            this.button_Cancel.TabIndex = 2;
            this.button_Cancel.Text = "&Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new EventHandler(this.button_Cancel_Click);
            this.comboBox_SessionCloseOrSuspend.FormattingEnabled = true;
            this.comboBox_SessionCloseOrSuspend.Items.AddRange(new object[] { "Close Session", "Suspend Session" });
            this.comboBox_SessionCloseOrSuspend.Location = new Point(0x53, 0x15);
            this.comboBox_SessionCloseOrSuspend.Name = "comboBox_SessionCloseOrSuspend";
            this.comboBox_SessionCloseOrSuspend.Size = new Size(0x79, 0x15);
            this.comboBox_SessionCloseOrSuspend.TabIndex = 0;
            base.AcceptButton = this.button_OK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.button_Cancel;
            base.ClientSize = new Size(0x11e, 0x68);
            base.Controls.Add(this.button_OK);
            base.Controls.Add(this.button_Cancel);
            base.Controls.Add(this.comboBox_SessionCloseOrSuspend);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmSessionClose";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Close Session";
            base.ResumeLayout(false);
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
            }
        }
    }
}

