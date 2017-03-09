using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using This4That_library;
using This4That_serverNode.Nodes;

namespace This4That_serverNode
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog Log
        {
            get
            {
                return log;
            }
        }

        static void Main(string[] args)
        {
            XmlDocument xmlDoc;
            TaskCreator taskCreator;
            TaskDistributor taskDistributor;
            Console.WriteLine("Press a Key to Start...");
            Console.ReadLine();
            if (!LoadXMLConfiguration(@"..\..\Config\configInstances.xml", @"..\..\Config\configInstances.xsd", "This4ThatNS", out xmlDoc))
                return;
            if (!StartInstances(xmlDoc, out taskCreator, out taskDistributor))
                return;

            Console.ReadLine();
        }


        private static bool LoadXMLConfiguration(string configXMLFileName, string configXSDFileName, string targetNS, out XmlDocument xmlDoc)
        {
            xmlDoc = null;
            try
            {
                XMLParser xmlParser = null;
                if (String.IsNullOrEmpty(configXMLFileName))
                {
                    Log.ErrorFormat("Invalid XML File Name: [{0}]", configXMLFileName);
                    return false;
                }
                xmlParser = new XMLParser(configXMLFileName, configXSDFileName, targetNS);
                if (!xmlParser.ValidateXML())
                    return false;
                xmlDoc = xmlParser.XmlDoc;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(ex.Message);
                return false;
            }
        }

        private static bool StartInstances(XmlDocument xmlDoc, out TaskCreator taskCreator, out TaskDistributor taskDistributor)
        {
            taskCreator = null;
            taskDistributor = null;
            int port;
            string serverMgrURL;
            XmlNode xmlNode;
            try
            {
                xmlNode = xmlDoc.GetElementsByTagName(Global.SERVER_MANAGER_NAME)[0];
                if (xmlNode != null)
                {
                    serverMgrURL = $"tcp://{xmlNode.Attributes["hostName"].Value}:{xmlNode.Attributes["port"].Value}/{Global.SERVER_MANAGER_NAME}";
                }
                else
                {
                    Program.Log.Error("Server Manager Parameters are not defined in the XML!");
                    return false;
                }
                xmlNode = xmlDoc.GetElementsByTagName(Global.TASK_CREATOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    taskCreator = new TaskCreator(xmlNode.Attributes["hostName"].Value, port, Global.TASK_CREATOR_NAME);
                    if (!StartConnectRemoteIntance(taskCreator, serverMgrURL))
                        return false;                   
                }
                xmlNode = xmlDoc.GetElementsByTagName(Global.TASK_DISTRIBUTOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    taskDistributor = new TaskDistributor(xmlNode.Attributes["hostName"].Value, port, Global.TASK_DISTRIBUTOR_NAME);
                    if (!StartConnectRemoteIntance(taskDistributor, serverMgrURL))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Register Remote Object.
        /// </summary>
        /// <param name="networkNode"></param>
        /// <returns></returns>
        private static bool StartConnectRemoteIntance(Node networkNode, string serverMgrURL)
        {
            TcpChannel channel;
            try
            {
                if (String.IsNullOrEmpty(networkNode.HostName) || networkNode.Port < 0)
                {
                    Log.ErrorFormat("Invalid Hostname: [{0}] or Port: [{1}]", networkNode.HostName, networkNode.Port);
                    return false;
                }
                //register remote instance
                Log.DebugFormat("Valid Hostname: [{0}] Port: [{1}]", networkNode.HostName, networkNode.Port);
                channel = new TcpChannel(networkNode.Port);
                ChannelServices.RegisterChannel(channel, false);
                RemotingServices.Marshal(networkNode, networkNode.Name, networkNode.GetType());
                Log.DebugFormat("Node: [{0}] IS RUNNING!", networkNode.Name);
                //connect remote instance to server manager
                if (!networkNode.ConnectServerManager(serverMgrURL))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}
