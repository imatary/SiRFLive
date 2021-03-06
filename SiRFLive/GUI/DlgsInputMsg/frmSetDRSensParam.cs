﻿namespace SiRFLive.GUI.DlgsInputMsg
{
    using SiRFLive.Communication;
    using SiRFLive.General;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    public class frmSetDRSensParam : Form
    {
        private Button btn_Cancel;
        private Button btn_Send;
        private CheckBox checkBox_useDefaultValues;
        private CommunicationManager comm;
        private IContainer components;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textBox_GyroBias;
        private TextBox textBox_GyroSclFac;
        private TextBox textBox_SpdSclFac;

        public frmSetDRSensParam(CommunicationManager parentComm)
        {
            this.InitializeComponent();
            this.comm = parentComm;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            string messageName = "Set DR Sensors Parameters";
            byte num = 0;
            byte num2 = 0;
            ushort num3 = 0;
            try
            {
                num = Convert.ToByte(this.textBox_SpdSclFac.Text);
                num2 = Convert.ToByte(this.textBox_GyroBias.Text);
                num3 = Convert.ToByte(this.textBox_GyroSclFac.Text);
            }
            catch
            {
                MessageBox.Show("Invalid input!", "Error", MessageBoxButtons.OK);
                return;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("172,4,{0},{1},{2}", num, num2, num3));
            string msg = this.comm.m_Protocols.ConvertFieldsToRaw(builder.ToString(), messageName, "SSB");
            if (clsGlobal.PerformOnAll)
            {
                foreach (string str3 in clsGlobal.g_objfrmMDIMain.PortManagerHash.Keys)
                {
                    if (!(str3 == clsGlobal.FilePlayBackPortName))
                    {
                        PortManager manager = (PortManager) clsGlobal.g_objfrmMDIMain.PortManagerHash[str3];
                        if (manager != null)
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
            base.Close();
        }

        private void checkBox_useDefaultValues_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_useDefaultValues.Checked)
            {
                this.textBox_SpdSclFac.Text = "0";
                this.textBox_GyroBias.Text = "0";
                this.textBox_GyroSclFac.Text = "0";
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

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmSetDRSensParam));
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.textBox_SpdSclFac = new TextBox();
            this.textBox_GyroBias = new TextBox();
            this.textBox_GyroSclFac = new TextBox();
            this.checkBox_useDefaultValues = new CheckBox();
            this.btn_Send = new Button();
            this.btn_Cancel = new Button();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x2b, 0x17);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Baseline speed scale factor";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x2d, 0x45);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x5c, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Baseline gyro bias";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x2d, 0x71);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x80, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Baseline gyro scale factor";
            this.textBox_SpdSclFac.Location = new Point(0xce, 0x19);
            this.textBox_SpdSclFac.Name = "textBox_SpdSclFac";
            this.textBox_SpdSclFac.Size = new Size(0x45, 20);
            this.textBox_SpdSclFac.TabIndex = 3;
            this.textBox_SpdSclFac.Text = "0";
            this.textBox_SpdSclFac.TextChanged += new EventHandler(this.textBox_SpdSclFac_TextChanged);
            this.textBox_GyroBias.Location = new Point(0xce, 0x45);
            this.textBox_GyroBias.Name = "textBox_GyroBias";
            this.textBox_GyroBias.Size = new Size(0x45, 20);
            this.textBox_GyroBias.TabIndex = 3;
            this.textBox_GyroBias.Text = "0";
            this.textBox_GyroBias.TextChanged += new EventHandler(this.textBox_GyroBias_TextChanged);
            this.textBox_GyroSclFac.Location = new Point(0xce, 0x71);
            this.textBox_GyroSclFac.Name = "textBox_GyroSclFac";
            this.textBox_GyroSclFac.Size = new Size(0x45, 20);
            this.textBox_GyroSclFac.TabIndex = 3;
            this.textBox_GyroSclFac.Text = "0";
            this.textBox_GyroSclFac.TextChanged += new EventHandler(this.textBox_GyroSclFac_TextChanged);
            this.checkBox_useDefaultValues.AutoSize = true;
            this.checkBox_useDefaultValues.Location = new Point(50, 0x9c);
            this.checkBox_useDefaultValues.Name = "checkBox_useDefaultValues";
            this.checkBox_useDefaultValues.Size = new Size(0x72, 0x11);
            this.checkBox_useDefaultValues.TabIndex = 4;
            this.checkBox_useDefaultValues.Text = "Use default values";
            this.checkBox_useDefaultValues.UseVisualStyleBackColor = true;
            this.checkBox_useDefaultValues.CheckedChanged += new EventHandler(this.checkBox_useDefaultValues_CheckedChanged);
            this.btn_Send.Location = new Point(0x13c, 0x17);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new Size(0x44, 0x16);
            this.btn_Send.TabIndex = 5;
            this.btn_Send.Text = "Send";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new EventHandler(this.btn_Send_Click);
            this.btn_Cancel.Location = new Point(0x13c, 0x40);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new Size(0x44, 0x16);
            this.btn_Cancel.TabIndex = 5;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new EventHandler(this.btn_Cancel_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new Size(0x18c, 0xc6);
            base.Controls.Add(this.btn_Cancel);
            base.Controls.Add(this.btn_Send);
            base.Controls.Add(this.checkBox_useDefaultValues);
            base.Controls.Add(this.textBox_GyroSclFac);
            base.Controls.Add(this.textBox_GyroBias);
            base.Controls.Add(this.textBox_SpdSclFac);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmSetDRSensParam";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Set DR Sensor Parameters";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void textBox_GyroBias_TextChanged(object sender, EventArgs e)
        {
            this.checkBox_useDefaultValues.Checked = false;
        }

        private void textBox_GyroSclFac_TextChanged(object sender, EventArgs e)
        {
            this.checkBox_useDefaultValues.Checked = false;
        }

        private void textBox_SpdSclFac_TextChanged(object sender, EventArgs e)
        {
            this.checkBox_useDefaultValues.Checked = false;
        }
    }
}

