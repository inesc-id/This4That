using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
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
            Console.WriteLine("TRANSACTION NODE");
            Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port}");
            Console.WriteLine("----------------------------" + Environment.NewLine);
            Log = LogManager.GetLogger("TransactionNodeLOG");
        }


        public override bool ConnectServerManager(string serverMgrURL)
        {
            //do nothing
            return true;
        }

        public bool StartRemoteTransactionNodeInstance()
        {
            TcpServerChannel channel;
            try
            {
                if (String.IsNullOrEmpty(this.HostName) || this.Port < 0)
                {
                    Log.ErrorFormat("Invalid Hostname: [{0}] or Port: [{1}]", this.HostName, this.Port);
                    return false;
                }
                //register remote instance
                Log.DebugFormat("Valid Hostname: [{0}] Port: [{1}]", this.HostName, this.Port);
                channel = new TcpServerChannel(this.Name, this.Port);
                ChannelServices.RegisterChannel(channel, false);
                RemotingServices.Marshal(this, this.Name, this.GetType());
                Log.DebugFormat("Node: [{0}] IS RUNNING!", this.Name);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
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
