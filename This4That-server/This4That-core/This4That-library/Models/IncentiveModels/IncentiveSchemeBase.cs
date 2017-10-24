using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;

namespace This4That_library.Models.IncentiveModels
{
    [Serializable]
    public abstract class IncentiveSchemeBase
    {
        private Incentive incentive = null;
        private IRepository repository;
        private ITransactionNode txNode;
        private string managerAddress;
        protected ILog log;
        private Stopwatch watch;

        public Incentive Incentive
        {
            get
            {
                return incentive;
            }
        }

        public IRepository Repository
        {
            get
            {
                return repository;
            }
            set
            {
                repository = value;
            }
        }

        protected ILog Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }

        public string ManagerAddress
        {
            get
            {
                return managerAddress;
            }

            set
            {
                managerAddress = value;
            }
        }

        public ITransactionNode TxNode
        {
            get
            {
                return txNode;
            }

            set
            {
                txNode = value;
            }
        }

        public Stopwatch Watch
        {
            get
            {
                return watch;
            }

            set
            {
                watch = value;
            }
        }

        public IncentiveSchemeBase(IRepository repository, ITransactionNode txNode, Incentive incentive)
        {
            this.incentive = incentive;
            this.Repository = repository;
            this.TxNode = txNode;
            this.log = LogManager.GetLogger("TransactionLOG");
            this.watch = new Stopwatch();
        }

        public abstract bool PayTask(string sender, out string transactionId, out bool hasFunds);
        public abstract bool RewardUser(string receiver, IncentiveAssigned incentiveAssigned, out object reward, out string transactionId);
        public abstract bool RegisterTransaction(string sender, string recipient, IncentiveAssigned incentiveAssigned, out string transactionId, out bool hasFunds);
        public abstract bool CheckUserBalance(string userId, IncentiveAssigned incentiveAssigned);
        public abstract List<Transaction> GetUserTransactions(string userId);
        public abstract bool RegisterUser(out string transactionId, out string userAddress, ref string errorMessage);
    }

}
