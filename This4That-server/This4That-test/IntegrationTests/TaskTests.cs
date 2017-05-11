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
        //endpoints URL
        private const string API_URL = @"http://localhost:58949/api/v1/";
        private const string CALC_TASK_API_URL = API_URL  + "task/cost";
        private const string CREATE_TASK_API_URL = API_URL + "task";

        //integration files path
        private const string INTEGRATION_FILES_PATH = @"C:\Users\Calado\Documents\Documentos Word\Faculdade\IST 5ano\Tese\Dissertation\This4That\This4That-server\This4That-test\IntegrationFiles\";
        private const string VALID_CALC_TASK_COST_PATH = INTEGRATION_FILES_PATH + @"\CalcTaskCost\valid.json";
        private const string INVALID_CALC_TASK_COST_PATH = INTEGRATION_FILES_PATH + @"\CalcTaskCost\invalid.json";

        //errorCode
        private const int ERROR_CODE_SUCCESS = 1;
        private const int ERROR_CODE_FAIL = -1;

        [TestMethod]
        public void CalculateVALIDTaskCost()
        {
            string jsonBody = null;
            string response = null;
            CalcTaskCostResponseDTO responseDTO;

            jsonBody = Library.ReadStringFromFile(VALID_CALC_TASK_COST_PATH);

            response = Library.MakeHttpPOSTJSONRequest(CALC_TASK_API_URL, jsonBody);
            responseDTO = JsonConvert.DeserializeObject<CalcTaskCostResponseDTO>(response);

            Assert.AreEqual(responseDTO.ErrorCode, ERROR_CODE_SUCCESS);
            Assert.IsNotNull(responseDTO.Response.ValToPay);
        }

        [TestMethod]
        public void CalculateINVALIDTaskCost()
        {
            string jsonBody = null;
            string response = null;
            CalcTaskCostResponseDTO responseDTO;

            jsonBody = Library.ReadStringFromFile(INVALID_CALC_TASK_COST_PATH);

            response = Library.MakeHttpPOSTJSONRequest(CALC_TASK_API_URL, jsonBody);
            responseDTO = JsonConvert.DeserializeObject<CalcTaskCostResponseDTO>(response);

            Assert.AreEqual(responseDTO.ErrorCode, ERROR_CODE_FAIL);
            Assert.IsNull(responseDTO.Response);

        }

        [TestMethod]
        public void CreateVALIDTask_CheckTransaction_Centralized()
        {
            string jsonBody = null;
            string response = null;
            CreateTaskResponseDTO responseDTO;

            jsonBody = Library.ReadStringFromFile(VALID_CALC_TASK_COST_PATH);

            response = Library.MakeHttpPOSTJSONRequest(CREATE_TASK_API_URL, jsonBody);

            responseDTO = JsonConvert.DeserializeObject<CreateTaskResponseDTO>(response);

            Assert.AreEqual(responseDTO.ErrorCode, ERROR_CODE_SUCCESS);
            Assert.IsNotNull(responseDTO.Response.TaskID);
            Assert.IsNotNull(responseDTO.Response.TransactionID);


        }

        [TestMethod]
        public void CreateINVALIDTask()
        {
            throw new Exception();
        }
    }
}
