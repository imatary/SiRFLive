﻿namespace SiRFLive.DeviceControl
{
    using System;

    public class SPAzMgr
    {
        private int _Address = 0x378;

        public SPAzMgr(int address)
        {
            this._Address = address;
        }

        public string ReadSPAzAtten()
        {
            byte num;
            try
            {
                byte num2 = (byte) PortAccessAPI.Input(this._Address);
                num = (byte) (num2 & 0x7f);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return num.ToString();
        }

        public string ReadSPAzPower()
        {
            byte num;
            try
            {
                byte num2 = (byte) PortAccessAPI.Input(this._Address);
                num = (byte) (num2 & 0x80);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            if (num == 0)
            {
                return "ON";
            }
            return "OFF";
        }

        public string WriteSPAzAtten(byte value)
        {
            try
            {
                byte num = (byte) PortAccessAPI.Input(this._Address);
                byte num2 = (byte) (num & 0x80);
                PortAccessAPI.Output(this._Address, (byte) (num2 + value));
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "";
        }

        public string WriteSPAzPower(bool state)
        {
            try
            {
                ushort num2;
                byte num = (byte) PortAccessAPI.Input(this._Address);
                if (state)
                {
                    num2 = (ushort) (num & 0x7f);
                }
                else
                {
                    num2 = (ushort) (num | 0x80);
                }
                PortAccessAPI.Output(this._Address, (byte) num2);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "";
        }
    }
}

