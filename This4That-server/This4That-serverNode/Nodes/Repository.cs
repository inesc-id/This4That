﻿using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_library.Models.Integration.GetTasksByTopicDTO;
using This4That_serverNode.Domain;
using This4That_serverNode.IncentiveModels;

namespace This4That_serverNode.Nodes
{
    public class Repository : Node, IRepository
    {
        private Dictionary<string, Topic> colTopics = new Dictionary<string, Topic>();
        private Dictionary<string, CSTask> colTasks = new Dictionary<string, CSTask>();
        private UserStorage userStorage = new UserStorage();

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
        public UserStorage UserStorage
        {
            get
            {
                return userStorage;
            }

            set
            {
                userStorage = value;
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

        #region PRIVATE_METHODS
        private bool SaveTask(CSTask task, out string taskID)
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

        #endregion

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

        public bool RegisterTask(CSTask task, string userID, out string taskID)
        {
            User user;
            taskID = null;
            try {

                if (!SaveTask(task, out taskID))
                    return false;
                if (!UserStorage.Users.TryGetValue(userID, out user))
                {
                    Log.ErrorFormat("Invalid User ID: [{0}]", userID);
                    return false;
                }
                user.ColTasks.Add(task);
                return true;                            
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }            
        }

        public bool GetTasksByTopicName(out List<GetTasksDTO> listTaskDTO, string topicName)
        {
            Topic topic;
            GetTasksDTO taskDTO = null;
            CSTask task;
            listTaskDTO = null;

            try
            {
                if (!ColTopics.TryGetValue(topicName, out topic))
                {
                    Log.ErrorFormat("Invalid Topic Name: [{0}]", topicName);
                    return false;
                }
                listTaskDTO = new List<GetTasksDTO>();
                foreach (string taskID in topic.ListOfTaskIDs)
                {
                    taskDTO = new GetTasksDTO();
                    if (!ColTasks.TryGetValue(taskID, out task))
                    {
                        Log.ErrorFormat("TaskID: [{0}] already does not exist!. Going to remove from Topics.");
                        topic.ListOfTaskIDs.Remove(taskID);
                    }
                    else
                    {
                        taskDTO.TaskID = task.TaskID;
                        taskDTO.TaskName = task.Name;
                        listTaskDTO.Add(taskDTO);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public Dictionary<string, string> GetTopicsFromRepository()
        {
            Dictionary<string, string> topics = new Dictionary<string, string>();
            try
            {
                foreach (Topic topic in ColTopics.Values)
                {
                    topics.Add("name", topic.Name);
                }
                return topics;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public string RegisterUser()
        {
            User user;
            string userId = Guid.NewGuid().ToString();

            while (UserStorage.Users.ContainsKey(userId))
            {
                userId = Guid.NewGuid().ToString();
            }
            user = new User();
            user.UserID = userId;
            UserStorage.Users.Add(userId, user);
            return userId;
        }

        #endregion

    }
}