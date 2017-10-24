﻿using System;
using System.Collections.Generic;
using System.Web;
using This4That_library;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.CalcTaskCostDTO;
using This4That_library.Models.Integration.ExecuteTaskDTO;
using This4That_library.Models.Integration.GetTasksByTopicDTO;
using This4That_library.Models.Integration.GetUserTopicDTO;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.Integration.TaskPayCreateDTO;
using This4That_platform.Models.Integration;
using This4That_library.Models.Domain;
using This4That_library.Models.Integration.GetMultichainAddress;
using This4That_library.Models.Incentives;
using System.Diagnostics;

namespace This4That_platform.Handlers
{
    public class APIRequestHandler
    {
        private HttpRequest request;
        private ServerManager serverMgr;
        private Stopwatch watch;

        public Stopwatch Watch
        {
            get
            {
                return watch;
            }

            set
            {
                watch = value;
            }
        }

        public APIRequestHandler(HttpRequest request, ServerManager serverMgr)
        {
            this.request = request;
            this.serverMgr = serverMgr;
            this.watch = new Stopwatch();
        }

        #region PUBLIC_METHODS
        /// <summary>
        /// Calculate the cost for a crowd-sensing task.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool CalcCostCSTask(out APIResponseDTO response)
        {
            IncentiveAssigned incentiveValue = null;
            CalcTaskCostRequestDTO csTask;
            CalcTaskCostResponseDTO calcCostDTO = new CalcTaskCostResponseDTO();

            response = new APIResponseDTO();

            try
            {
                if (!GetCSTaskToPay(out csTask))
                {
                    response.SetErrorResponse("Invalid Request please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Global.Log.DebugFormat("Client ID: {0}", csTask.UserID);
                       
                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(csTask.Task, csTask.UserID, out incentiveValue) 
                    || incentiveValue == null)
                {
                    response.SetErrorResponse("Cannot calculate the task cost. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                calcCostDTO.IncentiveToPay = incentiveValue;
                response.SetResponse(calcCostDTO, APIResponseDTO.RESULT_TYPE.SUCCESS);
                Global.Log.DebugFormat("User ID: [{0}] has to Pay [{1}]", csTask.UserID, incentiveValue.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot calculate the task cost. Please try again!" } }
                                    , APIResponseDTO.RESULT_TYPE.ERROR);
                return false;
            }
        }

        public bool GetTopics(out APIResponseDTO response)
        {
            response = new APIResponseDTO();

            try
            {
                response.SetResponse(serverMgr.RemoteTaskDistributor.GetTopics()
                                    , APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain topics from server. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return false;
            }
        }

        /// <summary>
        /// Pay and create a crowd-sensing task.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool PayCreateCSTask(out APIResponseDTO response)
        {
            Watch.Start();
            string txId = null;
            string taskId = null;
            bool hasFunds = false;
            TaskPayCreateRequestDTO payRequest = null;
            TaskPayCreateResponseDTO payResponse = new TaskPayCreateResponseDTO();
            response = new APIResponseDTO();
            try
            {
                //get the DTO containing the userIDand task meta-info
                if (!GetCSTaskToCreate(out payRequest))
                {
                    response.SetErrorResponse("Invalid Request please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Watch.Stop();
                //try to pay the task
                if (!PayCSTask(out txId, out hasFunds, payRequest))
                {
                    response.SetErrorResponse("Cannot perform the payment!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Watch.Start();
                //transaction not performed due to insufficient credits
                if (hasFunds == false)
                {
                    response.SetErrorResponse("Insufficient Credits!", APIResponseDTO.RESULT_TYPE.INSUFFICIENT_CREDITS);
                    return true;
                }
                Watch.Stop();
                if (!CreateCSTask(out taskId, payRequest))
                {
                    response.SetErrorResponse("Cannot create the crowd-sensing task, please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Watch.Start();
                payResponse.TaskID = taskId;
                payResponse.TransactionID = txId;
                response.SetResponse(payResponse, APIResponseDTO.RESULT_TYPE.SUCCESS);
                Watch.Stop();
                Global.Log.DebugFormat("Execution Time Create Task: [{0}] in Milliseconds", Watch.ElapsedMilliseconds);
                watch.Reset();
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.ErrorFormat("PayCreateCrowdSensingTask", ex.Message);
                return false;
            }

        }

        public bool GetTasksByTopicName(out APIResponseDTO response)
        {
            GetTopicRequestDTO topicRequestDTO;
            response = new APIResponseDTO();
            List<GetTasksDTO> listOfTasks;
            try
            {
                if (!GetUserTopicNameFromRequest(this.request, out topicRequestDTO))
                {
                    response.SetErrorResponse("Invalid Request!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                listOfTasks = serverMgr.RemoteTaskDistributor.GetTasksByTopicName(topicRequestDTO.TopicName);
                if (listOfTasks == null)
                {
                    response.SetErrorResponse("Invalid TopicName!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                response.SetResponse(new Dictionary<string, object>() {{ "tasks",  listOfTasks} }
                                    , APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain topic from server. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return false;
            }
        }

        internal bool GetTaskReward(out APIResponseDTO response)
        {
            Watch.Start();
            ExecuteTaskDTO checkRewardDTO = null;
            response = new APIResponseDTO();
            string transactionId = null;
            object taskReward = null;
            

            try
            {
                if (!GetTaskIdFromRequest(this.request, out checkRewardDTO))
                {
                    response.SetErrorResponse("Invalid Request!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                this.serverMgr.RemoteIncentiveEngine.RewardUser(checkRewardDTO.UserID, checkRewardDTO.TaskId, out transactionId, out taskReward);
                response.SetResponse(taskReward, APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool ReportResultsCSTask(out APIResponseDTO response, string taskType)
        {
            Watch.Start();
            TaskTypeEnum taskTypeEnum;
            ReportDTO reportReqDTO = null;
            response = new APIResponseDTO();

            try
            {
                //get the enum type
                if (!Enum.TryParse(taskType, out taskTypeEnum))
                {
                    Global.Log.ErrorFormat("Invalid Type of Task: [{0}]", taskType);
                    response.SetErrorResponse("Cannot Report Task!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                if (!GetReportFromRequest(this.request, taskTypeEnum, out reportReqDTO))
                {
                    response.SetErrorResponse("Invalid Request!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Watch.Stop();
                if (!serverMgr.RemoteReportAggregator.SaveReport(reportReqDTO))
                {
                    Global.Log.Error("Cannot generate Report from user results!");
                    return false;
                }
                Watch.Start();
                response.SetResponse(new Dictionary<string, object>() { { "status", "submited" } }
                                    , APIResponseDTO.RESULT_TYPE.SUCCESS);
                Global.Log.Debug("Report generated with SUCCESS!");
                Watch.Stop();
                Global.Log.DebugFormat("Execution Time Reward Task: [{0}] in Milliseconds", Watch.ElapsedMilliseconds);
                Watch.Reset();
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool RegisterUser(out APIResponseDTO response)
        {
            string userId = null;
            string userMultichainAddress = null;  
            response = new APIResponseDTO();
            try
            {
                if (!this.serverMgr.RemoteIncentiveEngine.RegisterUser(out userId, out userMultichainAddress))
                {
                    response.SetErrorResponse("Cannot Register User!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                response.SetResponse(new Dictionary<string, string>()
                                    { {"userId", userId}, {"multichainAddress", userMultichainAddress }}
                                    , APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool GetUserTasks(string userID, out APIResponseDTO response)
        {
            response = new APIResponseDTO();
            List<CSTaskDTO> myTasks;
            try
            {
                myTasks = this.serverMgr.RemoteTaskDistributor.GetUserTasks(userID);
                if (myTasks == null)
                {
                    Global.Log.ErrorFormat("Invalid User ID: [{0}]", userID);
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Invalid User ID!" } },
                                        APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Global.Log.DebugFormat("Number of Tasks: [{0}]", myTasks.Count);
                response.SetResponse(myTasks
                                    , APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        internal bool SubscribeTopic(out APIResponseDTO response)
        {
            response = new APIResponseDTO();
            GetTopicRequestDTO topicRequestDTO;
            string errorMessage = null;

            try
            {
                if (!GetUserTopicNameFromRequest(this.request, out topicRequestDTO))
                {
                    response.SetErrorResponse("Invalid Request!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                if (!this.serverMgr.RemoteTaskDistributor.SubscribeTopic(topicRequestDTO.UserID, topicRequestDTO.TopicName, ref errorMessage))
                {
                    Global.Log.ErrorFormat(errorMessage);
                    response.SetErrorResponse(errorMessage, APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Global.Log.DebugFormat("Topic: [{0}] SUBSCRIBED by User ID: [{1}]", topicRequestDTO.TopicName, topicRequestDTO.UserID);
                response.SetResponse("Subscribed", APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool ExecuteSubscribedTask(out APIResponseDTO response)
        {
            ExecuteTaskDTO execTaskDTO;
            response = new APIResponseDTO();

            try
            {
                if (!GetTaskIdFromRequest(this.request, out execTaskDTO))
                {
                    response.SetErrorResponse("Invalid Request!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                if (!this.serverMgr.RemoteTaskDistributor.ExecuteTask(execTaskDTO.UserID, execTaskDTO.TaskId))
                {
                    Global.Log.Error("Cannot execute Task!");
                    response.SetErrorResponse("Cannot Execute Task!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                response.SetResponse("Success", APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;

            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool GetUserTransactions(string userId, out APIResponseDTO response)
        {
            response = new APIResponseDTO();
            List<Transaction> userTransactions = null;
            try
            {
                if (!this.serverMgr.RemoteIncentiveEngine.GetUserTransactions(userId, out userTransactions))
                {
                    response.SetErrorResponse("Cannot obtain transactions!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                response.SetResponse(userTransactions, APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool GetSubscribedTasks(string userID, out APIResponseDTO response)
        {
            response = new APIResponseDTO();
            List<CSTaskDTO> subscribedTasks;
            try
            {
                subscribedTasks = this.serverMgr.RemoteTaskDistributor.GetUserSubscribedTasks(userID);
                if (subscribedTasks == null)
                {
                    Global.Log.ErrorFormat("Invalid User ID: [{0}]", userID);
                    response.SetErrorResponse("Invalid UserID!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Global.Log.DebugFormat("Number of Tasks: [{0}]", subscribedTasks.Count);
                response.SetResponse(subscribedTasks, APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        internal bool GetSubscribedTasksByTopicName(out APIResponseDTO response)
        {
            response = new APIResponseDTO();
            List<CSTaskDTO> subscribedTasks;
            GetTopicRequestDTO topicRequestDTO = null;
            string errorMessage = null;
            try
            {
                if (!GetUserTopicNameFromRequest(this.request, out topicRequestDTO))
                {
                    response.SetErrorResponse("Invalid Request!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                subscribedTasks = this.serverMgr.RemoteTaskDistributor
                                                .GetSubscribedTasksByTopicName(topicRequestDTO.UserID
                                                                               , topicRequestDTO.TopicName
                                                                               , ref errorMessage);
                if (subscribedTasks == null)
                {
                    Global.Log.ErrorFormat(errorMessage);
                    response.SetErrorResponse(errorMessage, APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                response.SetResponse(subscribedTasks, APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        public bool AddNodeToChain(out APIResponseDTO response)
        {
            response = new APIResponseDTO();
            APIRequestDTO requestDTO;
            GetMultichainAddressDTO multichainAddressDTO;
            string errorMessage = null;
            try
            {
                //get post parameters
                if (!Library.GetDTOFromRequest(request, out requestDTO, typeof(GetMultichainAddressDTO).FullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    response.SetErrorResponse("Invalid Request", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                multichainAddressDTO = (GetMultichainAddressDTO)requestDTO;
                
                if (!this.serverMgr.RemoteIncentiveEngine.AddNodeToChain(multichainAddressDTO.UserID
                                                                        , multichainAddressDTO.MultichainAddress
                                                                        , ref errorMessage))
                {
                    Global.Log.ErrorFormat("Cannot add the node [{0}] to the multichain", multichainAddressDTO.MultichainAddress);
                    response.SetErrorResponse(errorMessage, APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                Global.Log.DebugFormat("Node [{0}] added to multichain", multichainAddressDTO.MultichainAddress);
                response.SetResponse("Connected", APIResponseDTO.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Debug(ex.Message);
                return false;
            }
        }


        #endregion

        #region PRIVATE_METHODS
        private bool CreateCSTask(out string taskID, TaskPayCreateRequestDTO csTask)
        {
            taskID = null;
            try
            {
                if (!serverMgr.RemoteTaskCreator.CreateTask(csTask.Task, csTask.UserID, out taskID))
                {
                    Global.Log.Error("Cannot create crowd-sensing task!");
                    return false;
                }
                Global.Log.DebugFormat("Task with the ID: [{0}] created with Success!", taskID);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private bool PayCSTask(out string transactionId, out bool hasFunds, TaskPayCreateRequestDTO request)
        {
            transactionId = null;
            hasFunds = false;

            try
            {
                if (!serverMgr.RemoteIncentiveEngine.PayTask(request.UserID, out transactionId, out hasFunds))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private bool GetCSTaskToCreate(out TaskPayCreateRequestDTO csTask)
        {
            string errorMessage = null;
            csTask = null;
            string typeFullName = null;
            APIRequestDTO requestDTO;

            try
            {
                typeFullName = typeof(TaskPayCreateRequestDTO).FullName;
                if (!Library.GetDTOFromRequest(request, out requestDTO, typeFullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                csTask = (TaskPayCreateRequestDTO)requestDTO;
                if (csTask.Task.InteractiveTask == null && csTask.Task.SensingTask == null)
                {
                    Global.Log.Error("No Task Specified!");
                    return false;
                }
                Global.Log.DebugFormat("User ID: [{0}] Task: {1}", csTask.UserID, csTask.Task.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private bool GetCSTaskToPay(out CalcTaskCostRequestDTO csTask)
        {
            string errorMessage = null;
            csTask = null;
            string typeFullName = null;
            APIRequestDTO requestDTO;
            try
            {
                typeFullName = typeof(CalcTaskCostRequestDTO).FullName;
                if (!Library.GetDTOFromRequest(request, out requestDTO, typeFullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                csTask = (CalcTaskCostRequestDTO) requestDTO;
                //Global.Log.DebugFormat("User ID: [{0}] Task: {1}", csTask.UserID, csTask.Task.ToString());
                if (csTask.Task.InteractiveTask == null && csTask.Task.SensingTask == null)
                {
                    Global.Log.Error("No Task Specified!");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private bool GetUserTopicNameFromRequest(HttpRequest request, out GetTopicRequestDTO topicDTO)
        {
            string errorMessage = null;
            topicDTO = null;
            string typeFullName = null;
            APIRequestDTO requestDTO;

            try
            {
                typeFullName = typeof(GetTopicRequestDTO).FullName;
                if (!Library.GetDTOFromRequest(request, out requestDTO, typeFullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                topicDTO = (GetTopicRequestDTO)requestDTO;
                Global.Log.DebugFormat("User ID: [{0}] Topic: [{1}]", topicDTO.UserID, topicDTO.TopicName);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private bool GetReportFromRequest(HttpRequest request, TaskTypeEnum taskTypeEnum, out ReportDTO reportReqDTO)
        {
            string errorMessage = null;
            reportReqDTO = null;
            APIRequestDTO requestDTO;

            try
            {
                if (!Library.GetDTOFromRequest(request, out requestDTO, taskTypeEnum.ToString(), ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                reportReqDTO = (ReportDTO)requestDTO;
                Global.Log.DebugFormat("User ID: [{0}] reported Info for Task: [{1}]", reportReqDTO.UserID, reportReqDTO.TaskId);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private bool GetTaskIdFromRequest(HttpRequest request, out ExecuteTaskDTO execTaskDTO)
        {
            string errorMessage = null;
            execTaskDTO = null;
            APIRequestDTO requestDTO;

            try
            {
                if (!Library.GetDTOFromRequest(request, out requestDTO, typeof(ExecuteTaskDTO).FullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                execTaskDTO = (ExecuteTaskDTO)requestDTO;
                Global.Log.DebugFormat("User ID: [{0}] going to execute Task: [{1}]", execTaskDTO.UserID, execTaskDTO.TaskId);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }

        private bool GetUserFromRequest(HttpRequest request, out APIRequestDTO requestDTO)
        {
            string errorMessage = null;
            requestDTO = null;

            try
            {
                if (!Library.GetDTOFromRequest(request, out requestDTO, typeof(APIRequestDTO).FullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                Global.Log.DebugFormat("User ID: [{0}] changed the scheme to [Descentralized]", requestDTO.UserID);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }
        #endregion
    }
}