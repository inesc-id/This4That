using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration;

namespace This4That_serverNode.IncentiveModels
{
    [Serializable]
    public class CentralIncentiveScheme : IncentiveSchemeBase
    {
        public override bool CalcTaskCost(CSTaskDTO taskSpec, out object incentiveValue)
        {
            incentiveValue = 50;
            return true;
        }
    }
}
