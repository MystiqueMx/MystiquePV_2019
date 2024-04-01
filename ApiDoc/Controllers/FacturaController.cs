using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Web;
using Hangfire;
using ApiDoc.Helpers.Facturacion;
using ApiDoc.Models;

namespace ApiDoc.Controllers
{
    public class FacturaController : BaseApiController
   {
//        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
//        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//        private PermisosApi validar = new PermisosApi();
//        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
//        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
//        readonly string MENSAJE_FACTURA_USADA = "MYSTIQUE_MENSAJE_FACTURA_USADA";
//        readonly string MENSAJE_FACTURA_SOLICITADA = "MYSTIQUE_MENSAJE_FACTURA_SOLICITADA";


//        [Route("api/obtenerCatUsoCFDI")]
//        public ResponseListaCatUsoCFDI obtenerCatUsoCFDI()
//        {
//            ResponseListaCatUsoCFDI respuesta = new ResponseListaCatUsoCFDI();

//            try
//            {
//                respuesta.Success = true;
//                respuesta.ErrorMessage = "";
//                respuesta.catUsoCFDI = obtenerCatalogoCFDI();
//            }
//            catch (Exception e)
//            {
//                logger.Error("ERROR:" + e.Message);
//                respuesta.Success = false;
//                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
//            }
//            return respuesta;
//        }


//        [Route("api/obtenerMisDatosFiscales")]
//        public ResponseListaDatosFiscalesReceptor obtenerMisDatosFiscales([FromBody]RequestFactura entradas)
//        {
//            ResponseListaDatosFiscalesReceptor respuesta = new ResponseListaDatosFiscalesReceptor();

//            try
//            {
//                if (validar.IsAppSecretValid)
//                {
//                    var datosFiscaleReceptor = obtenerDatosRecptorByCliente(entradas.clienteId);
//                    respuesta.Success = true;
//                    respuesta.ErrorMessage = "";
//                    respuesta.listaDatosFiscalesReceptor = datosFiscaleReceptor;
//                }
//                else
//                {
//                    respuesta.Success = false;
//                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
//                }
//            }
//            catch (Exception e)
//            {
//                logger.Error("ERROR:" + e.Message);
//                respuesta.Success = false;
//                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
//            }
//            return respuesta;
//        }



//        [Route("api/validarTicket")]
//        public ResponseValidarTicket validarTicket([FromBody]RequestValidarTicket entrada)
//        {
//            ResponseValidarTicket respuesta = new ResponseValidarTicket();
//            try
//            {
//                if (validar.IsAppSecretValid)
//                {
//                    Encryption64.Encryption64 encripta = new Encryption64.Encryption64();
//                    Char delimiterCodigo = '|';
//                    string codigoEntrada = encripta.Decrypt(entrada.codigoGenerado, ConfigurationManager.AppSettings.Get("tipoCodificacion"));
//                    // 0-ticket , 1-fechaCompra , 2-monto , 3-sucursal, 4-referencia
                    
//                    String[] resultadosCodigo = codigoEntrada.Split(delimiterCodigo);
//                    int sucursalPV = Int32.Parse(resultadosCodigo[3]);

//                    var sucursal = contextEntity.sucursales.Where(w => w.sucursalPuntoVenta == sucursalPV).FirstOrDefault();
//                    if (sucursal != null)
//                    {
//                        var ticketId = resultadosCodigo[0];
//                        var sucursalId = int.Parse(resultadosCodigo[3]);
//                        var totalT = decimal.Parse(resultadosCodigo[2]);
//                        var ticket =  contextEntity.ticketSucursal
//                            .FirstOrDefault(w => w.folio == ticketId
//                                && w.sucursales.sucursalPuntoVenta == sucursalId
//                                && w.total == totalT);
                        
//                       if(ticket !=null)
//                        {
//                            if (ValidarFechaTicket(ticket.fechaCompra))
//                            {
//                                switch ((EstatusTicket) ticket.catEstatusTicketId)
//                                {
//                                    case EstatusTicket.Nuevo : 
//                                        respuesta.ticket = ticket.folio;
//                                        respuesta.fechaCompra = ticket.fechaCompra;
//                                        respuesta.montoCompra = ticket.total;
//                                        respuesta.sucursal = sucursal.nombre;
//                                        respuesta.Success = true;
//                                        respuesta.ErrorMessage = "";
//                                        respuesta.listaDatosFiscalesReceptor = obtenerDatosRecptorByCliente(entrada.clienteId);
//                                        respuesta.catUsoCFDI = obtenerCatalogoCFDI();
//                                        respuesta.PendienteTicket = false;
//                                        respuesta.sucursalId = sucursal.idSucursal;
//                                        break;
//                                        case EstatusTicket.Facturado:
//                                            respuesta.Success = true;
//                                            respuesta.ErrorMessage = "Lo sentimos, el ticket que capturó ya ha sido facturado";
//                                            break;
//                                        default:
//                                            respuesta.Success = false;
//                                            respuesta.ErrorMessage = "Lo sentimos, para facturar este ticket es necesario que acudas a tu sucursal.";
//                                            break;
                                   
//                                }
//                            }
//                            else
//                            {
//                                respuesta.Success = false; 
//                                respuesta.ErrorMessage = "Lo sentimos, para facturar este ticket es necesario que acudas a tu sucursal.";
//                            }
                            
//                        }
//                       else
//                       {
//                           if (contextEntity.facturaPendiente.Any(c => c.ticket == ticketId))
//                           {
//                               return new ResponseValidarTicket
//                               {
//                                   Success = false,
//                                   ErrorMessage = ConfigurationManager.AppSettings[MENSAJE_FACTURA_USADA],
//                               };
//                           }
//                           else
//                           {
//                               respuesta.ticket = ticketId;
//                               respuesta.fechaCompra = DateTime.Now;
//                               respuesta.montoCompra = totalT;
//                               respuesta.sucursal = sucursal.nombre;
//                               respuesta.Success = true;
//                               respuesta.ErrorMessage = "";
//                               respuesta.listaDatosFiscalesReceptor = obtenerDatosRecptorByCliente(entrada.clienteId);
//                               respuesta.catUsoCFDI = obtenerCatalogoCFDI();
//                               respuesta.PendienteTicket = true;
//                               respuesta.sucursalId = sucursal.idSucursal;
//                           }
//                        }
//                    }
//                    else
//                    {
//                        respuesta.Success = false; 
//                        respuesta.ErrorMessage = "No existe la sucursal.";
//                    }
//                }
//                else
//                {
//                    respuesta.Success = false;
//                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
//                }
//            }
//            catch (Exception e)
//            {
//                logger.Error("ERROR:" + e.Message);
//                respuesta.Success = false;
//                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
//            }
//            return respuesta;
//        }

//        [Route("api/obtenerMisFacturas")]
//        public ResponseListadoMisFacturas obtenerMisFacturas([FromBody]RequestFactura entradas)
//        {
//            ResponseListadoMisFacturas respuesta = new ResponseListadoMisFacturas();

//            try
//            {
//                if (validar.IsAppSecretValid)
//                {
//                    var misFacturas = contextEntity.facturaCliente
//                        .Where(w=> w.receptorCliente.clienteId == entradas.clienteId)
//                        .Select(n => new ResponseMisFacturas
//                        {
//                            facturaClienteId = n.idFacturaClienteId,
//                            numeroFactura = n.ticketSucursal.folio,
//                            fecha = n.fechaRegistro,
//                            fechaCompra = n.ticketSucursal.fechaCompra,
//                            montoCompra = n.ticketSucursal.total,
//                            sucursal = n.ticketSucursal.sucursales.nombre,
//                            estatus = n.catEstatusFactura.descripcion,
//                            PuedeReenviar = (EstatusFactura) n.catEstatusFacturaId == EstatusFactura.ListaParaEnviar,
//                            rfc = n.receptorCliente.datosReceptor.rfc,
//                            razonSocial = n.receptorCliente.datosReceptor.companiaNombreLegal,
//                            email= n.receptorCliente.correo,
//                        }).ToList();
//                    misFacturas.AddRange(
//                        contextEntity.facturaPendiente
//                            .Where(w=>w.receptorCliente.clienteId == entradas.clienteId
//                                      && w.pendiente)
//                            .Select(n=> new ResponseMisFacturas
//                            {
//                                facturaClienteId = Guid.Empty,
//                                fecha = n.fechaRegistro,
//                                fechaCompra = n.fechaRegistro,
//                                estatus = EstatusFactura.Pendiente.ToString() ,
//                                PuedeReenviar = false,
//                                montoCompra = 0,
//                                numeroFactura = n.ticket,
//                                sucursal = n.sucursales.nombre,
//                                rfc = n.receptorCliente.datosReceptor.rfc,
//                                razonSocial = n.receptorCliente.datosReceptor.companiaNombreLegal,
//                                email= n.receptorCliente.correo,
//                            })
//                            .ToList()
//                        );
//                    respuesta.Success = true;
//                    respuesta.ErrorMessage = "";
//                    respuesta.listado = misFacturas;
//                }
//                else
//                {
//                    respuesta.Success = false;
//                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
//                }
//            }
//            catch (Exception e)
//            {
//                logger.Error("ERROR:" + e.Message);
//                respuesta.Success = false;
//                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
//            }
//            return respuesta;
//        }

        
//        [Route("api/solicitarFactura")]
//        public ResponseregistrarFacturas solicitarFactura([FromBody]RequestsolicitarFactura entradas)
//        {
//            ResponseregistrarFacturas respuesta = new ResponseregistrarFacturas();

//            try
//            {
//                if (validar.IsAppSecretValid)
//                {
//                    if (contextEntity.facturaCliente.Any(c => c.ticketSucursal.folio == entradas.folioTicket)
//                        || contextEntity.facturaPendiente.Any(c => c.ticket == entradas.folioTicket))
//                    {
//                        return new ResponseregistrarFacturas
//                        {
//                            Success = false,
//                            ErrorMessage = ConfigurationManager.AppSettings[MENSAJE_FACTURA_USADA],
//                        };
//                    }
//                    using (var tx = contextEntity.Database.BeginTransaction())
//                    {
//                        try
//                        {
//                            datosReceptor datosReceptor;
//                            receptorCliente receptor;

//                            if (entradas.receptorClienteId == 0)
//                            {
//                                datosReceptor =
//                                    new datosReceptor
//                                    {
//                                        companiaNombreLegal = entradas.companiaNombreLegal,
//                                        codigoPostal = entradas.codigoPostal,
//                                        direccion = entradas.direccion,
//                                        rfc = entradas.rfcReceptor
//                                    };

//                                contextEntity.datosReceptor.Add(datosReceptor);

//                                receptor = contextEntity.receptorCliente.Add(new receptorCliente
//                                {
//                                    datosReceptor = datosReceptor,
//                                    clienteId = entradas.clienteId,
//                                    prederteminada = true,
//                                    correo = entradas.correo,
//                                    estatus = true,
//                                    catUsoCFDId = entradas.claveUsoCFDI
//                                });

//                            }
//                            else
//                            {
//                                receptor = contextEntity.receptorCliente.Find(entradas.receptorClienteId);
//                                datosReceptor = contextEntity.datosReceptor.Find(receptor.datoReceptorId);
                                
//                                receptor.catUsoCFDId = entradas.claveUsoCFDI;
//                                receptor.correo = entradas.correo;
//                                receptor.prederteminada = true;

//                                datosReceptor.codigoPostal = entradas.codigoPostal;
//                                datosReceptor.companiaNombreLegal = entradas.companiaNombreLegal;
//                                datosReceptor.direccion = entradas.direccion;
//                                datosReceptor.rfc = entradas.rfcReceptor;

//                                contextEntity.Entry(receptor).State = EntityState.Modified;
//                                contextEntity.Entry(datosReceptor).State = EntityState.Modified;
//                            }
                            

//                            var datosFiscalesId = contextEntity.confDatosFiscalesSucursal
//                                .Where(c => c.sucursalId == entradas.SucursalId)
//                                .Select(c => c.datoFiscalId)
//                                .DefaultIfEmpty(0)
//                                .FirstOrDefault();

//                            if (datosFiscalesId == 0)
//                            {
//                                throw new ArgumentException(nameof(entradas.SucursalId));
//                            }

//                            var guidFactura = InsertarFacturaDb(entradas, receptor, datosFiscalesId);;
//                            contextEntity.SaveChanges();
//                            tx.Commit();

//                            SolicitarFactura(guidFactura);

//                            respuesta.Success = true;
//                            respuesta.ErrorMessage = ConfigurationManager.AppSettings[MENSAJE_FACTURA_SOLICITADA];

//                        }
//                        catch (System.Data.Entity.Validation.DbEntityValidationException)
//                        {
//                            tx.Rollback();
//                        }
//                    }                        
//                }
//                else
//                {
//                    respuesta.Success = false;
//                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
//                }
//            }
//            catch (Exception e)
//            {
//                logger.Error("ERROR:" + e.Message);
//                respuesta.Success = false;
//                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
//            }
//            return respuesta;
//        }


//        public List<ResponseDatosFiscalesReceptor> obtenerDatosRecptorByCliente(int clienteId)
//        {            
//            var datosFiscaleReceptor = contextEntity.receptorCliente
//                .Include("datosReceptor")
//                .Include("catUsoCFDI")
//                .Where(w => w.clienteId == clienteId 
//                            && w.estatus)
//                .Select(n => new ResponseDatosFiscalesReceptor
//                   {
//                       receptorClienteId = n.idReceptorCliente,
//                       prederteminada = n.prederteminada,
//                       email = n.correo,
//                       claveUsoCFDI = n.catUsoCFDI.descripcion,
//                       companiaNombreLegal = n.datosReceptor.companiaNombreLegal,
//                       rfc = n.datosReceptor.rfc,
//                       codigoPostal = n.datosReceptor.codigoPostal,
//                       direccion = n.datosReceptor.direccion
//                })
//                .ToList();

//            return datosFiscaleReceptor;
//        }


//        public List<ResponseCatUsoCFDI> obtenerCatalogoCFDI()
//        {
//            var catUsoCFDI = contextEntity.catUsoCFDI
//                   .Select(n => new ResponseCatUsoCFDI
//                   {
//                       idCatUsoCFDI = n.idCatUsoCFDI,
//                       descripcion = n.descripcion

//                   }).ToList();

//            return catUsoCFDI;
//        }

//        [HttpPost, Route("api/Factura/removerDatosFiscales")]
//        public ResponseBase RemoverDatosFiscales(RequestEliminarDatosFiscales req)
//        {
//            try
//            {
//                if (!validar.IsAppSecretValid || req.clienteId == 0 )
//                {
//                    return new ResponseBase
//                    {
//                        Success = false,
//                        ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS)
//                    };
//                }

//                var datosAEliminar = contextEntity.receptorCliente.Find(req.receptorClienteId);
            
//                if (datosAEliminar == null || datosAEliminar.clienteId != req.clienteId)
//                {
//                    return new ResponseBase {Success = false, ErrorMessage = "No se encontraron los datos fiscales"};
//                }
            
//                datosAEliminar.estatus = false;
//                contextEntity.Entry(datosAEliminar).State = EntityState.Modified;
//                contextEntity.SaveChanges();
//                return new ResponseBase
//                {
//                    Success = true
//                };
//            }
//            catch (Exception e)
//            {
//                #if DEBUG
//                Trace.WriteLine(e);
//                #endif
//                Logger.Error(e);
//                return new ResponseBase
//                {
//                    Success = false,
//                    ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR)
//                };
//            }
//        }

//        [HttpPost, Route("api/Factura/solicitarReenvioFactura")]
//        public ResponseBase ReenviarFactura(RequestReenvioFactura req)
//        {
//            try
//            {
//                if (!validar.IsAppSecretValid || req.clienteId == 0 )
//                {
//                    return new ResponseBase
//                    {
//                        Success = false,
//                        ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS)
//                    };
//                }

//                var datosAReenviar = contextEntity.facturaCliente
//                    .FirstOrDefault(c=> c.idFacturaClienteId == req.factura 
//                                        && c.receptorCliente.clienteId == req.clienteId);
            
//                if (datosAReenviar == null)
//                {
//                    return new ResponseBase {Success = false, ErrorMessage = "Ocurrió un problema al obtener la factura"};
//                }

//                if (string.IsNullOrEmpty(datosAReenviar.archivoXmlRuta)
//                    || string.IsNullOrEmpty(datosAReenviar.archivoPdfRuta))
//                {
//                    return new ResponseBase {Success = false, ErrorMessage = "Ocurrió un problema al obtener la factura"};
//                }

//                BackgroundJob.Enqueue(() => EnvioPostFacturacionProvider.EnviarFactura(datosAReenviar.idFacturaClienteId, HttpRuntime.AppDomainAppPath, true));
            
//                return new ResponseBase
//                {
//                    Success = true,
//                    ErrorMessage = $"La factura ha sido reenviada a: {req.email}"
//                };
//            }
//            catch (Exception e)
//            {
//#if DEBUG
//                Trace.WriteLine(e);
//#endif
//                Logger.Error(e);
//                return new ResponseBase
//                {
//                    Success = false,
//                    ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR)
//                };
//            }
//        }
//        #region Helpers
//        private Guid InsertarFacturaDb(RequestsolicitarFactura entradas, receptorCliente receptor, int datosFiscalesEmisorId)
//        {
//            if (entradas.PendienteTicket && !contextEntity.ticketSucursal.Any(c=>c.folio == entradas.folioTicket))
//            {
//                contextEntity.facturaPendiente.Add(new facturaPendiente
//                {
//                    ticket = entradas.folioTicket,
//                    receptorCliente = receptor,
//                    fechaRegistro = DateTime.Now,
//                    pendiente = true,
//                    datoFiscalId = datosFiscalesEmisorId,
//                    sucursalId = entradas.SucursalId
//                });
//                return Guid.Empty;
//            }
//            else
//            {
//                var ticketSucursalDb = contextEntity.ticketSucursal
//                    .FirstOrDefault(c => c.folio == entradas.folioTicket);
//                var guidFactura = Guid.NewGuid();

//                if (ticketSucursalDb == null)
//                {
//                    throw new ApplicationException("ticketSucursal");
//                }

//                contextEntity.facturaCliente.Add(new facturaCliente
//                {
//                    receptorCliente = receptor,
//                    ticketSucursalId = ticketSucursalDb.idTicketSucursal,
//                    catEstatusFacturaId = (int) EstatusFactura.Pendiente,
//                    fechaRegistro = DateTime.Now,
//                    idFacturaClienteId = guidFactura,
//                    datoFiscalId = datosFiscalesEmisorId,
//                });

//                ticketSucursalDb.catEstatusTicketId = (int) EstatusTicket.Facturado;
//                contextEntity.Entry(ticketSucursalDb).State = EntityState.Modified;
//                return guidFactura;
//            }
//        }
        
//        private static bool ValidarFechaTicket(DateTime ticketFechaCompra)
//        {
//            return ticketFechaCompra.Month == DateTime.Now.Month && ticketFechaCompra.Year == DateTime.Now.Year;
//        }
//        #endregion
    }
}