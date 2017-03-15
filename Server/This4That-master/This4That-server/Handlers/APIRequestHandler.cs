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
        internal static bool CalcCrowdSensingTaskCost(HttpRequest request, ServerManager serverMgr, out Object incentiveValue)
        {
            incentiveValue = null;
            JSONEncryptedTaskDTO encryptedTask;
            try
            {
                //get the DTO containing the UserID and the encrypted Task
                if (!serverMgr.SrvAuth.GetEncryptedTask(request, out encryptedTask, null) || encryptedTask == null)
                    return false;
                //authenticate the user identification
                if (!serverMgr.RemoteRepository.AuthtenticateUser(encryptedTask.UserID))
                    return false;

                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(encryptedTask.EncryptedTask, out incentiveValue) || incentiveValue == null)
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

        internal static bool CreateCrowdSensingTask(HttpRequest request, ServerManager serverMgr, out string taskID)
        {
            JSONEncryptedTaskDTO encryptedTask;
            taskID = null;
            try
            {
                //get the DTO containing the UserID and the encrypted Task
                if (!serverMgr.SrvAuth.GetEncryptedTask(request, out encryptedTask, null) || encryptedTask == null)
                    return false;

                Global.Log.DebugFormat("Encrypted Task: {0}", encryptedTask);
                if (!serverMgr.RemoteIncentiveEngine.IsTaskPaid(encryptedTask.TransactionID))
                {
                    return false;
                }
                if (!serverMgr.RemoteTaskCreator.CreateTask(encryptedTask.EncryptedTask, out taskID))
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

        internal static bool ReportTaskResults(HttpRequest request, ServerManager serverMgr)
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
    }
}