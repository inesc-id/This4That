using System;
using System.Net;
using System.Web;
using System.Web.Http;
using This4That_platform.Handlers;
using This4That_platform.Models.Integration;

namespace This4That_platform.ServiceLayer
{
    [RoutePrefix("api/v1")]
    public class ServerAPIController : ApiController
    {

        /// <summary>
        /// Calculate the Task cost.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Create a task.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get a list of topics.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get tasks info for a given topic name.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Report the tasks result based on the task type.
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Get user's tasks.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Subscribes a topic.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("subscribe")]
        public IHttpActionResult SubscribeTopic()
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

        /// <summary>
        /// Get all tasks that belong to the topics subscribed.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get all subscribed tasks based on a topic name.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Execute a subscribed task.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get user's transactions.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("user/{userId}/transactions")]
        public IHttpActionResult GetUserTransactions(string userId)
        {
            ServerManager serverMgr = null;
            APIRequestHandler handler;
            APIResponseDTO response = new APIResponseDTO();

            try
            {
                serverMgr = Global.GetCreateServerManager(HttpContext.Current.Server);
                handler = new APIRequestHandler(HttpContext.Current.Request, serverMgr);
                if (!handler.GetUserTransactions(userId, out response))
                {
                    return Content(HttpStatusCode.InternalServerError, response);
                }
                return Content(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                response.SetResponse("Cannot obtain user transactions. Please try again!", APIResponseDTO.RESULT_TYPE.ERROR);
                return Content(HttpStatusCode.InternalServerError, response);
            }

        }

    }
}
