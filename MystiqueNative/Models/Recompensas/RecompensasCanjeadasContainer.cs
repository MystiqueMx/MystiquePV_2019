using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class RecompensasCanjeadasContainer : BaseModel
    {
        [JsonProperty("listadoCupones")]
        public List<RecompensaCanjeada> RecompensasActivas { get; set; }
    }
}