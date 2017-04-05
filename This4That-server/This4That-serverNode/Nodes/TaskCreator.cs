using Emitter;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_serverNode.Domain;

namespace This4That_serverNode.Nodes
{
    public class TaskCreator : Node, ITaskCreator
    {
        private IRepository remoteRepository = null;
        private List<CSTask> onGoingTasks = new List<CSTask>();
        private EmitterConn emitter = null;

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

        public List<CSTask> OnGoingTasks
        {
            get
            {
                return onGoingTasks;
            }

            set
            {
                onGoingTasks = value;
            }
        }

        public EmitterConn Emitter
        {
            get
            {
                return emitter;
            }

            set
            {
                emitter = value;
            }
        }

        public TaskCreator(string hostName, int port, string name) : base(hostName, port, name)
        {
            Log = LogManager.GetLogger("TaskCreatorLOG");
            ConnectToEmmiterBroker();
        }

        ~TaskCreator()
        {
            if (this.Emitter.Connection != null)
                this.Emitter.Connection.Disconnect();
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
                if (!this.RemoteServerMgr.RegisterTaskCreatorNode($"tcp://{this.HostName}:{this.Port}/{Global.TASK_CREATOR_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("TASK CREATOR");
                Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port} CONNECTED to ServerManager");
                Console.WriteLine("----------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect TaskCreator to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }

        public bool ConnectToRepository(string repositoryUrl)
        {
            try
            {
                this.RemoteRepository = (IRepository)Activator.GetObject(typeof(IRepository), repositoryUrl);
                Log.DebugFormat("[INFO] Task Creator connected to Repository.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Connects to Emitter broker
        /// </summary>
        /// <param name="emitter"></param>
        /// <returns></returns>
        private bool ConnectToEmmiterBroker()
        {
            //CustomServer 
            string serverKey = "rzBrYBqh2nlglDHBC0oDQq10KCCEjjCw";

            try
            {
                Emitter = new EmitterConn(serverKey);
                Emitter.Connection = new Connection("192.168.1.101", 5010, serverKey);
                //config do emmiter tem de ser carregado a partir de ficheiro
                this.Emitter.Connection.Connect();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public void EmitterReceiver(string channelKey, string topic)
        {
            this.Emitter.Connection.On(channelKey, topic, (channel, msg) =>
            {
                Console.WriteLine("[MQTT - Message]: " + Encoding.UTF8.GetString(msg));
            }, 10);
        }

        #region REMOTE_INTERFACE

        public bool CreateTask(CSTask task, out string taskID)
        {
            string channelKey = null;
            Topic topic;
            taskID = null;
            try
            {                
                Console.WriteLine("Going to create a new Task!");
                taskID = Guid.NewGuid().ToString();
                Log.DebugFormat("TaskCreator : TaskID: [{0}]", taskID);
                topic = this.RemoteRepository.GetTopic(task.Topic);
                if (topic != null)
                {
                    this.Emitter.Connection.Publish(topic.ChannelKey, topic.Name, task.ToString(), 10080);
                }
                else
                {
                    if (!Emitter.GenerateKey(task.Topic, out channelKey))
                    {
                        Log.Error("Cannot generate a channel key!");
                        return false;
                    }
                    Log.DebugFormat("Emitter Generated Channel Key: [{0}]", channelKey);
                    if (!this.RemoteRepository.SaveTopics(task.Topic, channelKey))
                    {
                        Log.Error("Cannot save topic on Repository!");
                        return false;
                    }
                    this.Emitter.Connection.Publish(channelKey, task.Topic, task.ToString(), 10080);
                    EmitterReceiver(channelKey, task.Topic);
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
    }
}