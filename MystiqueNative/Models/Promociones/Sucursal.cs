using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class Sucursal
    {
        [JsonProperty("idSucursal")]
        public string Id { get; set; }

        [JsonProperty("Nombre")]
        public string Nombre { get; set; }

        [JsonProperty("Telefono")]
        public string Telefono { get; set; }

        [JsonProperty("Latitud")]
        public string Latitud { get; set; }

        [JsonProperty("Longitud")]
        public string Longitud { get; set; }

        [JsonProperty("DireccionColonia")]
        public string Colonia { get; set; }

        [JsonProperty("DireccionCalle")]
        public string Calle { get; set; }

    }
}
