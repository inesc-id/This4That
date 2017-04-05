using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class Topic
    {
        private string name;
        private string channelKey;

        public Topic(string topicName, string channelKey)
        {
            this.Name = topicName;
            this.ChannelKey = channelKey;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string ChannelKey
        {
            get
            {
                return channelKey;
            }

            set
            {
                channelKey = value;
            }
        }
    }
}
