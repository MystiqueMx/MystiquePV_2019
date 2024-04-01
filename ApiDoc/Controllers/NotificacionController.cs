using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiDoc.Controllers
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
                        var query = contextEntity.clienteNotificaciones
                            .Where(nc => nc.clienteId == entradas.idCliente && entradas.Empresas.Contains(nc.empresaId))
                            .OrderByDescending(c => c.fechaEnviado)
                            .AsQueryable();
                        var result = entradas.Idioma == Models.AppLanguage.Spanish 
                            ? query
                                .Select(n => new ResponseNotificacionCliente
                                {
                                    notificacionId = n.notificacionId,
                                    descripcion = n.notificaciones.descripcion,
                                    fechaRegistro = n.notificaciones.fechaRegistro,
                                    usuarioRegistro = 0,
                                    isBeneficio = n.notificaciones.isBeneficio,
                                    beneficioId = n.notificaciones.beneficioId,
                                    activo = n.revisado,
                                    titulo = n.notificaciones.titulo,
                                    linea1 = n.notificaciones.linea1,
                                    sucursalId = n.notificaciones.sucursalId,
                                    tipoNotificacion = n.notificaciones.tipoNotificacion,
                                    linea2 = n.notificaciones.linea2
                                }).ToList()
                              : query
                                .Select(n => new ResponseNotificacionCliente
                                {
                                    notificacionId = n.notificacionId,
                                    descripcion = n.notificaciones.descripcionIngles,
                                    fechaRegistro = n.notificaciones.fechaRegistro,
                                    usuarioRegistro = 0,
                                    isBeneficio = n.notificaciones.isBeneficio,
                                    beneficioId = n.notificaciones.beneficioId,
                                    activo = n.revisado,
                                    titulo = n.notificaciones.tituloIngles,
                                    linea1 = n.notificaciones.linea1ingles,
                                    linea2 = n.notificaciones.linea2ingles,
                                    sucursalId = n.notificaciones.sucursalId,
                                    tipoNotificacion = n.notificaciones.tipoNotificacion,
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
                    var notificacionesCliente = contextEntity.clienteNotificaciones.Where(w => w.clienteId == entradas.idCliente && entradas.Empresas.Contains(w.empresaId)).ToList();

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
}