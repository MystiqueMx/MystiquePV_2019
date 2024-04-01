using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestMembresia : RequestBase
    {
        public int membresiaId { get; set; }
        public int? tipoMembresiaId { get; set; }
        public int? clienteId { get; set; }
        public string folio { get; set; }
        public DateTime? fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public bool? estatus { get; set; }
        public string GUID { get; set; }
        public DateTime? fechaVinculo { get; set; }
        public bool isCompraDigital { get; set; }
        public decimal? costo { get; set; }
        public string codigoQR { get; set; }
        public string codigoBarra { get; set; }

    }

}