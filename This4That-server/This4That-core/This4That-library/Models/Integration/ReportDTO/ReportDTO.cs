using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Integration.ReportDTO
{
    [Serializable]
    public abstract class ReportDTO : APIRequestDTO
    {
        private string taskId;       
        private DateTime timestamp;
        private string reportId;      

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

        [JsonProperty(PropertyName = "timestamp", Required = Required.Always)]
        public DateTime Timestamp
        {
            get
            {
                return timestamp;
            }

            set
            {
                timestamp = value;
            }
        }

        public string ReportID
        {
            get
            {
                return reportId;
            }

            set
            {
                reportId = value;
            }
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskTypeEnum
    {
        SensingTask,
        InteractiveTask
    }
}
