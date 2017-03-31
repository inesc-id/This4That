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
        [Route("task/cost")]
        public IHttpActionResult GetTaskCost()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            object incentiveValue;
            string errorMessage = null;
            try
            {

                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                if (!handler.CalcCrowdSensingTaskCost(HttpContext.Current.Request, serverMgr, out incentiveValue, ref errorMessage))
                {
                    return Content(HttpStatusCode.InternalServerError, errorMessage);
                }
                return Content(HttpStatusCode.OK, "Incentive Value: " + incentiveValue);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return Content(HttpStatusCode.InternalServerError, "ERROR: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("task/pay")]
        public IHttpActionResult PayTask()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            String transactionId;
            string errorMessage = null;
            try
            {

                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                if (!handler.PayCrowdSensingTask(HttpContext.Current.Request, serverMgr, out transactionId))
                {
                    return Content(HttpStatusCode.InternalServerError, errorMessage);
                }
                return Content(HttpStatusCode.OK, transactionId);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return Content(HttpStatusCode.InternalServerError, "ERROR: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("task")]
        public IHttpActionResult CreateCrowdSensingTask()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            string taskID;
            string errorMessage = null;
            
            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                if (!handler.CreateCrowdSensingTask(HttpContext.Current.Request, serverMgr, out taskID, ref errorMessage))
                {
                    return Content(HttpStatusCode.InternalServerError, errorMessage);
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
            APIRequestHandler handler = null;
            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                if (!handler.ReportTaskResults(HttpContext.Current.Request, serverMgr))
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
