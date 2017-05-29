using MultiChainLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using This4That_library.Models.Incentives;
using System.Collections.Generic;
using This4That_library.Models.Domain;

namespace This4That_library.Models.IncentiveModels
{
    public class DescentralizedIncentiveScheme : IncentiveSchemeBase
    {

        private MultiChainClient multichainClient;

        public MultiChainClient MultichainClient
        {
            get
            {
                return multichainClient;
            }

            set
            {
                multichainClient = value;
            }
        }

        public DescentralizedIncentiveScheme(Incentive incentive) : base(incentive)
        {
            bool chainAlreadyExist = false;
            int chainPort;
            string username;
            string password;
            string chainName;

            //create the blockchain if it does not exist
            CreateBlockChain(out chainAlreadyExist);
            //start the blockchain
            StartBlockChain();
            //load the multichain parameters
            LoadMultichainParameters(out chainPort, out username, out password, out chainName);

            //instanciates a multi-chain client
            this.MultichainClient = new MultiChainClient("localhost", chainPort, false, username, password, chainName);
            var response = this.MultichainClient.GetInfoAsync();

            Console.WriteLine("[INFO] - Connected to Blockchain : [" + response.Result.Result.ChainName + "] with SUCCESS!");
        }

        public override object CheckUserBalance(IRepository repository, string userId)
        {
            throw new NotImplementedException();
        }

        public override bool RegisterTransaction(IRepository repository, string sender, string recipient, object incentiveValue, out string transactionId)
        {
            throw new NotImplementedException();
        }

        public override List<Transaction> GetUserTransactions(IRepository repository, string userId)
        {
            throw new NotImplementedException();
        }

        public bool AddNodeToChain(string address)
        {
            List<string> listofAddresses = new List<string>();

            try
            {
                listofAddresses.Add(address);
                var permConnect = this.MultichainClient.GrantAsync(listofAddresses, BlockchainPermissions.Connect);
                permConnect.Result.AssertOk();
                var permSend = this.MultichainClient.GrantAsync(listofAddresses, BlockchainPermissions.Send);
                permSend.Result.AssertOk();
                var permReceive = this.MultichainClient.GrantAsync(listofAddresses, BlockchainPermissions.Receive);
                permReceive.Result.AssertOk();
                var permMine = this.MultichainClient.GrantAsync(listofAddresses, BlockchainPermissions.Mine);
                permMine.Result.AssertOk();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        #region PRIVATE_METHODS

        private void LoadMultichainParameters(out int port, out string username, out string password, out string chainName)
        {
            XMLParser xmlParser;
            string errorMessage = null;
            port = -1;
            username = null;
            password = null;
            chainName = null;

            try
            {
                //take care of xml path, because it's realtive to the process which is running the app
                xmlParser = new XMLParser(@"..\..\multichain-core\config-server\chain_parameters.xml", @"..\..\multichain-core\config-server\chain_parameters.xsd", "This4ThatNS");
                if (!xmlParser.LoadXMLConfiguration(ref errorMessage))
                {
                    throw new Exception(errorMessage);
                }
                int.TryParse(xmlParser.XmlDoc.GetElementsByTagName(Global.PORT_TAG)[0].InnerText, out port);
                username = xmlParser.XmlDoc.GetElementsByTagName(Global.USERNAME_TAG)[0].InnerText;
                password = xmlParser.XmlDoc.GetElementsByTagName(Global.PASSWORD_TAG)[0].InnerText;
                chainName = xmlParser.XmlDoc.GetElementsByTagName(Global.CHAINNAME_TAG)[0].InnerText;

                if (port != -1 && username != null && password != null && chainName != null)
                    return;

                throw new Exception("Invalid chain_parameters.xml file!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateBlockChain(out bool alreadyExists)
        {
            DirectoryInfo dirInfo;

            //if the folder exists, so the we assume the blockchain is initialized
            if (Directory.Exists(@"..\..\multichain-core\config-server\"))
            {
                alreadyExists = true;
                return;
            }
            //else, the blockchain was not initialized, going to create the directory
            dirInfo = Directory.CreateDirectory(@"..\..\multichain-core\config-server\");

            if (dirInfo.Exists)
            {
                Process.Start(@"..\..\multichain-core\multichain-util.exe", @"create This4ThatChain -datadir=..\..\multichain-core\config-server");
                Console.Write("[INFO] - Chain CREATED");
                alreadyExists = false;
            }
            else
                throw new Exception("Cannot create config-server folder to initialize ht blockchain");
        }

        private void StartBlockChain()
        {
            Process.Start(@"..\..\multichain-core\multichaind.exe", @"This4ThatChain -datadir=..\..\multichain-core\config-server");
            //give time to client, to connect to the node
            Thread.Sleep(2000);
            Console.WriteLine("[INFO] - Chain INITIALIZED");
        }

        #endregion
    }
}
