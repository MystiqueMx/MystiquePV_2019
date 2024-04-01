using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class FacturasContainer : BaseModel
    {
        [JsonProperty("listado")]
        public List<Factura> Facturas { get; set; }
    }
}