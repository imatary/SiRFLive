﻿namespace SiRFLive.Analysis
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct CAI3ResultsSizeTest
    {
        public int navlibStat;
        public byte pos_results_flag;
        public byte pos_error_status;
        public unsafe fixed byte str_pos_error_status[100];
        public byte pos_acc_flag;
    }
}

