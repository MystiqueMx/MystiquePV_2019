using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseSucursal
    {
        public int SucursalId { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string DireccionColonia { get; set; }
        public string DireccionCalle { get; set; }
    }

    public class ResponseListaSucursal : ResponseBase
    {

        public List<SP_Obtener_Sucursal_Comercio_Result> ListSucursalPorComercio { get; set; }
    }

}