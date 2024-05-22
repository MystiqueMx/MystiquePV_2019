using FacturacionTDCAPI.Models.Requests.Restaurante;
using FacturacionTDCAPI.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models.Responses.Receptores
{
    public class ReceptorResponse : ResponseBase
    {
        public ReceptorFactura Receptor { get; set; }
    }

    public class ReceptorSearchResponse : ResponseBase
    {
        public Dictionary<string, string> Receptores { get; set; }
    }
}