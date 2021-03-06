﻿namespace Instruments
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class AnalogMeter : Control
    {
        private Bitmap bgImage;
        private Color frameColor = Color.Black;
        private Padding framePadding = new Padding(5);
        private Padding internalPadding = new Padding(5);
        private float maxValue = 15f;
        private float minValue;
        private Color pointerColor = Color.Red;
        private float r1x;
        private float r1y;
        private Graphics realGraphics;
        private bool stretch;
        private float tickLargeFrequency = 5f;
        private float tickLargeSize = 20f;
        private float tickLargeWidth = 2f;
        private float tickSmallFrequency = 1f;
        private float tickSmallSize = 15f;
        private float tickSmallWidth = 1f;
        private float tickStartAngle = 0.3490658f;
        private float tickTinyFrequency = 0.2f;
        private float tickTinySize = 5f;
        private float tickTinyWidth = 1f;
        private float value;

        public AnalogMeter()
        {
            this.BackColor = Color.White;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
        }

        private void CreateBackground()
        {
            if ((base.Width >= 1) && (base.Height >= 1))
            {
                float num;
                Rectangle rectangle;
                float num2;
                float num3;
                Bitmap image = new Bitmap(base.Width, base.Height);
                Graphics graphics = Graphics.FromImage(image);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                using (Brush brush = new SolidBrush(this.frameColor))
                {
                    graphics.FillRectangle(brush, base.ClientRectangle);
                }
                if (this.stretch)
                {
                    rectangle = new Rectangle(this.framePadding.Left, this.framePadding.Top, base.Width - this.framePadding.Horizontal, base.Height - this.framePadding.Vertical);
                }
                else
                {
                    rectangle = new Rectangle(this.framePadding.Left, this.framePadding.Top, base.Width - this.framePadding.Horizontal, (this.internalPadding.Vertical + (base.Width / 2)) - this.framePadding.Top);
                }
                graphics.IntersectClip(rectangle);
                using (new SolidBrush(this.BackColor))
                {
                    LinearGradientBrush brush3 = new LinearGradientBrush(new Point(0, base.Height), new Point(base.Width, 0), Color.LightBlue, Color.DodgerBlue);
                    graphics.FillRectangle(brush3, rectangle);
                }
                this.r1x = ((float) (rectangle.Width - this.internalPadding.Horizontal)) / 2f;
                this.r1y = rectangle.Height - this.internalPadding.Vertical;
                if (this.tickTinyFrequency > 0f)
                {
                    using (Pen pen = new Pen(this.ForeColor, this.tickTinyWidth))
                    {
                        num2 = this.r1x - this.tickTinySize;
                        num3 = this.r1y - this.tickTinySize;
                        for (num = this.minValue; num <= this.maxValue; num += this.tickTinyFrequency)
                        {
                            if (((this.tickSmallFrequency <= 0f) || (((num - this.minValue) % this.tickSmallFrequency) != 0f)) && ((this.tickLargeFrequency <= 0f) || (((num - this.minValue) % this.tickLargeFrequency) != 0f)))
                            {
                                PointF[] tfArray = this.GetLine(num, this.r1x, this.r1y, num2, num3);
                                graphics.DrawLine(pen, tfArray[0], tfArray[1]);
                            }
                        }
                    }
                }
                if (this.tickSmallFrequency > 0f)
                {
                    using (Pen pen2 = new Pen(this.ForeColor, this.tickSmallWidth))
                    {
                        num2 = this.r1x - this.tickSmallSize;
                        num3 = this.r1y - this.tickSmallSize;
                        for (num = this.minValue; num <= this.maxValue; num += this.tickSmallFrequency)
                        {
                            if ((this.tickLargeFrequency <= 0f) || (((num - this.minValue) % this.tickLargeFrequency) != 0f))
                            {
                                PointF[] tfArray2 = this.GetLine(num, this.r1x, this.r1y, num2, num3);
                                graphics.DrawLine(pen2, tfArray2[0], tfArray2[1]);
                            }
                        }
                    }
                }
                if (this.tickLargeFrequency > 0f)
                {
                    using (Pen pen3 = new Pen(this.ForeColor, this.tickLargeWidth))
                    {
                        num2 = this.r1x - this.tickLargeSize;
                        num3 = this.r1y - this.tickLargeSize;
                        float num4 = num2 - this.Font.Height;
                        float num5 = num3 - this.Font.Height;
                        for (num = this.minValue; num <= this.maxValue; num += this.tickLargeFrequency)
                        {
                            PointF[] tfArray3 = this.GetLine(num, this.r1x, this.r1y, num2, num3);
                            graphics.DrawLine(pen3, tfArray3[0], tfArray3[1]);
                            SizeF ef = graphics.MeasureString(num.ToString(), this.Font);
                            tfArray3 = this.GetLine(num, this.r1x, this.r1y, num4, num5);
                            graphics.DrawString(num.ToString(), this.Font, pen3.Brush, (float) (tfArray3[1].X - (ef.Width / 2f)), (float) (tfArray3[1].Y - (ef.Height / 2f)));
                        }
                    }
                }
                if (this.Text != "")
                {
                    using (Brush brush4 = new SolidBrush(this.ForeColor))
                    {
                        SizeF ef2 = graphics.MeasureString(this.Text, this.Font);
                        graphics.DrawString(this.Text, this.Font, brush4, (float) ((this.framePadding.Left + (rectangle.Width / 2)) - (ef2.Width / 2f)), (float) ((this.framePadding.Top + ((rectangle.Height * 3) / 4)) - (ef2.Height / 2f)));
                    }
                }
                graphics.Dispose();
                if (this.bgImage != null)
                {
                    this.bgImage.Dispose();
                }
                this.bgImage = image;
                if (this.realGraphics != null)
                {
                    this.DrawMeter(this.realGraphics);
                }
            }
        }

        private void DrawMeter(Graphics g)
        {
            g.DrawImageUnscaled(this.bgImage, 0, 0);
            using (Pen pen = new Pen(this.pointerColor, this.tickLargeWidth))
            {
                PointF[] tfArray = this.GetLine(this.value, this.r1x, this.r1y, 0f, 0f);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawLine(pen, tfArray[0], tfArray[1]);
            }
        }

        private PointF[] GetLine(float value, float r1x, float r1y, float r2x, float r2y)
        {
            PointF[] tfArray = new PointF[2];
            float num = ((((value > this.maxValue) ? this.maxValue : ((value < this.minValue) ? this.minValue : value)) - this.minValue) * ((3.141593f - (this.tickStartAngle * 2f)) / (this.maxValue - this.minValue))) + this.tickStartAngle;
            tfArray[0] = new PointF((float) ((this.framePadding.Left + this.internalPadding.Left) + (r1x - (r1x * Math.Cos((double) num)))), (float) ((this.framePadding.Top + this.internalPadding.Top) + (r1y - (r1y * Math.Sin((double) num)))));
            tfArray[1] = new PointF((float) ((this.framePadding.Left + this.internalPadding.Left) + (r1x - (r2x * Math.Cos((double) num)))), (float) ((this.framePadding.Top + this.internalPadding.Top) + (r1y - (r2y * Math.Sin((double) num)))));
            return tfArray;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.bgImage != null)
            {
                this.DrawMeter(e.Graphics);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.realGraphics != null)
            {
                this.realGraphics.Dispose();
            }
            this.realGraphics = base.CreateGraphics();
            this.CreateBackground();
        }

        [DefaultValue(typeof(Color), "White")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                this.CreateBackground();
            }
        }

        public override System.Drawing.Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                this.CreateBackground();
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                this.CreateBackground();
            }
        }

        [Category("Meter")]
        public Color FrameColor
        {
            get
            {
                return this.frameColor;
            }
            set
            {
                this.frameColor = value;
                this.CreateBackground();
            }
        }

        [Category("Meter")]
        public Padding FramePadding
        {
            get
            {
                return this.framePadding;
            }
            set
            {
                this.framePadding = value;
                this.CreateBackground();
            }
        }

        [Category("Meter")]
        public Padding InternalPadding
        {
            get
            {
                return this.internalPadding;
            }
            set
            {
                this.internalPadding = value;
                this.CreateBackground();
            }
        }

        [Description("Maximum value of the meter"), DefaultValue(15), Category("Meter")]
        public float MaxValue
        {
            get
            {
                return this.maxValue;
            }
            set
            {
                this.maxValue = value;
                this.CreateBackground();
            }
        }

        [DefaultValue(0), Description("Minimum value of the meter"), Category("Meter")]
        public float MinValue
        {
            get
            {
                return this.minValue;
            }
            set
            {
                this.minValue = value;
                this.CreateBackground();
            }
        }

        [Description("Color of the primary pointer"), DefaultValue(typeof(Color), "Red"), Category("Meter")]
        public Color PointerColor
        {
            get
            {
                return this.pointerColor;
            }
            set
            {
                this.pointerColor = value;
                this.CreateBackground();
            }
        }

        [Description("Set to true if the meter should fill the entire control. Set to false to maintain a rectangular outline."), Category("Meter"), DefaultValue(false)]
        public bool Stretch
        {
            get
            {
                return this.stretch;
            }
            set
            {
                this.stretch = value;
                this.CreateBackground();
            }
        }

        [Description("Title to display on the meter"), Category("Meter")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.CreateBackground();
            }
        }

        [Description("Frequency of large ticks (0 to disable)"), Category("Meter"), DefaultValue((float) 5f)]
        public float TickLargeFrequency
        {
            get
            {
                return this.tickLargeFrequency;
            }
            set
            {
                this.tickLargeFrequency = value;
                this.CreateBackground();
            }
        }

        [DefaultValue((float) 20f), Category("Meter")]
        public float TickLargeSize
        {
            get
            {
                return this.tickLargeSize;
            }
            set
            {
                this.tickLargeSize = value;
                this.CreateBackground();
            }
        }

        [Description("Stroke width of large ticks"), Category("Meter"), DefaultValue((float) 2f)]
        public float TickLargeWidth
        {
            get
            {
                return this.tickLargeWidth;
            }
            set
            {
                this.tickLargeWidth = value;
                this.CreateBackground();
            }
        }

        [Description("Frequency of small ticks (0 to disable)"), Category("Meter"), DefaultValue((float) 1f)]
        public float TickSmallFrequency
        {
            get
            {
                return this.tickSmallFrequency;
            }
            set
            {
                this.tickSmallFrequency = value;
                this.CreateBackground();
            }
        }

        [Category("Meter"), DefaultValue((float) 15f)]
        public float TickSmallSize
        {
            get
            {
                return this.tickSmallSize;
            }
            set
            {
                this.tickSmallSize = value;
                this.CreateBackground();
            }
        }

        [Description("Stroke width of small ticks"), DefaultValue((float) 1f), Category("Meter")]
        public float TickSmallWidth
        {
            get
            {
                return this.tickSmallWidth;
            }
            set
            {
                this.tickSmallWidth = value;
                this.CreateBackground();
            }
        }

        [Category("Meter"), Description("Angle the meter starts display at in degrees"), DefaultValue((float) 0.3490658f)]
        public float TickStartAngle
        {
            get
            {
                return (this.tickStartAngle * 57.29578f);
            }
            set
            {
                if ((value < 0f) || (value > 75f))
                {
                    throw new Exception("The angle must be between a value of 0 and 75 degrees");
                }
                this.tickStartAngle = value * 0.01745329f;
                this.CreateBackground();
            }
        }

        [Description("Frequency of tiny ticks (0 to disable)"), DefaultValue((float) 0.2f), Category("Meter")]
        public float TickTinyFrequency
        {
            get
            {
                return this.tickTinyFrequency;
            }
            set
            {
                this.tickTinyFrequency = value;
                this.CreateBackground();
            }
        }

        [Category("Meter"), DefaultValue((float) 5f)]
        public float TickTinySize
        {
            get
            {
                return this.tickTinySize;
            }
            set
            {
                this.tickTinySize = value;
                this.CreateBackground();
            }
        }

        [DefaultValue((float) 1f), Category("Meter"), Description("Stroke width of tiny ticks")]
        public float TickTinyWidth
        {
            get
            {
                return this.tickTinyWidth;
            }
            set
            {
                this.tickTinyWidth = value;
                this.CreateBackground();
            }
        }

        [DefaultValue(0), Description("Value of meter"), Category("Meter")]
        public float Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    this.DrawMeter(this.realGraphics);
                }
            }
        }
    }
}

