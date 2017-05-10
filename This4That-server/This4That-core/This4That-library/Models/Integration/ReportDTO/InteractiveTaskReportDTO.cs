using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Integration.ReportDTO
{
    [Serializable]
    public class InteractiveTaskReportDTO : ReportDTO
    {
        private InteractiveTaskResult result;

        [JsonProperty(PropertyName = "result", Required = Required.Always)]
        public InteractiveTaskResult Result
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
    public class InteractiveTaskResult
    {
        private string answerId;

        [JsonProperty(PropertyName = "answerId", Required = Required.Always)]
        public string AnswerId
        {
            get
            {
                return answerId;
            }

            set
            {
                answerId = value;
            }
        }
    }
}
