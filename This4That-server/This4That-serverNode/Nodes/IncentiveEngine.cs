using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using This4That_library;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;
using This4That_library.Domain;

namespace This4That_library.Nodes
{
    public class IncentiveEngine : Node, IIncentiveEngine
    {
        private IRepository repository = null;
        private CentralIncentiveScheme centralizedIncentiveScheme;
        private DescentralizedIncentiveScheme descentralizedIncentiveScheme;

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
            this.centralizedIncentiveScheme = new CentralIncentiveScheme(new Gamification());
            this.descentralizedIncentiveScheme = new DescentralizedIncentiveScheme(new Gamification());
            
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

        private bool GetUserIncentiveScheme(string userID, out IncentiveSchemeBase incentiveScheme)
        {
            IncentiveSchemesEnum incentiveSchemeEnum;
            incentiveScheme = null;
            try
            {
                if (!this.Repository.GetUserIncentiveScheme(userID, out incentiveSchemeEnum))
                    return false;

                if (incentiveSchemeEnum == IncentiveSchemesEnum.Centralized)
                    incentiveScheme = this.centralizedIncentiveScheme;
                else
                    incentiveScheme = this.descentralizedIncentiveScheme;
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }

        #region REMOTE_INTERFACE

        public bool CalcTaskCost(CSTaskDTO taskSpec, string userID, out object incentiveValue)
        {
            IncentiveSchemeBase incentiveScheme;
            incentiveValue = null;
            try
            {
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Calc Task Cost for User: " + userID);
                if (!GetUserIncentiveScheme(userID, out incentiveScheme))
                {
                    Log.Error("Cannot obtain User incentive scheme!");
                    return false;
                }
                incentiveValue = incentiveScheme.CalcTaskCost(taskSpec);
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Incentive Value: " + incentiveValue.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                incentiveValue = null;
                return false;
            }
        }

        public bool PayTask(string userId, object incentiveValue, out string transactionId)
        {
            transactionId = null;
            IncentiveSchemeBase incentiveScheme;
            object userWalletBalance;
            try
            {
                //get user incentive scheme
                if (!GetUserIncentiveScheme(userId, out incentiveScheme))
                {
                    Log.ErrorFormat("Cannot load incentive scheme for User: [{0}]", userId);
                }
                userWalletBalance = Repository.GetUserBalance(userId);
                if (userWalletBalance == null)
                {
                    Log.ErrorFormat("Invalid UserID!");
                    return false;
                }
                //check if user has sufficient credits, depending the incentive type
                if (!incentiveScheme.HasUserSufficientCredits(userWalletBalance, incentiveValue))
                {
                    Console.WriteLine("[INFO - INCENTIVE ENGINE] - User: [{0}] Insufficient Balance!", userId);
                    transactionId = null;
                    return true;
                }
                //create the transaction and store it into the TransactionStorage
                if (!incentiveScheme.RegisterTaskPayment(this.Repository, userId, incentiveValue, out transactionId))
                {
                    Log.ErrorFormat("Cannot register payment task for UserId: [{0}]", userId);
                    Console.WriteLine("[ERROR - INCENTIVE ENGINE] - Cannot register task payment!");
                }
                //associate in the user wallet the transaction ID and calc the new wallet balance
                this.Repository.AssociateTransactionUser(userId, incentiveScheme.Incentive, incentiveValue, transactionId);
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - UserID: [{0}] Account Balance: [{1}]", userId, Repository.GetUserBalance(userId));
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Payment Registered with Success!");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public string RegisterUser()
        {
            //centralized version as default
            return this.Repository.RegisterUser(this.centralizedIncentiveScheme.Incentive);
        }
        #endregion
    }
}