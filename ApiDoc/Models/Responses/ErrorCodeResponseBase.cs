using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Responses
{
    public class ErrorCodeResponseBase
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
    }


    public class ErrorObjCodeResponseBase
    {
        public ErrorCodeResponseBase estatusPeticion { get; set; }
    }
}