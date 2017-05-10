using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Integration.GetUserTopicDTO
{
    public class GetTopicRequestDTO : APIRequestDTO
    {
        private string topicName;

        [JsonProperty(PropertyName = "topicName", Required = Required.Always)]
        public string TopicName
        {
            get
            {
                return topicName;
            }

            set
            {
                topicName = value;
            }
        }
    }
}
