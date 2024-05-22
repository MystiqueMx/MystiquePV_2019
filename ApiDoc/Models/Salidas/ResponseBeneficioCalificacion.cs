using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseBeneficioCalificacion : ResponseBase
    {
        public Nullable<int> cantidadCalificados { get; set; }
        public Nullable<decimal> calificacionPromedio { get; set; }
        public string calificadoPorCliente { get; set; }
    }

    public class ResponseBeneficioCalificacionInsertar : ResponseBase
    {

    }
}