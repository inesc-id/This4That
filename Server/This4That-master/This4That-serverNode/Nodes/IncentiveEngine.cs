using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;
using This4That_library.Models.Domain;

namespace This4That_serverNode.Nodes
{
    public class IncentiveEngine : Node, IIncentiveEngine
    {
        public IncentiveEngine(string hostName, int port, string name) : base(hostName, port, name)
        {
            Log = LogManager.GetLogger("IncentiveEngineLOG");
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
                if (!this.RemoteServerMgr.RegisterIncentiveEngineNode($"tcp://{this.HostName}:{this.Port}/{Global.INCENTIVE_ENGINE_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("INCENTIVE ENGINE");
                Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port} CONNECTED to ServerManager");
                Console.WriteLine("----------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect Incentive Engine to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }


        #region REMOTE_INTERFACE
        public bool CalcTaskCost(CSTask taskSpec, out object incentiveValue)
        {
            try
            {
                incentiveValue = 1;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                incentiveValue = null;
                return false;
            }
        }

        public bool IsTaskPaid(string transactionId)
        {
            if (transactionId != null)
                return true;
            return false;
        }
        #endregion
    }
}