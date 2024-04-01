using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponsePedido
    {
        public int pedidoId { get; set; }
    }


    public class ResponseBasePedido : ErrorObjCodeResponseBase
    {
        public ResponsePedido respuesta { get; set; }
    }




    public class ResponseHistorialPedido : ErrorObjCodeResponseBase
    {
        public List<ResponseDatosHistorialPedido> respuesta { get; set; }
    }

    public class ResponseDatosHistorialPedido
    {
        public string nombreSucursal { get; set; }
        public int pedidoId { get; set; }
        public string folio { get; set; }
        public DateTime? fecha { get; set; }
        public decimal? montoCompra { get; set; }
        public int? calificacionPedido { get; set; }
    }



    public class ResponsePedidoActivo : ErrorObjCodeResponseBase
    {
        public List<ResponseDatosPedidoActivo> respuesta { get; set; }
    }

    public class ResponseDatosPedidoActivo
    {
        public string nombreSucursal { get; set; }
        public int pedidoId { get; set; }
        public string folio { get; set; }
        public DateTime? fecha { get; set; }
        public decimal? montoCompra { get; set; }
        public string estatus { get; set; }
        public int estatusId { get; set; }
        public int metodoPagoId { get; set; }

        public List<ResponseBitacoraPedidoActivo> bitacoraPedido { set; get; }
    }

    public class ResponseBitacoraPedidoActivo
    {
        public DateTime? fecha { get; set; }
        public string comentario { get; set; }
    }

    public class ResponseInfoPedido : ErrorObjCodeResponseBase
    {
        public ResponseListaInformacionPedido respuesta { get; set; }

    }

    public class ResponseListaInformacionPedido
    {
        public ResponseListadoInformacionPedido informacion { get; set; }

    }

    public class ResponseListadoInformacionPedido
    {
        public string nombreSucursal { get; set; }
        public DateTime? fecha { get; set; }
        public int pedidoId { get; set; }
        public string folio { get; set; }
        public decimal? total { get; set; }
        public decimal? subtotal { get; set; }
        public decimal? costoEnvio { get; set; }
        public string estatus { get; set; }
        public int estatusId { get; set; }

        public List<ResponseDetalleInformacionPedido> detallePedido { get; set; }
        public List<ResponseBitacoraPedidoActivo> bitacoraPedido { set; get; }

    }


    public class ResponseDetalleInformacionPedido
    {
        public int platilloId { get; set; }
        public string nombrePlatillo { get; set; }
        public string descripcion { get; set; }
        public decimal? precio { get; set; }
        public string urlImagen { get; set; }
    }


    public class ResponsePedidoStructPuntoVenta : ErrorObjCodeResponseBase
    {
        public List<Producto> productos { get; set; }
    }

    public class Producto
    {
        public int cantidad { get; set; }
        public string nombre { get; set; }
        public decimal? precio { get; set; }
        public int producto { get; set; }
        public bool? combo { get; set; }
        public List<DetalleCombo> detalleCombo { get; set; }
        public List<object> extras { get; set; }
    }

    public class DetalleCombo
    {
        public int agrupador { get; set; }
        public bool confirmarPorSeparado { get; set; }
        public Extra extra { get; set; }
        public int maximo { get; set; }
        public List<Opciones> opciones { get; set; }
        public bool puedeAgregarExtra { get; set; }
        public string titulo { get; set; }
    }

    public class Opciones
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int seleccionados { get; set; }
    }

    public class Extra
    {
        /** datos respecto al platillo, no a los ingredientes asociados **/
        public int? areaPreparacionId { get; set; }
        public int? categoriaProductoId { get; set; }
        public string clave { get; set; } // nombre del platillo
        public bool esCombo { get; set; }
        public bool esEnsalada { get; set; }
        public DateTime fechaRegistro { get; set; }
        public int id { get; set; }
        public string imagen { get; set; }
        public int indice { get; set; }
        public string nombre { get; set; }
        public string precio { get; set; }
        public bool principal { get; set; }
        public bool tieneVariedad { get; set; }
    }

}