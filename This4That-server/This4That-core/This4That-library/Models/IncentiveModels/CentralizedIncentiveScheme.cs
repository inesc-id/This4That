using System;
using System.Collections.Generic;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.IncentiveModels
{
    [Serializable]
    public class CentralizedIncentiveScheme : IncentiveSchemeBase
    {
        public CentralizedIncentiveScheme(Incentive incentive) : base(incentive)
        {

        }

        public override object CheckUserBalance(IRepository repository, string userId)
        {
            return repository.GetUserBalance(userId);
        }

        public override bool RegisterPayment(IRepository repository, string sender, string receiver, object incentiveValue, out string transactionId)
        {
            //in the centralized version the transactions are stored in the TransactionNode
            //create the transaction
            if (!repository.CreateTransactionCentralized(sender, receiver, IncentiveType, incentiveValue, out transactionId) || transactionId == null)
            {
                return false;
            }
            //calculate the new balances for the sender and receiver
            //associate in the user wallet the transaction ID
            if (!repository.ExecuteTransactionCentralized(sender, receiver, IncentiveType, incentiveValue, transactionId))
                return false;

            return true;
        }

        public override List<Transaction> GetUserTransactions(IRepository repository, string userId)
        {
            return repository.GetUserTransactionsCentralized(userId);
        }

    }
}
