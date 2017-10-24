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
        public const string POINTS = "POINTS";

        public Gamification(List<string> incentives) : base(incentives)
        {

        }

        public override bool CheckSufficientCredits(object balance, object incentiveValue)
        {
            if ((int)balance < (int)incentiveValue)
                return false;
            return true;
        }

        public override List<string> GetIncentivesName()
        {
            return Incentives;
        }

        public override Dictionary<string, int> InitIncentivesWallet()
        {
            Dictionary<string, int> incentivesDict = new Dictionary<string, int>();

            foreach (string incentive in Incentives)
            {
                incentivesDict.Add(incentive, WALLET_EMPTY);
            }
            return incentivesDict;
        }

        public override IncentiveAssigned RegisterUserIncentive()
        {
            return new IncentiveAssigned(POINTS, WALLET_INIT_VALUE);
        }

        public override IncentiveAssigned CreateTaskIncentive()
        {
            return new IncentiveAssigned(POINTS, TASK_CREATION_VALUE);
        }

        public override IncentiveAssigned CompleteTaskIncentive()
        {
            return new IncentiveAssigned(POINTS, TASK_REWARD_VALUE);
        }
    }
}
