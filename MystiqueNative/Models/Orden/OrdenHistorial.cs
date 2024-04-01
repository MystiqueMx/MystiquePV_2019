using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Orden
{
    public class HistorialContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public List<OrdenHistorial> Respuesta { get; set; }
    }

    public class OrdenHistorial
    {
        [JsonProperty("nombreSucursal")]
        public string Nombre { get; set; }

        [JsonProperty("pedidoId")]
        public int Id { get; set; }

        [JsonProperty("fecha")]
        public DateTime Fecha { get; set; }

        [JsonProperty("montoCompra")]
        public decimal Total { get; set; }

        [JsonProperty("calificacionPedido")]
        public int? Calificacion { get; set; }

        public string Folio => Id.ToString("D7");

        public string FechaConFormatoEspanyol => Fecha.ToString("dd/MM/yyyy");
    }
}
