using Emitter;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using This4That_library;
using This4That_library.Models.Domain;

namespace This4That_serverNode.Nodes
{
    public class TaskCreator : Node, ITaskCreator
    {
        private List<CSTask> onGoingTasks = new List<CSTask>();
        private Connection emitterConn = null;
        private string channelKey;

        public TaskCreator(string hostName, int port, string name) : base(hostName, port, name)
        {
            Log = LogManager.GetLogger("TaskCreatorLOG");
            ConnectToEmmiterBroker();
        }

        ~TaskCreator()
        {
            emitterConn.Disconnect();
        }

        /// <summary>
        /// Get Remote reference to Server Manager.
        /// </summary>
        /// <param name="serverMgrURL"></param>
        /// <returns></returns>
        public override bool ConnectServerManager(string serverMgrURL)
        {
            try
            {
                this.RemoteServerMgr = (IServerManager)Activator.GetObject(typeof(IServerManager), serverMgrURL);
                if (!this.RemoteServerMgr.RegisterTaskCreatorNode($"tcp://{this.HostName}:{this.Port}/{Global.TASK_CREATOR_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                }
                Log.DebugFormat("ServerManager: [{0}]", serverMgrURL);
                Console.WriteLine("TASK CREATOR");
                Console.WriteLine($"HOST: {this.HostName} PORT: {this.Port} CONNECTED to ServerManager");
                Console.WriteLine("----------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect TaskCreator to ServerManager: [{0}", serverMgrURL);
                return false;
            }
        }


        /// <summary>
        /// Connects to Emitter broker
        /// </summary>
        /// <param name="emitter"></param>
        /// <returns></returns>
        private bool ConnectToEmmiterBroker()
        {
            string channelKey = null;
            string serverKey = "MMVo_zBg1ZpRxMtxVIBixil3ofpIxdeZ";

            try
            {
                this.emitterConn = new Connection("192.168.128.128", 5010, serverKey);
                this.emitterConn.Connect();
                this.emitterConn.GenerateKey(
                                 serverKey,
                                 "This4That", Emitter.Messages.EmitterKeyType.ReadWrite,
                                 (response) => channelKey = response.Key);

                //await for generatedKey
                while (channelKey == null)
                {
                    Thread.Sleep(500);
                }
                Log.DebugFormat("Emitter Generated Channel Key: [{0}]", channelKey);
                this.channelKey = channelKey;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                this.emitterConn = null;
                return false;
            }
        }

        public void EmitterReceiver(string channelKey)
        {
            this.emitterConn.On(channelKey, "This4That", (channel, msg) =>
            {
                Console.WriteLine("[MQTT - Message]: " + Encoding.UTF8.GetString(msg));
            });
        }
        #region REMOTE_INTERFACE

        public bool CreateTask(CSTask task, out string taskID)
        {
            taskID = Guid.NewGuid().ToString();
            Log.DebugFormat("TaskCreator : TaskID: [{0}]", taskID);
            EmitterReceiver(channelKey);
            emitterConn.Publish("This4That", "Teste");
            emitterConn.Disconnect();
            return true;
        }

        #endregion
    }
}