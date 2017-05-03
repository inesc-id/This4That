using Newtonsoft.Json;
using System;

namespace This4That_library.Models.Integration
{
    [Serializable]
    public abstract class APIRequestDTO
    {
        private string userID;

        [JsonProperty(PropertyName = "userId", Required = Required.Always)]
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
