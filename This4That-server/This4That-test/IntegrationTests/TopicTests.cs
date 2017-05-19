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
    public class TopicTests
    {
        private const string SUBSCRIBED = "Subscribed";
        [TestMethod]
        public void GetTopics()
        {
            string response = null;
            GetTopicsDTO responseGetTopicsDTO;

            response = Library.MakeHttpGETRequest(Library.GET_TOPICS_API_URL);
            responseGetTopicsDTO= JsonConvert.DeserializeObject<GetTopicsDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseGetTopicsDTO.ErrorCode);
            Assert.IsNotNull(responseGetTopicsDTO.Response);

        }

        [TestMethod]
        public void GetTasksByTopicName()
        {
            string jsonBody = null;
            string response = null;
            GetTasksByTopicNameDTO responseDTO;           

            jsonBody = Library.ReadStringFromFile(Library.VALID_GET_TASKS_TOPIC_PATH);

            response = Library.MakeHttpPOSTJSONRequest(Library.GET_TASKS_TOPIC_NAME_API_URL, jsonBody);
            responseDTO = JsonConvert.DeserializeObject<GetTasksByTopicNameDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseDTO.ErrorCode);
            Assert.IsNotNull(responseDTO.Response.TasksList);
            foreach (ResponseTaskDTO task in responseDTO.Response.TasksList)
            {
                Assert.IsNotNull(task.TaskID);
                Assert.IsNotNull(task.TaskName);
            }
        }

        [TestMethod]
        public void SubscribeVALIDTopic()
        {
            string jsonBody = null;
            string response = null;
            SubscribeDTO responseDTO;

            jsonBody = Library.ReadStringFromFile(Library.VALID_GET_TASKS_TOPIC_PATH);

            response = Library.MakeHttpPOSTJSONRequest(Library.SUBSCRIBE_TOPIC_API_URL, jsonBody);
            responseDTO = JsonConvert.DeserializeObject<SubscribeDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_SUCCESS, responseDTO.ErrorCode);
            Assert.AreEqual(SUBSCRIBED, responseDTO.Response);

        }

        [TestMethod]
        public void SubscribeINVALIDTopic()
        {
            string jsonBody = null;
            string response = null;
            SubscribeDTO responseDTO;

            jsonBody = Library.ReadStringFromFile(Library.INVALID_GET_TASKS_TOPIC_PATH);

            response = Library.MakeHttpPOSTJSONRequest(Library.SUBSCRIBE_TOPIC_API_URL, jsonBody);
            responseDTO = JsonConvert.DeserializeObject<SubscribeDTO>(response);

            Assert.AreEqual(Library.ERROR_CODE_FAIL, responseDTO.ErrorCode);
            Assert.IsNull(responseDTO.Response);
        }
    }
}
