using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseCatColonias
    {
        public List<ResponseColonia> colonias { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ResponseColonia
    {
        public int coloniaId { get; set; }
        public string descripcionColonia { get; set; }
    }
}