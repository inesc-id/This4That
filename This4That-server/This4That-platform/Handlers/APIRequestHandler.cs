using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;
using This4That_library.Models.Domain;
using This4That_library.Models.Integration;
using This4That_platform.Domain;
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
        public bool CalcCostCSTask(out APIResponse response)
        {
            object incentiveValue = null;
            string refToPay = null;
            TaskPayCreationDTO csTask;
            Dictionary<string, object> taskCost;
            response = new APIResponse();

            try
            {
                //get the DTO containing the UserID and the encrypted Task
                if (!GetCrowdSensingTask(out csTask))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Invalid Request please try again!" } }
                                        , APIResponse.RESULT_TYPE.ERROR);
                    return false;
                }                    
                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(csTask.Task, csTask.UserID, out incentiveValue, out refToPay) 
                    || incentiveValue == null)
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot calculate the task cost. Please try again!" } }
                                        , APIResponse.RESULT_TYPE.ERROR);
                    return false;
                }
                taskCost = new Dictionary<string, object>() { { "refToPay", refToPay }, { "valToPay", incentiveValue.ToString() } };
                response.SetResponse(taskCost, APIResponse.RESULT_TYPE.SUCCESS);
                Global.Log.DebugFormat("User ID: [{0}] has to Pay [{1}] to Reference: [{2}]", csTask.UserID, incentiveValue.ToString(), refToPay);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot calculate the task cost. Please try again!" } }
                                    , APIResponse.RESULT_TYPE.ERROR);
                return false;
            }
        }

        public bool GetTopics(out APIResponse response)
        {
            response = new APIResponse();
            try
            {
                response.SetResponse(new Dictionary<string, object>() {
                                    { "topics", serverMgr.RemoteRepository.GetTopics() } }
                                    , APIResponse.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain topics from server. Please try again!", APIResponse.RESULT_TYPE.ERROR);
                return false;
            }
        }

        /// <summary>
        /// Pay and create a crowd-sensing task.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool PayCreateCSTask(out APIResponse response)
        {
            string txId = null;
            string taskId = null;
            TaskPayCreationDTO csTask = null;
            response = new APIResponse();
            try
            {
                //get the DTO containing the userID, refToPay and task meta-info
                if (!GetCrowdSensingTask(out csTask))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Invalid Request please try again!" } }
                    , APIResponse.RESULT_TYPE.ERROR);
                    return false;
                }
                //try to pay the task
                if (!PayCSTask(out txId, csTask.RefToPay))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot perform the payment!" } }
                    , APIResponse.RESULT_TYPE.ERROR);
                    return false;
                }
                if (txId.Equals(Resources.InsufficientFunds))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Insufficient Credits!" } }
                                        , APIResponse.RESULT_TYPE.ERROR);
                    return true;
                }
                if (!CreateCSTask(out taskId, csTask))
                {
                    response.SetResponse(new Dictionary<string, string>() { { "errorMessage", "Cannot create the crowd-sensing task, please try again!" } }
                    , APIResponse.RESULT_TYPE.ERROR);
                    return false;
                }
                response.SetResponse(new Dictionary<string, string>() { { "taskId", taskId } }, APIResponse.RESULT_TYPE.SUCCESS);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.ErrorFormat("PayCreateCrowdSensingTask", ex.Message);
                return false;
            }

        }

        public bool ReportResultsCSTask()
        {
            string errorMessage = null;
            string jsonBody;
            try
            {
                if (!Library.GetEncryptedReport(request, out jsonBody, null, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
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