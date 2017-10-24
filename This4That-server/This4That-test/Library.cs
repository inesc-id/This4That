using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using This4That_test.IntegrationDTOs;

namespace This4That_test
{
    public class Library
    {
        //endpoints URL
        public const string API_URL = @"http://31.22.162.198:58949/api/v1/";
        public const string CALC_TASK_API_URL = API_URL + "task/cost";
        public const string CREATE_TASK_API_URL = API_URL + "task";
        public const string GET_USER_TRANSACTIONS_API_URL = API_URL + "user/fdd47f01/transactions";
        public const string GET_USER_TASKS_API_URL = API_URL + "user/fdd47f01/task";
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

        //http proxie API
        public const string GIMMEPROXY_API_URL = @"https://gimmeproxy.com/api/getProxy?get=true&post=true&protocol=http&anonymityLevel=1&?maxCheckPeriod=3600";

        //errorCode
        public const int ERROR_CODE_SUCCESS = 1;
        public const int ERROR_CODE_FAIL = -1;

        public static string MakeHttpPostJSONRequestWithTime(string url, string postBody, ref ProxyDTO proxyDTO)
        {
            string response = null;
            WebProxy proxy;
            int proxyPort = -1;
            Stopwatch timer = new Stopwatch();

            while (response == null)
            {
                timer.Start();
                proxyDTO = GetRandomProxy();
                int.TryParse(proxyDTO.Port, out proxyPort);

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                proxy = new WebProxy(proxyDTO.IpAddress, proxyPort);
                proxy.BypassProxyOnLocal = false;

                httpWebRequest.Proxy = proxy;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.ContentLength = Encoding.UTF8.GetBytes(postBody).Length;

                try
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(postBody);
                    }
                    response = GetResponseFromRequest(httpWebRequest);
                    timer.Stop();
                    if (response == null)
                        timer.Reset();
                    else
                    {
                        proxyDTO.ExecTime = timer.Elapsed;
                        if (response.Contains("<html>"))
                            response = null;
                    }
                }
                catch (Exception ex)
                {
                    response = null;
                    timer.Reset();
                }
            }
            return response;
        }

        public static string MakeHttpPOSTJSONRequest(string url, string postBody)
        {
            string response = null;
            WebProxy proxy;
            ProxyDTO proxyDTO;
            int proxyPort = -1;

            while (response == null)
            {
                proxyDTO = GetRandomProxy();
                int.TryParse(proxyDTO.Port, out proxyPort);

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                proxy = new WebProxy(proxyDTO.IpAddress, proxyPort);
                proxy.BypassProxyOnLocal = false;

                httpWebRequest.Proxy = proxy;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.ContentLength = Encoding.UTF8.GetBytes(postBody).Length;

                try
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(postBody);
                    }
                    response = GetResponseFromRequest(httpWebRequest);
                }
                catch (Exception ex)
                {
                    response = null;
                }
            }
            return response;
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

        public static ProxyDTO GetRandomProxy()
        {
            string resultGET = null;

            resultGET = MakeHttpGETRequest(Library.GIMMEPROXY_API_URL);
            return JsonConvert.DeserializeObject<ProxyDTO>(resultGET);
        }

        private static string GetResponseFromRequest(HttpWebRequest request)
        {
            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();

            try
            {
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
            catch(Exception ex)
            {
                return null;
            }

        }

    }
}
