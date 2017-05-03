using Newtonsoft.Json;
using This4That_library.Models.Integration;

namespace This4That_library.Models.Integration.TaskPayCreateDTO
{
    public class TaskPayCreateResponseDTO
    {
        private string taskID;
        private string transactionID;

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

        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionID
        {
            get
            {
                return transactionID;
            }

            set
            {
                transactionID = value;
            }
        }


    }
}
