using MystiqueMC.DAL;
using MystiqueMcApi.Helpers.OpenPay;
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
    public class OpenPayController : BaseApiController
    {
        private readonly string _mensajeDefaultOpenPay = ConfigurationManager.AppSettings.Get("MENSAJE_DEFAULT_OPENPAY");

        [Route("api/RegistrarTarjetaOP")]
        public ResponseAgregarTarjetasOP RegistrarTarjetaOP([FromBody]RequestOpenPay entradas)
        {
            ResponseAgregarTarjetasOP respuesta = new ResponseAgregarTarjetasOP();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        EnviarLlamadosOpenPay ejecutarOp = new EnviarLlamadosOpenPay();

                        var consumidor = Contexto.clientes.Where(w => w.idCliente == entradas.consumidorId).FirstOrDefault();

                        if (consumidor.ConsumidorOpenPay.Count() == 0)
                        {
                            var respuestaRegistrarCliente = ejecutarOp.RegistraClienteOP(consumidor.idCliente, consumidor.nombre, consumidor.paterno + ' ' + consumidor.materno, consumidor.email, consumidor.telefono);
                            InsertarBitacoraTransaccionOpenPay(entradas.consumidorId, 1, respuestaRegistrarCliente.datosEnvio, respuestaRegistrarCliente.datosRespuesta, respuestaRegistrarCliente.codigoError, respuestaRegistrarCliente.descripcionError);
                            InsertarRelacionConsumidorOpenPay(entradas.consumidorId, respuestaRegistrarCliente.customerId);
                            var resultadoCrearTarjetaToken = ejecutarOp.CrearTarjetaToken(entradas.consumidorId, respuestaRegistrarCliente.customerId, entradas.tokenId, entradas.deviceSesionId);
                            InsertarBitacoraTransaccionOpenPay(entradas.consumidorId, 2, resultadoCrearTarjetaToken.datosEnvio, resultadoCrearTarjetaToken.datosRespuesta, resultadoCrearTarjetaToken.codigoError, resultadoCrearTarjetaToken.descripcionError);
                            if (respuestaRegistrarCliente.resultado && resultadoCrearTarjetaToken.resultado)
                            {
                                respuesta.respuesta = resultadoCrearTarjetaToken.datosTarjeta;
                                respuesta.estatusPeticion = RespuestaOk;
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion((_mensajeDefaultOpenPay));
                                //respuesta.estatusPeticion = RespuestaErrorValidacion(respuestaRegistrarCliente.descripcionError + ' ' + resultadoCrearTarjetaToken.descripcionError);
                            }
                        }
                        else
                        {
                            var resultadoCrearTarjetaToken = ejecutarOp.CrearTarjetaToken(entradas.consumidorId, consumidor.ConsumidorOpenPay.ElementAt(0).customerId, entradas.tokenId, entradas.deviceSesionId);
                            InsertarBitacoraTransaccionOpenPay(entradas.consumidorId, 2, resultadoCrearTarjetaToken.datosEnvio, resultadoCrearTarjetaToken.datosRespuesta, resultadoCrearTarjetaToken.codigoError, resultadoCrearTarjetaToken.descripcionError);
                            if (resultadoCrearTarjetaToken.resultado)
                            {
                                respuesta.respuesta = resultadoCrearTarjetaToken.datosTarjeta;
                                respuesta.estatusPeticion = RespuestaOk;
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion((_mensajeDefaultOpenPay));
                                //respuesta.estatusPeticion = RespuestaErrorValidacion(resultadoCrearTarjetaToken.descripcionError);
                            }
                        }
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



        [Route("api/ListadoTarjetasOP")]
        public ResponseListadoTarjetasOP ListadoTarjetasOP([FromBody]RequestOpenPayListadoTarjetas entradas)
        {
            ResponseListadoTarjetasOP respuesta = new ResponseListadoTarjetasOP();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        EnviarLlamadosOpenPay ejecutarOp = new EnviarLlamadosOpenPay();

                        var consumidor = Contexto.clientes.Where(w => w.idCliente == entradas.consumidorId).FirstOrDefault();

                        if (consumidor.ConsumidorOpenPay.Count() > 0)
                        {
                            var respuestaObtenerT = ejecutarOp.ObtenerListadoTarjetas(consumidor.ConsumidorOpenPay.ElementAt(0).customerId);
                            respuesta.respuesta = respuestaObtenerT.listaTarjetas;
                            InsertarBitacoraTransaccionOpenPay(entradas.consumidorId, 3, respuestaObtenerT.datosEnvio, respuestaObtenerT.datosRespuesta, respuestaObtenerT.codigoError, respuestaObtenerT.descripcionError);

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




        [Route("api/RegistrarCargoOP")]
        public ResponseOpenPay RegistrarCargoOP([FromBody]RequestOpenPayRegistrarCargo entradas)
        {
            ResponseOpenPay respuesta = new ResponseOpenPay();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        EnviarLlamadosOpenPay ejecutarOp = new EnviarLlamadosOpenPay();

                        var consumidor = Contexto.clientes.Where(w => w.idCliente == entradas.consumidorId).FirstOrDefault();

                        if (consumidor.ConsumidorOpenPay.Count() > 0)
                        {
                            var respuestaObtenerT = ejecutarOp.AplicarCargoCliente(consumidor.ConsumidorOpenPay.ElementAt(0).customerId, entradas.sourceId, entradas.monto, entradas.descripcion, entradas.ordenId, entradas.deviceSesionId);
                            InsertarBitacoraTransaccionOpenPay(entradas.consumidorId, 4, respuestaObtenerT.datosEnvio, respuestaObtenerT.datosRespuesta, respuestaObtenerT.codigoError, respuestaObtenerT.descripcionError);
                            if (respuestaObtenerT.resultado)
                            {
                                respuesta.estatusPeticion = RespuestaOk;
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion(respuestaObtenerT.descripcionError);
                            }
                        }
                        else
                        {
                            respuesta.estatusPeticion = RespuestaErrorValidacion("No se encontro consumerId registrado.");
                        }
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



        [Route("api/EliminarTarjetaOP")]
        public ResponseOpenPay EliminarTarjetaOP([FromBody]RequestOpenPayEliminarTarjeta entradas)
        {
            ResponseOpenPay respuesta = new ResponseOpenPay();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        EnviarLlamadosOpenPay ejecutarOp = new EnviarLlamadosOpenPay();

                        var consumidor = Contexto.clientes.Where(w => w.idCliente == entradas.consumidorId).FirstOrDefault();

                        if (consumidor.ConsumidorOpenPay.Count() > 0)
                        {
                            var respuestaObtenerT = ejecutarOp.EliminarTarjeta(consumidor.ConsumidorOpenPay.ElementAt(0).customerId, entradas.tokenId);
                            InsertarBitacoraTransaccionOpenPay(entradas.consumidorId, 5, respuestaObtenerT.datosEnvio, respuestaObtenerT.datosRespuesta, respuestaObtenerT.codigoError, respuestaObtenerT.descripcionError);
                            if (respuestaObtenerT.resultado)
                            {
                                respuesta.estatusPeticion = RespuestaOk;
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion(respuestaObtenerT.descripcionError);
                            }
                        }
                        else
                        {
                            respuesta.estatusPeticion = RespuestaErrorValidacion("No se encontro consumerId registrado.");
                        }
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




        private bool InsertarRelacionConsumidorOpenPay(int consumidorId, string customerId)
        {
            bool respuesta = false;

            try
            {
                Contexto.ConsumidorOpenPay.Add(new ConsumidorOpenPay
                {
                    consumidorId = consumidorId,
                    customerId = customerId,
                    activo = true,
                    fechaRegistro = DateTime.Now,

                });

                Contexto.SaveChanges();
                respuesta = true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return respuesta;
        }


        internal bool InsertarBitacoraTransaccionOpenPay(int consumidorId, int catTipoTransaccion, string datosEnvio, string datosRespuesta, int codigoError, string desError)
        {
            bool respuesta = false;

            try
            {
                Contexto.OpenPayTransacciones.Add(new OpenPayTransacciones
                {
                    consumidorId = consumidorId,
                    catTransaccionOpenPayId = catTipoTransaccion,
                    fechaRegistro = DateTime.Now,
                    estatusProceso = true,
                    datosEnvio = datosEnvio,
                    datosRespuesta = datosRespuesta,
                    catCodigosErroresOpenPayId = codigoError,
                    descripcionError = desError,
                });

                Contexto.SaveChanges();
                respuesta = true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return respuesta;
        }
    }
}
