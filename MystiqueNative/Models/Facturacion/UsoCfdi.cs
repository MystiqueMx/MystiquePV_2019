using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models
{
    public class UsoCfdi
    {
        [JsonProperty("idCatUsoCFDI")]
        public string Id { get; set; }
        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }
    }
}
