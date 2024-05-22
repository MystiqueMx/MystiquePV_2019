using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseUsuario : ResponseBase
    {
        public bool registroCompleto { get; set; }
    }

    public class AseguranzasResponse : ResponseBase
    {
        public IEnumerable<CustomSelectItemAseguranza> Data { get; set; }
    }
    public class CustomSelectItemAseguranza
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string  Ruta { get; set; }
    }
}