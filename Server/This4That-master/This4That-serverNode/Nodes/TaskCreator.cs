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
                this.ServerMgr = (IServerManager)Activator.GetObject(typeof(IServerManager), serverMgrURL);
                if (!this.ServerMgr.RegisterTaskCreatorNode($"tcp://{this.HostName}:{this.Port}/{Global.TASK_CREATOR_NAME}"))
                {
                    Program.Log.Error("Cannot connect to Server Manager!");
                }
                Program.Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("Connected to Server Manager!");
                return true;
            }
            catch (Exception ex)
            {
                Program.Log.Error(ex.Message);
                Program.Log.ErrorFormat("Cannot connect TaskCreator to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }
    }
}