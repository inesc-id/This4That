using log4net;
using System;
using This4That_library;
using This4That_library.Models.Integration;

namespace This4That_ServerNode.Nodes
{
    public class TaskCreator : Node, ITaskCreator
    {
        private IRepository remoteRepository = null;

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

        public TaskCreator(string hostName, int port, string name) : base(hostName, port, name, "TaskCreatorLOG")
        {
            ConnectToRepository();
            Console.WriteLine("TASK CREATOR");
            Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port}");
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
                if (!this.RemoteServerMgr.RegisterTaskCreatorNode($"tcp://{this.HostName}:{this.Port}/{Global.TASK_CREATOR_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", Global.REPOSITORY_URL);
                Console.WriteLine("[INFO] - CONNECTED to ServerManager");
                Console.WriteLine("----------------------------" + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect TaskCreator to ServerManager: [{0}", Global.REPOSITORY_URL);
                return false;
            }
        }

        private bool ConnectToRepository()
        {
            try
            {
                this.RemoteRepository = (IRepository)Activator.GetObject(typeof(IRepository), Global.REPOSITORY_URL);
                Log.DebugFormat("[INFO - TASK CREATOR] - Connected to Repository.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        

        #region REMOTE_INTERFACE

        public bool CreateTask(CSTaskDTO task, string userID, out string taskID)
        {
            taskID = null;
            try
            {                
                if (!this.RemoteRepository.RegisterTask(task, userID, out taskID))
                {
                    Log.Error("Cannot save task on Repository!");
                    return false;
                }
                Console.WriteLine("[INFO - TASK CREATOR] - Task created with Sucess!");
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