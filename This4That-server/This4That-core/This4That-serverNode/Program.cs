using System;
using System.Xml;
using This4That_library;
using This4That_ServerNode.Nodes;

namespace This4That_ServerNode
{
    class Program
    {
        static void Main(string[] args)
        {
            XMLParser xmlParser;
            string errorMessage = null;

            TransactionNode transactionNode;
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

            StartInstances(xmlParser.XmlDoc, out taskCreator, out taskDistributor, out reportAggregator
                           , out incentiveEngine, out repository, out transactionNode);

            Console.ReadLine();
        }


        private static bool StartInstances(XmlDocument xmlDoc, out TaskCreator taskCreator, out TaskDistributor taskDistributor, 
                                           out ReportAggregator reportAggregator, out IncentiveEngine incentiveEngine, out Repository repository,
                                           out TransactionNode transactionNode)
        {
            taskCreator = null;
            taskDistributor = null;
            reportAggregator = null;
            incentiveEngine = null;
            repository = null;
            transactionNode = null;
            int port;
            string serverMgrURL;
            string repositoryURL = null;
            string transactionNodeURL = null;
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
                Console.WriteLine("[SERVER MANAGER] - Node Started");
                Console.WriteLine("----------------------------" + Environment.NewLine);
                
                /*********REPOSITORY**************/
                xmlNode = xmlDoc.GetElementsByTagName(Global.REPOSITORY_NAME)[0];
                if (xmlNode != null)
                {
                    repositoryURL = $"tcp://{xmlNode.Attributes["hostName"].Value}:{xmlNode.Attributes["port"].Value}/{Global.REPOSITORY_NAME}";
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    repository = new Repository(xmlNode.Attributes["hostName"].Value, port, Global.REPOSITORY_NAME);
                }

                /**************TRANSACTION NODE********/
                xmlNode = xmlDoc.GetElementsByTagName(Global.TRANSACTION_NODE_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    transactionNode = new TransactionNode(xmlNode.Attributes["hostName"].Value, port, Global.TRANSACTION_NODE_NAME);

                    transactionNodeURL = $"tcp://{xmlNode.Attributes["hostName"].Value}:{xmlNode.Attributes["port"].Value}/{Global.TRANSACTION_NODE_NAME}";
                    if (!repository.ConnectoTransactionNode(transactionNodeURL))
                        return false;
                }

                /**************TASK CREATOR********/
                xmlNode = xmlDoc.GetElementsByTagName(Global.TASK_CREATOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    taskCreator = new TaskCreator(xmlNode.Attributes["hostName"].Value, port, Global.TASK_CREATOR_NAME);
                }
                /***********TASK DISTRIBUTOR********/
                xmlNode = xmlDoc.GetElementsByTagName(Global.TASK_DISTRIBUTOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    taskDistributor = new TaskDistributor(xmlNode.Attributes["hostName"].Value, port, Global.TASK_DISTRIBUTOR_NAME);
                }
                /*********REPORT AGGREGATOR**********/
                xmlNode = xmlDoc.GetElementsByTagName(Global.REPORT_AGGREGATOR_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    reportAggregator = new ReportAggregator(xmlNode.Attributes["hostName"].Value, port, Global.REPORT_AGGREGATOR_NAME);
                }
                /***********INCENTIVE ENGINE********/
                xmlNode = xmlDoc.GetElementsByTagName(Global.INCENTIVE_ENGINE_NAME)[0];
                if (xmlNode != null)
                {
                    int.TryParse(xmlNode.Attributes["port"].Value, out port);
                    incentiveEngine = new IncentiveEngine(xmlNode.Attributes["hostName"].Value, port, Global.INCENTIVE_ENGINE_NAME);
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
