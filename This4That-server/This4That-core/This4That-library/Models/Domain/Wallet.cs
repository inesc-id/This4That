using System;
using System.Collections.Generic;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class Wallet
    {
        private object balance = null;
        private List<string> transactions = new List<string>();

        public object Balance
        {
            get
            {
                return balance;
            }

            set
            {
                balance = value;
            }
        }

        public List<string> Transactions
        {
            get
            {
                return transactions;
            }

            set
            {
                transactions = value;
            }
        }

        public void AssociateTransaction(string transactionId)
        {
            Transactions.Add(transactionId);
        }
    }
}
