using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models
{
    public class ReporteRecetaConteoManual
    {
        public string Anio { get; set; }
        public int Semana { get; set; }
        public int SucursalId { get; set; }
        public DateTime? FechaInicialSemana { get; set; }
        public DateTime? FechaFinalSemana { get; set; }
        public string UnidadMedida { get; set; }
        public int IdInsumo { get; set; }
        public string Nombre { get; set; }
        public decimal? TotalProductosVenta { get; set; }
        public decimal? TotalInventarioManual { get; set; }
        public decimal? Diferencia { get; set; }
    }

    public class ReporteRecetaConteoManualDetalle
    {
        public string Anio { get; set; }
        public int Semana { get; set; }
        public string DiaSemana { get; set; }
        public DateTime? Fecha { get; set; }
        public string UnidadMedida { get; set; }
        public int IdInsumo { get; set; }
        public string Nombre { get; set; }
        public decimal TotalProductosVenta { get; set; }
        public decimal TotalInventarioManual { get; set; }
        public decimal Diferencia { get; set; }
    }
}