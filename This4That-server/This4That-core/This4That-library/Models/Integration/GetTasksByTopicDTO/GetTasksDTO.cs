using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Integration.GetTasksByTopicDTO
{
    [Serializable]
    public class GetTasksDTO
    {
        private string taskName;
        private string taskID;

        [JsonProperty(PropertyName = "taskName")]
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

        [JsonProperty(PropertyName = "taskId")]
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
