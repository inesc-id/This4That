using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test.IntegrationDTOs
{
    public class GetTransactionsDTO
    {
        private int errorCode;
        private string errorMessage;
        private List<Transaction> response;

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

        public List<Transaction> Response
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

    public class Transaction
    {
        [JsonProperty(PropertyName = "transactionId")]
        private string txID;
        [JsonProperty(PropertyName = "sender")]
        private string sender;
        [JsonProperty(PropertyName = "receiver")]
        private string receiver;
        [JsonProperty(PropertyName = "value")]
        private object value;
        [JsonProperty(PropertyName = "timestamp")]
        private long timestamp;

        
        public string TxID
        {
            get
            {
                return txID;
            }
        }

        public string Sender
        {
            get
            {
                return sender;
            }
        }

        public string Receiver
        {
            get
            {
                return receiver;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
        }

        public long Timestamp
        {
            get
            {
                return timestamp;
            }
        }
    }


}
