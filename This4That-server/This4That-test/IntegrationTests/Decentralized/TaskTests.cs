using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_test.IntegrationDTOs;

namespace This4That_test.IntegrationTests.Decentralized
{
    [TestClass]
    public class TaskTests
    {
        [TestMethod]
        public void CalculateVALIDTaskCost()
        {
            string jsonBody = null;
            string response = null;
            CalcTaskCostResponseDTO responseDTO;
            
            jsonBody = Library.ReadStringFromFile(Library.VALID_CALC_TASK_COST_PATH);

            response = Library.MakeHttpPOSTJSONRequest(Library.CALC_TASK_API_URL, jsonBody);
            responseDTO = JsonConvert.DeserializeObject<CalcTaskCostResponseDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseDTO.ErrorCode);
            Assert.IsNotNull(responseDTO.Response.IncentiveToPay.Name);
            Assert.IsNotNull(responseDTO.Response.IncentiveToPay.Quantity);
        }

        [TestMethod]
        public void CalcMultipleProxieRequests()
        {
            string jsonBody = null;
            string response = null;
            CalcTaskCostResponseDTO responseDTO;
            int loopTimes = 20;
            List<ProxyDTO> listProxies = new List<ProxyDTO>();
            ProxyDTO proxyInfo = null;

            for(int i = 0; i < loopTimes; i++)
            {
                
                jsonBody = Library.ReadStringFromFile(Library.VALID_CALC_TASK_COST_PATH);
                response = Library.MakeHttpPostJSONRequestWithTime(Library.CALC_TASK_API_URL, jsonBody, ref proxyInfo);
                responseDTO = JsonConvert.DeserializeObject<CalcTaskCostResponseDTO>(response);
                listProxies.Add(proxyInfo);
            }
            foreach (ProxyDTO proxy in listProxies)
            {
                proxy.ToString();
            }
            /*
            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseDTO.ErrorCode);
            Assert.IsNotNull(responseDTO.Response.IncentiveToPay.Name);
            Assert.IsNotNull(responseDTO.Response.IncentiveToPay.Quantity);
            */
        }

        [TestMethod]
        public void CalculateINVALIDTaskCost()
        {
            string jsonBody = null;
            string response = null;
            CalcTaskCostResponseDTO responseDTO;

            jsonBody = Library.ReadStringFromFile(Library.INVALID_CALC_TASK_COST_PATH);

            response = Library.MakeHttpPOSTJSONRequest(Library.CALC_TASK_API_URL, jsonBody);
            responseDTO = JsonConvert.DeserializeObject<CalcTaskCostResponseDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_FAIL, responseDTO.ErrorCode);
            Assert.IsNull(responseDTO.Response);

        }

        [TestMethod]
        public void CreateVALIDTask_Decentralized()
        {
            string jsonBody = null;
            string response = null;
            CreateTaskResponseDTO responseCreateTaskDTO;
            GetMyTasksDTO responseGetMyTasksDTO;

            bool taskFound = false;

            jsonBody = Library.ReadStringFromFile(Library.VALID_CALC_TASK_COST_PATH);

            //create task
            response = Library.MakeHttpPOSTJSONRequest(Library.CREATE_TASK_API_URL, jsonBody);
            responseCreateTaskDTO = JsonConvert.DeserializeObject<CreateTaskResponseDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseCreateTaskDTO.ErrorCode);
            Assert.IsNotNull(responseCreateTaskDTO.Response.TaskID);
            Assert.IsNotNull(responseCreateTaskDTO.Response.TransactionID);

            /*
            //check for task created
            response = Library.MakeHttpGETRequest(Library.GET_USER_TASKS_API_URL);
            responseGetMyTasksDTO = JsonConvert.DeserializeObject<GetMyTasksDTO>(response);

            foreach (CSTaskDTO task in responseGetMyTasksDTO.Response)
            {
                if (task.TaskID == responseCreateTaskDTO.Response.TaskID)
                    taskFound = true;
            }
            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseGetMyTasksDTO.ErrorCode);
            Assert.IsTrue(taskFound);
            */
        }

        [TestMethod]
        public void CreateINVALIDTask()
        {
            string jsonBody = null;
            string response = null;
            CreateTaskResponseDTO responseCreateTaskDTO;

            jsonBody = Library.ReadStringFromFile(Library.INVALID_CALC_TASK_COST_PATH);

            //create task
            response = Library.MakeHttpPOSTJSONRequest(Library.CREATE_TASK_API_URL, jsonBody);
            responseCreateTaskDTO = JsonConvert.DeserializeObject<CreateTaskResponseDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_FAIL, responseCreateTaskDTO.ErrorCode);
            Assert.IsNull(responseCreateTaskDTO.Response);
        }
    }
}
