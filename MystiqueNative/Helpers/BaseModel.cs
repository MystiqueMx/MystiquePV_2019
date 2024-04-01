using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Helpers
{
    public class BaseContainer
    {
        [JsonProperty("estatusPeticion")]
        public BaseModel Estatus { get; set; }
    }

    public class BaseModel
    {
        [JsonProperty("ResponseCode")]
        public ResponseTypes ResponseCode { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        public bool IsSuccessful => (ResponseTypes)ResponseCode == ResponseTypes.CodigoOk;
    }

    public enum ResponseTypes
    {
        CodigoNoConexion = 0,
        CodigoExcepcion = 1000,
        CodigoOk = 2000,
        CodigoAutentificacion = 3000,
        CodigoPermisos = 4000,
        CodigoValidacion = 5000,
        CodigoOtros = 6000,
        CodigoInterno = 7000,
    }
}
