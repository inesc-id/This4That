using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace This4That_platform.Domain
{
    public class APIResponse
    {
        private int errorCode = -1;
        private object response = null;

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
            if (type == RESULT_TYPE.ERROR)
                Global.Log.Error(response.ToString());
        }
    }
}