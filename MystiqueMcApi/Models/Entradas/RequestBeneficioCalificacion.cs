using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestBeneficioCalificacion : RequestBase
    {
        public int beneficioId { get; set; }
        public int clienteId { get; set; }
    }


    public class RequestBeneficioCalificacionInsertar : RequestBase
    {
        public int beneficioId { get; set; }
        public int clienteId { get; set; }
        public int calificacion { get; set; }
    }
}