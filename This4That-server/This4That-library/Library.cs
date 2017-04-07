using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.CalcTaskCostDTO;
using This4That_library.Models.Integration.TaskPayCreateDTO;
using This4That_library.Properties;

namespace This4That_library
{
    public class Library
    {
        private static bool GetTaskFromPostBody(string postBody, out APIRequestDTO csTask, string typeFullName, ref string errorMessage)
        {
            csTask = null;
            string calcTaskCostType = typeof(CalcTaskCostRequestDTO).FullName;
            try
            {
                if (String.IsNullOrEmpty(postBody))
                    return false;
                if (typeof(CalcTaskCostRequestDTO).FullName.Equals(typeFullName))
                {
                    csTask = JsonConvert.DeserializeObject<CalcTaskCostRequestDTO>(postBody);
                    return true;
                }
                if (typeof(TaskPayCreateRequestDTO).FullName.Equals(typeFullName))
                {
                    csTask = JsonConvert.DeserializeObject<TaskPayCreateRequestDTO>(postBody);
                    return true;
                }
                return false;

            }
            catch (Exception)
            {
                errorMessage = Resources.InvalidJSONFormat;
                csTask = null;
                return false;
            }
        }
        private static bool GetPostBodyFromRequest(HttpRequest request, out string postBody, ref string errorMessage)
        {
            postBody = null;
            try
            {
                postBody = new StreamReader(request.InputStream).ReadToEnd();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            
        }

        public static bool GetCSTaskFromRequest(HttpRequest request, out APIRequestDTO csTask, string typeFullName, ref string errorMessage)
        {
            string postBody;
            csTask = null;

            try
            {
                //get the entire post body
                if (!GetPostBodyFromRequest(request, out postBody, ref errorMessage))
                {
                    return false;
                }
                //get the userID and the encrypted Task
                if (!GetTaskFromPostBody(postBody, out csTask, typeFullName, ref errorMessage))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage += ex.Message;
                return false;
            }
        }

        public static string makeHttpJSONRequest(string url, string postBody)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.ContentLength = Encoding.UTF8.GetBytes(postBody).Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(postBody);
                }
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
