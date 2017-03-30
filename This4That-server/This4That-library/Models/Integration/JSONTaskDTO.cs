using Newtonsoft.Json;
using This4That_library.Models.Domain;

namespace This4That_library.Models.Integration
{
    public class JSONTaskDTO : JSONRequestDTO
    {
        private CSTask task;

        [JsonProperty(PropertyName = "task")]
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
