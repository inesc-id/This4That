using System;
using System.Net;
using System.Web;
using System.Web.Http;
using This4That_platform.Handlers;

namespace This4That_platform.ServiceLayer
{
    [RoutePrefix("api")]
    public class ServerAPIController : ApiController
    {
        [HttpPost]
        [Route("taskCost")]
        public IHttpActionResult GetTaskCost()
        {
            ServerManager serverMgr = null;
            object incentiveValue;
            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                if (!APIRequestHandler.CalcCrowdSensingTaskCost(HttpContext.Current.Request, serverMgr, out incentiveValue))
                {
                    return Content(HttpStatusCode.InternalServerError, "ERROR");
                }
                return Content(HttpStatusCode.OK, "Incentive Value: " + incentiveValue);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return Content(HttpStatusCode.InternalServerError, "ERROR");
            }
        }
        
        [HttpPost]
        [Route("task")]
        public IHttpActionResult CreateCrowdSensingTask()
        {
            ServerManager serverMgr = null;
            string taskID;
            
            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                if (!APIRequestHandler.CreateCrowdSensingTask(HttpContext.Current.Request, serverMgr, out taskID))
                {
                    return Content(HttpStatusCode.InternalServerError, "ERROR");
                }
                return Content(HttpStatusCode.OK, "Task ID: " + taskID);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return Content(HttpStatusCode.InternalServerError, "ERROR");
            }
        }

        [HttpPost]
        [Route("report")]
        public IHttpActionResult ReportTaskResults()
        {
            ServerManager serverMgr = null;

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                if (!APIRequestHandler.ReportTaskResults(HttpContext.Current.Request, serverMgr))
                {
                    return Content(HttpStatusCode.InternalServerError, "ERROR");
                }
                return Content(HttpStatusCode.OK, "OK");
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return Content(HttpStatusCode.InternalServerError, "ERROR");
            }
        }

    }
}
