﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;

namespace This4That_serverNode.Nodes
{
    public class ReportAggregator : Node, IReportAggregator
    {
        public ReportAggregator(string hostName, int port, string name) : base(hostName, port, name)
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
                if (!this.RemoteServerMgr.RegisterReportAggregatorNode($"tcp://{this.HostName}:{this.Port}/{Global.REPORT_AGGREGATOR_NAME}"))
                {
                    Program.Log.Error("Cannot connect to Server Manager!");
                }
                Program.Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("REPORT AGGREGATOR");
                Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port} CONNECTED to ServerManager");
                Console.WriteLine("----------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Program.Log.Error(ex.Message);
                Program.Log.ErrorFormat("Cannot connect Report Aggregator to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }
    }
}