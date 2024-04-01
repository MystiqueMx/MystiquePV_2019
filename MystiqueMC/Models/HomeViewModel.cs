using MystiqueMC.DAL;
using MystiqueMC.Models.Graficas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models
{
    public class HomeViewModel
    {
        public List<comercios> Comercios {get;set;} 
        public List<sucursales> Sucursales {get;set;} 
        public Grafica GraficaUno { get; set; }
        public Grafica GraficaDos { get; set; }
        public Grafica GraficaTres { get; set; }
        public Resumen ResumenCompras { get; set; }
        public empresas Empresa { get; set; }
    }
}