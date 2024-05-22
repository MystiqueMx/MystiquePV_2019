using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiDoc.Controllers;

public class AgregarTicketsResponse : ErrorCodeResponseBase
{
    public int ResponseCode { get; set; }
    public string Message { get; set; }
    public List<string>FoliosInsertados{ get; set; }
    public List<string>FoliosFallidos{ get; set; }
}
