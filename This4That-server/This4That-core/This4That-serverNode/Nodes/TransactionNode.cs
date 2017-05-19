using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_ServerNode.Nodes;

namespace This4That_ServerNode.Nodes
{
    public class TransactionNode : Node, ITransactionNode
    {
        IRepository remoteRepository = null;
        private TransactionStorage txStorage = new TransactionStorage();

        public IRepository RemoteRepository
        {
            get
            {
                return remoteRepository;
            }

            set
            {
                remoteRepository = value;
            }
        }

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

        public TransactionNode(string hostName, int port, string name) : base(hostName, port, name)
        {
            Log = LogManager.GetLogger("TransactionNodeLOG");
        }


        public override bool ConnectServerManager(string serverMgrURL)
        {
            //do nothing
            return true;
        }

        public Transaction GetTransactionById(string txId)
        {
            return this.TxStorage.GetTransaction(txId);
        }

        public bool GenerateTransaction(string sender, string receiver, object value, out string transactionID)
        {
            return this.TxStorage.GenerateTransaction(sender, receiver, value, out transactionID);
        }
    }
}
