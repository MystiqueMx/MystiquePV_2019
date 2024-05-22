using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MystiqueMcApi.Controllers;

public class ErrorCodeResponseBase
{
    public int ResponseCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}
