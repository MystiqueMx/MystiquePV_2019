using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models.Responses.Cancelacion
{
    public class CancelacionResponse : ResponseBase
    {
        public string FolioTicket { get; set; }
    }
}