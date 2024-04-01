using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models
{
    public class ComerciosGiro
    {
        [JsonProperty("ComercioId")]
        public string Id { get; set; }

        [JsonProperty("NombreComercial")]
        public string Nombre { get; set; }

        [JsonProperty("urlLogoComercio")]
        public string Logo { get; set; }
    }
}
