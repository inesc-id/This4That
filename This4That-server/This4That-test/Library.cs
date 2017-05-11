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
