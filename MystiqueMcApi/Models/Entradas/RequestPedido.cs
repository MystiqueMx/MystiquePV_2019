using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestPedido : AuthorizedRequestBase
    {
        public RequestObjetoPedido pedido { get; set; }

        public RequestObjetoFormaPagoPedido formaPago { get; set; }

        public RequestObjetoDireccionEntrega direccionEntrega { get; set; }

        public RequestObjectoQuienRecibe clienteRecibe { get; set; }

        public bool solicitudPorAgente { get; set; }
    }

    public class RequestObjetoPedido
    {
        [Required]
        public int tipoReparto { get; set; }
        [Required]
        public decimal total { get; set; }
        [Required]
        public decimal subTotal { get; set; }
        public string tipoRepartoDescripcion { get; set; }
        [Required]
        public RequestPedidoSucursal restaurante { get; set; }
        [Required]
        public List<RequestListadoPlatillos> platillos { get; set; }
        [Required]
        public List<DatosEnsalada> ensaladas { get; set; }

    }

    public class RequestPedidoSucursal
    {
        [Required]
        public int idSucursal { get; set; }
        [Required]
        public decimal costoEnvio { get; set; }
    }

    public class RequestListadoPlatillos
    {
        public int platilloId { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public string notas { get; set; }
        public string contenido { get; set; }
        //public List<PlatilloItemUno> nivelUno { get; set; }
        //public List<PlatilloItemDos> nivelDos { get; set; }
        //public List<PlatilloItemTres> nivelTres { get; set; }
    }

    public class PlatilloItemUno
    {
        public int? id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
    }

    public class PlatilloItemDos
    {
        public int? id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
    }

    public class PlatilloItemTres
    {
        public int? id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
    }
    
    public class DatosEnsalada
    {
        public int platilloId { get; set; }
        public decimal precio { get; set; }
        public string descripcion { get; set; }
        public string contenido { get; set; }
        public int presentacion { get; set; }
        public string notas { get; set; }
        //public List<IngredienteEnsaladas> ingredientes { get; set; }
        //public List<ExtraEnsaladas> extras { get; set; }
    }

    public class ExtraEnsaladas
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
    }
    
    public class IngredienteEnsaladas
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
    }
       
    public class RequestObjetoFormaPagoPedido
    {
        public int metodo { get; set; }
        public string idTarjeta { get; set; }
        public string idSesion { get; set; }
        public string metodoDescripcion { get; set; }

    }
    
    public class RequestObjetoDireccionEntrega
    {
        public int tipoReparto { get; set; }
        public string tipoRepartoDescripcion { get; set; }
        public RequestUbicacionDireccionEntrega ubicacion { get; set; }
        public RequestDatosDireccionPedido direccion { get; set; }

    }
    
    public class RequestUbicacionDireccionEntrega
    {
        public float latitude { get; set; }
        public float longitud { get; set; }

    }
       
    public class RequestDatosDireccionPedido
    {
        public int? direccionId { get; set; }
        public int? coloniaId { get; set; }
        public string nombreColonia { get; set; }
        public string codigoPostal { get; set; }
        public string calle { get; set; }
        public string calleDos { get; set; }
        public string numeroExt { get; set; }
        public string numeroInt { get; set; }
        public string referencia { get; set; }
    }
       
    public class RequestHistorialPedido : AuthorizedRequestBase
    {

    }

    public class RequestPedidoInsertarMensaje : AuthorizedRequestBase
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int pedidoId { get; set; }
        [Required]
        [MinLength(3), MaxLength(255)]
        public string mensaje { get; set; }
    }
    
    public class RequestPedidoActivo : AuthorizedRequestBase
    {

    }
    
    public class RequestInformacionPedido : AuthorizedRequestBase
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int pedidoId { get; set; }
    }
    
    public class RequestCalificarPedido : AuthorizedRequestBase
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int pedidoId { get; set; }
        [Required]
        [Range(1, 5)]
        public int calProducto { get; set; }
        [Required]
        [Range(1, 5)]
        public int calReparticion { get; set; }
        [Required]
        [Range(1, 5)]
        public int calMovil { get; set; }
        public string comentario { get; set; }
    }

    public class RequestInformacionPedidoPuntoVenta
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int pedidoId { get; set; }
    }

    public class RequestObjectoQuienRecibe
    {
        [JsonProperty("ID")]
        public int clienteId { get; set; }

        public string nombreCompleto { get; set; }

        public string telefono { get; set; }
    }
}