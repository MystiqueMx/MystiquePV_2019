using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models.Cancelaciones
{
    public class FacturaCancelacion
    {
        public string Uuid { get; set; }
        public string RfcReceptor { get; set; }
        public decimal Total { get; set; }
    }
}