using MystiqueMC.DAL;
using MystiqueMC.Models.Graficas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models
{
    public class ClientesViewModel
    {
        public List<VW_ObtenerClientesConMembresia> Clientes { get; set; }
        public Grafica GraficaUno { get; set; }
        public Grafica GraficaDos { get; set; }
        public Grafica GraficaTres { get; set; }
        public Resumen ResumenCompras { get; set; }
    }
    public class Resumen
    {
        public decimal TotalCompras { get; set; }
        public decimal TotalComprasEnPuntos { get; set; }
        public decimal TotalCanjes { get; set; }
        public decimal TotalCanjesEnPuntos { get; set; }
        public decimal NumeroBeneficiosUsados { get; set; }
        public decimal TotalBeneficios { get; internal set; }
    }
}