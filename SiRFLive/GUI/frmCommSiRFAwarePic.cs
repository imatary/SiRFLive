﻿namespace SiRFLive.GUI
{
    using SiRFLive.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmCommSiRFAwarePic : Form
    {
        private Button button_OK;
        private IContainer components;
        private Label label1;
        private Label label2;
        private PictureBox pictureBox1;

        public frmCommSiRFAwarePic()
        {
            this.InitializeComponent();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
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
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmCommSiRFAwarePic));
            this.label1 = new Label();
            this.button_OK = new Button();
            this.pictureBox1 = new PictureBox();
            this.label2 = new Label();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(0x31, 0xf9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x19b, 0x10);
            this.label1.TabIndex = 1;
            this.label1.Text = "For 4e EVK: Toggle the Pulse switch to return to normal power mode.";
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_OK.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.button_OK.Location = new Point(0xd9, 0x137);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new Size(0x48, 0x17);
            this.button_OK.TabIndex = 3;
            this.button_OK.Text = "&OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new EventHandler(this.button_OK_Click);
            this.pictureBox1.BackgroundImage = Resources.EVK4e_Arrows;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new Point(0x24, 0x18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x1b4, 0xc9);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label2.Location = new Point(0x7b, 0x112);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x11b, 0x10);
            this.label2.TabIndex = 4;
            this.label2.Text = "Press OK and then Exit the SiRFaware window.";
            base.AcceptButton = this.button_OK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new Size(0x1fd, 0x15f);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.button_OK);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.pictureBox1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmCommSiRFAwarePic";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Switching from SiRFaware to Full Power Mode on 4e";
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

