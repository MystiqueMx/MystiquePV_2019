using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionApi.Models.Responses
{
    public class SolicitudFacturaResponse : ErrorCodeResponseBase
    {
        public Guid Id { get; set; }
        public EstatusFactura Estatus { get; set; }
        public string FacturaTimbrada { get; set; }
        public string CadenaOriginal { get; set; }
    }
}