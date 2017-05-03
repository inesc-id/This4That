using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Domain;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;

namespace This4That_library.Models.IncentiveModels
{
    [Serializable]
    public abstract class IncentiveSchemeBase
    {
        Incentive incentive = null;

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
        
        public bool HasUserSufficientCredits(object balance, object incentiveValue)
        {
            if (!Incentive.CheckSufficientCredits(balance, incentiveValue))
                return false;

            return true;
        }

        public abstract bool RegisterTaskPayment(IRepository repository, string userId, object incentiveValue, out string transactionId);
    }

    [Serializable]
    public enum IncentiveSchemesEnum
    {
        Centralized,
        Descentralized
    }
}
