using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class MovimientoCitypointsContainer : BaseModel
    {
        [JsonProperty("listadoMovimientos")]
        public List<MovimientoCitypoints> ListaMovimientos { get; set; }
    }
}