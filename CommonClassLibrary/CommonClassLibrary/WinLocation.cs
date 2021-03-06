﻿namespace CommonClassLibrary
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class WinLocation
    {
        private int _height;
        private int _left;
        private int _top;
        private int _width;
        public bool IsOpen;

        public int Height
        {
            get
            {
                return this._height;
            }
            set
            {
                this._height = value;
            }
        }

        public int Left
        {
            get
            {
                return this._left;
            }
            set
            {
                Rectangle virtualScreen = SystemInformation.VirtualScreen;
                if (value <= 0)
                {
                    if (value <= virtualScreen.X)
                    {
                        this._left = 0;
                    }
                }
                else if (value >= virtualScreen.X)
                {
                    this._left = 0;
                }
                this._left = value;
            }
        }

        public int Top
        {
            get
            {
                return this._top;
            }
            set
            {
                Rectangle virtualScreen = SystemInformation.VirtualScreen;
                if (value <= 0)
                {
                    if (value <= virtualScreen.Y)
                    {
                        this._top = 0;
                    }
                }
                else if (value >= virtualScreen.Y)
                {
                    this._top = 0;
                }
                this._top = value;
            }
        }

        public int Width
        {
            get
            {
                return this._width;
            }
            set
            {
                this._width = value;
            }
        }
    }
}

