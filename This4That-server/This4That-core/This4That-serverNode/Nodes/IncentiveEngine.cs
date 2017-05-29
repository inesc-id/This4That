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
        private IncentiveSchemeBase incentiveScheme;

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

        public IncentiveSchemeBase IncentiveScheme
        {
            get
            {
                return incentiveScheme;
            }

            set
            {
                incentiveScheme = value;
            }
        }

        public IncentiveEngine(string hostName, int port, string name) : base(hostName, port, name)
        {
            Console.WriteLine("INCENTIVE ENGINE");
            Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port}");

            Log = LogManager.GetLogger("IncentiveEngineLOG");
            try
            {
                this.IncentiveScheme = new CentralizedIncentiveScheme(new Gamification());
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(ex.Message + " Failed to connect to the Multichain node.");
                Console.WriteLine("[ERROR] - FAILED TO CONNECT THE MULTICHAIN NODE!");
            }            
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
                Console.WriteLine("[INFO] - CONNECTED to ServerManager");
                Console.WriteLine("----------------------------" + Environment.NewLine);
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

        #region REMOTE_INTERFACE

        public bool CalcTaskCost(CSTaskDTO taskSpec, string userID, out object incentiveValue)
        {
            incentiveValue = null;
            try
            {
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Calc Task Cost for User: " + userID);
                incentiveValue = IncentiveScheme.CalcTaskCost(taskSpec);
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
            object userWalletBalance;
            try
            {
                userWalletBalance = IncentiveScheme.CheckUserBalance(Repository, userId);
                if (userWalletBalance == null)
                {
                    Log.ErrorFormat("Invalid UserID!");
                    return false;
                }
                //check if user has sufficient credits, depending the incentive type
                if (!IncentiveScheme.CanPerformTransaction(userWalletBalance, incentiveValue))
                {
                    Console.WriteLine("[INFO - INCENTIVE ENGINE] - User: [{0}] Insufficient Balance!", userId);
                    transactionId = null;
                    return true;
                }
                //create the transaction and store it into the TransactionStorage
                if (!IncentiveScheme.RegisterTransaction(this.Repository, userId, "Platform", incentiveValue, out transactionId))
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
            transactionId = null;
            taskReward = null;
            try
            {
                //obtain the reward for completing the task
                taskReward = IncentiveScheme.Incentive.GetTaskReward();

                //create the transaction and store it into the TransactionStorage
                if (!IncentiveScheme.RegisterTransaction(this.Repository, "Platform", userId, taskReward, out transactionId))
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
            string userId;
            object initValue;
            string transactionId;

            try
            {
                userId = this.Repository.RegisterUser(this.IncentiveScheme.Incentive);
                //get init value based on the incentive
                initValue = IncentiveScheme.Incentive.InitWalletBalance();
                //register transaction
                if (!IncentiveScheme.RegisterTransaction(Repository, "Platform", userId, initValue, out transactionId))
                {
                    Log.ErrorFormat("Cannot register init value transaction for UserId: [{0}]", userId);
                    Console.WriteLine("[ERROR - INCENTIVE ENGINE] - Cannot register init value transaction!");
                }
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - User: [{0}] has an initial wallet value: [{1}]", userId, initValue.ToString());
                return userId;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
            

        }

        public bool GetUserTransactions(string userId, out List<Transaction> transactions)
        {
            transactions = null;

            try
            {
                //get user transaction based on the actual incentive scheme
                transactions = IncentiveScheme.GetUserTransactions(Repository, userId);
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


        #region DESCENTRALIZED_SCHEME_METHODS

        public bool AddNodeToChain(string userId, string multichainAddress, ref string message)
        {
            message = null;

            try
            {
                if (IncentiveScheme.GetType() == typeof(DescentralizedIncentiveScheme))
                {
                    //add node
                    if (!((DescentralizedIncentiveScheme)IncentiveScheme).AddNodeToChain(multichainAddress))
                    {
                        Log.ErrorFormat("Invalid Chain address: [{0}]", multichainAddress);
                        message = "Invalid address!";
                        return false;
                    }
                    //add association to user wallet
                    if (!this.Repository.AddNodeToUserWalletDescentralized(userId, multichainAddress))
                    {
                        Log.Error("Cannot associate the multichain node to the user's wallet.");
                        message = "Cannot associate block-chain node to user!";
                        return false;
                    }
                    return true;
                }
                message = "Your scheme must be changed to the descentralized version!";
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        #endregion

        #endregion
    }
}