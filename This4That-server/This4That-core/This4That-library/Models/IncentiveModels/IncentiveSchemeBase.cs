using System;
using System.Collections.Generic;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;

namespace This4That_library.Models.IncentiveModels
{
    [Serializable]
    public abstract class IncentiveSchemeBase
    {
        private Incentive incentive = null;
        private IRepository repository;

        public Incentive Incentive
        {
            get
            {
                return incentive;
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

        public IncentiveSchemeBase(IRepository repository, Incentive incentive)
        {
            this.incentive = incentive;
            this.Repository = repository;
        }

        public abstract bool PayTask(string sender, out string transactionId);
        public abstract bool RewardUser(string receiver, out object reward, out string transactionId);
        public abstract bool RegisterTransaction(string sender, string recipient, object incentiveValue, out string transactionId);
        public abstract bool CheckUserBalance(string userId, int incentiveQty, string incentiveName);
        public abstract List<Transaction> GetUserTransactions(string userId);
        public abstract bool RegisterUser(out string transactionId, out string userAddress, ref string errorMessage);
    }

}
