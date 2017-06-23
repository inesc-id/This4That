using System;
using System.Collections.Generic;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.IncentiveModels
{
    [Serializable]
    public class CentralizedIncentiveScheme : IncentiveSchemeBase
    {
        public CentralizedIncentiveScheme(IRepository repository, Incentive incentive) : base(repository, incentive)
        {

        }

        public override bool CheckUserBalance(string userId, int incentiveQty, string incentiveName)
        {
            /*
            object balance; 

            balance = Repository.GetUserBalance(userId);

            if (!Incentive.CheckSufficientCredits(balance, incentiveQty))
                return false;*/

            return false;
        }

        public override bool RegisterTransaction(string sender, string receiver, object incentiveValue, out string transactionId)
        {
            //in the centralized version the transactions are stored in the TransactionNode
            //create the transaction
            if (!Repository.CreateTransactionCentralized(sender, receiver, incentiveValue, out transactionId) || transactionId == null)
            {
                return false;
            }
            //calculate the new balances for the sender and receiver
            //associate in the user wallet the transaction ID
            if (!Repository.ExecuteTransactionCentralized(sender, receiver, Incentive, incentiveValue, transactionId))
                return false;

            return true;
        }

        public override List<Transaction> GetUserTransactions(string userId)
        {
            return Repository.GetUserTransactionsCentralized(userId);
        }

        public override bool RegisterUser(out string transactionId, out string userAddress, ref string errorMessage)
        {
            //FIXME: alterar initValue, para quantity e o incentivo atribuido
            object initValue = Incentive.InitIncentiveQuantity();
            userAddress = GenerateUserId();

            //register user on repository
            this.Repository.RegisterUser(userAddress, this.Incentive);

            //register transaction
            if (!RegisterTransaction("Platform", userAddress, initValue, out transactionId))
                return false;

            return true;
        }

        public override bool PayTask(string sender, out string transactionId)
        {
            transactionId = null;
            string incentiveName;
            int assetQty;
            try
            {
                incentiveName = this.Incentive.CreateTaskIncentiveName();
                assetQty = this.Incentive.CreateTaskIncentiveQty();

                //FIXME: in this version the object Incentive must distinct the balance between the Incentives Objects

                //check if user has sufficient credits, depending the incentive type
                if (!CheckUserBalance(sender, assetQty, incentiveName))
                {
                    //return true and tx = null for insuf. funds
                    Console.WriteLine("[INFO - INCENTIVE ENGINE] - User: [{0}] Insufficient Balance!", sender);
                    transactionId = null;
                    return true;
                }
                //create the transaction and store it into the TransactionStorage
                if (!RegisterTransaction(sender, "Platform", assetQty, out transactionId))
                {
                    Console.WriteLine("[ERROR - INCENTIVE ENGINE] - Cannot register task payment!");
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override bool RewardUser(string receiver, out object reward, out string transactionId)
        {
            reward = null;
            transactionId = null;
            try
            {
                //obtain the reward for completing the task
                object taskReward = Incentive.GetTaskReward();

                //create the transaction and store it into the TransactionStorage
                if (!RegisterTransaction("Platform", receiver, taskReward, out transactionId))
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        #region PRIVATE_METHODS

        private string GenerateUserId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
        #endregion
    }
}
