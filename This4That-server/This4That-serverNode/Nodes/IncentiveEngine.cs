using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_serverNode.Domain;
using This4That_serverNode.IncentiveModels;

namespace This4That_serverNode.Nodes
{
    public class IncentiveEngine : Node, IIncentiveEngine
    {
        private IRepository repository = null;

        private Dictionary<string, PendingPayment> pendingPayments = new Dictionary<string, PendingPayment>();

        public Dictionary<string, PendingPayment> PendingPayments
        {
            get
            {
                return pendingPayments;
            }

            set
            {
                pendingPayments = value;
            }
        }

        public IRepository Repository
        {
            get
            {
                return repository;
            }

            set
            {
                repository = value;
            }
        }

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

        public bool ConnectToRepository(string repositoryUrl)
        {
            try
            {
                this.Repository = (IRepository)Activator.GetObject(typeof(IRepository), repositoryUrl);
                Log.DebugFormat("[INFO] Incentive Engine connected to Repository.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        private string GenerateReferenceToPay()
        {
            string guid = Guid.NewGuid().ToString();
            lock (this)
            {
                while (PendingPayments.Keys.Contains(guid))
                {
                    guid = Guid.NewGuid().ToString();
                }
                return guid;
            }
        }

        private bool GetUserIncentiveScheme(string userID, out IncentiveSchemeBase incentiveScheme)
        {
            incentiveScheme = null;
            try
            {
                if (!this.Repository.GetUserIncentiveMechanism(userID, out incentiveScheme))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        #region REMOTE_INTERFACE
        public bool CalcTaskCost(CSTask taskSpec, string userID, out object incentiveValue, out string reftoPay)
        {
            IncentiveSchemeBase incentiveScheme;
            PendingPayment payment;
            incentiveValue = null;
            reftoPay = null;
            try
            {
                Console.WriteLine("[INFO-Incentive engine] - Calc Task Cost for User: " + userID);
                if (!GetUserIncentiveScheme(userID, out incentiveScheme))
                {
                    Log.Error("Cannot obtain User incentive scheme!");
                    return false;
                }
                if (!incentiveScheme.CalcTaskCost(taskSpec, out incentiveValue))
                {
                    Log.Error("Cannot calculate the incentive value!");
                    return false;
                }
                reftoPay = GenerateReferenceToPay();
                payment = new PendingPayment(userID, reftoPay, incentiveValue);
                lock (this)
                {
                    PendingPayments.Add(reftoPay, payment);
                }                
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                incentiveValue = null;
                return false;
            }
        }


        public bool PayTask(string refToPay, out string transactionId)
        {
            transactionId = null;
            try
            {
                //pagar a task consoante o mecanismo de incentivos.
                PendingPayments.Remove(refToPay);
                transactionId = Guid.NewGuid().ToString();
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