﻿namespace SiRFLive.Analysis
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct measSVInfostruct
    {
        public byte svId;
        public byte cNo;
        public double svDoppler;
        public ushort svCodePhaseWH;
        public double svCodePhaseFR;
        public byte multipathIndicator;
        public byte psudorangeRMSErr;
    }
}

