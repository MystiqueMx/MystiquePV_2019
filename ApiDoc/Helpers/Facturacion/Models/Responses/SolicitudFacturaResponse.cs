using System;
using FacturacionApi.Models;

namespace ApiDoc.Helpers.Facturacion.Models.Responses
{
    public class SolicitudFacturaResponse : BaseFacturacionResponse
    {
        public Guid Id { get; set; }
        public EstatusFactura Estatus { get; set; }
        public string FacturaTimbrada { get; set; }
        public string CadenaOriginal { get; set; }
    }
}