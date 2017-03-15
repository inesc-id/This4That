using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;

namespace This4That_serverNode.Nodes
{
    public class TaskCreator : Node, ITaskCreator
    {
        public TaskCreator(string hostName, int port, string name) : base(hostName, port, name)
        {

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
                    Program.Log.Error("Cannot connect to Server Manager!");
                }
                Program.Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("TASK CREATOR");
                Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port} CONNECTED to ServerManager");
                Console.WriteLine("----------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Program.Log.Error(ex.Message);
                Program.Log.ErrorFormat("Cannot connect TaskCreator to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }


        #region REMOTE_INTERFACE

        public bool CreateTask(string encryptedTask, out string taskID)
        {
            taskID = Guid.NewGuid().ToString();
            Program.Log.DebugFormat("TaskCreator : TaskID: [{0}]", taskID);
            return true;
        }

        #endregion
    }
}