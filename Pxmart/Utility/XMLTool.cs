using System;
using System.Collections.Generic;
using System.Xml;

namespace Pxmart.Utility
{
    public class XMLTool
    {
        private XmlDocument _xml = new XmlDocument();
        private string _xmlPath = string.Empty;

        public XMLTool(string xmlPath)
        {
            try
            {
                _xmlPath = xmlPath;
                _xml.Load(xmlPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetNodeInnerText(string nodePath, string initValue = "")
        {
            try
            {
                return _xml.SelectSingleNode(nodePath).InnerText;
            }
            catch
            {
                return initValue;
            }
        }

        public string GetNodeAttribute(string nodePath, string attribute, string initValue = "")
        {
            try
            {
                return ((XmlElement)_xml.SelectSingleNode(nodePath)).GetAttribute(attribute);
            }
            catch
            {
                return initValue;
            }
        }

        public void SetNodeInnerText(string nodePath, string value)
        {
            try
            {
                _xml.SelectSingleNode(nodePath).InnerText = value;
                _xml.Save(_xmlPath);
            }
            catch
            { }
        }

        public void SetNodeAttribute(string nodePath, string attribute, string value)
        {
            try
            {
                ((XmlElement)_xml.SelectSingleNode(nodePath)).SetAttribute(attribute, value);
                _xml.Save(_xmlPath);
            }
            catch
            { }
        }

        public string[] GetChildAttributeText(string nodePath, char split, params string[] args)
        {
            List<string> appList = new List<string>();
            int argsCount = args.Length;
            try
            {
                XmlNode node = _xml.SelectSingleNode(nodePath);
                foreach (XmlElement elm in node.ChildNodes)
                {
                    string appItem = string.Empty;
                    foreach (string arg in args)
                    {
                        appItem += (elm.GetAttribute(arg) + split.ToString());
                    }
                    appList.Add(appItem.Substring(0, appItem.Length - 1));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return appList.ToArray();
        }
    }
}
