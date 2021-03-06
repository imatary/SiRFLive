﻿namespace SiRFLive.Analysis
{
    using System;
    using System.Runtime.InteropServices;

    public class NavLibInterface
    {
        public CAI3Results Ai3ResultValues = new CAI3Results();
        public APR AprValues = new APR();
        public string EphClkMsg = "A0 A2 01 E3 D3 02 08 01 02 00 02 00 00 00 00 51 DE 5D 71 00 00 04 DD 70 00 00 00 A1 0C E6 40 80 52 00 00 76 F9 70 18 00 00 26 FC 46 66 00 00 47 50 EF 00 FF FF AC 00 00 00 80 52 00 00 00 40 FF FE A0 69 01 06 00 02 00 00 00 00 D7 74 18 DD 00 00 03 31 00 00 00 00 A1 0C 84 00 80 52 00 00 4B 31 21 7A 00 00 26 12 26 66 00 00 B0 AD 5B 00 FF FF A8 20 00 00 80 52 00 00 03 00 FF FB 14 EC 01 0A 00 02 00 00 00 00 5E ED 68 14 00 00 03 42 00 00 00 00 A1 0C 53 00 80 52 00 00 A2 38 B1 77 00 00 27 E1 46 66 00 00 0B E6 4A 00 FF FF A9 40 00 00 80 52 00 00 00 00 00 01 E0 00 01 11 00 02 00 00 00 00 F4 12 9D C7 00 00 08 F8 F0 00 00 00 A1 0D 6D A0 80 52 00 00 7B B4 C7 C1 00 00 27 56 F6 66 00 00 92 18 9E 00 FF FF AC 80 00 00 80 52 00 00 00 A0 FF FD 11 07 01 12 00 02 00 00 00 00 9A 1D 5C 06 00 00 02 D5 A0 00 00 00 A1 0D 59 E0 80 52 00 00 A3 67 97 D1 00 00 27 38 66 66 00 00 8D C4 E8 00 FF FF A7 A0 00 00 80 52 00 00 FF E0 FF FC 17 CB 01 15 00 02 00 00 00 00 D3 A3 9D A2 00 00 04 C8 60 00 00 00 A1 0C D7 00 80 52 00 00 78 BD 2C 1C 00 00 26 B6 66 66 00 00 7E CB FA 00 FF FF AA E0 00 00 80 52 00 00 00 00 00 02 E0 00 01 1A 00 02 00 00 00 00 15 A9 79 84 00 00 08 1E 10 00 00 00 A1 0D A3 40 80 52 00 00 CC DE 8E C8 00 00 28 1E 96 66 00 00 1A 8F 67 00 FF FF AA A0 00 00 80 52 00 00 00 40 00 0A 68 69 01 1D 00 02 00 00 00 00 6A C2 43 6A 00 00 04 7F B0 00 00 00 A1 0C 68 60 80 52 00 00 CB 84 03 C9 00 00 27 FD F6 66 00 00 CE 32 51 00 FF FF AA 40 00 00 80 52 00 00 FF 60 00 05 BE F9 08 A9 B0 B3";
        public MeasConfig MeasConfigValues = new MeasConfig();
        public string MeasRespMsg = "A0 A2 00 53 45 02 01 01 00 05 1A 1E EE A3 E0 01 07 06 21 F2 75 03 F3 02 00 00 27 1A 21 D4 AB 01 E3 00 F8 00 27 11 21 0F 95 01 3F 02 A5 00 27 0A 21 2F 22 01 4E 03 5B 00 27 02 21 2F 50 02 93 03 F2 00 27 15 21 CA D2 03 85 00 DD 00 27 12 21 CD 28 01 48 01 80 00 27 14 67 B0 B3";
        public CTTBEph TTBEphValues = new CTTBEph();
        private static double TWO_POW_MIN10 = 0.0009765625;

        public bool GetPositionValues()
        {
            int navLib = 0;
            int unhealthySV = 0;
            CNavLibOutputFix navLibOutputFix = null;
            string[] measRespCsv = msgDecoded.DecodeMsg(this.MeasRespMsg);
            string[] setEphClkCsv = msgInputDecoded.DecodeInputMsg(this.EphClkMsg);
            if ((measRespCsv.Length == 0) || (this.EphClkMsg.Length == 0))
            {
                return false;
            }
            int index = 2;
            this.TTBEphValues.nLastSV = Convert.ToByte(setEphClkCsv[index++]);
            int num4 = index;
            index = num4;
            try
            {
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph1, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph2, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph3, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph4, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph5, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph6, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph7, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph8, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph9, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph10, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph11, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph12, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph13, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph14, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph15, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph16, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph17, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph18, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph19, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph20, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph21, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph22, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph23, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph24, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph25, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph26, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph27, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph28, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph29, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph30, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph31, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph32, setEphClkCsv, ref index);
                this.SetEphValuesHelper(ref this.TTBEphValues.Eph33, setEphClkCsv, ref index);
            }
            catch
            {
            }
            index = 3;
            this.Ai3ResultValues.gpsMeasFlag = Convert.ToByte(measRespCsv[index++]);
            this.Ai3ResultValues.measErrStat = Convert.ToByte(measRespCsv[index++]);
            this.Ai3ResultValues.measWeek = Convert.ToUInt16(measRespCsv[index++]);
            this.Ai3ResultValues.measTow = Convert.ToUInt32(measRespCsv[index++]);
            this.Ai3ResultValues.measTimeAcc = Convert.ToByte(measRespCsv[index++]);
            this.Ai3ResultValues.measNumSv = Convert.ToByte(measRespCsv[index++]);
            num4 = index;
            index = num4;
            try
            {
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info1, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info2, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info3, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info4, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info5, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info6, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info7, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info8, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info9, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info10, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info11, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info12, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info13, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info14, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info15, measRespCsv, ref index);
                this.SetMeasSVInfoHelper(ref this.Ai3ResultValues.measSV_Info16, measRespCsv, ref index);
            }
            catch
            {
            }
            this.NLcompute(ref this.AprValues, ref this.TTBEphValues, ref this.Ai3ResultValues, ref this.MeasConfigValues, navLib, ref navLibOutputFix, unhealthySV);
            return true;
        }

        [DllImport("navLib.dll")]
        public static extern unsafe int navLibLSQ(APR* aprPos, CTTBEph* EphData, CAI3Results* AI3Res, MeasConfig* measConfig, int navLib, ref CNavLibOutputFix navLibOutputFix, int unhealthySV);
        public unsafe void NLcompute(ref APR aprPos, ref CTTBEph EphData, ref CAI3Results AI3Res, ref MeasConfig measConfig, int navLib, ref CNavLibOutputFix navLibOutputFix, int unhealthySV)
        {
            APR apr = aprPos;
            APR* aprPtr = &apr;
            CTTBEph eph = EphData;
            CTTBEph* ephData = &eph;
            CAI3Results results = AI3Res;
            CAI3Results* resultsPtr = &results;
            MeasConfig config = measConfig;
            MeasConfig* configPtr = &config;
            navLibLSQ(aprPtr, ephData, resultsPtr, configPtr, navLib, ref navLibOutputFix, unhealthySV);
            aprPos = apr;
            EphData = eph;
            AI3Res = results;
            measConfig = config;
        }

        public void SetAPRValues(double lat, double lon, int height, int horzErr)
        {
            this.AprValues.lat = lat;
            this.AprValues.lon = lon;
            this.AprValues.height = height;
            this.AprValues.horzErr = horzErr;
        }

        private void SetEphValuesHelper(ref SDataSetRIAEph eph, string[] setEphClkCsv, ref int index)
        {
            if (index < setEphClkCsv.Length)
            {
                try
                {
                    eph.EphFlag = Convert.ToByte(setEphClkCsv[index++]);
                    eph.SVPRN = Convert.ToByte(setEphClkCsv[index++]);
                    eph.URA_IND = Convert.ToByte(setEphClkCsv[index++]);
                    eph.IODE = Convert.ToByte(setEphClkCsv[index++]);
                    eph.C_RS = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.DeltaN = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.M0 = Convert.ToInt32(setEphClkCsv[index++]);
                    eph.C_UC = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.Eccentricity = Convert.ToUInt32(setEphClkCsv[index++]);
                    eph.C_US = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.A_SQRT = Convert.ToUInt32(setEphClkCsv[index++]);
                    eph.TOE = Convert.ToUInt16(setEphClkCsv[index++]);
                    eph.C_IC = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.Omega0 = Convert.ToInt32(setEphClkCsv[index++]);
                    eph.C_IS = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.AngleInclination = Convert.ToInt32(setEphClkCsv[index++]);
                    eph.C_RC = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.Omega = Convert.ToInt32(setEphClkCsv[index++]);
                    eph.OmegaDOT = Convert.ToInt32(setEphClkCsv[index++]);
                    eph.IDOT = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.TOC = Convert.ToUInt16(setEphClkCsv[index++]);
                    eph.T_GD = Convert.ToSByte(setEphClkCsv[index++]);
                    eph.AF2 = Convert.ToSByte(setEphClkCsv[index++]);
                    eph.AF1 = Convert.ToInt16(setEphClkCsv[index++]);
                    eph.AF0 = Convert.ToInt32(setEphClkCsv[index++]);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        private void SetMeasSVInfoHelper(ref measSVInfostruct measSV_Info, string[] measRespCsv, ref int index)
        {
            if (index < measRespCsv.Length)
            {
                try
                {
                    measSV_Info.svId = Convert.ToByte(measRespCsv[index++]);
                    measSV_Info.cNo = Convert.ToByte(measRespCsv[index++]);
                    measSV_Info.svDoppler = ((double) Convert.ToUInt16(measRespCsv[index++])) / 5.0;
                    measSV_Info.svCodePhaseWH = Convert.ToUInt16(measRespCsv[index++]);
                    measSV_Info.svCodePhaseFR = TWO_POW_MIN10 * Convert.ToUInt16(measRespCsv[index++]);
                    measSV_Info.multipathIndicator = Convert.ToByte(measRespCsv[index++]);
                    measSV_Info.psudorangeRMSErr = Convert.ToByte(measRespCsv[index++]);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        public void SetUncertValues(int MeterUnc, int MsUnc)
        {
            this.MeasConfigValues.measPosUncertInMeter = MeterUnc;
            this.MeasConfigValues.measTimeUncertInMs = MsUnc;
        }
    }
}

