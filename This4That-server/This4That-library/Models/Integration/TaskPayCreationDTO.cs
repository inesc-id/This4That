using Newtonsoft.Json;
using This4That_library.Models.Domain;

namespace This4That_library.Models.Integration
{
    public class TaskPayCreationDTO : JSONRequestDTO
    {
        private CSTask task;
        private string refToPay;

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

        [JsonProperty(PropertyName = "refToPay")]
        public string RefToPay
        {
            get
            {
                return refToPay;
            }

            set
            {
                refToPay = value;
            }
        }
    }
}
