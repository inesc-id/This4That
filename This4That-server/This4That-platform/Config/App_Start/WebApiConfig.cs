using Newtonsoft.Json;
using System.Web.Http;

namespace This4That_platform.App_Start
{
    public class WebApiConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "This4ThatAPI",
                routeTemplate: "api/{controller}"
            );
            //response JSON format
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
        }
    }
}