using MultiChainLib;
using System;
using This4That_library.Models.Incentives;


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
            //FIXME: tem de iniciar a blockchain se nao existir
            //se a existir, corre o daemon para instanciar a blockchain
            //ler parameters do ficheiro /config-server/
            int port;
            string username;
            string password;
            string chainName;

            //load the multichain parameters
            LoadMultichainParameters(out port, out username, out password, out chainName);

            //instanciates a multi-chain client
            this.MultichainClient = new MultiChainClient("localhost", port, false, username, password, chainName);
            var response = this.MultichainClient.GetInfoAsync();

            Console.WriteLine("[INFO] - Connected to Blockchain : [" + response.Result.Result.ChainName + "] with SUCCESS!");
        }

        public override object CheckUserBalance(IRepository repository, string userId)
        {
            throw new NotImplementedException();
        }

        public override bool RegisterPayment(IRepository repository, string sender, string recipient, object incentiveValue, out string transactionId)
        {
            throw new NotImplementedException();
        }

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
    }
}
