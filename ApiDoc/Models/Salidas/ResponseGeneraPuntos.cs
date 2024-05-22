using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseGeneraPuntos : ResponseBase
    {

    }

    public class ResponseEstadoCuentaPuntos : ResponseBase
    {
        public decimal puntosActuales { get; set; }
        public decimal puntosAnteriores { get; set; }
        public int? visitasActuales { get; set; }
        public int? visitasAnteriores { get; set; }
    }


    public class ResponseListadoMovimientos : ResponseBase
    {
        public List<SP_ObtenerListadoMovimientosPuntos_Result> listadoMovimientos { get; set; }

    }


    public class ResponseListadoMovimientosV2 : ResponseBase
    {
        public List<SP_ObtenerListaMovimientosPuntos_Result> listadoMovimientos { get; set; }

    }
}