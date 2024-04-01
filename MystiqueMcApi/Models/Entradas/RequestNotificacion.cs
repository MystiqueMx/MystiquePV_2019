using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestNotificacion
    {
    }

    public class RequestObtenerNotificacionesCliente : RequestBase
    {
        public int idCliente { get; set; }
      
    }

    public class RequestNotificacionHazPedido : AuthorizedRequestBase
    {

    }

}