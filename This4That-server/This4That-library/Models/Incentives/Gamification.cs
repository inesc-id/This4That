using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Domain;

namespace This4That_library.Models.Incentives
{
    [Serializable]
    public class Gamification : Incentive
    {
        private const int TASK_CREATION_VALUE = 200;


        public override object GetTaskCreationValue()
        {
            return TASK_CREATION_VALUE;
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

        internal override object CalcNewBalance(object balance, object incentiveValue)
        {
            int intBalance;
            int intIncentiveValue;

            int.TryParse(balance.ToString(), out intBalance);
            int.TryParse(incentiveValue.ToString(), out intIncentiveValue);

            return intBalance - intIncentiveValue;
        }

        internal override object InitWalletValue()
        {
            return 1000;
        }
    }
}
