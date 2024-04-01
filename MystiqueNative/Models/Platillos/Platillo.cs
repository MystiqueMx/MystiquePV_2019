using MystiqueNative.Helpers;
using MystiqueNative.Models.Menu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Platillos
{
    public class PlatilloContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public ListaPlatillo Resultados { get; set; }
    }

    public class ListaPlatillo
    {
        [JsonProperty("listaPlatillos")]
        public List<Platillo> Platillos { get; set; }
    }

    public class Platillo
    {
        [JsonProperty("idPlatillo")]
        public int Id { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("desPlatillo")]
        public string Descripcion { get; set; }

        [JsonProperty("cantidad")]
        public int ConfiguracionCantidad { get; set; }

        [JsonProperty("urlImagen")]
        public string ImagenUrl { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        [JsonProperty("orden")]
        public int? Orden { get; set; }

        [JsonProperty("datosSucursal")]
        public Restaurante Restaurante { get; set; }

        [JsonProperty("menuId")]
        public int? IdMenu { get; set; }

        [JsonProperty("nombresRestaurante")]
        public string NombreRestaurante { get; set; }

        [JsonProperty("configuracionUno")]
        public List<PlatilloPrimerNivel> Hijos { get; set; }

        public int CantidadEnCarrito { get; set; }

        public bool EsTerminal => Hijos.Count == 0;

    }

    public class PlatilloPrimerNivel : BasePlatilloMultiNivel
    {

        [JsonProperty("configuracionDos")]
        public List<PlatilloSegundoNivel> Hijos { get; set; }

        public bool EsTerminal => Hijos.Count == 0;
    }

    public class PlatilloSegundoNivel : BasePlatilloMultiNivel
    {

        [JsonProperty("configuracionTres")]
        public List<PlatilloTercerNivel> Hijos { get; set; }

        public bool EsTerminal => Hijos.Count == 0;
    }

    public class PlatilloTercerNivel : BasePlatilloMultiNivel
    {
        public bool EsTerminal => true;
    }

    public class BasePlatilloMultiNivel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        public bool Completado { get; set; }
    }

    public enum NivelesPlatillo
    {
        NoDefinido = 0,
        PrimerNivel = 1,
        SegundoNivel = 2,
        TercerNivel = 3,
    }
}
