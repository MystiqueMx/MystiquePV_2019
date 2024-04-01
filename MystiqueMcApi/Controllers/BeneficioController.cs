using MystiqueMC.DAL;
using MystiqueMC.Helpers.Emails;
using MystiqueMcApi.Helpers;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class BeneficioController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly string TITULO_NOTIFICACION_BENEFICIO_CANJEADO = ConfigurationManager.AppSettings.Get("TITULO_NOTIFICACION_BENEFICIO_CANJEADO");
        readonly string CONTENIDO_NOTIFICACION_BENEFICIO_CANJEADO = ConfigurationManager.AppSettings.Get("CONTENIDO_NOTIFICACION_BENEFICIO_CANJEADO");
        private const int USUARIO_NOTIFICACION = 1;
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
        private PermisosApi validar = new PermisosApi();


        [Route("api/obtenerBeneficiosPorSucursal")]
        public ResponseBeneficioSucursal obtenerBeneficiosPorSucursal([FromBody]RequestObtenerBeneficioSucursal entradas)
        {
            ResponseBeneficioSucursal respuesta = new ResponseBeneficioSucursal();
            ListaBeneficioSucursal uno = new ListaBeneficioSucursal();

            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.listaBeneficiosSucursal = contextEntity.SP_ObtenerBeneficiosSucursal(entradas.sucursalId, entradas.empresaId,entradas.clienteId,entradas.membresiaId).ToList();
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


        [Route("api/obtenerBeneficioDetalle")]
        public ResponseBeneficioDetalle obtenerBeneficioDetalle([FromBody]RequestObtenerBeneficioDetalle entradas)
        {
            ResponseBeneficioDetalle respuesta = new ResponseBeneficioDetalle();
            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    respuesta.beneficioDetalle = contextEntity.SP_Obtener_DetalleBeneficio(entradas.beneficioId, entradas.clienteId, entradas.sucursalId).FirstOrDefault();
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


        [Route("api/registrarBeneficioCliente")]
        public ResponseBase registrarBeneficioCliente([FromBody]RequestInsertarBeneficioCliente entradas)
        {
            ResponseBase respuesta = new ResponseBase();
            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {

                    sucursales miSucursal = contextEntity.sucursales.Where(w => w.sucursalPuntoVenta == entradas.sucursalId).FirstOrDefault();

                    contextEntity.beneficioAplicados.Add(new beneficioAplicados
                    {
                     clienteId = entradas.clienteId,
                     beneficioId = entradas.beneficioId,
                     sucursalId = miSucursal.idSucursal,
                     fechaRegistro = DateTime.Now,
                     membresiaId = entradas.membresiaId,
                     folioCompra = entradas.folioCompra,
                     fechaCompra = entradas.fechaCompra,
                     montoCompra = entradas.montoCompra
                    });
                    contextEntity.SaveChanges();

                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    EnviarNotificacionBeneficioCanjeado(entradas.clienteId, entradas.beneficioId);
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

        [Route("api/obtenerRespuestaTestServer")]
        public ResponseBase obtenerRespuestaTestServer()
        {
            ResponseBase respuesta = new ResponseBase();
            try
            {
                respuesta.Success = true;
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }

        #region Helpers
        private bool EnviarNotificacionBeneficioCanjeado(int clienteId, int beneficioId)
        {
            bool resultado = false;
            try
            {
                    var UsuariosMovil = contextEntity.login
                        .Where(c => c.clientes.idCliente == clienteId)
                        .Select(c => new { c.playerId, c.clienteId, c.clientes.empresaId })
                        .ToList();
                if (UsuariosMovil.Count == 0)
                {
                    resultado = true;
                    return resultado;
                }
                
                    var PlayerIds = UsuariosMovil
                        .Where(c => c.playerId!=null && c.playerId.Length == 36) // limpia los player ids invalidos
                        .Select(c => c.playerId)
                        .Distinct()
                        .ToArray();
                    var Beneficio = contextEntity.beneficios.Find(beneficioId);
                    notificaciones notificacion = new notificaciones
                    {
                        activo = true,
                        fechaRegistro = DateTime.Now,
                        descripcion = string.Format(CONTENIDO_NOTIFICACION_BENEFICIO_CANJEADO, Beneficio.descripcion),
                        titulo = TITULO_NOTIFICACION_BENEFICIO_CANJEADO,
                        usuarioRegistro = USUARIO_NOTIFICACION,
                        isBeneficio = false,
                        empresaId = UsuariosMovil.First().empresaId,
                    };

                    contextEntity.notificaciones.Add(notificacion);
                    contextEntity.clienteNotificaciones.Add(new clienteNotificaciones
                    {
                        notificaciones = notificacion,
                        clienteId = clienteId,
                        fechaEnviado = notificacion.fechaRegistro,
                        revisado = false,
                        empresaId = notificacion.empresaId,
                    });

                    contextEntity.SaveChanges();

                    SendNotificationDelegate @delegate = new SendNotificationDelegate();
                    @delegate.SendNotificationPorPlayerIds(PlayerIds, notificacion.titulo, notificacion.descripcion);

                resultado = true;
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
             
            }
            return resultado;
        }
        #endregion
    }

}