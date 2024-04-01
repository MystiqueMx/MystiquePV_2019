using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using MystiqueNative.Helpers;
using RestSharp.Extensions;

namespace MystiqueNative.Models.Menu
{
    
    public class Restaurante
    {
        [JsonProperty("idSucursal")]
        public int Id { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("logoUrl")]
        public string ImagenUrl { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        public string HoraApertura => _HoraApertura.HasValue
            ? $"{_HoraApertura.Value.Hours:D2}:{_HoraApertura.Value.Minutes:D2}"
            : string.Empty;

        public string HoraCierre => _HoraCierre.HasValue
            ? $"{_HoraCierre.Value.Hours:D2}:{_HoraCierre.Value.Minutes:D2}"
            : string.Empty;

        [JsonProperty("montoMinimo")]
        public decimal CompraMinima { get; set; }

        [JsonProperty("costoEnvio")]
        public decimal CostoEnvio { get; set; }

        [JsonProperty("abierto")]
        public bool EstaAbierto { get; set; }

        [JsonProperty("direccion")]
        public string Direccion { get; set; }
        
        [JsonProperty("activoPlataforma")]
        public bool EstaOperando { get; set; }

        [JsonProperty("tieneRepartoDomicilio")]
        public bool TieneRepartoADomicilio { get; set; }

        [JsonProperty("tieneDriveThru")]
        public bool TieneDriveThru { get; set; }

        [JsonProperty("ReparteDomicilio")]
        public bool ReparteDomicilio { set => TieneRepartoADomicilio = value; }

        [JsonProperty("horaFin")]
        public TimeSpan? _HoraCierre { get; set; }

        [JsonProperty("horaInicio")]
        public TimeSpan? _HoraApertura { get; set; }
    }

    public class ResultadoRestaurantes
    {
        [JsonProperty("listadoSucursales")]
        public List<Restaurante> Restaurantes { get; set; }
    }
    public class RestaurantesContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public ResultadoRestaurantes Resultados { get; set; }
    }
    public class DirectorioContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public DirectorioContainer2 Resultados { get; set; }
    }

    public class DirectorioContainer2
    {
        [JsonProperty("listadoSucursalesActivas")]
        public List<Restaurante> Restaurantes { get; set; }
    }
}
