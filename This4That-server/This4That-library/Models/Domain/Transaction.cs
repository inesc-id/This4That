using System;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class Transaction
    {
        private string txID;
        private string sender;
        private string receiver;
        private object value;
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

        public Transaction(string ptxID, string pSender, string pReceiver, object pValue)
        {
            this.txID = ptxID;
            this.sender = pSender;
            this.receiver = pReceiver;
            this.value = pValue;
            this.timestamp = Library.GetUnixTimestamp(DateTime.Now);
        }

        public override string ToString()
        {
            return String.Format("TxID: [{0}] Sender:[{1}] Receiver:[{2}] IncentiveValue: [{3}] TimeStamp: [{4}]",
                                 TxID.ToString(), Sender.ToString(), Receiver.ToString(), Value.ToString(), Timestamp.ToString()); 
        }
    }
}
