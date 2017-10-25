using MultiChainLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using This4That_library.Models.Incentives;
using System.Collections.Generic;
using This4That_library.Models.Domain;
using log4net;

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

        
        public DescentralizedIncentiveScheme(IRepository repository, Incentive incentive) : base(repository, null, incentive)
        {
            bool chainAlreadyExist = false;
            int chainPort;
            string username;
            string password;
            string chainName;

            //create the blockchain if it does not exist
            //CreateBlockChain(out chainAlreadyExist);
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

            //FIXME: remove, just for testing
            //UserTesting();
            
        }

        public void UserTesting()
        {
            IncentiveAssigned incentive = new IncentiveAssigned("BRONZE_BADGE", 2000);
            string transactionId;
            string userId = "1FwQkWSxzFMz8utyhZzwF2K5QqtZ59uQuGbAb8";
            bool hasfunds;

            //FIXME: remove
            this.Repository.RegisterUser(userId, this.Incentive);
            IssueMoreIncentives(incentive);
            RegisterTransaction(this.ManagerAddress, userId, incentive, out transactionId, out hasfunds);
        }


        #region MULTICHAIN_OPERATIONS

        public override bool CheckUserBalance(string sender, IncentiveAssigned incentiveAssigned)
        {
            int balance;
            try
            {
                var respMultibal = this.MultichainClient.GetMultiBalancesAsync(sender, incentiveAssigned.IncentiveName);
                //respMultibal.Result.AssertOk();

                foreach (AssetBalanceResponse asset in respMultibal.Result.Result.Assets)
                {
                    if (asset.Name.Equals(incentiveAssigned.IncentiveName))
                    {
                        balance = (int)asset.Qty;

                        if (!this.Incentive.CheckSufficientCredits(balance, incentiveAssigned.IncentiveQty))
                        {
                            log.DebugFormat("Insufficient Funds. User Balance: [{0}] for Incentive:  [{1}]", asset.Qty, incentiveAssigned.IncentiveQty);
                            return false;
                        }
                        return true;
                    }
                }
                throw new Exception("Incentive not Found");
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.Message);
                return false;
            }
        }

        public override bool RegisterTransaction(string sender, string recipient, IncentiveAssigned incentiveAssigned, out string transactionId, out bool hasFunds)
        {
            transactionId = null;

            hasFunds = false;

            try
            {
                //check if the user can make the transaction
                if (!CheckUserBalance(sender, incentiveAssigned))
                {
                    
                    //check if the Manager has  the necessary incentives to distribute among the users
                    if (sender.Equals(this.ManagerAddress))
                    {
                        log.Debug("Manager does not have the necessary incentive quantity. Going to issue more incentives!");
                        IssueMoreIncentives(incentiveAssigned);
                    }
                    else
                    {
                        hasFunds = false;
                        return true;
                    }
                }
                hasFunds = true;
                log.DebugFormat("Going to transfer [{0}] from incentive [{1}]", incentiveAssigned.IncentiveQty, incentiveAssigned.IncentiveName);
                var response = this.MultichainClient.SendAssetFromAsync(sender, recipient, incentiveAssigned.IncentiveName, incentiveAssigned.IncentiveQty);
                response.Result.AssertOk();
                transactionId = response.Result.Result;
                log.DebugFormat("Transaction Id: [{0}]", transactionId);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.Message);
                return false;
            }
        }

        private bool IssueMoreIncentives(IncentiveAssigned incentiveAsseigned)
        {

            try
            {
                log.DebugFormat("Going to issue more [{0}] from asset [{1}]", incentiveAsseigned.IncentiveQty, incentiveAsseigned.IncentiveName);
                var response = this.MultichainClient.IssueMoreAsync(ManagerAddress, incentiveAsseigned.IncentiveName, incentiveAsseigned.IncentiveQty);
                response.Result.AssertOk();
                return true;                

            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Create the incentives in the blockchain
        /// </summary>
        private void IssueIncentives()
        {
            List<string> incentives;
            List<string> assets = new List<string>();
            try
            {
                incentives = Incentive.GetIncentivesName();

                var respAssetExist = this.MultichainClient.ListAssetsAsync();
                respAssetExist.Result.AssertOk();
                
                foreach (AssetResponse asset in respAssetExist.Result.Result)
                {
                    assets.Add(asset.Name);
                }

                foreach (string incentiveName in incentives)
                {
                    
                    //if the asset already exists, do not create
                    if (assets.Contains(incentiveName))
                    {
                        log.DebugFormat("Asset: [{0}] already created in blockchain", incentiveName);
                        continue;
                    }
                    //Create asset
                    var resCreateAsset = this.MultichainClient.IssueAsync(this.ManagerAddress, incentiveName, true, 0, 1);
                    resCreateAsset.Result.AssertOk();
                    log.DebugFormat("Asset: [{0}] Created", incentiveName);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.Message);
            }
        }

        private void GiveUserPermissions(string multichainId)
        {
            List<string> addressesPermissions = new List<string>();

            log.DebugFormat("Permissions for UserId: [{0}]", multichainId);
            addressesPermissions.Add(multichainId);
            //permission to connect to the blockchain
            var resPermissionConnect = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Connect);
            resPermissionConnect.Result.AssertOk();
            log.Debug("Permission to Connect.");

            //permission to perform trasactions
            var resPermissionSend = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Send);
            resPermissionSend.Result.AssertOk();
            log.Debug("Permission to Send.");

            //permission to receive transactions
            var resPermissionsReceive = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Receive);
            resPermissionsReceive.Result.AssertOk();
            log.Debug("Permission to Receive.");

            //permission to mine transactions
            var resPermissionsMine = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Mine);
            resPermissionsMine.Result.AssertOk();
            log.Debug("Permission to Mine.");
        }

        private void GiveAdminPermissions(string multichainId)
        {
            List<string> addressesPermissions = new List<string>();

            log.DebugFormat("Permissions for Administrator UserId: [{0}]", multichainId);
            addressesPermissions.Add(multichainId);
            //permission to connect to the blockchain
            var resPermissionConnect = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Connect);
            resPermissionConnect.Result.AssertOk();
            log.Debug("Permission to Connect.");

            //permission to perform trasactions
            var resPermissionSend = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Send);
            resPermissionSend.Result.AssertOk();
            log.Debug("Permission to Send.");

            //permission to receive transactions
            var resPermissionsReceive = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Receive);
            resPermissionsReceive.Result.AssertOk();
            log.Debug("Permission to Receive.");

            //permission to issue assets to the blockchain
            var resPermissionsIssue = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Issue);
            resPermissionsIssue.Result.AssertOk();
            log.Debug("Permission to Issue.");

            //permission to admin the blockchain
            var resPermissionsAdmin = this.MultichainClient.GrantAsync(addressesPermissions, BlockchainPermissions.Admin);
            resPermissionsAdmin.Result.AssertOk();
            log.Debug("Permission to Admin.");
        }

        private void GetAddressForManager()
        {
            string managerAddress;
            List<string> addressesPermissions = new List<string>();
            try
            {
                var response = this.MultichainClient.GetNewAddressAsync();
               // response.Result.AssertOk();
                managerAddress = response.Result.Result;
                log.DebugFormat("Manager has the following address: [{0}]", managerAddress);
                addressesPermissions.Add(managerAddress);

                //give multichain admin permissions
                GiveAdminPermissions(managerAddress);

                this.ManagerAddress = managerAddress;
                this.Repository.RegisterUser(this.ManagerAddress, this.Incentive);
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.Message);
            }
        }

        #endregion

        #region MULTICHAIN_SETUP

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
                xmlParser = new XMLParser(@"..\..\multichain-core\config-client\chain_parameters.xml", @"..\..\multichain-core\config-client\chain_parameters.xsd", "This4ThatNS");
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
                log.Debug("Multichain installation detected!");
                return;
            }
            //else, the blockchain was not initialized, going to create the directory
            dirInfo = Directory.CreateDirectory(@"..\..\multichain-core\config-server\");
            log.Debug("Multichain installation NOT detected!");
            if (dirInfo.Exists)
            {
                Process.Start(@"..\..\multichain-core\multichain-util.exe", @"create This4ThatChain -datadir=..\..\multichain-core\config-server");
                Console.Write("[INFO] - Chain CREATED");
                alreadyExists = false;
            }
            else
                throw new Exception("Cannot create config-server folder to initialize blockchain");
        }

        private void StartBlockChain()
        {
            Process.Start(@"..\..\multichain-core\multichaind.exe", @"This4ThatChain -datadir=..\..\multichain-core\config-client");
            //give time to client, to connect to the node
            Console.WriteLine("[INFO] - Connecting to Multichain...");
            Thread.Sleep(10000);
            Console.WriteLine("[INFO] - Chain INITIALIZED");
            log.Debug("Blockchain initialized with SUCESS!");
        }

        #endregion


        public override bool RegisterUser(out string transactionId, out string userAddress, ref string errorMessage)
        {
            transactionId = null;
            IncentiveAssigned incentiveAssigned;
            bool hasfunds;
            try
            {
                log.DebugFormat("Goin to register a new user.");
                //get multichain address. this address will be used to deposit and raise rewards.
                var response = this.MultichainClient.GetNewAddressAsync();
                response.Result.AssertOk();
                userAddress = response.Result.Result;
                log.DebugFormat("User Address: [{0}]", userAddress);

                //register user on repository
                this.Repository.RegisterUser(userAddress, this.Incentive);

                //add useraddress to the user nodes list
                this.Repository.AddUserMultichainNode(userAddress, userAddress);

                //give user permissions
                GiveUserPermissions(userAddress);
                incentiveAssigned = Incentive.RegisterUserIncentive();

                if (!RegisterTransaction(this.ManagerAddress, userAddress, incentiveAssigned, out transactionId, out hasfunds))
                {
                    errorMessage = String.Format("Cannot Register transaction! From UserManager: [{0}] to User: [{1}]", this.ManagerAddress, userAddress);
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool PayTask(string sender, out string transactionId, out bool hasFunds)
        {
            Watch.Start();
            transactionId = null;
            hasFunds = false;
            IncentiveAssigned incentiveAssigned;

            try
            {
                incentiveAssigned = this.Incentive.CreateTaskIncentive();
                                                
                //register the transaction                
                log.DebugFormat("User: [{0}] going to pay [{1}] to create a Task", sender, incentiveAssigned.IncentiveQty);
                RegisterTransaction(sender, this.ManagerAddress, incentiveAssigned, out transactionId, out hasFunds);
                Watch.Stop();
                Log.DebugFormat("Execution Time on IncentiveEngineNode Decentralized: [{0}] milliseconds to Pay Task", Watch.ElapsedMilliseconds);
                Watch.Reset();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }

        public override bool RewardUser(string receiver, IncentiveAssigned incentiveAssigned, out object reward, out string transactionId)
        {
            Watch.Start();
            List<string> userNodes;
            reward = null;
            transactionId = null;
            bool hasFunds;

            try
            {
                userNodes = this.Repository.GetUserMultichainNodes(receiver);
                
                if (userNodes != null)
                {
                    //increase the incentive based on the number of nodes contributing to the blockchain
                    if (userNodes.Count != 0)
                        incentiveAssigned.IncentiveQty *= userNodes.Count;
                }
                else
                {
                    return false;
                }
                //transfer the incentive to the user
                if (!RegisterTransaction(ManagerAddress, receiver, incentiveAssigned, out transactionId, out hasFunds))
                {
                    return false;
                }
                reward = new Dictionary<string, string>() { { "quantity", incentiveAssigned.IncentiveQty.ToString()},
                                                            { "incentive", incentiveAssigned.IncentiveName } };
                Watch.Stop();
                Log.DebugFormat("Execution Time on IncentiveEngineNode Decentralized: [{0}] milliseconds to Reward User", Watch.ElapsedMilliseconds);
                Watch.Reset();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override List<Transaction> GetUserTransactions(string mutichainAddress)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool AddNodeToChain(string address)
        {
            try
            {
                GiveUserPermissions(address);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
