using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FacturacionApi.Models;
using MystiqueMcApi.Helpers.Facturacion;
using MystiqueMC.DAL;
using MystiqueMcApi.Helpers.Facturacion.Helpers;
using MystiqueMcApi.Helpers.Facturacion.Models.Requests.Restaurante;
using System.Web.Http.ModelBinding;
using HazPedidoErrorCodeResponseBase = MystiqueMcApi.Models.Salidas.ErrorCodeResponseBase;

public class BaseApiController : ApiController
{
    #region Contexto
    private MystiqueMeEntities _contexto;
    protected readonly string MensajeNoPermisos = ConfigurationManager.AppSettings["MYSTIQUE_MENSAJE_NO_PERMISOS"];
    protected readonly string MensajeErrorServidor = ConfigurationManager.AppSettings["MYSTIQUE_MENSAJE_ERROR_SERVIDOR"];

    protected MystiqueMeEntities Contexto
    {
        get
        {
            if (_contexto != null) return _contexto;
            _contexto = new MystiqueMeEntities();

            #if DEBUG
            _contexto.Database.Log = sql => Trace.WriteLine(sql);
            #endif

            return _contexto;
        
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _contexto?.Dispose();
        }
        base.Dispose(disposing);
    }
    #endregion

    #region  Log
    protected readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    #endregion

    #region Validaciones
    private readonly string _appSecret = ConfigurationManager.AppSettings.Get("MYSTIQUE_APP");

    /// <summary>
    /// Valida que el header de la peticion coincida con el token definido en webconfig
    /// </summary>
    protected bool IsAppSecretValid
    {
        get
        {
            var secret = HttpContext.Current.Request.Headers["X-Api-Secret"];
            return !string.IsNullOrEmpty(secret)
                   && secret.Equals(_appSecret);
        }
    }

    protected bool IsAuthenticated => throw new NotImplementedException();

    #endregion

    #region Responses
    private readonly string _mensajeNoPermisos = ConfigurationManager.AppSettings.Get("MENSAJE_NO_PERMISOS");
    private readonly string _mensajeNoAutentificado = ConfigurationManager.AppSettings.Get("MENSAJE_NO_AUTENTIFICADO");
    private readonly string _mensajeErrorServidor = ConfigurationManager.AppSettings.Get("MENSAJE_ERROR_SERVIDOR");

    protected HazPedidoErrorCodeResponseBase RespuestaNoPermisos => new HazPedidoErrorCodeResponseBase
    {
        Message = _mensajeNoPermisos,
        ResponseCode = (int)ResponseTypes.CodigoPermisos,
    };
    protected HazPedidoErrorCodeResponseBase RespuestaNoAutentificado => new HazPedidoErrorCodeResponseBase
    {
        Message = _mensajeNoAutentificado,
        ResponseCode = (int)ResponseTypes.CodigoAutentificacion,
    };
    protected HazPedidoErrorCodeResponseBase RespuestaOkMensaje(string mensaje) => new HazPedidoErrorCodeResponseBase
    {
        ResponseCode = (int)ResponseTypes.CodigoOk,
        Message = mensaje
    };
    protected HazPedidoErrorCodeResponseBase RespuestaOk => new HazPedidoErrorCodeResponseBase
    {
        ResponseCode = (int)ResponseTypes.CodigoOk,
    };
    protected HazPedidoErrorCodeResponseBase RespuestaErrorInterno => new HazPedidoErrorCodeResponseBase
    {
        Message = _mensajeErrorServidor,
        ResponseCode = (int)ResponseTypes.CodigoExcepcion,
    };
    protected static HazPedidoErrorCodeResponseBase RespuestaErrorValidacion(ModelStateDictionary modelState) => new HazPedidoErrorCodeResponseBase
    {
        Message = string.Join(", ", modelState.Select(f => $"({f.Key} : { string.Join(", ", f.Value.Errors.Select(c => $" {c.ErrorMessage} ").ToArray())})").ToArray()),
        ResponseCode = (int)ResponseTypes.CodigoValidacion,
    };
    protected static HazPedidoErrorCodeResponseBase RespuestaErrorValidacion(string errors) => new HazPedidoErrorCodeResponseBase
    {
        Message = errors,
        ResponseCode = (int)ResponseTypes.CodigoValidacion,
    };
    #endregion
    //    #region Facturacion
    //    protected void SolicitarFactura(Guid guidFactura)
    //    {
    //        if (guidFactura == Guid.Empty) return;

    //        try
    //        {
    //            var factura = ObtenerSolicitudFacturacion(guidFactura);
    //            factura.SerializedId = factura.Id.ToString();
    //            Hangfire.BackgroundJob.Enqueue(() =>
    //                FacturacionProvider.SolicitarFactura(factura, HttpRuntime.AppDomainAppPath));
    //        }
    //        catch (Exception e)
    //        {
    //            Logger.Error(e);
    //            Contexto.bitacoraFacturacion.Add(new bitacoraFacturacion
    //            {
    //                facturaClienteId = guidFactura,
    //                codigoRespuesta = 0,
    //                mensajeRespuesta = e.Message,
    //            });
    //#if DEBUG
    //            Trace.WriteLine(e);
    //#endif
    //            Logger.Error(e);
    //        }
    //    }
    //    protected void SolicitarFacturas(List<Guid> guidFacturas)
    //    {
    //        var guidsAFacturar = guidFacturas.Where(c => c != Guid.Empty);

    //        try
    //        {
    //            var solicitudes = ObtenerSolicitudesFacturacion(guidsAFacturar);
    //            if (solicitudes.Any())
    //            {
    //                Hangfire.BackgroundJob.Enqueue(() =>
    //                    FacturacionProvider.SolicitarFacturas(solicitudes, HttpRuntime.AppDomainAppPath));
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //#if DEBUG
    //            Trace.WriteLine(e);
    //#endif
    //            Logger.Error(e);
    //        }
    //    }

    //    protected List<Factura> ObtenerSolicitudesFacturacion(IEnumerable<Guid> guidsAFacturar)
    //    {
    //        var guidsFacturables = Contexto.facturaCliente
    //            .Where(c => guidsAFacturar.Contains(c.idFacturaClienteId)
    //                    && c.ticketSucursal.sucursales.confDatosFiscalesSucursal.Count > 0)
    //            .Select(c => c.idFacturaClienteId)
    //            .ToList();

    //        if (guidsFacturables.Count != guidsAFacturar.Count())
    //        {
    //            var guidsNoFacturables = guidsAFacturar.Except(guidsFacturables);
    //            var noFacturables = Contexto.facturaCliente
    //                .Where(c => guidsNoFacturables.Contains(c.idFacturaClienteId))
    //                .ToList();
    //            var bitacorasNoFacturadas = new List<bitacoraFacturacion>();
    //            foreach (var facturaCliente in noFacturables)
    //            {
    //                facturaCliente.catEstatusFacturaId = (int) EstatusFactura.ErrorFacturacion;
    //                Contexto.Entry(facturaCliente);
    //                bitacorasNoFacturadas.Add(new bitacoraFacturacion
    //                {
    //                    facturaCliente = facturaCliente,
    //                    codigoRespuesta = 0,
    //                    mensajeRespuesta = $"no confDatosFiscalesSucursal para: {facturaCliente.idFacturaClienteId.ToString()}",
    //                });
    //            }
    //            Contexto.bitacoraFacturacion.AddRange(bitacorasNoFacturadas);
    //        }

    //        var solicitudesFactura = Contexto.facturaCliente
    //            .Where(c=> guidsFacturables.Contains(c.idFacturaClienteId))
    //            .AsNoTracking()
    //            .Select(c => new Factura
    //                {
    //                    TipoComprobante = c.ticketSucursal.claveTipoComprobante,
    //                    Serie = c.ticketSucursal.serieFactura,
    //                    Folio = c.ticketSucursal.folioFactura,
    //                    LugarExpedicion = c.ticketSucursal.lugarExpedicionFactura,
    //                    Consumo = new ConsumoFactura
    //                    {
    //                        FolioConsumo = c.ticketSucursal.idTicketSucursal,
    //                        FechaConsumo = c.ticketSucursal.fechaCompra,
    //                        Subtotal = c.ticketSucursal.subtotal,
    //                        Total = c.ticketSucursal.total,
    //                        Iva = c.ticketSucursal.iva,
    //                        ClaveMetodoPago = c.ticketSucursal.claveMetodoPago,
    //                        ClaveFormaPago = c.ticketSucursal.claveFormaPago,
    //                        ClaveTipoComprobante = c.ticketSucursal.claveTipoComprobante,
    //                        Descuento = c.ticketSucursal.descuento,
    //                        Referencia = c.ticketSucursal.referencia,
    //                        CondicionesPago = c.ticketSucursal.condicionesPago,
    //                        Moneda = c.ticketSucursal.moneda,
    //                        TipoCambio = c.ticketSucursal.tipoCambio,
    //                    },
    //                    Emisor = new EmisorFactura
    //                    {
    //                        RazonSocial = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.nombreFiscal,
    //                        RegimenFiscal = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.catRegimenFiscal.catalogosSatId,
    //                        RFC = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.rfc,
    //                        CodigoPostal = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.cp,
    //                        Sucursal = c.ticketSucursal.sucursales.nombre,
    //                    },
    //                    Receptor = new ReceptorFactura
    //                    {
    //                        RazonSocial = c.receptorCliente.datosReceptor.companiaNombreLegal,
    //                        RFC = c.receptorCliente.datosReceptor.rfc,
    //                        CorreoElectrónico = c.receptorCliente.correo,
    //                        Direccion = c.receptorCliente.datosReceptor.direccion,
    //                        CodigoPostal = c.receptorCliente.datosReceptor.codigoPostal,
    //                        UsoCFDI = c.receptorCliente.catUsoCFDI.idCatalogosSat,
    //                    },
    //                    Id = c.idFacturaClienteId,
    //                    DescripcionRegimenFiscal = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.catRegimenFiscal.descripcion,
    //                    DescripcionUsoCFDI = c.receptorCliente.catUsoCFDI.descripcion
    //                })
    //            .ToList();
    //        if (solicitudesFactura == null)
    //        {
    //            throw new ConfigurationErrorsException();
    //        }
    //        var direccionDb = Contexto.facturaCliente
    //            .Where(c=> guidsFacturables.Contains(c.idFacturaClienteId))
    //            .AsNoTracking()
    //            .Select(c =>
    //                new
    //                {
    //                    id = c.idFacturaClienteId, 
    //                    content = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.direccion
    //                })
    //            .ToList();
    //        var detallesTicketSucursal = Contexto.facturaCliente
    //            .Where(c=> guidsFacturables.Contains(c.idFacturaClienteId))
    //            .Select(c =>
    //                new
    //                {
    //                    id = c.idFacturaClienteId, 
    //                    content = c.ticketSucursal.detalleTicketSucursal
    //                })
    //            .AsNoTracking()
    //            .ToList();
    //        var trasladosTicketSucursal = Contexto.facturaCliente
    //            .Where(c=> guidsFacturables.Contains(c.idFacturaClienteId))
    //            .Select(c =>
    //                new
    //                {
    //                    id = c.idFacturaClienteId, 
    //                    content = c.ticketSucursal.trasladosImpuestosTicket
    //                })
    //            .AsNoTracking()
    //            .ToList();

    //       foreach (var factura in solicitudesFactura)
    //       {
    //           factura.Consumo.Traslados = new List<ImpuestoTraslados>();
    //           factura.Consumo.Detalle = new List<DetalleConsumo>();

    //           factura.Consumo.Traslados.AddRange(
    //               trasladosTicketSucursal
    //                   .Where(c => c.id == factura.Id)
    //                   .Select(c => new ImpuestoTraslados
    //                   {
    //                       Impuesto = c.content.FirstOrDefault().impuesto,
    //                       Tasa = c.content.FirstOrDefault().tasaImpuestos,
    //                       ClaveImpuesto = c.content.FirstOrDefault().claveImpuesto
    //                   })
    //                   .ToList());

    //           factura.Consumo.Detalle.AddRange(
    //               detallesTicketSucursal
    //                   .Where(c => c.id == factura.Id)
    //                   .Select(c => new DetalleConsumo
    //                   {
    //                       ClaveProdServ = c.content.FirstOrDefault().claveProdServ,
    //                       Cantidad = c.content.FirstOrDefault().cantidad,
    //                       Unidad = c.content.FirstOrDefault().unidad,
    //                       ClaveUnidad = c.content.FirstOrDefault().claveUnidad,
    //                       ValorUnitario = c.content.FirstOrDefault().valorUnitario,
    //                       IvaDetalle = c.content.FirstOrDefault().ivaDetalle,
    //                       ImporteDetalle = c.content.FirstOrDefault().importeDetalle,
    //                       Descripcion = c.content.FirstOrDefault().descripcion,
    //                   })
    //                   .ToList());
    //           var direc = direccionDb
    //               .Where(c => c.id == factura.Id)
    //               .Select(c => new { c.content.calle, c.content.numExterior, c.content.colonia })
    //               .FirstOrDefault();
    //           factura.Emisor.Direccion = $"{direc.calle} {direc.numExterior} {direc.colonia}";

    //           factura.Consumo.ImporteLetra = factura.Consumo.Total.ToCardinalString();
    //           factura.SerializedId = factura.Id.ToString();
    //       }

    //        return solicitudesFactura;

    //    }

    //    protected Factura ObtenerSolicitudFacturacion(Guid clienteFacturaId)
    //        {
    //            if (Contexto.facturaCliente != null && !Contexto.facturaCliente
    //                    .Where(c => c.idFacturaClienteId == clienteFacturaId)
    //                    .Select(c => c.ticketSucursal.sucursales.confDatosFiscalesSucursal)
    //                    .FirstOrDefault().Any())
    //            {
    //                var facturaDb = Contexto.facturaCliente.Find(clienteFacturaId);
    //                facturaDb.catEstatusFacturaId = (int) EstatusFactura.ErrorFacturacion;
    //                var ex = new ConfigurationErrorsException(
    //                    $"no confDatosFiscalesSucursal para: {clienteFacturaId.ToString()}");
    //                throw ex;
    //            }

    //            var solicitudFactura = Contexto.facturaCliente
    //                .Where(c => c.idFacturaClienteId == clienteFacturaId).AsNoTracking()
    //                .Select(c => new Factura
    //                {
    //                    TipoComprobante = c.ticketSucursal.claveTipoComprobante,
    //                    Serie = c.ticketSucursal.serieFactura,
    //                    Folio = c.ticketSucursal.folioFactura,
    //                    LugarExpedicion = c.ticketSucursal.lugarExpedicionFactura,
    //                    Consumo = new ConsumoFactura
    //                    {
    //                        FolioConsumo = c.ticketSucursal.idTicketSucursal,
    //                        FechaConsumo = c.ticketSucursal.fechaCompra,
    //                        Subtotal = c.ticketSucursal.subtotal,
    //                        Total = c.ticketSucursal.total,
    //                        Iva = c.ticketSucursal.iva,
    //                        ClaveMetodoPago = c.ticketSucursal.claveMetodoPago,
    //                        ClaveFormaPago = c.ticketSucursal.claveFormaPago,
    //                        ClaveTipoComprobante = c.ticketSucursal.claveTipoComprobante,
    //                        Descuento = c.ticketSucursal.descuento,
    //                        Referencia = c.ticketSucursal.referencia,
    //                        CondicionesPago = c.ticketSucursal.condicionesPago,
    //                        Moneda = c.ticketSucursal.moneda,
    //                        TipoCambio = c.ticketSucursal.tipoCambio,
    //                    },
    //                    Emisor = new EmisorFactura
    //                    {
    //                        RazonSocial = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.nombreFiscal,
    //                        RegimenFiscal = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.catRegimenFiscal.catalogosSatId,
    //                        RFC = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.rfc,
    //                        CodigoPostal = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.cp,
    //                        Sucursal = c.ticketSucursal.sucursales.nombre,
    //                    },
    //                    Receptor = new ReceptorFactura
    //                    {
    //                        RazonSocial = c.receptorCliente.datosReceptor.companiaNombreLegal,
    //                        RFC = c.receptorCliente.datosReceptor.rfc,
    //                        CorreoElectrónico = c.receptorCliente.correo,
    //                        Direccion = c.receptorCliente.datosReceptor.direccion,
    //                        CodigoPostal = c.receptorCliente.datosReceptor.codigoPostal,
    //                        UsoCFDI = c.receptorCliente.catUsoCFDI.idCatalogosSat,
    //                    },
    //                    Id = c.idFacturaClienteId,
    //                    DescripcionRegimenFiscal = c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.catRegimenFiscal.descripcion,
    //                    DescripcionUsoCFDI = c.receptorCliente.catUsoCFDI.descripcion
    //                })
    //                .FirstOrDefault();

    //            if (solicitudFactura == null)
    //            {
    //                throw new ConfigurationErrorsException(clienteFacturaId.ToString());
    //            }

    //            var direccionDb = Contexto.facturaCliente
    //                .Where(c => c.idFacturaClienteId == clienteFacturaId).AsNoTracking()
    //                .Select(c =>
    //                    c.ticketSucursal.sucursales.confDatosFiscalesSucursal.FirstOrDefault().datosFiscales.direccion)
    //                .FirstOrDefault();
    //            var detallesTicketSucursal = Contexto.facturaCliente
    //                .Where(c => c.idFacturaClienteId == clienteFacturaId)
    //                .Select(c => c.ticketSucursal.detalleTicketSucursal)
    //                .AsNoTracking()
    //                .FirstOrDefault();
    //            var trasladosTicketSucursal = Contexto.facturaCliente
    //                .Where(c => c.idFacturaClienteId == clienteFacturaId)
    //                .Select(c => c.ticketSucursal.trasladosImpuestosTicket)
    //                .AsNoTracking()
    //                .FirstOrDefault();

    //            if (detallesTicketSucursal != null)
    //            {
    //                solicitudFactura.Consumo.Detalle = detallesTicketSucursal
    //                    .Select(c => new DetalleConsumo
    //                    {
    //                        ClaveProdServ = c.claveProdServ,
    //                        Cantidad = c.cantidad,
    //                        Unidad = c.unidad,
    //                        ClaveUnidad = c.claveUnidad,
    //                        ValorUnitario = c.valorUnitario,
    //                        IvaDetalle = c.ivaDetalle,
    //                        ImporteDetalle = c.importeDetalle,
    //                        Descripcion = c.descripcion,
    //                    })
    //                    .ToList();
    //            }

    //            if (trasladosTicketSucursal != null)
    //            {
    //                solicitudFactura.Consumo.Traslados = trasladosTicketSucursal
    //                    .Select(c => new ImpuestoTraslados
    //                    {
    //                        Impuesto = c.impuesto,
    //                        Tasa = c.tasaImpuestos,
    //                        ClaveImpuesto = c.claveImpuesto
    //                    })
    //                    .ToList();
    //            }

    //            if (direccionDb != null)
    //            {
    //                solicitudFactura.Emisor.Direccion =
    //                    $"{direccionDb.calle} {direccionDb.numExterior} {direccionDb.colonia}";
    //            }

    //            solicitudFactura.Consumo.ImporteLetra = solicitudFactura.Consumo.Total.ToCardinalString();

    //            return solicitudFactura;
    //       }

    // #endregion
}
