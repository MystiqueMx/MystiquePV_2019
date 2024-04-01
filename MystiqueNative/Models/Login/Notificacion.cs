using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models
{
    public class Notificacion
    {
        [JsonProperty("notificacionId")]
        public string Id { get; set; }
        [JsonProperty("titulo")]
        public string Titulo { get; set; }
        [JsonProperty("descripcion")]
        public string Contenido { get; set; }
        [JsonProperty("fechaEnviado")]
        public string FechaNotificacionAsString { get; set; }
        [JsonProperty("isBeneficio")]
        public bool EsBeneficio { get; set; }
        [JsonProperty("activo")]
        public bool Leido { get; set; }
        [JsonProperty("usuarioRegistro")]
        public string UsuarioId { get; set; }
        [JsonProperty("clienteId")]
        public string ClienteId { get; set; }

    }

    /** HAZ PEDIDO **/
    public class Notificacion_HP
    {
        public object cell;

        [JsonProperty("notificacionId")]
        public int Id { get; set; }

        [JsonProperty("titulo")]
        public string Titulo { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [JsonProperty("pedidoId")]
        public int? IdPedido { get; set; }

        [JsonProperty("fechaPedido")]
        public DateTime? FechaPedido { get; set; }

        [JsonProperty("montoPedido")]
        public decimal? TotalPedido { get; set; }

        [JsonProperty("consumidorId")]
        public int IdConsumidor { get; set; }

        public string FolioPedido => IdPedido?.ToString("D7") ?? string.Empty;

        public string FechaConFormatoEspanyol => FechaRegistro.ToString("dd/MM/yyyy") ?? string.Empty;
    }

    public class NotificacionesContainer_HP : Helpers.BaseContainer
    {
        [JsonProperty("respuesta")]
        public List<Notificacion_HP> Resultados { get; set; }

    }
}
