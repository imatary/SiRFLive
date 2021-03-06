﻿namespace SiRFLive.MessageHandling
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class PositionInfo
    {
        public ArrayList PositionList = new ArrayList();

        ~PositionInfo()
        {
            this.PositionList.Clear();
            this.PositionList = null;
        }

        public double GetMaximalLatitude()
        {
            if (this.PositionList.Count == 0)
            {
                return 0.0;
            }
            double latitude = 0.0;
            for (int i = 0; i < this.PositionList.Count; i++)
            {
                if (((PositionStruct) this.PositionList[i]).Latitude > latitude)
                {
                    latitude = ((PositionStruct) this.PositionList[i]).Latitude;
                }
            }
            return latitude;
        }

        public double GetMaximalLongitude()
        {
            if (this.PositionList.Count == 0)
            {
                return 0.0;
            }
            double longitude = 0.0;
            for (int i = 0; i < this.PositionList.Count; i++)
            {
                if (((PositionStruct) this.PositionList[i]).Longitude > longitude)
                {
                    longitude = ((PositionStruct) this.PositionList[i]).Longitude;
                }
            }
            return longitude;
        }

        public double GetMinimalLatitude()
        {
            if (this.PositionList.Count == 0)
            {
                return 0.0;
            }
            double latitude = 0.0;
            for (int i = 0; i < this.PositionList.Count; i++)
            {
                if (((PositionStruct) this.PositionList[i]).Latitude < latitude)
                {
                    latitude = ((PositionStruct) this.PositionList[i]).Latitude;
                }
            }
            return latitude;
        }

        public double GetMinimalLongitude()
        {
            if (this.PositionList.Count == 0)
            {
                return 0.0;
            }
            double longitude = 0.0;
            for (int i = 0; i < this.PositionList.Count; i++)
            {
                if (((PositionStruct) this.PositionList[i]).Longitude < longitude)
                {
                    longitude = ((PositionStruct) this.PositionList[i]).Longitude;
                }
            }
            return longitude;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionStruct
        {
            public double TOW;
            public int RxTime_Hour;
            public int RxTime_Minute;
            public ushort RxTime_second;
            public double Longitude;
            public double Latitude;
            public double Altitude;
            public uint SatellitesUsed;
            public double HDOP;
            public double Speed;
            public double Heading;
            public ushort NavValid;
            public ushort NavType;
            public ushort ExtWeek;
            public string NavModeString;
            public double MaxCN0;
            public string SW_Version;
            public ushort NumSVInFix;
        }
    }
}

