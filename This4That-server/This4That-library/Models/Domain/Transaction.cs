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

        public Transaction(string ptxID, string pSender, string pReceiver, object pValue)
        {
            this.txID = ptxID;
            this.sender = pSender;
            this.receiver = pReceiver;
            this.value = pValue;
            this.timestamp = Library.GetUnixTimestamp(DateTime.Now);
        }
    }
}
