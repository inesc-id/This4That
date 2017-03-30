using Emitter;
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
        private List<CSTask> onGoingTasks = new List<CSTask>();

        public TaskDistributor(string hostName, int port, string name) : base(hostName, port, name)
        {
            Log = LogManager.GetLogger("TaskDistributorLOG");
            //ConnectToEmmiterBroker(out emitterConn);
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


        #region REMOTE_INTERFACE
        public bool ReceiveTask(CSTask task)
        {
            try
            {
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