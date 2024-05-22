using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models.Responses.Restaurante
{
    public class SolicitudFacturaResponse : ResponseBase
    {
        public Guid Id { get; set; }
        public EstatusFactura Estatus { get; set; }
        public string FacturaTimbrada { get; set; }
        public string CadenaOriginal { get; set; }

        public string UsoCfdi { get; set; }
        public string FormaPago { get; set; }
        public string Direccion { get; set; }
        public decimal[] Impuestos { get; set; }
        public string RegimenFiscal { get; set; }
        public string CantidadLetra { get; set; }
        public string ErrorUsuario { get; set; }
    }
}