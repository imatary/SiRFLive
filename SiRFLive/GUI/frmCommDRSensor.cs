﻿namespace SiRFLive.GUI
{
    using SiRFLive.Communication;
    using SiRFLive.General;
    using SiRFLive.Utilities;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class frmCommDRSensor : Form
    {
        private static int _numberOpen;
        private string _persistedWindowName = "Input Command Window";
        private CommunicationManager comm;
        private IContainer components;
        private GroupBox groupBox1;
        private GroupBox groupBox7;
        private MyPanel myPanel1;
        private TextBox textBox_DRSensorDataV2;
        public int WinHeight;
        public int WinLeft;
        public int WinTop;
        public int WinWidth;

        public event updateParentEventHandler updateMainWindow;

        public event UpdateWindowEventHandler UpdatePortManager;

        public frmCommDRSensor()
        {
            this.InitializeComponent();
            _numberOpen++;
            this._persistedWindowName = "DR Sensor " + _numberOpen.ToString();
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

        private void frmCommDRSensor_Load(object sender, EventArgs e)
        {
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

        private void frmCommDRSensor_LocationChanged(object sender, EventArgs e)
        {
            this.WinTop = base.Top;
            this.WinLeft = base.Left;
            if (this.UpdatePortManager != null)
            {
                this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, true);
            }
        }

        private void frmCommDRSensor_Resize(object sender, EventArgs e)
        {
            this.Refresh();
            this.myPanel1.Refresh();
        }

        private void frmCommDRSensor_ResizeEnd(object sender, EventArgs e)
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
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmCommDRSensor));
            this.myPanel1 = new MyPanel();
            this.groupBox7 = new GroupBox();
            this.textBox_DRSensorDataV2 = new TextBox();
            this.groupBox1 = new GroupBox();
            TextBox box = new TextBox();
            this.myPanel1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            box.BackColor = SystemColors.ControlLight;
            box.BorderStyle = BorderStyle.None;
            box.Dock = DockStyle.Fill;
            box.Location = new Point(3, 0x10);
            box.Multiline = true;
            box.Name = "textBox_DRSensorDataV1";
            box.Size = new Size(10, 0x1f1);
            box.TabIndex = 11;
            box.Text = "sensor data v1";
            this.myPanel1.Controls.Add(this.groupBox7);
            this.myPanel1.Controls.Add(this.groupBox1);
            this.myPanel1.Dock = DockStyle.Fill;
            this.myPanel1.Location = new Point(0, 0);
            this.myPanel1.Name = "myPanel1";
            this.myPanel1.Size = new Size(580, 0x211);
            this.myPanel1.TabIndex = 5;
            this.myPanel1.Paint += new PaintEventHandler(this.myPanel1_Paint);
            this.groupBox7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox7.BackColor = SystemColors.ControlLight;
            this.groupBox7.Controls.Add(this.textBox_DRSensorDataV2);
            this.groupBox7.Location = new Point(12, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new Size(0x223, 0x204);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Car Bus Data";
            this.textBox_DRSensorDataV2.BackColor = SystemColors.ControlLight;
            this.textBox_DRSensorDataV2.BorderStyle = BorderStyle.None;
            this.textBox_DRSensorDataV2.Dock = DockStyle.Fill;
            this.textBox_DRSensorDataV2.Location = new Point(3, 0x10);
            this.textBox_DRSensorDataV2.Multiline = true;
            this.textBox_DRSensorDataV2.Name = "textBox_DRSensorDataV2";
            this.textBox_DRSensorDataV2.Size = new Size(0x21d, 0x1f1);
            this.textBox_DRSensorDataV2.TabIndex = 10;
            this.textBox_DRSensorDataV2.Text = "sensor data v2";
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.BackColor = SystemColors.ControlLight;
            this.groupBox1.Controls.Add(box);
            this.groupBox1.Location = new Point(0x235, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x10, 0x204);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SiRF DRive 1.x";
            this.groupBox1.Visible = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new Size(580, 0x211);
            base.Controls.Add(this.myPanel1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmCommDRSensor";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SiRFDRive Sensor View";
            base.Load += new EventHandler(this.frmCommDRSensor_Load);
            base.Resize += new EventHandler(this.frmCommDRSensor_Resize);
            base.LocationChanged += new EventHandler(this.frmCommDRSensor_LocationChanged);
            base.ResizeEnd += new EventHandler(this.frmCommDRSensor_ResizeEnd);
            this.myPanel1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            base.ResumeLayout(false);
        }

        private void myPanel1_Paint(object sender, PaintEventArgs e)
        {
            this.textBox_DRSensorDataV2.Text = this.comm.StringDRInputCarBusData;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.comm != null)
            {
                this.comm.EnableLocationMapView = false;
                if (this.updateMainWindow != null)
                {
                    this.updateMainWindow(base.Name);
                }
                if (this.UpdatePortManager != null)
                {
                    this.UpdatePortManager(base.Name, base.Left, base.Top, base.Width, base.Height, false);
                }
            }
            base.OnClosed(e);
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
                this.comm.DisplayPanelDRSensors = this.myPanel1;
                this.Text = this.comm.sourceDeviceName + ": DR Sensors";
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

