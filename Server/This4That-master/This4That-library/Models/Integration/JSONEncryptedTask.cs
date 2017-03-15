using Newtonsoft.Json;
using This4That_library.Models.Domain;

namespace This4That_library.Models.Integration
{
    public class JSONEncryptedTaskDTO : JSONTask
    {
        private string encryptedTask;

        [JsonProperty(PropertyName = "encryptedTask")]
        public string EncryptedTask
        {
            get
            {
                return encryptedTask;
            }

            set
            {
                encryptedTask = value;
            }
        }
    }
}
