using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;

namespace This4That_platform.Handlers
{
    public class APIRequestHandler
    {
        public static bool CalcCrowdSensingTaskCost(HttpRequest request, ServerManager serverMgr, out Object incentiveValue)
        {
            string encryptedTask = null;
            string errorMessage = null;
            incentiveValue = null;
            try
            {
                if (!Library.GetJsonTaskFromHttpRequest(request, out encryptedTask, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                Global.Log.DebugFormat("Encrypted Task: {0}", encryptedTask);
                if (!serverMgr.RemoteIncentiveEngine.CalcTaskCost(encryptedTask, out incentiveValue))
                {
                    Global.Log.Error("Cannot calculate incentive value!");
                    return false;
                }
                if (incentiveValue == null)
                {
                    Global.Log.Error("Invalid Incentive Value!");
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

        internal static bool CreateCrowdSensingTask(HttpRequest request, ServerManager serverMgr, out int taskID)
        {
            string encryptedTask = null;
            string errorMessage = null;
            taskID = -1;
            try
            {
                if (!Library.GetJsonTaskFromHttpRequest(request, out encryptedTask, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                Global.Log.DebugFormat("Encrypted Task: {0}", encryptedTask);
                if (!serverMgr.RemoteIncentiveEngine.IsTaskPaid())
                {
                    return true;
                }
                if (!serverMgr.RemoteTaskCreator.CreateTask(encryptedTask, out taskID))
                {
                    Global.Log.Error("Cannot create crowd-sensing task!");
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
    }
}