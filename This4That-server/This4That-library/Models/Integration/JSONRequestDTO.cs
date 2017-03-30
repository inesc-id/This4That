using Newtonsoft.Json;

namespace This4That_library.Models.Integration
{
    public abstract class JSONRequestDTO
    {
        private string userID;
        private string transactionID;

        [JsonProperty(PropertyName = "userId")]
        public string UserID
        {
            get
            {
                return userID;
            }

            set
            {
                userID = value;
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
