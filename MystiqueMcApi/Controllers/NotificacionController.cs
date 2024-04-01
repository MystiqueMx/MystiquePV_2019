using MystiqueMC.DAL;
using MystiqueMcApi.Helpers;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class NotificacionController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";


        [Route("api/obtenerNotificacionesCliente")]
        public ResponseListaNotificacionCliente obtenerNotificacionesCliente([FromBody]RequestObtenerNotificacionesCliente entradas)
        {
            ResponseListaNotificacionCliente respuesta = new ResponseListaNotificacionCliente();

            try
            {
                // if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    using (contextEntity)
                    {
                        var result = contextEntity.clienteNotificaciones.Where(nc => nc.clienteId == entradas.idCliente && nc.empresaId == entradas.empresaId)
                            .Select(n => new ResponseNotificacionCliente
                            {
                                notificacionId = n.notificacionId,
                                descripcion = n.notificaciones.descripcion,
                                fechaRegistro = n.notificaciones.fechaRegistro,
                                //usuarioRegistro = n.notificaciones.usuarioRegistro,
                                isBeneficio = n.notificaciones.isBeneficio,
                                beneficioId = n.notificaciones.beneficioId,
                                activo = n.revisado,
                                titulo = n.notificaciones.titulo
                            }).ToList();

                        respuesta.listaNoticacionesCliente = result;
                    }
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }



        [Route("api/actualizarNotificacionesCliente")]
        public ResponseBase actualizarNotificacionesCliente([FromBody]RequestObtenerNotificacionesCliente entradas)
        {
            ResponseBase respuesta = new ResponseBase();

            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    var notificacionesCliente = contextEntity.clienteNotificaciones.Where(w => w.clienteId == entradas.idCliente).ToList();

                    foreach (var item in notificacionesCliente)
                    {
                        item.revisado = true;
                        contextEntity.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                    contextEntity.SaveChanges();

                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }

    }

    public class NotificacionHazPedidoController : BaseApiController
    {
        [Route("api/hazPedido/ObtenerNotificacionesConsumidor")]
        public ResponseListaNotificacionHazPedido ObtenerNotificacionesConsumidor([FromBody]RequestNotificacionHazPedido entradas)
        {
            ResponseListaNotificacionHazPedido respuesta = new ResponseListaNotificacionHazPedido();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var datosPedido = Contexto.ConsumidorNotificaciones.OrderByDescending(o => o.fechaEnviado).Where(w => w.consumidorId == entradas.consumidorId).Select(c => new ResponseNotificacionHazPedido
                        {
                            notificacionId = c.notificacionId,
                            titulo = c.NotificacionesHazPedido.titulo,
                            descripcion = c.NotificacionesHazPedido.descripcion,
                            fechaRegistro = c.NotificacionesHazPedido.fechaRegistro,
                            pedidoId = c.NotificacionesHazPedido.pedidoId,
                            fechaPedido = c.NotificacionesHazPedido.Pedidos1 != null ? c.NotificacionesHazPedido.Pedidos1.fechaRegistro : null,
                            montoPedido = c.NotificacionesHazPedido.Pedidos1 != null ? c.NotificacionesHazPedido.Pedidos1.totalPagar : null,
                            consumidorId = entradas.consumidorId,
                        }).ToList();

                        respuesta.respuesta = new List<ResponseNotificacionHazPedido>();
                        respuesta.respuesta = datosPedido;
                        respuesta.estatusPeticion = RespuestaOk;
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta;
        }
               
        [Route("api/hazPedido/ActualizarNotificacionesConsumidor")]
        public ErrorObjCodeResponseBase ActualizarNotificacionesConsumidor([FromBody]RequestNotificacionHazPedido entradas)
        {
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var notificacionesConsumidor = Contexto.ConsumidorNotificaciones.Where(w => w.consumidorId == entradas.consumidorId).ToList();

                        foreach (var item in notificacionesConsumidor)
                        {
                            item.revisado = true;
                            Contexto.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                        Contexto.SaveChanges();
                        respuesta.estatusPeticion = RespuestaOk;
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta;
        }
    }
}