using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Integration.ExecuteTaskDTO
{
    public class ExecuteTaskDTO : APIRequestDTO
    {
        private string taskId;

        [JsonProperty(PropertyName = "taskId", Required = Required.Always)]
        public string TaskId
        {
            get
            {
                return taskId;
            }

            set
            {
                taskId = value;
            }
        }
    }
}
