using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.Integration.CalcTaskCostDTO
{
    public class CalcTaskCostResponseDTO
    {
        private IncentiveAssigned incentiveToPay;

        [JsonProperty(PropertyName = "incentiveToPay")]
        public IncentiveAssigned IncentiveToPay
        {
            get
            {
                return incentiveToPay;
            }

            set
            {
                incentiveToPay = value;
            }
        }

    }
}
