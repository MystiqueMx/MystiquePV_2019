using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models
{
    public class ReporteVentasViewModel
    {
        public int cveVenta { get; set; }
        public Nullable<int> semana { get; set; }
        public Nullable<int> numero { get; set; }
        public string dia { get; set; }
        public Nullable<int> comensales { get; set; }
        public Nullable<decimal> ventasTotales { get; set; }
        public System.DateTime fechaInicial { get; set; }
        public Nullable<System.DateTime> fechaFinal { get; set; }
    }

    public class ReporteVentasxHoraViewModel
    {
        public int Hora { get; set; }
        public int Domingo { get; set; }
        public int Lunes { get; set; }
        public int Martes { get; set; }
        public int Miercoles { get; set; }
        public int Jueves { get; set; }
        public int Viernes { get; set; }
        public int Sabado { get; set; }
        public decimal Promedio { get; set; }
    }

    public class ReporteVentasxHoraViewModel2
    {
        public string diaSemana { get; set; }
        public string fechaHora { get; set; }
        public Nullable<decimal> importe { get; set; }
        public Nullable<decimal> tickets { get; set; }
    }

}