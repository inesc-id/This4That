﻿using System;
using System.Net;
using System.Web;
using System.Web.Http;
using This4That_platform.Domain;
using This4That_platform.Handlers;

namespace This4That_platform.ServiceLayer
{
    [RoutePrefix("api")]
    public class ServerAPIController : ApiController
    {
        [HttpPost]
        [Route("task/cost")]
        public IHttpActionResult GetCostCSTaskAPI()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            APIResponse response = new APIResponse();
            try
            {

                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);

                if (!handler.CalcCostCSTask(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot calculate the task cost. Please try again!", APIResponse.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("task")]
        public IHttpActionResult PayCreateCSTaskAPI()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            APIResponse response = new APIResponse();
            
            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.PayCreateCSTask(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot pay and create the crowd-sensing task. Please try again!", APIResponse.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet]
        [Route("topic")]
        public IHttpActionResult GetTopics()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            APIResponse response = new APIResponse();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.GetTopics(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain topics from server. Please try again!", APIResponse.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("report")]
        public IHttpActionResult ReportTaskResults()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                
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
