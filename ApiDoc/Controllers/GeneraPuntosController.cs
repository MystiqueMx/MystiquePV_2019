using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class GeneraPuntosController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
        readonly string MENSAJE_INVITAR_CAPTURAR_PUNTOS = "MYSTIQUE_MENSAJE_INVITAR_CAPTURAR_PUNTOS";


        [Route("api/registrarCompraParaPuntos")]
        public ResponseGeneraPuntos registrarCompraParaPuntos([FromBody]RequestGeneraPuntos entrada)
        {
            ResponseGeneraPuntos respuesta = new ResponseGeneraPuntos();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia,entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    HelperEncriptor encripta = new HelperEncriptor();
                    Char delimiterCodigo = '|';
                    string codigoEntrada = encripta.Decrypt(entrada.codigoGenerado, ConfigurationManager.AppSettings.Get("tipoCodificacion"), ConfigurationManager.AppSettings.Get("IV"));

                    logger.Debug("codigoEntrada:" + codigoEntrada);
                    String[] resultadosCodigo = codigoEntrada.Split(delimiterCodigo);
                    int sucursalPV = Int32.Parse(resultadosCodigo[3]);

                    var sucursal = contextEntity.sucursales.Where(w => w.sucursalPuntoVenta == sucursalPV).FirstOrDefault();

                    ///DateTime dt = DateTime.ParseExact(resultadosCodigo[1], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
               
                  if (sucursal != null)
                    {
                        var outResultadoParameter = new ObjectParameter("resultado", typeof(string));
                        contextEntity.SP_CargarPuntosPorCompra(entrada.membresiaId, DateTime.Parse(resultadosCodigo[1]), resultadosCodigo[0], Convert.ToDecimal(resultadosCodigo[2]), sucursal.idSucursal, entrada.codigoGenerado , entrada.comercioId, outResultadoParameter);

                        Char delimiter = ';';
                        String[] resultados = outResultadoParameter.Value.ToString().Split(delimiter);
                        if (resultados[0].Equals("OK"))
                        {
                            respuesta.Success = true;
                            respuesta.ErrorMessage = resultados[1];
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
                        respuesta.ErrorMessage = "No existe la sucursal.";
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



        [Route("api/obtenerEstadoCuentaPuntos")]
        public ResponseEstadoCuentaPuntos obtenerEstadoCuentaPuntos([FromBody]RequestGeneraPuntos entrada)
        {
            ResponseEstadoCuentaPuntos respuesta = new ResponseEstadoCuentaPuntos();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    saldoPuntos estadoCuenta = contextEntity.saldoPuntos.Where(w => w.membresiaId == entrada.membresiaId && w.comercioId== entrada.comercioId).FirstOrDefault();

                    if (estadoCuenta != null)
                    {
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "";
                        respuesta.puntosActuales = estadoCuenta.puntosActuales;
                        respuesta.puntosAnteriores = estadoCuenta.puntosAnteriores;
                        respuesta.visitasActuales = estadoCuenta.visitasActuales;
                        respuesta.visitasAnteriores = estadoCuenta.visitasAnteriores;
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = "";
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_INVITAR_CAPTURAR_PUNTOS);
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

        [Route("api/obtenerListadoMovimientoPuntos")]
        public ResponseListadoMovimientos obtenerListadoMovimientoPuntos([FromBody]RequestListadoPuntos entrada)
        {
            ResponseListadoMovimientos respuesta = new ResponseListadoMovimientos();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    respuesta.listadoMovimientos = contextEntity.SP_ObtenerListadoMovimientosPuntos(entrada.membresiaId, entrada.comercioId).ToList();

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


        [Route("api/obtenerListadoMovimientoPuntosV2")]
        public ResponseListadoMovimientosV2 obtenerListadoMovimientoPuntosV2([FromBody]RequestListadoPuntos entrada)
        {
            ResponseListadoMovimientosV2 respuesta = new ResponseListadoMovimientosV2();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    respuesta.listadoMovimientos = contextEntity.SP_ObtenerListaMovimientosPuntos(entrada.membresiaId, entrada.comercioId).ToList();

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