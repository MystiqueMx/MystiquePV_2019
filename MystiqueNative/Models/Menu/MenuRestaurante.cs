using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using MystiqueNative.Helpers;

namespace MystiqueNative.Models.Menu
{
    public class MenuRestaurante
    {
        [JsonProperty("menuId")]
        public int Id { get; set; }

        [JsonProperty("imagen")]
        public string ImagenUrl { get; set; }

        [JsonProperty("orden")]
        public int Orden { get; set; }

        [JsonProperty("esEnsalada")]
        public bool FlujoEnsalada { get; set; }

        [JsonProperty("descripcion")]
        public string Nombre { get; set; }
    }

    public class MenuRestauranteContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public List<MenuRestaurante> Resultados { get; set; }
    }
}
