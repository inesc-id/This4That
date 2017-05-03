using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration;

namespace This4That_library.Models.Integration.ReportDTO
{
    [Serializable]
    public class SensingTaskReportDTO : ReportDTO
    {
        private SensingTaskResult result;

        [JsonProperty(PropertyName = "result", Required = Required.Always)]
        public SensingTaskResult Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    [Serializable]
    public class SensingTaskResult
    {
        private SensorType sensor;
        private object data;

        [JsonProperty(PropertyName = "sensor", Required = Required.Always)]
        public SensorType Sensor
        {
            get
            {
                return sensor;
            }

            set
            {
                sensor = value;
            }
        }

        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

    }
}
