﻿namespace SiRFLive.DeviceControl
{
    using NationalInstruments.NI4882;
    using System;
    using System.Windows.Forms;

    public class GPIB_Mgr
    {
        private int _BoardID;
        private byte _PrimaryAddress;
        private byte _SecondaryAddress;
        private Device device;

        internal GPIB_Mgr(int BoardID, byte PrimaryAddress, byte SecondaryAddress)
        {
            this._BoardID = BoardID;
            this._PrimaryAddress = PrimaryAddress;
            this._SecondaryAddress = SecondaryAddress;
            this.GPIBOpen();
        }

        internal void GPIBClose()
        {
            try
            {
                this.device.Clear();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        internal void GPIBOpen()
        {
            try
            {
                this.device = new Device(this._BoardID, this._PrimaryAddress, this._SecondaryAddress);
            }
            catch (Exception exception)
            {
                this.device = null;
                MessageBox.Show(exception.Message);
            }
        }

        internal string GPIBRead()
        {
            string str = "";
            try
            {
                str = this.device.ReadString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            return str;
        }

        internal void GPIBWrite(string str)
        {
            try
            {
                this.device.Write(str);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}

