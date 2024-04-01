using MystiqueMC.DAL;
using MystiqueMC.Helpers.Emails;
using MystiqueMcApi.Helpers;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class CanjePuntosController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int USUARIO_NOTIFICACION = 1;
        readonly string TITULO_NOTIFICACION_RECOMPENSA_CANJEADA = ConfigurationManager.AppSettings.Get("TITULO_NOTIFICACION_RECOMPENSA_CANJEADA");
        readonly string CONTENIDO_NOTIFICACION_RECOMPENSA_CANJEADO = ConfigurationManager.AppSettings.Get("CONTENIDO_NOTIFICACION_RECOMPENSA_CANJEADO");
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_NO_EXISTE_CODIGO = "MYSTIQUE_MENSAJE_NO_EXISTE_CODIGO";
        readonly string MENSAJE_CODIGO_INVALIDO = "MYSTIQUE_MENSAJE_CODIGO_INVALIDO";
        readonly string MENSAJE_NO_EXISTE_MEMBRESIA = "MYSTIQUE_MENSAJE_NO_EXISTE_MEMBRESIA";
        
        private PermisosApi validar = new PermisosApi();


        [Route("api/obtenerListadoRecompensas")]
        public ResponseListadoRecompensas obtenerListadoRecompensas([FromBody]RequestCanjePuntos entrada)
        {
            ResponseListadoRecompensas respuesta = new ResponseListadoRecompensas();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    var membresia = contextEntity.membresias.Where(w => w.idMembresia == entrada.membresiaId).FirstOrDefault();

                    if (membresia != null)
                    {
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "";
                        respuesta.listadoRecompensas = contextEntity.SP_ObtenerListadoRecompensas(entrada.comercioId, membresia.catTipoMembresiaId).ToList();
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_EXISTE_MEMBRESIA);
                    }
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


        [Route("api/registrarCanjePuntos")]
        public ResponseRegistrarCanjePuntos registrarCanjePuntos([FromBody]RequestRegistrarCanjePuntos entrada)
        {
            ResponseRegistrarCanjePuntos respuesta = new ResponseRegistrarCanjePuntos();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    recompensas validarRecompensa = contextEntity.recompensas.Where(w => w.idRecompesa == entrada.recompensaId).FirstOrDefault();

                    confProductoPVEquivalencia puntoVenta = contextEntity.confProductoPVEquivalencia.Where(w => w.catProductoId == validarRecompensa.productoId).FirstOrDefault();
                    int productoIdPuntoVenta = puntoVenta.productoPuntoVentaId;

                    
                    StringBuilder generarCodigo = new StringBuilder();
                    string codigoCanje = "";
                    generarCodigo.Append(productoIdPuntoVenta);
                    generarCodigo.Append("|");
                    generarCodigo.Append(entrada.membresiaId);
                    generarCodigo.Append("|");
                    generarCodigo.Append(entrada.recompensaId);
                    generarCodigo.Append("|");
                    generarCodigo.Append(DateTime.Now);
                    generarCodigo.Append("|");
                    generarCodigo.Append(entrada.correoElectronico);

                    codigoCanje = ConfigurationManager.AppSettings.Get("tipoCupon") + generarCodigo.ToString() + ConfigurationManager.AppSettings.Get("finRetornoPV"); //% fin de retorno de punto de venta
                    

                    var outResultadoParameter = new ObjectParameter("resultado", typeof(string));
                    contextEntity.SP_RegistrarCanjePuntos(entrada.membresiaId, entrada.recompensaId, codigoCanje, outResultadoParameter);

                    Char delimiter = ';';
                    String[] resultados = outResultadoParameter.Value.ToString().Split(delimiter);
                    if (resultados[0].Equals("OK"))
                    {
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "";
                        respuesta.codigoGenerado = codigoCanje;
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = resultados[1];
                    }
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


        [Route("api/validarCuponRecompensa")]
        public ResponseCanjePuntos validarCuponRecompensa([FromBody]RequestCanjePuntosValidarCupon entrada)
        {
            ResponseCanjePuntos respuesta = new ResponseCanjePuntos();
            int canjePuntoId = 0;

            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {

                    canjePuntos existe = contextEntity.canjePuntos.Where(w => w.folioCanje == entrada.codigoCupon).FirstOrDefault();

                    if (existe != null)
                    {
                        bool res = true;
                        string mensaje = "";

                        confProductoPVEquivalencia puntoVenta = contextEntity.confProductoPVEquivalencia.Where(w => w.catProductoId == existe.recompensas.productoId).FirstOrDefault();
                                      
                        respuesta.productoPuntoVentaId = puntoVenta.productoPuntoVentaId;
                        respuesta.nombreProducto = existe.recompensas.catProductos.nombre;

                        switch (existe.catEstatusCanjePuntoId)
                        {
                            case 1:
                                mensaje = existe.catEstatusCanjePuntos.descripcion;
                                canjePuntoId = existe.idCanjePunto;
                                break;
                            case 2:
                                res = false;
                                mensaje = existe.catEstatusCanjePuntos.descripcion;
                                canjePuntoId = existe.idCanjePunto;
                                break;
                            case 3:
                                res = false;
                                mensaje = existe.catEstatusCanjePuntos.descripcion;
                                canjePuntoId = existe.idCanjePunto;
                                break;
                            case 4:
                                res = false;
                                mensaje = existe.catEstatusCanjePuntos.descripcion;
                                canjePuntoId = existe.idCanjePunto;
                                break;
                        }
                        respuesta.Success = res;
                        respuesta.ErrorMessage = mensaje;
                        respuesta.canjePuntoId = canjePuntoId;
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_EXISTE_CODIGO);
                        respuesta.canjePuntoId = canjePuntoId;
                    }
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
                respuesta.canjePuntoId = canjePuntoId;
            }
            return respuesta;
        }


        [Route("api/canjearRecompensa")]
        public ResponseCanjeRecompensa canjearRecompensa([FromBody]RequestCanjeRecompensa entrada)
        {
            ResponseCanjeRecompensa respuesta = new ResponseCanjeRecompensa();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    sucursales validarSucursal = new sucursales();
                    membresias clienteMembresia = new membresias();
                    int membresiaId = 0;
                    int recompensaId = 0;
                    int sucursalId = 0;
                    try
                    {                       
                        Char delimiterCodigo = '|';
                        string codigoEntrada = entrada.codigoCupon.Substring(1, entrada.codigoCupon.Length - 2);
                        String[] resultadosCodigo = codigoEntrada.Split(delimiterCodigo);
                        membresiaId = Int32.Parse(resultadosCodigo[1]);
                        recompensaId = Int32.Parse(resultadosCodigo[2]);

                        validarSucursal = contextEntity.sucursales.Where(w => w.sucursalPuntoVenta == entrada.sucursalId).FirstOrDefault();

                        clienteMembresia = contextEntity.membresias.Where(w => w.idMembresia == membresiaId).FirstOrDefault();

                        sucursalId = validarSucursal.idSucursal;
                        respuesta.Success = true;
                        
                    }
                    catch (Exception e)
                    {

                        logger.Error("ERROR:" + e.Message);
                        respuesta.Success = false;
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_CODIGO_INVALIDO);
                    }
                    if (respuesta.Success)
                    {
                        var outResultadoParameter = new ObjectParameter("resultado", typeof(string));
                        contextEntity.SP_CanjearRecompensa(membresiaId, recompensaId, entrada.codigoCupon, sucursalId, entrada.canjepuntoId, entrada.folioCompra,entrada.montoCompra, outResultadoParameter);

                        Char delimiter = ';';
                        String[] resultados = outResultadoParameter.Value.ToString().Split(delimiter);
                        if (resultados[0].Equals("OK"))
                        {
                            EnviarNotificacionRecompensaCanjeada(clienteMembresia.clientes.email, entrada.empresaId, membresiaId);
                            respuesta.Success = true;
                            respuesta.ErrorMessage = "";
                        }
                        else
                        {
                            respuesta.Success = false;
                            respuesta.ErrorMessage = resultados[1];
                        }
                    }
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


        [Route("api/obtenerListaDeCuponesActivos")]
        public ResponseListaCupones obtenerListaDeCuponesActivos([FromBody]RequesListaCupones entrada)
        {
            ResponseListaCupones respuesta = new ResponseListaCupones();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    respuesta.listadoCupones = contextEntity.VW_ObtenerListadoCupones.Where(w => w.membresiaId == entrada.membresiaId && w.catEstatusCanjePuntoId == 1 && w.comercioId== entrada.comercioId).ToList();
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



        [Route("api/eliminarRecompensa")]
        public ResponseCanjeRecompensa eliminarRecompensa([FromBody]RequestEliminarRecompensa entrada)
        {
            ResponseCanjeRecompensa respuesta = new ResponseCanjeRecompensa();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    var outResultadoParameter = new ObjectParameter("resultado", typeof(string));
                    contextEntity.SP_EliminarRecompensa(entrada.membresiaId, entrada.canjepuntoId, outResultadoParameter);

                    Char delimiter = ';';
                    String[] resultados = outResultadoParameter.Value.ToString().Split(delimiter);
                    if (resultados[0].Equals("OK"))
                    {
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "";
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = resultados[1];
                    }
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

        #region Helpers
        private bool EnviarNotificacionRecompensaCanjeada(string CorreoElectronico, int EmpresaId, int MembresiaId)
        {
            bool resultado = false;
            try
            {
                var UsuariosMovil = contextEntity.login
                .Where(c => c.clientes.email == CorreoElectronico && c.clientes.empresaId == EmpresaId)
                .Select(c => new { c.playerId, c.clienteId, c.clientes.empresaId })
                .ToList();
            if (UsuariosMovil.Count == 0)
                {
                    resultado = true;
                    return resultado;
                }
            var PlayerIds = UsuariosMovil
                .Where(c => c.playerId != null && c.playerId.Length == 36) // limpia los player ids invalidos
                .Select(c => c.playerId)
                .Distinct()
                .ToArray();

                var EstadoCuenta = contextEntity.saldoPuntos
                    .Where(c => c.membresiaId == MembresiaId)
                    .Select(c => c.puntosActuales)
                    .FirstOrDefault();

                notificaciones notificacion = new notificaciones
            {
                activo = true,
                fechaRegistro = DateTime.Now,
                descripcion = string.Format(CONTENIDO_NOTIFICACION_RECOMPENSA_CANJEADO, EstadoCuenta),
                titulo = TITULO_NOTIFICACION_RECOMPENSA_CANJEADA,
                usuarioRegistro = USUARIO_NOTIFICACION,
                isBeneficio = false,
                empresaId = UsuariosMovil.First().empresaId,
            };

            contextEntity.notificaciones.Add(notificacion);
            contextEntity.clienteNotificaciones.Add(new clienteNotificaciones
            {
                notificaciones = notificacion,
                fechaEnviado = notificacion.fechaRegistro,
                revisado = false,
                clienteId = UsuariosMovil.First().clienteId,
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