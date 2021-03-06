﻿namespace SiRFLive.MessageHandling
{
    using SiRFLive.Communication;
    using SiRFLive.Utilities;
    using System;
    using System.Collections;
    using System.Text;

    public class AutoReplyMgr_F : AutoReplyMgr
    {
        private double _freqAidingAccuracy;
        private double _freqAidingEClkSkew;
        private long _freqAidingNorminalFreq;
        private int _freqAidingRefClk;
        private int _freqAidingType;
        private float _timeAidingAccuracy;
        private byte F_ID_APPROX_POS;
        private byte F_ID_FREQ_TRANS;
        private byte F_ID_HW_CONFIG;
        private byte F_ID_REJECT;
        private byte F_ID_TIME_TRANS;
        private byte F_MSG_TYPE;
        private string F_PROTOCOL_NAME;
        public double ref_altitude;
        public double ref_horizontalErr;
        public double ref_latitude;
        public double ref_longitude;
        public double ref_verticalErr;

        public AutoReplyMgr_F()
        {
            this.F_MSG_TYPE = 2;
            this.F_ID_HW_CONFIG = 0x10;
            this.F_ID_TIME_TRANS = 0x11;
            this.F_ID_FREQ_TRANS = 0x12;
            this.F_ID_APPROX_POS = 0x13;
            this.F_ID_REJECT = 0x15;
            this.F_PROTOCOL_NAME = "F";
            this._timeAidingAccuracy = 2000f;
            this._freqAidingType = 1;
            this._freqAidingRefClk = 1;
            this._freqAidingAccuracy = 0.5;
            this._freqAidingNorminalFreq = 0x124f800L;
        }

        public AutoReplyMgr_F(string xmlFile)
        {
            this.F_MSG_TYPE = 2;
            this.F_ID_HW_CONFIG = 0x10;
            this.F_ID_TIME_TRANS = 0x11;
            this.F_ID_FREQ_TRANS = 0x12;
            this.F_ID_APPROX_POS = 0x13;
            this.F_ID_REJECT = 0x15;
            this.F_PROTOCOL_NAME = "F";
            this._timeAidingAccuracy = 2000f;
            this._freqAidingType = 1;
            this._freqAidingRefClk = 1;
            this._freqAidingAccuracy = 0.5;
            this._freqAidingNorminalFreq = 0x124f800L;
            base.ProtocolFile = xmlFile;
            base._ai3ProtocolFile = base.ProtocolFile.Replace("Protocols_F.xml", "Protocols_AI3_Request.xml");
        }

        private string AI3_Request_ConvertFieldsToRaw(string csvMessage, ArrayList fieldList)
        {
            int num = 0;
            char[] separator = new char[] { ',' };
            string[] strArray = new string[0x7d0];
            strArray = csvMessage.Split(separator);
            int.Parse(strArray[0]);
            int.Parse(strArray[1]);
            if (fieldList.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < fieldList.Count; i++)
            {
                builder.Append(MsgFactory.ConvertDecimalToHex(strArray[i], ((SLCMsgStructure) fieldList[i]).datatype, ((SLCMsgStructure) fieldList[i]).scale));
            }
            string str = string.Empty;
            string msg = builder.ToString().Replace(" ", "");
            if (base.AutoReplyParams.AutoAid_Eph_fromTTB && (base.EphDataMsg != ""))
            {
                int num3 = msg.Length;
                string str3 = msg.Substring(0, 0x26);
                string str4 = msg.Substring(0xf26, num3 - 0xf26);
                int num4 = base.EphDataMsg.Length / 2;
                for (int k = 0; k < (0x780 - num4); k++)
                {
                    base.EphDataMsg = base.EphDataMsg + "00";
                }
                msg = str3 + base.EphDataMsg + str4;
            }
            if (base.AutoReplyParams.AutoAid_AcqData_fromTTB && (base.AcqAssistDataMsg != ""))
            {
                int num6 = msg.Length;
                string str5 = msg.Substring(0, 0x156c);
                string str6 = msg.Substring(0x1936, num6 - 0x1936);
                int num7 = base.AcqAssistDataMsg.Length / 2;
                for (int m = 0; m < (0x1e4 - num7); m++)
                {
                    base.AcqAssistDataMsg = base.AcqAssistDataMsg + "00";
                }
                msg = str5 + "01" + base.AcqAssistDataMsg + str6;
            }
            byte[] buffer = HelperFunctions.HexToByte(msg);
            buffer[0xcab] = 8;
            byte[] buffer2 = this.compressMsg(buffer, 0xcac);
            int length = buffer2.GetLength(0);
            byte num10 = (byte) (length / 0x3f8);
            int num11 = length % 0x3f8;
            if (0 < (length % 0x3f8))
            {
                num10 = (byte) (num10 + 1);
            }
            byte[] input = new byte[0x3fc];
            input[0] = 1;
            input[1] = 1;
            input[2] = num10;
            byte[] buffer4 = new byte[num11 + 4];
            buffer4[0] = 1;
            buffer4[1] = 1;
            buffer4[2] = num10;
            for (byte j = 0; j < num10; j = (byte) (j + 1))
            {
                if (j < (num10 - 1))
                {
                    input[3] = (byte) (j + 1);
                    for (int n = 0; n < 0x3f8; n++)
                    {
                        input[n + 4] = buffer2[(j * 0x3f8) + n];
                    }
                    num = 0x3fc;
                    string message = HelperFunctions.ByteToHex(input);
                    string str8 = "A0A2" + num.ToString("X").PadLeft(4, '0') + message + utils_AutoReply.GetChecksum(message, true) + "B0B3";
                    str = str + str8.Replace(" ", "") + "\r\n";
                }
                else
                {
                    buffer4[3] = (byte) (j + 1);
                    for (int num14 = 0; num14 < num11; num14++)
                    {
                        buffer4[num14 + 4] = buffer2[(j * 0x3f8) + num14];
                    }
                    num = num11 + 4;
                    string str9 = HelperFunctions.ByteToHex(buffer4);
                    string str10 = "A0A2" + num.ToString("X").PadLeft(4, '0') + str9 + utils_AutoReply.GetChecksum(str9, true) + "B0B3";
                    str = str + str10.Replace(" ", "") + "\r\n";
                }
            }
            return str;
        }

        public override void AutoReplyApproxPositionResp()
        {
            base.ApproxPosRespMsg = this.F_ApproxPos_DataToHex(base.ControlChannelVersion, base.ApproxPositionCtrl.Reject, base.ApproxPositionCtrl.Lat, base.ApproxPositionCtrl.Lon, base.ApproxPositionCtrl.Alt, base.ApproxPositionCtrl.DistanceSkew, base.ApproxPositionCtrl.HeadingSkew, base.ApproxPositionCtrl.EstHorrErr, base.ApproxPositionCtrl.EstVertiErr);
        }

        public override string AutoReplyFreqTransferResp()
        {
            base.FreqTransferRespMsg = this.F_FreqTransResp_DataToHex(base.ControlChannelVersion, base.FreqTransferCtrl.UseFreqAiding, base.FreqTransferCtrl.TimeTag, base.FreqTransferCtrl.RefClkInfo, base.FreqTransferCtrl.Accuracy, base.FreqTransferCtrl.ScaledFreqOffset, base.FreqTransferCtrl.ExtClkSkewppm, base.FreqTransferCtrl.NomFreq, base.FreqTransferCtrl.IncludeNormFreq);
            return base.FreqTransferRespMsg;
        }

        public override void AutoReplyHWCfgResp()
        {
            if (base.HWCfgCtrl.Reply)
            {
                base.HWCfgRespMsg = this.F_HW_ConfigResp_DataToHex(base.ControlChannelVersion, base.HWCfgCtrl.Reply, base.HWCfgCtrl.PreciseTimeEnabled, base.HWCfgCtrl.PreciseTimeDirection, base.HWCfgCtrl.FreqAidEnabled, base.HWCfgCtrl.FreqAidMethod, base.HWCfgCtrl.RTCAvailabe, base.HWCfgCtrl.RTCSource, base.HWCfgCtrl.CoarseTimeEnabled, base.HWCfgCtrl.RefClkEnabled, base.HWCfgCtrl.NorminalFreqHz);
                base.HwCfgRespMsgToTTB = base.HWCfgRespMsg;
            }
        }

        public override void AutoReplyTimeTransferResp()
        {
            base.TimeTransferRespMsg = this.F_TimeTransResp_DataToHex(base.ControlChannelVersion, base.TimeTransferCtrl.Reject, base.TimeTransferCtrl.TTType, base.TimeTransferCtrl.WeekNum, base.TimeTransferCtrl.TimeOfWeek, base.TimeTransferCtrl.Accuracy);
        }

        public override string AutoSendPositionRequestMsg()
        {
            int num;
            ArrayList fieldList = new ArrayList();
            fieldList = utils_AutoReply.GetMessageStructure(base._ai3ProtocolFile, CommunicationManager.ReceiverType.SLC, 1, 2, "AI3", "2.2");
            if (fieldList.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            if (base._aidingProtocolVersion == "2.1")
            {
                builder.Append("33");
            }
            else
            {
                builder.Append(((SLCMsgStructure) fieldList[0]).defaultValue);
            }
            for (num = 0; num < fieldList.Count; num++)
            {
                SLCMsgStructure structure = (SLCMsgStructure) fieldList[num];
                switch (structure.fieldName)
                {
                    case "NUM_FIXES":
                        structure.defaultValue = base.PositionRequestCtrl.NumFixes.ToString();
                        break;

                    case "TIME_BTW_FIXES":
                        structure.defaultValue = base.PositionRequestCtrl.TimeBtwFixes.ToString();
                        break;

                    case "HORI_ERROR_MAX":
                        structure.defaultValue = base.PositionRequestCtrl.HorrErrMax.ToString();
                        break;

                    case "VERT_ERROR_MAX":
                        structure.defaultValue = base.PositionRequestCtrl.VertErrMax.ToString();
                        break;

                    case "RESP_TIME_MAX":
                        structure.defaultValue = base.PositionRequestCtrl.RespTimeMax.ToString();
                        break;

                    case "TIME_ACC_PRIORITY":
                        structure.defaultValue = base.PositionRequestCtrl.TimeAccPriority.ToString();
                        break;

                    case "LOCATION_METHOD":
                        structure.defaultValue = base.PositionRequestCtrl.LocMethod.ToString();
                        break;
                }
                fieldList[num] = structure;
            }
            string[] strArray = new string[800];
            string ephDataMsg = base.EphDataMsg;
            for (int i = 0; i < 800; i++)
            {
                strArray[i] = "0";
            }
            if ((base.AutoReplyParams.AutoAid_Eph_fromFile || base.AutoReplyParams.AutoAid_ExtEph_fromFile) && (ephDataMsg != ""))
            {
                strArray = ephDataMsg.Split(new char[] { ',' });
            }
            num = 0;
            if (base._aidingProtocolVersion == "2.1")
            {
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "ICD_REV_NUM"))
                {
                    num++;
                }
                SLCMsgStructure structure2 = (SLCMsgStructure) fieldList[num];
                structure2.defaultValue = "33";
                fieldList[num] = structure2;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "HORI_ERROR_MAX"))
                {
                    num++;
                }
                SLCMsgStructure structure3 = (SLCMsgStructure) fieldList[num];
                switch (structure3.defaultValue)
                {
                    case "1":
                        structure3.defaultValue = "0";
                        break;

                    case "5":
                        structure3.defaultValue = "1";
                        break;

                    case "10":
                        structure3.defaultValue = "2";
                        break;

                    case "20":
                        structure3.defaultValue = "3";
                        break;

                    case "40":
                        structure3.defaultValue = "4";
                        break;

                    case "80":
                        structure3.defaultValue = "5";
                        break;

                    case "160":
                        structure3.defaultValue = "6";
                        break;

                    default:
                        structure3.defaultValue = "7";
                        break;
                }
                fieldList[num] = structure3;
            }
            while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "1st EPH_FLAG"))
            {
                num++;
            }
            int num3 = num;
            for (int j = 0; num3 < (num + strArray.Length); j++)
            {
                SLCMsgStructure structure4 = (SLCMsgStructure) fieldList[num3];
                structure4.defaultValue = strArray[j];
                fieldList[num3] = structure4;
                num3++;
            }
            int num5 = 0x161;
            string[] strArray2 = new string[num5];
            string[] strArray3 = new string[num5];
            string acqAssistDataMsg = base.AcqAssistDataMsg;
            int num6 = 10;
            for (int k = 0; k < num5; k++)
            {
                strArray2[k] = "0";
            }
            while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "ACQ_ASSIST_FLAG"))
            {
                num++;
            }
            if (acqAssistDataMsg != "")
            {
                SLCMsgStructure structure5 = (SLCMsgStructure) fieldList[num];
                structure5.defaultValue = "1";
                fieldList[num] = structure5;
            }
            else
            {
                SLCMsgStructure structure6 = (SLCMsgStructure) fieldList[num];
                structure6.defaultValue = "0";
                fieldList[num] = structure6;
            }
            if (base.AutoReplyParams.AutoAid_AcqData_fromFile)
            {
                strArray2 = acqAssistDataMsg.Split(new char[] { ',' });
                int num8 = (strArray2.Length - 1) / num6;
                int num9 = 0;
                for (int num10 = 1; num9 < (num8 * 11); num10 += 10)
                {
                    strArray3[num9++] = "1";
                    for (int num11 = 0; num11 < num6; num11++)
                    {
                        double num12 = Convert.ToDouble(strArray2[num11 + num10]);
                        switch (num11)
                        {
                            case 2:
                                if (num12 == 0.0)
                                {
                                    strArray2[num11 + num10] = "-1.015265";
                                }
                                break;

                            case 3:
                                if (num12 >= 200.0)
                                {
                                    strArray2[num11 + num10] = "0";
                                }
                                else if (num12 >= 100.0)
                                {
                                    strArray2[num11 + num10] = "1";
                                }
                                else if (num12 >= 50.0)
                                {
                                    strArray2[num11 + num10] = "2";
                                }
                                else if (num12 >= 25.0)
                                {
                                    strArray2[num11 + num10] = "3";
                                }
                                else if (num12 >= 12.5)
                                {
                                    strArray2[num11 + num10] = "4";
                                }
                                else if (num12 > 0.0)
                                {
                                    strArray2[num11 + num10] = "0";
                                }
                                else
                                {
                                    strArray2[num11 + num10] = "255";
                                }
                                break;

                            default:
                                if ((num11 == 4) && (num12 != 0.0))
                                {
                                    strArray2[num11 + num10] = ((int) (1023.0 - num12)).ToString();
                                }
                                break;
                        }
                        strArray3[num9++] = strArray2[num11 + num10];
                    }
                }
            }
            while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "REFERENCE_TIME"))
            {
                num++;
            }
            SLCMsgStructure structure7 = (SLCMsgStructure) fieldList[num];
            structure7.defaultValue = strArray2[0];
            fieldList[num] = structure7;
            while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "1st ACQ_ASSIST_VALID_FLAG"))
            {
                num++;
            }
            int num14 = num;
            for (int m = 0; num14 < (num + strArray3.Length); m++)
            {
                SLCMsgStructure structure8 = (SLCMsgStructure) fieldList[num14];
                structure8.defaultValue = strArray3[m];
                fieldList[num14] = structure8;
                num14++;
            }
            while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "NW_ENHANCE_TYPE"))
            {
                num++;
            }
            SLCMsgStructure structure9 = (SLCMsgStructure) fieldList[num];
            structure9.defaultValue = base.HWCfgCtrl.NetworkEnhanceType.ToString();
            fieldList[num] = structure9;
            for (int n = 1; n < fieldList.Count; n++)
            {
                if (string.IsNullOrEmpty(((SLCMsgStructure) fieldList[n]).defaultValue))
                {
                    builder.Append(",");
                    builder.Append("0");
                }
                else
                {
                    builder.Append(",");
                    builder.Append(((SLCMsgStructure) fieldList[n]).defaultValue);
                }
            }
            return this.AI3_Request_ConvertFieldsToRaw(builder.ToString(), fieldList);
        }

        private byte[] compressMsg(byte[] in_buf, int in_len)
        {
            byte[] buffer = new byte[0xe10];
            int index = 0;
            int num = 0;
            int num2 = 0;
            int num4 = 0;
            int num5 = 0;
            bool flag = false;
            bool flag2 = false;
            while (num5 < (in_len - 1))
            {
                if (in_buf[num5] == in_buf[num5 + 1])
                {
                    num++;
                    if (num < 4)
                    {
                        num2++;
                    }
                    else
                    {
                        flag2 = true;
                        if ((num2 >= num) && flag)
                        {
                            buffer[num4++] = (byte) (((num2 - num) + 1) >> 8);
                            buffer[num4++] = (byte) (((num2 - num) + 1) & 0xff);
                            int num6 = num4;
                            int num7 = index;
                            for (int j = 0; j < ((num2 - num) + 1); j++)
                            {
                                buffer[num6] = in_buf[num7];
                                num6++;
                                num7++;
                            }
                            index = ((index + num2) - num) + 1;
                            num4 = ((num4 + num2) - num) + 1;
                            flag = false;
                        }
                        num2 = 0;
                    }
                }
                else
                {
                    if (flag2)
                    {
                        num++;
                        flag2 = false;
                    }
                    else
                    {
                        num2++;
                    }
                    if (num >= 4)
                    {
                        if ((num2 >= num) && flag)
                        {
                            buffer[num4++] = (byte) (((num2 - num) + 1) >> 8);
                            buffer[num4++] = (byte) (((num2 - num) + 1) & 0xff);
                            int num9 = num4;
                            int num10 = index;
                            for (int k = 0; k < ((num2 - num) + 1); k++)
                            {
                                buffer[num9] = in_buf[num10];
                                num9++;
                                num10++;
                            }
                            index = ((index + num2) - num) + 1;
                            num4 = ((num4 + num2) - num) + 1;
                            flag = false;
                        }
                        num2 = 0;
                        buffer[num4++] = (byte) (0x80 | (num >> 8));
                        buffer[num4++] = (byte) (num & 0xff);
                        buffer[num4++] = in_buf[index];
                        index += num;
                    }
                    num = 0;
                    flag = true;
                }
                num5++;
            }
            if (num >= 4)
            {
                buffer[num4++] = (byte) (0x80 | ((num + 1) >> 8));
                buffer[num4++] = (byte) ((num + 1) & 0xff);
                buffer[num4++] = in_buf[index];
            }
            else
            {
                buffer[num4++] = (byte) ((num2 + 1) >> 8);
                buffer[num4++] = (byte) ((num2 + 1) & 0xff);
                int num12 = num4;
                int num13 = index;
                for (int m = 0; m < (num2 + 1); m++)
                {
                    buffer[num12] = in_buf[num13];
                    num12++;
                    num13++;
                }
                num4 = (num4 + num2) + 1;
            }
            byte[] buffer2 = new byte[num4];
            for (int i = 0; i < num4; i++)
            {
                buffer2[i] = buffer[i];
            }
            return buffer2;
        }

        public uint decmprss(byte[] input, uint input_length, byte[] output, uint max_out_len)
        {
            byte[] buffer = new byte[2];
            uint index = 0;
            uint num4 = 0;
            while ((index < input_length) && (num4 < max_out_len))
            {
                buffer[0] = input[index];
                buffer[1] = input[(int) ((IntPtr) (index + 1))];
                index += 2;
                ushort num = (ushort) (((buffer[0] & 0x7f) * 0x100) + buffer[1]);
                if ((buffer[0] & 0x80) == 0x80)
                {
                    for (ushort i = 0; i < num; i = (ushort) (i + 1))
                    {
                        output[num4 + i] = input[index];
                    }
                    num4 += num;
                    index++;
                }
                else
                {
                    for (ushort j = 0; j < num; j = (ushort) (j + 1))
                    {
                        output[num4 + j] = input[index + j];
                    }
                    num4 += num;
                    index += num;
                }
            }
            if ((index < input_length) && (num4 >= max_out_len))
            {
                return 0;
            }
            return num4;
        }

        public string F_ApproxPos_DataToHex(string F_ICD, bool reject, double inLat, double inLon, double inAlt, double latSkew, double lonSkew, double estHorErr, double estVertErr)
        {
            string str;
            ArrayList fieldList = new ArrayList();
            if (reject)
            {
                str = this.F_Reject(F_ICD, this.F_ID_APPROX_POS, 4);
            }
            else
            {
                Convert.ToDouble(F_ICD);
                fieldList = utils_AutoReply.GetMessageStructure(base.ProtocolFile, CommunicationManager.ReceiverType.SLC, this.F_ID_APPROX_POS, 0, this.F_PROTOCOL_NAME, F_ICD);
                int num9 = 0;
                while ((num9 < fieldList.Count) && (((SLCMsgStructure) fieldList[num9]).fieldName != "LAT"))
                {
                    num9++;
                }
                double num = inLat + latSkew;
                ulong num4 = (ulong) ((num * 4294967296) / 180.0);
                SLCMsgStructure structure = (SLCMsgStructure) fieldList[num9];
                structure.defaultValue = num4.ToString();
                fieldList[num9] = structure;
                num9 = 0;
                while ((num9 < fieldList.Count) && (((SLCMsgStructure) fieldList[num9]).fieldName != "LON"))
                {
                    num9++;
                }
                double num2 = inLon + lonSkew;
                ulong num5 = (ulong) (((num2 * 4294967296) / 360.0) + 4294967296);
                structure = (SLCMsgStructure) fieldList[num9];
                structure.defaultValue = num5.ToString();
                fieldList[num9] = structure;
                num9 = 0;
                while ((num9 < fieldList.Count) && (((SLCMsgStructure) fieldList[num9]).fieldName != "ALT"))
                {
                    num9++;
                }
                double num3 = inAlt;
                ushort num6 = (ushort) ((num3 + 500.0) / 0.1);
                structure = (SLCMsgStructure) fieldList[num9];
                structure.defaultValue = num6.ToString();
                fieldList[num9] = structure;
                num9 = 0;
                while ((num9 < fieldList.Count) && (((SLCMsgStructure) fieldList[num9]).fieldName != "EST_HOR_ER"))
                {
                    num9++;
                }
                byte num7 = utils_AutoReply.metersToICDHorzErr(estHorErr);
                structure = (SLCMsgStructure) fieldList[num9];
                structure.defaultValue = num7.ToString();
                fieldList[num9] = structure;
                num9 = 0;
                while ((num9 < fieldList.Count) && (((SLCMsgStructure) fieldList[num9]).fieldName != "EST_VER_ER"))
                {
                    num9++;
                }
                this.ref_verticalErr = Convert.ToDouble(((SLCMsgStructure) fieldList[num9]).defaultValue);
                ushort num8 = (ushort) (10.0 * estVertErr);
                structure = (SLCMsgStructure) fieldList[num9];
                structure.defaultValue = num8.ToString();
                fieldList[num9] = structure;
                num9 = 0;
                while ((num9 < fieldList.Count) && (((SLCMsgStructure) fieldList[num9]).fieldName != "USE_ALT_AIDING"))
                {
                    num9++;
                }
                structure = (SLCMsgStructure) fieldList[num9];
                structure.defaultValue = (num8 > 0) ? "1" : "0";
                fieldList[num9] = structure;
                str = utils_AutoReply.FieldList_to_HexString(true, fieldList, this.F_MSG_TYPE);
            }
            fieldList.Clear();
            return str;
        }

        public string F_ApproxPosResp(bool reject, string F_ICD, string testSite, double latSkew, double lonSkew)
        {
            int num;
            string str;
            double num2 = Convert.ToDouble(F_ICD);
            if (num2 > 2.0)
            {
                num = 0x10;
            }
            else
            {
                num = 15;
            }
            byte[] bytes = new byte[num];
            string inputMsgFile = @"..\Protocols\" + testSite + @"\Protocols_F.xml";
            ArrayList list = new ArrayList();
            list = utils_AutoReply.GetMessageStructure(inputMsgFile, CommunicationManager.ReceiverType.SLC, 0x13, 0, "F", "2.1");
            int num3 = 0;
            while ((num3 < list.Count) && (((SLCMsgStructure) list[num3]).fieldName != "LAT"))
            {
                num3++;
            }
            this.ref_latitude = Convert.ToDouble(((SLCMsgStructure) list[num3]).defaultValue);
            this.ref_longitude = Convert.ToDouble(((SLCMsgStructure) list[num3 + 1]).defaultValue);
            this.ref_altitude = Convert.ToDouble(((SLCMsgStructure) list[num3 + 2]).defaultValue);
            this.ref_horizontalErr = Convert.ToDouble(((SLCMsgStructure) list[num3 + 3]).defaultValue);
            this.ref_verticalErr = Convert.ToDouble(((SLCMsgStructure) list[num3 + 4]).defaultValue);
            if (reject)
            {
                str = utils_AutoReply.ByteArrayToHexString(new byte[] { 2, 0x15, 0x13, 4 });
                num = 4;
                return ("A0A2" + num.ToString("X").PadLeft(4, '0') + str + utils_AutoReply.GetChecksum(str, true) + "B0B3");
            }
            bytes[0] = 2;
            bytes[1] = 0x13;
            double num4 = this.ref_latitude + latSkew;
            double num5 = this.ref_longitude + lonSkew;
            double num6 = this.ref_altitude;
            byte num10 = utils_AutoReply.metersToICDHorzErr(this.ref_horizontalErr);
            ushort num11 = (ushort) (10.0 * this.ref_verticalErr);
            ulong num7 = (ulong) ((num4 * 4294967296) / 180.0);
            ulong num8 = (ulong) (((num5 * 4294967296) / 360.0) + 4294967296);
            ushort num9 = (ushort) ((num6 + 500.0) / 0.1);
            bytes[2] = (byte) ((num7 & 0xff000000L) >> 0x18);
            bytes[3] = (byte) ((num7 & 0xff0000L) >> 0x10);
            bytes[4] = (byte) ((num7 & 0xff00L) >> 8);
            bytes[5] = (byte) (num7 & ((ulong) 0xffL));
            bytes[6] = (byte) ((num8 & 0xff000000L) >> 0x18);
            bytes[7] = (byte) ((num8 & 0xff0000L) >> 0x10);
            bytes[8] = (byte) ((num8 & 0xff00L) >> 8);
            bytes[9] = (byte) (num8 & ((ulong) 0xffL));
            bytes[10] = (byte) ((num9 & 0xff00) >> 8);
            bytes[11] = (byte) (num9 & 0xff);
            bytes[12] = num10;
            bytes[13] = (byte) ((num11 & 0xff00) >> 8);
            bytes[14] = (byte) (num11 & 0xff);
            if (num2 > 2.0)
            {
                if (num11 > 0)
                {
                    bytes[15] = 1;
                }
                else
                {
                    bytes[15] = 0;
                }
            }
            str = utils_AutoReply.ByteArrayToHexString(bytes);
            return ("A0A2" + num.ToString("X").PadLeft(4, '0') + str + utils_AutoReply.GetChecksum(str, true) + "B0B3");
        }

        public string F_FreqTransferResp(short lastClkDrift)
        {
            string str;
            int num6;
            int num = this._freqAidingType;
            int num2 = this._freqAidingRefClk;
            double num3 = this._freqAidingAccuracy;
            double num4 = this._freqAidingEClkSkew;
            long num5 = this._freqAidingNorminalFreq;
            byte[] bytes = new byte[15];
            if (num == 2)
            {
                str = utils_AutoReply.ByteArrayToHexString(new byte[] { 2, 0x15, 0x12, 4 });
                num6 = 4;
                return ("A0A2" + num6.ToString("X").PadLeft(4, '0') + str + utils_AutoReply.GetChecksum(str, true) + "B0B3");
            }
            bytes[0] = 2;
            bytes[1] = 0x12;
            bytes[2] = (byte) ((lastClkDrift & 0xff00) >> 8);
            bytes[3] = (byte) (lastClkDrift & 0xff);
            if (num3 > 239.0)
            {
                bytes[4] = 0xff;
            }
            else if ((num3 > 231.0) && (num3 < 240.0))
            {
                bytes[4] = 0xfe;
            }
            else if (num3 < 0.00390625)
            {
                bytes[4] = 0;
            }
            else if ((num3 > 0.00390625) && (num3 < 0.004150390625))
            {
                bytes[4] = 1;
            }
            else
            {
                int num7 = 0;
                while (num7 < 15)
                {
                    if ((0.00390625 * (((int) 1) << num7)) >= num3)
                    {
                        break;
                    }
                    num7++;
                }
                if (num7 > 0)
                {
                    num7--;
                }
                int num8 = 0;
                while (num8 <= 15)
                {
                    if (((0.00390625 * (((int) 1) << num7)) * (1.0 + (((double) num8) / 16.0))) >= num3)
                    {
                        break;
                    }
                    num8++;
                }
                num8 = utils_AutoReply.min(num8, 15);
                if (((0.00390625 * (((int) 1) << num7)) * (1.0 + (((double) num8) / 16.0))) >= num3)
                {
                    bytes[4] = (byte) ((num7 << 4) | num8);
                }
                else
                {
                    if (num7 < 15)
                    {
                        num7++;
                    }
                    bytes[4] = (byte) (num7 << 4);
                }
            }
            bytes[5] = 0xff;
            bytes[6] = 0xff;
            bytes[7] = 0xff;
            bytes[8] = 0xfe;
            bytes[9] = (byte) num2;
            double num10 = num5 * (1.0 + (num4 * 1E-06));
            ulong num9 = (ulong) (num10 * 1000.0);
            bytes[10] = (byte) ((num9 & 0xff00000000L) >> 0x20);
            bytes[11] = (byte) ((num9 & 0xff000000L) >> 0x18);
            bytes[12] = (byte) ((num9 & 0xff0000L) >> 0x10);
            bytes[13] = (byte) ((num9 & 0xff00L) >> 8);
            bytes[14] = (byte) (num9 & ((ulong) 0xffL));
            str = utils_AutoReply.ByteArrayToHexString(bytes);
            num6 = 15;
            return ("A0A2" + num6.ToString("X").PadLeft(4, '0') + str + utils_AutoReply.GetChecksum(str, true) + "B0B3");
        }

        public string F_FreqTransResp_DataToHex(string F_ICD, int useFreqAiding, uint timeTag, int refClkInfo, double fAccuracy, short lastClkDrift, double fEclkScewppm, long nomFreq, bool includeNormFreq)
        {
            string str;
            ArrayList fieldList = new ArrayList();
            if (useFreqAiding == 2)
            {
                str = this.F_Reject(F_ICD, this.F_ID_FREQ_TRANS, 4);
            }
            else
            {
                fieldList = utils_AutoReply.GetMessageStructure(base.ProtocolFile, CommunicationManager.ReceiverType.SLC, this.F_ID_FREQ_TRANS, 0, this.F_PROTOCOL_NAME, F_ICD);
                int num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "SCALED_FREQ_OFFSET"))
                {
                    num++;
                }
                SLCMsgStructure structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = lastClkDrift.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "REL_FREQ_ACC"))
                {
                    num++;
                }
                byte num2 = utils_AutoReply.get_REL_FREQ_ACC(fAccuracy);
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = num2.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "TIME_TAG"))
                {
                    num++;
                }
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = timeTag.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "REF_CLOCK_INFO"))
                {
                    num++;
                }
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = ((byte) refClkInfo).ToString();
                fieldList[num] = structure;
                double num4 = nomFreq * (1.0 + (fEclkScewppm * 1E-06));
                ulong num3 = (ulong) (num4 * 1000.0);
                ulong num5 = num3 & ((ulong) 0xffffffffffL);
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "NOMINAL_FREQ"))
                {
                    num++;
                }
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = num5.ToString();
                fieldList[num] = structure;
                if (!includeNormFreq)
                {
                    fieldList.RemoveAt(fieldList.Count - 1);
                }
                str = utils_AutoReply.FieldList_to_HexString(true, fieldList, this.F_MSG_TYPE);
            }
            fieldList.Clear();
            return str;
        }

        public string F_HW_ConfigResp_DataToHex(string F_ICD, bool reply, byte precTimeEnabled, byte precTimeDir, byte freqAidEnabled, byte freqAidMethod, byte rtcAvailble, byte rtcSrc, byte coarseTimeEnabled, byte refClkEnabled, long normFreq)
        {
            string str;
            ArrayList fieldList = new ArrayList();
            if (!reply)
            {
                str = this.F_Reject(F_ICD, this.F_ID_HW_CONFIG, 4);
            }
            else
            {
                fieldList = utils_AutoReply.GetMessageStructure(base.ProtocolFile, CommunicationManager.ReceiverType.SLC, this.F_ID_HW_CONFIG, 0, this.F_PROTOCOL_NAME, F_ICD);
                int num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "HW_CONFIG"))
                {
                    num++;
                }
                byte num2 = (byte) (((((((precTimeEnabled | (precTimeDir << 1)) | (freqAidEnabled << 2)) | (freqAidMethod << 3)) | (rtcAvailble << 4)) | (rtcSrc << 5)) | (coarseTimeEnabled << 6)) | (refClkEnabled << 7));
                SLCMsgStructure structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = num2.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "NOMINAL_FREQ"))
                {
                    num++;
                }
                structure = (SLCMsgStructure) fieldList[num];
                long num3 = normFreq * 0x3e8L;
                structure.defaultValue = (num3 & 0xffffffffffL).ToString();
                fieldList[num] = structure;
                str = utils_AutoReply.FieldList_to_HexString(true, fieldList, this.F_MSG_TYPE);
            }
            fieldList.Clear();
            return str;
        }

        public string F_Reject(string F_ICD, byte msgID, byte reason)
        {
            ArrayList fieldList = new ArrayList();
            fieldList = utils_AutoReply.GetMessageStructure(base.ProtocolFile, CommunicationManager.ReceiverType.SLC, this.F_ID_REJECT, 0, this.F_PROTOCOL_NAME, F_ICD);
            int num = 0;
            while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "REJ_MESS_ID"))
            {
                num++;
            }
            SLCMsgStructure structure = (SLCMsgStructure) fieldList[num];
            structure.defaultValue = msgID.ToString();
            fieldList[num] = structure;
            num = 0;
            while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "REJ_REASON"))
            {
                num++;
            }
            structure = (SLCMsgStructure) fieldList[num];
            structure.defaultValue = reason.ToString();
            fieldList[num] = structure;
            string str = utils_AutoReply.FieldList_to_HexString(true, fieldList, this.F_MSG_TYPE);
            fieldList.Clear();
            return str;
        }

        public string F_TimeTransferResp(ushort wkNum, ulong TOW)
        {
            float num = this._timeAidingAccuracy;
            byte[] bytes = new byte[14];
            bytes[0] = 2;
            bytes[1] = 0x11;
            bytes[2] = 0;
            bytes[3] = (byte) (wkNum >> 8);
            bytes[4] = (byte) (wkNum & 0xff);
            ulong num2 = TOW * ((ulong) 0xf4240L);
            bytes[9] = (byte) (num2 & ((ulong) 0xffL));
            bytes[8] = (byte) ((num2 >> 8) & ((ulong) 0xffL));
            bytes[7] = (byte) ((num2 >> 0x10) & ((ulong) 0xffL));
            bytes[6] = (byte) ((num2 >> 0x18) & ((ulong) 0xffL));
            bytes[5] = (byte) ((num2 >> 0x20) & ((ulong) 0xffL));
            bytes[10] = 0;
            bytes[11] = 50;
            bytes[12] = 200;
            bytes[13] = utils_AutoReply.EncodeTimeAccuracy((double) num, 0);
            string message = utils_AutoReply.ByteArrayToHexString(bytes);
            int num3 = 14;
            return ("A0A2" + num3.ToString("X").PadLeft(4, '0') + message + utils_AutoReply.GetChecksum(message, true) + "B0B3");
        }

        public string F_TimeTransResp_DataToHex(string F_ICD, bool reject, byte ttType, ushort wkNum, ulong TOW, double acc)
        {
            string str;
            ArrayList fieldList = new ArrayList();
            if (reject)
            {
                str = this.F_Reject(F_ICD, this.F_ID_TIME_TRANS, 4);
            }
            else
            {
                fieldList = utils_AutoReply.GetMessageStructure(base.ProtocolFile, CommunicationManager.ReceiverType.SLC, this.F_ID_TIME_TRANS, 0, this.F_PROTOCOL_NAME, F_ICD);
                int num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "TT_TYPE"))
                {
                    num++;
                }
                SLCMsgStructure structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = ttType.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "GPS_WEEK_NUM"))
                {
                    num++;
                }
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = wkNum.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "GPS_TIME"))
                {
                    num++;
                }
                ulong num2 = TOW * ((ulong) 0xf4240L);
                num2 &= (ulong) 0xffffffffffL;
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = num2.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "DELTAT_UTC"))
                {
                    num++;
                }
                uint num3 = 0x32c8;
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = num3.ToString();
                fieldList[num] = structure;
                num = 0;
                while ((num < fieldList.Count) && (((SLCMsgStructure) fieldList[num]).fieldName != "TIME_ACCURACY"))
                {
                    num++;
                }
                byte num4 = utils_AutoReply.EncodeTimeAccuracy(acc, 0);
                structure = (SLCMsgStructure) fieldList[num];
                structure.defaultValue = num4.ToString();
                fieldList[num] = structure;
                str = utils_AutoReply.FieldList_to_HexString(true, fieldList, this.F_MSG_TYPE);
            }
            fieldList.Clear();
            return str;
        }

        private string FHWConfigRespDataToHex()
        {
            string msgPayload = string.Empty;
            string str2 = string.Empty;
            int num = 0;
            if (base.HWCfgCtrl.Reply)
            {
                return str2;
            }
            if ((base.HWCfgCtrl.PreciseTimeEnabled ^ base.HWCfgCtrl.CoarseTimeEnabled) == 0)
            {
                base.HWCfgCtrl.PreciseTimeEnabled = 0;
                base.HWCfgCtrl.CoarseTimeEnabled = 1;
            }
            num = ((((((base.HWCfgCtrl.PreciseTimeEnabled | (base.HWCfgCtrl.PreciseTimeDirection << 1)) | (base.HWCfgCtrl.FreqAidEnabled << 2)) | (base.HWCfgCtrl.FreqAidMethod << 3)) | (base.HWCfgCtrl.RTCAvailabe << 4)) | (base.HWCfgCtrl.RTCSource << 5)) | (base.HWCfgCtrl.CoarseTimeEnabled << 6)) | (base.HWCfgCtrl.RefClkEnabled << 7);
            if (base.HWCfgCtrl.FreqAidEnabled == 1)
            {
                msgPayload = Convert.ToString(num, 0x10).PadLeft(2, '0') + Convert.ToString((long) (base.HWCfgCtrl.NorminalFreqHz * 0x3e8L), 0x10).PadLeft(10, '0') + Convert.ToString(base.HWCfgCtrl.NetworkEnhanceType, 0x10).PadLeft(2, '0');
            }
            else
            {
                msgPayload = Convert.ToString(num, 0x10).PadLeft(2, '0') + "0000000000" + Convert.ToString(base.HWCfgCtrl.NetworkEnhanceType, 0x10).PadLeft(2, '0');
            }
            return SS3AndGSD3TWReceiver.msgContruct(2, 0x10, msgPayload);
        }

        public override string SetupAuxNavMsgFromTTB(string fullMsg)
        {
            string str = string.Empty;
            if (fullMsg != "")
            {
                string str2 = fullMsg.Replace(" ", "");
                int startIndex = 12;
                string message = "0125" + str2.Substring(startIndex, 20);
                int num2 = message.Length / 2;
                byte[] buffer = utils_AutoReply.HexStringToByteArray("A0A2" + num2.ToString("X").PadLeft(4, '0') + message + utils_AutoReply.GetChecksum(message, true) + "B0B3");
                str = utils_AutoReply.ByteArrayToHexString(this.compressMsg(buffer, buffer.Length));
            }
            return str;
        }

        public override string SetupNavSF45FromTTB(string fullMsg)
        {
            string str = string.Empty;
            if (fullMsg != "")
            {
                string str2 = fullMsg.Replace(" ", "");
                int length = str2.Length;
                string message = "0111" + str2.Substring(12, length - 0x18);
                int num2 = message.Length / 2;
                byte[] buffer = utils_AutoReply.HexStringToByteArray("A0A2" + num2.ToString("X").PadLeft(4, '0') + message + utils_AutoReply.GetChecksum(message, true) + "B0B3");
                str = utils_AutoReply.ByteArrayToHexString(this.compressMsg(buffer, buffer.Length));
            }
            return str;
        }
    }
}

