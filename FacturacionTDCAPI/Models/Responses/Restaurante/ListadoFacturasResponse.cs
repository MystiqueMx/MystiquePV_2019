using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models.Responses.Restaurante
{
    public class ListadoFacturasResponse : ResponseBase
    {
        public FacturaReimpresion[] Facturas { get; set; }
    }

    public class FacturaReimpresion
    {
        public decimal Total { get; set; }
        public string Rfc { get; set; }
        public string RazonSocial { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public Guid? Uuid { get; set; }
        public string Emisor { get; set; }
        public string FechaTimbradoIso => FechaTimbrado?.ToString("O");
    }
}