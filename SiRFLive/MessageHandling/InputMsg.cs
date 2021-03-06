﻿namespace SiRFLive.MessageHandling
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct InputMsg
    {
        public int messageID;
        public int subID;
        public string messageName;
        public int fieldNumber;
        public string fieldName;
        public int bytes;
        public string datatype;
        public string units;
        public double scale;
        public string defaultValue;
        public string savedValue;
    }
}

