using System;
using System.Collections.Generic;

namespace This4That_library.Models.Incentives
{
    [Serializable]
    public abstract class Incentive
    {
        private List<string> incentives;

        public List<string> Incentives
        {
            get
            {
                return incentives;
            }
        }

        public Incentive(List<string> incentives)
        {
            this.incentives = incentives;
        }

        public abstract bool CheckSufficientCredits(object balance, object incentiveValue);
        public abstract object CalcSenderNewBalance(object balance, object incentiveValue);
        public abstract object CalcReceiverNewBalance(object balance, object incentiveValue);
        public abstract int CreateTaskIncentiveQty();
        public abstract int InitIncentiveQuantity();
        public abstract object WalletEmpty();
        public abstract object GetTaskReward();
        public abstract List<string> GetIncentivesName();
        public abstract string CreateTaskIncentiveName();
    }

    [Serializable]
    public enum IncentivesEnum
    {
        Gamification,
        Money
    }
}
