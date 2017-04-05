using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace This4That_platform.Domain
{
    public class APIResponse
    {
        private int _errorCode = -1;
        private object _response = null;

        public int errorCode
        {
            get
            {
                return _errorCode;
            }

            set
            {
                _errorCode = value;
            }
        }
        public object response
        {
            get
            {
                return _response;
            }

            set
            {
                _response = value;
            }
        }

        public enum RESULT_TYPE{
            SUCCESS = 1,
            ERROR = -1
        }
        
        public void SetResponse(object response, RESULT_TYPE type)
        {
            this.response = response;
            errorCode = (int)type;
            if (type == RESULT_TYPE.ERROR)
                Global.Log.Error(response.ToString());
        }
    }
}