using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using FacturacionApi.DAL;
using FacturacionApi.Models.Facturacion;
using FacturacionApi.Models.Requests.Restaurante;
using FacturacionApi.Providers;
using WebGrease.Css.Extensions;
using System.Web;
using System.Web.Http.ModelBinding;
using FacturacionApi.Data.Sat.Catalogos;
using FacturacionApi.Models;
using FacturacionApi.Models.Responses;

namespace FacturacionApi.Controllers
{
    [RoutePrefix("api/v1/restaurantes")]
    public class RestauranteController : BaseApiController
    {
        private const int EstatusInicialFactura = 1;
        #region Consultas
        [HttpGet, Route("facturas/{id}")]
        public ErrorCodeResponseBase ObtenerFactura([FromUri] Guid id)
        {
            try
            {
                if (!IsAppSecretValid) return RespuestaNoPermisos;
                if (!ModelState.IsValid) return RespuestaErrorValidacion(ModelState);

                Logger.Debug($"ObtenerFactura request:{id}, user: {HttpContext.Current.Request.UserHostAddress }");

                if (!Contexto.facturaComprobanteFiscal.Any(c => c.idfacturaComprobanteFiscal == id))
                {
                    return RespuestaErrorValidacion("(id : factura not found)");
                }

                var facturaDb = Contexto.facturaComprobanteFiscal.Find(id);
                if (facturaDb != null && facturaDb.catEstatusComprobanteId == (int) EstatusFactura.Facturada)
                {
                    return RespuestaOkMensaje(facturaDb.cadenaOriginal);
                }
                else
                {
                    return RespuestaErrorValidacion(((EstatusFactura) facturaDb.catEstatusComprobanteId).ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RespuestaErrorInterno;
            }
        }
        #endregion
        #region Procesos
        [HttpPost, Route("facturas")]
        public async Task<SolicitudFacturaResponse> SolicitarFacturaNueva([FromBody] Factura factura)
        {
            int configFacturacion;
            try
            {
                #region Validaciones
                if (!IsAppSecretValid) return RespuestaFacturarNoPermisos;
                if (!ModelState.IsValid) return RespuestaFacturarErrorValidacion(ModelState);

                if (ObtenerCodigoPostal(factura.Emisor.CodigoPostal) == -1 )
                {
                    return RespuestaFacturarErrorValidacion("(factura.emisor.codigoPostal : not found)");
                }
                
                if (ObtenerCodigoPostal(factura.Receptor.CodigoPostal) == -1 )
                {
                    return RespuestaFacturarErrorValidacion("(factura.receptor.codigoPostal : not found)");
                }

                Logger.Debug($"request:{Newtonsoft.Json.JsonConvert.SerializeObject(factura)} user: {HttpContext.Current.Request.UserHostAddress }");

                configFacturacion = Contexto.configuracionFacturacionSucursal
                    .Where(c => c.rfc == factura.Emisor.RFC)
                    .AsNoTracking()
                    .Select(c => c.idConfiguracionFacturacionSucursal)
                    .DefaultIfEmpty(0)
                    .SingleOrDefault();

                if (configFacturacion == 0)
                {
                    return RespuestaFacturarErrorValidacion("(factura.emisor.rfc : configuracionFacturacion not found)");
                }
                
                if (Contexto.facturaCompras.Any(c=>c.idTicketSucursal == factura.Consumo.FolioConsumo))
                {
                    return RespuestaFacturarErrorValidacion("(factura.consumo.folioConsumo : found)");
                }

                var totalFacturaDetalles = 0m;
                foreach (var detalle in factura.Consumo.Detalle)
                {
                    var totalDetalle = detalle.ValorUnitario * detalle.Cantidad;
                    if (totalDetalle != detalle.ImporteDetalle)
                    {
                        return RespuestaFacturarErrorValidacion(
                            $"(factura.consumo.detalle.{detalle.Descripcion}.ImporteDetalle : not equal to (ValorUnitario * Cantidad) )");
                    }

                    totalFacturaDetalles += totalDetalle;
                    totalFacturaDetalles += detalle.IvaDetalle;
                }

                if (totalFacturaDetalles != factura.Consumo.Total)
                    return RespuestaFacturarErrorValidacion($"(factura.consumo.Total : not equal to (totalDetalle) )");
                
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RespuestaFacturarErrorInterno;
            }

            try
            {
                #region PaxFacturacion
                var idFactura = InsertarSolicitudFactura(configFacturacion, factura);
                if(idFactura == Guid.Empty) throw new ApplicationException();
                var configuracion = ObtenerConfiguracion(idFactura);
                var comprobanteFiscal = ObtenerComprobanteFiscal(idFactura);
                comprobanteFiscal.Version = VersionFacturacion;
                await PaxFacturacionProvider.SolicitarFactura(idFactura, comprobanteFiscal, configuracion, HttpRuntime.AppDomainAppPath);
                
                var facturaDb = Contexto.facturaComprobanteFiscal
                    .AsNoTracking()
                    .FirstOrDefault(c=>c.idfacturaComprobanteFiscal == idFactura);
                return facturaDb != null && facturaDb.catEstatusComprobanteId == (int) EstatusFactura.Facturada
                    ? RespuestaFacturarOkMensaje(facturaDb)
                    : RespuestaFacturarErrorFacturacion(facturaDb);
                #endregion
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return RespuestaFacturarErrorInterno;
            }
            
        }

        [HttpPost, Route("facturas/{id}/solicitar")]
        public async Task<SolicitudFacturaResponse> ReSolicitarFactura([FromUri] Guid id)
        {
            try
            {
                #region Validaciones
                if (!IsAppSecretValid) return RespuestaFacturarNoPermisos;
                if (!ModelState.IsValid) return RespuestaFacturarErrorValidacion(ModelState);

                Logger.Debug($"ReSolicitarFactura request:{id} user: {HttpContext.Current.Request.UserHostAddress }");

                if (!Contexto.facturaComprobanteFiscal.Any(c => c.idfacturaComprobanteFiscal == id))
                {
                    return RespuestaFacturarErrorValidacion("(id : factura not found)");
                }

                var facturaDb = Contexto.facturaComprobanteFiscal.Find(id);
                if (facturaDb != null && facturaDb.catEstatusComprobanteId == (int) EstatusFactura.Facturada)
                {
                    return RespuestaFacturarOkMensaje(facturaDb);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RespuestaFacturarErrorInterno;
            }

            try
            {
                #region PaxFacturacion
                var configuracion = ObtenerConfiguracion(id);
                var comprobanteFiscal = ObtenerComprobanteFiscal(id);
                comprobanteFiscal.Version = configuracion.VersionPaxFacturacion;
                await PaxFacturacionProvider.SolicitarFactura(id, comprobanteFiscal, configuracion, HttpRuntime.AppDomainAppPath);
                var factura = Contexto.facturaComprobanteFiscal.Find(id);
                return factura != null && factura.catEstatusComprobanteId == (int) EstatusFactura.Facturada
                    ? RespuestaFacturarOkMensaje(factura)
                    : RespuestaFacturarErrorFacturacion(factura);
                #endregion
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return RespuestaFacturarErrorInterno;
            }
        }

        #endregion

        #region Responses
        private readonly string _mensajeNoPermisos = ConfigurationManager.AppSettings.Get("MENSAJE_NO_PERMISOS");
        private readonly string _mensajeNoAutentificado = ConfigurationManager.AppSettings.Get("MENSAJE_NO_AUTENTIFICADO");
        private readonly string _mensajeErrorServidor = ConfigurationManager.AppSettings.Get("MENSAJE_ERROR_SERVIDOR");
        protected SolicitudFacturaResponse RespuestaFacturarNoPermisos => new SolicitudFacturaResponse
        {
            Message = _mensajeNoPermisos,
            ResponseCode = (int) ResponseTypes.CodigoPermisos,
        };
        protected SolicitudFacturaResponse RespuestaFacturarOkMensaje(facturaComprobanteFiscal factura) => new SolicitudFacturaResponse
        {
            ResponseCode = (int) ResponseTypes.CodigoOk,
            Id = factura.idfacturaComprobanteFiscal,
            Estatus = (EstatusFactura) factura.catEstatusComprobanteId,
            FacturaTimbrada = factura.documentoTimbrado,
            CadenaOriginal = factura.cadenaOriginal,
        };

        protected SolicitudFacturaResponse RespuestaFacturarErrorInterno => new SolicitudFacturaResponse
        {
            Message = _mensajeErrorServidor,
            ResponseCode = (int) ResponseTypes.CodigoExcepcion,
        };
        protected static SolicitudFacturaResponse RespuestaFacturarErrorValidacion(ModelStateDictionary modelState) => new SolicitudFacturaResponse
        {
            Message = string.Join(", ", modelState.Select( f=> $"({f.Key} : { string.Join(", ",f.Value.Errors.Select(c=> $" {c.ErrorMessage} ").ToArray())})" ).ToArray()),
            ResponseCode = (int) ResponseTypes.CodigoValidacion,
        };
        protected static SolicitudFacturaResponse RespuestaFacturarErrorValidacion(string errors) => new SolicitudFacturaResponse
        {
            Message = errors,
            ResponseCode = (int) ResponseTypes.CodigoValidacion,
        };
        protected SolicitudFacturaResponse RespuestaFacturarErrorFacturacion(facturaComprobanteFiscal factura) => new SolicitudFacturaResponse
        {
            ResponseCode = (int) ResponseTypes.CodigoOtros,
            Message = "Ocurrió un error en el timbrado",
            Id = factura.idfacturaComprobanteFiscal,
            Estatus = (EstatusFactura) factura.catEstatusComprobanteId,
        };
        #endregion
        #region Helpers
        private ConfiguracionEmisor ObtenerConfiguracion(Guid id)
        {
            return Contexto.facturaComprobanteFiscal
                .Where(c => c.idfacturaComprobanteFiscal == id)
                .Select(c =>
                    new ConfiguracionEmisor
                    {
                        PathRespaldoDocumentos = c.configuracionFacturacionSucursal.rutaDocumentos,
                        PathCertificado = c.configuracionFacturacionSucursal.rutaCER,
                        PathLlavePrivada = c.configuracionFacturacionSucursal.rutaPrivateKey,
                        PasswordLlavePrivada = c.configuracionFacturacionSucursal.passwordPrivateKey,
                        UsuarioPaxFacturacion = c.configuracionFacturacionSucursal.usuarioPac,
                        PasswordPaxFacturacion = c.configuracionFacturacionSucursal.contraseniaPac,
                        VersionPaxFacturacion = VersionFacturacion,
                        IdEstructuraPaxFacturacion = 0,
                        TipoDocumentoPaxFacturacion = "01",
                    })
                .FirstOrDefault();
        }
        private Guid InsertarSolicitudFactura(int idConfiguracionFacturacion, Factura factura)
        {
            var idComprobante = Guid.Empty;
            using (var tx = Contexto.Database.BeginTransaction())
            {
                var emisor = Contexto.facturaEmisor.Add(new facturaEmisor
                {
                    cp = ObtenerCodigoPostal(factura.Emisor.CodigoPostal),
                    direccion = factura.Emisor.Direccion,
                    nombre = factura.Emisor.RazonSocial,
                    regimenFiscal = (int)factura.Emisor.RegimenFiscal,
                    rfc = factura.Emisor.RFC,
                    sucursal = factura.Emisor.Sucursal,
                    
                });
                var receptor = Contexto.facturaReceptor.Add(new facturaReceptor
                {
                    rfc = factura.Receptor.RFC,
                    nombre = factura.Receptor.RazonSocial,
                    direccion = factura.Receptor.Direccion,
                    cp = ObtenerCodigoPostal(factura.Receptor.CodigoPostal),
                    catUsoCFDId = (int)factura.Receptor.UsoCFDI,
                    correo = factura.Receptor.CorreoElectrónico,
                });
                var consumo = Contexto.facturaCompras.Add(new facturaCompras()
                {
                    fechaCompra = factura.Consumo.FechaConsumo,
                    claveMetodoPago = (int)factura.Consumo.ClaveMetodoPago,
                    claveFormaPago =(int) factura.Consumo.ClaveFormaPago,
                    claveTipoComprobante = (int)factura.Consumo.ClaveTipoComprobante,
                    moneda = (int) factura.Consumo.Moneda,
                    iva = factura.Consumo.Iva,
                    total = factura.Consumo.Total,
                    subtotal = factura.Consumo.Subtotal,
                    descuento = factura.Consumo.Descuento,
                    importeLetraTotal = factura.Consumo.ImporteLetra,
                    numeroTarjeta = factura.Consumo.Tarjeta,
                    referencia = factura.Consumo.Referencia,
                    catEstatusTicketId = 0,
                    condicionesPago = factura.Consumo.CondicionesPago,
                    idTicketSucursal = factura.Consumo.FolioConsumo,
                    tipoCambio = factura.Consumo.TipoCambio,
                });
                var detalles = new List<detalleFacturaCompras>();
                factura.Consumo.Detalle.ForEach(c =>
                {
                    detalles.Add(new detalleFacturaCompras
                    {
                        facturaCompras = consumo,
                        claveProdServ = (int)c.ClaveProdServ,
                        descripcion = c.Descripcion,
                        cantidad = c.Cantidad,
                        ivaDetalle = c.IvaDetalle,
                        importeDetalle = c.ImporteDetalle,
                        unidad = c.Unidad,
                        claveUnidad = c.ClaveUnidad,
                        valorUnitario = c.ValorUnitario,
                    });
                });
                Contexto.detalleFacturaCompras.AddRange(detalles);
                var traslados = new List<impuestosTraslados>();
                factura.Consumo.Traslados.ForEach(c =>
                {
                    traslados.Add(new impuestosTraslados
                    {
                        facturaCompras = consumo,
                        claveImpuesto = (int)c.ClaveImpuesto,
                        tasa = c.Tasa,
                        importe = c.Impuesto
                    });
                });
                Contexto.impuestosTraslados.AddRange(traslados);

                idComprobante = Guid.NewGuid();
                Contexto.facturaComprobanteFiscal.Add(new facturaComprobanteFiscal
                {
                    idfacturaComprobanteFiscal = idComprobante,
                    configuracionFacturacionSucursalId = idConfiguracionFacturacion,
                    tipoComprobante =  (int)factura.TipoComprobante,
                    facturaCompras = consumo,
                    facturaEmisor = emisor,
                    facturaReceptor = receptor,
                    fechaRegistro = DateTime.Now,
                    lugarExpedicion = factura.LugarExpedicion,
                    folio = factura.Folio,
                    serie = factura.Serie,
                    catEstatusComprobanteId = EstatusInicialFactura
                });
                Contexto.SaveChanges();
                tx.Commit();
            }
            return idComprobante;

        }
        
        private Comprobante ObtenerComprobanteFiscal(Guid idFactura)
        {
            var fecha = Convert.ToDateTime(DateTime.Now.ToString("s"));
            var comprobante = Contexto.facturaComprobanteFiscal
                .Where(c => c.idfacturaComprobanteFiscal == idFactura)
                .Select(c =>
                new Comprobante
                {
                    Fecha = fecha,
                    Folio = c.folio,
                    Serie = c.serie,
                    LugarExpedicion = c.lugarExpedicion,
                    SubTotal = c.facturaCompras.subtotal,
                    Total = c.facturaCompras.total,
                    MetodoPagoSpecified = true,
                    FormaPagoSpecified = true,
                    CondicionesDePago = c.facturaCompras.condicionesPago,
                    FormaPago = (c_FormaPago) c.facturaCompras.claveFormaPago,
                    MetodoPago = (c_MetodoPago) c.facturaCompras.claveMetodoPago,
                    Moneda = (c_Moneda) c.facturaCompras.moneda,
                    TipoCambio = c.facturaCompras.tipoCambio,
                    TipoDeComprobante = (c_TipoDeComprobante) c.facturaCompras.claveTipoComprobante,
                    Impuestos = new ComprobanteImpuestos
                    {
                        TotalImpuestosTrasladadosSpecified = true,
                        TotalImpuestosTrasladados = c.facturaCompras.iva,
                    },
                    Emisor = new ComprobanteEmisor
                    {
                        Rfc = c.facturaEmisor.rfc,
                        Nombre = c.facturaEmisor.nombre,
                        RegimenFiscal = (c_RegimenFiscal) c.facturaEmisor.regimenFiscal,
                    },
                    Receptor = new ComprobanteReceptor
                    {
                        Rfc = c.facturaReceptor.rfc,
                        Nombre = c.facturaReceptor.nombre,
                        UsoCFDI = (c_UsoCFDI) c.facturaReceptor.catUsoCFDId,
                    },
                    
                })
                .FirstOrDefault();
            
            var conceptos = Contexto.facturaComprobanteFiscal
                .Where(c => c.idfacturaComprobanteFiscal == idFactura)
                .Select(c => c.facturaCompras.detalleFacturaCompras)
                .FirstOrDefault();

            var traslados = Contexto.facturaComprobanteFiscal
                .Where(c => c.idfacturaComprobanteFiscal == idFactura)
                .Select(c => c.facturaCompras.impuestosTraslados)
                .FirstOrDefault();

            if(conceptos == null || comprobante == null || traslados == null) throw new DataException();
            
            comprobante.Conceptos = conceptos
                .Select(d => new ComprobanteConcepto
                {
                    ClaveProdServ = (c_ClaveProdServ) d.claveProdServ,
                    Cantidad = d.cantidad,
                    Unidad = d.unidad,
                    ClaveUnidad = d.claveUnidad,
                    Descripcion = d.descripcion,
                    ValorUnitario = d.valorUnitario,
                    Importe = d.importeDetalle,
                })
                .ToArray();
            comprobante.Impuestos.Traslados = traslados
                .Select(d => new ComprobanteImpuestosTraslado
                {
                    Importe = d.importe,
                    TasaOCuota = d.tasa,
                    Impuesto = (c_Impuesto) d.claveImpuesto
                })
                .ToArray();
            return comprobante;
        }

        public static int ObtenerCodigoPostal(string cp)
        {
            try
            {


                if (!cp.StartsWith("Item"))
                    cp = "Item" + cp;
                var val = (int)(c_CodigoPostal) Enum.Parse(typeof(c_CodigoPostal), cp.Trim());
                return val;
            }
            catch (Exception e)
            {
                #if DEBUG
                System.Diagnostics.Trace.WriteLine(e);
                #endif
                return -1;
            }
        }
        #endregion
    }
}

 