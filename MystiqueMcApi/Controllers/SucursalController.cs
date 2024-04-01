using MystiqueMC.DAL;
using MystiqueMcApi.Helpers;
using MystiqueMcApi.Models;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class SucursalController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";

        [Route("api/obtenerListaSucursalesPorComercio")]
        public ResponseListaSucursal obtenerListaSucursalesPorComercio([FromBody]RequestSucursaComercio entradas)
        {
            ResponseListaSucursal respuesta = new ResponseListaSucursal();

            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    respuesta.ListSucursalPorComercio = contextEntity.SP_Obtener_Sucursal_Comercio(entradas.idComercio).ToList();
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

    public class HazPedidoSucursalController : BaseApiController
    {
        private readonly string _hostImagenes = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");

        [Route("api/hazPedido/ObtenerSucursalesPoligono")]
        public ResponseHazPedidoSucursal ObtenerSucursalesPoligono([FromBody]RequestHazPedidoSucursal entradas)
        {
            ResponseHazPedidoSucursal respuesta = new ResponseHazPedidoSucursal();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        int dia = (int)(DateTime.Now.DayOfWeek);
                        respuesta.respuesta = new ResponseHazPedidoDatosSucursal();
                        //respuesta.respuesta.listadoSucursales = Contexto.SP_Obtener_Sucursales_Poligono(entradas.Latitud, entradas.Longitud, dia).ToList();
                        var sucursales = Contexto.SP_Obtener_Sucursales_Poligono(entradas.Latitud, entradas.Longitud, dia).Select(s => new SPObtenerSucursalesPoligonoResult
                        {
                            idSucursal = s.idSucursal,
                            nombre = s.nombre,
                            logoUrl = _hostImagenes + s.logoUrl,
                            descripcion = s.descripcion,
                            horaInicio = s.horaInicio,
                            horaFin = s.horaFin,
                            montoMinimo = s.montoMinimo,
                            costoEnvio = s.costoEnvio,
                            abierto = s.abierto,
                            direccion = s.direccion,
                            activoPlataforma = s.activoPlataforma,
                            tieneRepartoDomicilio = s.ReparteDomicilio,
                            tieneDriveThru = s.tieneDriveThru
                        });

                        switch (entradas.RestauranteTiposReparto)
                        {

                            case TiposReparto.Domicilio:
                                respuesta.respuesta.listadoSucursales = sucursales.Where(c => c.tieneRepartoDomicilio).ToList();
                                break;
                            case TiposReparto.Sucursal:
                            case TiposReparto.Todos:
                            default:
                                respuesta.respuesta.listadoSucursales = sucursales.ToList();
                                break;
                        }

                        respuesta.estatusPeticion = RespuestaOk;
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaNoPermisos;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta;
        }

        [Route("api/hazPedido/ObtenerSucursalesActivas")]
        public ResponseHazPedidoSucursalActivas ObtenerSucursalesActivas([FromBody]RequestHazPedidoSucursalActivas entradas)
        {
            ResponseHazPedidoSucursalActivas respuesta = new ResponseHazPedidoSucursalActivas();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        int dia = (int)(DateTime.Now.DayOfWeek);
                        respuesta.respuesta = new ResponseHazPedidoDatosSucursalActiva();
                        respuesta.respuesta.listadoSucursalesActivas = Contexto.SP_Obtener_Sucursales_Activas(dia).Select(s => new SPObtenerSucursalesActivasResult
                        {
                            idSucursal = s.idSucursal,
                            nombre = s.nombre,
                            logoUrl = _hostImagenes + s.logoUrl,
                            descripcion = s.descripcion,
                            horaInicio = s.horaInicio,
                            horaFin = s.horaFin,
                            montoMinimo = s.montoMinimo ?? 0,
                            costoEnvio = s.costoEnvio ?? 0,
                            abierto = s.abierto,
                            direccion = s.direccion,
                            tieneRepartoDomicilio = s.ReparteDomicilio
                        }).ToList();

                        respuesta.estatusPeticion = RespuestaOk;
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaNoPermisos;
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