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

        public Incentive Incentive
        {
            get
            {
                return incentive;
            }
        }

        public IncentiveSchemeBase(Incentive incentive)
        {
            this.incentive = incentive;
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

        public abstract bool RegisterTransaction(IRepository repository, string sender, string recipient, object incentiveValue, out string transactionId);
        public abstract object CheckUserBalance(IRepository repository, string userId);
        public abstract List<Transaction> GetUserTransactions(IRepository repository, string userId);
    }

    [Serializable]
    public enum IncentiveSchemesEnum
    {
        None,
        Centralized,
        Descentralized
    }
}
