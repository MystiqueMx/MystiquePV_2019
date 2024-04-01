using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestComentario
    {
    }

    public class RequestComentarioInsertar : RequestBase
    {
        public int clienteId { get; set; }
        public int tipoComentarioId { get; set; }
        public string mensaje { get; set; }
        public bool? fromComercio { get; set; }
        public bool? fromCliente { get; set; }

    }
}