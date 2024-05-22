using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class RangoEdadesResponse : ResponseBase
    {
        public IEnumerable<CustomSelectItem> Data { get; set; }
    }
    public class CustomSelectItem
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}