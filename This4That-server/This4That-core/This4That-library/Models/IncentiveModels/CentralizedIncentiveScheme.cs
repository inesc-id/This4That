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

        public override object CheckUserBalance(string userId)
        {
            return Repository.GetUserBalance(userId);
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

        public override bool SaveCreateUserTransaction(string userId, object initValue, out string transactionId, out string userAddress, ref string errorMessage)
        {
            userAddress = null;
            //register transaction
            if (!RegisterTransaction("Platform", userId, initValue, out transactionId))
                return false;

            return true;
        }
    }
}
