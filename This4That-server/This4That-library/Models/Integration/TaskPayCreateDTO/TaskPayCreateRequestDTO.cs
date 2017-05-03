using Newtonsoft.Json;
using This4That_library.Models.Integration;

namespace This4That_library.Models.Integration.TaskPayCreateDTO
{
    public class TaskPayCreateRequestDTO : APIRequestDTO
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
