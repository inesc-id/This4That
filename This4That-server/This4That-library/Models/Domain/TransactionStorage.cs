using System;
using System.Collections.Generic;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class TransactionStorage
    {
        private Dictionary<string, Transaction> transactions = new Dictionary<string, Transaction>();

        

        public bool GenerateTransaction(string sender, string receiver, object value, out string transactionID)
        {
            Transaction tx;
            transactionID = null;

            try
            {
                transactionID = Guid.NewGuid().ToString();
                lock (transactions)
                {
                    while (transactions.ContainsKey(transactionID))
                    {
                        transactionID = Guid.NewGuid().ToString();
                    }
                    tx = new Transaction(transactionID, sender, receiver, value);
                    transactions.Add(transactionID, tx);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Transaction GetTransaction(string txID)
        {
            if (transactions.ContainsKey(txID))
                return transactions[txID];
            else
                return null;
        }
    }
}
