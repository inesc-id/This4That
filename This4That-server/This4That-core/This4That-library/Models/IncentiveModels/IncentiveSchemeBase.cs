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


        public object CalcTaskCost(CSTaskDTO taskSpec)
        {
            return this.Incentive.GetTaskCreationValue();
        }
        public bool CanPerformTransaction(object balance, object incentiveValue)
        {
            if (!Incentive.CheckSufficientCredits(balance, incentiveValue))
                return false;

            return true;
        }

        public abstract bool RegisterTransaction(string sender, string recipient, object incentiveValue, out string transactionId);
        public abstract object CheckUserBalance(string userId);
        public abstract List<Transaction> GetUserTransactions(string userId);
        public abstract bool SaveCreateUserTransaction(string userId, object incentiveValue, out string transactionId, out string userAddress, ref string errorMessage);
    }

    [Serializable]
    public enum IncentiveSchemesEnum
    {
        None,
        Centralized,
        Descentralized
    }
}
