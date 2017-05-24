using System;
using System.Collections.Generic;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class Wallet
    {
        private object balance = null;
        private List<KeyValuePair<IncentiveSchemesEnum, string>> transactions = new List<KeyValuePair<IncentiveSchemesEnum, string>>();
        private List<string> chainAddresses = new List<string>();

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

        public List<KeyValuePair<IncentiveSchemesEnum, string>> Transactions
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

        public List<string> ChainAddresses
        {
            get
            {
                return chainAddresses;
            }

            set
            {
                chainAddresses = value;
            }
        }

        public void AssociateTransaction(string transactionId, IncentiveSchemesEnum incentiveScheme)
        {
            Transactions.Add(new KeyValuePair<IncentiveSchemesEnum, string>(incentiveScheme, transactionId));
        }
    }
}
