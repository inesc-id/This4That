using System;
using System.Collections.Generic;
using System.Web;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.CalcTaskCostDTO;
using This4That_library.Models.Integration.TaskPayCreateDTO;
using This4That_platform.Integration;
using This4That_platform.Properties;

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
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Invalid Request please try again!" } }
                                        , APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }                    
                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(csTask.Task, csTask.UserID, out incentiveValue) 
                    || incentiveValue == null)
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot calculate the task cost. Please try again!" } }
                                        , APIResponseDTO.RESULT_TYPE.ERROR);
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
                response.SetResponse(new Dictionary<string, object>() {
                                    { "topics", serverMgr.RemoteTaskDistributor.GetTopics() } }
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
                //get the DTO containing the userID, refToPay and task meta-info
                if (!GetCSTaskToCreate(out payRequest))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Invalid Request please try again!" } }
                    , APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                //try to pay the task
                if (!PayCSTask(out txId, payRequest))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot perform the payment!" } }
                    , APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                if (txId.Equals(Resources.InsufficientFunds))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Insufficient Credits!" } }
                                        , APIResponseDTO.RESULT_TYPE.ERROR);
                    return true;
                }
                if (!CreateCSTask(out taskId, payRequest))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot create the crowd-sensing task, please try again!" } }
                    , APIResponseDTO.RESULT_TYPE.ERROR);
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

        public bool GetTasksByTopicName(string topicName, out APIResponseDTO response)
        {
            response = new APIResponseDTO();
            try
            {
                response.SetResponse(new Dictionary<string, object>() {
                                    { "tasks", serverMgr.RemoteTaskDistributor.GetTasksByTopicName(topicName) } }
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

        public bool ReportResultsCSTask()
        {
            string jsonBody = null;
            try
            {
                if (!serverMgr.RemoteReportAggregator.CreateReport(jsonBody))
                {
                    Global.Log.Error("Cannot generate Report from user results!");
                    return false;
                }
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
                userId = this.serverMgr.RemoteRepository.RegisterUser();
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
                if (!serverMgr.RemoteIncentiveEngine.PayTask(request.UserID, out transactionId))
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
                if (!Library.GetCSTaskFromRequest(request, out requestDTO, typeFullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                csTask = (TaskPayCreateRequestDTO) requestDTO;
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
                if (!Library.GetCSTaskFromRequest(request, out requestDTO, typeFullName, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                csTask = (CalcTaskCostRequestDTO) requestDTO;
                Global.Log.DebugFormat("User ID: [{0}] Task: {1}", csTask.UserID, csTask.Task.ToString());
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