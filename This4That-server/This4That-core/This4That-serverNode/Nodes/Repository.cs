using log4net;
using System;
using System.Collections.Generic;
using This4That_library;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.GetTasksByTopicDTO;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Domain;
using This4That_library.Integration;
using System.Diagnostics;
using System.Threading;

namespace This4That_ServerNode.Nodes
{
    public class Repository : Node, IRepository
    {
        private Dictionary<string, Topic> colTopics = new Dictionary<string, Topic>();
        private UserStorage userStorage = new UserStorage();
        private ReportStorage reportStorage = new ReportStorage();
        private TaskStorage taskStorage = new TaskStorage();

        private Stopwatch watch = new Stopwatch();

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

        public ReportStorage ReportStorage
        {
            get
            {
                return reportStorage;
            }

            set
            {
                reportStorage = value;
            }
        }

        public TaskStorage TaskStorage
        {
            get
            {
                return taskStorage;
            }

            set
            {
                taskStorage = value;
            }
        }

        public Repository(string hostName, int port, string name) : base(hostName, port, name, "RepositoryLOG")
        {

        }

        


        /// <summary>
        /// Get Remote reference to Server Manager.
        /// </summary>
        /// <param name="serverMgrURL"></param>
        /// <returns></returns>
        public override bool ConnectServerManager()
        {
            try
            {
                this.RemoteServerMgr = (IServerManager)Activator.GetObject(typeof(IServerManager), Global.SERVER_MANAGER_URL);
                if (!this.RemoteServerMgr.RegisterRepositoryNode($"tcp://{this.HostName}:{this.Port}/{Global.REPOSITORY_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat(" Connected to ServerManager: [{0}]", Global.SERVER_MANAGER_URL);
                Console.WriteLine("[INFO] - CONNECTED to ServerManager");
                Console.WriteLine("----------------------------" + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect Repository to ServerManager: [{0}", Global.SERVER_MANAGER_URL);
                return false;
            }
        }

        #region PRIVATE_METHODS
        private bool SaveTask(CSTaskDTO task, out string taskID)
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
                if (!TaskStorage.CreateTask(task))
                    return false;
                taskID = task.TaskID;
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

        public bool RegisterTask(CSTaskDTO task, string userID, out string taskID)
        {
            watch.Start();
            User user;
            taskID = null;
            try {
                user = UserStorage.GetUserByID(userID);
                if (user == null)
                {
                    Log.ErrorFormat("Invalid User ID: [{0}]", userID);
                    return false;
                }
                if (!SaveTask(task, out taskID))
                    return false;
                user.MyTasks.Add(taskID);
                watch.Stop();
                Log.DebugFormat("Execution Time: [{0}] in milliseconds to Register a task", watch.ElapsedMilliseconds);
                watch.Reset();
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
                    task = TaskStorage.GetTaskByID(taskID);
                    if (task == null)
                    {
                        Log.ErrorFormat("TaskID: [{0}] already does not exist!. Going to remove from Topics.", taskID);
                        topic.ListOfTaskIDs.Remove(taskID);
                    }
                    taskDTO.TaskID = task.TaskID;
                    taskDTO.TaskName = task.Name;
                    listTaskDTO.Add(taskDTO);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public List<String> GetTopicsFromRepository()
        {
            List<String> allTopics = new List<string>();
            try
            {
                foreach (Topic topic in ColTopics.Values)
                {
                    allTopics.Add(topic.Name);
                }
                return allTopics;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public bool RegisterUser(string userAddress, Incentive incentive)
        {
            try
            {
                if (!UserStorage.CreateUser(userAddress, incentive))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
            
        }

        

        public List<string> GetUserMultichainNodes(string userId)
        {
            User user;

            user = this.UserStorage.GetUserByID(userId);

            if (user != null)
                return user.ChainNodesAddresses;

            return null;
        }

        public bool SubscribeTopic(string userId, string topicName, ref string errorMessage)
        {
            User user;

            try
            {
                user = UserStorage.GetUserByID(userId); 
                if (user == null)
                {
                    errorMessage = "Invalid UserID!";
                    return false;
                }
                if (ColTopics.ContainsKey(topicName))
                {
                    user.SubscribeTopic(topicName);
                    Console.WriteLine("[INFO - REPOSITORY] : Topic Subscribed: [{0}] for UserID: [{1}]", topicName, userId);
                    return true;
                }
                else
                {
                    errorMessage = "Invalid TopicName!";
                    return false;
                }                    
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public List<CSTaskDTO> GetTasksByUserID(string userID)
        {
            List<string> myTasksID = new List<string>();
            List<CSTaskDTO> myTasks = new List<CSTaskDTO>();
            CSTask task;
            User user;
            try
            {
                Console.WriteLine("[INFO - REPOSITORY] : Fetching My Tasks");
                user = UserStorage.GetUserByID(userID);

                if (user != null)
                {
                    myTasksID = user.MyTasks;
                    foreach (string taskID in myTasksID)
                    {
                        task = TaskStorage.GetTaskByID(taskID);
                        if (task != null)
                        {
                            myTasks.Add(task.ToDTO());
                        }
                    }
                    return myTasks;
                }                    
                else
                    return null;                    
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public List<CSTaskDTO> GetSubscribedTasksbyUserID(string userID)
        {
            List<string> subscribedTopics = new List<string>();
            List<CSTaskDTO> subscribedTasks = new List<CSTaskDTO>();
            Topic auxTopic;
            CSTask task;
            User user;
            try
            {
                Console.WriteLine("[INFO - REPOSITORY] : Fetching Subscribed Tasks");
                user = UserStorage.GetUserByID(userID);
                if (user != null)
                {
                    subscribedTopics = user.SubscribedTopics;
                    foreach (string topicName in subscribedTopics)
                    {
                        if (ColTopics.ContainsKey(topicName))
                        {
                            auxTopic = ColTopics[topicName];
                            foreach (string taskId in auxTopic.ListOfTaskIDs)
                            {
                                //if is a subscribed task but not created by the user
                                task = TaskStorage.GetTaskByID(taskId);
                                if (task != null && !user.MyTasks.Contains(taskId))
                                {
                                    subscribedTasks.Add(task.ToDTO());
                                }
                            }
                        }
                        else
                            return null;                                             
                    }
                    return subscribedTasks;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public List<CSTaskDTO> GetSubscribedTasksbyTopic(string userID, string topicName, ref string errorMessage)
        {
            List<string> subscribedTopics = new List<string>();
            List<CSTaskDTO> subscribedTasks = new List<CSTaskDTO>();
            Topic auxTopic;
            CSTask task;
            try
            {
                Console.WriteLine("[INFO - REPOSITORY] : Fetching Subscribed Tasks For UserID: [{0}] and TopicName: [{1}]", userID, topicName);
                if (UserStorage.GetUserByID(userID) == null)
                {
                    errorMessage = "Invalid UserID!";
                    return null;
                }
                if (ColTopics.ContainsKey(topicName))
                {
                    auxTopic = ColTopics[topicName];
                    foreach (string taskId in auxTopic.ListOfTaskIDs)
                    {
                        task = TaskStorage.GetTaskByID(taskId);
                        if (task != null)
                        {
                            subscribedTasks.Add(task.ToDTO());
                        }
                    }
                    return subscribedTasks;
                }
                else
                {
                    errorMessage = "Invalid Topic Name!";
                    return null;
                }
                    
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public List<CSTask> GetCSTasks()
        {
            List<CSTask> taskList = new List<CSTask>();

            foreach (CSTask task in TaskStorage.Tasks.Values)
            {
                taskList.Add(task);
            }

            return taskList;
        }

        public InteractiveReport GetInteractiveReportsByID(string reportID)
        {
            return (InteractiveReport) ReportStorage.Reports[reportID];
        }

        public string GetUserReportByTaskId(string taskId, string userId)
        {
            CSTask task;

            task = this.TaskStorage.GetTaskByID(taskId);
            if (!task.ReportsID.ContainsKey(userId))
                return null;
            else
                return task.ReportsID[userId];
        }

        public bool SaveReportInRepository(ReportDTO reportDTO)
        {
            Report report;
            watch.Start();

            try
            {
                if (UserStorage.GetUserByID(reportDTO.UserID) == null)
                {
                    Log.ErrorFormat("User Id: [{0}] does not exist!", reportDTO.UserID);
                    return false;
                }
                //Save report info
                if (!ReportStorage.SaveReport(ref reportDTO, out report))
                    return false;

                //associate to user
                if (!UserStorage.SaveUserReport(report))
                    return false;

                //associate to task
                if (!TaskStorage.AssociateReport(report.UserID, report.ReportID, report.TaskId))
                    return false;

                Log.Debug("[INFO - REPOSITORY] - Report Saved!");
                watch.Stop();
                Log.DebugFormat("Execution Time: [{0}] in milliseconds to Save a Report", watch.ElapsedMilliseconds);
                watch.Reset();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool ExecuteTask(string userID, string taskId)
        {
            User user;

            try
            {
                user = UserStorage.GetUserByID(userID);
                if (TaskStorage.GetTaskByID(taskId) != null)
                    user.MyTasks.Add(taskId);
                else
                {
                    Log.ErrorFormat("Invalid Task Id: [{0}]", taskId);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
        
        public bool AddUserMultichainNode(string userId, string address)
        {
            User user;

            user = this.UserStorage.GetUserByID(userId);

            if (user != null)
            {
                user.ChainNodesAddresses.Add(address);
                return true;
            }
            return false;
                
        }

        public void SaveReportReward(string taskId, string reportId, Dictionary<string, string> reward, string txId)
        {
            this.TaskStorage.GetTaskByID(taskId).ReportsValidated = true;
            this.ReportStorage.AssociateReportReward(reportId, reward, txId);
        }

        public List<CSTask> GetCSTasksToValidate()
        {
            List<CSTask> taskList = new List<CSTask>();

            foreach (CSTask task in TaskStorage.Tasks.Values)
            {
                if (task.ReportsValidated == false && (DateTime.Now > task.ExpirationDate))
                    taskList.Add(task);
            }

            return taskList;
        }

        #endregion

    }
}