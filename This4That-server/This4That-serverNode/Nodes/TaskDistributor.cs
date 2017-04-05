using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using This4That_library;
using This4That_library.Models.Domain;

namespace This4That_serverNode.Nodes
{
    public class TaskDistributor : Node, ITaskDistributor
    {
        private IRepository remoteRepository = null;

        public TaskDistributor(string hostName, int port, string name) : base(hostName, port, name)
        {
            Log = LogManager.GetLogger("TaskDistributorLOG");
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
                if (!this.RemoteServerMgr.RegisterTaskDistributorNode($"tcp://{this.HostName}:{this.Port}/{Global.TASK_DISTRIBUTOR_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("TASK DISTRIBUTOR");
                Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port} CONNECTED to ServerManager");
                Console.WriteLine("----------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect Task Distributor to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }

        public bool ConnectToRepository(string repositoryUrl)
        {
            try
            {
                this.RemoteRepository = (IRepository)Activator.GetObject(typeof(IRepository), repositoryUrl);
                Log.DebugFormat("[INFO-Task Distributor] Connected to Repository.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        #region REMOTE_INTERFACE

        public Topic GetTopic(string topicName)
        {
            try
            {
                return this.RemoteRepository.GetTopicFromRepository(topicName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public List<Topic> GetTopics()
        {
            try
            {
                return this.RemoteRepository.GetTopicsFromRepository();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        #endregion
    }
}