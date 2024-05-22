using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FacturacionTDCAPI.Models.Requests.Respuesta
{
    public class Respuesta
    {
        public int opok { get; set; }
        public List<string> mensaje { get; set; }
        public List<string> Error { get; set; }
        public string objeto { get; set; }
        public int? solicitud { get; set; }
    }
}