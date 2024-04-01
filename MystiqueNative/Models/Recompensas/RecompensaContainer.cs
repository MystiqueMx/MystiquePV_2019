using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class RecompensaContainer: BaseModel
    {
        [JsonProperty("listadoRecompensas")]
        public List<Recompensa> Recompensas { get; set; }
    }
}