using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models
{
    public class ReporteVentascs
    {
        public string cveVenta { get; set; }
        public string semana { get; set; }
        public string numero { get; set; }
        public string dia { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int ventasTotales { get; set; }
        public string comensales { get; set; }
    }
}