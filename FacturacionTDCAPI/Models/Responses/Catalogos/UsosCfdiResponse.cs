using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models.Responses.Catalogos
{
    public class UsosCfdiResponse : ResponseBase
    {
        public Dictionary<int, string> Data { get; set; } = new Dictionary<int, string>();
    }
}