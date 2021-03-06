﻿namespace SiRFLive.Analysis
{
    using SiRFLive.TruthData;
    using System;

    public class ComputeSVPos
    {
        public static int ACQ_ASSIST_UPDATE = 8;
        public static int ALMANAC_UPDATE = 2;
        public static int EPH_AGE_BIT_MASK = 0x1f;
        public static byte EPH_AGE_CGEE_INDICATOR = 0x20;
        public static int EPH_AGE_CGEE_MAX = 3;
        public static int EPH_AGE_SGEE_INDICATOR = 0x10;
        public static int EPH_AGE_SGEE_MAX = 7;
        public static int EPHEMRIS_UPDATE = 1;
        public static byte EPHSTATUS_ALM_AVAIL = 2;
        public static byte EPHSTATUS_EPH_AVAIL = 1;
        public static int EPHSTATUS_GOOD_CN0 = 4;
        public static int EPHSTATUS_NO_DATA = 0;
        public static double FREQL1 = 1575420000.0;
        public static float[] g_posVar = new float[] { 0f, 0.518f, 14.82f, 39.56f, 48.58f, 57.15f, 69.72f, 89.11f };
        public static float[] g_svClkVar = new float[] { 0f, 9.12f, 5.76f, 86.49f, 343.36f, 646.68f, 1013.78f, 1189.56f };
        public static double L1LGTH = 0.1902936727983649;
        public static int MAX_GPS_WEEK = 0x1462;
        public static double NLASQR = (NLMAJA * NLMAJA);
        public static double NLC = 299792458.0;
        public static double NLCC = (NLC * NLC);
        public static double NLEFOR = (NLESQR * NLESQR);
        public static double NLEQG = 9.78033267714;
        public static double NLERTE = 7.2921151467E-05;
        public static double NLESQR = (NLFLAT * (2.0 - NLFLAT));
        public static double NLFLAT = 0.0033528106647474805;
        public static double NLGM = 398600150000000;
        public static double NLJ2 = 0.00108263;
        public static double NLMAJA = 6378137.0;
        public static double NLMINB = (NLMAJA * (1.0 - NLFLAT));
        public static double NLMU = 398600500000000;
        public static double NLOMES = (1.0 - NLESQR);
        public static double NLRC = (1.0 / NLC);
        public static double NLRF = 4.442807633E-10;
        public static double NLSQMU = 19964981.843217388;
        public static double SEC_IN_WEEK_FLOAT = 604800.0;
        public static int SECONDS_IN_DAY = 0x15180;
        public static int SECONDS_IN_HOUR = 0xe10;
        public static int SECONDS_IN_MINUTE = 60;
        public static int SECONDS_IN_WEEK = 0x93a80;
        public static int SECONDS_PER_WEEK = 0x93a80;
        public static int SV_HEALTH_UPDATE = 4;
        public static int SV_STATE_UPDATE = 0x10;
        public static int SVD_ALM_HEALTH_REQ = 1;
        public static int SVD_DISABLE_SV_EPH = 1;
        public static int SVD_EPH_HEALTH_REQ = 2;
        public static int SVD_SAT_HEALTH_REQ = 0;
        public static int SVD_SV_HEALTHY = 0;
        public static int SVD_SV_UNHEALTHY = 4;
        public static int SVD_SV_UNKNOWN = 1;

        public void computeSVState(tSVD_SVState st, short svid, RinexEph eph, tGPSTime gTime)
        {
            double num50 = 0.0;
            double ntcld = 0.0;
            double num52 = 0.0;
            double num53 = 0.0;
            double num54 = 0.0;
            double num55 = 0.0;
            double num56 = 0.0;
            tSVD_EphemerisConsts constptr = new tSVD_EphemerisConsts();
            st.svid = (byte) svid;
            double num59 = ((SECONDS_IN_WEEK * (gTime.week - eph.weekNo)) + gTime.time) - eph.toe;
            if ((eph.status & EPHSTATUS_EPH_AVAIL) != 0)
            {
                if (eph.age == 0)
                {
                    st.dataAvail = 1;
                }
                else if ((eph.age & EPH_AGE_CGEE_INDICATOR) != 0)
                {
                    st.dataAvail = 6;
                }
                else
                {
                    st.dataAvail = 5;
                }
            }
            else if ((eph.status & EPHSTATUS_ALM_AVAIL) != 0)
            {
                if ((num59 < 302400.0) && (num59 > -302400.0))
                {
                    st.dataAvail = 4;
                }
                else
                {
                    st.dataAvail = 2;
                }
            }
            else
            {
                st.dataAvail = 0;
                return;
            }
            this.setEphemerisConstants(ref constptr, svid, ref eph, ref gTime);
            num59 = ((SECONDS_IN_WEEK * (gTime.week - eph.weekNo)) + gTime.time) - eph.toe;
            double a = eph.m0 + (constptr.ntcn * num59);
            double num14 = Math.Sin(a);
            double num25 = Math.Cos(a);
            double num15 = num14 * num14;
            double num16 = num15 * num14;
            double num17 = num16 * num14;
            double num18 = num17 * num14;
            double num19 = num18 * num14;
            double num20 = num19 * num14;
            double num23 = constptr.ntck * (((((((constptr.ntcs1 * num14) + ((constptr.ntcs2 * num14) * num25)) + (constptr.ntcs3 * num16)) + ((constptr.ntcs4 * num16) * num25)) + (constptr.ntcs5 * num18)) + ((constptr.ntcs6 * num18) * num25)) + (constptr.ntcs7 * num20));
            double num28 = (((((num25 + (constptr.ntcc2 * num15)) + ((constptr.ntcc3 * num15) * num25)) + (constptr.ntcc4 * num17)) + ((constptr.ntcc5 * num17) * num25)) + (constptr.ntcc6 * num19)) + ((constptr.ntcc7 * num19) * num25);
            double num21 = (num23 * constptr.ntccw) + (num28 * constptr.ntcsw);
            double num26 = (num28 * constptr.ntccw) - (num23 * constptr.ntcsw);
            double num12 = (2.0 * num21) * num26;
            double num24 = (num26 * num26) - (num21 * num21);
            double num45 = (eph.Cus * num12) + (eph.Cuc * num24);
            double num13 = num45;
            double num48 = 1.0 - ((num13 * num13) / 2.0);
            double num22 = (num21 * num48) + (num26 * num13);
            double num27 = (num26 * num48) - (num21 * num13);
            double num = 1.0 + (eph.ecc * num28);
            double num8 = constptr.ntcrx / num;
            double num10 = (eph.Crs * num12) + (eph.Crc * num24);
            double num37 = num8 + num10;
            double num39 = num37 * num27;
            num50 = num37 * num22;
            num52 = (eph.Cis * num12) + (eph.Cic * num24);
            double num29 = (eph.i0 + num52) + (eph.idot * num59);
            ntcld = constptr.ntcld;
            double d = (eph.omega0 + (ntcld * num59)) - constptr.ntcl2;
            num53 = Math.Cos(d);
            num55 = Math.Sin(d);
            num54 = Math.Sin(num29);
            num56 = Math.Cos(num29);
            st.pos[0] = (num39 * num53) - ((num50 * num56) * num55);
            st.pos[1] = (num39 * num55) + ((num50 * num56) * num53);
            st.pos[2] = num50 * num54;
            double num5 = ((constptr.ntcn * num) * num) / constptr.ntck2;
            double num44 = (eph.Cus * num24) - (eph.Cuc * num12);
            double num3 = 2.0 * num5;
            double num42 = num5 + (num3 * num44);
            double num11 = (eph.Crs * num24) - (eph.Crc * num12);
            double num57 = ((constptr.sqrta_2 * eph.ecc) * constptr.ntcn) / constptr.ntck;
            double num38 = (num57 * num23) + (num3 * num11);
            double num40 = (num38 * num27) - (num50 * num42);
            double num41 = (num38 * num22) + (num39 * num42);
            double num32 = (eph.Cis * num24) - (eph.Cic * num12);
            double num30 = (num3 * num32) + eph.idot;
            double num33 = num40 - ((num50 * ntcld) * num56);
            double num35 = ((num41 * num56) - (st.pos[2] * num30)) + (num39 * ntcld);
            st.vel[0] = (num33 * num53) - (num35 * num55);
            st.vel[1] = (num33 * num55) + (num35 * num53);
            st.vel[2] = (num41 * num54) + ((num50 * num30) * num56);
            double num2 = num3 * num3;
            double num6 = (-(((num3 * eph.ecc) * constptr.ntcn) * num23) / constptr.ntck2) * num;
            double num9 = (((num57 * num5) * num28) + ((2.0 * num6) * num11)) - (num2 * num10);
            double num43 = (num6 + ((2.0 * num6) * num44)) - (num2 * num45);
            double num46 = (((num9 * num27) - ((num38 * num42) * num22)) - (num41 * num42)) - (num43 * num50);
            double num47 = (((num9 * num22) + ((num38 * num42) * num27)) + (num40 * num42)) + (num43 * num39);
            double num31 = ((2.0 * num6) * num32) - (num2 * num52);
            double num34 = (num46 - ((num41 * ntcld) * num56)) + (((num50 * num30) * ntcld) * num54);
            double num36 = ((((num47 * num56) - ((num41 * num30) * num54)) - (st.vel[2] * num30)) - (st.pos[2] * num31)) + (num40 * ntcld);
            st.acc[0] = (float) (((-ntcld * st.vel[1]) + (num34 * num53)) - (num36 * num55));
            st.acc[1] = (float) (((ntcld * st.vel[0]) + (num34 * num55)) + (num36 * num53));
            st.acc[2] = (float) ((((num47 * num54) + (((2.0 * num41) * num30) * num56)) + ((num50 * num31) * num56)) - ((num30 * num30) * st.pos[2]));
            st.jrk[0] = (float) ((((-3.0 * NLERTE) * NLERTE) * ((float) st.vel[0])) + ((2.0 * NLERTE) * st.acc[1]));
            st.jrk[1] = (float) ((((-3.0 * NLERTE) * NLERTE) * st.vel[1]) - ((2.0 * NLERTE) * st.acc[0]));
            st.jrk[2] = (float) (((-4.0 * NLERTE) * NLERTE) * st.vel[2]);
            double num49 = constptr.ntecos / (1.0 + (eph.ecc * num28));
            st.tcr = (float) ((num49 * constptr.ntck) * num23);
            st.rcd = (float) ((num49 * constptr.ntcn) * (eph.ecc + num28));
            st.slw = 0;
            st.tow = (int) eph.toe;
            st.wno = (short) eph.weekNo;
            st.cct = (int) eph.toc;
            st.tgd = eph.tgd;
            st.iode = (byte) (((byte) eph.iode) & 0xff);
            st.af0 = eph.af0;
            st.af1 = eph.af1;
            st.af2 = eph.af2;
            st.gct = gTime.time;
            st.gcw = (short) gTime.week;
            if (st.dataAvail == 5)
            {
                st.posVar = g_posVar[eph.age];
                st.clkVar = g_svClkVar[eph.age];
                st.ephAge = eph.age;
            }
            else if (st.dataAvail == 6)
            {
                if (eph.age == 1)
                {
                    st.posVar = g_posVar[3];
                }
                else if (eph.age == 2)
                {
                    st.posVar = g_posVar[4];
                }
                else if (eph.age == 3)
                {
                    st.posVar = g_posVar[6];
                }
                else
                {
                    st.posVar = g_posVar[eph.age];
                }
                st.clkVar = g_svClkVar[eph.age];
                st.ephAge = eph.age;
            }
            else
            {
                st.posVar = 0f;
                st.clkVar = 0f;
                st.ephAge = 0;
            }
            double num58 = ((SECONDS_PER_WEEK * (st.gcw - st.wno)) + st.gct) - st.cct;
            st.clockBias = ((st.af0 + (num58 * (st.af1 + (num58 * st.af2)))) + st.tcr) - st.tgd;
            st.clockDrift = (st.af1 + ((2.0 * st.af2) * num58)) + st.rcd;
        }

        private void setEphemerisConstants(ref tSVD_EphemerisConsts constptr, short svid, ref RinexEph eph, ref tGPSTime gTime)
        {
            constptr.sqrta_2 = eph.sqrta * eph.sqrta;
            if (constptr.sqrta_2 != 0.0)
            {
                constptr.ntcn = (NLSQMU / (constptr.sqrta_2 * eph.sqrta)) + eph.deltan;
            }
            else
            {
                constptr.ntcn = eph.deltan;
            }
            double num = eph.ecc * eph.ecc;
            double num2 = num * eph.ecc;
            double num3 = num2 * eph.ecc;
            double num4 = num3 * eph.ecc;
            double num5 = num4 * eph.ecc;
            constptr.ntcs1 = ((1.0 + (3.0 * num)) + (5.0 * num3)) + (7.0 * num5);
            constptr.ntcs2 = ((2.0 * eph.ecc) + (4.0 * num2)) + (6.0 * num4);
            constptr.ntcs3 = -(((4.5 * num) + (28.3333333333333 * num3)) + (96.8333333333333 * num5));
            constptr.ntcs4 = -((10.6666666666667 * num2) + (52.0 * num4));
            constptr.ntcs5 = (26.0416666666667 * num3) + (247.975 * num5);
            constptr.ntcs6 = 64.8 * num4;
            constptr.ntcs7 = -163.401388888889 * num5;
            constptr.ntcc2 = -(((2.0 * eph.ecc) + (8.0 * num2)) + (18.0 * num4));
            constptr.ntcc3 = -(((4.5 * num) + (12.5 * num3)) + (24.5 * num5));
            constptr.ntcc4 = (10.6666666666667 * num2) + (78.0 * num4);
            constptr.ntcc5 = (26.0416666666667 * num3) + (151.083333333333 * num5);
            constptr.ntcc6 = -constptr.ntcs6;
            constptr.ntcc7 = constptr.ntcs7;
            constptr.ntc1e2 = 1.0 - num;
            constptr.ntcrx = constptr.sqrta_2 * constptr.ntc1e2;
            constptr.ntck = Math.Sqrt(constptr.ntc1e2);
            constptr.ntck2 = constptr.ntck * constptr.ntc1e2;
            constptr.ntcsw = Math.Sin(eph.omega);
            constptr.ntccw = Math.Cos(eph.omega);
            constptr.ntcl = eph.omega0 - (eph.omegaDot * eph.toe);
            constptr.ntcld = eph.omegaDot - NLERTE;
            constptr.ntcl2 = NLERTE * eph.toe;
            constptr.ntecos = (-NLRF * eph.ecc) * eph.sqrta;
        }

        private enum tSVD_SVStateEnum
        {
            SVSTATE_NO_DATA,
            SVSTATE_FROM_EPH,
            SVSTATE_FROM_ALM,
            SVSTATE_FROM_ACQ_ASSIST,
            SVSTATE_FROM_CURRENT_ALM,
            SVSTATE_FROM_EPH_EXTENSION_SERVER,
            SVSTATE_FROM_EPH_EXTENSION_CLIENT,
            MAX_SVSTATE_ENUM
        }
    }
}

