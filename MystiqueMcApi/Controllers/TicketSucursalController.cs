using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using MystiqueMcApi.Helpers;
using MystiqueMcApi.Models;
using MystiqueMcApi.Models.Entradas.TicketSucursal;
using MystiqueMC.DAL;

namespace MystiqueMcApi.Controllers
{
    [RoutePrefix("api/v1")]
    public class TicketSucursalController : BaseApiController
    {
        //internal static readonly int IdConfiguracionFacturacion = int.Parse(ConfigurationManager.AppSettings.Get("FACTURACION_API_ID"));
        //protected readonly string MensajeNoPermisos = ConfigurationManager.AppSettings["MYSTIQUE_MENSAJE_NO_PERMISOS"];
        //protected readonly string MensajeErrorServidor = ConfigurationManager.AppSettings["MYSTIQUE_MENSAJE_ERROR_SERVIDOR"];

        //[Route("Ticket"), HttpPost]
        //public ErrorCodeResponseBase Post([FromBody]Ticket ticket)
        //{
        //    try
        //    {
        //        var validar = new PermisosApi();
        //        if (!validar.IsAppSecretValid)
        //        {
        //            return RespuestaNoPermisos;
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            return RespuestaErrorValidacion(ModelState);
        //        }

        //        if (!Contexto.sucursales.Any(c=> c.sucursalPuntoVenta == ticket.IdSucursal))
        //        {
        //            return RespuestaErrorValidacion($"ticket.IdSucursal - Not found");
        //        }

        //        var nuevoId = $"{ticket.IdSucursal}{ticket.Folio}";

        //        if (Contexto.ticketSucursal.Any(c=> c.idTicketSucursal == nuevoId ))
        //        {
        //            return RespuestaErrorValidacion($"ticket.Folio - Found");
        //        }

        //        if (!ticket.Moneda.HasValue)
        //        {
        //            ticket.Moneda = 99; //MXN
        //            ticket.TipoCambio = 1m;
        //        }
        //        else
        //        {
        //            if (!ticket.TipoCambio.HasValue)
        //            {
        //                return RespuestaErrorValidacion($"ticket.tipoCambio - Required");
        //            }
        //        }

        //        var guidFactura = InsertarTicketBd(ticket);
        //        SolicitarFactura(guidFactura);
                
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        return RespuestaErrorInterno;
        //    }

        //    return RespuestaOk;
        //}
        //[Route("Tickets"), HttpPost]
        //public AgregarTicketsResponse Post([FromBody]List<Ticket> tickets)
        //{
        //    try
        //    {
        //        var foliosInsertados = new List<string>();
        //        var foliosFallidos = new List<string>();
        //        var erroresValidacion = new List<string>();

        //        var validar = new PermisosApi();
        //        if (!validar.IsAppSecretValid)
        //        {
        //            return RespuestaNoPermisosTickets;
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            return RespuestaErrorValidacionTickets(ModelState);
        //        }

        //        var sucursal = tickets.First().IdSucursal;
        //        if (!Contexto.sucursales.Any(c => c.sucursalPuntoVenta == sucursal))
        //        {
        //            return RespuestaErrorValidacionTickets($"ticket.IdSucursal - Not found");
        //        }


        //        foreach (var ticket in tickets)
        //        {
        //            var nuevoId = $"{ticket.IdSucursal}{ticket.Folio}";

        //            if (Contexto.ticketSucursal.Any(c => c.idTicketSucursal == nuevoId))
        //            {
        //                erroresValidacion.Add($"ticket-{ticket.Folio}.Folio - Found");
        //                foliosFallidos.Add(ticket.Folio);
        //                continue;
        //            }

        //            if (!ticket.Moneda.HasValue)
        //            {
        //                ticket.Moneda = 99; //MXN
        //                ticket.TipoCambio = 1m;
        //            }
        //            else
        //            {
        //                if (!ticket.TipoCambio.HasValue)
        //                {
        //                    erroresValidacion.Add($"ticket-{ticket.Folio}.tipoCambio - Required");
        //                    foliosFallidos.Add(ticket.Folio);
        //                    continue;
        //                }
        //            }

        //            foliosInsertados.Add(ticket.Folio);
        //        }

        //        var ticketsAInsertar = tickets.Where(c => foliosInsertados.Contains(c.Folio)).ToList();
        //        if (!ticketsAInsertar.Any())
        //        {
        //            return RespuestaOkTickets(foliosInsertados, foliosFallidos, erroresValidacion);
        //        }

        //        var guidTickets = InsertarTicketsBd(ticketsAInsertar);
        //        Contexto.SaveChanges();

        //        SolicitarFacturas(guidTickets);

        //        return RespuestaOkTickets(foliosInsertados, foliosFallidos, erroresValidacion);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        return RespuestaErrorInternoTickets;
        //    }
            
        //}

        //#region HELPERS
        //private Guid InsertarTicketBd(Ticket ticket)
        //{
        //    var idTicketNuevo = $"{ticket.IdSucursal}{ticket.Folio}";
        //    var facturaPendiente = Contexto.facturaPendiente
        //        .FirstOrDefault(c=> c.ticket == ticket.Folio 
        //                        && c.sucursales.sucursalPuntoVenta == ticket.IdSucursal 
        //                        && c.pendiente);
        //    var facturaClienteId = Guid.Empty;
        //    var sucursalId = Contexto.sucursales
        //        .Where(c => c.sucursalPuntoVenta == ticket.IdSucursal)
        //        .Select(c => c.idSucursal).FirstOrDefault();
        //    var ticketDb = Contexto.ticketSucursal.Add(new ticketSucursal
        //    {
        //        idTicketSucursal = idTicketNuevo,
        //        catEstatusTicketId = (int) (facturaPendiente == null ?  EstatusTicket.Nuevo : EstatusTicket.Facturado),
        //        claveFormaPago = ticket.ClaveFormaPago,
        //        claveTipoComprobante = ticket.ClaveTipoComprobante,
        //        claveMetodoPago = ticket.ClaveMetodoPago,
        //        fechaRegistro = DateTime.Now,
        //        fechaCompra = ticket.FechaCompra,
        //        sucursalId = sucursalId,
        //        referencia = ticket.Referencia ?? null,
        //        iva = ticket.Iva,
        //        descuento = ticket.Descuento,
        //        subtotal = ticket.Subtotal,
        //        total = ticket.Total,
        //        folio = ticket.Folio,
        //        serieFactura = ticket.SerieFactura,
        //        folioFactura = ticket.FolioFactura,
        //        condicionesPago = ticket.CondicionesPago,
        //        lugarExpedicionFactura = ticket.LugarExpedicion,
        //        moneda = ticket.Moneda.Value,
        //        tipoCambio = ticket.TipoCambio.Value,
        //    });

        //    var conceptos = new List<detalleTicketSucursal>();
        //    foreach (var detalle in ticket.ConceptosConsumo)
        //    {
        //        conceptos.Add(new detalleTicketSucursal
        //        {
        //            valorUnitario = detalle.ValorUnitario,
        //            cantidad = detalle.Cantidad,
        //            claveProdServ = detalle.ClaveProdServ,
        //            claveUnidad = detalle.ClaveUnidad,
        //            descripcion = detalle.Descripcion,
        //            importeDetalle = detalle.ImporteDetalle,
        //            ivaDetalle = detalle.IvaDetalle,
        //            ticketSucursal = ticketDb,
        //            unidad = detalle.Unidad,
        //        });
        //    }

        //    var traslados = new List<trasladosImpuestosTicket>();
        //    foreach (var traslado in ticket.ImpuestoTraslados)
        //    {
        //        traslados.Add(new trasladosImpuestosTicket
        //        {
        //            ticketSucursal = ticketDb,
        //            claveImpuesto = traslado.ClaveImpuesto,
        //            impuesto = traslado.Impuesto,
        //            tasaImpuestos = traslado.Tasa,
        //        });
        //    }

        //    Contexto.trasladosImpuestosTicket.AddRange(traslados);
        //    Contexto.detalleTicketSucursal.AddRange(conceptos);

        //    if (facturaPendiente != null)
        //    {
        //        facturaClienteId = Guid.NewGuid();
        //        Contexto.facturaCliente.Add(new facturaCliente
        //        {
        //            idFacturaClienteId = facturaClienteId,
        //            ticketSucursal =  ticketDb,
        //            fechaRegistro = DateTime.Now,
        //            datoFiscalId = facturaPendiente.datoFiscalId,
        //            receptorClienteId = facturaPendiente.receptorClienteId,
        //            catEstatusFacturaId = (int) EstatusFactura.Pendiente,
        //        });

        //        facturaPendiente.pendiente = false;
        //        Contexto.Entry(facturaPendiente).State = EntityState.Modified;
        //    }
            
        //    Contexto.SaveChanges();
        //    return facturaClienteId;
        //}
        //private List<Guid> InsertarTicketsBd(IReadOnlyCollection<Ticket> tickets)
        //{
        //    var results = new List<Guid>();
        //    var foliosTickets = tickets.Select(c => c.Folio).ToList();
        //    var primerSucursalPv = tickets.FirstOrDefault().IdSucursal;
        //    var sucursalId = Contexto.sucursales
        //        .Where(c => c.sucursalPuntoVenta == primerSucursalPv)
        //        .Select(c => c.idSucursal).FirstOrDefault();
        //    var facturasPendiente = Contexto.facturaPendiente
        //        .Where(c=> foliosTickets.Contains(c.ticket) 
        //                   && c.sucursalId == sucursalId
        //                   && c.pendiente)
        //        .ToList();
            
        //    var ticketsPorAgregar = new List<ticketSucursal>();
        //    var traslados = new List<trasladosImpuestosTicket>();
        //    var conceptos = new List<detalleTicketSucursal>();
            
        //    foreach (var ticket in tickets)
        //    {
        //        var idTicketNuevo = $"{ticket.IdSucursal}{ticket.Folio}";
        //        var facturaPendiente = facturasPendiente.FirstOrDefault(c => c.ticket == ticket.Folio);
        //        var ticketDb = new ticketSucursal
        //        {
        //            idTicketSucursal = idTicketNuevo,
        //            catEstatusTicketId =
        //                (int) (facturaPendiente == null ? EstatusTicket.Nuevo : EstatusTicket.Facturado),
        //            claveFormaPago = ticket.ClaveFormaPago,
        //            claveTipoComprobante = ticket.ClaveTipoComprobante,
        //            claveMetodoPago = ticket.ClaveMetodoPago,
        //            fechaRegistro = DateTime.Now,
        //            fechaCompra = ticket.FechaCompra,
        //            sucursalId = sucursalId,
        //            referencia = ticket.Referencia ?? null,
        //            iva = ticket.Iva,
        //            descuento = ticket.Descuento,
        //            subtotal = ticket.Subtotal,
        //            total = ticket.Total,
        //            folio = ticket.Folio,
        //            serieFactura = ticket.SerieFactura,
        //            folioFactura = ticket.FolioFactura,
        //            condicionesPago = ticket.CondicionesPago,
        //            lugarExpedicionFactura = ticket.LugarExpedicion,
        //            moneda = ticket.Moneda.Value,
        //            tipoCambio = ticket.TipoCambio.Value,
        //        };
        //        ticketsPorAgregar.Add(ticketDb);
                
        //        foreach (var detalle in ticket.ConceptosConsumo)
        //        {
        //            conceptos.Add(new detalleTicketSucursal
        //            {
        //                valorUnitario = detalle.ValorUnitario,
        //                cantidad = detalle.Cantidad,
        //                claveProdServ = detalle.ClaveProdServ,
        //                claveUnidad = detalle.ClaveUnidad,
        //                descripcion = detalle.Descripcion,
        //                importeDetalle = detalle.ImporteDetalle,
        //                ivaDetalle = detalle.IvaDetalle,
        //                ticketSucursal = ticketDb,
        //                unidad = detalle.Unidad,
        //            });
        //        }
                
        //        foreach (var traslado in ticket.ImpuestoTraslados)
        //        {
        //            traslados.Add(new trasladosImpuestosTicket
        //            {
        //                ticketSucursal = ticketDb,
        //                claveImpuesto = traslado.ClaveImpuesto,
        //                impuesto = traslado.Impuesto,
        //                tasaImpuestos = traslado.Tasa,
        //            });
        //        }

                
        //    }
        //    var ticketsDb = Contexto.ticketSucursal.AddRange(ticketsPorAgregar);
        //    Contexto.trasladosImpuestosTicket.AddRange(traslados);
        //    Contexto.detalleTicketSucursal.AddRange(conceptos);

        //    var facturasPorRealizar = new List<facturaCliente>();
        //    foreach (var fpendiente in facturasPendiente)
        //    {
        //        var facturaClienteId = Guid.NewGuid();
        //        var ticketDb = ticketsDb.FirstOrDefault(c => fpendiente.ticket == c.folio);
        //        fpendiente.pendiente = false;
        //        Contexto.Entry(fpendiente).State = EntityState.Modified;
        //        facturasPorRealizar.Add(new facturaCliente
        //        {
        //            idFacturaClienteId = facturaClienteId,
        //            ticketSucursal =  ticketDb,
        //            fechaRegistro = DateTime.Now,
        //            datoFiscalId = fpendiente.datoFiscalId,
        //            receptorClienteId = fpendiente.receptorClienteId,
        //            catEstatusFacturaId = (int) EstatusFactura.Pendiente,
        //        });
        //        results.Add(facturaClienteId);
        //    }
        //    Contexto.facturaCliente.AddRange(facturasPorRealizar);
        //    return results;
        //}
        
        //#endregion
        

        //#region RESPONSES
        //private static ErrorCodeResponseBase RespuestaOk => new ErrorCodeResponseBase
        //{
        //    Success = true,
        //    ResponseCode = (int) ResponseTypes.CodigoOk,
        //};
        //private ErrorCodeResponseBase RespuestaNoPermisos => new ErrorCodeResponseBase
        //{
        //    Success = false,
        //    Message = MensajeNoPermisos,
        //    ResponseCode = (int) ResponseTypes.CodigoPermisos
        //};
        //private ErrorCodeResponseBase RespuestaErrorInterno => new ErrorCodeResponseBase
        //{
        //    Success = false,
        //    Message = MensajeErrorServidor,
        //    ResponseCode = (int) ResponseTypes.CodigoExcepcion
        //};
        //private static ErrorCodeResponseBase RespuestaErrorValidacion(ModelStateDictionary modelState) => new ErrorCodeResponseBase
        //{
        //    Success = false,
        //    Message = string.Join(", ", modelState.Select( f=> $"{f.Key} - { string.Join(" ",f.Value.Errors.Select(c=> $" {c.ErrorMessage} ").ToArray())}" ).ToArray()),
        //    ResponseCode = (int) ResponseTypes.CodigoValidacion
        //};
        //private static ErrorCodeResponseBase RespuestaErrorValidacion(string errors) => new ErrorCodeResponseBase
        //{
        //    Success = false,
        //    Message = errors,
        //    ResponseCode = (int) ResponseTypes.CodigoValidacion
        //};

        //private static AgregarTicketsResponse RespuestaOkTickets(List<string> agregados, List<string> fallidos, IEnumerable<string> errores) => new AgregarTicketsResponse
        //{
        //    Success = true,
        //    ResponseCode = fallidos.Any() ?  (int) ResponseTypes.CodigoOtros : (int) ResponseTypes.CodigoOk,
        //    FoliosInsertados = agregados,
        //    FoliosFallidos = fallidos,
        //    Message = string.Join(", ", errores),
        //};
        //private AgregarTicketsResponse RespuestaNoPermisosTickets => new AgregarTicketsResponse
        //{
        //    Success = false,
        //    Message = MensajeNoPermisos,
        //    ResponseCode = (int) ResponseTypes.CodigoPermisos
        //};
        //private AgregarTicketsResponse RespuestaErrorInternoTickets => new AgregarTicketsResponse
        //{
        //    Success = false,
        //    Message = MensajeErrorServidor,
        //    ResponseCode = (int) ResponseTypes.CodigoExcepcion
        //};
        //private static AgregarTicketsResponse RespuestaErrorValidacionTickets(ModelStateDictionary modelState) => new AgregarTicketsResponse
        //{
        //    Success = false,
        //    Message = string.Join(", ", modelState.Select( f=> $"{f.Key} - { string.Join(" ",f.Value.Errors.Select(c=> $" {c.ErrorMessage} ").ToArray())}" ).ToArray()),
        //    ResponseCode = (int) ResponseTypes.CodigoValidacion
        //};
        //private static AgregarTicketsResponse RespuestaErrorValidacionTickets(string errors) => new AgregarTicketsResponse
        //{
        //    Success = false,
        //    Message = errors,
        //    ResponseCode = (int) ResponseTypes.CodigoValidacion
        //};
        //#endregion
    }
}

