using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Web;
using System.Xml;
using This4That_library;

namespace This4That_serverNode.Nodes
{
    public abstract class Node : MarshalByRefObject
    {
        string hostName;
        int port;
        string name;
        IServerManager serverMgr;

        #region PROPERTIES
        public string HostName
        {
            get
            {
                return hostName;
            }

            set
            {
                hostName = value;
            }
        }
        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public IServerManager ServerMgr
        {
            get
            {
                return serverMgr;
            }

            set
            {
                serverMgr = value;
            }
        }
        #endregion


        public Node(string hostName, int port, string name)
        {
            this.HostName = hostName;
            this.Port = port;
            this.Name = name;
        }

        /// <summary>
        /// Get Remote reference to Server Manager.
        /// </summary>
        /// <param name="serverMgrURL"></param>
        /// <returns></returns>
        public abstract bool ConnectServerManager(string serverMgrURL);

        private bool LoadXMLConfiguration(string configXMLFileName, string configXSDFileName, string targetNS, out XmlDocument xmlDoc)
        {
            xmlDoc = null;
            try
            {
                XMLParser xmlParser = null;
                if (String.IsNullOrEmpty(configXMLFileName))
                {
                    Program.Log.ErrorFormat("Invalid XML File Name: [{0}]", configXMLFileName);
                    return false;
                }
                xmlParser = new XMLParser(configXMLFileName, configXSDFileName, targetNS);
                if (!xmlParser.ValidateXML())
                    return false;
                xmlDoc = xmlParser.XmlDoc;
                return true;
            }
            catch (Exception ex)
            {
                Program.Log.ErrorFormat(ex.Message);
                return false;
            }
        }
    }
}