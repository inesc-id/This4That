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
        private Dictionary<string, Topic> colTopics = new Dictionary<string, Topic>();
        private Dictionary<string, CSTask> colTasks = new Dictionary<string, CSTask>();

        public Dictionary<string, Topic> ColTopics
        {
            get
            {
                return colTopics;
            }

            set
            {
                colTopics = value;
            }
        }

        public Dictionary<string, CSTask> ColTasks
        {
            get
            {
                return colTasks;
            }

            set
            {
                colTasks = value;
            }
        }

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

        public bool SaveTask(CSTask task, out string taskID)
        {
            taskID = null;
            Topic topic;
            try
            {
                if (task == null || task.TopicName == null)
                {
                    Log.Error("Task or Topic IS NULL");
                    return false;
                }
                //save task
                taskID = Guid.NewGuid().ToString();
                while (ColTasks.ContainsKey(taskID))
                {
                    taskID = Guid.NewGuid().ToString();
                }
                task.TaskID = taskID;
                ColTasks.Add(taskID, task);
                Log.DebugFormat("Generated TaskID: [{0}]", taskID);
                Log.Debug("Task Saved with sucess!");
                //save topic
                if (ColTopics.ContainsKey(task.TopicName))
                {
                    ColTopics[task.TopicName].ListOfTaskIDs.Add(task.TaskID);
                }
                //topico nao existe
                else
                {
                    topic = new Topic(task.TopicName);
                    topic.ListOfTaskIDs.Add(task.TaskID);
                    ColTopics.Add(task.TopicName, topic);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }            
        }

        public Topic GetTopicFromRepository(string topicName)
        {
            foreach (Topic aux_topic in ColTopics.Values)
            {
                if (aux_topic.Name.Equals(topicName))
                {
                    return aux_topic;
                }
            }
            return null;
        }

        public List<Topic> GetTopicsFromRepository()
        {
            return ColTopics.Values.ToList();
        }

        #endregion

    }
}