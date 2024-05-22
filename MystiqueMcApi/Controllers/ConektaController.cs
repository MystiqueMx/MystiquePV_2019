using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MystiqueMcApi.Models.Salidas;
using MystiqueMcApi.Models.Entradas;
using Openpay.Entities;
using MystiqueMC.DAL;
using Conekta;

namespace MystiqueMcApi.Controllers
{
    public class ConektaController : BaseApiController
    {
        private readonly string _mensajeDefaultOpenPay = ConfigurationManager.AppSettings.Get("MENSAJE_DEFAULT_OPENPAY");
        private readonly string _apiVersion = ConfigurationManager.AppSettings.Get("API_VERSION");
        private readonly string _apiKey = ConfigurationManager.AppSettings.Get("API_KEY");

        [Route("api/hazPedido/RegistrarTarjetaCK")]
        public ResponseAgregarTarjetasOP RegistrarTarjetaCk([FromBody]RequestOpenPay entradas)
        {
            var respuesta = new ResponseAgregarTarjetasOP();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var consumidor = Contexto.clientes.FirstOrDefault(w => w.idCliente == entradas.consumidorId);

                        if (consumidor != null && !consumidor.ConsumidoresConekta.Any())
                        {
                            //Crear consumidor en Conekta

                            string customerId = CreateCustomerConekta(consumidor.nombre + ' ' + consumidor.paterno + ' ' + consumidor.materno, consumidor.email, _apiKey, _apiVersion);

                            if (customerId != null)
                            {
                                Contexto.ConsumidoresConekta.Add(new ConsumidoresConekta
                                {
                                    consumidorId = entradas.consumidorId,
                                    customerId = customerId,
                                    tokenId = entradas.tokenId,
                                    activo = true,
                                    fechaRegistro = DateTime.Now,
                                });

                                Contexto.SaveChanges();

                                if (AddCustomerPaymentSource(customerId, entradas.tokenId, _apiKey, _apiVersion, out var paymentsource))
                                {
                                    var datosTdc = new Card
                                    {
                                        Id = paymentsource.id,
                                        Type = paymentsource.type,
                                        TokenId = paymentsource.token_id,
                                        ExpirationMonth = paymentsource.exp_month,
                                        ExpirationYear = paymentsource.exp_year,
                                        Cvv2 = paymentsource.cvc,
                                        HolderName = paymentsource.name,
                                        CardNumber = entradas.cardNumber,
                                        AllowsCharges = false,
                                        BankCode = string.Empty,
                                        BankName = string.Empty,
                                        Brand = string.Empty,
                                        CreationDate = DateTime.Now,
                                        AllowsPayouts = false,
                                    };

                                    Contexto.ConsumidorConektaTarjetas.Add(new ConsumidorConektaTarjetas
                                    {
                                        consumidorId = entradas.consumidorId,
                                        customerId = customerId,
                                        tarjetaId = paymentsource.id,
                                        numeroTarjeta = entradas.cardNumber,
                                        ultimosDigitos = string.Empty,
                                        nombreBanco = string.Empty,
                                        nombreHabiente = entradas.holderName,
                                        tipo = entradas.brand,
                                        activo = true,
                                        fechaRegistro = DateTime.Now,

                                    });

                                    Contexto.SaveChanges();

                                    respuesta.respuesta = datosTdc;
                                    respuesta.estatusPeticion = RespuestaOk;

                                    
                                }
                                else
                                {
                                    respuesta.estatusPeticion = RespuestaErrorValidacion((_mensajeDefaultOpenPay));
                                }
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion((_mensajeDefaultOpenPay));
                            }
                        }
                        else
                        {
                            //añadir tarjeta la consumidor

                            var customerId = consumidor.ConsumidoresConekta.First().customerId;

                            if (consumidor != null && AddCustomerPaymentSource(consumidor.ConsumidoresConekta.ElementAt(0).customerId, entradas.tokenId, _apiKey, _apiVersion, out var paymentsource))
                            {
                                var datosTdc = new Card
                                {
                                    Id = paymentsource.id,
                                    Type = paymentsource.type,
                                    TokenId = paymentsource.token_id,
                                    ExpirationMonth = paymentsource.exp_month,
                                    ExpirationYear = paymentsource.exp_year,
                                    Cvv2 = paymentsource.cvc,
                                    HolderName = paymentsource.name,
                                    CardNumber = entradas.cardNumber,
                                    AllowsCharges = false,
                                    BankCode = string.Empty,
                                    BankName = string.Empty,
                                    Brand = string.Empty,
                                    CreationDate = DateTime.Now,
                                    AllowsPayouts = false,
                                };

                                Contexto.ConsumidorConektaTarjetas.Add(new ConsumidorConektaTarjetas
                                {
                                    consumidorId = entradas.consumidorId,
                                    customerId = customerId,
                                    tarjetaId = paymentsource.id,
                                    numeroTarjeta = entradas.cardNumber,
                                    ultimosDigitos = string.Empty,
                                    nombreBanco = string.Empty,
                                    nombreHabiente = entradas.holderName,
                                    tipo = entradas.brand,
                                    activo = true,
                                    fechaRegistro = DateTime.Now,

                                });

                                Contexto.SaveChanges();

                                respuesta.respuesta = datosTdc;
                                respuesta.estatusPeticion = RespuestaOk;
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion((_mensajeDefaultOpenPay));
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
                    respuesta.estatusPeticion = RespuestaNoPermisos;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            CreateTransaccionConekta("RegistrarTarjetaCK", Newtonsoft.Json.JsonConvert.SerializeObject(entradas), Newtonsoft.Json.JsonConvert.SerializeObject(respuesta));
            return respuesta;
        }

        [Route("api/hazPedido/ListadoTarjetasCK")]
        public ResponseListadoTarjetasOP ListadoTarjetasCk([FromBody]RequestOpenPayListadoTarjetas entradas)
        {
            var respuesta = new ResponseListadoTarjetasOP();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        conekta.Api.apiKey = _apiKey;
                        conekta.Api.version = _apiVersion;

                        var listadoTdc = new List<Card>();

                        var consumidorCk = Contexto.clientes.Where(c => c.idCliente == entradas.consumidorId).Select(c => c.ConsumidoresConekta).FirstOrDefault();
                        if (consumidorCk != null && consumidorCk.Any())
                        {
                            var customerId = consumidorCk.First().customerId;

                            // var customer = new conekta.Customer().find(customerId);

                            listadoTdc.AddRange(Contexto.ConsumidorConektaTarjetas.Where(w => w.customerId == customerId).Select(paymentsource => new Card
                            {
                                Id = paymentsource.tarjetaId,
                                Type = paymentsource.tipo,
                                TokenId = string.Empty,
                                ExpirationMonth = string.Empty,
                                ExpirationYear = string.Empty,
                                Cvv2 = string.Empty,
                                HolderName = paymentsource.nombreHabiente,
                                CardNumber = paymentsource.numeroTarjeta,
                                AllowsCharges = false,
                                BankCode = string.Empty,
                                BankName = string.Empty,
                                Brand = string.Empty,
                                CreationDate = DateTime.Now,
                                AllowsPayouts = false,

                            }));

                            respuesta.respuesta = listadoTdc;
                        }
                        else
                        {
                            respuesta.respuesta = listadoTdc;

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
            CreateTransaccionConekta("ListadoTarjetasCk", Newtonsoft.Json.JsonConvert.SerializeObject(entradas), Newtonsoft.Json.JsonConvert.SerializeObject(respuesta));
            return respuesta;
        }

        [Route("api/hazPedido/EliminarTarjetaCK")]
        public ResponseOpenPay EliminarTarjetaCk([FromBody]RequestOpenPayEliminarTarjeta entradas)
        {
            ResponseOpenPay respuesta = new ResponseOpenPay();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var consumidor = Contexto.clientes.FirstOrDefault(w => w.idCliente == entradas.consumidorId);

                        if (consumidor != null && consumidor.ConsumidoresConekta.Any())
                        {
                            var consumerId = consumidor.ConsumidoresConekta.ElementAt(0).customerId;
                            if (RemoveCustomerPaymentSource(consumerId, entradas.tokenId, _apiKey, _apiVersion))
                            {
                                ConsumidorConektaTarjetas tarjetas = Contexto.ConsumidorConektaTarjetas.Where(w => w.customerId == consumerId && w.tarjetaId == entradas.tokenId).FirstOrDefault();
                                if (tarjetas != null)
                                {
                                    Contexto.ConsumidorConektaTarjetas.Remove(tarjetas);
                                }
                                respuesta.estatusPeticion = RespuestaOk;
                            }
                            else
                            {
                                respuesta.estatusPeticion = RespuestaErrorValidacion(_mensajeDefaultOpenPay);
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
                    respuesta.estatusPeticion = RespuestaNoPermisos;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            CreateTransaccionConekta("EliminarTarjetaCK", Newtonsoft.Json.JsonConvert.SerializeObject(entradas), Newtonsoft.Json.JsonConvert.SerializeObject(respuesta));
            return respuesta;
        }

        internal string CreateCustomerConekta(string nombre, string email, string key, string api)
        {
            string respuesta = null;

            try
            {
                conekta.Api.apiKey = key;
                conekta.Api.version = api;

                var customer = new conekta.Customer { name = nombre, email = email };


                var customer2 = new conekta.Customer().create(customer.toJSON());
                respuesta = customer2.id;

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return respuesta;
        }

        internal string CreateOrderConekta(string customerId, decimal unitPrice, string tokenId, string nombreContacto, string direccionContacto, string telefonoContacto, decimal cobroEnvio, string portador, string codigoPostal, string colonia, string ciudad, string estado)
        {
            var respuesta = string.Empty;

            Logger.Debug($"customerId:{customerId},unitPrice:{unitPrice},---INICIO");
            try
            {
                conekta.Api.apiKey = _apiKey;
                conekta.Api.version = _apiVersion;

                var customer = new conekta.Customer().find(customerId);
                customer.update(@"{
                    ""default_payment_source_id"": """ + tokenId + @"""
                }");
                //PaymentSource payment = customer.payment_sources.Where(ps => ps.token_id == tokenId).FirstOrDefault();                

                if (bool.Parse(ConfigurationManager.AppSettings.Get("ISMONTOPRUEBA")))
                {
                    unitPrice = decimal.Parse(ConfigurationManager.AppSettings.Get("MONTOPRUEBA"));
                    cobroEnvio = 1;
                }


                var orden = new conekta.Order().create(@"{
                  ""currency"":""MXN"",
                  ""pre_authorize"":true,
                  ""customer_info"": {
                    ""customer_id"": """ + customerId + @"""
                  },
                  ""line_items"": [{
                    ""name"": ""Pedido"",
                    ""unit_price"": " + (int)unitPrice * 100 + @",
                    ""quantity"": 1
                  }],
                  ""charges"": [{
                     ""payment_method"": {
                        ""type"": ""default""
                       }
                  }],
                  ""shipping_lines"": [{
                      ""amount"": " + (int)cobroEnvio * 100 + @",
                      ""carrier"": """ + portador + @"""
                  }],
                  ""shipping_contact"":{
                     ""receiver"": """ + nombreContacto + @""",
                     ""phone"": """ + telefonoContacto + @""",
                     ""address"": {
                       ""street1"": """ + direccionContacto + @""",                       
                       ""city"": """ + ciudad + @""",
                       ""state"": """ + estado + @""",
                       ""postal_code"": """ + codigoPostal + @""",
                       ""country"": ""MX""
                     }
                   }
                }");

                Logger.Debug($"customerId:{customerId},unitPrice:{unitPrice},---TERMINO ORDEN : {respuesta}");

                respuesta = orden.id;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return respuesta;
        }

        //Metodo para agregar la tarjeta al cliente
        internal bool AddCustomerPaymentSource(string customerId, string token, string key, string api, out PaymentSource source)
        {
            var respuesta = false;
            try
            {
                conekta.Api.apiKey = key;
                conekta.Api.version = api;
                var customer = new conekta.Customer().find(customerId);
                var paymentsource = new PaymentSource
                {
                    type = "card",
                    token_id = token
                };

                source = customer.createPaymentSource(paymentsource.toJSON());
                respuesta = true;
            }
            catch (Exception e)
            {
                source = new PaymentSource();
                Logger.Error(e);
            }
            return respuesta;
        }

        //Metodo para registrar en la bitacora de transacciones
        internal bool CreateTransaccionConekta(string transsacion, string entrada, string salida)
        {
            bool respuesta = false;

            try
            {
                Contexto.TransaccionesConekta.Add(new TransaccionesConekta
                {
                    transaccion = transsacion,
                    entrada = entrada,
                    salida = salida,
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

        internal bool RemoveCustomerPaymentSource(string customerId, string tokenId, string key, string api)
        {
            var respuesta = false;
            try
            {
                conekta.Api.apiKey = key;
                conekta.Api.version = api;

                var customer = new conekta.Customer().find(customerId);
                var payment = customer.payment_sources.FirstOrDefault(ps => ps.id == tokenId);
                if (payment == null) return false;
                var source = payment.destroy();

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
