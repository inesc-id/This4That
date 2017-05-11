using System;
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
            try
            {
                //create the transaction and calculate the new balances for the sender and receiver
                repository.GenerateTransaction(sender, receiver, IncentiveType, incentiveValue, out transactionId);
                //associate in the user wallet the transaction ID
                repository.AssociateTransactionUser(sender, receiver, transactionId);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
