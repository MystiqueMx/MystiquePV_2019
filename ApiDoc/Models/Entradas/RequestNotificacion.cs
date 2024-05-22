using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Entradas
{
    public class RequestNotificacion
    {
    }

    public class RequestObtenerNotificacionesCliente : RequestBase
    {
        public int idCliente { get; set; }
        public AppLanguage Idioma { get; set; }
      
    }

}