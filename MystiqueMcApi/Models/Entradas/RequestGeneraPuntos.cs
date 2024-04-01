using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MystiqueMC.DAL;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestGeneraPuntos : RequestBase
    {
        public int membresiaId { get; set; }
        public string codigoGenerado { get; set; }
        public int comercioId { get; set; }

    }

    public class RequestListadoPuntos : RequestBase
    {
        public int membresiaId { get; set; }
        public int comercioId { get; set; }

    }

  

}