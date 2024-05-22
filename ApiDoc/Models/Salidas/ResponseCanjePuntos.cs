using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseCanjePuntos : ResponseBase
    {
        public int canjePuntoId { get; set; }
        public int productoPuntoVentaId { get; set; }
        public string nombreProducto { get; set; }
    }

    public class ResponseListadoRecompensas : ResponseBase
    {
        public List<SP_ObtenerListadoRecompensas_Result> listadoRecompensas { get; set; }
    }


    public class ResponseListaCupones : ResponseBase
    {
        public List<VW_ObtenerListadoCupones> listadoCupones { get; set; }
    }


    public class ResponseRegistrarCanjePuntos : ResponseBase
    {
        public string codigoGenerado { get; set; }
    }

    public class ResponseCanjeRecompensa : ResponseBase
    {

    }
}