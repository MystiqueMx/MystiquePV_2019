
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FacturacionTDCAPI.DAL;
using System.Web.Http;
using FacturacionTDCAPI.Models.Requests.Restaurante;
using FacturacionTDCAPI.Models.Responses;
using FacturacionTDCAPI.Models;
using FacturacionTDCApi.Helpers;
using System.Collections.Generic;
using Facturacion.v33;
using FacturacionTDCAPI.Providers;
using FacturacionTDCAPI.Models.Responses.Restaurante;
using System.Web.Hosting;
using FacturacionTDCAPI.Helpers;
using FacturacionTDCAPI.Models.Responses.Cancelacion;

namespace FacturacionTDCAPI.Controllers
{
    [RoutePrefix("api/v1/facturacion")]
    public class FacturacionController : BaseApiController
    {
        private const int EstatusInicialFactura = (int)EstatusFactura.Recibida;

        #region Consultas 

        [HttpGet, Route("facturas/{emisor}")]
        public ListadoFacturasResponse ObtenerFacturas([FromUri] string emisor, string receptor = null, string nombre = null)
        {
            #region ObtenerFacturas
            try
            {
                if (!IsAppSecretValid) return new ListadoFacturasResponse { EstatusPeticion = RespuestaNoPermisos };
                if (!ModelState.IsValid) return new ListadoFacturasResponse { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("ObtenerFacturas from: {0}, emisor: {1}", Request.Headers.UserAgent, emisor);

                if (!Contexto.ConfiguracionEmisores.Any(c => c.rfc == emisor))
                {
                    return new ListadoFacturasResponse { EstatusPeticion = RespuestaErrorValidacion($"(emisor : emisor {emisor} not found)") };
                }

                if (string.IsNullOrEmpty(receptor) && string.IsNullOrEmpty(nombre))
                {
                    return new ListadoFacturasResponse { EstatusPeticion = RespuestaErrorValidacion($"(receptor, nombre : no filters applied)") };
                }

                var initialDate = DateTime.Now.Date.AddMonths(-3);

                var facturas = Contexto.ComprobanteFiscals
                      .Include("ReceptorFactura")
                     .Include("ConfiguracionEmisore")
                     .Include("BitacoraPaxFacturacions")
                     .Include("ConsumoFactura")
                    .Where(c => c.ConfiguracionEmisore.rfc == emisor
                              // && c.fechaCompra >= initialDate
                               && c.catEstatusComprobanteId == (int)EstatusFactura.Facturada)
                    .OrderByDescending(c => c.fechaCompra)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(receptor))
                {
                    facturas = facturas.Where(c => c.ReceptorFactura.rfc == receptor);
                }
                if (!string.IsNullOrEmpty(nombre))
                {
                    facturas = facturas.Where(c => c.ReceptorFactura.razonSocial.Contains(nombre));
                }
                if (facturas.Any())
                {
                    var listadoFacturas = facturas.Select(c => new FacturaReimpresion
                    {
                        RazonSocial = c.ReceptorFactura.razonSocial,
                        Rfc = c.ReceptorFactura.rfc,
                        FechaTimbrado = c.fechaCompra,
                        Emisor = c.ConfiguracionEmisore.rfc,
                        Uuid = c.uuid,
                        Total = c.ConsumoFactura.total
                    })
                    .ToArray();
                    return new ListadoFacturasResponse { EstatusPeticion = RespuestaOk, Facturas = listadoFacturas };
                }
                else
                {
                    return new ListadoFacturasResponse { EstatusPeticion = RespuestaOk, Facturas = new FacturaReimpresion[] { } };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ListadoFacturasResponse { EstatusPeticion = RespuestaErrorInterno };
            }
            #endregion
        }
        #endregion
        #region Procesos
        [HttpPost, Route("facturas")]
        public async Task<SolicitudFacturaResponse> SolicitarFacturaNueva([FromBody] Factura factura)
        {
            #region SolicitarFacturaNueva
            int configFacturacion;
            try
            {
                #region Validaciones

                if (!IsAppSecretValid) return new SolicitudFacturaResponse { EstatusPeticion = RespuestaNoPermisos };
                //if (!ModelState.IsValid) return new SolicitudFacturaResponse { EstatusPeticion = RespuestaErrorValidacion(ModelState) };


                Logger.Debug("SolicitarFacturaNueva from: {0}, request: {1}", Request.Headers.UserAgent, factura);

             
                configFacturacion = Contexto.ConfiguracionEmisores
                    .Where(c => c.rfc == factura.Emisor.RFC)
                    .Select(c => c.idConfiguracionEmisores)
                    .DefaultIfEmpty(-1)
                    .SingleOrDefault();

                if (configFacturacion < 0)
                {
                    return new SolicitudFacturaResponse { EstatusPeticion = RespuestaErrorValidacion("La configuración del emisor es incorrecta, por favor contacta a tu administrador") };
                }

                if (Contexto.ComprobanteFiscals.Any(c => c.folio == factura.Folio
                    && c.ConfiguracionEmisore.rfc == factura.Emisor.RFC
                    && c.catEstatusComprobanteId == (int)EstatusFactura.Facturada))
                {
                    var response = Contexto.ComprobanteFiscals.First(c =>
                        c.folio == factura.Folio
                        && c.ConfiguracionEmisore.rfc == factura.Emisor.RFC
                        && c.catEstatusComprobanteId == (int)EstatusFactura.Facturada);
                    return new SolicitudFacturaResponse
                    {
                        EstatusPeticion = RespuestaOk,
                        Estatus = (EstatusFactura)response.catEstatusComprobanteId,
                        Id = response.uuid.GetValueOrDefault(),
                        FacturaTimbrada = response.documentoTimbrado,
                        CadenaOriginal = response.cadenaOriginal
                    };
                }

                var totalFacturaDetalles = 0m;
                foreach (var detalle in factura.Consumo.Detalle)
                {
                    var totalDetalle = detalle.ValorUnitario * detalle.Cantidad;
                    var importedetalle = detalle.ImporteDetalle * detalle.Cantidad;
                    if (totalDetalle != importedetalle)
                    {
                        return new SolicitudFacturaResponse
                        {
                            EstatusPeticion = RespuestaErrorValidacion("El importe del consumo es incorrecto")
                        };
                    }

                    totalFacturaDetalles += totalDetalle;
                    //totalFacturaDetalles += detalle.IvaDetalle;
                }

                if (totalFacturaDetalles != factura.Consumo.Total)
                    return new SolicitudFacturaResponse
                    {
                        EstatusPeticion = RespuestaErrorValidacion("El total de la factura es incorrecto")
                    };
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new SolicitudFacturaResponse { EstatusPeticion = RespuestaErrorInterno, ErrorUsuario = "Ha ocurrido un error al facturar antes de procesar la solicitud" };
            }

            try
            {
                #region Facturar
                var idFactura = InsertarSolicitudFactura(factura);
                if (idFactura <= 0) throw new ApplicationException("idFactura <= 0");
              

                var configuracion = ObtenerConfiguracion(idFactura);
                var comprobanteFiscal = ObtenerComprobanteFiscal(idFactura);
                comprobanteFiscal.Version = VersionFacturacion;
                comprobanteFiscal.Receptor.Rfc = comprobanteFiscal.Receptor.Rfc.ToUpper();
                comprobanteFiscal.Emisor.Rfc = comprobanteFiscal.Emisor.Rfc.ToUpper();
                var facturaDb = await FacturacionProvider.SolicitarFactura(idFactura, comprobanteFiscal, configuracion, HttpRuntime.AppDomainAppPath);
                if (facturaDb?.catEstatusComprobanteId == (int)EstatusFactura.Facturada)
                {
                    return new SolicitudFacturaResponse
                    {
                        EstatusPeticion = RespuestaOk,
                        Estatus = (EstatusFactura)facturaDb.catEstatusComprobanteId,
                        Id = facturaDb.uuid.GetValueOrDefault(),
                        FacturaTimbrada = facturaDb.documentoTimbrado,
                        CadenaOriginal = facturaDb.cadenaOriginal,
                    };
                }
                else
                {
                    var bitacora = Contexto.BitacoraPaxFacturacions.First(c => c.comprobanteFiscalId == idFactura);
                    return new SolicitudFacturaResponse
                    {
                        EstatusPeticion = RespuestaErrorFacturacion(facturaDb.idComprobanteFiscal),
                        ErrorUsuario = bitacora.mensajeRespuesta,
                    };
                }



                #endregion
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new SolicitudFacturaResponse { EstatusPeticion = RespuestaErrorInterno, ErrorUsuario = RespuestaErrorInterno.Message };
            }
            #endregion

        }
        private ConfiguracionEmisor ObtenerConfiguracion(long id)
        {
            #region ObtenerConfiguracion
            return Contexto.ComprobanteFiscals
                .Where(c => c.idComprobanteFiscal == id)
                .Select(c =>
                    new ConfiguracionEmisor
                    {
                        PathRespaldoDocumentos = c.ConfiguracionEmisore.rutaDocumentos,
                        PathCertificado = c.ConfiguracionEmisore.rutaCer,
                        PathLlavePrivada = c.ConfiguracionEmisore.rutaPrivateKey,
                        PasswordLlavePrivada = c.ConfiguracionEmisore.passwordPrivateKey,
                        UsuarioPaxFacturacion = c.ConfiguracionEmisore.usuarioPax,
                        PasswordPaxFacturacion = c.ConfiguracionEmisore.contrasenaPax,
                        VersionPaxFacturacion = VersionFacturacion,
                        IdEstructuraPaxFacturacion = 0,
                        TipoDocumentoPaxFacturacion = "01",
                        Direccion = c.ConfiguracionEmisore.direccion,
                        CodigoPostal = (c_CodigoPostal)c.ConfiguracionEmisore.codigoPostal,
                        RazonSocial = c.ConfiguracionEmisore.razonSocial,
                        RegimenFiscal = (c_RegimenFiscal)c.ConfiguracionEmisore.regimenFiscal,
                        Rfc = c.ConfiguracionEmisore.rfc,
                        SerieFactura = c.ConfiguracionEmisore.serieFacturas,
                        
                    }).First();
            #endregion
        }

        private long InsertarSolicitudFactura(Factura factura)
        {
            #region InsertarSolicitudFactura
            long id = -1;
            using (var tx = Contexto.Database.BeginTransaction())
            {
                var emisor = Contexto.ConfiguracionEmisores.First(c => c.rfc == factura.Emisor.RFC);
                var receptor = Contexto.ReceptorFacturas.Add(new DAL.ReceptorFactura
                {
                    rfc = NormalizeStrings.RemoveDiacritics(factura.Receptor.Rfc, true),
                    razonSocial = NormalizeStrings.RemoveDiacritics(factura.Receptor.RazonSocial, true),
                    direccion = NormalizeStrings.RemoveDiacritics(factura.Receptor.Direccion, true),
                    codigoPostal = factura.Receptor.CodigoPostal,
                    catUsoCfdi = factura.Receptor.UsoCfdi,
                    correo = factura.Receptor.CorreoElectrónico,
                });
                var consumo = Contexto.ConsumoFacturas.Add(new DAL.ConsumoFactura
                {
                    fechaCompra = factura.Consumo.FechaConsumo.Date,
                    claveMetodoPago = factura.Consumo.ClaveMetodoPago,
                    claveFormaPago = factura.Consumo.ClaveFormaPago,
                    claveTipoComprobante = factura.Consumo.ClaveTipoComprobante,
                    moneda = (int)factura.Consumo.Moneda,
                    iva = factura.Consumo.Iva,
                    total = factura.Consumo.Total,
                    subtotal = factura.Consumo.Subtotal,
                    descuento = factura.Consumo.Descuento,
                    importeLetraTotal = factura.Consumo.ImporteLetra,
                    numeroTareta = NormalizeStrings.RemoveDiacritics(factura.Consumo.Tarjeta, true),
                    referencia = NormalizeStrings.RemoveDiacritics(factura.Consumo.Referencia, true),
                    condicionesPago = NormalizeStrings.RemoveDiacritics(factura.Consumo.CondicionesPago, true),
                    tipoCambio = factura.Consumo.TipoCambio,
                });
                var detalles = new List<ConsumoFacturaDetalle>();
                factura.Consumo.Detalle.ForEach(c =>
                {
                    detalles.Add(new ConsumoFacturaDetalle
                    {
                        ConsumoFactura = consumo,
                        ClaveSat = c.ClaveSat,
                        descripcion = NormalizeStrings.RemoveDiacritics(c.Descripcion, true),
                        cantidad = c.Cantidad,
                        ivaDetalle = c.IvaDetalle,
                        importeDetalle = c.ImporteDetalle,
                        unidad = NormalizeStrings.RemoveDiacritics(c.Unidad, true),
                        claveUnidadSat = c.ClaveUnidadSat,
                        valorUnitario = c.ValorUnitario,
                    });
                });
                Contexto.ConsumoFacturaDetalles.AddRange(detalles);
                var traslados = new List<ConsumoFacturaImpuesto>();
                factura.Consumo.Traslados.ForEach(c =>
                {
                    traslados.Add(new ConsumoFacturaImpuesto
                    {
                        ConsumoFactura = consumo,
                        claveImpuesto = c.ClaveImpuesto,
                        tasa = c.Tasa,
                        importe = c.Impuesto
                    });
                });
                Contexto.ConsumoFacturaImpuestos.AddRange(traslados);

                var comprobante = Contexto.ComprobanteFiscals.Add(new ComprobanteFiscal
                {
                    ConfiguracionEmisore = emisor,
                    tipoComprobante = factura.TipoComprobante,
                    ConsumoFactura = consumo,
                    ReceptorFactura = receptor,
                    fechaCompra = factura.Consumo.FechaConsumo,
                    fechaRegistro = DateTime.Now,
                    folio = NormalizeStrings.RemoveDiacritics(factura.Folio, true),
                    serie = NormalizeStrings.RemoveDiacritics(emisor.serieFacturas, true),
                    catEstatusComprobanteId = EstatusInicialFactura
                });
                Contexto.SaveChanges();
                tx.Commit();
                id = comprobante.idComprobanteFiscal;
            }
            return id;
            #endregion
        }

        private Comprobante ObtenerComprobanteFiscal(long idFactura)
        {
            #region ObtenerComprobanteFiscal
            var comprobanteContext = Contexto.ComprobanteFiscals.Where(c => c.idComprobanteFiscal == idFactura);
            var comprobante = comprobanteContext
                .AsEnumerable()
                .Select(c =>
                    new Comprobante
                    {
                        Fecha = c.fechaRegistro.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Folio = c.folio,
                        Serie = c.serie,
                        Descuento = 0,
                        DescuentoSpecified = true,
                        LugarExpedicion = (c_CodigoPostal)c.ConfiguracionEmisore.codigoPostal,
                        SubTotal = c.ConsumoFactura.subtotal,
                        Total = c.ConsumoFactura.total,
                        MetodoPagoSpecified = true,
                        FormaPagoSpecified = true,
                        CondicionesDePago = c.ConsumoFactura.condicionesPago,
                        FormaPago = (c_FormaPago)c.ConsumoFactura.claveFormaPago,
                        MetodoPago = (c_MetodoPago)c.ConsumoFactura.claveMetodoPago,
                        Moneda = (c_Moneda)c.ConsumoFactura.moneda,
                        TipoCambio = String.Format("{0:0.##}", c.ConsumoFactura.tipoCambio),
                        TipoCambioSpecified = true,
                        TipoDeComprobante = (c_TipoDeComprobante)c.ConsumoFactura.claveTipoComprobante,
                        Impuestos = new ComprobanteImpuestos
                        {
                            TotalImpuestosTrasladadosSpecified = true,
                            TotalImpuestosTrasladados = c.ConsumoFactura.iva,
                        },
                        Emisor = new ComprobanteEmisor
                        {
                            Rfc = c.ConfiguracionEmisore.rfc,
                            Nombre = c.ConfiguracionEmisore.razonSocial,
                            RegimenFiscal = (c_RegimenFiscal)c.ConfiguracionEmisore.regimenFiscal,

                        },
                        Receptor = new ComprobanteReceptor
                        {
                            Rfc = c.ReceptorFactura.rfc,
                            Nombre = c.ReceptorFactura.razonSocial,
                            UsoCFDI = (c_UsoCFDI)c.ReceptorFactura.catUsoCfdi,
                        },

                    })
                    .FirstOrDefault();
            comprobante.FechaDateTime = comprobante.FechaDateTime.AddTicks(-(comprobanteContext.FirstOrDefault().fechaRegistro.Ticks % TimeSpan.TicksPerSecond));
            var conceptos = Contexto.ComprobanteFiscals
                .Where(c => c.idComprobanteFiscal == idFactura)
                .Select(c => c.ConsumoFactura.ConsumoFacturaDetalles)
                .FirstOrDefault();

            var traslados = Contexto.ComprobanteFiscals
                .Where(c => c.idComprobanteFiscal == idFactura)
                .Select(c => c.ConsumoFactura.ConsumoFacturaImpuestos)
                .FirstOrDefault();

            if (conceptos == null || comprobante == null || traslados == null) throw new DataException();

            comprobante.Conceptos = conceptos
                .Select(d => new ComprobanteConcepto
                {
                    //ClaveProdServ = d.ClaveSat,
                    Cantidad = d.cantidad,
                    Unidad = d.unidad,
                    //ClaveUnidad = d.claveUnidadSat,
                    Descripcion = d.descripcion,
                    ValorUnitario = d.valorUnitario,
                    Importe = d.importeDetalle,
                    Descuento = 0,
                    DescuentoSpecified = true,
                    Impuestos = new ComprobanteConceptoImpuestos
                    {
                        Traslados = traslados.AsEnumerable().Select(s => new ComprobanteConceptoImpuestosTraslado
                        {
                            Base = d.importeDetalle,
                            Importe = s.importe,
                            TasaOCuota = s.tasa.ToString("N6"),
                            TipoFactor = c_TipoFactor.Tasa,
                            Impuesto = (c_Impuesto)s.claveImpuesto,
                            ImporteSpecified = true,
                            TasaOCuotaSpecified = true
                        }
                        ).ToArray()
                    },
                })
                .ToArray();
            comprobante.Impuestos.Traslados = traslados.AsEnumerable()
                .Select(d => new ComprobanteImpuestosTraslado
                {
                    Importe = d.importe,
                    TasaOCuota = d.tasa.ToString("N6"),
                    TipoFactor = c_TipoFactor.Tasa,
                    Impuesto = (c_Impuesto)d.claveImpuesto,
                })
                .ToArray();
            return comprobante;
            #endregion
        }

        [HttpPost, Route("facturas/{id}/reenviar")]
        public ResponseBase ReenviarFactura([FromUri] Guid id, ReenvioFacturaRequest request)
        {
            #region ReenviarFactura
            try
            {
                if (!IsAppSecretValid) return new ResponseBase { EstatusPeticion = RespuestaNoPermisos };
                if (!ModelState.IsValid) return new ResponseBase { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("ReSolicitarFactura from: {0}, id: {1}", Request.Headers.UserAgent, id);

                if (!Contexto.ComprobanteFiscals.Any(c => c.uuid == id))
                {
                    return new ResponseBase { EstatusPeticion = RespuestaErrorValidacion("(id : uuid not found)") };
                }

                var facturaDb = Contexto.ComprobanteFiscals.First(c => c.uuid == id);
                if (facturaDb.catEstatusComprobanteId == (int)EstatusFactura.Facturada && !string.IsNullOrEmpty(facturaDb.rutaXml))
                {
                    try
                    {
                        var email = request?.Email ?? null;
                        Hangfire.BackgroundJob.Enqueue(() => GenerarFactura.Procesar(facturaDb.idComprobanteFiscal, HostingEnvironment.ApplicationPhysicalPath, HostingEnvironment.ApplicationPhysicalPath + facturaDb.rutaXml, email));
                        return new ResponseBase { EstatusPeticion = RespuestaOk };
                    }
                    catch (Exception ex1)
                    {
                        Logger.Error(ex1);
                        return new ResponseBase { EstatusPeticion = RespuestaErrorInterno };
                    }
                }
                else
                {
                    return new ResponseBase
                    {
                        EstatusPeticion = RespuestaErrorValidacion(((EstatusFactura)facturaDb.catEstatusComprobanteId)
                        .ToString())
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ResponseBase { EstatusPeticion = RespuestaErrorInterno };
            }
            #endregion
        }

        [HttpPost, Route("facturas/{id}/cancelar")]
        public CancelacionResponse CancelarFactura([FromUri] Guid id)
        {
            #region CancelarFactura
            try
            {
                if (!IsAppSecretValid) return new CancelacionResponse { EstatusPeticion = RespuestaNoPermisos };
                if (!ModelState.IsValid) return new CancelacionResponse { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("CancelarFactura from: {0}, id: {1}", Request.Headers.UserAgent, id);

                if (!Contexto.ComprobanteFiscals.Any(c => c.uuid == id && c.catEstatusComprobanteId == (int)EstatusFactura.Facturada))
                {
                    return new CancelacionResponse { EstatusPeticion = RespuestaErrorValidacion("(id : uuid not found)") };
                }

                var facturaDb = Contexto.ComprobanteFiscals
                    .First(c => c.uuid == id && c.catEstatusComprobanteId == (int)EstatusFactura.Facturada);
                var configuracion = ObtenerConfiguracion(facturaDb.idComprobanteFiscal);

                FacturacionProvider.SolicitarCancelacion(facturaDb.idComprobanteFiscal, configuracion);

                return new CancelacionResponse { EstatusPeticion = RespuestaOk, FolioTicket = facturaDb.folio };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new CancelacionResponse { EstatusPeticion = RespuestaErrorInterno };
            }
            #endregion
        }
        #endregion

        #region Comprobantes
        [HttpGet, Route("comprobantes/{id}/json")]
        public SolicitudFacturaResponse ReenviarFacturaJson([FromUri] Guid id)
        {
            #region  ReenviarFacturaJson
            try
            {
                if (!IsAppSecretValid) return new SolicitudFacturaResponse { EstatusPeticion = RespuestaNoPermisos };
                //if (!ModelState.IsValid) return new ResponseBase { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("ReSolicitarFactura from: {0}, id: {1}", Request.Headers.UserAgent, id);

                if (!Contexto.ComprobanteFiscals.Any(c => c.uuid == id))
                {
                    return new SolicitudFacturaResponse { EstatusPeticion = RespuestaErrorValidacion("uuid") };
                }

                var facturaDb = Contexto.ComprobanteFiscals
                       .Include("ReceptorFactura")
                     .Include("ConfiguracionEmisore")
                     .Include("BitacoraPaxFacturacions")
                     .Include("ConsumoFactura")
                     .Include("ConsumoFactura.ConsumoFacturaImpuestos")
                    .First(c => c.uuid == id);
                if (facturaDb.catEstatusComprobanteId == (int)EstatusFactura.Facturada)
                {
                    var usoCfdi = Contexto.CatalogoUsoCfdis
                        .First(c => c.idCatalogoUsoCfdi == facturaDb.ReceptorFactura.catUsoCfdi);
                    var formaPago = Contexto.CatalogoFormaPagoes
                        .First(c => c.idCatalogoFormaPago == facturaDb.ConsumoFactura.claveFormaPago);
                    return new SolicitudFacturaResponse
                    {
                        EstatusPeticion = RespuestaOk,
                        Id = facturaDb.uuid.GetValueOrDefault(Guid.Empty),
                        FacturaTimbrada = facturaDb.documentoTimbrado,
                        CadenaOriginal = facturaDb.cadenaOriginal,
                        Estatus = EstatusFactura.Facturada,
                        FormaPago = formaPago.descripcion,
                        UsoCfdi = usoCfdi.descripcion,
                        Direccion = facturaDb.ReceptorFactura.direccion,
                        CantidadLetra = facturaDb.ConsumoFactura.importeLetraTotal,
                        Impuestos = facturaDb.ConsumoFactura.ConsumoFacturaImpuestos.Select(c => c.tasa).ToArray(),
                        RegimenFiscal = facturaDb.ConfiguracionEmisore.descRegimenFiscal,
                    };
                }
                else
                {
                    return new SolicitudFacturaResponse { EstatusPeticion = RespuestaErrorValidacion($"EstatusFactura: {facturaDb.catEstatusComprobanteId}") };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new SolicitudFacturaResponse { EstatusPeticion = RespuestaErrorInterno };
            }
            #endregion
        }
        #endregion

    }
}