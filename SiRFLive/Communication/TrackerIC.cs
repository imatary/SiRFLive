﻿namespace SiRFLive.Communication
{
    using System;

    public class TrackerIC
    {
        private ushort _i2CMasterAddress = 0x62;
        private ushort _i2CMaxMsgLength = 500;
        private byte _i2CMode = 1;
        private byte _i2CRate = 1;
        private ushort _i2CSlaveAddress = 0x60;
        private byte _iOPinConfigEnable = 1;
        private ushort[] _iOPinConfigs = new ushort[] { 0x3fc, 0x3fc, 4, 0, 0, 0x7c, 0, 0, 0, 0, 0 };
        private byte _lDOModeEnabled = 1;
        private byte _lNASelect;
        private byte _pwrCtrlOnOff;
        private double _refClkOffset;
        private uint _refClkUncertainty = 0xbb8;
        private uint _refFreq = 0xf9c568;
        private ushort _startupDelay = 0x3ff;
        private uint _uARTBaud = 0x1c200;
        private byte _uARTFlowControlEnable;
        private byte _uARTPreambleMax;
        private byte _uARTWakeupDelay;
        public byte Access;
        public uint Address;
        public string Data = "00000000";
        public string DataFileName = string.Empty;
        public byte ioPin;
        public uint IOPinConfig;
        public byte ioType;
        public byte ioVersion;
        public ushort NumBytes;
        public byte PeekPoke;
        public byte UARTWakeupCount;
        public byte UARTWakeupPattern;
        public double Version = 2.0;

        public ushort I2CMasterAddress
        {
            get
            {
                return this._i2CMasterAddress;
            }
            set
            {
                this._i2CMasterAddress = value;
            }
        }

        public ushort I2CMaxMsgLength
        {
            get
            {
                return this._i2CMaxMsgLength;
            }
            set
            {
                this._i2CMaxMsgLength = value;
            }
        }

        public byte I2CMode
        {
            get
            {
                return this._i2CMode;
            }
            set
            {
                this._i2CMode = value;
            }
        }

        public byte I2CRate
        {
            get
            {
                return this._i2CRate;
            }
            set
            {
                this._i2CRate = value;
            }
        }

        public ushort I2CSlaveAddress
        {
            get
            {
                return this._i2CSlaveAddress;
            }
            set
            {
                this._i2CSlaveAddress = value;
            }
        }

        public byte IOPinConfigEnable
        {
            get
            {
                return this._iOPinConfigEnable;
            }
            set
            {
                this._iOPinConfigEnable = value;
            }
        }

        public ushort[] IOPinConfigs
        {
            get
            {
                return this._iOPinConfigs;
            }
            set
            {
                this._iOPinConfigs = value;
            }
        }

        public byte LDOModeEnabled
        {
            get
            {
                return this._lDOModeEnabled;
            }
            set
            {
                this._lDOModeEnabled = value;
            }
        }

        public byte LNASelect
        {
            get
            {
                return this._lNASelect;
            }
            set
            {
                this._lNASelect = value;
            }
        }

        public byte PwrCtrlOnOff
        {
            get
            {
                return this._pwrCtrlOnOff;
            }
            set
            {
                this._pwrCtrlOnOff = value;
            }
        }

        public double RefClkOffset
        {
            get
            {
                return this._refClkOffset;
            }
            set
            {
                this._refClkOffset = value;
            }
        }

        public uint RefClkUncertainty
        {
            get
            {
                return this._refClkUncertainty;
            }
            set
            {
                this._refClkUncertainty = value;
            }
        }

        public uint RefFreq
        {
            get
            {
                return this._refFreq;
            }
            set
            {
                this._refFreq = value;
            }
        }

        public ushort StartupDelay
        {
            get
            {
                return this._startupDelay;
            }
            set
            {
                this._startupDelay = value;
            }
        }

        public uint UARTBaud
        {
            get
            {
                return this._uARTBaud;
            }
            set
            {
                this._uARTBaud = value;
            }
        }

        public byte UARTFlowControlEnable
        {
            get
            {
                return this._uARTFlowControlEnable;
            }
            set
            {
                this._uARTFlowControlEnable = value;
            }
        }

        public byte UARTPreambleMax
        {
            get
            {
                return this._uARTPreambleMax;
            }
            set
            {
                this._uARTPreambleMax = value;
            }
        }

        public byte UARTWakeupDelay
        {
            get
            {
                return this._uARTWakeupDelay;
            }
            set
            {
                this._uARTWakeupDelay = value;
            }
        }
    }
}

