using System;
using System.Net;
using System.Web;
using System.Web.Http;
using This4That_platform.Integration;
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
            APIResponseDTO response = new APIResponseDTO();
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
                response.SetResponse("Cannot calculate the task cost. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("task")]
        public IHttpActionResult PayCreateCSTaskAPI()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            APIResponseDTO response = new APIResponseDTO();

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
                response.SetResponse("Cannot pay and create the crowd-sensing task. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet]
        [Route("topic")]
        public IHttpActionResult GetTopics()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            APIResponseDTO response = new APIResponseDTO();

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
                response.SetResponse("Cannot obtain topics from server. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("topic/tasks")]
        public IHttpActionResult GetTasksByTopic()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.GetTasksByTopicName(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain topic from server. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("report/{taskType}")]
        public IHttpActionResult ReportTaskResults(string taskType)
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler = null;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.ReportResultsCSTask(out response, taskType))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return Content(HttpStatusCode.InternalServerError, "ERROR");
            }
        }


        [HttpPost]
        [Route("user")]
        public IHttpActionResult RegisterNewUser()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.RegisterUser(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Registration failed. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet]
        [Route("user/{userId}/task")]
        public IHttpActionResult GetMyTasks(string userId)
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.GetUserTasks(userId, out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain my tasks. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost]
        [Route("subscribe")]
        public IHttpActionResult SubscribeTask()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.SubscribeTopic(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain subscribed tasks. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }

        }

        [HttpGet]
        [Route("user/{userId}/subscribedtasks")]
        public IHttpActionResult GetSubscribedTasks(string userId)
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.GetSubscribedTasks(userId, out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain subscribed tasks. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }

        }

        [HttpPost]
        [Route("subscribedtasks")]
        public IHttpActionResult GetSubscribedTasksByTopicName()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.GetSubscribedTasksByTopicName(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain subscribed tasks. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }

        }

        [HttpPost]
        [Route("subscribedtask/execute")]
        public IHttpActionResult ExecuteSubscribedTask()
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.ExecuteSubscribedTask(out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain subscribed tasks. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }

        }

    }
}
