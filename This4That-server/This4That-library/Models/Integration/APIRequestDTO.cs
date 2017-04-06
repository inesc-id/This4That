using Newtonsoft.Json;

namespace This4That_library.Models.Integration
{
    public abstract class APIRequestDTO
    {
        private string userID;

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

    }
}
