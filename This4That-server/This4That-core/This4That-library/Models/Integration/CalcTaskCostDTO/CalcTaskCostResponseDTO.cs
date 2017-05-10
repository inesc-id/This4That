using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Integration.CalcTaskCostDTO
{
    public class CalcTaskCostResponseDTO
    {
        private string valToPay;

        [JsonProperty(PropertyName = "valToPay")]
        public string ValToPay
        {
            get
            {
                return valToPay;
            }

            set
            {
                valToPay = value;
            }
        }
    }

}
