using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_serverNode.IncentiveModels;

namespace This4That_serverNode.Nodes
{
    public class Repository : Node, IRepository
    {
        private List<Topic> topics = new List<Topic>();

        public Repository(string hostName, int port, string name) : base(hostName, port, name)
        {
            Log = LogManager.GetLogger("RepositoryLOG");
        }


        /// <summary>
        /// Get Remote reference to Server Manager.
        /// </summary>
        /// <param name="serverMgrURL"></param>
        /// <returns></returns>
        public override bool ConnectServerManager(string serverMgrURL)
        {
            try
            {
                this.RemoteServerMgr = (IServerManager)Activator.GetObject(typeof(IServerManager), serverMgrURL);
                if (!this.RemoteServerMgr.RegisterRepositoryNode($"tcp://{this.HostName}:{this.Port}/{Global.REPOSITORY_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("REPOSITORY");
                Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port} CONNECTED to ServerManager");
                Console.WriteLine("----------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect Repository to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }

        
        
        #region REMOTE_INTERFACE
        public bool AuthenticateUser(string userID)
        {
            return true;
        }

        public bool GetUserIncentiveMechanism(string userID, out IncentiveSchemeBase incentiveScheme)
        {
           incentiveScheme = new CentralIncentiveScheme();
           return true;
        }

        public bool SaveTopics(string topicName, string channelKey)
        {
            Topic topic = new Topic(topicName, channelKey);
            if (!topics.Contains(topic))
            {
                topics.Add(topic);
            }
            return true;
        }

        public Topic GetTopic(string topicName)
        {
            foreach (Topic aux_topic in topics)
            {
                if (aux_topic.Name.Equals(topicName))
                {
                    return aux_topic;
                }
            }
            return null;
        }

        public List<Topic> GetTopics()
        {
            return topics;
        }

        #endregion

    }
}