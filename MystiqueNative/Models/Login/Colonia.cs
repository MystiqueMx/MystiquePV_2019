using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class Colonia
    {
        [JsonProperty("coloniaId")]
        public string Id { get; set; }

        [JsonProperty("descripcionColonia")]
        public string Descripcion { get; set; }

    }
}
