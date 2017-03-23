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
        static void Main(string[] args)
        {
            XMLParser xmlParser;
            string errorMessage = null;

            TaskCreator taskCreator;
            TaskDistributor taskDistributor;
            ReportAggregator reportAggregator;
            IncentiveEngine incentiveEngine;
            Repository repository;

            Console.WriteLine("Press a Key to Start...");
            Console.ReadLine();
            //parsing config file
            xmlParser = new XMLParser(@"..\..\Config\configInstances.xml", @"..\..\Config\configInstances.xsd", "This4ThatNS");
            if (!xmlParser.LoadXMLConfiguration(ref errorMessage))
            {
                Console.WriteLine(errorMessage);
                return;
            }
            else
                Console.WriteLine(errorMessage);

            StartInstances(xmlParser.XmlDoc, out taskCreator, out taskDistributor, out reportAggregator, out incentiveEngine, out repository);

            Console.ReadLine();
        }


        private static bool StartInstances(XmlDocument xmlDoc, out TaskCreator taskCreator, out TaskDistributor taskDistributor, 
                                           out ReportAggregator reportAggregator, out IncentiveEngine incentiveEngine, out Repository repository)
        {
            taskCreator = null;
            taskDistributor = null;
            reportAggregator = null;
            incentiveEngine = null;
            repository = null;
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
                    Console.WriteLine("Server Manager Parameters are not defined in the XML!");
                    return false;
                }
                Console.WriteLine("SERVER MANAGER");
                Console.WriteLine(serverMgrURL);
                Console.WriteLine("----------------------------");
                xmlNode = xmlDoc.GetElementsByTagName(Global.TASK_CREATOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    taskCreator = new TaskCreator(xmlNode.Attributes["hostName"].Value, port, Global.TASK_CREATOR_NAME);
                    if (!taskCreator.StartConnectRemoteIntance(serverMgrURL))
                        return false;    
                }
                xmlNode = xmlDoc.GetElementsByTagName(Global.TASK_DISTRIBUTOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    taskDistributor = new TaskDistributor(xmlNode.Attributes["hostName"].Value, port, Global.TASK_DISTRIBUTOR_NAME);
                    if (!taskDistributor.StartConnectRemoteIntance(serverMgrURL))
                        return false;
                }
                xmlNode = xmlDoc.GetElementsByTagName(Global.REPORT_AGGREGATOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    reportAggregator = new ReportAggregator(xmlNode.Attributes["hostName"].Value, port, Global.REPORT_AGGREGATOR_NAME);
                    if (!reportAggregator.StartConnectRemoteIntance(serverMgrURL))
                        return false;
                }
                xmlNode = xmlDoc.GetElementsByTagName(Global.INCENTIVE_ENGINE_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    incentiveEngine = new IncentiveEngine(xmlNode.Attributes["hostName"].Value, port, Global.INCENTIVE_ENGINE_NAME);
                    if (!incentiveEngine.StartConnectRemoteIntance(serverMgrURL))
                        return false;
                }
                xmlNode = xmlDoc.GetElementsByTagName(Global.REPOSITORY_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    repository = new Repository(xmlNode.Attributes["hostName"].Value, port, Global.REPOSITORY_NAME);
                    if (!repository.StartConnectRemoteIntance(serverMgrURL))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
