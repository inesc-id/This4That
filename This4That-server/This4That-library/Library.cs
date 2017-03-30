using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;
using This4That_library.Models.Domain;
using This4That_library.Models.Integration;

namespace This4That_library
{
    public class Library
    {
        private static bool GetTaskFromPostBody(string postBody, out JSONTaskDTO csTask, ref string errorMessage)
        {
            csTask = null;

            try
            {
                if (String.IsNullOrEmpty(postBody))
                    return false;
                csTask = JsonConvert.DeserializeObject<JSONTaskDTO>(postBody);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
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

        public static bool GetCSTaskFromRequest(HttpRequest request, out JSONTaskDTO csTask, ref string errorMessage)
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
                if (!GetTaskFromPostBody(postBody, out csTask, ref errorMessage))
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

        public static bool GetEncryptedReport(HttpRequest request, out string encryptedTask, object serverMgrKey, ref string errorMessage)
        {
            encryptedTask = null;
            return true;
        }
    }
    
    public class SecurityLibrary
    {
        public static bool DecryptString(string inputString, out string outputString, object key, ref string errorMessage)
        {
            outputString = null;
            return true;
        }
    }
}
