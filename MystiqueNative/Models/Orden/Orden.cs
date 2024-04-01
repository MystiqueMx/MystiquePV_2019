using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Orden
{
    public class OrdenesContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public List<Orden> Respuesta { get; set; }
    }

    public class Orden
    {
        [JsonProperty("nombreSucursal")]
        public string Restaurante { get; set; }

        [JsonProperty("pedidoId")]
        public int Id { get; set; }

        [JsonProperty("fecha")]
        public DateTime Fecha { get; set; }

        [JsonProperty("montoCompra")]
        public decimal Total { get; set; }

        [JsonProperty("estatus")]
        public string DescripcionEstatus { get; set; }

        [JsonProperty("estatusId")]
        public EstatusOrden Estatus { get; set; }

        [JsonProperty("metodoPagoId")]
        public MetodoPago FormaPago { get; set; }

        [JsonProperty("bitacoraPedido")]
        public List<SeguimientoPedido> Seguimientos { get; set; }

        [JsonProperty("calificacionPedido")]
        public int? Calificacion { get; set; }

        [JsonProperty("total")]
        public decimal _total
        {
            get => Total;
            set => Total = value;
        }

        [JsonProperty("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonProperty("costoEnvio")]
        public decimal CostoDeEnvio { get; set; }

        [JsonProperty("detallePedido")]
        public List<DetallePedido> Detalle { get; set; }

        public string Folio => Id.ToString("D7");

        public string FechaConFormatoEspanyol => Fecha.ToString("dd/MM/yyyy");
    }

    public enum EstatusOrden
    {
        Cancelada = 0,
        EstatusUno = 1,
        EstatusDos = 2,
        EstatusTres = 3,
        EstatusCuatro = 4,
    }
}
