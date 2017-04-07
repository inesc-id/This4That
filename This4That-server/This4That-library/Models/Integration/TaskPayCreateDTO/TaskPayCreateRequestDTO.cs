using Newtonsoft.Json;
using This4That_library.Models.Domain;

namespace This4That_library.Models.Integration.TaskPayCreateDTO
{
    public class TaskPayCreateRequestDTO : APIRequestDTO
    {
        private CSTask task;

        [JsonProperty(PropertyName = "task", Required = Required.Always)]
        public CSTask Task
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
