using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseNotificacion
    {
        public int notificacionId { get; set; }
        public int clienteId { get; set; }
        public System.DateTime FechaEnviado { get; set; }
        public bool revisado { get; set; }
        public Nullable<System.DateTime> fechaRevisado { get; set; }
    }


    public class ResponseListaNotificacicones : ResponseBase
    {
        public List<ResponseNotificacion> ListaNotificaciones { get; set; }
    }



    public class ResponseNotificacionCliente : ResponseBase
    {
        public int notificacionId { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public bool? isBeneficio { get; set; }
        public int? beneficioId { get; set; }
        public bool? activo { get; set; }
        public int usuarioRegistro { get; set; }
        public int clienteId { get; set; }
    }


    public class ResponseListaNotificacionCliente : ResponseBase
    {
        public List<ResponseNotificacionCliente> listaNoticacionesCliente { get; set; }
    }


    public class ResponseNotificacionHazPedido
    {
        public int notificacionId { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public int? pedidoId { get; set; }
        public System.DateTime? fechaPedido { get; set; }
        public decimal? montoPedido { get; set; }
        public int consumidorId { get; set; }
    }

    public class ResponseListaNotificacionHazPedido : ErrorObjCodeResponseBase
    {
        public List<ResponseNotificacionHazPedido> respuesta { get; set; }
    }
}