using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Entradas
{
    public class SucursalesPorZonaFiltrosRequest : RequestBase
    {
        public int Zona { get; set; }
        public string Filtro { get; set; }
        public TipoSucursales[] Giros { get; set; }
        public string Especialidad { get; set; }
        public AppLanguage Idioma { get; set; } = AppLanguage.Spanish;

        public int? catAseguranzaId { get; set; }
    }
}