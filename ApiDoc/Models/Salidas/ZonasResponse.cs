using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ZonasResponse : ResponseBase
    {
        public IEnumerable<ZonasJson> Data { get; set; }
    }
    public class ZonasJson
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}