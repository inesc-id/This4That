using log4net;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Http;
using This4That_platform.App_Start;

namespace This4That_platform
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ServerManager serverMgr;

        public static ILog Log
        {
            get
            {
                return log;
            }
        }

        public static ServerManager GetCreateServerManager(HttpServerUtility server)
        {
            if (serverMgr == null)
            {
                serverMgr = new ServerManager(server);
            }
            return serverMgr;
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Configure);
            GetCreateServerManager(Server);
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}