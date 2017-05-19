using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test
{
    public class Library
    {
        //endpoints URL
        public const string API_URL = @"http://localhost:58949/api/v1/";
        public const string CALC_TASK_API_URL = API_URL + "task/cost";
        public const string CREATE_TASK_API_URL = API_URL + "task";
        public const string GET_USER_TRANSACTIONS_API_URL = API_URL + "user/1234/transactions";
        public const string GET_USER_TASKS_API_URL = API_URL + "user/1234/task";
        public const string GET_TOPICS_API_URL = API_URL + "topic";
        public const string GET_TASKS_TOPIC_NAME_API_URL = API_URL + "topic/tasks";
        public const string REPORT_INTERACTIVE_TASK_RESULT_API_URL = API_URL + "report/InteractiveTask";
        public const string SUBSCRIBE_TOPIC_API_URL = API_URL + "subscribe/";

        //integration files path
        public const string INTEGRATION_FILES_PATH = @"C:\Users\Calado\Documents\Documentos Word\Faculdade\IST 5ano\Tese\Dissertation\This4That\This4That-server\This4That-test\IntegrationFiles\";
        public const string VALID_CALC_TASK_COST_PATH = INTEGRATION_FILES_PATH + @"\CalcTaskCost\valid.json";
        public const string INVALID_CALC_TASK_COST_PATH = INTEGRATION_FILES_PATH + @"\CalcTaskCost\invalid.json";
        public const string VALID_GET_TASKS_TOPIC_PATH = INTEGRATION_FILES_PATH + @"\GetTasksByTopic\valid.json";
        public const string INVALID_GET_TASKS_TOPIC_PATH = INTEGRATION_FILES_PATH + @"\GetTasksByTopic\invalid.json";
        public const string VALID_REPORT_TASK__RESULT_PATH = INTEGRATION_FILES_PATH + @"\ReportTaskResult\valid.json";

        //errorCode
        public const int ERROR_CODE_SUCCESS = 1;
        public const int ERROR_CODE_FAIL = -1;

        public static string MakeHttpPOSTJSONRequest(string url, string postBody)
        {
            HttpWebResponse httpResponse;
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

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (WebException webEx)
            {
                httpResponse = (HttpWebResponse) webEx.Response;

                //force to read the response
                if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string MakeHttpGETRequest(string url)
        {
            HttpWebResponse httpResponse;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (WebException webEx)
            {
                httpResponse = (HttpWebResponse)webEx.Response;

                //force to read the response
                if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ReadStringFromFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
