﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.CalcTaskCostDTO;
using This4That_library.Models.Integration.ExecuteTaskDTO;
using This4That_library.Models.Integration.GetMultichainAddress;
using This4That_library.Models.Integration.GetUserTopicDTO;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.Integration.TaskPayCreateDTO;
using This4That_library.Properties;

namespace This4That_library
{
    public class Library
    {
        private static bool GetDTOFromPostBody(string postBody, out APIRequestDTO requestDTO, string typeFullName, ref string errorMessage)
        {
            requestDTO = null;

            try
            {
                //check the right type to deserialize
                if (String.IsNullOrEmpty(postBody))
                    return false;

                if (typeof(APIRequestDTO).FullName.Equals(typeFullName))
                {
                    requestDTO = JsonConvert.DeserializeObject<APIRequestDTO>(postBody);
                    return true;
                }
                if (typeof(CalcTaskCostRequestDTO).FullName.Equals(typeFullName))
                {
                    requestDTO = JsonConvert.DeserializeObject<CalcTaskCostRequestDTO>(postBody);
                    return true;
                }
                if (typeof(TaskPayCreateRequestDTO).FullName.Equals(typeFullName))
                {
                    requestDTO = JsonConvert.DeserializeObject<TaskPayCreateRequestDTO>(postBody);
                    return true;
                }
                if (typeof(GetTopicRequestDTO).FullName.Equals(typeFullName))
                {
                    requestDTO = JsonConvert.DeserializeObject<GetTopicRequestDTO>(postBody);
                    return true;
                }
                //the typeFullName variable is used to check the sensingtasktype
                if (typeFullName.Equals(TaskTypeEnum.InteractiveTask.ToString()))
                {
                    requestDTO = JsonConvert.DeserializeObject<InteractiveTaskReportDTO>(postBody);
                    return true;
                }
                if (typeFullName.Equals(TaskTypeEnum.SensingTask.ToString()))
                {
                    requestDTO = JsonConvert.DeserializeObject<SensingTaskReportDTO>(postBody);
                    return true;
                }

                if (typeof(ExecuteTaskDTO).FullName.Equals(typeFullName))
                {
                    requestDTO = JsonConvert.DeserializeObject<ExecuteTaskDTO>(postBody);
                    return true;
                }
                if (typeof(GetMultichainAddressDTO).FullName.Equals(typeFullName))
                {
                    requestDTO = JsonConvert.DeserializeObject<GetMultichainAddressDTO>(postBody);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                errorMessage = Resources.InvalidJSONFormat;
                requestDTO = null;
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

        public static bool GetDTOFromRequest(HttpRequest request, out APIRequestDTO requestDTO, string typeFullName, ref string errorMessage)
        {
            string postBody;
            requestDTO = null;

            try
            {
                //get the entire post body
                if (!GetPostBodyFromRequest(request, out postBody, ref errorMessage))
                {
                    return false;
                }
                if (!GetDTOFromPostBody(postBody, out requestDTO, typeFullName, ref errorMessage))
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

        public static long GetUnixTimestamp(DateTime time)
        {
            return (long)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }


        //STATISTICAL METHODS
        // https://www.mathsisfun.com/data/quartiles.html -> Quartiles
        //https://en.wikipedia.org/wiki/Outlier -> Tukeys Test
        // Q1 - 1.5* IQR and Q3 + 1.5IQR

        public static double GetMedian(List<double> colValues)
        {
            // Create a copy of the input, and sort the copy
            double[] temp = colValues.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                return -1;
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                double a = temp[ count / 2 - 1];
                double b = temp[count / 2];
                return (a + b) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }
    }
}
