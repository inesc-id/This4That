﻿using log4net;
using System;
using This4That_library;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;
using System.Collections.Generic;
using This4That_library.Models.Domain;

namespace This4That_ServerNode.Nodes
{
    public class IncentiveEngine : Node, IIncentiveEngine
    {
        private IRepository repository = null;
        private CentralizedIncentiveScheme centralizedIncentiveScheme;
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
            this.centralizedIncentiveScheme = new CentralizedIncentiveScheme(new Gamification());
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
                userWalletBalance = incentiveScheme.CheckUserBalance(Repository, userId);
                if (userWalletBalance == null)
                {
                    Log.ErrorFormat("Invalid UserID!");
                    return false;
                }
                //check if user has sufficient credits, depending the incentive type
                if (!incentiveScheme.CanPerformTransaction(userWalletBalance, incentiveValue))
                {
                    Console.WriteLine("[INFO - INCENTIVE ENGINE] - User: [{0}] Insufficient Balance!", userId);
                    transactionId = null;
                    return true;
                }
                //create the transaction and store it into the TransactionStorage
                if (!incentiveScheme.RegisterPayment(this.Repository, userId, "Platform", incentiveValue, out transactionId))
                {
                    Log.ErrorFormat("Cannot register payment task for UserId: [{0}]", userId);
                    Console.WriteLine("[ERROR - INCENTIVE ENGINE] - Cannot register task payment!");
                }
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Payment Registered with Success!");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool RewardUser(string userId, out string transactionId, out object taskReward)
        {
            IncentiveSchemeBase incentiveScheme;

            transactionId = null;
            taskReward = null;
            try
            {
                //get user incentive scheme
                if (!GetUserIncentiveScheme(userId, out incentiveScheme))
                {
                    Log.ErrorFormat("Cannot load incentive scheme for User: [{0}]", userId);
                    return false;
                }
                //obtain the reward for completing the task
                taskReward = incentiveScheme.IncentiveType.GetTaskReward();

                //create the transaction and store it into the TransactionStorage
                if (!incentiveScheme.RegisterPayment(this.Repository, "Platform", userId, taskReward, out transactionId))
                {
                    Log.ErrorFormat("Cannot register reward for UserId: [{0}]", userId);
                    Console.WriteLine("[ERROR - INCENTIVE ENGINE] - Cannot register task reward!");
                    return false;
                }
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
            return this.Repository.RegisterUser(this.centralizedIncentiveScheme.IncentiveType);
        }

        public bool GetUserTransactions(string userId, out List<Transaction> transactions)
        {
            IncentiveSchemeBase incentiveScheme;
            transactions = null;

            try
            {
                //get user incentive scheme
                if (!GetUserIncentiveScheme(userId, out incentiveScheme))
                {
                    Log.ErrorFormat("Cannot load incentive scheme for User: [{0}]", userId);
                    return false;
                }
                transactions = this.Repository.GetUserTransactions(userId);
                if (transactions == null)
                {
                    Log.Error("Transactions List IS NULL");
                    return false;
                }

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