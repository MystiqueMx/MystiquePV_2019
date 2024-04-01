using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestBase
    {
        public string correoElectronico { get; set; }
        public string contrasenia { get; set; }
        public int empresaId { get; set; }
    }
}