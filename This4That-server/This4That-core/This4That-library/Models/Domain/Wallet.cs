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
        private List<string> transactions = new List<string>();
        private List<string> chainNodesAddresses = new List<string>();
        private string walletAddress = null;

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

        public List<string> ChainNodesAddresses
        {
            get
            {
                return chainNodesAddresses;
            }

            set
            {
                chainNodesAddresses = value;
            }
        }

        public string WalletAddress
        {
            get
            {
                return walletAddress;
            }

            set
            {
                walletAddress = value;
            }
        }

        public void AssociateTransaction(string transactionId)
        {
            Transactions.Add(transactionId);
        }
    }
}
