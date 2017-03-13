using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Xml;
using This4That_library;

namespace This4That_platform
{
    public class ServerManager : MarshalByRefObject, IServerManager
    {
        private ITaskCreator remoteTaskCreator;
        private ITaskDistributor remoteTaskDistributor;
        private IReportAggregator remoteReportAggregator;
        private IIncentiveEngine remoteIncentiveEngine;
        private IRepository remoteRepository;

        #region PROPERTIES
        public ITaskCreator RemoteTaskCreator
        {
            get
            {
                return remoteTaskCreator;
            }

            set
            {
                remoteTaskCreator = value;
            }
        }

        public ITaskDistributor RemoteTaskDistributor
        {
            get
            {
                return remoteTaskDistributor;
            }

            set
            {
                remoteTaskDistributor = value;
            }
        }

        public IReportAggregator RemoteReportAggregator
        {
            get
            {
                return remoteReportAggregator;
            }

            set
            {
                remoteReportAggregator = value;
            }
        }

        public IIncentiveEngine RemoteIncentiveEngine
        {
            get
            {
                return remoteIncentiveEngine;
            }

            set
            {
                remoteIncentiveEngine = value;
            }
        }

        public IRepository RemoteRepository
        {
            get
            {
                return remoteRepository;
            }

            set
            {
                remoteRepository = value;
            }
        }

        #endregion

        /// <summary>
        /// Read from the config file and register the ServerManager instance.
        /// </summary>
        /// <param name="configXMLFileName"></param>
        /// <param name="configXSDFileName"></param>
        /// <param name="targetNS"></param>
        /// <returns></returns>
        public bool LoadServerInstance(string configXMLFileName, string configXSDFileName, string targetNS)
        {
            XMLParser xmlParser;
            string errorMessage = null;
            try
            {
                xmlParser = new XMLParser(configXMLFileName, configXSDFileName, targetNS);
                if (!xmlParser.LoadXMLConfiguration(ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                else
                    Global.Log.Debug(errorMessage);

                if (!RegisterServerManagerNode(xmlParser.XmlDoc))
                    return false;
                Global.Log.Debug("ServerManager LOADED!");
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
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
                this.RemoteTaskCreator = (ITaskCreator)Activator.GetObject(typeof(ITaskCreator), tcpUrl);
                if (this.RemoteTaskCreator == null)
                {
                    Global.Log.ErrorFormat("Could not locate Task Creator!");
                    return false;
                }
                Global.Log.DebugFormat("Connected to TaskCreator Node! Address: [{0}]", tcpUrl);
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
                this.RemoteTaskDistributor = (ITaskDistributor)Activator.GetObject(typeof(ITaskDistributor), tcpUrl);
                if (this.RemoteTaskDistributor == null)
                {
                    Global.Log.ErrorFormat("Could not locate Task Distributor!");
                    return false;
                }
                Global.Log.DebugFormat("Connected to Task Distributor Node! Address: [{0}]", tcpUrl);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool RegisterReportAggregatorNode(string tcpUrl)
        {
            this.RemoteReportAggregator = (IReportAggregator)Activator.GetObject(typeof(IReportAggregator), tcpUrl);
            if (this.RemoteReportAggregator == null)
            {
                Global.Log.ErrorFormat("Could not locate ReportAggregator!");
                return false;
            }
            Global.Log.DebugFormat("Connected to Report Aggregator Node! Address: [{0}]", tcpUrl);
            return true;
        }

        public bool RegisterIncentiveEngineNode(string tcpUrl)
        {
            this.RemoteIncentiveEngine = (IIncentiveEngine)Activator.GetObject(typeof(IIncentiveEngine), tcpUrl);
            if (this.RemoteIncentiveEngine == null)
            {
                Global.Log.ErrorFormat("Could not locate IncentiveEngine!");
                return false;
            }
            Global.Log.DebugFormat("Connected to Incentive Engine Node! Address: [{0}]", tcpUrl);
            return true;
        }

        public bool RegisterRepositoryNode(string tcpUrl)
        {
            this.RemoteRepository = (IRepository)Activator.GetObject(typeof(IRepository), tcpUrl);
            if (this.RemoteRepository == null)
            {
                Global.Log.ErrorFormat("Could not locate Repository!");
                return false;
            }
            Global.Log.DebugFormat("Connected to Repository Node! Address: [{0}]", tcpUrl);
            return true;
        }
        #endregion
    }
}