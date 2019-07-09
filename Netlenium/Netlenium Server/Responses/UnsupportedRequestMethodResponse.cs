﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetleniumServer.Responses
{
    public class UnsupportedRequestMethodResponse : IResponse
    {
        public bool Status { get; set; }

        public int ResponseCode { get; set; }

        public string Message { get; set; }

        public UnsupportedRequestMethodResponse()
        {
            Status = false;
            ResponseCode = 400;
            Message = "The request method used is not supported for this Web Service";
        }
    }
}