﻿namespace SiRFLive.GUI.General
{
    using SiRFLive.General;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmAbout : Form
    {
        private IContainer components;
        private TextBox textBox1;

        public frmAbout()
        {
            this.InitializeComponent();
            this.textBox1.Lines = new string[] { "SiRFLive (c) 2009-2010", "", "SiRF Technology Inc.", "A CSR plc Company", "", "A tool for real time GPS data collection and interaction.", "", "Features:", " L)ogging", " I)interactivity", " V)erification", " E)valuation", "", clsGlobal.SiRFLiveVersion, clsGlobal.SiRFLiveChangeNum, clsGlobal.SiRFLiveChangeDate };
            this.textBox1.ForeColor = Color.Black;
            this.textBox1.Select(0, 0);
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmAbout));
            this.textBox1 = new TextBox();
            base.SuspendLayout();
            this.textBox1.BackColor = SystemColors.Control;
            this.textBox1.BorderStyle = BorderStyle.None;
            this.textBox1.CausesValidation = false;
            this.textBox1.Cursor = Cursors.Arrow;
            this.textBox1.ForeColor = SystemColors.Desktop;
            this.textBox1.Location = new Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new Size(0xe3, 0xe2);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "(updated in frmAbout.cs code)";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new Size(0xf9, 0xf5);
            base.Controls.Add(this.textBox1);
            base.Icon = (Icon) resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "frmAbout";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "About SiRFLive";
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

