using Newtonsoft.Json;
using This4That_library.Models.Integration;

namespace This4That_library.Models.Integration.CalcTaskCostDTO
{
    public class CalcTaskCostRequestDTO : APIRequestDTO
    {
        private CSTaskDTO task;

        [JsonProperty(PropertyName = "task", Required = Required.Always)]
        public CSTaskDTO Task
        {
            get
            {
                return task;
            }

            set
            {
                task = value;
            }
        }
    }
}
