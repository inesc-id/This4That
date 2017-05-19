using Newtonsoft.Json;

namespace This4That_test.IntegrationDTOs
{
    public class ReportTaskDTO
    {
        private int errorCode;
        private string errorMessage;

        [JsonProperty(PropertyName = "response")]
        private ReportResultTask response;

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

        public ReportResultTask Response
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

    public class ReportResultTask
    {
        [JsonProperty(PropertyName = "reward")]
        private object reward;

        [JsonProperty(PropertyName = "txId")]
        private string txId;

        public object Reward
        {
            get
            {
                return reward;
            }

            set
            {
                reward = value;
            }
        }

        public string TxId
        {
            get
            {
                return txId;
            }

            set
            {
                txId = value;
            }
        }
    }
}
