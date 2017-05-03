using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;


namespace This4That_library.Models.IncentiveModels
{
    public class DescentralizedIncentiveScheme : IncentiveSchemeBase
    {
        public DescentralizedIncentiveScheme(Incentive incentive) : base(incentive)
        {

        }

        public override bool RegisterTaskPayment(IRepository repository, string userId, object incentiveValue, out string transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
