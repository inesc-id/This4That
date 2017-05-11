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
        private ValtoPay response;

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

        [JsonProperty(PropertyName = "response")]
        public ValtoPay Response
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

    public class ValtoPay
    {
        private object valToPay;

        [JsonProperty(PropertyName = "valToPay")]
        public object ValToPay
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
