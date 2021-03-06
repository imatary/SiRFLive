﻿namespace SiRFLive.DeviceControl
{
    using Microsoft.Win32;
    using SiRFLive.General;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Simplex
    {
        private string _IPAddress;
        private string _Port;

        public Simplex()
        {
            this._IPAddress = string.Empty;
            this._Port = string.Empty;
            this._IPAddress = string.Empty;
            this._Port = string.Empty;
        }

        public Simplex(string IPAddress, string Port)
        {
            this._IPAddress = string.Empty;
            this._Port = string.Empty;
            this._IPAddress = IPAddress;
            this._Port = Port;
        }

        public string EndScenario(bool saveLogedData)
        {
            if (this.GetSimStatus() == "Ended")
            {
                return "Already ended";
            }
            this.PopUps(false);
            string datain = string.Format("-,EN,0,{0}", saveLogedData ? 1 : 0);
            string str2 = this.Simplex_Send_Receive(datain);
            string str3 = this.RewindScenario();
            if (str2 != str3)
            {
                return (str2 + "\n" + str3);
            }
            return str3;
        }

        public string GetDeviceInfo()
        {
            return this.Simplex_Send_Receive("*IDN?");
        }

        public string GetSimStatus()
        {
            string inputStr = this.Simplex_Send_Receive("NULL");
            return this.StatusCodeToDescription(inputStr);
        }

        public string GetTimeIntoRun()
        {
            string resp = this.Simplex_Send_Receive("TIME");
            string str2 = this.SimPlex_ParseData(resp);
            if (str2.Length == 0)
            {
                str2 = "0";
            }
            return str2;
        }

        public string PopUps(bool popups)
        {
            string datain = string.Format("POPUPS_ON,{0}", popups ? 1 : 0);
            return this.Simplex_Send_Receive(datain);
        }

        public string RewindScenario()
        {
            return this.Simplex_Send_Receive("RW");
        }

        public string RunScenario()
        {
            string str = this.PopUps(false);
            string str2 = this.Simplex_Send_Receive("RU");
            if (str != str2)
            {
                return (str + "\n" + str2);
            }
            return str2;
        }

        public void RunSimPLEX()
        {
            Process.Start(clsGlobal.InstalledDirectory + @"\scripts\RunSimPLEX.exe");
        }

        public string SelectScenario(string dir)
        {
            string datain = "SC," + dir;
            return this.Simplex_Send_Receive(datain);
        }

        public string SetPowerLevel(string when, float level, uint channel, bool byChannel_SV, bool all_specified, bool abs_relative)
        {
            if (byChannel_SV)
            {
                if ((channel < 0) || (channel > 12))
                {
                    return "Error: Invalid Channel";
                }
            }
            else if ((channel <= 0) || (channel > 0x20))
            {
                return "Error: Invalid SV/PRN";
            }
            string datain = ((when + ",POW_LEV,V1_A1," + string.Format("{0:0.0},", level)) + channel.ToString() + ",") + (byChannel_SV ? "1," : "0,") + (all_specified ? "1," : "0,") + (abs_relative ? "1" : "0");
            return this.Simplex_Send_Receive(datain);
        }

        public string SetPowerOnOff(string when, uint channel, bool powerOnOff, bool byChannel_SV, bool all_specified)
        {
            if (byChannel_SV)
            {
                if ((channel < 0) || (channel > 12))
                {
                    return "Error: Invalid Channel";
                }
            }
            else if ((channel <= 0) || (channel > 0x20))
            {
                return "Error: Invalid SV/PRN";
            }
            string str3 = when + ",POW_ON,V1_A1," + (powerOnOff ? "1," : "0,");
            string datain = str3 + channel.ToString() + "," + (byChannel_SV ? "1," : "0,") + (all_specified ? "1" : "0");
            return this.Simplex_Send_Receive(datain);
        }

        public string SetTriggerMode(SimTriggerMode mode)
        {
            string str;
            switch (mode)
            {
                case SimTriggerMode.Software:
                    str = "TR, 0";
                    break;

                case SimTriggerMode.Ext_Immediate:
                    str = "TR, 1";
                    break;

                case SimTriggerMode.EXT_Next_1PPS:
                    str = "TR, 2";
                    break;

                default:
                    str = "TR, 0";
                    break;
            }
            return this.Simplex_Send_Receive(str);
        }

        public string SimPlex_ParseData(string resp)
        {
            Regex regex = new Regex("<data>");
            Regex regex2 = new Regex("</data>");
            Match match = regex.Match(resp);
            int startIndex = match.Index + match.Length;
            int index = regex2.Match(resp).Index;
            return resp.Substring(startIndex, index - startIndex);
        }

        public string SimPlex_ParseError(string resp)
        {
            Regex regex = new Regex("<error>");
            Regex regex2 = new Regex("</error>");
            Match match = regex.Match(resp);
            int startIndex = match.Index + match.Length;
            int index = regex2.Match(resp).Index;
            string str = resp.Substring(startIndex, index - startIndex);
            if (str.Length == 0)
            {
                str = resp;
            }
            return str;
        }

        public int SimPlex_ParseStatus(string resp)
        {
            Regex regex = new Regex("<status>");
            Regex regex2 = new Regex("</status>");
            Match match = regex.Match(resp);
            if (match.Success)
            {
                int startIndex = match.Index + match.Length;
                Match match2 = regex2.Match(resp);
                if (match2.Success)
                {
                    int index = match2.Index;
                    return int.Parse(resp.Substring(startIndex, index - startIndex), NumberStyles.Number);
                }
                return -1;
            }
            return -1;
        }

        public string Simplex_Send_Receive(string datain)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int port = Convert.ToInt16(this._Port, 10);
            IPEndPoint remoteEP = new IPEndPoint(System.Net.IPAddress.Parse(this._IPAddress), port);
            try
            {
                socket.Connect(remoteEP);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            byte[] bytes = Encoding.ASCII.GetBytes(datain);
            socket.Send(bytes);
            byte[] buffer = new byte[0x100];
            socket.Receive(buffer, buffer.Length, SocketFlags.None);
            string str = Encoding.ASCII.GetString(buffer);
            socket.Close();
            return str;
        }

        public void SpirentRegKey()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Spirent Communications\SimPLEX\SimPLEX Settings");
            key.SetValue("remote enabled", 1);
            key.SetValue("remote manual start", 0);
            key.SetValue("remote from socket", 1);
            key.SetValue("Dummy Run", 0);
            key.SetValue("Auto re-run", 0);
            key.SetValue("UA replay on", 0);
            key.Close();
        }

        public string StatusCodeToDescription(string inputStr)
        {
            switch (this.SimPlex_ParseStatus(inputStr))
            {
                case 0:
                    return "No Scenario Specified";

                case 1:
                    return "Invalid Scenario";

                case 2:
                    return "Initialized";

                case 3:
                    return "Arming";

                case 4:
                    return "Running";

                case 5:
                    return "Paused";

                case 6:
                    return "Ended";
            }
            return inputStr;
        }

        public string IPAddress
        {
            get
            {
                return this._IPAddress;
            }
            set
            {
                this._IPAddress = value;
            }
        }

        public string Port
        {
            get
            {
                return this._Port;
            }
            set
            {
                this._Port = value;
            }
        }

        public enum SimTriggerMode
        {
            Software,
            Ext_Immediate,
            EXT_Next_1PPS
        }
    }
}

