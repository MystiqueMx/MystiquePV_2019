using conekta;
using Microsoft.AspNet.Identity.Owin;
using MystiqueMC.DAL;
using MystiqueMC.Helpers.Emails;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Pedidos;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MystiqueMcApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LogisticaController : BaseApiController
    {
        private readonly string _mensajeCambioEstatus = ConfigurationManager.AppSettings.Get("MENSAJE_CAMBIOESTATUS");
        private readonly string _tituloCambioEstatus = ConfigurationManager.AppSettings.Get("TITULO_CAMBIOESTATUS");
        private readonly string _tituloMensajeLogistica = ConfigurationManager.AppSettings.Get("TITULO_MENSAJE_LOGISTICA");

        [Route("api/ActualizarPedidoEnCocina")]
        public async Task<ErrorObjCodeResponseBase> ActualizarPedidoEnCocinaAsync([FromBody]RequestLogisticaActualizarEstatus entradas)
        {
            #region ActualizarPedidoEnCocinaAsync
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();
            try
            {
                if (ModelState.IsValid)
                {

                    //var usuarioId = await VerificarUsuarioAsync();
                    //if (usuarioId > 0)
                    //{
                    using (var tx = Contexto.Database.BeginTransaction())
                    {
                        try
                        {
                            var pedido = Contexto.Pedidos1
                                .Include(c => c.PedidosConekta)
                                .FirstOrDefault(w => w.idPedido == entradas.pedidoId);
                            if (pedido == null) return new ErrorObjCodeResponseBase { estatusPeticion = RespuestaErrorValidacion("El pedido no existe") };

                            //var sucursalValidada = Contexto.ConfUsuariosComercioSucursal.FirstOrDefault(w => w.usuarioId == usuarioId);
                            //if (sucursalValidada != null)
                            bool sucursalValidada = Contexto.sucursales.Where(w => w.idSucursal == entradas.sucursalId).Count() > 0;
                            if (sucursalValidada)
                            {
                                pedido.catPedidoEstatusId = (int)EstatusPedido.ENCOCINA;

                                Contexto.SeguimientoPedidos.Add(new MystiqueMC.DAL.SeguimientoPedidos
                                {
                                    pedidoId = entradas.pedidoId,
                                    fechaRegistro = DateTime.Now,
                                    catPedidoEstatusId = (int)EstatusPedido.ENCOCINA,
                                    nombreRegistro = entradas.nombreUsuarioActualizo
                                });

                                if (pedido.PedidosConekta?.Any() ?? false)
                                {
                                    foreach (var pedidosConekta in pedido.PedidosConekta)
                                    {
                                        var order = new Order().find(pedidosConekta.idOrdenPedido);
                                        order.capture();
                                        pedidosConekta.fechaActualizacion = DateTime.Now;
                                        pedidosConekta.estatusConekta = (int)PedidoTarjetaEstatus.Pagado;
                                        Contexto.Entry(pedidosConekta).State = EntityState.Modified;
                                    }
                                }
                                Contexto.SaveChanges();
                                tx.Commit();
                                respuesta.estatusPeticion = RespuestaOk;
                                EnviarNotificacionPedido(pedido, entradas.sucursalId, string.Format(_mensajeCambioEstatus, "En cocina"), string.Format(_tituloCambioEstatus, pedido.idPedido.ToString("D7")), entradas.usuarioId);
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion("No cuenta con permisos para cambiar el estatus de este restaurante.");
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                            respuesta.estatusPeticion = RespuestaErrorInterno;
                            tx.Rollback();
                        }
                    }
                    //}
                    //else
                    //{
                    //    respuesta.estatusPeticion = RespuestaErrorValidacion("Usuario y Constraseña no validos.");
                    //}
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
            #endregion
        }

        [Route("api/ActualizarPedidoListo")]
        public async Task<ErrorObjCodeResponseBase> ActualizarPedidoListoAsync([FromBody]RequestLogisticaActualizarEstatus entradas)
        {
            #region ActualizarPedidoListoAsync
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();
            try
            {
                if (ModelState.IsValid)
                {

                    //var usuarioId = await VerificarUsuarioAsync();
                    //if (usuarioId > 0)
                    //{
                        using (var tx = Contexto.Database.BeginTransaction())
                        {
                            try
                            {
                                string estatusActualizado = "";
                                var pedido = Contexto.Pedidos1.Where(w => w.idPedido == entradas.pedidoId).FirstOrDefault();
                                int actualizaPedidoEstatus = 0;
                                //var sucursalValidada = Contexto.ConfUsuariosComercioSucursal.Where(w => w.usuarioId == usuarioId).FirstOrDefault();
                                //if (sucursalValidada != null)
                                //{
                                    if (pedido.recogerEnSucursal == true)
                                    {
                                        pedido.catPedidoEstatusId = (int)EstatusPedido.LISTOPARAENTREGA;
                                        actualizaPedidoEstatus = (int)EstatusPedido.LISTOPARAENTREGA;
                                        estatusActualizado = "Listo Recoger";
                                    }
                                    else
                                    {
                                        pedido.catPedidoEstatusId = (int)EstatusPedido.ENTRANSITO;
                                        actualizaPedidoEstatus = (int)EstatusPedido.ENTRANSITO;
                                        estatusActualizado = "En camino";
                                    }


                                    Contexto.SeguimientoPedidos.Add(new SeguimientoPedidos
                                    {
                                        pedidoId = entradas.pedidoId,
                                        fechaRegistro = DateTime.Now,
                                        catPedidoEstatusId = actualizaPedidoEstatus,
                                        nombreRegistro = entradas.nombreUsuarioActualizo
                                    });

                                    Contexto.SaveChanges();
                                    tx.Commit();
                                    respuesta.estatusPeticion = RespuestaOk;
                                    EnviarNotificacionPedido(pedido, entradas.sucursalId, string.Format(_mensajeCambioEstatus, estatusActualizado), string.Format(_tituloCambioEstatus, pedido.idPedido.ToString("D7")), entradas.usuarioId);
                                //}
                                //else
                                //{
                                //    respuesta.estatusPeticion = RespuestaErrorValidacion("No cuenta con permisos para cambiar el estatus de este restaurante.");
                                //}
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                                respuesta.estatusPeticion = RespuestaErrorInterno;
                                tx.Rollback();
                            }
                        }
                    //}
                    //else
                    //{
                    //    respuesta.estatusPeticion = RespuestaErrorValidacion("Usuario y Constraseña no validos.");
                    //}
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
            #endregion
        }

        [Route("api/ActualizarPedidoEntregado")]
        public async Task<ErrorObjCodeResponseBase> ActualizarPedidoEntregadoAsync([FromBody]RequestLogisticaActualizarEstatus entradas)
        {
            #region ActualizarPedidoEntregadoAsync
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();
            try
            {
                if (ModelState.IsValid)
                {

                    //var usuarioId = await VerificarUsuarioAsync();
                    //if (usuarioId > 0)
                    //{
                        using (var tx = Contexto.Database.BeginTransaction())
                        {
                            try
                            {

                                var pedido = Contexto.Pedidos1.Where(w => w.idPedido == entradas.pedidoId).FirstOrDefault();

                                //var sucursalValidada = Contexto.ConfUsuariosComercioSucursal.Where(w => w.usuarioId == usuarioId).FirstOrDefault();
                                //if (sucursalValidada != null)
                                //{
                                    pedido.catPedidoEstatusId = (int)EstatusPedido.ENTREGADO;

                                    Contexto.SeguimientoPedidos.Add(new SeguimientoPedidos
                                    {
                                        pedidoId = entradas.pedidoId,
                                        fechaRegistro = DateTime.Now,
                                        catPedidoEstatusId = (int)EstatusPedido.ENTREGADO,
                                        nombreRegistro = entradas.nombreUsuarioActualizo
                                    });

                                    Contexto.SaveChanges();
                                    tx.Commit();
                                    respuesta.estatusPeticion = RespuestaOk;
                                    EnviarNotificacionPedido(pedido, entradas.sucursalId, string.Format(_mensajeCambioEstatus, "Entregado"), string.Format(_tituloCambioEstatus, pedido.idPedido.ToString("D7")), entradas.usuarioId);
                                //}
                                //else
                                //{
                                //    respuesta.estatusPeticion = RespuestaErrorValidacion("No cuenta con permisos para cambiar el estatus de este restaurante.");
                                //}
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                                respuesta.estatusPeticion = RespuestaErrorInterno;
                                tx.Rollback();
                            }
                        }
                    //}
                    //else
                    //{
                    //    respuesta.estatusPeticion = RespuestaErrorValidacion("Usuario y Constraseña no validos.");
                    //}
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
            #endregion
        }

        [Route("api/VerificarEstatusSucursal")]
        public async Task<ResponseLogisticaSucursal> VerificarEstatusSucursal([FromBody]RequestLogistica entradas)
        {
            #region VerificarEstatusSucursal
            ResponseLogisticaSucursal respuesta = new ResponseLogisticaSucursal();
            try
            {
                if (ModelState.IsValid)
                {
                    //var usuarioId = await VerificarUsuarioAsync();
                    //if (usuarioId > 0)
                    //{

                        //var sucursalValidada = Contexto.ConfUsuariosComercioSucursal.Where(w => w.usuarioId == usuarioId).FirstOrDefault();
                        //if (sucursalValidada != null)
                        //{
                            var isActivaSucursal = Contexto.ConfSucursales.Where(w => w.sucursalId == entradas.sucursalId).Select(s => s.activoPlataforma).FirstOrDefault();
                            respuesta.respuesta = new ResponseLogisticaEstatusSucursal();
                            respuesta.respuesta.sucursalEstatus = isActivaSucursal;
                            respuesta.estatusPeticion = RespuestaOk;
                        //}
                        //else
                        //{
                        //    respuesta.estatusPeticion = RespuestaErrorValidacion("No cuenta con permisos para cambiar el estatus de este restaurante.");
                        //}

                    //}
                    //else
                    //{
                    //    respuesta.estatusPeticion = RespuestaErrorValidacion("Usuario y Constraseña no validos.");
                    //}
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
            #endregion
        }

        [Route("api/ObtenerInformacionPedidoLogisticaSucursal")]
        public async Task<ResponseInfoPedidoLogisticaSucursal> ObtenerInformacionPedidoLogisticaSucursalAsync([FromBody]RequestLogisticaPedidos entrada)
        {
            #region ObtenerInformacionPedidoLogisticaSucursalAsync
            ResponseInfoPedidoLogisticaSucursal respuesta = new ResponseInfoPedidoLogisticaSucursal();
            try
            {
                if (ModelState.IsValid)
                {
                    //var usuarioId = await VerificarUsuarioAsync();
                    //if (usuarioId > 0)
                    //{
                    var fecha = DateTime.Now.Date;

                    //var sucursalValidada = Contexto.ConfUsuariosComercioSucursal.Where(w => w.usuarioId == usuarioId).Select(s => s.sucursalId).ToList();
                    //if (sucursalValidada != null)
                    //{
                    var detallePedido = Contexto.Pedidos1
                        .Where(w => w.catPedidoEstatusId < 6
                            && DbFunctions.TruncateTime(w.fechaRegistro) == fecha
                            && w.sucursalId == entrada.sucursalId
                            /*&& sucursalValidada.Contains(w.sucursalId)*/)
                        .Select(s => new ResponseListadoInformacionPedidoLogistica
                        {
                            nombreSucursal = s.sucursales.nombre,
                            fecha = s.fechaRegistro,
                            pedidoId = s.idPedido,
                            total = s.totalPagar,
                            subtotal = s.subTotal,
                            costoEnvio = s.costoEnvio,
                            estatusId = s.CatPedidoEstatus.estatusAplicacionId,
                            estatus = s.CatPedidoEstatus.etiquetaEstatus,
                            direccionEntrega = s.direccionEntrega,
                            formaPago = s.CatMetodoPago.descripcion,
                            isRecogerSucursal = s.recogerEnSucursal,
                            telefono = s.telefonoQuienRecibe,
                            //comsumidor = s.nombreQuienRealizo,
                            comsumidor = s.quienRecibe,
                            _listaBitacoraPedido = s.SeguimientoPedidos.Select(m => new ResponseBitacoraPedidoActivoLogisticaListado
                            {
                                bitacoraPedido = m.SeguimientoBitacoraPedidos.Select(v => new ResponseBitacoraPedidoActivoLogistica
                                {
                                    fecha = v.fechaRegistro,
                                    comentario = v.comentario,
                                    isConsumidor = v.isconsumidor
                                }).ToList()
                            }).ToList(),
                            detallePedido = s.DetallePedidos.Select(r => new ResponseDetalleInformacionPedidoLogistica
                            {
                                platilloId = r.platilloId,
                                //nombrePlatillo = r.Productos.ConfMenuPlatillos.FirstOrDefault().Menus.nombre + " " + r.Productos.nombre,
                                nombrePlatillo = r.Productos.CategoriaProductos.descripcion + " " + r.Productos.nombre,
                                descripcion = r.descripcion,
                                precio = r.precioUnitario,
                                notas = r.nota,
                                //enredId = Contexto.ConfQdcEnred.Where(w => w.idQdc == r.platilloId).FirstOrDefault().idEnred
                                SKU = r.Productos.sku ?? ""
                            }).ToList()
                        }).ToList();

                    foreach (var item in detallePedido)
                    {
                        item.listaBitacoraPedido = new List<ResponseBitacoraPedidoActivoLogistica>();
                        foreach (var bitacora in item._listaBitacoraPedido)
                        {
                            item.listaBitacoraPedido.AddRange(bitacora.bitacoraPedido);
                        }
                        item.listaBitacoraPedido = item.listaBitacoraPedido.OrderBy(o => o.fecha).ToList();
                    }

                    respuesta.respuesta = detallePedido;
                    respuesta.estatusPeticion = RespuestaOk;
                    //}
                    //else
                    //{
                    //    respuesta.estatusPeticion = RespuestaErrorValidacion("No cuenta con restaurantes configurados.");
                    //}
                    //}
                    //else
                    //{
                    //    respuesta.estatusPeticion = RespuestaErrorValidacion("Usuario y Constraseña no validos.");
                    //}
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
            #endregion
        }

        internal async Task<int> VerificarUsuarioAsync()
        {
            #region VerificarUsuarioAsync
            int respuesta = 0;

            try
            {
                const string AUTH_ENCODING = "ISO-8859-1";
                var auth = HttpContext.Current.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(auth))
                {
                    var cred = Encoding.GetEncoding(AUTH_ENCODING).GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                    var user = new { Name = cred[0], Pass = cred[1] };

                    respuesta = await ValidarUsuarioContraseniaAsync(user.Name, user.Pass);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
            }
            return respuesta; 
            #endregion
        }

        internal async Task<int> ValidarUsuarioContraseniaAsync(string usuario, string contrasenia)
        {
            #region ValidarUsuarioContraseniaAsync
            int respuesta = 0;
            try
            {
                var userManager = Request.GetOwinContext().GetUserManager<IdentityConfig.ApplicationUserManager>();
                var identityUser = await userManager.FindAsync(usuario, contrasenia);

                if (identityUser != null)
                {
                    respuesta = identityUser.UsuarioId;
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
            }
            return respuesta; 
            #endregion
        }

        internal bool EnviarNotificacionPedido(Pedidos1 pedido, int sucursalId, string titulo, string descripcion, int usuarioId)
        {
            #region EnviarNotificacionPedido
            bool resultado = false;
            try
            {
                var login = pedido.clientes.login.OrderByDescending(o => o.fechaRegistro).ToArray();

                var listaPlyer = login.Select(s => s.playerId).Distinct().ToArray();

                NotificacionesHazPedido notificacion = new NotificacionesHazPedido
                {
                    sucursalId = sucursalId,
                    descripcion = descripcion,
                    titulo = titulo,
                    activo = true,
                    fechaRegistro = DateTime.Now,
                    usuarioRegistro = usuarioId,
                    pedidoId = pedido.idPedido
                };


                Contexto.NotificacionesHazPedido.Add(notificacion);

                Contexto.ConsumidorNotificaciones.Add(new ConsumidorNotificaciones
                {
                    consumidorId = pedido.consumidorId,
                    fechaEnviado = DateTime.Now,
                    revisado = false
                });

                Contexto.SaveChanges();

                SendNotificationDelegate @delegate = new SendNotificationDelegate();
                @delegate.SendNotificationPorPlayerIds(listaPlyer, notificacion.titulo, notificacion.descripcion);
                resultado = true;


            }
            catch (Exception e)
            {
                Logger.Error("ERROR:" + e.Message);

            }

            return resultado;
            #endregion
        }
    }

    public enum EstatusPedido
    {
        RECIBIDO = 1,
        ACEPTADO = 2,
        ENCOCINA = 3,
        LISTOPARAENTREGA = 4,
        ENTRANSITO = 5,
        ENTREGADO = 6,
        CANCELADO = 7,
    }
}
