using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;
using This4That_ServerNode.Nodes;

namespace This4That_ServerNode.Nodes
{
    public class TransactionNode : Node, ITransactionNode
    {
        private TransactionStorage txStorage = new TransactionStorage();
        private Stopwatch watch = new Stopwatch();

        //col of Wallets
        private Dictionary<string, Wallet> wallets = new Dictionary<string, Wallet>();

        public TransactionStorage TxStorage
        {
            get
            {
                return txStorage;
            }

            set
            {
                txStorage = value;
            }
        }

        public Dictionary<string, Wallet> Wallets
        {
            get
            {
                return wallets;
            }

            set
            {
                wallets = value;
            }
        }

        public TransactionNode(string hostName, int port, string name) : base(hostName, port, name, "TransactionNodeLOG")
        {
        }

        #region PUBLIC_METHODS

        public override bool ConnectServerManager()
        {
            //do nothing
            Console.WriteLine("----------------------------" + Environment.NewLine);
            return true;
        }

        #endregion



        #region REMOTE_INTERFACE

        public Transaction GetTransactionById(string txId)
        {
            return this.TxStorage.GetTransaction(txId);
        }

        public bool CreateTransaction(string sender, string receiver, IncentiveAssigned incentiveAssgined, out string transactionID)
        {
            watch.Start();
            Wallet senderWallet;
            Wallet receiverWallet;
            transactionID = null;

            try
            {
                if (!GetUserWallet(sender, out senderWallet))
                {
                    log.DebugFormat("Invalid UserId: [{0}]", sender);
                    return false;
                }
                if (!GetUserWallet(receiver, out receiverWallet))
                {
                    log.DebugFormat("Invalid UserId: [{0}]", receiver);
                    return false;
                }
                //calc sender balance
                senderWallet.Balance[incentiveAssgined.IncentiveName] -= incentiveAssgined.IncentiveQty;
                //calc receiver balance
                receiverWallet.Balance[incentiveAssgined.IncentiveName] += incentiveAssgined.IncentiveQty;
                //generate transaction
                if (!TxStorage.GenerateTransaction(sender, receiver, incentiveAssgined, out transactionID))
                {
                    Log.Error("ERROR. Cannot Generate Transaction!");
                    return false;
                }
                senderWallet.AssociateTransaction(transactionID);
                receiverWallet.AssociateTransaction(transactionID);
                watch.Stop();
                Log.DebugFormat("Execution Time: [{0}] in milliseconds to register transaction", watch.ElapsedMilliseconds);
                watch.Reset();
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(ex.Message);
                return false;
            }
        }

        public bool GetUserWallet(string userId, out Wallet wallet)
        {
            wallet = null;

            if (!Wallets.Keys.Contains(userId))
                return false;
            else
            {
                wallet = Wallets[userId];
                return true;
            }
        }

        public bool CreateUserWallet(string userAddress, Incentive incentive)
        {
            try
            {
                Wallet wallet = new Wallet(userAddress, incentive.InitIncentivesWallet());
                Wallets.Add(userAddress, wallet);
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(ex.Message);
                return false;
            }
        }

        public bool IssueMoreIncentives(string managerAddress, IncentiveAssigned incentiveAssigned)
        {
            try
            {
                this.Wallets[managerAddress].Balance[incentiveAssigned.IncentiveName] += incentiveAssigned.IncentiveQty;
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat(ex.Message);
                return false;
            }            
        }

        #endregion
    }
}
