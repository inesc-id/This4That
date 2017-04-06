using Newtonsoft.Json;
using This4That_library.Models.Domain;

namespace This4That_library.Models.Integration.TaskPayDTO
{
    public class TaskPayResponseDTO
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
