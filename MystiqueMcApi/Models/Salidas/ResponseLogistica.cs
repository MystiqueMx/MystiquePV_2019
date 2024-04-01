using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseLogistica
    {
    }

    public class ResponseInfoPedidoLogisticaSucursal : ErrorObjCodeResponseBase
    {
        public List<ResponseListadoInformacionPedidoLogistica> respuesta { get; set; }
    }

    public class ResponseListadoInformacionPedidoLogistica
    {
        public string nombreSucursal { get; set; }
        public DateTime? fecha { get; set; }
        public int pedidoId { get; set; }
        public decimal? total { get; set; }
        public decimal? subtotal { get; set; }
        public decimal? costoEnvio { get; set; }
        public string estatus { get; set; }
        public int estatusId { get; set; }
        public string direccionEntrega { get; set; }
        public string formaPago { get; set; }
        public bool? isRecogerSucursal { get; set; }
        public string telefono { get; set; }
        public string comsumidor { get; set; }

        public List<ResponseDetalleInformacionPedidoLogistica> detallePedido { get; set; }
        [JsonIgnore]
        public List<ResponseBitacoraPedidoActivoLogisticaListado> _listaBitacoraPedido { set; get; }
        public List<ResponseBitacoraPedidoActivoLogistica> listaBitacoraPedido { set; get; }

        public static implicit operator ResponseListadoInformacionPedidoLogistica(List<ResponseListadoInformacionPedidoLogistica> v)
        {
            throw new NotImplementedException();
        }
        //public List<ResponseBitacoraPedidoActivoLogisticaListado
    }

    public class ResponseDetalleInformacionPedidoLogistica
    {
        public int platilloId { get; set; }
        public int? enredId { get; set; }
        public string nombrePlatillo { get; set; }
        public string descripcion { get; set; }
        public decimal? precio { get; set; }
        public string notas { get; set; }
        public string SKU { get; set; }

    }

    public class ResponseBitacoraPedidoActivoLogisticaListado
    {
        public List<ResponseBitacoraPedidoActivoLogistica> bitacoraPedido { set; get; }
    }

    public class ResponseBitacoraPedidoActivoLogistica
    {
        public DateTime? fecha { get; set; }
        public string comentario { get; set; }
        public bool isConsumidor { get; set; }
    }

    public class ResponseLogisticaSucursal : ErrorObjCodeResponseBase
    {
        public ResponseLogisticaEstatusSucursal respuesta { get; set; }
    }

    public class ResponseLogisticaEstatusSucursal
    {
        public bool sucursalEstatus { get; set; }

    }
}