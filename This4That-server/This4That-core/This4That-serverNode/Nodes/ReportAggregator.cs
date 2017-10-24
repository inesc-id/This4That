using log4net;
using System;
using System.Diagnostics;
using This4That_library;
using This4That_library.Models.Integration.ReportDTO;

namespace This4That_ServerNode.Nodes
{
    public class ReportAggregator : Node, IReportAggregator
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

        public ReportAggregator(string hostName, int port, string name) : base(hostName, port, name, "ReportAggregatorLOG")
        {
            ConnectToRepository();
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
                if (!this.RemoteServerMgr.RegisterReportAggregatorNode($"tcp://{this.HostName}:{this.Port}/{Global.REPORT_AGGREGATOR_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", Global.SERVER_MANAGER_URL);
                Console.WriteLine("[INFO] - CONNECTED to ServerManager");
                Console.WriteLine("----------------------------" + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect Report Aggregator to ServerManager: [{0}", Global.SERVER_MANAGER_URL);
                return false;
            }
        }

        private bool ConnectToRepository()
        {
            try
            {
                this.RemoteRepository = (IRepository)Activator.GetObject(typeof(IRepository), Global.REPOSITORY_URL);
                Log.DebugFormat("[INFO] Report Aggregator connected to Repository.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool SaveReport(ReportDTO report)
        {
            try
            {
                Log.Debug("Report Received!");
                Console.WriteLine("[INFO - REPORT_AGGREGATOR] - Report Received!");

                if (!this.RemoteRepository.SaveReportInRepository(report))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}