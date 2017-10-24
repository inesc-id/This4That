using System;
using System.Collections.Generic;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class Wallet
    {
        private Dictionary<string, int> balance = null;
        private List<string> transactions = new List<string>();

        private string walletAddress = null;

        public Wallet(string userAddress, Dictionary<string, int> userInitBalance)
        {
            this.WalletAddress = userAddress;
            this.Balance = userInitBalance;
        }

        public Dictionary<string, int> Balance
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
