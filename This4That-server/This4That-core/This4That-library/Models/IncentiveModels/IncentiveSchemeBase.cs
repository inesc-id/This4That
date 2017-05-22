using System;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;

namespace This4That_library.Models.IncentiveModels
{
    [Serializable]
    public abstract class IncentiveSchemeBase
    {
        Incentive incentive = null;

        public Incentive IncentiveType
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
            return this.IncentiveType.GetTaskCreationValue();
        }
        
        public bool CanPerformTransaction(object balance, object incentiveValue)
        {
            if (!IncentiveType.CheckSufficientCredits(balance, incentiveValue))
                return false;

            return true;
        }

        public abstract bool RegisterPayment(IRepository repository, string sender, string recipient, object incentiveValue, out string transactionId);
        public abstract object CheckUserBalance(IRepository repository, string userId);
    }

    [Serializable]
    public enum IncentiveSchemesEnum
    {
        None,
        Centralized,
        Descentralized
    }
}
