using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using This4That_library;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.GetTasksByTopicDTO;

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

        public List<GetTasksDTO> GetTasksByTopicName(string topicName)
        {
            List<GetTasksDTO> listTaskDTO = null;
            try
            {
                if (!this.RemoteRepository.GetTasksByTopicName(out listTaskDTO, topicName))
                {
                    Log.Error("Cannot retrieve tasks from Repository");
                    return null;
                }
                return listTaskDTO;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public List<String> GetTopics()
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

        public bool SubscribeTopic(string userId, string topicName, ref string errorMessage)
        {
            try
            {
                Console.WriteLine("[INFO - TASK DISTRIBUTOR] : Going to Subscribe Topic: [{0}] for UserID: [{1}]", topicName, userId);
                Log.DebugFormat("Going to subscribe Topic: [{0}] for UserID: [{1}]", topicName, userId);
                return this.RemoteRepository.SubscribeTopic(userId, topicName, ref errorMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public List<CSTaskDTO> GetUserSubscribedTasks(string userID)
        {
            try
            {
                Console.WriteLine("[INFO - TASK DISTRIBUTOR] : Fetching User Subscribed Tasks");
                Log.DebugFormat("Going to fetch all subscribed tasks for UserID: [{0}]", userID);
                return this.RemoteRepository.GetSubscribedTasksbyUserID(userID);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public List<CSTaskDTO> GetSubscribedTasksByTopicName(string userID, string topicName, ref string errorMessage)
        {
            try
            {
                Console.WriteLine("[INFO - TASK DISTRIBUTOR] : Fetching User: [{0}] Subscribed Tasks by Topic:[{0}]", userID, topicName);
                Log.DebugFormat("Going to fetch all tasks belonging to topicName: [{1}] for UserID: [{0}]", userID, topicName);
                return this.RemoteRepository.GetSubscribedTasksbyTopic(userID, topicName, ref errorMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public List<CSTaskDTO> GetUserTasks(string userID)
        {
            try
            {
                Console.WriteLine("[INFO - TASK DISTRIBUTOR] : Fetching User Tasks");
                Log.DebugFormat("Going to fetch all created tasks by UserID: [{0}]", userID);
                return this.RemoteRepository.GetTasksByUserID(userID);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public bool ExecuteTask(string userID, string taskId)
        {
            try
            {
                if (!this.RemoteRepository.ExecuteTask(userID, taskId))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }





        #endregion
    }
}