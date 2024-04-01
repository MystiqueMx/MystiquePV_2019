using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestComercio
    {
    }

    public class RequestObtenerComerciosCercanosUsuario : RequestBase
    {
        public int giroId { get; set; }
        public string filtro { get; set; }
    }

    public class RequestObtenerListadoComerciosPorGiro : RequestBase
    {
        public int giroId { get; set; }
    }
}