﻿using MultiChainLib;
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
        private string managerAddress = null;

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

        public string ManagerAddress
        {
            get
            {
                return managerAddress;
            }

            set
            {
                managerAddress = value;
            }
        }

        public DescentralizedIncentiveScheme(IRepository repository, Incentive incentive) : base(repository, incentive)
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
            //generates and assoaciates an address for the system Manager
            GetAddressForManager();
            //create the incentives to be distributed in the blockchain
            IssueIncentives();
        }

        public override object CheckUserBalance(string userId)
        {
            throw new NotImplementedException();
        }

        public override bool RegisterTransaction(string sender, string recipient, object incentiveValue, out string transactionId)
        {
            throw new NotImplementedException();
        }
        /*
        public bool IssueIncentiveToUser(string sender, string recipient, object incentiveValue, out string transactionId)
        {
            try
            {
                var response = this.MultichainClient.IssAsync(recipient, )

            }
            catch (Exception ex)
            {

                return false;
            }
        }*/

        public override List<Transaction> GetUserTransactions(string userId)
        {
            throw new NotImplementedException();
        }

        public override bool SaveCreateUserTransaction(string userId, object initValue, out string transactionId, out string userAddress, ref string errorMessage)
        {
            transactionId = null;
            try
            {
                //get multichain address. this address will be used to deposit and raise rewards.
                var response = this.MultichainClient.GetNewAddressAsync();
                response.Result.AssertOk();
                userAddress = response.Result.Result;
                //associate the address to user
                if (!Repository.AddMultiChainAddressToUser(userId, userAddress))
                {
                    errorMessage = "Cannot add Multichain address to the user's wallet.";
                    transactionId = null;
                    return false;
                }
                /*
                //register the transaction
                if (!IssueIncentiveToUser("Platform", userAddress, initValue, out transactionId))
                {
                    errorMessage = "Cannot register transaction on multichain.";
                    return false;
                }*/
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
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

        private void GetAddressForManager()
        {
            string managerAddress;
            List<string> addressesPermissions = new List<string>();
            try
            {
                var response = this.MultichainClient.GetNewAddressAsync();
                response.Result.AssertOk();
                managerAddress = response.Result.Result;
                addressesPermissions.Add(managerAddress);
                //permission to connect to the blockchain
                var resPermissionConnect = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Connect);
                resPermissionConnect.Result.AssertOk();
                //permission to perform trasactions
                var resPermissionSend = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Send);
                resPermissionSend.Result.AssertOk();
                //permission to receive transactions
                var resPermissionsReceive = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Receive);
                resPermissionsReceive.Result.AssertOk();
                //permission to issue assets to the blockchain
                var resPermissionsIssue = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Issue);
                resPermissionsIssue.Result.AssertOk();
                //permission to admin the blockchain
                var resPermissionsAdmin = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Admin);
                resPermissionsAdmin.Result.AssertOk();

                Repository.AddMultiChainAddressToUser("Platform", managerAddress);
                this.ManagerAddress = managerAddress;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Create the incentives in the blockchain
        /// </summary>
        private void IssueIncentives()
        {
            List<string> incentives;

            try
            {
                incentives = Incentive.GetIncentivesName();
                
                foreach (string incentiveName in incentives)
                {
                    var response = this.MultichainClient.IssueAsync(managerAddress, incentiveName, true, 0, 1);
                    response.Result.AssertOk();
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
