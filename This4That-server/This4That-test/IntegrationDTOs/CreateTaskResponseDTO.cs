using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test.IntegrationDTOs
{
    public class CreateTaskResponseDTO
    {
        private int errorCode;
        private string errorMessage;
        private TransactionInfo response;

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
        public TransactionInfo Response
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

    public class TransactionInfo
    {
        private string taskID;
        private string transactionID;

        [JsonProperty(PropertyName = "taskId")]
        public string TaskID
        {
            get
            {
                return taskID;
            }

            set
            {
                taskID = value;
            }
        }

        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionID
        {
            get
            {
                return transactionID;
            }

            set
            {
                transactionID = value;
            }
        }
    }
}
