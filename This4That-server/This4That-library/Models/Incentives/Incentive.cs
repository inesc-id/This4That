using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Domain;

namespace This4That_library.Models.Incentives
{
    [Serializable]
    public abstract class Incentive
    {
        public abstract object GetTaskCreationValue();

        public abstract bool CheckSufficientCredits(object balance, object incentiveValue);
        internal abstract object CalcNewBalance(object balance, object incentiveValue);
        internal abstract object InitWalletValue();
    }

    [Serializable]
    public enum IncentivesEnum
    {
        Gamification,
        Money
    }
}
