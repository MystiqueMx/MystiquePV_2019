using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Helpers.OpenPay.Modelos
{
    public class BaseRespuestaOpenPay
    {
        public bool resultado { get; set; }
        public string datosEnvio { get; set; }
        public string datosRespuesta { get; set; }
        public int codigoError { get; set; }
        public string descripcionError { get; set; }
    }
}