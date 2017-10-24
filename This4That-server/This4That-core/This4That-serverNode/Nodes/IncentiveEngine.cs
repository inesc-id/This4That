using log4net;
using System;
using This4That_library;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;
using System.Collections.Generic;
using This4That_library.Models.Domain;
using System.Threading;
using This4That_library.Integration;
using System.Linq;

namespace This4That_ServerNode.Nodes
{
    public class IncentiveEngine : Node, IIncentiveEngine
    {
        private IRepository repositoryRemote = null;
        private ITransactionNode txNodeRemote= null;
        private IncentiveSchemeBase incentiveScheme;

        public IRepository RepositoryRemote
        {
            get
            {
                return repositoryRemote;
            }

            set
            {
                repositoryRemote = value;
            }
        }

        public IncentiveSchemeBase IncentiveScheme
        {
            get
            {
                return incentiveScheme;
            }

            set
            {
                incentiveScheme = value;
            }
        }

        public ITransactionNode TxNodeRemote
        {
            get
            {
                return txNodeRemote;
            }

            set
            {
                txNodeRemote = value;
            }
        }

        public IncentiveEngine(string hostName, int port, string name, string repoURL, string txNodeURL) : base(hostName, port, name, "IncentiveEngineLOG")
        {
            List<string> incentives = new List<string>();
            Thread checkReportsTh = new Thread(AnalyzeReports);
            try
            {
                //get the repository ref
                ConnectToRepository(repoURL);
                //get remote ref to TxNode, this ref will be needed by the CentralizedSchema
                ConnectToTransactionNode(txNodeURL);
                incentives.Add(Gamification.GOLD_BADGE);
                incentives.Add(Gamification.SILVER_BADGE);
                incentives.Add(Gamification.BRONZE_BADGE);
                incentives.Add(Gamification.POINTS);

                this.IncentiveScheme = new DescentralizedIncentiveScheme(this.RepositoryRemote, new Gamification(incentives));
                //this.IncentiveScheme = new CentralizedIncentiveScheme(this.RepositoryRemote, this.TxNodeRemote, new Gamification(incentives));

                checkReportsTh.Start();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(ex.InnerException.Message + " Failed to connect to the Multichain node.");
                Console.WriteLine("[ERROR] - FAILED TO CONNECT THE MULTICHAIN NODE!");
            }            
        }

        /// <summary>
        /// Get Remote reference to Server Manager.
        /// </summary>
        /// <param name="serverMgrURL"></param>
        /// <returns></returns>
        public override bool ConnectServerManager()
        {
            try
            {
                this.RemoteServerMgr = (IServerManager)Activator.GetObject(typeof(IServerManager), Global.SERVER_MANAGER_URL);
                if (!this.RemoteServerMgr.RegisterIncentiveEngineNode($"tcp://{this.HostName}:{this.Port}/{Global.INCENTIVE_ENGINE_NAME}"))
                {
                    Log.Error("Cannot connect to Server Manager!");
                    return false;
                }
                Log.DebugFormat("ServerManager: [{0}]", Global.SERVER_MANAGER_URL);
                Console.WriteLine("[INFO] - CONNECTED to ServerManager");
                Console.WriteLine("----------------------------" + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.ErrorFormat("Cannot connect Incentive Engine to ServerManager: [{0}", Global.SERVER_MANAGER_URL);
                return false;
            }
        }

        private bool ConnectToRepository(string repositoryURL)
        {
            try
            {
                this.RepositoryRemote = (IRepository)Activator.GetObject(typeof(IRepository), Global.REPOSITORY_URL);
                Log.DebugFormat("[INFO] Incentive Engine connected to Repository.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        private bool ConnectToTransactionNode(string txNodeURL)
        {
            try
            {
                if (txNodeURL == null)
                {
                    Log.Debug("Decentralized Mode, there is no need to have a Transaction Node!");
                    return true;
                }
                this.TxNodeRemote = (ITransactionNode)Activator.GetObject(typeof(ITransactionNode), txNodeURL);
                Log.DebugFormat("[INFO] Incentive Engine connected to TransactionNode.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public void AnalyzeReports()
        {

            while (true)
            {
                Thread.Sleep(5000);
                foreach (CSTask task in RepositoryRemote.GetCSTasksToValidate())
                {
                    ProcessTaskReports(task);
                }
            }
        }

        public void ProcessTaskReports(CSTask task)
        {
            Dictionary<string, int> answersMap = new Dictionary<string, int>();
            List<double> listOfAnswersIndex = new List<double>();
            Dictionary<int, int> answersIncentive = null;
            List<InteractiveReport> listResports = new List<InteractiveReport>();
            InteractiveReport intReport = null;
            IncentiveAssigned incentive = null;
            int index = 1;
            object taskReward = null;
            string txId = null;

            //creates association between answer Index and answerId
            //this will allow to sort the answers to calculate the z-score modified
            foreach (TaskAnswer answer in task.InteractiveTask.Answers)
            {
                answersMap.Add(answer.AnswerId, index);
                index++;
            }

            foreach (string reportID in task.ReportsID.Values.ToList())
            {
                //get report from repository
                intReport = RepositoryRemote.GetInteractiveReportsByID(reportID);
                //add the first report result
                if (answersMap.ContainsKey(intReport.Result.AnswerId))
                    listOfAnswersIndex.Add(answersMap[intReport.Result.AnswerId]);
                listResports.Add(intReport);
            }
            if (listOfAnswersIndex.Count == 0)
            {
                Log.Debug("No Answers to Process");
                return;
            }
            //get for each answer its reward value
            answersIncentive = DataQualityProcess(listOfAnswersIndex);
            incentive = this.IncentiveScheme.Incentive.CompleteTaskIncentive();
            foreach (InteractiveReport report in listResports)
            {
                index = answersMap[report.Result.AnswerId];
                incentive.IncentiveQty = answersIncentive[index];

                this.IncentiveScheme.RewardUser(report.UserID, incentive, out taskReward, out txId);
                this.RepositoryRemote.SaveReportReward(task.TaskID, report.ReportID, (Dictionary<string, string>)taskReward, txId);
            }



        }

        private Dictionary<int, int> DataQualityProcess(List<double> answersSortByIndex)
        {
            List<double> medDevValues = new List<double>();
            List<double> modZscoreVals = new List<double>();
            List<double> answersWeight = new List<double>();
            Dictionary<int, int> rewardByAnswer = new Dictionary<int, int>();
            IncentiveAssigned incentive = null;
            double medianVal = 0;
            double finalMedianDev = 0;
            double auxZscoreVal = 0;
            double maxAbsValue = 0;
            double rewardvalue = 0;
            

            //calculate the median value
            medianVal = Library.GetMedian(answersSortByIndex);
            medDevValues.Add(Math.Abs(1 - medianVal));
            medDevValues.Add(Math.Abs(2 - medianVal));
            medDevValues.Add(Math.Abs(3 - medianVal));
            medDevValues.Add(Math.Abs(4 - medianVal));
            //calculate median deviation final
            finalMedianDev = Library.GetMedian(medDevValues);
            
            //answer1
            auxZscoreVal = 0.6745 * (1 - medianVal) / finalMedianDev;
            modZscoreVals.Add(auxZscoreVal);
            //answer2
            auxZscoreVal = 0.6745 * (2 - medianVal) / finalMedianDev;
            modZscoreVals.Add(auxZscoreVal);
            //answer3
            auxZscoreVal = 0.6745 * (3 - medianVal) / finalMedianDev;
            modZscoreVals.Add(auxZscoreVal);
            //answer4
            auxZscoreVal = 0.6745 * (4 - medianVal) / finalMedianDev;
            modZscoreVals.Add(auxZscoreVal);

            //getMaxvalue
            maxAbsValue = Math.Abs(modZscoreVals.Max());

            //get answer1 weight, reuse zscoreval variable
            auxZscoreVal = Math.Abs(modZscoreVals[0] / maxAbsValue);
            answersWeight.Add(auxZscoreVal);
            //get answer2 weight, reuse zscoreval variable
            auxZscoreVal = Math.Abs(modZscoreVals[1] / maxAbsValue);
            answersWeight.Add(auxZscoreVal);
            //get answer3 weight, reuse zscoreval variable
            auxZscoreVal = Math.Abs(modZscoreVals[2] / maxAbsValue);
            answersWeight.Add(auxZscoreVal);
            //get answer4 weight, reuse zscoreval variable
            auxZscoreVal = Math.Abs(modZscoreVals[3] / maxAbsValue);
            answersWeight.Add(auxZscoreVal);

            //get the incentive for completing tasks
            incentive = this.IncentiveScheme.Incentive.CompleteTaskIncentive();
            //reward for the 1 answer
            rewardvalue = incentive.IncentiveQty - (incentive.IncentiveQty / 2 * answersWeight[0]);
            rewardvalue = Math.Round(rewardvalue, 0);
            rewardByAnswer.Add(1, (int)rewardvalue);
            //reward for the 2 answer
            rewardvalue = incentive.IncentiveQty - (incentive.IncentiveQty / 2 * answersWeight[1]);
            rewardvalue = Math.Round(rewardvalue, 0);
            rewardByAnswer.Add(2, (int)rewardvalue);
            //reward for the 3 answer
            rewardvalue = incentive.IncentiveQty - (incentive.IncentiveQty / 2 * answersWeight[2]);
            rewardvalue = Math.Round(rewardvalue, 0);
            rewardByAnswer.Add(3, (int)rewardvalue);
            //reward for the 4 answer
            rewardvalue = incentive.IncentiveQty - (incentive.IncentiveQty / 2 * answersWeight[3]);
            rewardvalue = Math.Round(rewardvalue, 0);
            rewardByAnswer.Add(4, (int)rewardvalue);

            return rewardByAnswer;
        }

        #region REMOTE_INTERFACE

        public bool CalcTaskCost(CSTaskDTO taskSpec, string userID, out IncentiveAssigned incentiveAssigned)
        {
            incentiveAssigned = null;

            try
            {
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Calc Task Cost for User: " + userID);
                incentiveAssigned = IncentiveScheme.Incentive.CreateTaskIncentive();

                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Incentive Value: " + incentiveAssigned.IncentiveQty 
                                + " " + incentiveAssigned.IncentiveName);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool PayTask(string userId, out string transactionId, out bool hasfunds)
        {
            transactionId = null;
            hasfunds = false;
                        
            try
            {
                if (!IncentiveScheme.PayTask(userId, out transactionId, out hasfunds))
                {
                    return false;                   
                }
                if (hasfunds == false)
                    Console.WriteLine("[INFO - INCENTIVE ENGINE] - Insufficient funds!");
                else
                    Console.WriteLine("[INFO - INCENTIVE ENGINE] - Payment Registered with Success!");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool RewardUser(string userId, string taskId, out string transactionId, out object response)
        {
            Dictionary<string, string> taskReward = null;
            transactionId = null;
            response = null;
            string reportId;
            InteractiveReport userReport;
            Dictionary<string, object>  auxResponse = new Dictionary<string, object>();
            try
            {
                reportId = this.RepositoryRemote.GetUserReportByTaskId(taskId, userId);

                if (reportId == null)
                {
                    taskReward = new Dictionary<string, string>() { { "status", "pending for validation" } };
                }
                else
                {
                    userReport = this.RepositoryRemote.GetInteractiveReportsByID(reportId);
                    taskReward = new Dictionary<string, string>() { { "incentive", userReport.ReportReward.IncentiveName},
                                                                    { "quantity", userReport.ReportReward.IncentiveValue } };

                    auxResponse.Add("txId", userReport.ReportReward.TransactionId);
                    transactionId = userReport.ReportReward.TransactionId;
                }
                auxResponse.Add("reward", taskReward);
                Console.WriteLine("[INFO - INCENTIVE ENGINE] - Payment Registered with Success!");
                response = auxResponse;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }

        }

        public bool RegisterUser(out string  userId, out string userAddress)
        {
            string transactionId;
            string errorMessage = null;
            userId = null;
            userAddress = null;
            try
            {                
                if (!IncentiveScheme.RegisterUser(out transactionId, out userAddress, ref errorMessage))
                {
                    Log.Error(errorMessage);
                    Console.WriteLine("[ERROR - INCENTIVE ENGINE] - Cannot save user creation transaction!");
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
            

        }

        public bool GetUserTransactions(string userId, out List<Transaction> transactions)
        {
            transactions = null;

            try
            {
                //get user transaction based on the actual incentive scheme
                transactions = IncentiveScheme.GetUserTransactions(userId);
                if (transactions == null)
                {
                    Log.Error("Transactions List IS NULL");
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


        #region DESCENTRALIZED_SCHEME_METHODS

        public bool AddNodeToChain(string userId, string multichainAddress, ref string message)
        {
            message = null;

            try
            {
                if (IncentiveScheme.GetType() == typeof(DescentralizedIncentiveScheme))
                {
                    //add node
                    if (!((DescentralizedIncentiveScheme)IncentiveScheme).AddNodeToChain(multichainAddress))
                    {
                        Log.ErrorFormat("Invalid Chain address: [{0}]", multichainAddress);
                        message = "Invalid address!";
                        return false;
                    }
                    //add association to user wallet
                    if (!this.RepositoryRemote.AddUserMultichainNode(userId, multichainAddress))
                    {
                        Log.Error("Cannot associate the multichain node to the user's wallet.");
                        message = "Cannot associate block-chain node to user!";
                        return false;
                    }
                    return true;
                }
                message = "Your scheme must be changed to the descentralized version!";
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        #endregion

        #endregion
    }
}