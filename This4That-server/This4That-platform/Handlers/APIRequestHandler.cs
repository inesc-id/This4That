using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;
using This4That_library.Models.Integration;

namespace This4That_platform.Handlers
{
    public class APIRequestHandler
    {
        public bool CalcCrowdSensingTaskCost(HttpRequest request, ServerManager serverMgr, out Object incentiveValue, ref string errorMessage)
        {
            incentiveValue = null;
            JSONTaskDTO csTask;
            try
            {
                //get the DTO containing the UserID and the encrypted Task
                if (!GetCrowdSensingTask(request, out csTask, ref errorMessage))
                    return false;
                //authenticate the user identification
                if (!serverMgr.RemoteRepository.AuthenticateUser(csTask.UserID))
                    return false;

                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(csTask.Task, out incentiveValue) || incentiveValue == null)
                {
                    Global.Log.Error("Cannot calculate incentive value!");
                    return false;
                }
                Global.Log.DebugFormat("Incentive Value: [{0}]", incentiveValue.ToString());         
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }
        
        public bool CreateCrowdSensingTask(HttpRequest request, ServerManager serverMgr, out string taskID, ref string errorMessage)
        {
            JSONTaskDTO csTask;
            taskID = null;
            try
            {
                //get the DTO containing the UserID and the encrypted Task
                if (!GetCrowdSensingTask(request, out csTask, ref errorMessage))
                    return false;

                if (!serverMgr.RemoteIncentiveEngine.IsTaskPaid(csTask.TransactionID))
                {
                    return false;
                }
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

        internal bool PayCrowdSensingTask(HttpRequest request, ServerManager serverMgr, out string transactionId)
        {
            throw new NotImplementedException();
        }

        public bool ReportTaskResults(HttpRequest request, ServerManager serverMgr)
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

        private bool GetCrowdSensingTask(HttpRequest request, out JSONTaskDTO csTask, ref string errorMessage)
        {
            csTask = null;

            try
            {
                if (!Library.GetCSTaskFromRequest(HttpContext.Current.Request, out csTask, ref errorMessage))
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

    }
}