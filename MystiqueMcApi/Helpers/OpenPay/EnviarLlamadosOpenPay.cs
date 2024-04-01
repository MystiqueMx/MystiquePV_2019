using log4net;
using System.Configuration;
using Openpay;
using Openpay.Entities;
using MystiqueMcApi.Helpers.OpenPay.Modelos;
using Openpay.Entities.Request;
using System.Collections.Generic;
using System;

namespace MystiqueMcApi.Helpers.OpenPay
{
    public class EnviarLlamadosOpenPay
    {
        private readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        readonly string OPENPAY_PRIVATEKEY = ConfigurationManager.AppSettings.Get("PRIVATEKEY");

        readonly string OPENPAY_MERCHANTID = ConfigurationManager.AppSettings.Get("MERCHANTID");


        public RespuestaEjecucionRegistraClienteOP RegistraClienteOP(int consumidorId, string nombre, string apellidos, string email, string telefono)
        {
            RespuestaEjecucionRegistraClienteOP respuesta = new RespuestaEjecucionRegistraClienteOP();

            try
            {
                OpenpayAPI api = new OpenpayAPI(OPENPAY_PRIVATEKEY, OPENPAY_MERCHANTID);
                api.Production = bool.Parse(ConfigurationManager.AppSettings.Get("PRODUCTION"));
                Customer request = new Customer();
                request.ExternalId = consumidorId.ToString();
                request.Name = nombre;
                request.LastName = apellidos.Trim();
                request.Email = email;
                request.PhoneNumber = telefono;
                request.RequiresAccount = false;

                respuesta.datosEnvio = request.ToJson();
                request = api.CustomerService.Create(request);
                respuesta.datosRespuesta = request.ToJson();
                respuesta.customerId = request.Id;
                respuesta.resultado = true;
            }
            catch (OpenpayException e)
            {
                Logger.Error(e);
                respuesta.codigoError = e.ErrorCode;
                respuesta.descripcionError = e.Description;
                respuesta.resultado = false;

            }
            return respuesta;
        }

        public RespuestaEjecucionCrearTarjetaToken CrearTarjetaToken(int consumidorId, string customerId, string tokenId, string deviceSessionId)
        {
            RespuestaEjecucionCrearTarjetaToken respuesta = new RespuestaEjecucionCrearTarjetaToken();

            try
            {
                OpenpayAPI api = new OpenpayAPI(OPENPAY_PRIVATEKEY, OPENPAY_MERCHANTID);
                api.Production = bool.Parse(ConfigurationManager.AppSettings.Get("PRODUCTION"));
                Card request = new Card();
                request.TokenId = tokenId;
                request.DeviceSessionId = deviceSessionId;
                Card tarjetaAgregada = new Card();

                respuesta.datosEnvio = request.ToJson();

                tarjetaAgregada = api.CardService.Create(customerId, request);
                respuesta.datosRespuesta = tarjetaAgregada.ToJson();
                respuesta.datosTarjeta = tarjetaAgregada;
                respuesta.resultado = true;
                //respuesta.idTarjeta = request.Id;

            }
            catch (OpenpayException e)
            {
                Logger.Error(e);
                respuesta.codigoError = e.ErrorCode;
                respuesta.descripcionError = e.Description;
                respuesta.resultado = false;
            }
            return respuesta;
        }



        public RespuestaEjecucionListadoTarjeta ObtenerListadoTarjetas(string customerId)
        {
            RespuestaEjecucionListadoTarjeta respuesta = new RespuestaEjecucionListadoTarjeta();

            try
            {
                OpenpayAPI api = new OpenpayAPI(ConfigurationManager.AppSettings.Get("PRIVATEKEY"), ConfigurationManager.AppSettings.Get("MERCHANTID"));
                api.Production = bool.Parse(ConfigurationManager.AppSettings.Get("PRODUCTION"));
                SearchParams request = new SearchParams();
                request.Offset = 0;
                request.Limit = 10;
                respuesta.datosEnvio = "";

                List<Card> cards = api.CardService.List(customerId, request);

                respuesta.listaTarjetas = cards;
                respuesta.datosRespuesta = "";
                respuesta.resultado = true;

            }
            catch (OpenpayException e)
            {
                Logger.Error(e);
                respuesta.codigoError = e.ErrorCode;
                respuesta.descripcionError = e.Description;
                respuesta.resultado = false;
            }
            return respuesta;
        }


        public RespuestaEjecucionAplicarCargo AplicarCargoCliente(string customerId, string sourceId, decimal monto, string descripcion, string ordenId, string deviceSessionID)
        {
            RespuestaEjecucionAplicarCargo respuesta = new RespuestaEjecucionAplicarCargo();

            try
            {
                OpenpayAPI api = new OpenpayAPI(ConfigurationManager.AppSettings.Get("PRIVATEKEY"), ConfigurationManager.AppSettings.Get("MERCHANTID"));
                api.Production = bool.Parse(ConfigurationManager.AppSettings.Get("PRODUCTION"));


                ChargeRequest request = new ChargeRequest();

                request.Method = "card";
                request.SourceId = sourceId;
                if (bool.Parse(ConfigurationManager.AppSettings.Get("ISMONTOPRUEBA")))
                {
                    request.Amount = decimal.Parse(ConfigurationManager.AppSettings.Get("MONTOPRUEBA"));
                }
                else
                {
                    request.Amount = monto;
                }
                request.Currency = "MXN";
                request.Description = descripcion;
                request.OrderId = ordenId;
                request.DeviceSessionId = deviceSessionID;

                respuesta.datosEnvio = request.ToJson();
                api.ChargeService.Create(customerId, request);

                respuesta.datosRespuesta = request.ToJson();
                respuesta.resultado = true;

            }
            catch (OpenpayException e)
            {
                Logger.Error(e);
                respuesta.codigoError = e.ErrorCode;
                respuesta.descripcionError = e.Description;
                respuesta.resultado = false;
            }
            return respuesta;
        }



        public RespuestaEjecucionEliminarTarjeta EliminarTarjeta(string customerId, string cardId)
        {
            RespuestaEjecucionEliminarTarjeta respuesta = new RespuestaEjecucionEliminarTarjeta();

            try
            {
                OpenpayAPI api = new OpenpayAPI(ConfigurationManager.AppSettings.Get("PRIVATEKEY"), ConfigurationManager.AppSettings.Get("MERCHANTID"));
                api.Production = bool.Parse(ConfigurationManager.AppSettings.Get("PRODUCTION"));
                respuesta.datosEnvio = "{ customerId=" + customerId + ", cardId=" + cardId + "}";
                api.CardService.Delete(customerId, cardId);

                respuesta.datosRespuesta = "";
                respuesta.resultado = true;

            }
            catch (OpenpayException e)
            {
                Logger.Error(e);
                respuesta.codigoError = e.ErrorCode;
                respuesta.descripcionError = e.Description;
                respuesta.resultado = false;
            }
            return respuesta;
        }

    }
}