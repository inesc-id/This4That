using Emitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace This4That_serverNode.Domain
{
    public class EmitterConn
    {
        private string serverKey;
        private Connection connection;

        public EmitterConn(string pServerKey)
        {
            this.serverKey = pServerKey;
        }

        public string ServerKey
        {
            get
            {
                return serverKey;
            }
        }

        public Connection Connection
        {
            get
            {
                return connection;
            }

            set
            {
                connection = value;
            }
        }

        public bool GenerateKey(string topic, out string channelKey)
        {
            int MAX_TIME = 2000;
            int actual_Time = 0;
            int INT_TIME = 500;
            channelKey = null;
            string tmp_ChannelKey = null;
            try
            {
                this.Connection.GenerateKey(serverKey, topic, Emitter.Messages.EmitterKeyType.ReadWrite,
                                        (response) => tmp_ChannelKey = response.Key);

                //await for generatedKey
                while (tmp_ChannelKey == null)
                {
                    Thread.Sleep(INT_TIME);
                    actual_Time += INT_TIME;
                    if (actual_Time >= MAX_TIME)
                        return false;
                }
                channelKey = tmp_ChannelKey;
                return true;
            }
            catch (Exception)
            {                
                return false;
            }            
        }
    }
}
