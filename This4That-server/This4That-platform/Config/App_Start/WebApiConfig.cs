using System.Web.Http;

namespace This4That_platform.App_Start
{
    public class WebApiConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "T4TApi",
                routeTemplate: "api/{controller}"
            );
        }
    }
}