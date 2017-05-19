using System;
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

namespace This4That_platform.Handlers
{
    public class APIRequestHandler
    {
        private HttpRequest request;
        private ServerManager serverMgr;

        public APIRequestHandler(HttpRequest request, ServerManager serverMgr)
        {
            this.request = request;
            this.serverMgr = serverMgr;
        }

        #region PUBLIC_METHODS
        /// <summary>
        /// Calculate the cost for a crowd-sensing task.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool CalcCostCSTask(out APIResponseDTO response)
        {
            object incentiveValue = null;
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
                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(csTask.Task, csTask.UserID, out incentiveValue) 
                    || incentiveValue == null)
                {
                    response.SetErrorResponse("Cannot calculate the task cost. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                calcCostDTO.ValToPay = incentiveValue.ToString();
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
            string txId = null;
            string taskId = null;
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
                //try to pay the task
                if (!PayCSTask(out txId, payRequest))
                {
                    response.SetErrorResponse("Cannot perform the payment!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                //transaction not performed due to insufficient credits
                if (txId == null)
                {
                    response.SetErrorResponse("Insufficient Credits!", APIResponseDTO.RESULT_TYPE.INSUFFICIENT_CREDITS);
                    return true;
                }
                if (!CreateCSTask(out taskId, payRequest))
                {
                    response.SetErrorResponse("Cannot create the crowd-sensing task, please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                payResponse.TaskID = taskId;
                payResponse.TransactionID = txId;
                response.SetResponse(payResponse, APIResponseDTO.RESULT_TYPE.SUCCESS);
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

        public bool ReportResultsCSTask(out APIResponseDTO response, string taskType)
        {
            TaskTypeEnum taskTypeEnum;
            ReportDTO reportReqDTO = null;
            object rewardObj;
            string txId = null;
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
                if (!serverMgr.RemoteReportAggregator.SaveReport(reportReqDTO))
                {
                    Global.Log.Error("Cannot generate Report from user results!");
                    return false;
                }
                if (!serverMgr.RemoteIncentiveEngine.RewardUser(reportReqDTO.UserID, out txId, out rewardObj))
                {
                    Global.Log.Error("Cannot reward user!");
                    return false;
                }
                response.SetResponse(new Dictionary<string, string>() { { "reward", rewardObj.ToString() }, { "txId", txId } }
                                    , APIResponseDTO.RESULT_TYPE.SUCCESS);
                Global.Log.Debug("Report generated with SUCCESS!");
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
            response = new APIResponseDTO();
            try
            {
                userId = this.serverMgr.RemoteIncentiveEngine.RegisterUser();
                response.SetResponse(new Dictionary<string, object>() {
                                    { "userId", userId} }
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

        private bool PayCSTask(out string transactionId, TaskPayCreateRequestDTO request)
        {
            transactionId = null;
            object incentiveValue;
            try
            {
                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(request.Task, request.UserID, out incentiveValue)
                    || incentiveValue == null)
                {
                    return false;
                }
                if (!serverMgr.RemoteIncentiveEngine.PayTask(request.UserID, incentiveValue, out transactionId))
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
                Global.Log.DebugFormat("User ID: [{0}] Task: {1}", csTask.UserID, csTask.Task.ToString());
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
        #endregion
    }
}