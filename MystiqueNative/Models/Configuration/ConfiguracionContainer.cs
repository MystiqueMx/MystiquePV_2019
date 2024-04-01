using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class ConfiguracionContainer : BaseModel
    {
        [JsonProperty("listaComercios")]
        public List<Comercio> Comercios { get; set; }
        [JsonProperty("configuraciones")]
        public Configuracion Configuracion { get; set; }
    }
}