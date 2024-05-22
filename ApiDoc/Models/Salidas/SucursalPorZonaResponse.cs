using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class SucursalPorZonaResponse : ResponseBase
    {
        public IEnumerable<SucursalJson> Data { get; set; }
    }
    public class SucursalJson
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public TipoSucursales Tipo { get; set; }
        public string Especialidad { get; set; }
    }
}