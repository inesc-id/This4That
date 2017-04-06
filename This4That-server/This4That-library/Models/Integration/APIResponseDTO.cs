using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace This4That_platform.Integration
{
    public class APIResponseDTO
    {
        private int errorCode = -1;
        private object response = null;

        [JsonProperty(PropertyName = "errorCode")]
        public int ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                errorCode = value;
            }
        }

        [JsonProperty(PropertyName = "response")]
        public object Response
        {
            get
            {
                return response;
            }

            set
            {
                response = value;
            }
        }

        public enum RESULT_TYPE{
            SUCCESS = 1,
            ERROR = -1
        }
        
        public void SetResponse(object response, RESULT_TYPE type)
        {
            this.Response = response;
            ErrorCode = (int)type;
        }
    }
}