﻿namespace SiRFLive.Configuration
{
    using System;
    using System.IO;
    using System.Xml;

    public class Ini_GUI
    {
        private XmlDocument _IniGuiXMLDocument;
        private string _InitGuiXmlFile;

        public Ini_GUI(string xmlFile)
        {
            try
            {
                this._InitGuiXmlFile = xmlFile;
                this._IniGuiXMLDocument = new XmlDocument();
                this._IniGuiXMLDocument.Load(xmlFile);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public string LoadControlDataFromFile(string windowName, string controlName)
        {
            string str = "";
            XmlNode node = this._IniGuiXMLDocument.SelectSingleNode("/GUI/" + windowName + "/control[@name='" + controlName + "']");
            try
            {
                str = node.Attributes["value"].Value;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str;
        }

        public void SaveControlDataToFile(string windowName, string controlName, string controlValue)
        {
            if ((File.GetAttributes(this._InitGuiXmlFile) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
            {
                XmlNode node = this._IniGuiXMLDocument.SelectSingleNode("/GUI/" + windowName + "/control[@name='" + controlName + "']");
                try
                {
                    node.Attributes["value"].Value = controlValue;
                    this._IniGuiXMLDocument.Save(this._InitGuiXmlFile);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }
    }
}

