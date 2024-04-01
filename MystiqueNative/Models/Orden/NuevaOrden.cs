using MystiqueNative.Models.Location;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MystiqueNative.Models.Orden
{
    public class NuevaOrden
    {
        [JsonProperty("consumidorId")]
        public int IdConsumidor { get; set; }

        [JsonProperty("pedido")]
        public Carrito.Carrito Pedido { get; set; }

        [JsonProperty("formaPago")]
        public FormaPago FormaPago = new FormaPago();

        [JsonProperty("direccionEntrega")]
        public DireccionOrden DireccionEntrega = new DireccionOrden();

        [JsonProperty("clienteRecibe")]
        public Clientes.ClienteCallCenter cliente = new Clientes.ClienteCallCenter();

        public bool solicitudPorAgente { get; set; }
    }

    public class FormaPago
    {
        [JsonProperty("metodo")]
        public MetodoPago Metodo = MetodoPago.NoDefinido;

        [JsonProperty("idTarjeta")]
        public string IdTarjeta { get; set; }

        [JsonProperty("idSesion")]
        public string IdSesion { get; set; }

        [JsonProperty("metodoDescripcion")]
        public string MetodoDescripcion => Metodo.ToString();
    }

    public class DireccionOrden
    {
        [JsonProperty("ubicacion")]
        public Point Ubicacion;

        [JsonProperty("direccion")]
        public Direction Direccion;
    }

    public enum MetodoPago
    {
        NoDefinido = 0,
        [Description("Pago con tarjeta de credito o debito")]
        Tarjeta = 1,
        [Description("Pago en efectivo")]
        Efectivo = 2,
    }

    public enum TipoReparto
    {
        NoDefinido = 0,
        [Description("Recoger en sucursales")]
        Sucursal = 1,
        [Description("Reparto a domicilio particular")]
        Particular = 2,
    }
}
