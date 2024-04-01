using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Orden
{
    public class DetallePedido
    {
        [JsonProperty("platilloId")]
        public int Id { get; set; }

        [JsonProperty("nombrePlatillo")]
        public string Nombre { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        [JsonProperty("urlImagen")]
        public string Imagen { get; set; }
        
    }
}
