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
        private string managerAddress = null;
        protected ILog log;

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
        public DescentralizedIncentiveScheme(IRepository repository, Incentive incentive) : base(repository, incentive)
        {
            bool chainAlreadyExist = false;
            int chainPort;
            string username;
            string password;
            string chainName;

            this.log = LogManager.GetLogger("MultichainLOG");
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

        public override bool CheckUserBalance(string sender, int incentiveQty, string incentiveName)
        {
            try
            {
                var respMultibal = this.MultichainClient.GetMultiBalancesAsync(sender, incentiveName);
                respMultibal.Result.AssertOk();

                foreach (AssetBalanceResponse asset in respMultibal.Result.Result.Assets)
                {
                    if (asset.Name.Equals(incentiveName))
                    {
                        if (!this.Incentive.CheckSufficientCredits(asset.Qty, incentiveQty))
                        {
                            log.DebugFormat("Insufficient Funds. User Balance: [{0}] for Incentive:  [{1}]", asset.Qty, incentiveQty);
                            return false;
                        }
                        return true;
                    }
                }
                throw new Exception("Incentive not Found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override bool RegisterTransaction(string sender, string recipient, object incentiveValue, out string transactionId)
        {
            transactionId = null;
            decimal assetQty;
            try
            {
                decimal.TryParse(incentiveValue.ToString(), out assetQty);

                //FIXME: do not send all incentives
                foreach (string incentive in this.Incentive.Incentives)
                {
                    log.DebugFormat("Going to transfer [{0}] from incentive [{1}]", assetQty, incentive);
                    var response = this.MultichainClient.SendAssetFromAsync(sender, recipient,
                                                                            incentive, assetQty);
                    response.Result.AssertOk();
                    transactionId = response.Result.Result;
                    log.DebugFormat("Transaction Id: [{0}]", transactionId);
                }
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

        public override bool RegisterUser(out string transactionId, out string userAddress, ref string errorMessage)
        {
            transactionId = null;
            int incentiveQty = -1;
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

                //give user permissions
                GiveUserPermissions(userAddress);

                incentiveQty = Incentive.InitIncentiveQuantity();
                //create value for the asset (creates the necessary incentive's quantity to give to the user)
                if (!CreateAssetValue(incentiveQty))
                {
                    errorMessage = "Cannot register transaction on multichain.";
                    return false;
                }

                if (!RegisterTransaction(this.ManagerAddress, userAddress, incentiveQty, out transactionId))
                {
                    errorMessage = String.Format("Cannot Register transaction! From UserManager: [{0}] to User: [{1}]", managerAddress, userAddress);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public bool CreateAssetValue(int incentiveQty)
        {
            List<string> incentives;

            try
            {
                incentives = Incentive.GetIncentivesName();

                foreach (string incentiveName in incentives)
                {
                    log.DebugFormat("Going to issue more [{0}] from asset [{1}]", incentiveQty, incentiveName);
                    var response = this.MultichainClient.IssueMoreAsync(ManagerAddress, incentiveName, incentiveQty);
                    response.Result.AssertOk();
                }
                return true;
                

            }
            catch (Exception)
            {
                return false;
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
            Process.Start(@"..\..\multichain-core\multichaind.exe", @"This4ThatChain -datadir=..\..\multichain-core\config-server");
            //give time to client, to connect to the node
            Thread.Sleep(5000);
            Console.WriteLine("[INFO] - Chain INITIALIZED");
            log.Debug("Blockchain initialized with SUCESS!");
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
                response.Result.AssertOk();
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
                    var respAssetExist = this.MultichainClient.ListAssetsAsync(incentiveName);
                    //if the asset already exists, do not create
                    if (respAssetExist.Result.Result.Count == 1)
                    {
                        log.DebugFormat("Asset: [{0}] already created in blockchain", incentiveName);
                        continue;
                    }
                    //Create asset
                    var resCreateAsset = this.MultichainClient.IssueAsync(managerAddress, incentiveName, true, 0, 1);
                    resCreateAsset.Result.AssertOk();
                    log.DebugFormat("Asset: [{0}] Created", incentiveName);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool PayTask(string sender, out string transactionId)
        {
            int assetQty;
            transactionId = null;
            string incentiveName = null;
            
            try
            {
                incentiveName = this.Incentive.CreateTaskIncentiveName();
                assetQty = this.Incentive.CreateTaskIncentiveQty();

                //check if the user can make the transaction
                if (!CheckUserBalance(sender, assetQty, incentiveName))
                    return false;

                //register the transaction                
                log.DebugFormat("User: [{0}] going to pay [{1}]", sender, assetQty);
                RegisterTransaction(sender, this.ManagerAddress, assetQty, out transactionId);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool RewardUser(string receiver, out object reward, out string transactionId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
