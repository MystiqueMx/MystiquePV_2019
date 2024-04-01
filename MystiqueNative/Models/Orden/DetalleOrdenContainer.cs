using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Orden
{
    public class DetalleOrdenContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public RespuestaDetalle Respuesta { get; set; }
    }
}
