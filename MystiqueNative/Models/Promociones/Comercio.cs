using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class Comercio
    {
        [JsonProperty("idComercio")]
        public string Id { get; set; }
        [JsonProperty("empresaId")]
        public string IdEmpresa { get; set; }

        [JsonProperty("nombreComercial")]
        public string Nombre { get; set; }

        [JsonProperty("logoUrl")]
        public string LogotipoUrl { get; set; }

    }
}
