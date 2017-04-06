using System;
using System.Collections.Generic;
using System.Web;
using This4That_library;
using This4That_library.Models.Integration.CalcTaskCostDTO;
using This4That_library.Models.Integration.TaskPayDTO;
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
            string refToPay = null;
            TaskPayCreationDTO csTask;
            CalcTaskCostResponseDTO calcCostDTO = new CalcTaskCostResponseDTO();

            response = new APIResponseDTO();

            try
            {
                //get the DTO containing the UserID and the encrypted Task
                if (!GetCrowdSensingTask(out csTask))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Invalid Request please try again!" } }
                                        , APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }                    
                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(csTask.Task, csTask.UserID, out incentiveValue, out refToPay) 
                    || incentiveValue == null)
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot calculate the task cost. Please try again!" } }
                                        , APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                calcCostDTO.RefToPay = refToPay;
                calcCostDTO.ValToPay = incentiveValue.ToString();
                response.SetResponse(calcCostDTO, APIResponseDTO.RESULT_TYPE.SUCCESS);
                Global.Log.DebugFormat("User ID: [{0}] has to Pay [{1}] to Reference: [{2}]", csTask.UserID, incentiveValue.ToString(), refToPay);
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
            TaskPayCreationDTO csTask = null;
            TaskPayResponseDTO payResponse = new TaskPayResponseDTO();
            response = new APIResponseDTO();
            try
            {
                //get the DTO containing the userID, refToPay and task meta-info
                if (!GetCrowdSensingTask(out csTask))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Invalid Request please try again!" } }
                    , APIResponseDTO.RESULT_TYPE.ERROR);
                    return false;
                }
                //try to pay the task
                if (!PayCSTask(out txId, csTask.RefToPay))
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
                if (!CreateCSTask(out taskId, csTask))
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
                                    { "topic", serverMgr.RemoteTaskDistributor.GetTopic(topicName) } }
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

        #endregion

        #region PRIVATE_METHODS
        private bool CreateCSTask(out string taskID, TaskPayCreationDTO csTask)
        {
            taskID = null;
            try
            {
                if (!serverMgr.RemoteTaskCreator.CreateTask(csTask.Task, out taskID))
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

        private bool PayCSTask(out string transactionId, string refToPay)
        {
            transactionId = null;
            try
            {
                if (!serverMgr.RemoteIncentiveEngine.PayTask(refToPay, out transactionId))
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

        private bool GetCrowdSensingTask(out TaskPayCreationDTO csTask)
        {
            string errorMessage = null;
            csTask = null;

            try
            {
                if (!Library.GetCSTaskFromRequest(request, out csTask, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
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

        #endregion
    }
}