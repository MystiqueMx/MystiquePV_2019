using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseDomicilio : ErrorObjCodeResponseBase
    {
        public List<ResponseListaDomicilio> respuesta { get; set; }
    }

    public class ResponseListaDomicilio
    {
        public int direccionId { get; set; }
        public string calle { get; set; }
        public string numeroInt { get; set; }
        public string numeroExt { get; set; }
        public int? coloniaId { get; set; }
        public string nombreColonia { get; set; }
        public string codigoPostal { get; set; }
        public int estadoId { get; set; }
        public string nombreEstado { get; set; }
        public string codigoPais { get; set; }
        public string nombrePais { get; set; }
        public string referencias { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public string alias { get; set; }

    }

    public class ResponseColoniaHazPedido : ErrorObjCodeResponseBase
    {
        public List<SP_Obtener_Colonias_Result> respuesta { get; set; }
    }

    public class ResponseListaColonia
    {
        public int? coloniaId { get; set; }
        public string nombreColonia { get; set; }
    }
}