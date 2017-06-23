using System;
using System.Collections.Generic;

namespace This4That_library.Models.Incentives
{
    [Serializable]
    public class Gamification : Incentive
    {
        private const int WALLET_INIT_VALUE = 1000;
        private const int TASK_CREATION_VALUE = 100;
        private const int TASK_REWARD_VALUE = 50;
        private const int WALLET_EMPTY = 0;
        public const string GOLD_BADGE = "GOLD_BADGE";
        public const string SILVER_BADGE = "SILVER_BADGE";
        public const string BRONZE_BADGE = "BRONZE_BADGE";

        public Gamification(List<string> incentives) : base(incentives)
        {

        }

        public override bool CheckSufficientCredits(object balance, object incentiveValue)
        {
            int intPoints;
            int intBalance;

            int.TryParse(incentiveValue.ToString(), out intPoints);
            int.TryParse(balance.ToString(), out intBalance);

            if (intBalance < intPoints)
                return false;
            return true;
        }

        public override object CalcSenderNewBalance(object balance, object incentiveValue)
        {
            int intBalance;
            int intIncentiveValue;

            int.TryParse(balance.ToString(), out intBalance);
            int.TryParse(incentiveValue.ToString(), out intIncentiveValue);

            return intBalance - intIncentiveValue;
        }

        public override object CalcReceiverNewBalance(object balance, object incentiveValue)
        {
            int intBalance;
            int intIncentiveValue;

            int.TryParse(balance.ToString(), out intBalance);
            int.TryParse(incentiveValue.ToString(), out intIncentiveValue);

            return intBalance + intIncentiveValue;
        }

        public override object GetTaskReward()
        {
            return TASK_REWARD_VALUE;
        }

        public override int InitIncentiveQuantity()
        {
            return WALLET_INIT_VALUE;
        }

        public override List<string> GetIncentivesName()
        {
            return Incentives;
        }

        public override object WalletEmpty()
        {
            return WALLET_EMPTY;
        }

        public override int CreateTaskIncentiveQty()
        {
            return TASK_CREATION_VALUE;
        }

        public override string CreateTaskIncentiveName()
        {
            return this.Incentives.Find(elem => elem.Equals(GOLD_BADGE));
        }
    }
}
