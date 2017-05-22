using log4net;
using System;
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

        public ReportAggregator(string hostName, int port, string name) : base(hostName, port, name)
        {
            Console.WriteLine("REPORT AGGREGATOR");
            Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port}");
            Log = LogManager.GetLogger("ReportAggregatorLOG");
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
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("[INFO] - CONNECTED to ServerManager");
                Console.WriteLine("----------------------------" + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect Report Aggregator to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }

        public bool ConnectToRepository(string repositoryUrl)
        {
            try
            {
                this.RemoteRepository = (IRepository)Activator.GetObject(typeof(IRepository), repositoryUrl);
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