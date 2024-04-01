using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MystiqueNative.Models.Ensaladas;

namespace MystiqueNative.Models.Carrito
{
    public class EnsaladaCarrito : ItemCarrito
    {
        [JsonProperty("presentacion")]
        public PresentacionEnsalada Presentacion { get; set; }

        [JsonProperty("ingredientes")]
        public Dictionary<int, int> Ingredientes { get; set; }

        [JsonProperty("extras")]
        public Dictionary<int, int> Extras { get; set; }
    }

    public class PlatilloCarrito : ItemCarrito
    {
        [JsonProperty("nivelUno")]
        public List<int> Nivel1 { get; set; }

        [JsonProperty("nivelDos")]
        public List<int> Nivel2 { get; set; }

        [JsonProperty("nivelTres")]
        public List<int> Nivel3 { get; set; }
    }

    public class ItemCarrito
    {
        [JsonProperty("platilloId")]
        public int Id { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        [JsonProperty("descripcion")]
        public string Nombre { get; set; }

        [JsonProperty("contenido")]
        public string Contenido { get; set; }

        [JsonProperty("notas")]
        public string Notas { get; set; }

        [JsonIgnore]
        public string Imagen { get; set; }

        [JsonIgnore]
        public Guid Hash { get; set; }

        [JsonIgnore]
        public TipoPlatillos TipoItem { get; set; }
    }

    public enum TipoPlatillos
    {
        NoDefinido = 0,
        Ensalada = 1,
        Platillo = 2,
    }
}
