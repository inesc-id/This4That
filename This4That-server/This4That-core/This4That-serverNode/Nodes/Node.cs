using log4net;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;
using This4That_library;

namespace This4That_ServerNode.Nodes
{
    public abstract class Node : MarshalByRefObject
    {
        private string hostName;
        private int port;
        private string name;
        private IServerManager remoteServerMgr;
        protected ILog log;

        #region PROPERTIES
        public string HostName
        {
            get
            {
                return hostName;
            }

            set
            {
                hostName = value;
            }
        }
        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public IServerManager RemoteServerMgr
        {
            get
            {
                return remoteServerMgr;
            }

            set
            {
                remoteServerMgr = value;
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
        #endregion


        public Node(string hostName, int port, string name, string loggerName)
        {
            this.HostName = hostName;
            this.Port = port;
            this.Name = name;
            this.Log = LogManager.GetLogger(loggerName);
            StartConnectRemoteInstance();

        }

        public override Object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();

            // Normally, the initial lease time would be much longer.
            // It is shortened here for demonstration purposes.
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.FromSeconds(0);
            }
            return lease;
        }

        /// <summary>
        /// Get Remote reference to Server Manager.
        /// </summary>
        /// <param name="serverMgrURL"></param>
        /// <returns></returns>
        public abstract bool ConnectServerManager();

        /// <summary>
        /// Register Remote Object.
        /// </summary>
        /// <param name="networkNode"></param>
        /// <returns></returns>
        private bool StartConnectRemoteInstance()
        {
            TcpServerChannel channel;
            try
            {
                Log.DebugFormat("NAME: [{0}] HOST: [{1}] PORT: [{2}]", Name, HostName, Port);
                if (String.IsNullOrEmpty(this.HostName) || this.Port < 0)
                {
                    Log.Error("INVALID HOSTNAME OR PORT!");
                    return false;
                }
                channel = new TcpServerChannel(this.Name, this.Port);
                ChannelServices.RegisterChannel(channel, false);
                RemotingServices.Marshal(this, this.Name, this.GetType());
                Log.DebugFormat("Listening on Port: [{0}]", Port);
                //connect remote instance to server manager
                if (!this.ConnectServerManager())
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
    }
}