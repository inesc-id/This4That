using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test.IntegrationDTOs
{
    public class CalcTaskCostResponseDTO
    {
        private int errorCode;
        private string errorMessage;

        [JsonProperty(PropertyName = "response")]
        private Response response;

        [JsonProperty(PropertyName = "errorCode")]
        public int ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                errorCode = value;
            }
        }

        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }

        public Response Response
        {
            get
            {
                return response;
            }

            set
            {
                response = value;
            }
        }
    }

    public class Response
    {
        [JsonProperty(PropertyName = "incentiveToPay")]
        private IncentiveToPay incentiveToPay;

        public IncentiveToPay IncentiveToPay
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

    public class IncentiveToPay
    {
        [JsonProperty(PropertyName = "name")]
        private string name;

        [JsonProperty(PropertyName = "quantity")]
        private int quantity;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public int Quantity
        {
            get
            {
                return quantity;
            }

            set
            {
                quantity = value;
            }
        }
    }
}
