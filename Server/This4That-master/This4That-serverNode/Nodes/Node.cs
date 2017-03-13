using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Xml;
using This4That_library;

namespace This4That_serverNode.Nodes
{
    public abstract class Node : MarshalByRefObject
    {
        string hostName;
        int port;
        string name;
        IServerManager remoteServerMgr;

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
        #endregion


        public Node(string hostName, int port, string name)
        {
            this.HostName = hostName;
            this.Port = port;
            this.Name = name;
        }

        /// <summary>
        /// Get Remote reference to Server Manager.
        /// </summary>
        /// <param name="serverMgrURL"></param>
        /// <returns></returns>
        public abstract bool ConnectServerManager(string serverMgrURL);

        /// <summary>
        /// Register Remote Object.
        /// </summary>
        /// <param name="networkNode"></param>
        /// <returns></returns>
        public bool StartConnectRemoteIntance(string serverMgrURL)
        {
            TcpServerChannel channel;
            try
            {
                if (String.IsNullOrEmpty(this.HostName) || this.Port < 0)
                {
                    Program.Log.ErrorFormat("Invalid Hostname: [{0}] or Port: [{1}]", this.HostName, this.Port);
                    return false;
                }
                //register remote instance
                Program.Log.DebugFormat("Valid Hostname: [{0}] Port: [{1}]", this.HostName, this.Port);
                channel = new TcpServerChannel(this.Name, this.Port);
                ChannelServices.RegisterChannel(channel, false);
                RemotingServices.Marshal(this, this.Name, this.GetType());
                Program.Log.DebugFormat("Node: [{0}] IS RUNNING!", this.Name);
                //connect remote instance to server manager
                if (!this.ConnectServerManager(serverMgrURL))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Program.Log.Error(ex.Message);
                return false;
            }
        }

        
    }
}