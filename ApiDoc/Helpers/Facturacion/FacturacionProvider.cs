using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using Hangfire;
using ApiDoc.Helpers.Facturacion.Service;
using ApiDoc.Models;
using MystiqueMC.DAL;
using Newtonsoft.Json;
using ApiDoc.Helpers.Facturacion.Models.Requests.Restaurante;

namespace ApiDoc.Helpers.Facturacion
{
    public class FacturacionProvider
    {
//        [AutomaticRetry(Attempts = 0)]
//        public static async Task SolicitarFactura(Factura factura, string directorioAplicacion)
//        {
//            factura.Id = Guid.Parse(factura.SerializedId);
//            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//            try
//            {
//                using (var contexto = new MystiqueMeEntities())
//                {
//                    #if DEBUG
//                    contexto.Database.Log = sql => Trace.WriteLine(sql);
//                    #endif
//                    logger.Debug(
//                        $"IdFactura : {factura.Id}, requestBody: {JsonConvert.SerializeObject(factura)}");
                    
//                    var dbFacturaCliente = contexto.facturaCliente.Find(factura.Id);
//                    if(dbFacturaCliente == null) return;
//                    if ((EstatusFactura) dbFacturaCliente.catEstatusFacturaId == EstatusFactura.Pendiente)
//                    {
//                        dbFacturaCliente.catEstatusFacturaId = (int)EstatusFactura.Procesando;
//                        contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
//                        contexto.SaveChanges();
//                        await ProcesarFactura(factura, dbFacturaCliente, contexto, directorioAplicacion);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                #if DEBUG
//                Trace.WriteLine(e);
//                #endif
//                logger.Error(e);
//                using (var contexto = new MystiqueMeEntities())
//                {
//                    var dbFacturaCliente = contexto.facturaCliente.Find(factura.Id);
//                    if (dbFacturaCliente != null)
//                    {
//                        dbFacturaCliente.catEstatusFacturaId = (int) EstatusFactura.Error;
//                        contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
//                        contexto.SaveChanges();
//                    }
                    
//                }
//            }
//        }
//        [AutomaticRetry(Attempts = 0)]
//        public static Task SolicitarFacturas(List<Factura> facturas, string directorioAplicacion)
//        {
//            return Task.Factory.StartNew(async () =>
//            {
//                var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//                foreach (var factura in facturas)
//                {
//                    try
//                    {
//                        using (var contexto = new MystiqueMeEntities())
//                        {
//#if DEBUG
//                            contexto.Database.Log = sql => Trace.WriteLine(sql);
//#endif
//                            logger.Debug(
//                                $"IdFactura : {factura.Id}, requestBody: {JsonConvert.SerializeObject(factura)}");
            
//                            var dbFacturaCliente = contexto.facturaCliente.Find(factura.Id);
//                            if(dbFacturaCliente == null) continue;
//                            if ((EstatusFactura) dbFacturaCliente.catEstatusFacturaId != EstatusFactura.Pendiente)
//                                continue;

//                            dbFacturaCliente.catEstatusFacturaId = (int)EstatusFactura.Procesando;
//                            contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
//                            contexto.SaveChanges();
//                            await ProcesarFactura(factura, dbFacturaCliente, contexto, directorioAplicacion);
//                        }
//                    }
//                    catch (Exception e)
//                    {
//#if DEBUG
//                        Trace.WriteLine(e);
//#endif
//                        logger.Error(e);
//                        using (var contexto = new MystiqueMeEntities())
//                        {
//                            var dbFacturaCliente = contexto.facturaCliente.Find(factura.Id);
//                            if (dbFacturaCliente == null) continue;

//                            dbFacturaCliente.catEstatusFacturaId = (int) EstatusFactura.Error;
//                            contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
//                            contexto.SaveChanges();

//                        }
//                    }
//                }
                
//            });
//        }

//        public static Task ProcesarFactura(Factura factura, facturaCliente dbFacturaCliente, MystiqueMeEntities contexto, string directorioAplicacion)
//        {
//            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            
//            var delegateFacturacion = new FacturacionApiV1.Restaurantes();
//            var response = delegateFacturacion.CallFacturarTicket(factura); // Solicitud de factura

//            logger.Debug(
//                $"IdFactura : {factura.Id}, ResponseCode:{response.ResponseCode}, message: {response.Message}, cadena: {response.CadenaOriginal}");
//            if (response.IsSuccessful)
//            {
//                contexto.bitacoraFacturacion.Add(new bitacoraFacturacion
//                {
//                    facturaClienteId = factura.Id,
//                    codigoRespuesta = (int) response.ResponseCode,
//                    mensajeRespuesta = response.CadenaOriginal
//                });
//            }
//            else
//            {
//                contexto.bitacoraFacturacion.Add(new bitacoraFacturacion
//                {
//                    facturaClienteId = factura.Id,
//                    codigoRespuesta = (int) response.ResponseCode,
//                    mensajeRespuesta = response.Message,
//                });
//            }
            
//            if (response.Id != Guid.Empty)
//            {
//                logger.Debug(
//                    $"IdFactura : {factura.Id}, Actualizando Id en facturacion : {response.Id}");
//                dbFacturaCliente.originalFacturacion = response.Id; // Si tiene ID en facturacion algo fallo en api facturas
//                dbFacturaCliente.catEstatusFacturaId = (int)EstatusFactura.Error;
//                contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
//            }

//            if (response.IsSuccessful)
//            {
//                logger.Debug(
//                    $"IdFactura : {factura.Id}, Actualizando cadena original y factura : {response.CadenaOriginal}");
//                dbFacturaCliente.catEstatusFacturaId = (int)EstatusFactura.ListaParaEnviar;
//                dbFacturaCliente.cadenaOriginal = response.CadenaOriginal;
//                dbFacturaCliente.documentoTimbrado = response.FacturaTimbrada;

//                contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
//            }

//            contexto.SaveChanges();
            
//            if (response.IsSuccessful)
//            {
//                Hangfire.BackgroundJob.Enqueue(() => PostFacturacionProvider.Procesar(factura, directorioAplicacion));
//            }

//            return Task.CompletedTask;
//        }

        
    }
}
