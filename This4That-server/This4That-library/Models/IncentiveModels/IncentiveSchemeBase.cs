using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Domain;

namespace This4That_serverNode.IncentiveModels
{
    [Serializable]
    public abstract class IncentiveSchemeBase
    {
        public abstract bool CalcTaskCost(CSTask taskSpec, out object incentiveValue);
    }
}
