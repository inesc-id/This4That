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
    public class ReportTests
    {
        [TestMethod]
        public void ReportTaskResults()
        {
            string response = null;
            string taskId = null;
            string answerId;
            string jsonBody;
            GetMyTasksDTO responseGetMyTasksDTO;
            ReportTaskDTO responseReportTaskDTO;

            //check for task created
            response = Library.MakeHttpGETRequest(Library.GET_USER_TASKS_API_URL);
            responseGetMyTasksDTO = JsonConvert.DeserializeObject<GetMyTasksDTO>(response);

            taskId = responseGetMyTasksDTO.Response.First().TaskID;
            answerId = responseGetMyTasksDTO.Response.First().InteractiveTask.Answers.First().AnswerId;

            jsonBody = Library.ReadStringFromFile(Library.VALID_REPORT_TASK__RESULT_PATH);
            jsonBody = jsonBody.Replace("##taskId_replace##", taskId);
            jsonBody = jsonBody.Replace("##answerId_replace##", answerId);

            //report task result
            response = Library.MakeHttpPOSTJSONRequest(Library.REPORT_INTERACTIVE_TASK_RESULT_API_URL, jsonBody);
            responseReportTaskDTO = JsonConvert.DeserializeObject<ReportTaskDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseReportTaskDTO.ErrorCode);
            Assert.IsNotNull(responseReportTaskDTO.Response.Reward);
            Assert.IsNotNull(responseReportTaskDTO.Response.TxId);

        }

    }
}
