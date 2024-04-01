using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Orden
{
    public class RespuestaDetalle
    {
        [JsonProperty("informacion")]
        public Orden Detalle { get; set; }

        [JsonProperty("bitacoraPedido")]
        public SeguimientoPedido[] Seguimientos { get; set; }
    }
}
