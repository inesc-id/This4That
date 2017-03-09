using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Xml;
using This4That_library;

namespace This4That_platform.Nodes
{
    public class ServerManager : MarshalByRefObject, IServerManager
    {
        private ITaskCreator remoteTaskCreator;
        private ITaskDistributor remoteTaskDistributor;

        public bool LoadServerInstance(string configXMLFileName, string configXSDFileName, string targetNS)
        {
            XmlDocument xmlDoc;

            if (!LoadXMLConfiguration(configXMLFileName, configXSDFileName, targetNS, out xmlDoc))
                return false;
            if (!RegisterServerManagerNode(xmlDoc))
                return false;

            WaitingForNodesRegistration();
            Global.Log.Debug("TaskCreator LOADED!");
            return true;
        }

        /// <summary>
        /// Create the ServerManager Remote Object.
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private bool RegisterServerManagerNode(XmlDocument xmlDoc)
        {
            TcpChannel channel;
            string hostName;
            int port;
            string tcpUrl;
            try
            {
                hostName = xmlDoc.GetElementsByTagName(This4That_library.Global.SERVER_MANAGER_NAME)[0].Attributes["hostName"].Value;
                int.TryParse(xmlDoc.GetElementsByTagName(This4That_library.Global.SERVER_MANAGER_NAME)[0].Attributes["port"].Value, out port);
                tcpUrl = $"tcp://{hostName}:{port}/{This4That_library.Global.SERVER_MANAGER_NAME}";
                channel = new TcpChannel(port);
                ChannelServices.RegisterChannel(channel, false);
                RemotingServices.Marshal(this, This4That_library.Global.SERVER_MANAGER_NAME, typeof(IServerManager));
                Global.Log.Debug("ServerManager INITIATED!");
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private void WaitingForNodesRegistration()
        {
            while (this.remoteTaskCreator == null)
            {
                Global.Log.Debug("Waiting for other nodes...");
                Thread.Sleep(2000);
            }
        }

        private bool LoadXMLConfiguration(string configXMLFileName, string configXSDFileName, string targetNS, out XmlDocument xmlDoc)
        {
            xmlDoc = null;
            try
            {
                XMLParser xmlParser = null;
                if (String.IsNullOrEmpty(configXMLFileName))
                {
                    Global.Log.ErrorFormat("Invalid XML File Name: [{0}]", configXMLFileName);
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
                Global.Log.ErrorFormat(ex.Message);
                return false;
            }
        }

        #region REMOTE_METHODS
        /// <summary>
        /// Get Remote Reference Object to Task Creator!
        /// </summary>
        /// <param name="tcpUrl"></param>
        /// <returns></returns>
        public bool RegisterTaskCreatorNode(string tcpUrl)
        {
            try
            {
                this.remoteTaskCreator = (ITaskCreator)Activator.GetObject(typeof(ITaskCreator), tcpUrl);
                if (this.remoteTaskCreator == null)
                {
                    Global.Log.ErrorFormat("Could not locate Task Creator!");
                    return false;
                }
                Global.Log.Debug("Connected to TaskCreator Node!");
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool RegisterTaskDistributorNode(string tcpUrl)
        {
            try
            {
                this.remoteTaskDistributor = (ITaskDistributor)Activator.GetObject(typeof(ITaskDistributor), tcpUrl);
                if (this.remoteTaskDistributor == null)
                {
                    Global.Log.ErrorFormat("Could not locate Task Distributor!");
                    return false;
                }
                Global.Log.Debug("Connected to Task Distributor Node!");
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }
        #endregion
    }
}