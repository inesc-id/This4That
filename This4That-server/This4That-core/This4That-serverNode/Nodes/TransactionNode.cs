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
using This4That_ServerNode.Nodes;

namespace This4That_ServerNode.Nodes
{
    public class TransactionNode : Node, ITransactionNode
    {
        private TransactionStorage txStorage = new TransactionStorage();

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

        public TransactionNode(string hostName, int port, string name) : base(hostName, port, name, "TransactionNodeLOG")
        {
            Console.WriteLine("TRANSACTION NODE");
            Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port}");
            Console.WriteLine("----------------------------" + Environment.NewLine);
        }

        #region PUBLIC_METHODS

        public override bool ConnectServerManager()
        {
            //do nothing
            return true;
        }

        #endregion



        #region REMOTE_INTERFACE

        public Transaction GetTransactionById(string txId)
        {
            return this.TxStorage.GetTransaction(txId);
        }

        public bool CreateTransaction(string sender, string receiver, object value, out string transactionID)
        {
            return this.TxStorage.GenerateTransaction(sender, receiver, value, out transactionID);
        }

        #endregion
    }
}
