﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test.IntegrationDTOs
{
    public class GetTopicsDTO
    {
        private int errorCode;
        private string errorMessage;
        private List<string> response;

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

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }

        public List<string> Response
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
    }
}
