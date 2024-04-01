using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.SyncApi.Helpers.Base;

namespace WebApp.SyncApi.Models.Responses
{
    public class ResponseVentas : ResponseBase
    {
        public ResponseEntidadesVentas respuesta { get; set; }
    }


    public class ResponseEntidadesVentas
    {
        public List<EntidadAperturas> ListadoEntidadAperturas { get; set; } = new List<EntidadAperturas>();
        public List<EntidadVentas> ListadoEntidadVentas { get; set; } = new List<EntidadVentas>();
       
    }


    public class EntidadAperturas
    {
        public int id { get; set; }
        public string uuidApertura { get; set; }
    }


    public class EntidadVentas
    {
        public int id { get; set; }
        public string uuidVenta { get; set; }
    }



}