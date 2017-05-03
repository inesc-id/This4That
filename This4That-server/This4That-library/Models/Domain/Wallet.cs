using System;
using System.Collections.Generic;
using This4That_library.Models.Incentives;

namespace This4That_library.Domain
{
    [Serializable]
    public class Wallet
    {
        private object balance = null;
        private List<string> Transactions = new List<string>();
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



        public void AssociateTransaction(Incentive incentive, object incentiveValue, string transactionId)
        {
            Balance = incentive.CalcNewBalance(Balance, incentiveValue);
            Transactions.Add(transactionId);
        }
    }
}
