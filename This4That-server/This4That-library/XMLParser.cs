using System;
using System.Xml;
using System.Xml.Schema;

namespace This4That_library
{
    public class XMLParser
    {
        private string xmlFileName;
        private string xsdFileName;
        private string targetNS;
        private XmlDocument xmlDoc;

        #region PROPERTIES
        public string XmlFileName
        {
            get
            {
                return xmlFileName;
            }
        }

        public string XsdFileName
        {
            get
            {
                return xsdFileName;
            }
        }

        public string TargetNS
        {
            get
            {
                return targetNS;
            }
        }

        public XmlDocument XmlDoc
        {
            get
            {
                return xmlDoc;
            }
        }
        #endregion

        public XMLParser(string xmlFileName, string xsdFileName, string targetNS)
        {
            this.xmlFileName = xmlFileName;
            this.xsdFileName = xsdFileName;
            this.targetNS = targetNS;
        }

        public bool LoadXMLConfiguration(ref string errorMessage)
        {
            xmlDoc = null;
            try
            {
                if (String.IsNullOrEmpty(this.XmlFileName))
                {
                    errorMessage = "Invalid XML file name!";
                    return false;
                }
                if (!ValidateXML(ref errorMessage))
                    return false;
                errorMessage = "Valid XML File!";
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private bool ValidateXML(ref string errorMessage)
        {
            XmlReaderSettings settings;
            XmlReader reader;
            XmlDocument document;
            ValidationEventHandler eventHandler;

            try
            {
                settings = new XmlReaderSettings();
                settings.Schemas.Add(TargetNS, XsdFileName);
                settings.ValidationType = ValidationType.Schema;

                reader = XmlReader.Create(xmlFileName, settings);
                document = new XmlDocument();
                document.Load(reader);
                eventHandler = new ValidationEventHandler(ValidateXMLHandler);
                document.Validate(eventHandler);
                this.xmlDoc = document;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private void ValidateXMLHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    break;
                default:
                    break;
            }
        }

    }
}