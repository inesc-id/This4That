using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test.IntegrationDTOs
{
    public class GetTasksByTopicNameDTO
    {
        private int errorCode;
        private string errorMessage;
        private TasksResponseDTO response;

        public int ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                errorCode = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }

        public TasksResponseDTO Response
        {
            get
            {
                return response;
            }

            set
            {
                response = value;
            }
        }
    }

    public class TasksResponseDTO
    {
        [JsonProperty(PropertyName = "tasks")]
        private List<ResponseTaskDTO> tasks;

        public List<ResponseTaskDTO> TasksList
        {
            get
            {
                return tasks;
            }

            set
            {
                tasks = value;
            }
        }
    }

    public class ResponseTaskDTO
    {
        [JsonProperty(PropertyName = "taskName")]
        private string taskName;

        [JsonProperty(PropertyName = "taskID")]
        private string taskID;

        public string TaskName
        {
            get
            {
                return taskName;
            }

            set
            {
                taskName = value;
            }
        }

        public string TaskID
        {
            get
            {
                return taskID;
            }

            set
            {
                taskID = value;
            }
        }
    }
}
