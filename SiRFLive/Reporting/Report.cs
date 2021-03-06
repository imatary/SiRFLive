﻿namespace SiRFLive.Reporting
{
    using LogManagerClassLibrary;
    using SiRFLive.Analysis;
    using SiRFLive.Communication;
    using SiRFLive.Configuration;
    using SiRFLive.General;
    using SiRFLive.GUI;
    using SiRFLive.GUI.General;
    using SiRFLive.MessageHandling;
    using SiRFLive.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using ZedGraph;

    public class Report : IDisposable
    {
        private HelperFunctions _helperFunctions = new HelperFunctions();
        private string _hrErrLimit = string.Empty;
        private string _invalidSV = string.Empty;
        private string _limitVal = string.Empty;
        private NavigationAnalysisData _navReport = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
        private string _percentile = string.Empty;
        private Hashtable _perRxSummary = new Hashtable();
        private PositionErrorCalc _positionErrorCalcEngine = new PositionErrorCalc();
        private string _timeoutVal = string.Empty;
        private string _ttffLimit = string.Empty;
        private int _ttffReportType = 3;
        private bool isDisposed;
        private static List<messageChCounts> MsgChCounts = new List<messageChCounts>();
        private static List<messageCounts> MsgCounts = new List<messageCounts>();
        private static StreamWriter pcLogFileSw;
        private List<double> percentileList;
        public Hashtable perRxSetupData = new Hashtable();
        private Hashtable reportDataHash = new Hashtable();
        public ReportLayoutType ReportLayout;

        private static string AddLogFileListToPrtclCoverageXMLFile(List<string> pcxmllist, string pcsumXML)
        {
            string str = "<LogFileList>";
            string str2 = "</LogFileList>";
            string str3 = pcsumXML.Substring(0, pcsumXML.LastIndexOf(@"\") + 1);
            string str4 = pcsumXML.Substring(pcsumXML.LastIndexOf(@"\") + 1);
            string path = str3 + "LogFileHeader_" + str4;
            StreamReader reader = new StreamReader(pcsumXML);
            StreamWriter writer = new StreamWriter(path);
            while (!reader.EndOfStream)
            {
                string str6 = reader.ReadLine();
                if (str6.IndexOf("<OSPProtocolCoverageTest") >= 0)
                {
                    writer.WriteLine(str6);
                    writer.WriteLine(str);
                    foreach (string str7 in pcxmllist)
                    {
                        string str8 = string.Format("\t<filepath name=\"{0}\"/>", str7);
                        writer.WriteLine(str8);
                    }
                    writer.WriteLine(str2);
                }
                else
                {
                    writer.WriteLine(str6);
                }
            }
            reader.Close();
            writer.Close();
            return path;
        }

        private static void AppLog(string ln)
        {
            string str2 = DateTime.Now.ToString("MMddyyyyHH:mm:ss.ff") + ": " + ln;
            pcLogFileSw.WriteLine(str2);
        }

        public void ConvertCSVToSDOFormat(string resultDir)
        {
            if (Directory.Exists(resultDir))
            {
                FileInfo[] files = new DirectoryInfo(resultDir).GetFiles("*.csv");
                if (files.Length != 0)
                {
                    string path = resultDir + @"\SDO";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    int index = 8;
                    int num2 = index + 1;
                    int num3 = num2 + 3;
                    int num4 = num3 + 3;
                    int num5 = num4 + 5;
                    int num6 = 12;
                    double sample = 0.0;
                    foreach (FileInfo info2 in files)
                    {
                        StreamReader reader = null;
                        StreamWriter s = null;
                        Hashtable dH = null;
                        Hashtable hashtable2 = null;
                        ReportElements elements = null;
                        try
                        {
                            reader = info2.OpenText();
                            string str2 = info2.Name.Replace(".csv", ".yaml");
                            s = new StreamWriter(path + @"\" + str2);
                            string str3 = reader.ReadLine();
                            dH = new Hashtable();
                            elements = new ReportElements();
                            while (str3 != null)
                            {
                                if (str3 == "End Summary")
                                {
                                    break;
                                }
                                if (str3 == string.Empty)
                                {
                                    str3 = reader.ReadLine();
                                }
                                else
                                {
                                    string[] strArray = str3.Split(new char[] { '=' });
                                    if (strArray.Length > 1)
                                    {
                                        string key = strArray[0].Trim();
                                        string str5 = strArray[1].Trim();
                                        if (!dH.ContainsKey(key))
                                        {
                                            dH.Add(key, str5);
                                        }
                                    }
                                    else if (strArray.Length == 1)
                                    {
                                        string str6 = strArray[0].Trim();
                                        if (!dH.ContainsKey(str6))
                                        {
                                            dH.Add(str6, "Unknown");
                                        }
                                    }
                                    str3 = reader.ReadLine();
                                }
                            }
                            if (dH.Count < 1)
                            {
                                reader.Close();
                                reader = null;
                                s.Close();
                                s = null;
                            }
                            else
                            {
                                if (!dH.ContainsKey("Test Data Store"))
                                {
                                    dH.Add("Test Data Store", resultDir);
                                }
                                double num8 = 0.0;
                                double num9 = 0.0;
                                if (dH.ContainsKey("Test Description"))
                                {
                                    string text1 = (string) dH["Test Description"];
                                }
                                if (dH.ContainsKey("TTFF 95% Limit|s"))
                                {
                                    string str7 = (string) dH["TTFF 95% Limit|s"];
                                    if (str7 == "N/A")
                                    {
                                        num8 = -9999.0;
                                    }
                                    else
                                    {
                                        num8 = Convert.ToDouble(str7);
                                    }
                                }
                                if (dH.ContainsKey("2D Position Error 95% Limit|m"))
                                {
                                    string str8 = (string) dH["2D Position Error 95% Limit|m"];
                                    if (str8 == "N/A")
                                    {
                                        num9 = -9999.0;
                                    }
                                    else
                                    {
                                        num9 = Convert.ToDouble(str8);
                                    }
                                }
                                if (dH.ContainsKey("TTFF Timeout"))
                                {
                                    this._timeoutVal = (string) dH["TTFF Timeout"];
                                }
                                this.prinSDOHeader(s, dH);
                                str3 = reader.ReadLine();
                                for (str3 = reader.ReadLine(); str3 != null; str3 = reader.ReadLine())
                                {
                                    string[] strArray2 = str3.Split(new char[] { ',' });
                                    double num10 = -9999.0;
                                    if (strArray2.Length >= 5)
                                    {
                                        double num11 = Convert.ToDouble(strArray2[this._ttffReportType]);
                                        double num12 = Convert.ToDouble(strArray2[index]);
                                        if (num11 <= 0.0)
                                        {
                                            elements.NumberOfTTFFMisses++;
                                        }
                                        else if (num11 == Convert.ToDouble(this._timeoutVal))
                                        {
                                            elements.NumberOfTTFFMisses++;
                                        }
                                        else if ((num11 > num8) && (num8 != 0.0))
                                        {
                                            elements.NumberOfTTFFOutOfLimits++;
                                        }
                                        if (num12 <= 0.0)
                                        {
                                            elements.NumberOf2DMisses++;
                                        }
                                        if ((num12 > num9) && (num9 != 0.0))
                                        {
                                            elements.NumberOf2DOutOfLimits++;
                                        }
                                        if (num11 <= 0.0)
                                        {
                                            elements.NumberOfMisses++;
                                        }
                                        else if (num11 > num8)
                                        {
                                            elements.NumberOfMisses++;
                                        }
                                        else if (num12 <= 0.0)
                                        {
                                            elements.NumberOfMisses++;
                                        }
                                        else if ((num12 > num9) && (num9 != 0.0))
                                        {
                                            elements.NumberOfMisses++;
                                        }
                                        double num13 = Convert.ToDouble(strArray2[num3]);
                                        if (num13 != -9999.0)
                                        {
                                            num10 = Math.Abs((double) (num13 - Convert.ToDouble(strArray2[num4])));
                                        }
                                        elements.TTFFSamples.InsertSample(num11);
                                        elements.Position2DErrorSamples.InsertSample(num12);
                                        elements.VerticalErrorSamples.InsertSample(num10);
                                        for (int i = 0; i < num6; i++)
                                        {
                                            sample = Convert.ToDouble(strArray2[num5 + i]);
                                            if (sample != 0.0)
                                            {
                                                elements.CNOSamples.InsertSample(sample);
                                            }
                                        }
                                    }
                                }
                                hashtable2 = new Hashtable();
                                double percentile = elements.TTFFSamples.GetPercentile((double) 95.0, -9999.0);
                                double num16 = elements.TTFFSamples.Stats_Max((double) -9999.0);
                                double num17 = elements.Position2DErrorSamples.GetPercentile((double) 95.0, -9999.0);
                                double num18 = elements.Position2DErrorSamples.Stats_Max((double) -9999.0);
                                double num19 = elements.VerticalErrorSamples.GetPercentile((double) 95.0, -9999.0);
                                double num20 = elements.VerticalErrorSamples.Stats_Max((double) -9999.0);
                                double num21 = elements.CNOSamples.GetPercentile((double) 95.0, -9999.0);
                                double num22 = elements.CNOSamples.Stats_Max((double) -9999.0);
                                string str9 = "Test Result";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt = new SDOStatsElememt();
                                    elememt.Value = "N/A";
                                    hashtable2.Add(str9, elememt);
                                }
                                str9 = "Samples";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt2 = new SDOStatsElememt();
                                    elememt2.Value = elements.TTFFSamples.Samples.ToString("D");
                                    hashtable2.Add(str9, elememt2);
                                }
                                str9 = "Number of Misses TTFF";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt3 = new SDOStatsElememt();
                                    elememt3.Value = elements.NumberOfTTFFMisses.ToString("D");
                                    hashtable2.Add(str9, elememt3);
                                }
                                str9 = "Number of Over Limit TTFF";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt4 = new SDOStatsElememt();
                                    elememt4.Value = elements.NumberOfTTFFOutOfLimits.ToString("D");
                                    hashtable2.Add(str9, elememt4);
                                }
                                str9 = "Number of Misses 2D Position";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt5 = new SDOStatsElememt();
                                    elememt5.Value = elements.NumberOf2DMisses.ToString("D");
                                    hashtable2.Add(str9, elememt5);
                                }
                                str9 = "Number of Over Limit 2D Position";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt6 = new SDOStatsElememt();
                                    elememt6.Value = elements.NumberOf2DOutOfLimits.ToString("D");
                                    hashtable2.Add(str9, elememt6);
                                }
                                str9 = "TTFF 95%|s";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt7 = new SDOStatsElememt();
                                    if (num8 != 0.0)
                                    {
                                        elememt7.Target = num8.ToString("F2");
                                        elememt7.Comparison = "<";
                                        elememt7.ComparisonResult = (percentile < num8) ? "Pass" : "False";
                                    }
                                    elememt7.Value = percentile.ToString("F2");
                                    hashtable2.Add(str9, elememt7);
                                }
                                str9 = "TTFF Max|s";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt8 = new SDOStatsElememt();
                                    elememt8.Value = num16.ToString("F2");
                                    hashtable2.Add(str9, elememt8);
                                }
                                str9 = "2D Position Error 95%|m";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt9 = new SDOStatsElememt();
                                    if (num9 != 0.0)
                                    {
                                        elememt9.Target = num9.ToString("F2");
                                        elememt9.Comparison = "<";
                                        elememt9.ComparisonResult = (num17 < num9) ? "Pass" : "False";
                                    }
                                    elememt9.Value = num17.ToString("F6");
                                    hashtable2.Add(str9, elememt9);
                                }
                                str9 = "2D Position Error Max|m";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt10 = new SDOStatsElememt();
                                    elememt10.Value = num18.ToString("F6");
                                    hashtable2.Add(str9, elememt10);
                                }
                                str9 = "Vertical Position Error 95%|m";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt11 = new SDOStatsElememt();
                                    elememt11.Value = num19.ToString("F6");
                                    hashtable2.Add(str9, elememt11);
                                }
                                str9 = "Vertical Position Error Max|m";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt12 = new SDOStatsElememt();
                                    elememt12.Value = num20.ToString("F6");
                                    hashtable2.Add(str9, elememt12);
                                }
                                str9 = "CNo 95%|dBHz";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt13 = new SDOStatsElememt();
                                    elememt13.Value = num21.ToString("F2");
                                    hashtable2.Add(str9, elememt13);
                                }
                                str9 = "CNo Max|dBHz";
                                if (!hashtable2.ContainsKey(str9))
                                {
                                    SDOStatsElememt elememt14 = new SDOStatsElememt();
                                    elememt14.Value = num22.ToString("F2");
                                    hashtable2.Add(str9, elememt14);
                                }
                                this.printResultStats(s, hashtable2);
                                reader.Close();
                                reader = null;
                                s.Close();
                                s = null;
                                hashtable2.Clear();
                                hashtable2 = null;
                            }
                        }
                        catch (Exception exception)
                        {
                            if (reader != null)
                            {
                                reader.Close();
                                reader = null;
                            }
                            if (s != null)
                            {
                                s.Close();
                                s = null;
                            }
                            if (hashtable2 != null)
                            {
                                hashtable2.Clear();
                                hashtable2 = null;
                            }
                            if (dH != null)
                            {
                                dH.Clear();
                                dH = null;
                            }
                            if (elements != null)
                            {
                                elements.Dispose();
                                dH = null;
                            }
                            throw exception;
                        }
                    }
                }
            }
        }

        public void ConvertXMLToSDOFormat(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileInfo info = new FileInfo(filePath);
                string directoryName = info.DirectoryName;
                string path = directoryName + @"\SDO";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                XmlDocument document = new XmlDocument();
                document.Load(filePath);
                XmlElement documentElement = document.DocumentElement;
                XmlNode node = documentElement.SelectSingleNode("testSetup");
                Hashtable hashtable = new Hashtable();
                List<Hashtable> list = new List<Hashtable>();
                if (node != null)
                {
                    string key = "RX Name";
                    foreach (XmlElement element2 in node.ChildNodes)
                    {
                        Hashtable item = new Hashtable();
                        if (!item.Contains(key))
                        {
                            item.Add(key, element2.GetAttribute("name").ToString().Trim());
                        }
                        foreach (XmlElement element3 in element2.ChildNodes)
                        {
                            if (!item.Contains(element3.GetAttribute("name").ToString().Trim()))
                            {
                                item.Add(element3.GetAttribute("name").ToString().Trim(), element3.GetAttribute("value").ToString().Trim());
                            }
                        }
                        item.Add("Test Data Store", directoryName);
                        list.Add(item);
                    }
                }
                int num = 1;
                foreach (XmlNode node2 in documentElement)
                {
                    if (node2.Name != "testSetup")
                    {
                        string str5 = node2.Attributes["name"].Value.ToString().Trim();
                        foreach (XmlElement element4 in node2.ChildNodes)
                        {
                            string str6 = element4.GetAttribute("name").ToString().Trim();
                            foreach (XmlElement element5 in element4.ChildNodes)
                            {
                                string str7 = element5.GetAttribute("number").ToString().Replace(".", "p").Trim();
                                StreamWriter s = new StreamWriter(string.Format(@"{0}\{1}_{2}_Idx{3}.yaml", new object[] { path, str6, str7, num }));
                                bool flag = false;
                                string str9 = string.Empty;
                                string str10 = string.Empty;
                                int num2 = 0;
                                foreach (Hashtable hashtable3 in list)
                                {
                                    num2++;
                                    if (hashtable3.ContainsKey("SW Version"))
                                    {
                                        str9 = (string) hashtable3["SW Version"];
                                    }
                                    if (hashtable3.ContainsKey("Test"))
                                    {
                                        str10 = (string) hashtable3["Test"];
                                    }
                                    if ((str9 != string.Empty) && (str10 != string.Empty))
                                    {
                                        if (!(str9 == str5) || !(str10 == str6))
                                        {
                                            continue;
                                        }
                                        flag = true;
                                        break;
                                    }
                                    str9 = string.Empty;
                                    str10 = string.Empty;
                                    flag = false;
                                }
                                if (!flag)
                                {
                                    this.prinSDOHeader(s, null);
                                }
                                else
                                {
                                    this.prinSDOHeader(s, list[--num2]);
                                }
                                s.WriteLine("Test Statistics:");
                                foreach (XmlElement element6 in element5.ChildNodes)
                                {
                                    StringBuilder builder = new StringBuilder();
                                    string str11 = element6.GetAttribute("name").ToString().Trim();
                                    string str12 = element6.GetAttribute("units").ToString().Trim();
                                    if (str12 != string.Empty)
                                    {
                                        builder.Append(string.Format("\t{0}|{1}:", str11, str12));
                                    }
                                    else
                                    {
                                        builder.Append(string.Format("\t{0}:", str11));
                                    }
                                    string str13 = element6.GetAttribute("value").ToString().Trim();
                                    string str14 = element6.GetAttribute("criteria").ToString().Trim();
                                    if ((str14 != string.Empty) && (str14 != "Pass/Fail"))
                                    {
                                        double result = 0.0;
                                        double num4 = 0.0;
                                        double.TryParse(str13, out result);
                                        double.TryParse(str14, out num4);
                                        builder.Append("\r\n");
                                        builder.Append(string.Format("\t\t- Target:{0}\r\n", str14));
                                        builder.Append(string.Format("\t\t- Result:{0}\r\n", str13));
                                        bool isTrue = false;
                                        string str15 = this.getComparisonString(element6.GetAttribute("direction").ToString().Trim(), result, num4, out isTrue);
                                        builder.Append(string.Format("\t\t- Comparison:{0}\r\n", str15));
                                        builder.Append(string.Format("\t\t- Comparison Result:{0}\r\n", isTrue ? "Pass" : "False"));
                                    }
                                    else
                                    {
                                        builder.Append(string.Format("{0}", str13));
                                    }
                                    s.WriteLine(builder.ToString());
                                }
                                s.Close();
                                s.Dispose();
                                s = null;
                            }
                        }
                        num++;
                    }
                }
                info = null;
                document = null;
                documentElement = null;
                node = null;
                hashtable.Clear();
                hashtable = null;
                list.Clear();
                list = null;
                directoryName = null;
                path = null;
            }
        }

        public static void CreateDirectoryProtocolCoverageReport(string dirPath)
        {
            List<string> filelist = new List<string>();
            List<string> pcxmllist = new List<string>();
            string str = DateTime.Now.ToString("MMddyyyy_HHmmss");
            pcLogFileSw = new StreamWriter(dirPath + @"\ProtocolCoverageSummary-" + str + ".log");
            AppLog(string.Format("Starting Protocol Coverage Summary processing of directory {0}", dirPath));
            List<string> list3 = RecuresivelyBuildFilenameList("*.gps", dirPath);
            List<string> list4 = RecuresivelyBuildFilenameList("*.gp2", dirPath);
            foreach (string str4 in list3)
            {
                filelist.Add(str4);
            }
            foreach (string str5 in list4)
            {
                filelist.Add(str5);
            }
            string ln = "Starting to create Protocol Coverage Count XML files";
            AppLog(ln);
            foreach (string str6 in filelist)
            {
                string item = CreateProtocolCoverageCountXMLFile(str6);
                pcxmllist.Add(item);
            }
            ln = "Completed creating Protocol Coverage Count XML files";
            AppLog(ln);
            CreateProtocolCoverageTotalsReport(dirPath, pcxmllist, filelist);
            ln = string.Format("*** Completed Protocol Coverage Summary Report for {0} ***", dirPath);
            Console.WriteLine(ln);
            AppLog(ln);
            pcLogFileSw.Close();
        }

        private static string CreateProtocolCoverageCountXMLFile(string fp)
        {
            string str = "";
            switch (fp.Substring(fp.Length - 3))
            {
                case "gps":
                    return NewProtocolGPSCoverageXMLFile(fp);

                case "gp2":
                    str = NewProtocolGP2CoverageXMLFile(fp);
                    break;
            }
            return str;
        }

        private static string CreateProtocolCoverageTotalsReport(string dirPath, List<string> pcxmllist, List<string> filelist)
        {
            string filename = "";
            string ln = "Starting to create summary report";
            AppLog(ln);
            ReadOSPMasterListIntoMsgCounts();
            foreach (string str3 in pcxmllist)
            {
                StreamReader xmlSr = new StreamReader(str3);
                UpdateMsgCountTotals(xmlSr);
                xmlSr.Close();
            }
            string filepath = dirPath + @"\ProtocolCoverageSummary.xml";
            CreatePrtclCoverageXMLFile(filepath);
            string uri = AddLogFileListToPrtclCoverageXMLFile(filelist, filepath);
            ln = "Beginning translation into HTML";
            AppLog(ln);
            try
            {
                string str6 = ConfigurationManager.AppSettings["InstalledDirectory"];
                string stylesheetUri = str6 + @"\scripts\ProtocolHeaderCoverageReport.xsl";
                filename = dirPath + @"\ProtocolCoverageSummary.html";
                XPathDocument document = new XPathDocument(uri);
                XslCompiledTransform transform = new XslCompiledTransform();
                XmlTextWriter writer = new XmlTextWriter(filename, null);
                transform.Load(stylesheetUri);
                transform.Transform((IXPathNavigable) document, null, (XmlWriter) writer);
                writer.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR!");
            }
            return filename;
        }

        private static string CreatePrtclCoverageXMLFile(string filepath)
        {
            string str = "";
            string path = "";
            if (filepath.IndexOf(".xml") > 0)
            {
                path = filepath;
            }
            else
            {
                if (filepath.IndexOf(".gp2") > 0)
                {
                    str = filepath.Replace(".gp2", ".xml");
                }
                else if (filepath.IndexOf(".gps") > 0)
                {
                    str = filepath.Replace(".gps", ".xml");
                }
                string str3 = str.Substring(0, str.LastIndexOf(@"\") + 1);
                string str4 = str.Substring(str.LastIndexOf(@"\") + 1);
                path = str3 + "ProtocolCoverage_" + str4;
            }
            StreamWriter writer = new StreamWriter(path);
            MsgCounts.Sort(new Comparison<messageCounts>(Report.MsgCountCompare));
            string str5 = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            string str6 = "\t</swVersion>";
            string str7 = "<OSPProtocolCoverageTest>";
            string str8 = "</OSPProtocolCoverageTest>";
            string str9 = "\t\t</midID_subID>";
            writer.WriteLine(str5);
            writer.WriteLine(str7);
            string str10 = string.Format("SiRFLive {0}", SiRFLiveVersion.VersionNum);
            int num = 0;
            for (int i = 0; i < MsgCounts.Count; i++)
            {
                if (MsgCounts[i].count != 0)
                {
                    num++;
                }
            }
            float num3 = ((float) num) / ((float) MsgCounts.Count);
            string str11 = string.Format("\t<swVersion name=\"{0}\" percentage_of_coverage=\"{1:f2}\" non-zero-msgs=\"{2}\" msg-count=\"{3}\" filepath=\"{4}\">", new object[] { str10, num3, num, MsgCounts.Count, filepath });
            writer.WriteLine(str11);
            for (int j = 0; j < MsgCounts.Count; j++)
            {
                string str13 = string.Format("\t\t<midID_subID number=\"{0},{1}\" name=\"{2}\">", MsgCounts[j].mid, MsgCounts[j].subID, MsgCounts[j].name);
                writer.WriteLine(str13);
                string str12 = string.Format("\t\t\t<count value=\"{0}\"/>", MsgCounts[j].count);
                writer.WriteLine(str12);
                writer.WriteLine(str9);
            }
            writer.WriteLine(str6);
            writer.WriteLine(str8);
            writer.Close();
            Console.WriteLine(string.Format("Completed {0}", path));
            return path;
        }

        private static void CreatePrtclGP2ChannelCoverageXMLFile()
        {
            string str = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamWriter writer = new StreamWriter(str + @"\Log\PrtocolGP2Coverage.xml");
            MsgChCounts.Sort(new Comparison<messageChCounts>(Report.MsgChCountCompare));
            string str3 = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            string str4 = "\t<swVersion name=\"GSW3.5.0\">";
            string str5 = "\t</swVersion>";
            string str6 = "<OSPProtocolCoverageTest>";
            string str7 = "</OSPProtocolCoverageTest>";
            string str8 = "\t\t</midID_subID>";
            writer.WriteLine(str3);
            writer.WriteLine(str6);
            writer.WriteLine(str4);
            for (int i = 0; i < MsgChCounts.Count; i++)
            {
                string str10 = string.Format("\t\t<midID_subID number=\"{0},{1}\" name=\"{2}\">", MsgChCounts[i].mid, MsgChCounts[i].subID, MsgChCounts[i].name);
                writer.WriteLine(str10);
                string str9 = string.Format("\t\t\t<channel name=\"D\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[0]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"F\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[1]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"CP_Centric\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[2]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"CP_Channel\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[3]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"CP_Channel\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[4]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"SL_Stats\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[5]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"Timexfr\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[6]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"NMEA\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[7]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"SSB\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[8]);
                writer.WriteLine(str9);
                str9 = string.Format("\t\t\t<channel name=\"DEBUG\" value=\"{0}\"/>", MsgChCounts[i].channelIDCount[9]);
                writer.WriteLine(str9);
                writer.WriteLine(str8);
            }
            writer.WriteLine(str5);
            writer.WriteLine(str7);
            writer.Close();
        }

        private static void CreatePrtclGP2CoverageXMLFile()
        {
            string str = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamWriter writer = new StreamWriter(str + @"\Log\PrtocolGP2Coverage.xml");
            MsgCounts.Sort(new Comparison<messageCounts>(Report.MsgCountCompare));
            string str3 = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            string str4 = "\t<swVersion name=\"GSW3.5.0\">";
            string str5 = "\t</swVersion>";
            string str6 = "<OSPProtocolCoverageTest>";
            string str7 = "</OSPProtocolCoverageTest>";
            string str8 = "\t\t</midID_subID>";
            writer.WriteLine(str3);
            writer.WriteLine(str6);
            writer.WriteLine(str4);
            for (int i = 0; i < MsgCounts.Count; i++)
            {
                string str10 = string.Format("\t\t<midID_subID number=\"{0},{1}\" name=\"{2}\">", MsgCounts[i].mid, MsgCounts[i].subID, MsgCounts[i].name);
                writer.WriteLine(str10);
                string str9 = string.Format("\t\t\t<count value=\"{0}\"/>", MsgCounts[i].count);
                writer.WriteLine(str9);
                writer.WriteLine(str8);
            }
            writer.WriteLine(str5);
            writer.WriteLine(str7);
            writer.Close();
            Console.WriteLine("All Done");
        }

        public void CW_Summary(string dir)
        {
            Hashtable hashtable = new Hashtable();
            string path = dir + @"\summary_cw.xml";
            StreamWriter f = new StreamWriter(path);
            try
            {
                if (!Directory.Exists(dir))
                {
                    MessageBox.Show(string.Format("Directory Not Found!\n\n{0}", dir), "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                    if (files.Length != 0)
                    {
                        foreach (FileInfo info2 in files)
                        {
                            Header1DataClass class2;
                            PerEnvReport report;
                            PerPowerReport report2;
                            string str10;
                            int num2;
                            List<string> list;
                            Invalid_Data data = new Invalid_Data();
                            StreamReader reader = info2.OpenText();
                            string name = info2.Name;
                            string pattern = @"_P(?<state>\d)_(?<power>-?\d+.\d?)_(?<msgID>msg\d+)";
                            Regex regex = new Regex(pattern, RegexOptions.Compiled);
                            bool flag = regex.IsMatch(name);
                            string str4 = "Unknown";
                            string str5 = "Unknown";
                            string str6 = "Unknown";
                            string key = "Unknown";
                            if (flag)
                            {
                                str4 = regex.Match(name).Result("${state}");
                                str5 = regex.Match(name).Result("${msgID}");
                                str6 = regex.Match(name).Result("${power}");
                            }
                            string str8 = string.Empty;
                            string str9 = string.Empty;
                            if (str4 == "0")
                            {
                                str9 = "RF OFF -- CW(" + str6 + ")";
                            }
                            else
                            {
                                str9 = "RF ON -- CW(" + str6 + ")";
                            }
                            string str26 = str5;
                            if (str26 != null)
                            {
                                if (!(str26 == "msg2"))
                                {
                                    if (str26 == "msg28")
                                    {
                                        goto Label_0185;
                                    }
                                    if (str26 == "msg41")
                                    {
                                        goto Label_018E;
                                    }
                                }
                                else
                                {
                                    key = "Nav";
                                }
                            }
                            goto Label_0195;
                        Label_0185:
                            key = "Track";
                            goto Label_0195;
                        Label_018E:
                            key = "Nav";
                        Label_0195:
                            str10 = reader.ReadLine();
                            if (str10 != null)
                            {
                                pattern = "SW Version:(?<swVer>.*)";
                                Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                                if (regex2.IsMatch(str10))
                                {
                                    str8 = regex2.Match(str10).Result("${swVer}");
                                }
                                else
                                {
                                    str8 = "SW Version: Not detected";
                                }
                            }
                            else
                            {
                                str8 = "SW Version: Not detected";
                            }
                            if (hashtable.Contains(str8))
                            {
                                class2 = (Header1DataClass) hashtable[str8];
                            }
                            else
                            {
                                Header1DataClass class3 = new Header1DataClass();
                                hashtable.Add(str8, class3);
                                class2 = (Header1DataClass) hashtable[str8];
                            }
                            if (class2.Header2DataHash.Contains(key))
                            {
                                report = (PerEnvReport) class2.Header2DataHash[key];
                            }
                            else
                            {
                                PerEnvReport report3 = new PerEnvReport();
                                class2.Header2DataHash.Add(key, report3);
                                report = (PerEnvReport) class2.Header2DataHash[key];
                            }
                            if (report.PerSiteData.Contains(str9))
                            {
                                report2 = (PerPowerReport) report.PerSiteData[str9];
                            }
                            else
                            {
                                PerPowerReport report4 = new PerPowerReport();
                                report.PerSiteData.Add(str9, report4);
                                report2 = (PerPowerReport) report.PerSiteData[str9];
                            }
                            str10 = reader.ReadLine();
                            int num = 0;
                            string str11 = "N/A";
                            Hashtable hashtable2 = new Hashtable();
                            if (str10 != null)
                            {
                                string[] strArray = str10.Split(new char[] { '=' });
                                num2 = 5;
                                if (strArray.Length > 1)
                                {
                                    str11 = strArray[1];
                                    if (this.perRxSetupData.ContainsKey(str11))
                                    {
                                        goto Label_039C;
                                    }
                                    if (strArray.Length > 1)
                                    {
                                        hashtable2.Add(strArray[0], strArray[1]);
                                    }
                                    while (((str10 = reader.ReadLine()) != null) && (num < num2))
                                    {
                                        strArray = str10.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable2.Add(strArray[0], strArray[1]);
                                        }
                                        num++;
                                    }
                                    this.perRxSetupData.Add(str11, hashtable2);
                                }
                            }
                            goto Label_03AE;
                        Label_0396:
                            num++;
                        Label_039C:
                            if (((str10 = reader.ReadLine()) != null) && (num < num2))
                            {
                                goto Label_0396;
                            }
                        Label_03AE:
                            list = new List<string>();
                            if (this.perRxSetupData.ContainsKey(str11))
                            {
                                Hashtable hashtable3 = (Hashtable) this.perRxSetupData[str11];
                                string str12 = "SV List";
                                if (hashtable3.ContainsKey(str12))
                                {
                                    foreach (string str13 in ((string) hashtable3[str12]).Split(new char[] { ',' }))
                                    {
                                        if (!list.Contains(str13))
                                        {
                                            list.Add(str13);
                                        }
                                    }
                                }
                            }
                            Invalid_Data data2 = new Invalid_Data();
                            while (str10 != null)
                            {
                                string str14;
                                string str15;
                                int num5;
                                string[] strArray3 = str10.Split(new char[] { ',' });
                                if (strArray3.Length >= 5)
                                {
                                    str14 = strArray3[2];
                                    str15 = strArray3[3];
                                    str26 = strArray3[4];
                                    if (str26 != null)
                                    {
                                        if (!(str26 == "2"))
                                        {
                                            if (str26 == "28")
                                            {
                                                goto Label_0659;
                                            }
                                            if (str26 == "41")
                                            {
                                                goto Label_079C;
                                            }
                                        }
                                        else
                                        {
                                            string str17 = strArray3[0x10];
                                            if (report2.PerFreqData.Contains("Total"))
                                            {
                                                data = (Invalid_Data) report2.PerFreqData["Total"];
                                            }
                                            else
                                            {
                                                Invalid_Data data3 = new Invalid_Data();
                                                report2.PerFreqData.Add("Total", data3);
                                                data = (Invalid_Data) report2.PerFreqData["Total"];
                                            }
                                            data.EpochCount++;
                                            if (strArray3.Length >= 20)
                                            {
                                                if (Convert.ToInt16(str17) > 4)
                                                {
                                                    if (report2.PerFreqData.Contains(str14))
                                                    {
                                                        data2 = (Invalid_Data) report2.PerFreqData[str14];
                                                    }
                                                    else
                                                    {
                                                        Invalid_Data data4 = new Invalid_Data();
                                                        report2.PerFreqData.Add(str14, data4);
                                                        data2 = (Invalid_Data) report2.PerFreqData[str14];
                                                    }
                                                    data2.InvalidSVsCount++;
                                                    data.InvalidSVsCount++;
                                                }
                                                else
                                                {
                                                    int num3 = 0;
                                                    int num4 = 12;
                                                    for (num3 = 0; num3 < num4; num3++)
                                                    {
                                                        if (strArray3[num3 + 0x11] != "0")
                                                        {
                                                            string str18 = strArray3[num3 + 0x11];
                                                            if (!list.Contains(str18))
                                                            {
                                                                data2.InvalidSVsCount++;
                                                                data.InvalidSVsCount++;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (report2.PerFreqData.Contains(str14))
                                                {
                                                    data2 = (Invalid_Data) report2.PerFreqData[str14];
                                                    data2.EpochCount++;
                                                }
                                            }
                                        }
                                    }
                                }
                                goto Label_08A8;
                            Label_0659:
                                num5 = Convert.ToInt16(strArray3[13]);
                                string item = strArray3[7];
                                if (report2.PerFreqData.Contains("Total"))
                                {
                                    data = (Invalid_Data) report2.PerFreqData["Total"];
                                }
                                else
                                {
                                    Invalid_Data data5 = new Invalid_Data();
                                    report2.PerFreqData.Add("Total", data5);
                                    data = (Invalid_Data) report2.PerFreqData["Total"];
                                }
                                if ((num5 & 1) != 0)
                                {
                                    int num6 = 0;
                                    int num7 = 0;
                                    for (num7 = 0; num7 < 10; num7++)
                                    {
                                        num6 += Convert.ToInt16(strArray3[num7 + 14]);
                                    }
                                    double num1 = ((double) num6) / 10.0;
                                    if ((str15 == "0") || !list.Contains(item))
                                    {
                                        if (report2.PerFreqData.Contains(str14))
                                        {
                                            data2 = (Invalid_Data) report2.PerFreqData[str14];
                                        }
                                        else
                                        {
                                            Invalid_Data data6 = new Invalid_Data();
                                            report2.PerFreqData.Add(str14, data6);
                                            data2 = (Invalid_Data) report2.PerFreqData[str14];
                                        }
                                        data2.InvalidSVsCount++;
                                        data.InvalidSVsCount++;
                                    }
                                }
                                goto Label_08A8;
                            Label_079C:
                                if (report2.PerFreqData.Contains("Total"))
                                {
                                    data = (Invalid_Data) report2.PerFreqData["Total"];
                                }
                                else
                                {
                                    Invalid_Data data7 = new Invalid_Data();
                                    report2.PerFreqData.Add("Total", data7);
                                    data = (Invalid_Data) report2.PerFreqData["Total"];
                                }
                                data.EpochCount++;
                                if (report2.PerFreqData.Contains(str14))
                                {
                                    data2 = (Invalid_Data) report2.PerFreqData[str14];
                                }
                                else
                                {
                                    Invalid_Data data8 = new Invalid_Data();
                                    report2.PerFreqData.Add(str14, data8);
                                    data2 = (Invalid_Data) report2.PerFreqData[str14];
                                }
                                if ((str15 == "1") && (Convert.ToDouble(strArray3[strArray3.Length - 1]) > Convert.ToDouble(this._hrErrLimit)))
                                {
                                    data2.HrErrCount++;
                                    data.HrErrCount++;
                                }
                            Label_08A8:
                                str10 = reader.ReadLine();
                            }
                            reader.Close();
                        }
                        f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        f.WriteLine("<cw>");
                        foreach (string str20 in hashtable.Keys)
                        {
                            f.WriteLine("\t<swVersion name=\"{0}\">", str20);
                            Header1DataClass class4 = (Header1DataClass) hashtable[str20];
                            int num9 = 0;
                            int num10 = 0;
                            foreach (string str21 in class4.Header2DataHash.Keys)
                            {
                                f.WriteLine("\t\t<msg name=\"{0}\">", str21);
                                PerEnvReport report5 = (PerEnvReport) class4.Header2DataHash[str21];
                                foreach (string str22 in report5.PerSiteData.Keys)
                                {
                                    f.WriteLine("\t\t\t<environment name=\"{0}\">", str22);
                                    PerPowerReport report6 = (PerPowerReport) report5.PerSiteData[str22];
                                    foreach (string str23 in report6.PerFreqData.Keys)
                                    {
                                        Invalid_Data data9 = (Invalid_Data) report6.PerFreqData[str23];
                                        if (str21 == "Nav")
                                        {
                                            num9 = data9.EpochCount / 2;
                                        }
                                        if (((data9.InvalidSVsCount != 0) || (data9.HrErrCount != 0)) && !(str23 == "Total"))
                                        {
                                            f.WriteLine("\t\t\t\t<freq value=\"{0}\">", str23);
                                            f.WriteLine("\t\t\t\t\t<field name=\"# Invalid SV\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", data9.InvalidSVsCount, this._invalidSV);
                                            f.WriteLine("\t\t\t\t\t<field name=\"# Hr Error &gt;{0} m\" value=\"{1}\" criteria=\"{2}\" direction=\"&lt;\" units=\"\"/>", this._hrErrLimit, data9.HrErrCount, this._limitVal);
                                            f.WriteLine("\t\t\t\t\t<field name=\"# Epoch\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num9);
                                            f.WriteLine("\t\t\t\t</freq>");
                                        }
                                    }
                                    string str24 = "Total";
                                    Invalid_Data data10 = (Invalid_Data) report6.PerFreqData[str24];
                                    f.WriteLine("\t\t\t\t<freq value=\"{0}\">", str24);
                                    if (data10 != null)
                                    {
                                        if (str21 == "Nav")
                                        {
                                            num10 = data10.EpochCount / 2;
                                        }
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Invalid SV\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", data10.InvalidSVsCount, this._invalidSV);
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Hr Error &gt;{0} m\" value=\"{1}\" criteria=\"{2}\" direction=\"&lt;\" units=\"\"/>", this._hrErrLimit, data10.HrErrCount, this._limitVal);
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Epoch\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num10);
                                        f.WriteLine("\t\t\t\t</freq>");
                                    }
                                    else
                                    {
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Invalid SV\" value=\"0\" criteria=\"{0}\" direction=\"&lt;\" units=\"\"/>", this._invalidSV);
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Hr Error &gt;{0} m\" value=\"0\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", this._hrErrLimit, this._limitVal);
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Epoch\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num10);
                                        f.WriteLine("\t\t\t\t</freq>");
                                    }
                                    f.WriteLine("\t\t\t</environment>");
                                }
                                f.WriteLine("\t\t</msg>");
                            }
                            f.WriteLine("\t</swVersion>");
                        }
                        this.printTestSetup(f);
                        f.WriteLine("</cw>");
                        f.Close();
                        hashtable.Clear();
                        this.perRxSetupData.Clear();
                        string outputFile = dir + @"\summary_cw.html";
                        this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\cwReport.xsl");
                    }
                }
            }
            catch (Exception exception)
            {
                this.perRxSetupData.Clear();
                hashtable.Clear();
                f.Close();
                MessageBox.Show(exception.Message, "Error");
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                if (this.percentileList != null)
                {
                    this.percentileList.Clear();
                }
                this.percentileList = null;
                if (this._perRxSummary != null)
                {
                    this._perRxSummary.Clear();
                }
                this._perRxSummary = null;
                if (this.perRxSetupData != null)
                {
                    this.perRxSetupData.Clear();
                }
                this.perRxSetupData = null;
            }
            this.isDisposed = true;
        }

        public void E911_Summary(string dir)
        {
            Hashtable hashtable = new Hashtable();
            int index = 8;
            int num2 = index + 1;
            int num3 = num2 + 3;
            int num4 = num3 + 3;
            int num5 = num4 + 5;
            int num6 = 12;
            double sample = 0.0;
            string path = dir + @"\summary_e911.xml";
            StreamWriter f = new StreamWriter(path);
            try
            {
                if (!Directory.Exists(dir))
                {
                    MessageBox.Show(string.Format("Directory Not Found!\n\n{0}", dir), "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                    if (files.Length != 0)
                    {
                        foreach (FileInfo info2 in files)
                        {
                            Header1DataClass class2;
                            PerEnvReport report;
                            ReportElements elements;
                            StreamReader reader = info2.OpenText();
                            string name = info2.Name;
                            string pattern = @"_Idx\d+_(?<site>\w+_Site\d+)(?<subName>\D*)_";
                            Regex regex = new Regex(pattern, RegexOptions.Compiled);
                            bool flag = regex.IsMatch(name);
                            string key = "Unknown";
                            string str5 = "Unknown";
                            if (flag)
                            {
                                key = regex.Match(name).Result("${site}");
                                str5 = regex.Match(name).Result("${subName}");
                            }
                            string str6 = string.Empty;
                            string str7 = string.Empty;
                            switch (key)
                            {
                                case "Seattle_Site101":
                                case "Seattle_Site107":
                                case "Dallas_Site115":
                                case "Dallas_Site116":
                                    str7 = "Indoor";
                                    break;

                                case "Seattle_Site11":
                                case "Seattle_Site19":
                                case "Seattle_Site23":
                                case "Dallas_Site106":
                                case "Dallas_Site105":
                                case "Dallas_Site107":
                                case "Dallas_Site108":
                                    str7 = "Urban";
                                    break;

                                case "Seattle_Site4":
                                case "Seattle_Site29":
                                case "Dallas_Site109":
                                case "Dallas_Site110":
                                case "Dallas_Site112":
                                    str7 = "Suburban";
                                    break;

                                case "Seattle_Site17":
                                case "Seattle_Site20":
                                case "Seattle_Site25":
                                case "Dallas_Site101":
                                case "Dallas_Site102":
                                case "Dallas_Site103":
                                case "Dallas_Site104":
                                    str7 = "Dense Urban";
                                    break;

                                case "Seattle_Site3":
                                case "Seattle_Site103":
                                case "Seattle_Site105":
                                case "Dallas_Site111":
                                case "Dallas_Site113":
                                case "Dallas_Site114":
                                    str7 = "Rural";
                                    break;

                                default:
                                    str7 = str5;
                                    key = str5;
                                    break;
                            }
                            string input = reader.ReadLine();
                            if (input != null)
                            {
                                pattern = @"SW Version\s*=\s*(?<swVer>.*)";
                                Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                                if (regex2.IsMatch(input))
                                {
                                    str6 = regex2.Match(input).Result("${swVer}");
                                }
                                else
                                {
                                    str6 = "SW Version: Not detected";
                                }
                            }
                            else
                            {
                                str6 = "SW Version: Not detected";
                            }
                            if (hashtable.Contains(str6))
                            {
                                class2 = (Header1DataClass) hashtable[str6];
                            }
                            else
                            {
                                Header1DataClass class3 = new Header1DataClass();
                                hashtable.Add(str6, class3);
                                class2 = (Header1DataClass) hashtable[str6];
                            }
                            if (class2.Header2DataHash.Contains(str7))
                            {
                                report = (PerEnvReport) class2.Header2DataHash[str7];
                            }
                            else
                            {
                                PerEnvReport report2 = new PerEnvReport();
                                class2.Header2DataHash.Add(str7, report2);
                                report = (PerEnvReport) class2.Header2DataHash[str7];
                            }
                            if (report.PerSiteData.Contains(key))
                            {
                                elements = (ReportElements) report.PerSiteData[key];
                            }
                            else
                            {
                                ReportElements elements2 = new ReportElements();
                                report.PerSiteData.Add(key, elements2);
                                elements = (ReportElements) report.PerSiteData[key];
                            }
                            input = reader.ReadLine();
                            string str9 = "N/A";
                            Hashtable hashtable2 = new Hashtable();
                            int num8 = 0;
                            if (input != null)
                            {
                                string[] strArray = input.Split(new char[] { '=' });
                                if (strArray.Length > 1)
                                {
                                    str9 = strArray[1];
                                    if (this.perRxSetupData.ContainsKey(str9))
                                    {
                                        goto Label_0595;
                                    }
                                    if (strArray.Length > 1)
                                    {
                                        hashtable2.Add(strArray[0], strArray[1]);
                                    }
                                    while ((input = reader.ReadLine()) != null)
                                    {
                                        if (input == "End Summary")
                                        {
                                            break;
                                        }
                                        strArray = input.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable2.Add(strArray[0], strArray[1]);
                                        }
                                        num8++;
                                    }
                                    this.perRxSetupData.Add(str9, hashtable2);
                                }
                            }
                            goto Label_0746;
                        Label_0579:
                            if (input == "End Summary")
                            {
                                input = reader.ReadLine();
                                goto Label_0746;
                            }
                        Label_0595:
                            if ((input = reader.ReadLine()) != null)
                            {
                                goto Label_0579;
                            }
                        Label_0746:
                            while (input != null)
                            {
                                if (input.Contains("Time,"))
                                {
                                    input = reader.ReadLine();
                                    input = reader.ReadLine();
                                }
                                else
                                {
                                    string[] strArray2 = input.Split(new char[] { ',' });
                                    double num9 = -9999.0;
                                    if (strArray2.Length >= 5)
                                    {
                                        double num10 = Convert.ToDouble(strArray2[this._ttffReportType]);
                                        double num11 = Convert.ToDouble(strArray2[index]);
                                        double num12 = Convert.ToDouble(strArray2[num3]);
                                        if (num12 != -9999.0)
                                        {
                                            num9 = Math.Abs((double) (num12 - Convert.ToDouble(strArray2[num4])));
                                        }
                                        elements.TTFFSamples.InsertSample(num10);
                                        elements.Position2DErrorSamples.InsertSample(num11);
                                        elements.VerticalErrorSamples.InsertSample(num9);
                                        report.PerEnvSamples.TTFFSamples.InsertSample(num10);
                                        report.PerEnvSamples.Position2DErrorSamples.InsertSample(num11);
                                        report.PerEnvSamples.VerticalErrorSamples.InsertSample(num9);
                                        class2.ReportDataSamples.TTFFSamples.InsertSample(num10);
                                        class2.ReportDataSamples.Position2DErrorSamples.InsertSample(num11);
                                        class2.ReportDataSamples.VerticalErrorSamples.InsertSample(num9);
                                        for (int i = 0; i < num6; i++)
                                        {
                                            sample = Convert.ToDouble(strArray2[num5 + i]);
                                            if (sample != 0.0)
                                            {
                                                elements.CNOSamples.InsertSample(sample);
                                                report.PerEnvSamples.CNOSamples.InsertSample(sample);
                                                class2.ReportDataSamples.CNOSamples.InsertSample(sample);
                                            }
                                        }
                                    }
                                    input = reader.ReadLine();
                                }
                            }
                            reader.Close();
                        }
                        f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        f.WriteLine("<e911>");
                        foreach (string str10 in hashtable.Keys)
                        {
                            Header1DataClass class4 = (Header1DataClass) hashtable[str10];
                            f.WriteLine("\t<swVersion name=\"{0}\">", str10);
                            foreach (string str11 in class4.Header2DataHash.Keys)
                            {
                                PerEnvReport report3 = (PerEnvReport) class4.Header2DataHash[str11];
                                f.WriteLine("\t\t<environment name=\"{0}\">", str11);
                                foreach (string str12 in report3.PerSiteData.Keys)
                                {
                                    ReportElements elements3 = (ReportElements) report3.PerSiteData[str12];
                                    f.WriteLine("\t\t\t<site number=\"{0}\">", str12);
                                    this.printE911Summary(f, elements3.TTFFSamples);
                                    this.printSampleData(f, elements3.TTFFSamples, this._percentile, this._ttffLimit, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                                    this.printSampleData(f, elements3.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                                    this.printSampleData(f, elements3.VerticalErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "Vertical Error", "m");
                                    this.printSampleData(f, elements3.CNOSamples, this._percentile, "", 0.0, "CNO", "dbHz");
                                    f.WriteLine("\t\t\t</site>");
                                }
                                f.WriteLine("\t\t\t<average>");
                                this.printE911Summary(f, report3.PerEnvSamples.TTFFSamples);
                                this.printSampleData(f, report3.PerEnvSamples.TTFFSamples, this._percentile, this._ttffLimit, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                                this.printSampleData(f, report3.PerEnvSamples.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                                this.printSampleData(f, report3.PerEnvSamples.VerticalErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "Vertical Error", "m");
                                this.printSampleData(f, report3.PerEnvSamples.CNOSamples, this._percentile, "", 0.0, "CNO", "dbHz");
                                f.WriteLine("\t\t\t</average>");
                                f.WriteLine("\t\t</environment>");
                            }
                            f.WriteLine("\t\t<summary>");
                            this.printE911Summary(f, class4.ReportDataSamples.TTFFSamples);
                            this.printSampleData(f, class4.ReportDataSamples.TTFFSamples, this._percentile, this._ttffLimit, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                            this.printSampleData(f, class4.ReportDataSamples.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                            this.printSampleData(f, class4.ReportDataSamples.VerticalErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "Vertical Error", "m");
                            this.printSampleData(f, class4.ReportDataSamples.CNOSamples, this._percentile, "", 0.0, "CNO", "dbHz");
                            f.WriteLine("\t\t</summary>");
                            f.WriteLine("\t</swVersion>");
                        }
                        this.printTestSetup(f);
                        f.WriteLine("</e911>");
                        f.Close();
                        hashtable.Clear();
                        this.perRxSetupData.Clear();
                        string outputFile = dir + @"\summary_e911.html";
                        this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\e911Report.xsl");
                    }
                }
            }
            catch (Exception exception)
            {
                this.perRxSetupData.Clear();
                hashtable.Clear();
                f.Close();
                MessageBox.Show(exception.Message, "ERROR");
            }
        }

        ~Report()
        {
            this.Dispose(true);
        }

        public void GenerateHTMLReport(string xmlFile, string outputFile, string transFile)
        {
            try
            {
                XPathDocument document = new XPathDocument(xmlFile);
                XslCompiledTransform transform = new XslCompiledTransform();
                XmlTextWriter writer = new XmlTextWriter(outputFile, null);
                transform.Load(transFile);
                transform.Transform((IXPathNavigable) document, null, (XmlWriter) writer);
                writer.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR!");
            }
        }

        private static int GetChannelIDIndex(uint cx)
        {
            int num = 0;
            uint num2 = cx;
            if (num2 <= 0xbb)
            {
                if (num2 <= 0x80)
                {
                    switch (num2)
                    {
                        case 1:
                            return 0;

                        case 2:
                            return 1;

                        case 3:
                            return 2;

                        case 0x80:
                            return 3;
                    }
                    return num;
                }
                switch (num2)
                {
                    case 0xb1:
                        return 9;

                    case 0xbb:
                        return 4;
                }
                return num;
            }
            if (num2 <= 0xdd)
            {
                switch (num2)
                {
                    case 0xcc:
                        return 5;

                    case 0xdd:
                        return 6;
                }
                return num;
            }
            switch (num2)
            {
                case 0xee:
                    return 7;

                case 0xff:
                    return 8;
            }
            return num;
        }

        private string getComparisonString(string inS, double d1, double d2, out bool isTrue)
        {
            switch (inS)
            {
                case "<":
                    if (d1 < d2)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                    }
                    return "<";

                case ">":
                    if (d1 > d2)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                    }
                    return ">";

                case "<=":
                    if (d1 <= d2)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                    }
                    return "<=";

                case ">=":
                    if (d1 >= d2)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                    }
                    return ">=";
            }
            isTrue = false;
            return "N/A";
        }

        private static bool IsGP2DateLine(string ln)
        {
            bool flag = false;
            if ((ln.IndexOf("(1)") <= 0) && (ln.IndexOf("(0)") <= 0))
            {
                return flag;
            }
            return true;
        }

        private static int MidMsgCountsIndexOf(uint md)
        {
            int num = -1;
            for (int i = 0; i < MsgCounts.Count; i++)
            {
                if (MsgCounts[i].mid == md)
                {
                    num = i;
                }
            }
            return num;
        }

        private static int MidSubIDMsgChCountsIndexOf(uint md, uint sb)
        {
            int num = -1;
            for (int i = 0; i < MsgChCounts.Count; i++)
            {
                if ((MsgChCounts[i].mid == md) && (MsgChCounts[i].subID == sb))
                {
                    num = i;
                }
            }
            return num;
        }

        private static int MidSubIDMsgCountsIndexOf(uint md, uint sb)
        {
            int num = -1;
            for (int i = 0; i < MsgCounts.Count; i++)
            {
                if ((MsgCounts[i].mid == md) && (MsgCounts[i].subID == sb))
                {
                    num = i;
                }
            }
            return num;
        }

        private void modifyResetReportCSS(string path, int styleCt)
        {
            string str = string.Empty;
            string str2 = " {\n\t position:absolute; \n\t visibility:hidden; \n\t border:solid 1px #CCC; \n\t  padding:5px; }";
            if (styleCt > 0)
            {
                for (int i = 1; i <= styleCt; i++)
                {
                    str = str + " #Style" + i.ToString() + ",";
                }
                str = str.Substring(0, str.Length - 1);
            }
            string str3 = str + str2;
            StreamWriter writer = new StreamWriter(path + @"\ResetReport.css");
            writer.Write(str3);
            writer.Close();
        }

        private void mpmReportGen(string dir)
        {
            string path = dir + @"\summary_MPMTest.xml";
            StreamWriter writer = new StreamWriter(path);
            try
            {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*mpmPar.csv");
                if (files.Length != 0)
                {
                    int num;
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<MPM>");
                    ArrayList list = new ArrayList();
                    foreach (FileInfo info2 in files)
                    {
                        StreamReader reader = info2.OpenText();
                        List<string> list2 = new List<string>();
                        for (string str2 = reader.ReadLine(); str2 != null; str2 = reader.ReadLine())
                        {
                            if (str2 != null)
                            {
                                if (str2.Contains("End Summary"))
                                {
                                    break;
                                }
                                string pattern = string.Empty;
                                if (str2.Contains("SW Version"))
                                {
                                    pattern = @"=\s*(?<Data>.*)";
                                }
                                else
                                {
                                    pattern = @"=\s*(?<Data>[+-]*\w+\.*\w*)";
                                }
                                Regex regex = new Regex(pattern, RegexOptions.Compiled);
                                if (regex.IsMatch(str2))
                                {
                                    list2.Add(regex.Match(str2).Result("${Data}"));
                                }
                            }
                        }
                        list.Add(list2);
                        reader.Close();
                    }
                    int num2 = 1;
                    writer.WriteLine("<data name=\"SW Version\">");
                    for (num = 0; num < list.Count; num++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num])[0], ((List<string>) list[num])[num2]);
                    }
                    writer.WriteLine("</data>");
                    num2++;
                    writer.WriteLine("<data name=\"Total SV Update\">");
                    for (num = 0; num < list.Count; num++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num])[0], ((List<string>) list[num])[num2]);
                    }
                    writer.WriteLine("</data>");
                    num2++;
                    writer.WriteLine("<data name=\"Total Eph Collection\">");
                    for (num = 0; num < list.Count; num++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num])[0], ((List<string>) list[num])[num2]);
                    }
                    writer.WriteLine("</data>");
                    num2++;
                    writer.WriteLine("<data name=\"Total Alm Collection\">");
                    for (num = 0; num < list.Count; num++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num])[0], ((List<string>) list[num])[num2]);
                    }
                    writer.WriteLine("</data>");
                    num2++;
                    writer.WriteLine("<data name=\"Total GPS Update\">");
                    for (num = 0; num < list.Count; num++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num])[0], ((List<string>) list[num])[num2]);
                    }
                    writer.WriteLine("</data>");
                    num2++;
                    writer.WriteLine("<data name=\"Total Recovery\">");
                    for (num = 0; num < list.Count; num++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num])[0], ((List<string>) list[num])[num2]);
                    }
                    writer.WriteLine("</data>");
                    num2++;
                    writer.WriteLine("<data name=\"Total Full Power SV Update\">");
                    for (num = 0; num < list.Count; num++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num])[0], ((List<string>) list[num])[num2]);
                    }
                    writer.WriteLine("</data>");
                    writer.WriteLine("</MPM>");
                    writer.Close();
                    string outputFile = dir + @"\summary_MPMTest.html";
                    this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\mpmReport.xsl");
                }
            }
            catch (Exception exception)
            {
                writer.Close();
                MessageBox.Show(exception.Message, "ERROR");
            }
        }

        private static int MsgChCountCompare(messageChCounts a, messageChCounts b)
        {
            int num = 0;
            if (a.mid > b.mid)
            {
                return 1;
            }
            if (a.mid < b.mid)
            {
                return -1;
            }
            if (a.mid == b.mid)
            {
                if (a.subID > b.subID)
                {
                    return 1;
                }
                if (a.subID < b.subID)
                {
                    num = -1;
                }
            }
            return num;
        }

        private static int MsgCountCompare(messageCounts a, messageCounts b)
        {
            int num = 0;
            if (a.mid > b.mid)
            {
                return 1;
            }
            if (a.mid < b.mid)
            {
                return -1;
            }
            if (a.mid == b.mid)
            {
                if (a.subID > b.subID)
                {
                    return 1;
                }
                if (a.subID < b.subID)
                {
                    num = -1;
                }
            }
            return num;
        }

        public static string NewProtocolGP2CoverageXMLFile(string filepath)
        {
            string str = "";
            AppLog(string.Format("Starting to process {0}", filepath));
            ReadOSPMasterListIntoMsgCounts();
            StreamReader reader = new StreamReader(filepath);
            try
            {
                uint sb = 0;
                for (string str2 = reader.ReadLine(); !reader.EndOfStream; str2 = reader.ReadLine())
                {
                    if (IsGP2DateLine(str2))
                    {
                        string[] strArray = str2.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            uint md = Convert.ToUInt32(strArray[5], 0x10);
                            int num3 = MidMsgCountsIndexOf(md);
                            if (num3 >= 0)
                            {
                                if (MsgCounts[num3].subIDFlag)
                                {
                                    sb = Convert.ToUInt32(strArray[6], 0x10);
                                    int num4 = MidSubIDMsgCountsIndexOf(md, sb);
                                    if (num4 >= 0)
                                    {
                                        messageCounts local1 = MsgCounts[num4];
                                        local1.count++;
                                        if ((md == 0) && (sb == 0))
                                        {
                                            AppLog("Found mid,subID=[0,0]");
                                        }
                                    }
                                }
                                else
                                {
                                    messageCounts local2 = MsgCounts[num3];
                                    local2.count++;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                str = CreatePrtclCoverageXMLFile(filepath);
                Console.WriteLine("All Done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            AppLog(string.Format("Completed creating: {0}", str));
            return str;
        }

        public static string NewProtocolGPSCoverageXMLFile(string filepath)
        {
            string str = "";
            AppLog(string.Format("Starting to process {0}", filepath));
            ReadOSPMasterListIntoMsgCounts();
            StreamReader reader = new StreamReader(filepath);
            try
            {
                uint sb = 0;
                string str2 = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    str2 = reader.ReadLine();
                    if (str2.IndexOf("(1)") <= 0)
                    {
                        if (str2.IndexOfAny(new char[] { ':', '=', '/' }) < 0)
                        {
                            string[] strArray = str2.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (strArray.Length >= 2)
                            {
                                try
                                {
                                    uint md = Convert.ToUInt32(strArray[0]);
                                    if ((strArray[0] == "255") || (strArray[1] == "MEI"))
                                    {
                                        continue;
                                    }
                                    int num3 = MidMsgCountsIndexOf(md);
                                    if (num3 < 0)
                                    {
                                        continue;
                                    }
                                    if (MsgCounts[num3].subIDFlag)
                                    {
                                        sb = Convert.ToUInt32(strArray[1]);
                                        int num4 = MidSubIDMsgCountsIndexOf(md, sb);
                                        if (num4 >= 0)
                                        {
                                            messageCounts local1 = MsgCounts[num4];
                                            local1.count++;
                                            if ((md == 0) && (sb == 0))
                                            {
                                                AppLog("Found mid,subID=[0,0]");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        messageCounts local2 = MsgCounts[num3];
                                        local2.count++;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] strArray2 = str2.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (strArray2.Length >= 2)
                        {
                            try
                            {
                                uint num5 = Convert.ToUInt32(strArray2[5], 0x10);
                                int num6 = MidMsgCountsIndexOf(num5);
                                if (num6 >= 0)
                                {
                                    if (MsgCounts[num6].subIDFlag)
                                    {
                                        sb = Convert.ToUInt32(strArray2[6], 0x10);
                                        int num7 = MidSubIDMsgCountsIndexOf(num5, sb);
                                        if (num7 >= 0)
                                        {
                                            messageCounts local3 = MsgCounts[num7];
                                            local3.count++;
                                            if ((num5 == 0) && (sb == 0))
                                            {
                                                AppLog("Found mid,subID=[0,0]");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        messageCounts local4 = MsgCounts[num6];
                                        local4.count++;
                                    }
                                }
                                continue;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
                str = CreatePrtclCoverageXMLFile(filepath);
                Console.WriteLine("All Done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            AppLog(string.Format("Completed creating: {0}", str));
            return str;
        }

        public void PointToPointErrorCalculation(string dataFile, string refFile, double startPoint, double endPoint)
        {
            ArrayList list = this._helperFunctions.ReadIMUFile(dataFile, startPoint, endPoint);
            ArrayList list2 = this._helperFunctions.ReadIMUFile(refFile, startPoint, endPoint);
            Stats stats = new Stats();
            Stats stats2 = new Stats();
            Stats stats3 = new Stats();
            Stats stats4 = new Stats();
            Stats stats5 = new Stats();
            Stats stats6 = new Stats();
            Stats stats7 = new Stats();
            Stats stats8 = new Stats();
            PositionErrorCalc calc = new PositionErrorCalc();
            List<string> list3 = new List<string>();
            string item = string.Empty;
            List<double> positionDataList = new List<double>(8);
            for (int i = 0; i < 8; i++)
            {
                positionDataList.Add(0.0);
            }
            PositionInfo.PositionStruct struct2 = new PositionInfo.PositionStruct();
            PositionInfo.PositionStruct struct3 = new PositionInfo.PositionStruct();
            int num2 = 0;
            int num3 = 0;
            double tOW = 0.0;
            double num5 = 0.0;
            double sample = 0.0;
            double num7 = 0.0;
            double num8 = 0.0;
            double num9 = 0.0;
            double num10 = 0.0;
            double num11 = 0.0;
            double num12 = 0.0;
            int num13 = 0;
            int num14 = 0;
            int num15 = 0;
            int num16 = 0;
            int num17 = 0;
            int num18 = 0;
            int num19 = 0;
            int num20 = 0;
            int num21 = 0;
            string str2 = "Not detected";
            if ((list.Count > 0) && (list2.Count > 0))
            {
                for (num2 = 0; (num2 < list.Count) && (num3 < list2.Count); num2++)
                {
                    struct2 = (PositionInfo.PositionStruct) list[num2];
                    struct3 = (PositionInfo.PositionStruct) list2[num3];
                    if (struct2.SW_Version != string.Empty)
                    {
                        str2 = struct2.SW_Version.TrimStart(new char[] { ':' });
                    }
                    if (struct2.NavType > 0)
                    {
                        tOW = struct2.TOW;
                        num5 = struct3.TOW;
                        while (tOW > num5)
                        {
                            num3++;
                            if (num3 >= list2.Count)
                            {
                                break;
                            }
                            struct3 = (PositionInfo.PositionStruct) list2[num3];
                            num5 = struct3.TOW;
                        }
                        if (num8 == 0.0)
                        {
                            num8 = tOW;
                        }
                        num9 = tOW;
                        if (tOW == num5)
                        {
                            positionDataList[0] = struct2.Latitude;
                            positionDataList[1] = struct2.Longitude;
                            positionDataList[2] = struct2.Altitude;
                            positionDataList[3] = struct2.Heading;
                            positionDataList[4] = struct3.Latitude;
                            positionDataList[5] = struct3.Longitude;
                            positionDataList[6] = struct3.Altitude;
                            positionDataList[7] = struct3.Heading;
                            calc.GetPositionErrorsInMeter(positionDataList);
                            sample = calc.HorizontalError;
                            stats.InsertSample(sample);
                            num7 = Math.Abs(calc.VerticalErrorInMeter);
                            stats2.InsertSample(num7);
                            num11 = Math.Abs(calc.AlongTrackErrorInMeter);
                            stats7.InsertSample(num11);
                            num12 = Math.Abs(calc.XTrackErrorInMeter);
                            stats8.InsertSample(num12);
                            num10 = Math.Abs((double) (struct2.Heading - struct3.Heading));
                            if (num10 > 180.0)
                            {
                                num10 = 360.0 - num10;
                            }
                            stats6.InsertSample(num10);
                            stats3.InsertSample(struct2.HDOP);
                            stats4.InsertSample((double) struct2.NumSVInFix);
                            stats5.InsertSample(struct2.MaxCN0);
                            if (sample > 50.0)
                            {
                                num13++;
                                num19++;
                                if (num13 == 4)
                                {
                                    num16++;
                                }
                            }
                            else
                            {
                                num13 = 0;
                            }
                            if (sample > 25.0)
                            {
                                num14++;
                                num20++;
                                if (num14 == 4)
                                {
                                    num17++;
                                }
                            }
                            else
                            {
                                num14 = 0;
                            }
                            if (sample > 10.0)
                            {
                                num15++;
                                num21++;
                                if (num15 == 4)
                                {
                                    num18++;
                                }
                            }
                            else
                            {
                                num15 = 0;
                            }
                            item = string.Format("{0},{1}, {2:F6},{3:F6},{4:F6},{5:F6},{6:F6},{7:F6},{8:F6}, {9:F6}, {10:F2}, {11:F2}, {12:F2}, {13:F2},{14:F2},{15:F2},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25}, {26},{27}", new object[] { 
                                tOW, num5, struct2.Latitude, struct2.Longitude, struct2.Altitude, struct3.Latitude, struct3.Longitude, struct3.Altitude, struct2.Heading, struct3.Heading, sample, num7, num12, num11, struct2.HDOP, struct2.NumSVInFix, 
                                struct2.MaxCN0, struct2.NavType, num19, num16, num20, num17, num21, num18, num13, num14, num15, num10
                             });
                            list3.Add(item);
                            num3++;
                        }
                    }
                }
                if (stats.Samples > 0)
                {
                    double percentile = stats.GetPercentile((double) 50.0, -9999.0);
                    double num23 = stats.GetPercentile((double) 68.0, -9999.0);
                    double num24 = stats.GetPercentile((double) 95.0, -9999.0);
                    double num25 = stats.Stats_Max((double) -9999.0);
                    stats.Stats_Min((double) -9999.0);
                    stats.Stats_Mean((double) -9999.0);
                    double num26 = stats2.GetPercentile((double) 50.0, -9999.0);
                    double num27 = stats2.GetPercentile((double) 68.0, -9999.0);
                    double num28 = stats2.GetPercentile((double) 95.0, -9999.0);
                    double num29 = stats2.Stats_Max((double) -9999.0);
                    stats2.Stats_Min((double) -9999.0);
                    stats2.Stats_Mean((double) -9999.0);
                    double num30 = stats8.GetPercentile((double) 50.0, -9999.0);
                    double num31 = stats8.GetPercentile((double) 68.0, -9999.0);
                    double num32 = stats8.GetPercentile((double) 95.0, -9999.0);
                    double num33 = stats8.Stats_Max((double) -9999.0);
                    stats8.Stats_Min((double) -9999.0);
                    stats8.Stats_Mean((double) -9999.0);
                    double num34 = stats7.GetPercentile((double) 50.0, -9999.0);
                    double num35 = stats7.GetPercentile((double) 68.0, -9999.0);
                    double num36 = stats7.GetPercentile((double) 95.0, -9999.0);
                    double num37 = stats7.Stats_Max((double) -9999.0);
                    stats7.Stats_Min((double) -9999.0);
                    stats7.Stats_Mean((double) -9999.0);
                    int samples = stats.Samples;
                    double num39 = stats3.Stats_Mean((double) -9999.0);
                    double num40 = stats4.Stats_Mean((double) -9999.0);
                    double num41 = (((double) samples) / ((num9 - num8) + 1.0)) * 100.0;
                    double num42 = stats5.Stats_Mean((double) -9999.0);
                    double num43 = stats6.Stats_Max((double) -9999.0);
                    stats6.Stats_Min((double) -9999.0);
                    stats6.Stats_Mean((double) -9999.0);
                    double num44 = stats6.GetPercentile((double) 50.0, -9999.0);
                    double num45 = stats6.GetPercentile((double) 68.0, -9999.0);
                    double num46 = stats6.GetPercentile((double) 95.0, -9999.0);
                    string path = dataFile.Replace(".gps", "_PointToPointAccuracy.csv");
                    FileInfo info = new FileInfo(dataFile);
                    StreamWriter writer = new StreamWriter(path);
                    FileInfo info2 = new FileInfo(refFile);
                    writer.WriteLine("IMU file path= {0}", info2.Name);
                    writer.WriteLine("Data file path= {0}", info.Name);
                    writer.WriteLine("SW Version= {0}", str2);
                    writer.WriteLine("MaxCN0 Ave|dBHz= {0:F1}", num42);
                    writer.WriteLine("Samples= {0:}", samples);
                    writer.WriteLine("Tracking Availability= {0:F1}%", num41);
                    writer.WriteLine("HDOP Ave= {0:F1}", num39);
                    writer.WriteLine("Satellite Number Ave= {0:F1}", num40);
                    writer.WriteLine("Horizontal Error 50% CEP|m= {0:F1}", percentile);
                    writer.WriteLine("Horizontal Error 68% CEP|m= {0:F1}", num23);
                    writer.WriteLine("Horizontal Error 95% CEP|m= {0:F1}", num24);
                    writer.WriteLine("Horizontal Error Max|m= {0:F1}", num25);
                    writer.WriteLine("Altitude Error 50% CEP|m= {0:F1}", num26);
                    writer.WriteLine("Altitude Error 68% CEP|m= {0:F1}", num27);
                    writer.WriteLine("Altitude Error 95% CEP|m= {0:F1}", num28);
                    writer.WriteLine("Altitude Error Max|m= {0:F1}", num29);
                    writer.WriteLine("Cross Track Error 50% CEP|m= {0:F1}", num30);
                    writer.WriteLine("Cross Track Error 68% CEP|m= {0:F1}", num31);
                    writer.WriteLine("Cross Track Error 95% CEP|m= {0:F1}", num32);
                    writer.WriteLine("Cross Track Error Max|m= {0:F1}", num33);
                    writer.WriteLine("Along Track Error 50% CEP|m= {0:F1}", num34);
                    writer.WriteLine("Along Track Error 68% CEP|m= {0:F1}", num35);
                    writer.WriteLine("Along Track Error 95% CEP|m= {0:F1}", num36);
                    writer.WriteLine("Along Track Error Max|m= {0:F1}", num37);
                    writer.WriteLine("Exceed50mCount= {0}", num19);
                    writer.WriteLine("Exceed50m3sNotPullInCount= {0}", num16);
                    writer.WriteLine("Exceed25mCount= {0}", num20);
                    writer.WriteLine("Exceed25m3sNotPullInCount= {0}", num17);
                    writer.WriteLine("Exceed10mCount= {0}", num21);
                    writer.WriteLine("Exceed10m3sNotPullInCount= {0}", num18);
                    writer.WriteLine("Heading Error 50% CEP|m= {0:F1}", num44);
                    writer.WriteLine("Heading Error 68% CEP|m= {0:F1}", num45);
                    writer.WriteLine("Heading Error 95% CEP|m= {0:F1}", num46);
                    writer.WriteLine("Heading Error Max|m= {0:F1}", num43);
                    writer.WriteLine("End Summary");
                    writer.WriteLine("Tow,RefTow,Latitude,Longitude,Altitude,RefLatitude,RefLongitude,RefAltitude,Heading,RefHeading,HzError,AltError,CrossTrackError,AlongTrackError,HDOP,NumSVInFix,MaxCN0,NavType,Exceed50mCount,Exceed50m3sNotPullInCount,Exceed25mCount,Exceed25m3sNotPullInCount,Exceed10mCount,Exceed10m3sNotPullInCount,PullInTime50m,PullInTime25m,ullInTime10m,headingError");
                    foreach (string str4 in list3)
                    {
                        writer.WriteLine(str4);
                    }
                    writer.Close();
                }
                list.Clear();
                list2.Clear();
            }
            stats.Dispose();
            stats = null;
            stats2.Dispose();
            stats2 = null;
            stats3.Dispose();
            stats3 = null;
            stats4.Dispose();
            stats4 = null;
            stats5.Dispose();
            stats5 = null;
            stats8.Dispose();
            stats8 = null;
            stats7.Dispose();
            stats7 = null;
            stats6.Dispose();
            stats6 = null;
            calc = null;
            list3.Clear();
            list3 = null;
        }

        private bool prinSDOHeader(StreamWriter s, Hashtable dH)
        {
            if (s == null)
            {
                return false;
            }
            string path = clsGlobal.InstalledDirectory + @"\Config\sdoHeaderName.cfg";
            if (!File.Exists(path))
            {
                path = null;
                return false;
            }
            IniHelper helper = new IniHelper(path);
            List<string> keys = helper.GetKeys("General");
            List<string> list2 = helper.GetKeys("TestStationInfo");
            List<string> list3 = helper.GetKeys("TestParameters");
            if (dH == null)
            {
                string str2 = "N/A";
                if (keys != null)
                {
                    foreach (string str3 in keys)
                    {
                        s.WriteLine(string.Format("{0}:{1}", str3, str2));
                    }
                }
                if (list2 != null)
                {
                    s.WriteLine("Test Station Info:");
                    foreach (string str4 in list2)
                    {
                        s.WriteLine(string.Format("{0}:{1}", str4, str2));
                    }
                }
                if (list3 != null)
                {
                    s.WriteLine("Test Parameters:");
                    foreach (string str5 in list3)
                    {
                        s.WriteLine(string.Format("{0}:{1}", str5, str2));
                    }
                }
            }
            else
            {
                string str6 = string.Empty;
                if (keys != null)
                {
                    foreach (string str7 in keys)
                    {
                        if (dH.ContainsKey(str7))
                        {
                            str6 = (string) dH[str7];
                            s.WriteLine(string.Format("{0}:{1}", str7, str6));
                        }
                    }
                }
                if (list2 != null)
                {
                    s.WriteLine("Test Station Info:");
                    foreach (string str8 in list2)
                    {
                        if (dH.ContainsKey(str8))
                        {
                            str6 = (string) dH[str8];
                            if (str8.Contains("SW Version"))
                            {
                                s.WriteLine(string.Format("\tDUT {0}:{1}", str8, str6));
                            }
                            else
                            {
                                s.WriteLine(string.Format("\t{0}:{1}", str8, str6));
                            }
                        }
                    }
                }
                if (list3 != null)
                {
                    s.WriteLine("Test Parameters:");
                    foreach (string str9 in list3)
                    {
                        if (dH.ContainsKey(str9))
                        {
                            str6 = (string) dH[str9];
                            s.WriteLine(string.Format("\t{0}:{1}", str9, str6));
                        }
                    }
                }
            }
            path = null;
            if (keys != null)
            {
                keys.Clear();
                keys = null;
            }
            if (list2 != null)
            {
                list2.Clear();
                list2 = null;
            }
            if (list3 != null)
            {
                list3.Clear();
                list3 = null;
            }
            return true;
        }

        private void print3GPPSummary(StreamWriter f, int checkSamples, int totalSamples, int numberMisses, int numTTFFMisses, int numTTFFOOO, int num2DMisses, int num2DOOO)
        {
            int num = 0;
            int[] numArray = new int[] { 
                0x4d, 0x6a, 0x83, 0x9a, 0xb0, 0xc5, 0xda, 0xee, 0x101, 0x115, 0x127, 0x13a, 0x14d, 0x15f, 0x171, 0x183, 
                0x195, 0x1a6, 440, 0x1c9, 0x1da, 0x1ec, 0x1fd, 0x20e, 0x21f, 560, 0x241, 0x251, 610, 0x273, 0x283, 660, 
                0x2a4, 0x2b5, 0x2c5, 0x2d5, 0x2e6, 0x2f6, 0x306, 790, 0x327, 0x337, 0x347, 0x357, 0x367, 0x377, 0x387, 0x397, 
                0x3a7, 0x3b7, 0x3c7, 0x3d6, 0x3e6, 0x3f6, 0x406, 0x416, 0x425, 0x435, 0x454, 0x464, 0x474, 0x483, 0x493, 0x4a2, 
                0x4b2, 0x4c1, 0x4d1, 0x4e0, 0x4f0, 0x4ff, 0x50f, 0x51e, 0x52e, 0x53d, 0x54d, 0x55c, 0x56b, 0x57b, 0x58a, 0x599, 
                0x5a9, 0x5b8, 0x5c7, 0x5d7, 0x5e6, 0x5f5, 0x604, 0x614, 0x623, 0x632, 0x641, 0x651, 0x660, 0x66f, 0x67e, 0x68d, 
                0x2dac, 0x6ac, 0x6bb, 0x6ca, 0x6d9, 0x6e8, 0x6f7, 0x706, 0x715, 0x724, 0x734, 0x743, 0x752, 0x761, 0x770, 0x77f, 
                0x78e, 0x79d, 0x7ac, 0x7bb, 0x7ca, 0x7d9, 0x7e8, 0x7f7, 0x806, 0x815, 0x824, 0x833, 0x842, 0x850, 0x85f, 0x86e, 
                0x87d, 0x88c, 0x89b, 0x8aa, 0x8b9, 0x8c8, 0x8d7, 0x8e5, 0x8f4, 0x903, 0x912, 0x921, 0x930, 0x93f, 0x94d, 0x95c, 
                0x96b, 0x97a, 0x989, 0x998, 0x9a6, 0x9b5, 0x9c4, 0x9d3, 0x9e2, 0x9f0, 0x9ff, 0xa0e, 0xa1d, 0xa2b, 0xa3a, 0xa49, 
                0xa58, 0xa66, 0xa75, 0xa84, 0xa8e, 0xaa1, 0xab0, 0xabf, 0xacd
             };
            string str = string.Empty;
            if (f != null)
            {
                if (numberMisses >= numArray.Length)
                {
                    str = "Fail";
                }
                else
                {
                    num = numArray[numberMisses];
                    if (numberMisses == 0)
                    {
                        if (totalSamples < num)
                        {
                            str = "Fail";
                        }
                        else if ((checkSamples > 0) && (totalSamples < checkSamples))
                        {
                            str = "Fail";
                        }
                        else
                        {
                            str = "Pass";
                        }
                    }
                    else if (totalSamples >= numArray[numberMisses])
                    {
                        if ((checkSamples > 0) && (totalSamples < checkSamples))
                        {
                            str = "Fail";
                        }
                        else
                        {
                            str = "Pass";
                        }
                    }
                    else
                    {
                        str = "Fail";
                    }
                }
                f.WriteLine("\t\t\t\t<field name=\"Pass/Fail\" value=\"{0}\" criteria=\"Pass/Fail\" direction=\"\" units=\"\"/>", str);
                if (checkSamples > 0)
                {
                    f.WriteLine("\t\t\t\t<field name=\"Samples\" value=\"{0}\" criteria=\"{1}\" direction=\"&gt;\" units=\"\"/>", totalSamples, checkSamples);
                }
                else
                {
                    f.WriteLine("\t\t\t\t<field name=\"Samples\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", totalSamples);
                }
                f.WriteLine("\t\t\t\t<field name=\"Misses\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", numberMisses);
                f.WriteLine("\t\t\t\t<field name=\"# Miss TTFF\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", numTTFFMisses);
                f.WriteLine("\t\t\t\t<field name=\"# Over Limit TTFF\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", numTTFFOOO);
                f.WriteLine("\t\t\t\t<field name=\"# Miss Position\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num2DMisses);
                f.WriteLine("\t\t\t\t<field name=\"# Over Limit Position\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num2DOOO);
            }
        }

        private void printCDFHelper(StreamWriter f, Stats s, string name)
        {
            f.WriteLine("\t\t<test name=\"{0}\">\r\n", name);
            double percentile = s.GetPercentile((double) 50.0, 0.0);
            double num2 = s.GetPercentile((double) 95.0, 0.0);
            double num3 = s.GetPercentile((double) 100.0, 0.0);
            int samplesExcludingThese = s.GetSamplesExcludingThese(0.0);
            f.WriteLine("\t\t\t<field name=\"Num Samples\" value=\"{0}\" criteria=\"1\" direction=\"&gt;\" />\r\n", samplesExcludingThese);
            f.WriteLine("\t\t\t<field name=\"{0} (50%)\" value=\"{1}\" criteria=\"10\" direction=\"&lt;\" />\r\n", name, percentile);
            f.WriteLine("\t\t\t<field name=\"{0} (95%)\" value=\"{1}\" criteria=\"10\" direction=\"&lt;\" />\r\n", name, num2);
            f.WriteLine("\t\t\t<field name=\"{0} (MAX)\" value=\"{1}\" criteria=\"10\" direction=\"&lt;\" />\r\n", name, num3);
            f.WriteLine("\t\t</test>\r\n");
        }

        private void printE911Summary(StreamWriter f, Stats s)
        {
            double num = 0.0;
            if (f != null)
            {
                double num2 = Convert.ToDouble(this._limitVal);
                int samples = s.Samples;
                int inBoundCount = s.GetInBoundCount(num2);
                int outBoundCount = s.GetOutBoundCount(Convert.ToDouble(this._timeoutVal));
                int num6 = s.GetOutBoundCount(num2);
                if (samples > 0)
                {
                    num = (((double) inBoundCount) / ((double) samples)) * 100.0;
                }
                f.WriteLine("\t\t\t\t<field name=\"Yield\" value=\"{0:F2}\" criteria=\"99\" direction=\"&gt;\" units=\"%\"/>", num);
                f.WriteLine("\t\t\t\t<field name=\"Samples\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", samples);
                f.WriteLine("\t\t\t\t<field name=\"TTFF &gt;{0:F2}s\" value=\"{1}\" criteria=\"\" direction=\"&lt; TBD\" units=\"sec\"/>", num2, num6);
                f.WriteLine("\t\t\t\t<field name=\"TTFF Timeout &gt;{0:F2}s\" value=\"{1}\" criteria=\"\" direction=\"&lt; TBD\" units=\"sec\"/>", this._timeoutVal, outBoundCount);
            }
        }

        private void printImageList(StreamWriter f, List<string> il)
        {
            f.WriteLine("\t<imageList>");
            foreach (string str in il)
            {
                string str2 = string.Format("\t\t\t<image name=\"{0}\" />", str);
                f.WriteLine(str2);
            }
            f.WriteLine("\t</imageList>");
        }

        private void printResetSummary(StreamWriter f, int totalSamples, int numberMisses, int numSVInFix)
        {
            string str = string.Empty;
            double num = (((double) (totalSamples - numberMisses)) / ((double) totalSamples)) * 100.0;
            if (f != null)
            {
                if (numberMisses == 0)
                {
                    str = "Pass";
                }
                else
                {
                    str = "Fail";
                }
                f.WriteLine("\t\t\t\t<field name=\"Pass/Fail\" value=\"{0}\" criteria=\"Pass/Fail\" direction=\"\" units=\"\"/>", str);
                f.WriteLine("\t\t\t\t<field name=\"Samples\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", totalSamples);
                f.WriteLine("\t\t\t\t<field name=\"Yield %\" value=\"{0:F2}\" criteria=\"\" direction=\"\" units=\"\"/>", num);
                if (numSVInFix >= 0)
                {
                    f.WriteLine("\t\t\t\t<field name=\"3SVs Fix Count\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", numSVInFix);
                }
            }
        }

        private void printResultStats(StreamWriter s, Hashtable dH)
        {
            if (((s != null) && (dH != null)) && (s != null))
            {
                string path = clsGlobal.InstalledDirectory + @"\Config\sdoHeaderName.cfg";
                if (!File.Exists(path))
                {
                    path = null;
                }
                else
                {
                    IniHelper helper = new IniHelper(path);
                    List<string> keys = helper.GetKeys("Stats");
                    s.WriteLine("Test Statistics:");
                    SDOStatsElememt elememt = new SDOStatsElememt();
                    if (keys != null)
                    {
                        foreach (string str2 in keys)
                        {
                            elememt.Init();
                            if (dH.ContainsKey(str2))
                            {
                                elememt = (SDOStatsElememt) dH[str2];
                                if (elememt.Target != "N/A")
                                {
                                    s.WriteLine(string.Format("\t{0}:", str2));
                                    s.WriteLine(string.Format("\t\t- Target:{0}", elememt.Target));
                                    s.WriteLine(string.Format("\t\t- Result:{0}", elememt.Value));
                                    s.WriteLine(string.Format("\t\t- Comparison:{0}", elememt.Comparison));
                                    s.WriteLine(string.Format("\t\t- Comparison Result:{0}", elememt.ComparisonResult));
                                }
                                else
                                {
                                    s.WriteLine(string.Format("\t{0}:{1}", str2, elememt.Value));
                                }
                            }
                        }
                    }
                    elememt = null;
                    keys.Clear();
                    keys = null;
                    helper = null;
                }
            }
        }

        private void printSampleData(StreamWriter f, Stats s, string percentile, string limitStr, double exclude, string reportType, string unit)
        {
            if (f != null)
            {
                int index = 0;
                string[] strArray = percentile.Split(new char[] { ',' });
                string[] strArray2 = limitStr.Split(new char[] { ',' });
                this.percentileList = new List<double>();
                foreach (string str in strArray)
                {
                    double p = Convert.ToDouble(str);
                    double item = s.GetPercentile(p, exclude);
                    if (strArray2.Length >= strArray.Length)
                    {
                        this.percentileList.Add(item);
                        f.WriteLine("\t\t\t\t<field name=\"{0:F1}% {1}\" value=\"{2:F2}\" criteria=\"{3:F2}\" direction=\"&lt;\" units=\"{4}\"/>", new object[] { p, reportType, item, strArray2[index], unit });
                    }
                    else
                    {
                        this.percentileList.Add(item);
                        f.WriteLine("\t\t\t\t<field name=\"{0:F1}% {1}\" value=\"{2:F2}\" criteria=\"\" direction=\"\" units=\"{3}\"/>", new object[] { p, reportType, item, unit });
                    }
                    index++;
                }
                double num4 = s.Stats_Max(exclude);
                f.WriteLine("\t\t\t\t<field name=\"Max {0}\" value=\"{1:F2}\" criteria=\"\" direction=\"\" units=\"{2}\"/>", reportType, num4, unit);
            }
        }

        private void printTestSetup(StreamWriter f)
        {
            f.WriteLine("\t<testSetup>");
            foreach (string str in this.perRxSetupData.Keys)
            {
                Hashtable hashtable = (Hashtable) this.perRxSetupData[str];
                if (hashtable.ContainsKey("Rx"))
                {
                    f.WriteLine("\t\t<rx name=\"{0}\">", (string) hashtable["Rx"]);
                }
                else
                {
                    f.WriteLine("\t\t<rx name=\"{0}\">", "Unknown");
                }
                if (hashtable.ContainsKey("Test"))
                {
                    f.WriteLine("\t\t\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", "Test", (string) hashtable["Test"]);
                }
                foreach (string str2 in hashtable.Keys)
                {
                    if (!(str2 == "Rx") && !(str2 == "Test"))
                    {
                        f.WriteLine("\t\t\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", str2, (string) hashtable[str2]);
                    }
                }
                f.WriteLine("\t\t</rx>");
            }
            f.WriteLine("\t</testSetup>");
        }

        private void printTestSummary(StreamWriter f)
        {
            f.WriteLine("\t<testSummary>");
            foreach (string str in this._perRxSummary.Keys)
            {
                Hashtable hashtable = (Hashtable) this._perRxSummary[str];
                f.WriteLine("\t\t<rx name=\"{0}\">", str);
                ArrayList list = new ArrayList();
                foreach (string str2 in hashtable.Keys)
                {
                    list.Add(str2);
                }
                list.Sort();
                foreach (string str3 in list)
                {
                    f.WriteLine("\t\t\t<field name=\"{0}\" value=\"{1:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", str3, (double) hashtable[str3]);
                }
                f.WriteLine("\t\t</rx>");
            }
            f.WriteLine("\t</testSummary>");
        }

        private void printTIA916Summary(StreamWriter f, Stats posStats, Stats ttffStats, List<int> data)
        {
            int[] numArray = new int[] { 60, 0x19, 50, 60, 0x23 };
            int[] numArray2 = new int[] { 180, 0x4b, 150, 180, 0x69 };
            string str = string.Empty;
            int num = 0;
            int index = data[num];
            int num3 = data[num++];
            int num4 = data[num++];
            int num5 = data[num++];
            int num6 = data[num++];
            int num7 = data[num++];
            int num8 = data[num];
            double ascendingValueByIndex = 0.0;
            double num10 = 0.0;
            double num11 = 0.0;
            double num12 = 0.0;
            bool flag = true;
            if (f != null)
            {
                int samples = posStats.Samples;
                ascendingValueByIndex = posStats.GetAscendingValueByIndex(0x4a, -9999.0);
                num10 = posStats.GetAscendingValueByIndex(0x62, -9999.0);
                if ((ascendingValueByIndex < 0.0) || (num10 < 0.0))
                {
                    flag = false;
                }
                else if ((ascendingValueByIndex > numArray[index]) || (num10 > numArray2[index]))
                {
                    flag = false;
                }
                double excludedVal = 31.0;
                double num15 = 16.0;
                try
                {
                    excludedVal = Convert.ToDouble(this.TimeoutVal);
                    num15 = Convert.ToDouble(this._limitVal);
                }
                catch
                {
                }
                num11 = ttffStats.GetAscendingValueByIndex(0x4a, excludedVal);
                num12 = ttffStats.GetAscendingValueByIndex(0x62, excludedVal);
                if ((num11 < 0.0) || (num12 < 0.0))
                {
                    flag = false;
                }
                else if ((num11 > num15) || (num12 > num15))
                {
                    flag = false;
                }
                double num16 = (((double) (samples - num4)) / ((double) samples)) * 100.0;
                double num17 = 95.0;
                if (flag && (num16 > num17))
                {
                    str = "Pass";
                }
                else
                {
                    str = "Fail";
                }
                f.WriteLine("\t\t\t\t<field name=\"Pass/Fail\" value=\"{0}\" criteria=\"Pass/Fail\" direction=\"\" units=\"\"/>", str);
                f.WriteLine("\t\t\t\t<field name=\"Yield %\" value=\"{0:F2}\" criteria=\"{1:F2}\" direction=\"&gt;=\" units=\"\"/>", num16, num17);
                f.WriteLine("\t\t\t\t<field name=\"Samples\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", samples);
                f.WriteLine("\t\t\t\t<field name=\"Misses\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num4);
                f.WriteLine("\t\t\t\t<field name=\"# Miss TTFF\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num5);
                f.WriteLine("\t\t\t\t<field name=\"# Over Limit TTFF\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num6);
                f.WriteLine("\t\t\t\t<field name=\"# Miss Position\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num7);
                f.WriteLine("\t\t\t\t<field name=\"# Over Limit Position\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num8);
                f.WriteLine("\t\t\t\t<field name=\"75th TTFF\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", (num11 > 0.0) ? num11 : 9999.0, this._limitVal);
                f.WriteLine("\t\t\t\t<field name=\"99th TTFF\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", (num12 > 0.0) ? num12 : 9999.0, this._limitVal);
                f.WriteLine("\t\t\t\t<field name=\"75th 2-D\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", (ascendingValueByIndex > 0.0) ? ascendingValueByIndex : 9999.0, numArray[index]);
                f.WriteLine("\t\t\t\t<field name=\"99th 2-D\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", (num10 > 0.0) ? num10 : 9999.0, numArray2[index]);
                if (num3 >= 0)
                {
                    f.WriteLine("\t\t\t\t<field name=\"3SVs Fix Count\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num3);
                }
            }
        }

        public static void ProtocolChannelCoverageReportHTMLGenerator(string filename)
        {
            try
            {
                string str = ConfigurationManager.AppSettings["InstalledDirectory"];
                string str2 = str + @"\Log\ProtocolCoverageTest.html";
                string uri = str + @"\Log\PrtocolGP2Coverage.xml";
                string stylesheetUri = str + @"\scripts\ProtocolCoverageReport.xsl";
                XPathDocument document = new XPathDocument(uri);
                XslCompiledTransform transform = new XslCompiledTransform();
                XmlTextWriter writer = new XmlTextWriter(str2, null);
                transform.Load(stylesheetUri);
                transform.Transform((IXPathNavigable) document, null, (XmlWriter) writer);
                writer.Close();
                Console.WriteLine("All Done");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR!");
            }
        }

        public static void ProtocolCoverageReportHTMLGenerator(string filename)
        {
            try
            {
                string str = ConfigurationManager.AppSettings["InstalledDirectory"];
                string str2 = str + @"\Log\ProtocolCoverageTest.html";
                string uri = str + @"\Log\PrtocolGP2Coverage.xml";
                string stylesheetUri = str + @"\scripts\ProtocolCoverageReport.xsl";
                XPathDocument document = new XPathDocument(uri);
                XslCompiledTransform transform = new XslCompiledTransform();
                XmlTextWriter writer = new XmlTextWriter(str2, null);
                transform.Load(stylesheetUri);
                transform.Transform((IXPathNavigable) document, null, (XmlWriter) writer);
                writer.Close();
                Console.WriteLine("All Done");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR!");
            }
        }

        public static void ProtocolGP2ChannelCoverageFile(string filename)
        {
            ReadOSPMasterListIntoMsgChCounts();
            string str = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamReader reader = new StreamReader(str + @"\Log\" + filename);
            try
            {
                for (string str3 = reader.ReadLine(); !reader.EndOfStream; str3 = reader.ReadLine())
                {
                    if (IsGP2DateLine(str3))
                    {
                        string[] strArray = str3.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            uint cx = Convert.ToUInt32(strArray[6], 0x10);
                            uint md = Convert.ToUInt32(strArray[7], 0x10);
                            uint sb = Convert.ToUInt32(strArray[8], 0x10);
                            int num4 = MidSubIDMsgChCountsIndexOf(md, sb);
                            if (num4 >= 0)
                            {
                                int channelIDIndex = GetChannelIDIndex(cx);
                                MsgChCounts[num4].channelIDCount[channelIDIndex]++;
                            }
                            else
                            {
                                messageChCounts item = new messageChCounts(cx, md, sb);
                                MsgChCounts.Add(item);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                CreatePrtclGP2ChannelCoverageXMLFile();
                Console.WriteLine("All Done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        public static void ProtocolGP2CoverageFile(string filename)
        {
            ReadOSPMasterListIntoMsgCounts();
            string str = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamReader reader = new StreamReader(str + @"\Log\" + filename);
            try
            {
                uint sb = 0;
                for (string str3 = reader.ReadLine(); !reader.EndOfStream; str3 = reader.ReadLine())
                {
                    if (IsGP2DateLine(str3))
                    {
                        string[] strArray = str3.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            uint md = Convert.ToUInt32(strArray[5], 0x10);
                            int num3 = MidMsgCountsIndexOf(md);
                            if (num3 >= 0)
                            {
                                if (MsgCounts[num3].subIDFlag)
                                {
                                    sb = Convert.ToUInt32(strArray[6], 0x10);
                                    int num4 = MidSubIDMsgCountsIndexOf(md, sb);
                                    if (num4 >= 0)
                                    {
                                        messageCounts local1 = MsgCounts[num4];
                                        local1.count++;
                                    }
                                }
                                else
                                {
                                    messageCounts local2 = MsgCounts[num3];
                                    local2.count++;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                CreatePrtclGP2CoverageXMLFile();
                Console.WriteLine("All Done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        public static void ProtocolGPSCoverageXMLFile(string filename)
        {
            ReadOSPMasterListIntoMsgCounts();
            string str = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamReader reader = new StreamReader(str + @"\Log\" + filename);
            try
            {
                uint sb = 0;
                string str3 = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    str3 = reader.ReadLine();
                    if (str3.IndexOfAny(new char[] { ':', '=', '/' }) < 0)
                    {
                        string[] strArray = str3.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (strArray.Length >= 2)
                        {
                            try
                            {
                                uint md = Convert.ToUInt32(strArray[0]);
                                int num3 = MidMsgCountsIndexOf(md);
                                if (num3 >= 0)
                                {
                                    if (MsgCounts[num3].subIDFlag)
                                    {
                                        sb = Convert.ToUInt32(strArray[1]);
                                        int num4 = MidSubIDMsgCountsIndexOf(md, sb);
                                        if (num4 >= 0)
                                        {
                                            messageCounts local1 = MsgCounts[num4];
                                            local1.count++;
                                        }
                                    }
                                    else
                                    {
                                        messageCounts local2 = MsgCounts[num3];
                                        local2.count++;
                                    }
                                }
                                continue;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
                CreatePrtclGP2CoverageXMLFile();
                Console.WriteLine("All Done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        public static void ProtocolReportHTMLGenerator(string filename)
        {
            try
            {
                string str = ConfigurationManager.AppSettings["InstalledDirectory"];
                string str2 = str + @"\Log\ProtocolTest.html";
                string uri = str + @"\Log\ProtocolTest.xml";
                string stylesheetUri = str + @"\scripts\ProtocolReport.xsl";
                XPathDocument document = new XPathDocument(uri);
                XslCompiledTransform transform = new XslCompiledTransform();
                XmlTextWriter writer = new XmlTextWriter(str2, null);
                transform.Load(stylesheetUri);
                transform.Transform((IXPathNavigable) document, null, (XmlWriter) writer);
                writer.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR!");
            }
        }

        public static void ProtocolReportXMLGenerator(string filename)
        {
            string str = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamReader pSr = new StreamReader(str + @"\Log\ProtocolTest.csv");
            StreamWriter pSw = new StreamWriter(str + @"\Log\ProtocolTest.xml");
            string str4 = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            string str5 = "  <swVersion name=\"GSW3.5.0\">";
            string str6 = "  </swVersion>";
            string str7 = "<OSPProtocolTest>";
            string str8 = "</OSPProtocolTest>";
            pSw.WriteLine(str4);
            pSw.WriteLine(str7);
            pSw.WriteLine(str5);
            WriteOutProtocolTestResults(pSr, pSw);
            pSw.WriteLine(str6);
            pSw.WriteLine(str8);
            pSr.Close();
            pSw.Close();
        }

        public void PseudoRangeError_Summary(string filename)
        {
            int num = 0x20;
            int num2 = 0;
            Stats s = new Stats();
            Stats stats2 = new Stats();
            Stats[] statsArray = new Stats[num];
            Stats[] statsArray2 = new Stats[num];
            string str = "SW Version: Not detected";
            try
            {
                LogManager manager = new LogManager();
                manager.OpenFileRead(filename);
                string str2 = manager.ReadLine();
                for (str2 = manager.ReadLine(); str2 != null; str2 = manager.ReadLine())
                {
                    string[] strArray = str2.Split(new char[] { ',' });
                    if (strArray.Length >= 8)
                    {
                        double num3 = Convert.ToDouble(strArray[7]);
                        int index = Convert.ToInt32(strArray[0]) - 1;
                        if (num3 != -1.0)
                        {
                            s.InsertSample(Math.Abs(num3));
                            if ((index < num) && (index >= 0))
                            {
                                if (statsArray[index] == null)
                                {
                                    statsArray[index] = new Stats();
                                }
                                statsArray[index].InsertSample(Math.Abs(num3));
                            }
                        }
                        else
                        {
                            num2++;
                        }
                    }
                }
                manager.CloseFile();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Exception while generating PR error report");
            }
            try
            {
                string[] strArray2 = filename.Split(new char[] { '\\' });
                string str3 = "";
                if (strArray2.GetLength(0) > 1)
                {
                    for (int k = 0; k < (strArray2.GetLength(0) - 1); k++)
                    {
                        str3 = str3 + strArray2[k] + '\\';
                    }
                }
                StreamWriter f = new StreamWriter(str3 + @"\summary_PRError.xml");
                f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                f.WriteLine("<PseudoRangeError>");
                f.WriteLine("\t<swVersion name=\"{0}\">", str);
                this.printCDFHelper(f, s, "Pseudo Range Error - Overall");
                for (int i = 0; i < num; i++)
                {
                    if ((statsArray[i] != null) && (statsArray[i].Samples > 0))
                    {
                        this.printCDFHelper(f, statsArray[i], "Pseudo Range Error - Per SV (SV=" + i.ToString() + ")");
                    }
                }
                this.printCDFHelper(f, stats2, "Pseudo Range Error - Overall at Tunnel Exit");
                for (int j = 0; j < num; j++)
                {
                    if ((statsArray2[j] != null) && (statsArray2[j].Samples > 0))
                    {
                        this.printCDFHelper(f, statsArray2[j], "Pseudo Range Error - Per SV at Tunnel Exit (SV=" + j.ToString() + ")");
                    }
                }
                f.WriteLine("\t</swVersion>");
                f.WriteLine("</PseudoRangeError>");
                f.Close();
            }
            catch (Exception exception2)
            {
                MessageBox.Show(exception2.Message, "Exception while generating PR error report");
            }
        }

        private static void ReadOSPMasterListIntoMsgChCounts()
        {
            uint md = 0;
            uint sb = 0;
            string nm = "";
            string str2 = "";
            MsgChCounts.Clear();
            string str3 = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamReader reader = new StreamReader(str3 + @"\scripts\OSPmasterMIDlist.csv");
            string str5 = reader.ReadLine();
            str5 = reader.ReadLine();
            str5 = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                if (((str5.IndexOf("removed from OSP") <= 0) && (str5.Substring(0, 6) != ",,,,,,")) && (str5.Substring(0, 12) != ",,,,,Nothing"))
                {
                    messageChCounts counts;
                    if (str5.Substring(0, 3) == ",,,")
                    {
                        string[] strArray = str5.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                        sb = Convert.ToUInt32(strArray[0], 0x10);
                        str2 = strArray[2];
                        counts = new messageChCounts(md, sb, nm + " - " + str2);
                        MsgChCounts.Add(counts);
                    }
                    else if (str5.IndexOf('x') > 0)
                    {
                        string[] strArray2 = str5.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                        md = Convert.ToUInt32(strArray2[0], 0x10);
                        sb = 0;
                        if (strArray2.Length > 2)
                        {
                            nm = strArray2[2];
                        }
                        else
                        {
                            nm = "Unknown";
                        }
                        counts = new messageChCounts(md, sb, nm);
                        MsgChCounts.Add(counts);
                    }
                }
                str5 = reader.ReadLine();
                Console.WriteLine(str5);
            }
            reader.Close();
        }

        private static void ReadOSPMasterListIntoMsgCounts()
        {
            uint md = 0;
            uint sb = 0;
            string nm = "";
            string str2 = "";
            messageCounts item = new messageCounts(0, 0, " ");
            messageCounts counts2 = new messageCounts(0, 0, " ");
            bool flag = true;
            MsgCounts.Clear();
            string str3 = ConfigurationManager.AppSettings["InstalledDirectory"];
            StreamReader reader = new StreamReader(str3 + @"\scripts\OSPmasterMIDlist.csv");
            string str5 = reader.ReadLine();
            str5 = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                str5 = reader.ReadLine();
                if (((str5.IndexOf("removed from OSP") <= 0) && (str5.Substring(0, 6) != ",,,,,,")) && (str5.Substring(0, 12) != ",,,,,Nothing"))
                {
                    if (str5.Substring(0, 3) == ",,,")
                    {
                        string[] strArray = str5.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                        sb = Convert.ToUInt32(strArray[0], 0x10);
                        str2 = strArray[2];
                        if (flag)
                        {
                            MsgCounts.Remove(counts2);
                            flag = false;
                        }
                        item = new messageCounts(md, sb, nm + " - " + str2);
                        MsgCounts.Add(item);
                    }
                    else if ((str5.IndexOf('x') > 0) || (str5.IndexOf('X') > 0))
                    {
                        string[] strArray2 = str5.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                        md = Convert.ToUInt32(strArray2[0], 0x10);
                        if (strArray2.Length > 2)
                        {
                            nm = strArray2[2];
                        }
                        else
                        {
                            nm = "Unknown";
                        }
                        item = new messageCounts(md, nm);
                        MsgCounts.Add(item);
                        counts2 = item;
                        flag = true;
                    }
                }
            }
            reader.Close();
        }

        private static List<string> RecuresivelyBuildFilenameList(string extension, string directory)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory, extension);
                for (int i = 0; i < files.Length; i++)
                {
                    list.Add(files[i]);
                }
                string[] directories = Directory.GetDirectories(directory);
                for (int j = 0; j < directories.Length; j++)
                {
                    foreach (string str in RecuresivelyBuildFilenameList(extension, directories[j]))
                    {
                        list.Add(str);
                    }
                }
            }
            return list;
        }

        public static void ResumeCreationOfDirectoryProtocolCoverageReport(string dirPath)
        {
            List<string> filelist = new List<string>();
            List<string> pcxmllist = new List<string>();
            string str = DateTime.Now.ToString("MMddyyyy_HHmmss");
            pcLogFileSw = new StreamWriter(dirPath + @"\ProtocolCoverageSummary-" + str + ".log");
            AppLog(string.Format("Starting Protocol Coverage Summary processing of directory {0}", dirPath));
            pcxmllist = RecuresivelyBuildFilenameList("ProtocolCoverage*.xml", dirPath);
            string ln = "Completed creating Protocol Coverage Count XML files";
            AppLog(ln);
            CreateProtocolCoverageTotalsReport(dirPath, pcxmllist, filelist);
            ln = string.Format("*** Completed Protocol Coverage Summary Report for {0} ***", dirPath);
            Console.WriteLine(ln);
            AppLog(ln);
            pcLogFileSw.Close();
        }

        private void setTIA916_2DErrorLimit(string testName)
        {
            if (testName.Contains("TEST1"))
            {
                this._hrErrLimit = "60,180";
            }
            else if (testName.Contains("TEST2"))
            {
                this._hrErrLimit = "25,75";
            }
            else if (testName.Contains("TEST3"))
            {
                this._hrErrLimit = "50,150";
            }
            else if (testName.Contains("TEST4"))
            {
                this._hrErrLimit = "60,180";
            }
            else if (testName.Contains("TEST5"))
            {
                this._hrErrLimit = "35,105";
            }
        }

        public void Spur_Summary(string dir)
        {
            Hashtable hashtable = new Hashtable();
            string path = dir + @"\summary_spur.xml";
            StreamWriter f = new StreamWriter(path);
            try
            {
                if (!Directory.Exists(dir))
                {
                    MessageBox.Show(string.Format("Directory Not Found!\n\n{0}", dir), "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                    if (files.Length != 0)
                    {
                        foreach (FileInfo info2 in files)
                        {
                            PerRxReport report;
                            PerNavStateReport report2;
                            string str9;
                            int num2;
                            List<string> list;
                            Invalid_Data data = new Invalid_Data();
                            StreamReader reader = info2.OpenText();
                            string name = info2.Name;
                            string pattern = @"_P(?<loop>\d+)_(?<msgID>msg\d+)";
                            Regex regex = new Regex(pattern, RegexOptions.Compiled);
                            bool flag = regex.IsMatch(name);
                            string str4 = "Unknown";
                            string str5 = "Unknown";
                            string key = "Unknown";
                            if (flag)
                            {
                                str4 = regex.Match(name).Result("${loop}");
                                str5 = regex.Match(name).Result("${msgID}");
                            }
                            string str7 = string.Empty;
                            string str8 = string.Empty;
                            str8 = "Loop(" + str4 + ")";
                            string str25 = str5;
                            if (str25 != null)
                            {
                                if (!(str25 == "msg2"))
                                {
                                    if (str25 == "msg28")
                                    {
                                        goto Label_0146;
                                    }
                                    if (str25 == "msg41")
                                    {
                                        goto Label_014F;
                                    }
                                }
                                else
                                {
                                    key = "Nav";
                                }
                            }
                            goto Label_0156;
                        Label_0146:
                            key = "Track";
                            goto Label_0156;
                        Label_014F:
                            key = "Nav";
                        Label_0156:
                            str9 = reader.ReadLine();
                            if (str9 != null)
                            {
                                pattern = "SW Version:(?<swVer>.*)";
                                Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                                if (regex2.IsMatch(str9))
                                {
                                    str7 = regex2.Match(str9).Result("${swVer}");
                                }
                                else
                                {
                                    str7 = "SW Version: Not detected";
                                }
                            }
                            else
                            {
                                str7 = "SW Version: Not detected";
                            }
                            str9 = reader.ReadLine();
                            int num = 0;
                            string str10 = "N/A";
                            Hashtable hashtable2 = new Hashtable();
                            if (str9 != null)
                            {
                                string[] strArray = str9.Split(new char[] { '=' });
                                num2 = 3;
                                if (strArray.Length > 1)
                                {
                                    str10 = strArray[1];
                                    if (this.perRxSetupData.ContainsKey(str10))
                                    {
                                        goto Label_027C;
                                    }
                                    if (strArray.Length > 1)
                                    {
                                        hashtable2.Add(strArray[0], strArray[1]);
                                    }
                                    while (((str9 = reader.ReadLine()) != null) && (num < num2))
                                    {
                                        strArray = str9.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable2.Add(strArray[0], strArray[1]);
                                        }
                                        num++;
                                    }
                                    this.perRxSetupData.Add(str10, hashtable2);
                                }
                            }
                            goto Label_028E;
                        Label_0276:
                            num++;
                        Label_027C:
                            if (((str9 = reader.ReadLine()) != null) && (num < num2))
                            {
                                goto Label_0276;
                            }
                        Label_028E:
                            list = new List<string>();
                            if (this.perRxSetupData.ContainsKey(str10))
                            {
                                Hashtable hashtable3 = (Hashtable) this.perRxSetupData[str10];
                                string str11 = "SV List";
                                if (hashtable3.ContainsKey(str11))
                                {
                                    foreach (string str12 in ((string) hashtable3[str11]).Split(new char[] { ',' }))
                                    {
                                        if (!list.Contains(str12))
                                        {
                                            list.Add(str12);
                                        }
                                    }
                                }
                                if (!hashtable3.ContainsKey("SW Version"))
                                {
                                    hashtable3.Add("SW Version", str7);
                                }
                            }
                            string str13 = string.Empty;
                            if (this.perRxSetupData.ContainsKey(str10))
                            {
                                Hashtable hashtable4 = (Hashtable) this.perRxSetupData[str10];
                                string str14 = "Rx";
                                if (hashtable4.ContainsKey(str14))
                                {
                                    str13 = (string) hashtable4[str14];
                                }
                            }
                            if (hashtable.Contains(str13))
                            {
                                report = (PerRxReport) hashtable[str13];
                            }
                            else
                            {
                                PerRxReport report3 = new PerRxReport();
                                hashtable.Add(str13, report3);
                                report = (PerRxReport) hashtable[str13];
                            }
                            if (report.PerNavStateData.Contains(key))
                            {
                                report2 = (PerNavStateReport) report.PerNavStateData[key];
                            }
                            else
                            {
                                PerNavStateReport report4 = new PerNavStateReport();
                                report.PerNavStateData.Add(key, report4);
                                report2 = (PerNavStateReport) report.PerNavStateData[key];
                            }
                            Invalid_Data data2 = new Invalid_Data();
                            while (str9 != null)
                            {
                                string str15;
                                string[] strArray3 = str9.Split(new char[] { ',' });
                                if (strArray3.Length >= 5)
                                {
                                    str15 = strArray3[1];
                                    str25 = strArray3[2];
                                    if (str25 != null)
                                    {
                                        if (!(str25 == "2"))
                                        {
                                            if (str25 == "28")
                                            {
                                                goto Label_0607;
                                            }
                                            if (str25 == "41")
                                            {
                                                goto Label_0740;
                                            }
                                        }
                                        else
                                        {
                                            string str17 = strArray3[14];
                                            if (report2.PerMsgData.Contains("Total"))
                                            {
                                                data = (Invalid_Data) report2.PerMsgData["Total"];
                                            }
                                            else
                                            {
                                                Invalid_Data data3 = new Invalid_Data();
                                                report2.PerMsgData.Add("Total", data3);
                                                data = (Invalid_Data) report2.PerMsgData["Total"];
                                            }
                                            data.EpochCount++;
                                            if (report2.PerMsgData.Contains(str8))
                                            {
                                                data2 = (Invalid_Data) report2.PerMsgData[str8];
                                            }
                                            else
                                            {
                                                Invalid_Data data4 = new Invalid_Data();
                                                report2.PerMsgData.Add(str8, data4);
                                                data2 = (Invalid_Data) report2.PerMsgData[str8];
                                            }
                                            data2.EpochCount++;
                                            if (strArray3.Length >= 20)
                                            {
                                                if (Convert.ToInt16(str17) > 4)
                                                {
                                                    data2.InvalidSVsCount++;
                                                    data.InvalidSVsCount++;
                                                }
                                                else
                                                {
                                                    int num3 = 0;
                                                    int num4 = 12;
                                                    for (num3 = 0; num3 < num4; num3++)
                                                    {
                                                        if (strArray3[num3 + 15] != "0")
                                                        {
                                                            string str18 = strArray3[num3 + 15];
                                                            if (!list.Contains(str18))
                                                            {
                                                                data2.InvalidSVsCount++;
                                                                data.InvalidSVsCount++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                goto Label_0877;
                            Label_0607:
                                Convert.ToInt16(strArray3[11]);
                                string item = strArray3[5];
                                if (report2.PerMsgData.Contains("Total"))
                                {
                                    data = (Invalid_Data) report2.PerMsgData["Total"];
                                }
                                else
                                {
                                    Invalid_Data data5 = new Invalid_Data();
                                    report2.PerMsgData.Add("Total", data5);
                                    data = (Invalid_Data) report2.PerMsgData["Total"];
                                }
                                int num5 = 0;
                                int num6 = 0;
                                for (num6 = 0; num6 < 10; num6++)
                                {
                                    num5 += Convert.ToInt16(strArray3[num6 + 12]);
                                }
                                double num1 = ((double) num5) / 10.0;
                                if ((str15 == "0") || !list.Contains(item))
                                {
                                    if (report2.PerMsgData.Contains(str8))
                                    {
                                        data2 = (Invalid_Data) report2.PerMsgData[str8];
                                    }
                                    else
                                    {
                                        Invalid_Data data6 = new Invalid_Data();
                                        report2.PerMsgData.Add(str8, data6);
                                        data2 = (Invalid_Data) report2.PerMsgData[str8];
                                    }
                                    data2.InvalidSVsCount++;
                                    data.InvalidSVsCount++;
                                }
                                goto Label_0877;
                            Label_0740:
                                if (report2.PerMsgData.Contains("Total"))
                                {
                                    data = (Invalid_Data) report2.PerMsgData["Total"];
                                }
                                else
                                {
                                    Invalid_Data data7 = new Invalid_Data();
                                    report2.PerMsgData.Add("Total", data7);
                                    data = (Invalid_Data) report2.PerMsgData["Total"];
                                }
                                data.EpochCount++;
                                if (report2.PerMsgData.Contains(str8))
                                {
                                    data2 = (Invalid_Data) report2.PerMsgData[str8];
                                }
                                else
                                {
                                    Invalid_Data data8 = new Invalid_Data();
                                    report2.PerMsgData.Add(str8, data8);
                                    data2 = (Invalid_Data) report2.PerMsgData[str8];
                                }
                                data2.EpochCount++;
                                if (str15 == "1")
                                {
                                    double sample = Convert.ToDouble(strArray3[strArray3.Length - 1]);
                                    data.MyStats.InsertSample(sample);
                                    data2.MyStats.InsertSample(sample);
                                    if (sample > Convert.ToDouble(this._hrErrLimit))
                                    {
                                        data2.HrErrCount++;
                                        data.HrErrCount++;
                                    }
                                }
                            Label_0877:
                                str9 = reader.ReadLine();
                            }
                            reader.Close();
                        }
                        f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        f.WriteLine("<spur>");
                        foreach (string str20 in hashtable.Keys)
                        {
                            f.WriteLine("\t<rxName name=\"{0}\">", str20);
                            PerRxReport report5 = (PerRxReport) hashtable[str20];
                            int num8 = 0;
                            int num9 = 0;
                            double num10 = 0.0;
                            double num11 = 0.0;
                            double num12 = 0.0;
                            double num13 = 0.0;
                            Hashtable hashtable5 = new Hashtable();
                            foreach (string str21 in report5.PerNavStateData.Keys)
                            {
                                f.WriteLine("\t\t<msg name=\"{0}\">", str21);
                                PerNavStateReport report6 = (PerNavStateReport) report5.PerNavStateData[str21];
                                foreach (string str22 in report6.PerMsgData.Keys)
                                {
                                    Invalid_Data data9 = (Invalid_Data) report6.PerMsgData[str22];
                                    if (str21 == "Nav")
                                    {
                                        num8 = data9.EpochCount / 2;
                                        num10 = data9.MyStats.Stats_Max((double) Convert.ToInt32("-9999"));
                                        num11 = data9.MyStats.Stats_Min((double) Convert.ToInt32("-9999"));
                                        num12 = data9.MyStats.Stats_Mean((double) Convert.ToInt32("-9999"));
                                        num13 = data9.MyStats.StandardDeviation((double) Convert.ToInt32("-9999"));
                                    }
                                    else
                                    {
                                        num10 = 0.0;
                                        num11 = 0.0;
                                        num12 = 0.0;
                                        num13 = 0.0;
                                    }
                                    if (((data9.InvalidSVsCount != 0) || (data9.HrErrCount != 0)) && (str22 != "Total"))
                                    {
                                        f.WriteLine("\t\t\t\t<loop number=\"{0}\">", str22);
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Invalid SV\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", data9.InvalidSVsCount, this._invalidSV);
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Hr Error &gt;{0} m\" value=\"{1}\" criteria=\"{2}\" direction=\"&lt;\" units=\"\"/>", this._hrErrLimit, data9.HrErrCount, this._limitVal);
                                        if (str21 == "Nav")
                                        {
                                            f.WriteLine("\t\t\t\t\t<field name=\"Max Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num10);
                                            f.WriteLine("\t\t\t\t\t<field name=\"Min Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num11);
                                            f.WriteLine("\t\t\t\t\t<field name=\"Mean Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num12);
                                            f.WriteLine("\t\t\t\t\t<field name=\"Std Dev Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num13);
                                        }
                                        else
                                        {
                                            f.WriteLine("\t\t\t\t\t<field name=\"Max Error(m)\" value=\" -\" criteria=\"\" direction=\"\" units=\"\"/>");
                                            f.WriteLine("\t\t\t\t\t<field name=\"Min Error(m)\" value=\" -\" criteria=\"\" direction=\"\" units=\"\"/>");
                                            f.WriteLine("\t\t\t\t\t<field name=\"Mean Error(m)\" value=\" -\" criteria=\"\" direction=\"\" units=\"\"/>");
                                            f.WriteLine("\t\t\t\t\t<field name=\"Std Dev Error(m)\" value=\" -\" criteria=\"\" direction=\"\" units=\"\"/>");
                                        }
                                        f.WriteLine("\t\t\t\t\t<field name=\"# Epoch\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num8);
                                        f.WriteLine("\t\t\t\t</loop>");
                                    }
                                }
                                string str23 = "Total";
                                f.WriteLine("\t\t\t\t<loop number=\"{0}\">", str23);
                                Invalid_Data data10 = (Invalid_Data) report6.PerMsgData[str23];
                                if (str21 == "Nav")
                                {
                                    num9 = data10.EpochCount / 2;
                                    num10 = data10.MyStats.Stats_Max((double) -9999.0);
                                    num11 = data10.MyStats.Stats_Min((double) -9999.0);
                                    num12 = data10.MyStats.Stats_Mean((double) -9999.0);
                                    num13 = data10.MyStats.StandardDeviation((double) -9999.0);
                                    hashtable5.Add("Max Error(m)", num10);
                                    hashtable5.Add("Min Error(m)", num11);
                                    hashtable5.Add("Mean Error(m)", num12);
                                    hashtable5.Add("Std Dev Error(m)", num13);
                                    hashtable5.Add("Total Test Time (hour)", ((double) num9) / 3600.0);
                                    hashtable5.Add("Total Invalid SVs in Nav", (double) data10.InvalidSVsCount);
                                    hashtable5.Add("Total Horz Error > " + this._hrErrLimit.ToString() + "(m)", (double) data10.HrErrCount);
                                }
                                else
                                {
                                    num10 = 0.0;
                                    num11 = 0.0;
                                    num12 = 0.0;
                                    num13 = 0.0;
                                    hashtable5.Add("Total Invalid SVs in Track", (double) data10.InvalidSVsCount);
                                }
                                f.WriteLine("\t\t\t\t\t<field name=\"# Invalid SV\" value=\"{0}\" criteria=\"{1}\" direction=\"&lt;\" units=\"\"/>", data10.InvalidSVsCount, this._invalidSV);
                                f.WriteLine("\t\t\t\t\t<field name=\"# Hr Error &gt;{0} m\" value=\"{1}\" criteria=\"{2}\" direction=\"&lt;\" units=\"\"/>", this._hrErrLimit, data10.HrErrCount, this._limitVal);
                                if (str21 == "Nav")
                                {
                                    f.WriteLine("\t\t\t\t\t<field name=\"Max Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num10);
                                    f.WriteLine("\t\t\t\t\t<field name=\"Min Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num11);
                                    f.WriteLine("\t\t\t\t\t<field name=\"Mean Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num12);
                                    f.WriteLine("\t\t\t\t\t<field name=\"Std Dev Error(m)\" value=\"{0:F3}\" criteria=\"\" direction=\"\" units=\"\"/>", num13);
                                }
                                else
                                {
                                    f.WriteLine("\t\t\t\t\t<field name=\"Max Error(m)\" value=\" -\" criteria=\"\" direction=\"\" units=\"\"/>");
                                    f.WriteLine("\t\t\t\t\t<field name=\"Min Error(m)\" value=\" - \" criteria=\"\" direction=\"\" units=\"\"/>");
                                    f.WriteLine("\t\t\t\t\t<field name=\"Mean Error(m)\" value=\" -\" criteria=\"\" direction=\"\" units=\"\"/>");
                                    f.WriteLine("\t\t\t\t\t<field name=\"Std Dev Error(m)\" value=\" -\" criteria=\"\" direction=\"\" units=\"\"/>");
                                }
                                f.WriteLine("\t\t\t\t\t<field name=\"# Epoch\" value=\"{0}\" criteria=\"\" direction=\"\" units=\"\"/>", num9);
                                f.WriteLine("\t\t\t\t</loop>");
                                f.WriteLine("\t\t</msg>");
                                if (!this._perRxSummary.Contains(str20))
                                {
                                    this._perRxSummary.Add(str20, hashtable5);
                                }
                                else
                                {
                                    this._perRxSummary[str20] = hashtable5;
                                }
                            }
                            f.WriteLine("\t</rxName>");
                        }
                        this.printTestSummary(f);
                        this.printTestSetup(f);
                        f.WriteLine("</spur>");
                        f.Close();
                        hashtable.Clear();
                        this.perRxSetupData.Clear();
                        string outputFile = dir + @"\summary_spur.html";
                        this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\spurReport.xsl");
                    }
                }
            }
            catch (Exception exception)
            {
                this.perRxSetupData.Clear();
                hashtable.Clear();
                f.Close();
                MessageBox.Show(exception.Message, "Error");
            }
        }

        public void Summary_3GPP(string dir)
        {
            Hashtable hashtable = new Hashtable();
            int index = 8;
            int num2 = index + 1;
            int num3 = num2 + 3;
            int num4 = num3 + 3;
            int num5 = num4 + 5;
            int num6 = 12;
            double sample = 0.0;
            string path = dir + @"\summary_3GPP.xml";
            StreamWriter f = new StreamWriter(path);
            try
            {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                if (files.Length != 0)
                {
                    foreach (FileInfo info2 in files)
                    {
                        Header1DataClass class2;
                        PerEnvReport report;
                        ReportElements elements;
                        StreamReader reader = info2.OpenText();
                        string name = info2.Name;
                        string pattern = @"_(com|COM|tcp|TCP|i2c|I2C)\d+_(?<testName>\w+)_(?<margin>\d+p\d?)";
                        Regex regex = new Regex(pattern, RegexOptions.Compiled);
                        bool flag = regex.IsMatch(name);
                        string key = "Unknown";
                        string str5 = "Unknown";
                        if (flag)
                        {
                            key = regex.Match(name).Result("${margin}").Replace('p', '.');
                            str5 = regex.Match(name).Result("${testName}");
                        }
                        string str6 = string.Empty;
                        string input = reader.ReadLine();
                        if (input != null)
                        {
                            pattern = @"SW Version\s*=\s*(?<swVer>.*)";
                            Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                            if (regex2.IsMatch(input))
                            {
                                str6 = regex2.Match(input).Result("${swVer}");
                            }
                            else
                            {
                                str6 = "SW Version= Not detected";
                            }
                        }
                        else
                        {
                            str6 = "SW Version= Not detected";
                        }
                        if (hashtable.Contains(str6))
                        {
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        else
                        {
                            Header1DataClass class3 = new Header1DataClass();
                            hashtable.Add(str6, class3);
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        if (class2.Header2DataHash.Contains(str5))
                        {
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        else
                        {
                            PerEnvReport report2 = new PerEnvReport();
                            class2.Header2DataHash.Add(str5, report2);
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        if (report.PerSiteData.Contains(key))
                        {
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        else
                        {
                            ReportElements elements2 = new ReportElements();
                            report.PerSiteData.Add(key, elements2);
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        input = reader.ReadLine();
                        string str8 = "N/A";
                        Hashtable hashtable2 = new Hashtable();
                        int num8 = 0;
                        if (input != null)
                        {
                            string[] strArray = input.Split(new char[] { '=' });
                            if (strArray.Length > 1)
                            {
                                str8 = strArray[1] + "_" + str5;
                                if (this.perRxSetupData.ContainsKey(str8))
                                {
                                    goto Label_0345;
                                }
                                hashtable2.Add("Test", str5);
                                if (strArray.Length > 1)
                                {
                                    hashtable2.Add(strArray[0], strArray[1]);
                                }
                                while ((input = reader.ReadLine()) != null)
                                {
                                    if (input == "End Summary")
                                    {
                                        break;
                                    }
                                    if (input != string.Empty)
                                    {
                                        strArray = input.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable2.Add(strArray[0], strArray[1]);
                                        }
                                        num8++;
                                    }
                                }
                                this.perRxSetupData.Add(str8, hashtable2);
                            }
                        }
                        goto Label_0351;
                    Label_0323:
                        if (input == "End Summary")
                        {
                            goto Label_0351;
                        }
                        if (!(input == string.Empty))
                        {
                            num8++;
                        }
                    Label_0345:
                        if ((input = reader.ReadLine()) != null)
                        {
                            goto Label_0323;
                        }
                    Label_0351:
                        input = reader.ReadLine();
                        for (input = reader.ReadLine(); input != null; input = reader.ReadLine())
                        {
                            string[] strArray2 = input.Split(new char[] { ',' });
                            double num9 = -9999.0;
                            if (strArray2.Length >= 5)
                            {
                                double num10 = Convert.ToDouble(strArray2[this._ttffReportType]);
                                double num11 = Convert.ToDouble(strArray2[index]);
                                double num12 = Convert.ToDouble(this._limitVal);
                                double num13 = 0.0;
                                string[] strArray3 = this._hrErrLimit.Split(new char[] { ',' });
                                if (strArray3.Length > 0)
                                {
                                    num13 = Convert.ToDouble(strArray3[strArray3.Length - 1]);
                                }
                                else
                                {
                                    num13 = Convert.ToDouble(strArray3[0]);
                                }
                                if (str5.Contains("TEST2") && (num13 > 30.0))
                                {
                                    num13 = 30.0;
                                }
                                if (str5.Contains("TEST5"))
                                {
                                    num12 = 240.0;
                                }
                                if (num10 <= 0.0)
                                {
                                    elements.NumberOfTTFFMisses++;
                                }
                                else if (num10 == Convert.ToDouble(this._timeoutVal))
                                {
                                    elements.NumberOfTTFFMisses++;
                                }
                                else if (num10 > num12)
                                {
                                    elements.NumberOfTTFFOutOfLimits++;
                                }
                                if (num11 <= 0.0)
                                {
                                    elements.NumberOf2DMisses++;
                                }
                                if (num11 > num13)
                                {
                                    elements.NumberOf2DOutOfLimits++;
                                }
                                if (num10 <= 0.0)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num10 > num12)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num11 <= 0.0)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num11 > num13)
                                {
                                    elements.NumberOfMisses++;
                                }
                                double num14 = Convert.ToDouble(strArray2[num3]);
                                if (num14 != -9999.0)
                                {
                                    num9 = Math.Abs((double) (num14 - Convert.ToDouble(strArray2[num4])));
                                }
                                elements.TTFFSamples.InsertSample(num10);
                                elements.Position2DErrorSamples.InsertSample(num11);
                                elements.VerticalErrorSamples.InsertSample(num9);
                                report.PerEnvSamples.TTFFSamples.InsertSample(num10);
                                report.PerEnvSamples.Position2DErrorSamples.InsertSample(num11);
                                report.PerEnvSamples.VerticalErrorSamples.InsertSample(num9);
                                class2.ReportDataSamples.TTFFSamples.InsertSample(num10);
                                class2.ReportDataSamples.Position2DErrorSamples.InsertSample(num11);
                                class2.ReportDataSamples.VerticalErrorSamples.InsertSample(num9);
                                for (int i = 0; i < num6; i++)
                                {
                                    sample = Convert.ToDouble(strArray2[num5 + i]);
                                    if (sample != 0.0)
                                    {
                                        elements.CNOSamples.InsertSample(sample);
                                        report.PerEnvSamples.CNOSamples.InsertSample(sample);
                                        class2.ReportDataSamples.CNOSamples.InsertSample(sample);
                                    }
                                }
                            }
                        }
                        reader.Close();
                    }
                    f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    f.WriteLine("<threeGPP>");
                    string limitStr = this._ttffLimit;
                    foreach (string str10 in hashtable.Keys)
                    {
                        Header1DataClass class4 = (Header1DataClass) hashtable[str10];
                        f.WriteLine("\t<swVersion name=\"{0}\">", str10);
                        foreach (string str11 in class4.Header2DataHash.Keys)
                        {
                            int checkSamples = 0;
                            if (str11.Contains("TEST5"))
                            {
                                checkSamples = 0x71;
                                limitStr = "240";
                            }
                            PerEnvReport report3 = (PerEnvReport) class4.Header2DataHash[str11];
                            f.WriteLine("\t\t<environment name=\"{0}\">", str11);
                            ArrayList list = new ArrayList();
                            foreach (string str12 in report3.PerSiteData.Keys)
                            {
                                list.Add(str12);
                            }
                            list.Sort();
                            foreach (string str13 in list)
                            {
                                ReportElements elements3 = (ReportElements) report3.PerSiteData[str13];
                                f.WriteLine("\t\t\t<site number=\"{0}\">", str13);
                                this.print3GPPSummary(f, checkSamples, elements3.TTFFSamples.Samples, elements3.NumberOfMisses, elements3.NumberOfTTFFMisses, elements3.NumberOfTTFFOutOfLimits, elements3.NumberOf2DMisses, elements3.NumberOf2DOutOfLimits);
                                this.printSampleData(f, elements3.TTFFSamples, this._percentile, limitStr, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                                this.printSampleData(f, elements3.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                                this.printSampleData(f, elements3.VerticalErrorSamples, this._percentile, "", -9999.0, "Vertical Error", "m");
                                this.printSampleData(f, elements3.CNOSamples, this._percentile, "", -9999.0, "CNO", "dbHz");
                                f.WriteLine("\t\t\t</site>");
                            }
                            f.WriteLine("\t\t</environment>");
                        }
                        f.WriteLine("\t</swVersion>");
                    }
                    this.printTestSetup(f);
                    f.WriteLine("</threeGPP>");
                    f.Close();
                    hashtable.Clear();
                    this.perRxSetupData.Clear();
                    string outputFile = dir + @"\summary_3GPP.html";
                    this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\3GPP\3GPPReport.xsl");
                }
            }
            catch (Exception exception)
            {
                this.perRxSetupData.Clear();
                hashtable.Clear();
                f.Close();
                MessageBox.Show(exception.Message, "ERROR");
            }
        }

        public void Summary_CompareWithRef(string dir)
        {
            string path = dir + @"\summary_RoadTest.xml";
            StreamWriter writer = new StreamWriter(path);
            try
            {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                if (files.Length != 0)
                {
                    int num2;
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<P2P>");
                    ArrayList list = new ArrayList();
                    foreach (FileInfo info2 in files)
                    {
                        StreamReader reader = info2.OpenText();
                        List<string> list2 = new List<string>();
                        string input = reader.ReadLine();
                        input = reader.ReadLine();
                        for (int i = 0; i < 0x23; i++)
                        {
                            if (input != null)
                            {
                                string pattern = string.Empty;
                                if (input.Contains("SW Version"))
                                {
                                    pattern = @"=\s*(?<Data>.*)";
                                }
                                else
                                {
                                    pattern = @"=\s*(?<Data>[+-]*\w+\.*\w*)";
                                }
                                Regex regex = new Regex(pattern, RegexOptions.Compiled);
                                if (regex.IsMatch(input))
                                {
                                    list2.Add(regex.Match(input).Result("${Data}"));
                                }
                            }
                            else
                            {
                                list2.Add("9999");
                            }
                            input = reader.ReadLine();
                        }
                        list.Add(list2);
                        reader.Close();
                    }
                    int num3 = 1;
                    writer.WriteLine("<data name=\"SW Version\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"MaxCN0 Ave (dBHz)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Samples\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Tracking Availability\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"HDOP Ave\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Satellite Number Ave\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Horizontal Error 50% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Horizontal Error 68% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Horizontal Error 95% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Horizontal Error Max (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Altitude Error 50% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Altitude Error 68% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Altitude Error 95% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Altitude Error Max (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Cross Track Error 50% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Cross Track Error 68% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Cross Track Error 95% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Cross Track Error Max (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Along Track Error 50% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Along Track Error 68% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Along Track Error 95% (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Along Track Error Max (m)\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Exceed50mCount\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Exceed50m3sNotPullInCount\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Exceed25mCount\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Exceed25m3sNotPullInCount\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Exceed10mCount\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Exceed10m3sNotPullInCount\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Heading Error 50%\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Heading Error 68%\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    num3++;
                    writer.WriteLine("<data name=\"Heading Error 95%\">");
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    writer.WriteLine("<data name=\"Heading Error Max%\">");
                    num3++;
                    for (num2 = 0; num2 < list.Count; num2++)
                    {
                        writer.WriteLine("\t<field name=\"{0}\" value=\"{1}\" criteria=\"\" direction=\"\" units=\"\"/>", ((List<string>) list[num2])[0], ((List<string>) list[num2])[num3]);
                    }
                    writer.WriteLine("</data>");
                    writer.WriteLine("</P2P>");
                    writer.Close();
                    string outputFile = dir + @"\summary_RoadTest.html";
                    this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\p2pReport.xsl");
                }
            }
            catch (Exception exception)
            {
                writer.Close();
                MessageBox.Show(exception.Message, "ERROR");
            }
        }

        public void Summary_LSM(string dir, double refLat, double refLon, double refAlt, int latIdx, int lonIdx, int altIdx, int ttffIdx, int startCNoIdx)
        {
            frmResetTestReportPlots plots = new frmResetTestReportPlots();
            new ZedGraphControl();
            Hashtable hashtable = new Hashtable();
            PositionErrorCalc calc = new PositionErrorCalc();
            int num = 12;
            int index = (startCNoIdx + num) + 1;
            double sample = 0.0;
            string path = dir + @"\summary_reset.xml";
            StreamWriter f = new StreamWriter(path);
            try
            {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                if (files.Length != 0)
                {
                    foreach (FileInfo info2 in files)
                    {
                        Header1DataClass class2;
                        PerEnvReport report;
                        ReportElements elements;
                        StreamReader reader = info2.OpenText();
                        string name = info2.Name;
                        string pattern = @"_(com|COM|tcp|TCP|i2c|I2C)\d+_reset(?<testName>\w+)_(?<cno>-?\d+\.*\d?)";
                        Regex regex = new Regex(pattern, RegexOptions.Compiled);
                        bool flag = regex.IsMatch(name);
                        string key = "Unknown";
                        string str5 = "Unknown";
                        if (flag)
                        {
                            key = regex.Match(name).Result("${cno}");
                            str5 = regex.Match(name).Result("${testName}");
                        }
                        string str6 = string.Empty;
                        string input = reader.ReadLine();
                        if (input != null)
                        {
                            pattern = @"SW Version:\s*(?<swVer>.*)";
                            Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                            if (regex2.IsMatch(input))
                            {
                                str6 = regex2.Match(input).Result("${swVer}");
                            }
                            else
                            {
                                str6 = "SW Version: Not detected";
                            }
                        }
                        else
                        {
                            str6 = "SW Version: Not detected";
                        }
                        if (hashtable.Contains(str6))
                        {
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        else
                        {
                            Header1DataClass class3 = new Header1DataClass();
                            hashtable.Add(str6, class3);
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        if (class2.Header2DataHash.Contains(str5))
                        {
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        else
                        {
                            PerEnvReport report2 = new PerEnvReport();
                            class2.Header2DataHash.Add(str5, report2);
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        if (report.PerSiteData.Contains(key))
                        {
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        else
                        {
                            ReportElements elements2 = new ReportElements();
                            report.PerSiteData.Add(key, elements2);
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        input = reader.ReadLine();
                        string str8 = "N/A";
                        Hashtable hashtable2 = new Hashtable();
                        int num4 = 0;
                        if (input != null)
                        {
                            string[] strArray = input.Split(new char[] { '=' });
                            if (strArray.Length > 1)
                            {
                                str8 = strArray[1];
                                if (this.perRxSetupData.ContainsKey(str8))
                                {
                                    goto Label_0310;
                                }
                                if (strArray.Length > 1)
                                {
                                    hashtable2.Add(strArray[0], strArray[1]);
                                }
                                while ((input = reader.ReadLine()) != null)
                                {
                                    if (input == "End Summary")
                                    {
                                        break;
                                    }
                                    if (input != string.Empty)
                                    {
                                        strArray = input.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable2.Add(strArray[0], strArray[1]);
                                        }
                                        num4++;
                                    }
                                }
                                this.perRxSetupData.Add(str8, hashtable2);
                            }
                        }
                        goto Label_031C;
                    Label_0302:
                        if (input == "End Summary")
                        {
                            goto Label_031C;
                        }
                    Label_0310:
                        if ((input = reader.ReadLine()) != null)
                        {
                            goto Label_0302;
                        }
                    Label_031C:
                        input = reader.ReadLine();
                        for (input = reader.ReadLine(); input != null; input = reader.ReadLine())
                        {
                            string[] strArray2 = input.Split(new char[] { ',' });
                            double num5 = -9999.0;
                            if (strArray2.Length >= 5)
                            {
                                double num6 = Convert.ToDouble(strArray2[ttffIdx]);
                                double horizontalError = -9999.0;
                                double num8 = Convert.ToDouble(this._limitVal);
                                string[] strArray3 = this._hrErrLimit.Split(new char[] { ',' });
                                int num9 = -1;
                                try
                                {
                                    num9 = Convert.ToInt32(strArray2[index]);
                                }
                                catch
                                {
                                }
                                if ((num9 >= 0) && (num9 <= 3))
                                {
                                    elements.NumberOfSVInFix++;
                                }
                                if (strArray3.Length > 0)
                                {
                                    Convert.ToDouble(strArray3[strArray3.Length - 1]);
                                }
                                else
                                {
                                    Convert.ToDouble(strArray3[0]);
                                }
                                if (num6 <= 0.0)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num6 >= num8)
                                {
                                    elements.NumberOfMisses++;
                                }
                                double num10 = Convert.ToDouble(strArray2[altIdx]);
                                if (num10 != -9999.0)
                                {
                                    num5 = Math.Abs((double) (num10 - refAlt));
                                }
                                calc.GetPositionErrorsInMeter(Convert.ToDouble(strArray2[latIdx]), Convert.ToDouble(strArray2[lonIdx]), Convert.ToDouble(strArray2[altIdx]), refLat, refLon, refAlt);
                                horizontalError = calc.HorizontalError;
                                try
                                {
                                    elements.TTFFSamples.InsertSample(num6);
                                    elements.Position2DErrorSamples.InsertSample(horizontalError);
                                    elements.VerticalErrorSamples.InsertSample(num5);
                                    report.PerEnvSamples.TTFFSamples.InsertSample(num6);
                                    report.PerEnvSamples.Position2DErrorSamples.InsertSample(horizontalError);
                                    report.PerEnvSamples.VerticalErrorSamples.InsertSample(num5);
                                    class2.ReportDataSamples.TTFFSamples.InsertSample(num6);
                                    class2.ReportDataSamples.Position2DErrorSamples.InsertSample(horizontalError);
                                    class2.ReportDataSamples.VerticalErrorSamples.InsertSample(num5);
                                    for (int i = 0; i < num; i++)
                                    {
                                        if (startCNoIdx != -1)
                                        {
                                            sample = Convert.ToDouble(strArray2[startCNoIdx + i]);
                                        }
                                        else
                                        {
                                            sample = -9999.0;
                                        }
                                        if (sample != 0.0)
                                        {
                                            elements.CNOSamples.InsertSample(sample);
                                            report.PerEnvSamples.CNOSamples.InsertSample(sample);
                                            class2.ReportDataSamples.CNOSamples.InsertSample(sample);
                                        }
                                    }
                                }
                                catch (FormatException exception)
                                {
                                    exception.ToString();
                                }
                            }
                        }
                        reader.Close();
                    }
                    f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    f.WriteLine("<reset>");
                    int styleCt = 0;
                    List<string> il = new List<string>();
                    foreach (string str9 in hashtable.Keys)
                    {
                        Header1DataClass class4 = (Header1DataClass) hashtable[str9];
                        f.WriteLine("\t<swVersion name=\"{0}\">", str9);
                        foreach (string str10 in class4.Header2DataHash.Keys)
                        {
                            plots.PaneTitle = str9 + "\r\n" + str10;
                            plots.CreateCharts();
                            GraphPane myPane = plots.AddPane("TTFF Resets", "Secs");
                            GraphPane pane2 = plots.AddPane("2-D Error", "Meters");
                            GraphPane pane3 = plots.AddPane("Verical Error", "Meters");
                            PerEnvReport report3 = (PerEnvReport) class4.Header2DataHash[str10];
                            styleCt++;
                            string item = str10 + styleCt.ToString() + ".jpg";
                            string str12 = "Style" + styleCt.ToString();
                            il.Add(item);
                            f.WriteLine("\t\t<environment name=\"{0}\" plotImage=\"{1}\"  showpicturestyle=\"{2}\">", str10, item, str12);
                            ArrayList list2 = new ArrayList();
                            foreach (string str13 in report3.PerSiteData.Keys)
                            {
                                list2.Add(str13);
                            }
                            list2.Sort();
                            int num13 = 0;
                            foreach (string str14 in list2)
                            {
                                ReportElements elements3 = (ReportElements) report3.PerSiteData[str14];
                                f.WriteLine("\t\t\t<site number=\"{0}\">", str14);
                                this.printResetSummary(f, elements3.TTFFSamples.Samples, elements3.NumberOfMisses, elements3.NumberOfSVInFix);
                                this.printSampleData(f, elements3.TTFFSamples, this._percentile, this._ttffLimit, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                                plots.AddCurve(myPane, elements3.TTFFSamples, elements3.TTFFSamples.Samples, str14);
                                this.printSampleData(f, elements3.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                                plots.AddCurve(pane2, elements3.Position2DErrorSamples, elements3.Position2DErrorSamples.Samples, str14);
                                this.printSampleData(f, elements3.VerticalErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "Vertical Error", "m");
                                plots.AddCurve(pane3, elements3.VerticalErrorSamples, elements3.VerticalErrorSamples.Samples, str14);
                                this.printSampleData(f, elements3.CNOSamples, this._percentile, "", -9999.0, "CNO", "dbHz");
                                f.WriteLine("\t\t\t</site>");
                                num13++;
                            }
                            plots.RefreshGraph();
                            plots.SaveGraphToFile(dir, item);
                            plots.PaneTitle = string.Empty;
                            f.WriteLine("\t\t</environment>");
                        }
                        f.WriteLine("\t</swVersion>");
                    }
                    this.modifyResetReportCSS(dir, styleCt);
                    this.printTestSetup(f);
                    this.printImageList(f, il);
                    f.WriteLine("</reset>");
                    f.Close();
                    hashtable.Clear();
                    this.perRxSetupData.Clear();
                    string outputFile = dir + @"\summary_reset.html";
                    this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\resetReport.xsl");
                }
            }
            catch (Exception exception2)
            {
                this.perRxSetupData.Clear();
                hashtable.Clear();
                f.Close();
                MessageBox.Show(exception2.Message, "ERROR");
            }
        }

        public void Summary_MPM(string directoryPath)
        {
            StreamReader reader = null;
            StreamWriter writer = null;
            PortManager manager = new PortManager();
            List<string> list = null;
            List<double> list2 = new List<double>();
            List<double> list3 = new List<double>();
            List<double> list4 = new List<double>();
            List<double> list5 = new List<double>();
            List<double> list6 = new List<double>();
            List<double> list7 = new List<double>();
            try
            {
                FileInfo[] files = new DirectoryInfo(directoryPath).GetFiles("*.gps");
                int[] numArray = new int[8];
                string[] strArray = new string[] { "Not Use", "SV Data", "Eph Collection", "Almanac Collection", "GPS Update", "Recovery Mode", "Not Use", "Full Pwr SV Data Update" };
                if (files.Length != 0)
                {
                    new ArrayList();
                    foreach (FileInfo info2 in files)
                    {
                        reader = info2.OpenText();
                        writer = new StreamWriter(info2.FullName.Replace(".gps", "_mpmPar.csv"));
                        List<SiRFawareStatsParams> list8 = new List<SiRFawareStatsParams>();
                        list = new List<string>();
                        string name = info2.Name;
                        Hashtable siRFAwareScanResHash = null;
                        string csvString = reader.ReadLine();
                        string str3 = "Not Detect";
                        bool flag = false;
                        bool flag2 = false;
                        for (int i = 0; i < numArray.Length; i++)
                        {
                            numArray[i] = 0;
                        }
                        while (csvString != null)
                        {
                            if (csvString.StartsWith("77,1"))
                            {
                                siRFAwareScanResHash = manager.comm.getMPMStatsDataForGUIFromCSV(0x4d, 1, "OSP", csvString);
                                flag2 = true;
                                list.Add(csvString);
                            }
                            else if (csvString.StartsWith("77,2"))
                            {
                                siRFAwareScanResHash = manager.comm.getMPMStatsDataForGUIFromCSV(0x4d, 2, "OSP", csvString);
                                flag2 = true;
                                list.Add(csvString);
                            }
                            else if (csvString.StartsWith("77,3"))
                            {
                                siRFAwareScanResHash = manager.comm.getMPMStatsDataForGUIFromCSV(0x4d, 3, "OSP", csvString);
                                flag2 = true;
                                list.Add(csvString);
                            }
                            else if (csvString.StartsWith("77,4"))
                            {
                                siRFAwareScanResHash = manager.comm.getMPMStatsDataForGUIFromCSV(0x4d, 4, "OSP", csvString);
                                flag2 = true;
                                list.Add(csvString);
                            }
                            else if (csvString.StartsWith("77,5"))
                            {
                                siRFAwareScanResHash = manager.comm.getMPMStatsDataForGUIFromCSV(0x4d, 5, "OSP", csvString);
                                flag2 = true;
                                list.Add(csvString);
                            }
                            else if (csvString.StartsWith("77,7"))
                            {
                                siRFAwareScanResHash = manager.comm.getMPMStatsDataForGUIFromCSV(0x4d, 7, "OSP", csvString);
                                flag2 = true;
                                list.Add(csvString);
                            }
                            else if (!flag && csvString.Contains("SW Version"))
                            {
                                string pattern = @"SW Version*:\s*(?<swVer>.*)";
                                Regex regex = new Regex(pattern, RegexOptions.Compiled);
                                if (regex.IsMatch(csvString))
                                {
                                    str3 = regex.Match(csvString).Result("${swVer}");
                                    flag = true;
                                    list.Add(csvString);
                                }
                            }
                            if (flag2)
                            {
                                SiRFawareStatsParams item = manager.comm.RxCtrl.DecodeMPMStats(siRFAwareScanResHash);
                                if (item != null)
                                {
                                    list8.Add(item);
                                }
                            }
                            csvString = reader.ReadLine();
                        }
                        list2.Clear();
                        list3.Clear();
                        list4.Clear();
                        list5.Clear();
                        list6.Clear();
                        list7.Clear();
                        foreach (SiRFawareStatsParams params2 in list8)
                        {
                            if (params2.isValid_TimeSpentInFullPowerSec)
                            {
                                list2.Add((double) params2.TOW);
                                list3.Add((double) params2.TimeSpentInFullPowerSec);
                            }
                            if (params2.isValid_TotalSVMeasureWithBE)
                            {
                                list4.Add((double) params2.TOW);
                                list5.Add((double) params2.TotalSVMeasureWithBE);
                            }
                            if (params2.isValid_TotalSVMeasureWithEE)
                            {
                                list6.Add((double) params2.TOW);
                                list7.Add((double) params2.TotalSVMeasureWithEE);
                            }
                            if ((params2.UpdateType >= 0) && (params2.UpdateType < numArray.Length))
                            {
                                numArray[params2.UpdateType]++;
                            }
                        }
                        frmMPMStatsPlots plots = new frmMPMStatsPlots();
                        string label = "On Time(s)";
                        Color blue = Color.Blue;
                        plots.SetPlotData(list2.ToArray(), list3.ToArray(), "SiRFaware Plot(s)", label, blue);
                        plots.Show();
                        label = "Total SVs With BE";
                        blue = Color.Green;
                        plots.SetPlotData(list4.ToArray(), list5.ToArray(), "SiRFaware Plot(s)", label, blue);
                        plots.AddCurveToPlot();
                        label = "Total SVs With EE";
                        blue = Color.Purple;
                        plots.SetPlotData(list6.ToArray(), list7.ToArray(), "SiRFaware Plot(s)", label, blue);
                        plots.AddCurveToPlot();
                        plots.Width = 0x3e8;
                        plots.SavePlots(info2.FullName.Replace(".gps", ".jpeg"));
                        plots.Close();
                        writer.WriteLine("Data File Path = {0}", name);
                        writer.WriteLine("SW Version = {0}", str3);
                        for (int j = 1; j < numArray.Length; j++)
                        {
                            if (j != 6)
                            {
                                writer.WriteLine("{0} = {1}", strArray[j], numArray[j]);
                            }
                        }
                        writer.WriteLine("End Summary");
                        foreach (string str6 in list)
                        {
                            writer.WriteLine(str6);
                        }
                        reader.Close();
                        writer.Close();
                        if (list != null)
                        {
                            list.Clear();
                            list = null;
                        }
                    }
                    this.mpmReportGen(directoryPath);
                }
            }
            catch
            {
                list2.Clear();
                list3.Clear();
                list4.Clear();
                list5.Clear();
                list6.Clear();
                list7.Clear();
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (manager != null)
                {
                    manager = null;
                }
                if (list != null)
                {
                    list.Clear();
                    list = null;
                }
            }
        }

        public void Summary_Reset(string dir)
        {
            frmResetTestReportPlots plots = new frmResetTestReportPlots();
            new ZedGraphControl();
            Hashtable hashtable = new Hashtable();
            int index = 8;
            int num2 = index + 1;
            int num3 = num2 + 3;
            int num4 = num3 + 3;
            int num5 = num4 + 5;
            int num6 = 12;
            int num7 = (num6 + num5) + 1;
            double sample = 0.0;
            string path = dir + @"\summary_reset.xml";
            StreamWriter f = new StreamWriter(path);
            try
            {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                if (files.Length != 0)
                {
                    foreach (FileInfo info2 in files)
                    {
                        Header1DataClass class2;
                        PerEnvReport report;
                        ReportElements elements;
                        StreamReader reader = info2.OpenText();
                        string name = info2.Name;
                        string pattern = @"_(com|COM|tcp|TCP|i2c|I2C)\d+_(reset)*(?<testName>\w+)_(?<cno>-?\d+\.*\d?)";
                        Regex regex = new Regex(pattern, RegexOptions.Compiled);
                        bool flag = regex.IsMatch(name);
                        string key = "Unknown";
                        string str5 = "Unknown";
                        if (flag)
                        {
                            key = regex.Match(name).Result("${cno}");
                            str5 = regex.Match(name).Result("${testName}");
                        }
                        string str6 = string.Empty;
                        string input = reader.ReadLine();
                        if (input != null)
                        {
                            pattern = @"SW Version=\s*(?<swVer>.*)";
                            Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                            if (regex2.IsMatch(input))
                            {
                                str6 = regex2.Match(input).Result("${swVer}");
                            }
                            else
                            {
                                str6 = "SW Version= Not detected";
                            }
                        }
                        else
                        {
                            str6 = "SW Version= Not detected";
                        }
                        if (hashtable.Contains(str6))
                        {
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        else
                        {
                            Header1DataClass class3 = new Header1DataClass();
                            hashtable.Add(str6, class3);
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        if (class2.Header2DataHash.Contains(str5))
                        {
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        else
                        {
                            PerEnvReport report2 = new PerEnvReport();
                            class2.Header2DataHash.Add(str5, report2);
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        if (report.PerSiteData.Contains(key))
                        {
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        else
                        {
                            ReportElements elements2 = new ReportElements();
                            report.PerSiteData.Add(key, elements2);
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        input = reader.ReadLine();
                        string str8 = "N/A";
                        Hashtable hashtable2 = new Hashtable();
                        int num9 = 0;
                        if (input != null)
                        {
                            string[] strArray = input.Split(new char[] { '=' });
                            if (strArray.Length > 1)
                            {
                                str8 = strArray[1];
                                if (this.perRxSetupData.ContainsKey(str8))
                                {
                                    goto Label_0323;
                                }
                                if (strArray.Length > 1)
                                {
                                    hashtable2.Add(strArray[0], strArray[1]);
                                }
                                while ((input = reader.ReadLine()) != null)
                                {
                                    if (input == "End Summary")
                                    {
                                        break;
                                    }
                                    if (input != string.Empty)
                                    {
                                        strArray = input.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable2.Add(strArray[0], strArray[1]);
                                        }
                                        num9++;
                                    }
                                }
                                this.perRxSetupData.Add(str8, hashtable2);
                            }
                        }
                        goto Label_032F;
                    Label_0315:
                        if (input == "End Summary")
                        {
                            goto Label_032F;
                        }
                    Label_0323:
                        if ((input = reader.ReadLine()) != null)
                        {
                            goto Label_0315;
                        }
                    Label_032F:
                        elements.RxName = str8;
                        input = reader.ReadLine();
                        for (input = reader.ReadLine(); input != null; input = reader.ReadLine())
                        {
                            string[] strArray2 = input.Split(new char[] { ',' });
                            double num10 = -9999.0;
                            if (strArray2.Length >= 5)
                            {
                                double num11 = Convert.ToDouble(strArray2[this._ttffReportType]);
                                double num12 = Convert.ToDouble(strArray2[index]);
                                double num13 = Convert.ToDouble(this._limitVal);
                                string[] strArray3 = this._hrErrLimit.Split(new char[] { ',' });
                                int num14 = -1;
                                try
                                {
                                    num14 = Convert.ToInt32(strArray2[num7]);
                                }
                                catch
                                {
                                }
                                if (strArray3.Length > 0)
                                {
                                    Convert.ToDouble(strArray3[strArray3.Length - 1]);
                                }
                                else
                                {
                                    Convert.ToDouble(strArray3[0]);
                                }
                                if (num11 <= 0.0)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num11 >= num13)
                                {
                                    elements.NumberOfMisses++;
                                }
                                double num15 = Convert.ToDouble(strArray2[num3]);
                                if (num15 != -9999.0)
                                {
                                    num10 = Math.Abs((double) (num15 - Convert.ToDouble(strArray2[num4])));
                                }
                                if ((num14 >= 0) && (num14 <= 3))
                                {
                                    elements.NumberOfSVInFix++;
                                }
                                try
                                {
                                    elements.TTFFSamples.InsertSample(num11);
                                    elements.Position2DErrorSamples.InsertSample(num12);
                                    elements.VerticalErrorSamples.InsertSample(num10);
                                    report.PerEnvSamples.TTFFSamples.InsertSample(num11);
                                    report.PerEnvSamples.Position2DErrorSamples.InsertSample(num12);
                                    report.PerEnvSamples.VerticalErrorSamples.InsertSample(num10);
                                    class2.ReportDataSamples.TTFFSamples.InsertSample(num11);
                                    class2.ReportDataSamples.Position2DErrorSamples.InsertSample(num12);
                                    class2.ReportDataSamples.VerticalErrorSamples.InsertSample(num10);
                                    for (int i = 0; i < num6; i++)
                                    {
                                        sample = Convert.ToDouble(strArray2[num5 + i]);
                                        if (sample != 0.0)
                                        {
                                            elements.CNOSamples.InsertSample(sample);
                                            report.PerEnvSamples.CNOSamples.InsertSample(sample);
                                            class2.ReportDataSamples.CNOSamples.InsertSample(sample);
                                        }
                                    }
                                }
                                catch (FormatException exception)
                                {
                                    exception.ToString();
                                }
                            }
                        }
                        reader.Close();
                    }
                    f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    f.WriteLine("<reset>");
                    int styleCt = 0;
                    List<string> il = new List<string>();
                    foreach (string str9 in hashtable.Keys)
                    {
                        Header1DataClass class4 = (Header1DataClass) hashtable[str9];
                        f.WriteLine("\t<swVersion name=\"{0}\">", str9);
                        foreach (string str10 in class4.Header2DataHash.Keys)
                        {
                            plots.PaneTitle = str9 + "\r\n" + str10;
                            plots.CreateCharts();
                            GraphPane myPane = plots.AddPane("TTFF Resets", "Secs");
                            GraphPane pane2 = plots.AddPane("2-D Error", "Meters");
                            GraphPane pane3 = plots.AddPane("Verical Error", "Meters");
                            PerEnvReport report3 = (PerEnvReport) class4.Header2DataHash[str10];
                            styleCt++;
                            string item = str10 + styleCt.ToString() + ".jpg";
                            string str12 = "Style" + styleCt.ToString();
                            il.Add(item);
                            f.WriteLine("\t\t<environment name=\"{0}\" plotImage=\"{1}\"  showpicturestyle=\"{2}\">", str10, item, str12);
                            ArrayList list2 = new ArrayList();
                            foreach (string str13 in report3.PerSiteData.Keys)
                            {
                                list2.Add(str13);
                            }
                            list2.Sort();
                            int num18 = 0;
                            foreach (string str14 in list2)
                            {
                                ReportElements elements3 = (ReportElements) report3.PerSiteData[str14];
                                f.WriteLine("\t\t\t<site number=\"{0}\">", str14);
                                this.printResetSummary(f, elements3.TTFFSamples.Samples, elements3.NumberOfMisses, elements3.NumberOfSVInFix);
                                this.printSampleData(f, elements3.TTFFSamples, this._percentile, this._ttffLimit, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                                plots.AddCurve(myPane, elements3.TTFFSamples, elements3.TTFFSamples.Samples, str14);
                                this.printSampleData(f, elements3.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                                plots.AddCurve(pane2, elements3.Position2DErrorSamples, elements3.Position2DErrorSamples.Samples, str14);
                                this.printSampleData(f, elements3.VerticalErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "Vertical Error", "m");
                                plots.AddCurve(pane3, elements3.VerticalErrorSamples, elements3.VerticalErrorSamples.Samples, str14);
                                this.printSampleData(f, elements3.CNOSamples, this._percentile, "", -9999.0, "CNO", "dbHz");
                                f.WriteLine("\t\t\t</site>");
                                num18++;
                            }
                            plots.RefreshGraph();
                            plots.SaveGraphToFile(dir, item);
                            plots.PaneTitle = string.Empty;
                            f.WriteLine("\t\t</environment>");
                        }
                        f.WriteLine("\t</swVersion>");
                    }
                    this.modifyResetReportCSS(dir, styleCt);
                    this.printTestSetup(f);
                    this.printImageList(f, il);
                    f.WriteLine("</reset>");
                    f.Close();
                    hashtable.Clear();
                    this.perRxSetupData.Clear();
                    string outputFile = dir + @"\summary_reset.html";
                    this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\resetReport.xsl");
                }
            }
            catch (Exception exception2)
            {
                this.perRxSetupData.Clear();
                hashtable.Clear();
                f.Close();
                MessageBox.Show(exception2.Message, "ERROR");
            }
        }

        public void Summary_Reset_V2(string dir)
        {
            int index = 8;
            int num2 = index + 1;
            int num3 = num2 + 3;
            int num4 = num3 + 3;
            int num5 = num4 + 5;
            int num6 = 12;
            int num7 = (num6 + num5) + 1;
            double sample = 0.0;
            StreamReader reader = null;
            try
            {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                if (files.Length != 0)
                {
                    foreach (FileInfo info2 in files)
                    {
                        string str8;
                        reader = info2.OpenText();
                        string name = info2.Name;
                        string pattern = @"_(com|COM|tcp|TCP|i2c|I2C)\d+_(reset)*(?<testName>\w+)_(?<cno>-?\d+\.*\d?)";
                        Regex regex = new Regex(pattern, RegexOptions.Compiled);
                        bool flag = regex.IsMatch(name);
                        string key = "Unknown";
                        string str4 = "Unknown";
                        if (flag)
                        {
                            key = regex.Match(name).Result("${cno}");
                            str4 = regex.Match(name).Result("${testName}");
                        }
                        string str5 = string.Empty;
                        string input = reader.ReadLine();
                        if (input != null)
                        {
                            pattern = @"SW Version\s*=\s*(?<swVer>.*)";
                            Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                            if (regex2.IsMatch(input))
                            {
                                str5 = regex2.Match(input).Result("${swVer}");
                            }
                            else
                            {
                                str5 = "SW Version: Not detected";
                            }
                        }
                        else
                        {
                            str5 = "SW Version: Not detected";
                        }
                        input = reader.ReadLine();
                        string str7 = "N/A";
                        Hashtable hashtable = new Hashtable();
                        int num9 = 0;
                        if (input != null)
                        {
                            string[] strArray = input.Split(new char[] { '=' });
                            if (strArray.Length > 1)
                            {
                                str7 = strArray[1];
                                if (this.perRxSetupData.ContainsKey(str7))
                                {
                                    goto Label_0217;
                                }
                                if (strArray.Length > 1)
                                {
                                    hashtable.Add(strArray[0], strArray[1]);
                                }
                                while ((input = reader.ReadLine()) != null)
                                {
                                    if (input == "End Summary")
                                    {
                                        break;
                                    }
                                    if (input != string.Empty)
                                    {
                                        strArray = input.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable.Add(strArray[0], strArray[1]);
                                        }
                                        num9++;
                                    }
                                }
                                this.perRxSetupData.Add(str7, hashtable);
                            }
                        }
                        goto Label_0223;
                    Label_0209:
                        if (input == "End Summary")
                        {
                            goto Label_0223;
                        }
                    Label_0217:
                        if ((input = reader.ReadLine()) != null)
                        {
                            goto Label_0209;
                        }
                    Label_0223:
                        str8 = string.Empty;
                        Header1DataClass class2 = null;
                        PerEnvReport report = null;
                        switch (this.ReportLayout)
                        {
                            case ReportLayoutType.GroupBySWVersion:
                                str8 = key;
                                if (!this.reportDataHash.Contains(str5))
                                {
                                    Header1DataClass class3 = new Header1DataClass();
                                    this.reportDataHash.Add(str5, class3);
                                }
                                class2 = (Header1DataClass) this.reportDataHash[str5];
                                if (!class2.Header2DataHash.Contains(str4))
                                {
                                    PerEnvReport report2 = new PerEnvReport();
                                    class2.Header2DataHash.Add(str4, report2);
                                }
                                report = (PerEnvReport) class2.Header2DataHash[str4];
                                if (!report.PerSiteData.Contains(key))
                                {
                                    ReportElements elements = new ReportElements();
                                    report.PerSiteData.Add(key, elements);
                                }
                                class2.Header2DataHash[str4] = report;
                                this.reportDataHash[str5] = class2;
                                break;

                            case ReportLayoutType.GroupByResetType:
                                str8 = str7;
                                if (!this.reportDataHash.Contains(str4))
                                {
                                    Header1DataClass class4 = new Header1DataClass();
                                    this.reportDataHash.Add(str4, class4);
                                }
                                class2 = (Header1DataClass) this.reportDataHash[str4];
                                if (!class2.Header2DataHash.Contains(key))
                                {
                                    PerEnvReport report3 = new PerEnvReport();
                                    class2.Header2DataHash.Add(key, report3);
                                }
                                report = (PerEnvReport) class2.Header2DataHash[key];
                                if (!report.PerSiteData.Contains(str7))
                                {
                                    ReportElements elements2 = new ReportElements();
                                    report.PerSiteData.Add(str7, elements2);
                                }
                                class2.Header2DataHash[key] = report;
                                this.reportDataHash[str4] = class2;
                                break;

                            default:
                                str8 = string.Empty;
                                break;
                        }
                        if (str8 != string.Empty)
                        {
                            ReportElements elements3 = (ReportElements) report.PerSiteData[str8];
                            input = reader.ReadLine();
                            for (input = reader.ReadLine(); input != null; input = reader.ReadLine())
                            {
                                string[] strArray2 = input.Split(new char[] { ',' });
                                double num10 = -9999.0;
                                if (strArray2.Length >= 5)
                                {
                                    double num11 = Convert.ToDouble(strArray2[this._ttffReportType]);
                                    double num12 = Convert.ToDouble(strArray2[index]);
                                    double num13 = Convert.ToDouble(this._limitVal);
                                    string[] strArray3 = this._hrErrLimit.Split(new char[] { ',' });
                                    int num14 = -1;
                                    try
                                    {
                                        num14 = Convert.ToInt32(strArray2[num7]);
                                    }
                                    catch
                                    {
                                    }
                                    if (strArray3.Length > 0)
                                    {
                                        Convert.ToDouble(strArray3[strArray3.Length - 1]);
                                    }
                                    else
                                    {
                                        Convert.ToDouble(strArray3[0]);
                                    }
                                    if (num11 <= 0.0)
                                    {
                                        elements3.NumberOfMisses++;
                                    }
                                    else if (num11 >= num13)
                                    {
                                        elements3.NumberOfMisses++;
                                    }
                                    double num15 = Convert.ToDouble(strArray2[num3]);
                                    if (num15 != -9999.0)
                                    {
                                        num10 = Math.Abs((double) (num15 - Convert.ToDouble(strArray2[num4])));
                                    }
                                    if ((num14 >= 0) && (num14 <= 3))
                                    {
                                        elements3.NumberOfSVInFix++;
                                    }
                                    try
                                    {
                                        elements3.TTFFSamples.InsertSample(num11);
                                        elements3.Position2DErrorSamples.InsertSample(num12);
                                        elements3.VerticalErrorSamples.InsertSample(num10);
                                        for (int i = 0; i < num6; i++)
                                        {
                                            sample = Convert.ToDouble(strArray2[num5 + i]);
                                            if (sample != 0.0)
                                            {
                                                elements3.CNOSamples.InsertSample(sample);
                                            }
                                        }
                                    }
                                    catch (FormatException exception)
                                    {
                                        exception.ToString();
                                    }
                                }
                            }
                            reader.Close();
                        }
                    }
                    this.writeResetSummaryXMLFile(dir, this.ReportLayout);
                }
            }
            catch (Exception exception2)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                this.perRxSetupData.Clear();
                this.reportDataHash.Clear();
                MessageBox.Show(exception2.Message, "ERROR");
            }
        }

        public void Summary_TIA916(string dir)
        {
            Hashtable hashtable = new Hashtable();
            int index = 8;
            int num2 = index + 1;
            int num3 = num2 + 3;
            int num4 = num3 + 3;
            int num5 = num4 + 5;
            int num6 = 12;
            int num7 = (num6 + num5) + 1;
            double sample = 0.0;
            string path = dir + @"\summary_TIA916.xml";
            StreamWriter f = new StreamWriter(path);
            try
            {
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.csv");
                if (files.Length != 0)
                {
                    foreach (FileInfo info2 in files)
                    {
                        Header1DataClass class2;
                        PerEnvReport report;
                        ReportElements elements;
                        StreamReader reader = info2.OpenText();
                        string name = info2.Name;
                        string pattern = @"_(com|COM|tcp|TCP|i2c|I2C)\d+_(?<testName>\w+)_(?<margin>\d+p\d?)";
                        Regex regex = new Regex(pattern, RegexOptions.Compiled);
                        bool flag = regex.IsMatch(name);
                        string key = "Unknown";
                        string str5 = "Unknown";
                        if (flag)
                        {
                            key = regex.Match(name).Result("${margin}").Replace('p', '.');
                            str5 = regex.Match(name).Result("${testName}");
                        }
                        string str6 = string.Empty;
                        string input = reader.ReadLine();
                        if (input != null)
                        {
                            pattern = @"SW Version\s*=\s*(?<swVer>.*)";
                            Regex regex2 = new Regex(pattern, RegexOptions.Compiled);
                            if (regex2.IsMatch(input))
                            {
                                str6 = regex2.Match(input).Result("${swVer}");
                            }
                            else
                            {
                                str6 = "SW Version= Not detected";
                            }
                        }
                        else
                        {
                            str6 = "SW Version= Not detected";
                        }
                        if (hashtable.Contains(str6))
                        {
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        else
                        {
                            Header1DataClass class3 = new Header1DataClass();
                            hashtable.Add(str6, class3);
                            class2 = (Header1DataClass) hashtable[str6];
                        }
                        if (class2.Header2DataHash.Contains(str5))
                        {
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        else
                        {
                            PerEnvReport report2 = new PerEnvReport();
                            class2.Header2DataHash.Add(str5, report2);
                            report = (PerEnvReport) class2.Header2DataHash[str5];
                        }
                        if (report.PerSiteData.Contains(key))
                        {
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        else
                        {
                            ReportElements elements2 = new ReportElements();
                            report.PerSiteData.Add(key, elements2);
                            elements = (ReportElements) report.PerSiteData[key];
                        }
                        this.setTIA916_2DErrorLimit(str5);
                        input = reader.ReadLine();
                        string str8 = "N/A";
                        Hashtable hashtable2 = new Hashtable();
                        int num9 = 0;
                        if (input != null)
                        {
                            string[] strArray = input.Split(new char[] { '=' });
                            if (strArray.Length > 1)
                            {
                                str8 = strArray[1] + "_" + str5;
                                if (this.perRxSetupData.ContainsKey(str8))
                                {
                                    goto Label_0356;
                                }
                                hashtable2.Add("Test", str5);
                                if (strArray.Length > 1)
                                {
                                    hashtable2.Add(strArray[0], strArray[1]);
                                }
                                while ((input = reader.ReadLine()) != null)
                                {
                                    if (input == "End Summary")
                                    {
                                        break;
                                    }
                                    if (input != string.Empty)
                                    {
                                        strArray = input.Split(new char[] { '=' });
                                        if (strArray.Length > 1)
                                        {
                                            hashtable2.Add(strArray[0], strArray[1]);
                                        }
                                        num9++;
                                    }
                                }
                                this.perRxSetupData.Add(str8, hashtable2);
                            }
                        }
                        goto Label_0362;
                    Label_0334:
                        if (input == "End Summary")
                        {
                            goto Label_0362;
                        }
                        if (!(input == string.Empty))
                        {
                            num9++;
                        }
                    Label_0356:
                        if ((input = reader.ReadLine()) != null)
                        {
                            goto Label_0334;
                        }
                    Label_0362:
                        input = reader.ReadLine();
                        for (input = reader.ReadLine(); input != null; input = reader.ReadLine())
                        {
                            string[] strArray2 = input.Split(new char[] { ',' });
                            double num10 = -9999.0;
                            if (strArray2.Length >= 5)
                            {
                                double num11 = Convert.ToDouble(strArray2[this._ttffReportType]);
                                double num12 = Convert.ToDouble(strArray2[index]);
                                double num13 = Convert.ToDouble(this._limitVal);
                                double num14 = 0.0;
                                string[] strArray3 = this._hrErrLimit.Split(new char[] { ',' });
                                int num15 = -1;
                                try
                                {
                                    num15 = Convert.ToInt32(strArray2[num7]);
                                }
                                catch
                                {
                                }
                                if (str5.Contains("TEST5"))
                                {
                                    num13 = 240.0;
                                }
                                if (strArray3.Length > 0)
                                {
                                    num14 = Convert.ToDouble(strArray3[strArray3.Length - 1]);
                                }
                                else
                                {
                                    num14 = Convert.ToDouble(strArray3[0]);
                                }
                                if (num11 <= 0.0)
                                {
                                    elements.NumberOfTTFFMisses++;
                                }
                                else if (num11 == Convert.ToDouble(this._timeoutVal))
                                {
                                    elements.NumberOfTTFFMisses++;
                                }
                                else if (num11 > num13)
                                {
                                    elements.NumberOfTTFFOutOfLimits++;
                                }
                                if (num12 <= 0.0)
                                {
                                    elements.NumberOf2DMisses++;
                                }
                                if (num12 > num14)
                                {
                                    elements.NumberOf2DOutOfLimits++;
                                }
                                if (num11 <= 0.0)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num11 > num13)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num12 <= 0.0)
                                {
                                    elements.NumberOfMisses++;
                                }
                                else if (num12 > num14)
                                {
                                    elements.NumberOfMisses++;
                                }
                                double num16 = Convert.ToDouble(strArray2[num3]);
                                if (num16 != -9999.0)
                                {
                                    num10 = Math.Abs((double) (num16 - Convert.ToDouble(strArray2[num4])));
                                }
                                if ((num15 >= 0) && (num15 <= 3))
                                {
                                    elements.NumberOfSVInFix++;
                                }
                                elements.TTFFSamples.InsertSample(num11);
                                elements.Position2DErrorSamples.InsertSample(num12);
                                elements.VerticalErrorSamples.InsertSample(num10);
                                report.PerEnvSamples.TTFFSamples.InsertSample(num11);
                                report.PerEnvSamples.Position2DErrorSamples.InsertSample(num12);
                                report.PerEnvSamples.VerticalErrorSamples.InsertSample(num10);
                                class2.ReportDataSamples.TTFFSamples.InsertSample(num11);
                                class2.ReportDataSamples.Position2DErrorSamples.InsertSample(num12);
                                class2.ReportDataSamples.VerticalErrorSamples.InsertSample(num10);
                                for (int i = 0; i < num6; i++)
                                {
                                    sample = Convert.ToDouble(strArray2[num5 + i]);
                                    if (sample != 0.0)
                                    {
                                        elements.CNOSamples.InsertSample(sample);
                                        report.PerEnvSamples.CNOSamples.InsertSample(sample);
                                        class2.ReportDataSamples.CNOSamples.InsertSample(sample);
                                    }
                                }
                            }
                        }
                        reader.Close();
                    }
                    f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    f.WriteLine("<TIA916>");
                    string limitStr = this._ttffLimit;
                    foreach (string str10 in hashtable.Keys)
                    {
                        Header1DataClass class4 = (Header1DataClass) hashtable[str10];
                        f.WriteLine("\t<swVersion name=\"{0}\">", str10);
                        int item = 0;
                        foreach (string str11 in class4.Header2DataHash.Keys)
                        {
                            this.setTIA916_2DErrorLimit(str11);
                            if (str11.Contains("TEST1"))
                            {
                                item = 0;
                            }
                            else if (str11.Contains("TEST2"))
                            {
                                item = 1;
                            }
                            else if (str11.Contains("TEST3"))
                            {
                                item = 2;
                            }
                            else if (str11.Contains("TEST4"))
                            {
                                item = 3;
                            }
                            else if (str11.Contains("TEST5"))
                            {
                                item = 4;
                                limitStr = "240";
                            }
                            PerEnvReport report3 = (PerEnvReport) class4.Header2DataHash[str11];
                            f.WriteLine("\t\t<environment name=\"{0}\">", str11);
                            ArrayList list = new ArrayList();
                            foreach (string str12 in report3.PerSiteData.Keys)
                            {
                                list.Add(str12);
                            }
                            list.Sort();
                            foreach (string str13 in list)
                            {
                                ReportElements elements3 = (ReportElements) report3.PerSiteData[str13];
                                f.WriteLine("\t\t\t<site number=\"{0}\">", str13);
                                List<int> data = new List<int>();
                                data.Add(item);
                                data.Add(elements3.NumberOfSVInFix);
                                data.Add(elements3.NumberOfMisses);
                                data.Add(elements3.NumberOfTTFFMisses);
                                data.Add(elements3.NumberOfTTFFOutOfLimits);
                                data.Add(elements3.NumberOf2DMisses);
                                data.Add(elements3.NumberOf2DOutOfLimits);
                                this.printTIA916Summary(f, elements3.Position2DErrorSamples, elements3.TTFFSamples, data);
                                this.printSampleData(f, elements3.TTFFSamples, this._percentile, limitStr, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                                this.printSampleData(f, elements3.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                                this.printSampleData(f, elements3.VerticalErrorSamples, this._percentile, "", -9999.0, "Vertical Error", "m");
                                this.printSampleData(f, elements3.CNOSamples, this._percentile, "", -9999.0, "CNO", "dbHz");
                                f.WriteLine("\t\t\t</site>");
                            }
                            f.WriteLine("\t\t</environment>");
                        }
                        f.WriteLine("\t</swVersion>");
                    }
                    this.printTestSetup(f);
                    f.WriteLine("</TIA916>");
                    f.Close();
                    hashtable.Clear();
                    this.perRxSetupData.Clear();
                    string outputFile = dir + @"\summary_TIA916.html";
                    this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\TIA916\TIA916Report.xsl");
                }
            }
            catch (Exception exception)
            {
                this.perRxSetupData.Clear();
                hashtable.Clear();
                f.Close();
                MessageBox.Show(exception.Message, "ERROR");
            }
        }

        internal void TestSplitFunction()
        {
            StreamReader reader = new StreamReader(@"R:\Software\STAR_Test_Data\SiRFLive\capture1\10212008_175803\08_com6_Site17B_04242008.gp2");
            StreamWriter writer = new StreamWriter(@"R:\Software\STAR_Test_Data\SiRFLive\capture1\10212008_175803\test.gp2");
            string[] separator = new string[] { "(0)" };
            string[] strArray2 = new string[] { "B0 B3 A0 A2" };
            for (string str = reader.ReadLine(); str != null; str = reader.ReadLine())
            {
                string[] strArray3 = str.Split(separator, StringSplitOptions.None);
                string[] strArray4 = str.Split(strArray2, StringSplitOptions.None);
                if (strArray4.Length > 1)
                {
                    writer.WriteLine(strArray4[0] + "B0 B3\n");
                    writer.WriteLine(strArray3[0] + "(0)\tA0 A2" + strArray4[1] + "\n");
                }
                else
                {
                    writer.WriteLine(str);
                }
            }
            writer.Close();
        }

        public void Tunnels_Summary(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    MessageBox.Show(string.Format("Directory Not Found!\n\n{0}", dir), "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    DirectoryInfo info = new DirectoryInfo(dir);
                    FileInfo[] files = info.GetFiles("*.csv");
                    if (files.Length != 0)
                    {
                        Stats s = new Stats();
                        Stats stats2 = new Stats();
                        Stats stats3 = new Stats();
                        string str = "SW Version: Not detected";
                        foreach (FileInfo info2 in files)
                        {
                            LogManager manager = new LogManager();
                            manager.OpenFileRead(info.FullName + info2.Name);
                            string str2 = manager.ReadLine();
                            for (str2 = manager.ReadLine(); str2 != null; str2 = manager.ReadLine())
                            {
                                string[] strArray = str2.Split(new char[] { ',' });
                                if (strArray.Length >= 11)
                                {
                                    int num = Convert.ToInt32(strArray[8]);
                                    int num2 = Convert.ToInt32(strArray[9]);
                                    int num3 = Convert.ToInt32(strArray[10]);
                                    s.InsertSample((double) num);
                                    stats2.InsertSample((double) num2);
                                    stats3.InsertSample((double) num3);
                                }
                            }
                            manager.CloseFile();
                        }
                        string path = dir + @"\summary_tunnels.xml";
                        StreamWriter f = new StreamWriter(path);
                        f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        f.WriteLine("<tunnels>");
                        f.WriteLine("\t<swVersion name=\"{0}\">", str);
                        this.printCDFHelper(f, s, "Time to Lose One SV");
                        this.printCDFHelper(f, stats2, "Time to Lose All SVs");
                        this.printCDFHelper(f, stats3, "Time to Re-Acquire Signal");
                        f.WriteLine("\t</swVersion>");
                        f.WriteLine("</tunnels>");
                        f.Close();
                        string outputFile = dir + @"\summary_tunnels.html";
                        this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\tunnelsReport.xsl");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
        }

        private static void UpdateMsgCountTotals(StreamReader xmlSr)
        {
            string str = xmlSr.ReadLine();
            while (!xmlSr.EndOfStream)
            {
                str = xmlSr.ReadLine();
                if (str.IndexOf("midID_subID number") > 0)
                {
                    string[] strArray = str.Split(new string[] { ",", "\"" }, StringSplitOptions.RemoveEmptyEntries);
                    uint md = Convert.ToUInt32(strArray[1]);
                    uint sb = Convert.ToUInt32(strArray[2]);
                    uint num3 = Convert.ToUInt32(xmlSr.ReadLine().Split(new string[] { ",", "\"" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    int num4 = MidSubIDMsgCountsIndexOf(md, sb);
                    if (num4 >= 0)
                    {
                        messageCounts local1 = MsgCounts[num4];
                        local1.count += num3;
                    }
                }
            }
        }

        private static void WriteOutProtocolTestResults(StreamReader PSr, StreamWriter PSw)
        {
            MSIDChangeStates bgTestResults = MSIDChangeStates.BgTestResults;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            string str4 = "    </midID_subID>";
            bool flag = false;
            for (string str5 = PSr.ReadLine(); !PSr.EndOfStream; str5 = PSr.ReadLine())
            {
                string[] strArray = str5.Split(new string[] { ",", ":" }, StringSplitOptions.RemoveEmptyEntries);
                num = Convert.ToInt32(strArray[0]);
                num3 = Convert.ToInt32(strArray[1]);
                string str = strArray[2];
                if (strArray[3].Trim() == "untested")
                {
                    if (flag)
                    {
                        PSw.WriteLine(str4);
                        flag = false;
                    }
                    string str6 = string.Format("    <midID_subID number=\"{0},{1}\" name=\"{2}\">", num, num3, str);
                    PSw.WriteLine(str6);
                    string str7 = string.Format("           <field name=\"Normal\" value=\"Not tested\"/>", new object[0]);
                    PSw.WriteLine(str7);
                    str7 = string.Format("           <field name=\"Invalid Checksum\" value=\"Not tested\"/>", new object[0]);
                    PSw.WriteLine(str7);
                    str7 = string.Format("           <field name=\"Payload Too Short\" value=\"Not tested\"/>", new object[0]);
                    PSw.WriteLine(str7);
                    str7 = string.Format("           <field name=\"Payload Too Long\" value=\"Not tested\"/>", new object[0]);
                    PSw.WriteLine(str7);
                    str7 = string.Format("           <field name=\"Invalid Trailer\" value=\"Not tested\"/>", new object[0]);
                    PSw.WriteLine(str7);
                    str7 = string.Format("           <field name=\"Invalid Header\" value=\"Not tested\"/>", new object[0]);
                    PSw.WriteLine(str7);
                    str7 = string.Format("           <field name=\"Invalid ID\" value=\"Not tested\"/>", new object[0]);
                    PSw.WriteLine(str7);
                    PSw.WriteLine(str4);
                }
                else
                {
                    string str2;
                    string str3;
                    if (((num2 != num) || (num4 != num3)) && (bgTestResults == MSIDChangeStates.BgTestResults))
                    {
                        string str8 = string.Format("    <midID_subID number=\"{0},{1}\" name=\"{2}\">", num, num3, str);
                        PSw.WriteLine(str8);
                        str2 = strArray[4].Trim();
                        str3 = strArray[6].Trim();
                        string str9 = string.Format("           <field name=\"{0}\" value=\"{1}\"/>", str2, str3);
                        PSw.WriteLine(str9);
                        bgTestResults = MSIDChangeStates.CmptAndBgTestResult;
                        num2 = num;
                        num4 = num3;
                    }
                    else if ((num2 == num) && (num4 == num3))
                    {
                        str2 = strArray[4].Trim();
                        str3 = strArray[6].Trim();
                        string str10 = string.Format("           <field name=\"{0}\" value=\"{1}\"/>", str2, str3);
                        PSw.WriteLine(str10);
                    }
                    else if (((num2 != num) || (num4 != num3)) && (bgTestResults == MSIDChangeStates.CmptAndBgTestResult))
                    {
                        PSw.WriteLine(str4);
                        string str11 = string.Format("    <midID_subID number=\"{0},{1}\" name=\"{2}\">", num, num3, str);
                        PSw.WriteLine(str11);
                        str2 = strArray[4].Trim();
                        str3 = strArray[6].Trim();
                        string str12 = string.Format("           <field name=\"{0}\" value=\"{1}\"/>", str2, str3);
                        PSw.WriteLine(str12);
                        flag = true;
                        num2 = num;
                        num4 = num3;
                    }
                }
            }
        }

        private void writeResetSummaryXMLFile(string dir, ReportLayoutType rFormat)
        {
            frmResetTestReportPlots plots = new frmResetTestReportPlots();
            ZedGraphControl control = new ZedGraphControl();
            string path = dir + @"\SummaryReport.xml";
            switch (rFormat)
            {
                case ReportLayoutType.GroupBySWVersion:
                    path = dir + @"\PerSW_SummaryReport.xml";
                    break;

                case ReportLayoutType.GroupByResetType:
                    path = dir + @"\PerResetType_SummaryReport.xml";
                    break;
            }
            StreamWriter f = new StreamWriter(path);
            try
            {
                f.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                f.WriteLine("<reset>");
                int styleCt = 0;
                List<string> il = new List<string>();
                foreach (string str2 in this.reportDataHash.Keys)
                {
                    Header1DataClass class2 = (Header1DataClass) this.reportDataHash[str2];
                    f.WriteLine("\t<swVersion name=\"{0}\">", str2);
                    ArrayList list2 = new ArrayList();
                    foreach (string str3 in class2.Header2DataHash.Keys)
                    {
                        list2.Add(str3);
                    }
                    list2.Sort();
                    foreach (string str4 in list2)
                    {
                        plots.PaneTitle = str2 + "\r\n" + str4;
                        plots.CreateCharts();
                        GraphPane myPane = plots.AddPane("TTFF Resets", "Secs");
                        GraphPane pane2 = plots.AddPane("2-D Error", "Meters");
                        GraphPane pane3 = plots.AddPane("Verical Error", "Meters");
                        PerEnvReport report = (PerEnvReport) class2.Header2DataHash[str4];
                        if (this.ReportLayout == ReportLayoutType.GroupBySWVersion)
                        {
                            styleCt++;
                        }
                        else
                        {
                            styleCt = 1;
                        }
                        string item = str4 + styleCt.ToString() + ".jpg";
                        string str6 = "Style" + styleCt.ToString();
                        il.Add(item);
                        f.WriteLine("\t\t<environment name=\"{0}\" plotImage=\"{1}\"  showpicturestyle=\"{2}\">", str4, item, str6);
                        ArrayList list3 = new ArrayList();
                        foreach (string str7 in report.PerSiteData.Keys)
                        {
                            list3.Add(str7);
                        }
                        list3.Sort();
                        int num2 = 0;
                        foreach (string str8 in list3)
                        {
                            ReportElements elements = (ReportElements) report.PerSiteData[str8];
                            f.WriteLine("\t\t\t<site number=\"{0}\">", str8);
                            this.printResetSummary(f, elements.TTFFSamples.Samples, elements.NumberOfMisses, elements.NumberOfSVInFix);
                            this.printSampleData(f, elements.TTFFSamples, this._percentile, this._ttffLimit, Convert.ToDouble(this._timeoutVal), "TTFF", "sec");
                            this.printSampleData(f, elements.Position2DErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "2-D Error", "m");
                            this.printSampleData(f, elements.VerticalErrorSamples, this._percentile, this._hrErrLimit, -9999.0, "Vertical Error", "m");
                            this.printSampleData(f, elements.CNOSamples, this._percentile, "", -9999.0, "CNO", "dbHz");
                            f.WriteLine("\t\t\t</site>");
                            if (rFormat == ReportLayoutType.GroupBySWVersion)
                            {
                                plots.AddCurve(myPane, elements.TTFFSamples, elements.TTFFSamples.Samples, str8);
                                plots.AddCurve(pane2, elements.Position2DErrorSamples, elements.Position2DErrorSamples.Samples, str8);
                                plots.AddCurve(pane3, elements.VerticalErrorSamples, elements.VerticalErrorSamples.Samples, str8);
                            }
                            num2++;
                        }
                        if (rFormat == ReportLayoutType.GroupBySWVersion)
                        {
                            plots.RefreshGraph();
                            plots.SaveGraphToFile(dir, item);
                            plots.PaneTitle = string.Empty;
                        }
                        f.WriteLine("\t\t</environment>");
                        myPane = null;
                        pane2 = null;
                        pane3 = null;
                    }
                    f.WriteLine("\t</swVersion>");
                }
                this.printTestSetup(f);
                if (rFormat == ReportLayoutType.GroupBySWVersion)
                {
                    this.modifyResetReportCSS(dir, styleCt);
                    this.printImageList(f, il);
                }
                f.WriteLine("</reset>");
                f.Close();
                this.reportDataHash.Clear();
                this.perRxSetupData.Clear();
                string outputFile = path.Replace(".xml", ".html");
                this.GenerateHTMLReport(path, outputFile, ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts\resetReport.xsl");
                plots.Dispose();
                control.Dispose();
            }
            catch (Exception exception)
            {
                this.perRxSetupData.Clear();
                this.perRxSetupData.Clear();
                plots.Dispose();
                control.Dispose();
                if (f != null)
                {
                    f.Close();
                }
                MessageBox.Show(exception.Message, "Report Generate Error");
            }
        }

        public string HrErrLimit
        {
            get
            {
                return this._hrErrLimit;
            }
            set
            {
                this._hrErrLimit = value;
            }
        }

        public string InvalidSV
        {
            get
            {
                return this._invalidSV;
            }
            set
            {
                this._invalidSV = value;
            }
        }

        public string LimitVal
        {
            get
            {
                return this._limitVal;
            }
            set
            {
                this._limitVal = value;
            }
        }

        public string Percentile
        {
            get
            {
                return this._percentile;
            }
            set
            {
                this._percentile = value;
            }
        }

        public string TimeoutVal
        {
            get
            {
                return this._timeoutVal;
            }
            set
            {
                this._timeoutVal = value;
            }
        }

        public string TTFFLimit
        {
            get
            {
                return this._ttffLimit;
            }
            set
            {
                this._ttffLimit = value;
            }
        }

        public int TTFFReportType
        {
            get
            {
                return this._ttffReportType;
            }
            set
            {
                this._ttffReportType = value;
            }
        }

        public class Header1DataClass : IDisposable
        {
            public Hashtable Header2DataHash = new Hashtable();
            private bool isDisposed;
            public Report.ReportElements ReportDataSamples = new Report.ReportElements();

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed && disposing)
                {
                    this.Header2DataHash.Clear();
                    this.Header2DataHash = null;
                    this.ReportDataSamples.Dispose();
                    this.ReportDataSamples = null;
                }
                this.isDisposed = true;
            }

            ~Header1DataClass()
            {
                this.Dispose(true);
            }
        }

        public class Invalid_Data : IDisposable
        {
            public int EpochCount;
            public int HrErrCount;
            public int InvalidSVsCount;
            private bool isDisposed;
            public Stats MyStats = new Stats();

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed && disposing)
                {
                    this.MyStats.Dispose();
                    this.MyStats = null;
                }
                this.isDisposed = true;
            }

            ~Invalid_Data()
            {
                this.Dispose(true);
            }
        }

        public class messageChCounts
        {
            public uint[] channelIDCount;
            public uint mid;
            public string name;
            public uint subID;

            public messageChCounts(uint md, uint sb, string nm)
            {
                this.channelIDCount = new uint[10];
                this.mid = md;
                this.subID = sb;
                this.name = nm;
            }

            public messageChCounts(uint ch, uint md, uint sb)
            {
                this.channelIDCount = new uint[10];
                int channelIDIndex = Report.GetChannelIDIndex(ch);
                this.channelIDCount[channelIDIndex]++;
                this.mid = md;
                this.subID = sb;
            }
        }

        public class messageCounts
        {
            public uint count;
            public uint mid;
            public string name;
            public uint subID;
            public bool subIDFlag;

            public messageCounts(uint md)
            {
                this.count++;
                this.mid = md;
                this.subIDFlag = false;
            }

            public messageCounts(uint md, string nm)
            {
                this.mid = md;
                this.name = nm;
                this.subIDFlag = false;
            }

            public messageCounts(uint md, uint sb)
            {
                this.count++;
                this.mid = md;
                this.subID = sb;
                this.subIDFlag = true;
            }

            public messageCounts(uint md, uint sb, string nm)
            {
                this.mid = md;
                this.subID = sb;
                this.name = nm;
                this.subIDFlag = true;
            }
        }

        private enum MSIDChangeStates
        {
            BgTestResults,
            CmptAndBgTestResult
        }

        public class PerEnvReport : IDisposable
        {
            private bool isDisposed;
            public Report.ReportElements PerEnvSamples = new Report.ReportElements();
            public Hashtable PerSiteData = new Hashtable();

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed && disposing)
                {
                    this.PerSiteData.Clear();
                    this.PerSiteData = null;
                    this.PerEnvSamples.Dispose();
                    this.PerEnvSamples = null;
                }
                this.isDisposed = true;
            }

            ~PerEnvReport()
            {
                this.Dispose(true);
            }
        }

        public class PerNavStateReport : IDisposable
        {
            private bool isDisposed;
            public Hashtable PerMsgData = new Hashtable();
            public Report.ReportElements PerNavStateSamples = new Report.ReportElements();

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed && disposing)
                {
                    this.PerMsgData.Clear();
                    this.PerMsgData = null;
                    this.PerNavStateSamples.Dispose();
                    this.PerNavStateSamples = null;
                }
                this.isDisposed = true;
            }

            ~PerNavStateReport()
            {
                this.Dispose(true);
            }
        }

        public class PerPowerReport : IDisposable
        {
            private bool isDisposed;
            public Hashtable PerFreqData = new Hashtable();
            public Report.ReportElements PerFreqSamples = new Report.ReportElements();

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed && disposing)
                {
                    this.PerFreqData.Clear();
                    this.PerFreqData = null;
                    this.PerFreqSamples.Dispose();
                    this.PerFreqSamples = null;
                }
                this.isDisposed = true;
            }

            ~PerPowerReport()
            {
                this.Dispose(true);
            }
        }

        public class PerRxReport : IDisposable
        {
            private bool isDisposed;
            public Hashtable PerNavStateData = new Hashtable();
            public Report.ReportElements PerRxSamples = new Report.ReportElements();

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed && disposing)
                {
                    this.PerNavStateData.Clear();
                    this.PerNavStateData = null;
                    this.PerRxSamples.Dispose();
                    this.PerRxSamples = null;
                }
                this.isDisposed = true;
            }

            ~PerRxReport()
            {
                this.Dispose(true);
            }
        }

        public class ReportElements : IDisposable
        {
            public Stats CNOSamples = new Stats();
            public Hashtable ElementHash = new Hashtable();
            private bool isDisposed;
            public int NumberOf2DMisses;
            public int NumberOf2DOutOfLimits;
            public int NumberOfMisses;
            public int NumberOfSVInFix;
            public int NumberOfTTFFMisses;
            public int NumberOfTTFFOutOfLimits;
            public Stats Position2DErrorSamples = new Stats();
            public Stats Position3DErrorSamples = new Stats();
            public string RxName = "Unknown";
            public Stats TTFFSamples = new Stats();
            public Stats VerticalErrorSamples = new Stats();

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed && disposing)
                {
                    this.TTFFSamples.Dispose();
                    this.TTFFSamples = null;
                    this.Position2DErrorSamples.Dispose();
                    this.Position2DErrorSamples = null;
                    this.Position3DErrorSamples.Dispose();
                    this.Position3DErrorSamples = null;
                    this.VerticalErrorSamples.Dispose();
                    this.VerticalErrorSamples = null;
                    this.CNOSamples.Dispose();
                    this.CNOSamples = null;
                }
                this.isDisposed = true;
            }

            ~ReportElements()
            {
                this.Dispose(true);
            }
        }

        public enum ReportLayoutType
        {
            GroupBySWVersion,
            GroupByResetType,
            All
        }

        public class SDOStatsElememt
        {
            public string Comparison = "<";
            public string ComparisonResult = "N/A";
            public string Target = "N/A";
            public string Value = "0";

            public void Init()
            {
                this.Target = "N/A";
                this.Value = "0";
                this.Comparison = "<";
                this.ComparisonResult = "N/A";
            }
        }
    }
}

