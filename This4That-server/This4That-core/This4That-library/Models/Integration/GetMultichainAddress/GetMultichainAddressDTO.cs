using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Integration.GetMultichainAddress
{
    public class GetMultichainAddressDTO : APIRequestDTO
    {
        private string multichainAddress;

        [JsonProperty(PropertyName = "multichainAddress")]
        public string MultichainAddress
        {
            get
            {
                return multichainAddress;
            }

            set
            {
                multichainAddress = value;
            }
        }
    }
}
