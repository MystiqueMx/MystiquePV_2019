using System.Collections.Generic;
using Newtonsoft.Json;
using MystiqueNative.Models.Menu;
using MystiqueNative.Models.Orden;

namespace MystiqueNative.Models.Carrito
{
    public class Carrito
    {
        [JsonProperty("restaurante")]
        public Restaurante Restaurante { get; set; }

        [JsonProperty("total")]
        public decimal? Total { get; set; }

        [JsonProperty("subTotal")]
        public decimal SubTotal { get; set; }

        [JsonProperty("platillos")]
        public List<PlatilloCarrito> Platillos { get; set; }

        [JsonProperty("ensaladas")]
        public List<EnsaladaCarrito> Ensaladas { get; set; }

        [JsonProperty("tipoReparto")]
        public TipoReparto TipoReparto = TipoReparto.NoDefinido;

        [JsonProperty("tipoRepartoDescripcion")]
        public string TipoRepartoDescripcion => TipoReparto.ToString();
    }
}
