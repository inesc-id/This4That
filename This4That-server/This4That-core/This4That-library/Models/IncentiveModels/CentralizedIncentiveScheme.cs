using System;
using System.Collections.Generic;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.IncentiveModels
{
    [Serializable]
    public class CentralizedIncentiveScheme : IncentiveSchemeBase
    {
        public CentralizedIncentiveScheme(IRepository repository, ITransactionNode txNode, Incentive incentive) : base(repository, txNode, incentive)
        {
            InitManager(incentive);
        }

        public override bool CheckUserBalance(string userId, IncentiveAssigned incentiveAssigned)
        {
            
            Dictionary<string, int> incentivesBalance;
            int balance;


            Log.DebugFormat("Going to check the balance for UserId: [{0}]", userId);
            Watch.Stop();
            incentivesBalance = GetUserBalance(userId);
            Watch.Start();
            balance = incentivesBalance[incentiveAssigned.IncentiveName];

            Log.DebugFormat("Balance: [{0}] for Incentive: [{1}]", balance, incentiveAssigned.IncentiveName);
            if (!Incentive.CheckSufficientCredits(balance, incentiveAssigned.IncentiveQty))
            {
                log.Debug("Insufficient Funds.");
                return false;
            }
            return true;
        }

        public override bool RegisterTransaction(string sender, string receiver, IncentiveAssigned incentiveAssigned, out string transactionId, out bool hasFunds)
        {
            transactionId = null;
            hasFunds = false;

            //check if the user can make the transaction
            if (!CheckUserBalance(sender, incentiveAssigned))
            {

                //check if the Manager has  the necessary incentives to distribute among the users
                if (sender.Equals(this.ManagerAddress))
                {
                    log.Debug("Manager does not have the necessary incentive quantity. Going to issue more incentives!");
                    IssueMoreIncentives(incentiveAssigned);
                }
                else
                {
                    hasFunds = false;
                    return true;
                }
            }
            hasFunds = true;
            log.DebugFormat("Going to create transaction for transfer [{0}] of incentive [{1}] from [{2}] to [{3}]", incentiveAssigned.IncentiveQty
                                                                                                                   , incentiveAssigned.IncentiveName
                                                                                                                   , sender
                                                                                                                   , receiver);
            Watch.Stop();
            //in the centralized version the transactions are stored in the TransactionNode
            //create the transaction
            if (!this.TxNode.CreateTransaction(sender, receiver, incentiveAssigned, out transactionId))
            {
                Log.Error("Cannot Create transaction!");
                return false;
            }
            Log.DebugFormat("Transaction Create with Sucess!. ID: [{0}]", transactionId);
            return true;
        }

        public override List<Transaction> GetUserTransactions(string userId)
        {
            Transaction tx;
            List<Transaction> userTransactions = new List<Transaction>();
            Wallet wallet;

            if (!this.TxNode.GetUserWallet(userId, out wallet))
            {
                return null;
            }
            foreach (string transaction in wallet.Transactions)
            {
                tx = this.TxNode.GetTransactionById(transaction);

                if (tx != null)
                {
                    userTransactions.Add(tx);
                }
                else
                {
                    Log.ErrorFormat("Transaction ID: [{0}] does not exist", tx.TxID);
                }
            }
            return userTransactions;
        }

        public override bool RegisterUser(out string transactionId, out string userAddress, ref string errorMessage)
        {
            IncentiveAssigned incentiveAssigned;
            bool hasFunds;
            userAddress = null;
            transactionId = null;

            incentiveAssigned = Incentive.RegisterUserIncentive();
            userAddress = GenerateUserId();

            //register user on repository
            if (!this.Repository.RegisterUser(userAddress, this.Incentive))
            {
                return false;
            }
            
            //create user wallet on TransactionNode
            if (!this.TxNode.CreateUserWallet(userAddress, this.Incentive))
            {
                return false;
            }
            //register transaction
            if (!RegisterTransaction(this.ManagerAddress, userAddress, incentiveAssigned, out transactionId, out hasFunds))
            {
                Log.ErrorFormat("Cannot make transaction for registering user ID: [{0}]", userAddress);
                return false;
            }
            return true;
        }


        public override bool PayTask(string sender, out string transactionId, out bool hasFunds)
        {
            Watch.Start();
            transactionId = null;
            IncentiveAssigned incentiveAssigned;
            hasFunds = false;

            try
            {
                incentiveAssigned = Incentive.CreateTaskIncentive();

                //create the transaction and store it into the TransactionStorage
                log.DebugFormat("User: [{0}] going to pay [{1}] to create a Task", sender, incentiveAssigned.IncentiveQty);
                if (!RegisterTransaction(sender, this.ManagerAddress, incentiveAssigned, out transactionId, out hasFunds))
                {
                    Console.WriteLine("[ERROR - INCENTIVE ENGINE] - Cannot register task payment!");
                    return false;
                }
                Watch.Stop();
                Log.DebugFormat("Execution Time on IncentiveEngineNode Centralized: [{0}] milliseconds to Pay Task", Watch.ElapsedMilliseconds);
                Watch.Reset();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override bool RewardUser(string receiver, IncentiveAssigned incentiveAssigned, out object reward, out string transactionId)
        {
            Watch.Start();
            reward = null;
            transactionId = null;
            bool hasFunds = false;
            
            try
            {
                //obtain the reward for completing the task
                incentiveAssigned = Incentive.CompleteTaskIncentive();

                //create the transaction and store it into the TransactionStorage
                if (!RegisterTransaction(this.ManagerAddress, receiver, incentiveAssigned, out transactionId, out hasFunds))
                {
                    return false;
                }
                reward = new Dictionary<string, string>() { { "quantity", incentiveAssigned.IncentiveQty.ToString()},
                                                            { "incentive", incentiveAssigned.IncentiveName } };
                Watch.Stop();
                Log.DebugFormat("Execution Time on IncentiveEngineNode Centralized: [{0}] milliseconds to Reward User", Watch.ElapsedMilliseconds);
                Watch.Reset();
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat(ex.Message);
                return false;
            }
        }

        #region PRIVATE_METHODS

        private string GenerateUserId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        private Dictionary<string, int> GetUserBalance(string userID)
        {
            Wallet wallet;

            if (this.TxNode.GetUserWallet(userID, out wallet))
                return wallet.Balance;
            else
                return null;
        }

        private bool InitManager(Incentive incentive)
        {
            this.ManagerAddress = GenerateUserId();

            //register on repository the Manager
            if (!this.Repository.RegisterUser(ManagerAddress, incentive))
            {
                return false;
            }
            //create user wallet on TransactionNode
            if (!this.TxNode.CreateUserWallet(ManagerAddress, this.Incentive))
            {
                return false;
            }
            return true;
        }

        private bool IssueMoreIncentives(IncentiveAssigned incentiveAssigned)
        {
            if (!this.TxNode.IssueMoreIncentives(ManagerAddress, incentiveAssigned))
                return false;

            return true;
        }

        #endregion
    }
}
