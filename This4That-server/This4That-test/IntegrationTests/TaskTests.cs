using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_test.IntegrationDTOs;

namespace This4That_test.IntegrationTests
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
            Assert.IsNotNull(responseDTO.Response.ValToPay);
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
        public void CreateVALIDTask_CheckTransaction_Centralized()
        {
            string jsonBody = null;
            string response = null;
            CreateTaskResponseDTO responseCreateTaskDTO;
            GetTransactionsDTO responseGetTransactionsDTO;
            GetMyTasksDTO responseGetMyTasksDTO;

            bool txFound = false;
            bool taskFound = false;

            jsonBody = Library.ReadStringFromFile(Library.VALID_CALC_TASK_COST_PATH);

            //create task
            response = Library.MakeHttpPOSTJSONRequest(Library.CREATE_TASK_API_URL, jsonBody);
            responseCreateTaskDTO = JsonConvert.DeserializeObject<CreateTaskResponseDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseCreateTaskDTO.ErrorCode);
            Assert.IsNotNull(responseCreateTaskDTO.Response.TaskID);
            Assert.IsNotNull(responseCreateTaskDTO.Response.TransactionID);

            //check for transaction
            response = Library.MakeHttpGETRequest(Library.GET_USER_TRANSACTIONS_API_URL);
            responseGetTransactionsDTO = JsonConvert.DeserializeObject<GetTransactionsDTO>(response);
           
            foreach(Transaction transaction in responseGetTransactionsDTO.Response)
            {
                if (transaction.TxID == responseCreateTaskDTO.Response.TransactionID)
                    txFound = true;
            }
            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseGetTransactionsDTO.ErrorCode);
            Assert.IsTrue(txFound);

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
