using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test.IntegrationDTOs
{
    public class ProxyDTO
    {
        [JsonProperty(PropertyName = "ipPort")]
        private string ipAddressPort;

        [JsonProperty(PropertyName = "ip")]
        private string ipAddress;

        [JsonProperty(PropertyName = "port")]
        private string port;

        [JsonProperty(PropertyName = "country")]
        private string country;

        private TimeSpan execTime;

        public string IpAddressPort
        {
            get
            {
                return ipAddressPort;
            }

            set
            {
                ipAddressPort = value;
            }
        }

        public string IpAddress
        {
            get
            {
                return ipAddress;
            }

            set
            {
                ipAddress = value;
            }
        }

        public string Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }

        public TimeSpan ExecTime
        {
            get
            {
                return execTime;
            }

            set
            {
                execTime = value;
            }
        }
    }
}
