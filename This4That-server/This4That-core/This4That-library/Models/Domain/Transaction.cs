using Newtonsoft.Json;
using System;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class Transaction
    {
        [JsonProperty(PropertyName = "transactionId")]
        private string txID;
        private string sender;
        private string receiver;
        private IncentiveAssigned incentive;
        private long timestamp;

        public string TxID
        {
            get
            {
                return txID;
            }
        }

        [JsonProperty(PropertyName = "sender")]
        public string Sender
        {
            get
            {
                return sender;
            }
        }

        [JsonProperty(PropertyName = "receiver")]
        public string Receiver
        {
            get
            {
                return receiver;
            }
        }

        [JsonProperty(PropertyName = "incentive")]
        public IncentiveAssigned Incentive
        {
            get
            {
                return incentive;
            }
        }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp
        {
            get
            {
                return timestamp;
            }
        }

        public Transaction(string ptxID, string pSender, string pReceiver, IncentiveAssigned pIncentive)
        {
            this.txID = ptxID;
            this.sender = pSender;
            this.receiver = pReceiver;
            this.incentive = pIncentive;
            this.timestamp = Library.GetUnixTimestamp(DateTime.Now);
        }

        public override string ToString()
        {
            return String.Format("TxID: [{0}] Sender:[{1}] Receiver:[{2}] IncentiveValue: [{3}] TimeStamp: [{4}]",
                                 TxID.ToString(), Sender.ToString(), Receiver.ToString(), Incentive.ToString(), Timestamp.ToString()); 
        }
    }
}
