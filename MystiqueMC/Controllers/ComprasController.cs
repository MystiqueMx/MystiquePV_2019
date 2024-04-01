using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp.Web.Models.Compras;
using WebApp.Web.Models.Compras.JSON;

namespace MystiqueMC.Controllers
{
    [Authorize]
    public class ComprasController : BaseController
    {
        private Helpers.ControlInventario controlInventarioHelper = new ControlInventario();
        #region GET
        [HttpGet]
        public ActionResult Index(int? s, int? p, string fi, string ff, int? ec)
        {
            try
            {
                var compras = SucursalesFirmadas
                    .Include(c => c.Compras)
                    .Include(c => c.Compras.Select(d => d.CatEstatusCompras))
                    .Include(c => c.Compras.Select(d => d.sucursales))
                    .Include(c => c.Compras.Select(d => d.Proveedores))
                    .SelectMany(c => c.Compras)
                    .Where(c => c.estatusCompraId == 1)
                    .AsQueryable();

                if (ec.HasValue)
                {
                    compras = SucursalesFirmadas
                    .Include(c => c.Compras)
                    .Include(c => c.Compras.Select(d => d.CatEstatusCompras))
                    .Include(c => c.Compras.Select(d => d.sucursales))
                    .Include(c => c.Compras.Select(d => d.Proveedores))
                    .SelectMany(c => c.Compras)
                    .AsQueryable();

                    compras = compras.Where(c => c.estatusCompraId == ec);
                }

                if (s.HasValue)
                {
                    compras = compras.Where(c => c.sucursalId == s);
                }

                if (p.HasValue)
                {
                    compras = compras.Where(c => c.proveedorId == p);
                }

                

                if (!string.IsNullOrEmpty(fi) && DateTime.TryParseExact(fi,
                           format: "dd/MM/yyyy",
                           provider: System.Globalization.CultureInfo.InvariantCulture,
                           style: System.Globalization.DateTimeStyles.AssumeLocal,
                           result: out DateTime fechaInicio))
                {
                    ViewBag.fi = fechaInicio;
                    compras = compras.Where(c => DbFunctions.TruncateTime(c.fechaRegistro) >= DbFunctions.TruncateTime(fechaInicio));
                }

                if (!string.IsNullOrEmpty(ff) && DateTime.TryParseExact(ff,
                           format: "dd/MM/yyyy",
                           provider: System.Globalization.CultureInfo.InvariantCulture,
                           style: System.Globalization.DateTimeStyles.AssumeLocal,
                           result: out DateTime fechaFin))
                {
                    ViewBag.ff = fechaFin;
                    compras = compras.Where(c => DbFunctions.TruncateTime(c.fechaRegistro) <= DbFunctions.TruncateTime(fechaFin));
                }

                ViewBag.Proveedores = ObtenerProveedores(p);
                ViewBag.Sucursales = ObtenerSucursales(s);
                ViewBag.EstatusCompra = ObtenerEstatusCompra(ec);
                ViewBag.sucursalId = s;

                var listadoCompras = compras
                    .OrderByDescending(c => c.fechaRegistro)
                    .Select(c => new ComprasIndexViewModel
                    {
                        Id = c.idCompra,
                        FechaCaptura = c.fechaRegistro,
                        Observaciones = c.observaciones,
                        Proveedor = c.Proveedores.descripcion,
                        Sucursal = c.sucursales.nombre,
                        Total = c.total,
                        Estatus = c.CatEstatusCompras.descripcion,
                        Editable = c.estatusCompraId == (int)EstatusCompras.Pendiente,
                    }).ToArray();

                return View(listadoCompras);
            }
            catch (Exception e)
            {
                ShowAlertException(e);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Create(int? id, int? sucursalid)
        {
            try
            {

                if (id.HasValue)
                {
                    var compra = Contexto.Compras
                        .Include(c => c.DetalleCompra)
                        .Include(c => c.DetalleCompra.Select(d => d.Insumos))
                        .Include(c => c.DetalleCompra.Select(d => d.Insumos.UnidadMedida1))
                        .First(c => c.idCompra == id);
                    var insumos = compra.DetalleCompra.Select(c => c.insumoId).ToArray();
                    ViewBag.Sucursales = ObtenerSucursales(selected: compra.sucursalId);
                    ViewBag.Proveedores = ObtenerProveedores(selected: compra.proveedorId);
                    ViewBag.Insumos = ObtenerInsumos(except: insumos);
                    ViewBag.UnidadMedida = ObtenerUnidadesMedida();

                    if (sucursalid > 0)
                    {
                        ViewBag.sucursalId = sucursalid;
                    }

                    return View(new RegistroCompraViewModel
                    {
                        id = id,
                        remision = compra.noRemision,
                        observacion = compra.observaciones,
                        factura = compra.noFactura,
                        fechaCompra = compra.fechaCompra,
                        estatusCompra = compra.estatusCompraId,
                        descuentos = compra.descuento,
                        iva = compra.iva,
                        total = compra.total,                        
                        detalle = compra.DetalleCompra.Select(c => new DetalleRegistroCompra
                        {
                            idDetalleCompra = c.idDetalleCompra,
                            cantidad = (int)c.cantidad,
                            compra = c.compraId,
                            desc = c.Insumos.nombre,
                            insumo = c.insumoId,
                            importe = c.importe,
                            unidad = c.Insumos.UnidadMedida1.descripcion
                        }).ToArray()
                    });
                }
                else
                {
                    ViewBag.Sucursales = ObtenerSucursales(sucursalid);
                    ViewBag.Insumos = ObtenerInsumos();
                    ViewBag.Proveedores = ObtenerProveedores();
                    ViewBag.UnidadMedida = ObtenerUnidadesMedida();

                    if (sucursalid > 0)
                    {
                        ViewBag.sucursalId = sucursalid;
                    }

                    return View(new RegistroCompraViewModel());
                }
            }
            catch (Exception e)
            {
                ShowAlertException(e);
                return RedirectToAction("Index");
            }
        }
        #endregion
        #region POST

        #endregion
        #region AJAX
        [HttpPost]
        public ActionResult RegistrarCompra(RegistroCompraViewModel viewModel)
        {
            try
            {
                var compra = Contexto.Compras.Add(new DAL.Compras
                {
                    activo = true,
                    descuento = 0,
                    fechaRegistro = DateTime.Now,
                    iva = 0,
                    noFactura = viewModel.factura,
                    noRemision = viewModel.remision,
                    observaciones = viewModel.observacion,
                    sucursalId = viewModel.sucursal,
                    proveedorId = viewModel.proveedor,
                    total = 0,
                    usuarioRegistroId = IdUsuarioActual,
                    estatusCompraId = (int)EstatusCompras.Pendiente,
                    fechaCompra = viewModel.fechaCompra
                });

                Contexto.SaveChanges();
                ShowAlertSuccess("La compra ha sido guardada con éxito.");
                return Json(new { id = compra.idCompra });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult VerDetalleCompra(int id)
        {
            try
            {
                var compra = Contexto.Compras
                     .Include(c => c.sucursales)
                     .Include(c => c.Proveedores)
                     .Include(c => c.DetalleCompra)
                     .Include(c => c.DetalleCompra.Select(d => d.Insumos))
                     .First(c => c.idCompra == id
                         && SucursalesFirmadas.Any(d => d.idSucursal == c.sucursalId));

                return Json(new
                {
                    result = new DetalleCompraViewModel
                    {
                        total = compra.total.ToString("N2"),
                        //total = $"{compra.total.ToString("C")} MXN",
                        //iva = $"{compra.iva.ToString("C")} MXN",
                        iva = compra.iva.ToString("N2"),
                        //descuentos = $"{compra.descuento.ToString("C")} MXN",
                        descuentos = compra.descuento.ToString("N2"),
                        remision = compra.noRemision,
                        factura = compra.noFactura,
                        observacion = compra.observaciones,
                        proveedor = compra.Proveedores.descripcion,
                        sucursal = compra.sucursales.nombre,
                        fechaCompra = compra.fechaCompra.HasValue ? compra.fechaCompra.Value.ToShortDateString() : null,
                        detalle = compra.DetalleCompra.Select(c => new DetalleCompraJson
                        {
                            //importe = $"{c.importe.ToString("C")} MXN",
                            importe = c.importe.ToString("N2"),
                            insumo = c.Insumos.nombre,
                        }).ToArray()
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult ValidarInsumoCompra(DetalleRegistroCompra viewModel)
        {
            try
            {
                if (Contexto.DetalleCompra
                    .Any(c => c.insumoId == viewModel.insumo && c.cantidad > 0))
                {
                    var cfg = Contexto.ConfiguracionCompras.First();
                    var margenDiferencia = cfg.porcentajeDiferenciaEntreCompras / 100;
                    var compraAnterior = Contexto.DetalleCompra
                        .Include(c => c.Compras)
                        .OrderByDescending(c => c.Compras.fechaRegistro)
                        .First(c => c.insumoId == viewModel.insumo && c.cantidad > 0);

                    var precioUnitarioActual = viewModel.importe / viewModel.cantidad;
                    var precioUnitarioAnterior = compraAnterior.importe / compraAnterior.cantidad;

                    var margenPrecioSuperior = precioUnitarioAnterior * (1 + margenDiferencia); // 100 * (1 + .2) == 120
                    var margenPrecioInferior = precioUnitarioAnterior * (1 - margenDiferencia); // 100 * (1 - .2) == 80

                    var esValido = precioUnitarioActual >= margenPrecioInferior && precioUnitarioActual <= margenPrecioSuperior;

                    return Json(new
                    {
                        anterior = precioUnitarioAnterior,
                        actual = precioUnitarioActual,
                        success = esValido
                    });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult AgregarInsumo(DetalleRegistroCompra viewModel)
        {
            try
            {
                DAL.DetalleCompra detalle;
                if (!viewModel.compra.HasValue) throw new ApplicationException("!viewModel.compra.HasValue");              
                
                Insumos insumo = Contexto.Insumos.First(c => c.idInsumo == viewModel.insumo);
               
                if (viewModel.idDetalleCompra > 0)
                {
                    detalle = Contexto.DetalleCompra.FirstOrDefault(f => f.idDetalleCompra == viewModel.idDetalleCompra);
                    detalle.cantidad = viewModel.cantidad;
                    detalle.importe = viewModel.importe;
                    detalle.precioUnitario = viewModel.importe / viewModel.cantidad;
                    
                    Contexto.Entry(detalle).State = EntityState.Modified;                   

                }
                else
                {                    
                    detalle = Contexto.DetalleCompra.Add(new DetalleCompra
                    {
                        cantidad = viewModel.cantidad,
                        insumoId = viewModel.insumo,
                        importe = viewModel.importe,
                        compraId = viewModel.compra.Value,
                        precioUnitario = viewModel.importe / viewModel.cantidad,
                        unidadCompraId = insumo.UnidadMedida1.idUnidadMedida //Unidad de Medida compra
                    });
                }

                var desc = insumo.nombre;
                var unidad = insumo.UnidadMedida1.descripcion; //Unidad de Medida compra

                Contexto.SaveChanges();

                return Json(new
                {
                    detalle.idDetalleCompra,
                    desc,
                    viewModel.cantidad,
                    viewModel.importe,
                    viewModel.insumo,
                    viewModel.compra,
                    unidad 
                });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult ObtenerUnidadMedida(int id)
        {
            try
            {
                var insumo = Contexto.Insumos
                    .Include(c => c.UnidadMedida1)
                    .First(c => c.idInsumo == id);
                return Json(new SelectListItem
                {
                    Text = insumo.UnidadMedida1.descripcion,
                    Value = insumo.unidadMedidaIdMinima.ToString(),
                });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult EliminarDetalleCompra(int insumo, int compra)
        {
            try
            {
                var detalle = Contexto.DetalleCompra.First(c => c.insumoId == insumo && c.compraId == compra);
                Contexto.DetalleCompra.Remove(detalle);
                Contexto.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult ConfirmarCierreCompra(RegistroCompraViewModel viewModel)
        {
            try
            {
                var compra = Contexto.Compras.Include(c => c.DetalleCompra).First(c => c.idCompra == viewModel.id);
                compra.iva = viewModel.iva;
                compra.total = viewModel.total;
                compra.descuento = viewModel.descuentos;
                compra.observaciones = viewModel.observacion;

                bool hasError = false;

                //Generar movimientos negativos con los detalles de compra reapertura
                foreach (DetalleCompraReapertura detalle in compra.DetalleCompraReapertura)
                {
                    var inventarioInsumo = Contexto.Inventarios.FirstOrDefault(f => f.sucursalId == compra.sucursalId && f.insumoId == detalle.insumoId);

                    hasError = controlInventarioHelper.registroMovimientoInventario(Contexto, null, detalle.cantidad * -1, detalle.insumoId,
                            compra.sucursalId, (inventarioInsumo != null ? inventarioInsumo.cantidad : 0) + detalle.cantidad, UsuarioActual, 
                            TiposMovimientosInventario.Compra, 
                            null,//detalle.idDetalleCompraReapertura.idDetalleCompra,
                            "Ajuste por reapertura de compra.",
                            //"Fecha Compra:" + compra.fechaCompra.Value.ToShortDateString() +
                            //" Costo: " + detalle.importe +
                            //(String.IsNullOrEmpty(compra.noRemision) ? "" : " Remision: " + compra.noRemision) +
                            //(String.IsNullOrEmpty(compra.noFactura) ? "" : " Factura: " + compra.noFactura) +
                            //(String.IsNullOrEmpty(compra.observaciones) ? "" : " Observaciones: " + compra.observaciones)
                            false).hasError;

                    detalle.procesado = true;
                    Contexto.Entry(detalle).State = EntityState.Modified;
                }


                //Generar movimientos con los detalles actuales de la compra
                foreach (var detalle in compra.DetalleCompra)
                {
                    var inventarioInsumo = Contexto.Inventarios.FirstOrDefault(f => f.sucursalId == compra.sucursalId && f.insumoId == detalle.insumoId);

                    hasError = controlInventarioHelper.registroMovimientoInventario(Contexto, null, detalle.cantidad, detalle.insumoId, 
                        compra.sucursalId, (inventarioInsumo != null ? inventarioInsumo.cantidad : 0) + detalle.cantidad, UsuarioActual, TiposMovimientosInventario.Compra, detalle.idDetalleCompra,
                        "Fecha Compra:" + compra.fechaCompra.Value.ToShortDateString() + 
                        " Costo: " + detalle.importe + 
                        (String.IsNullOrEmpty(compra.noRemision)? "" : " Remision: " + compra.noRemision) +
                        (String.IsNullOrEmpty(compra.noFactura) ? "" : " Factura: " + compra.noFactura) +
                        (String.IsNullOrEmpty(compra.observaciones) ? "" : " Observaciones: " + compra.observaciones),false).hasError;
                }

                compra.estatusCompraId = (int)EstatusCompras.Cerrada;
                Contexto.Entry(compra).State = EntityState.Modified;

                if (!hasError)
                {
                    Contexto.SaveChanges();
                }                   
                else
                {
                    Logger.Error($"No fue posible actualizar el inventario de la compra: {compra.idCompra}, por reapertura.");
                }

                return Json(new { id = compra.idCompra });

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult ReabrirCompra(int idCompra)
        {
            try
            {
                var compra = Contexto.Compras.Include(c => c.DetalleCompra).First(c => c.idCompra == idCompra);
               
                compra.estatusCompraId = (int)EstatusCompras.Reapertura;               

                //clonar detalles compra actual a detalles compra reapertura
                foreach (DetalleCompra detalle in compra.DetalleCompra)
                {
                    compra.DetalleCompraReapertura.Add(new DetalleCompraReapertura {
                         insumoId = detalle.insumoId,
                         cantidad = detalle.cantidad,
                         precioUnitario = detalle.precioUnitario,
                         importe = detalle.importe,
                         unidadCompraId = detalle.unidadCompraId,
                         procesado = false
                    });                  
                }

                Contexto.Entry(compra).State = EntityState.Modified;
                Contexto.SaveChanges();
                
                return RedirectToAction("Create", new { id = compra.idCompra });              

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        #endregion
        #region HELPERS
        [ValidateInput(false)]
        public ActionResult ValidarFechaCompra(string fecha)
        {
            var isValid = false;
            var fechaSelected = Convert.ToDateTime(fecha);

            var minDate = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1);

            if (fechaSelected.Date <= DateTime.Now.Date && fechaSelected.Date >= minDate.Date)
            {
                isValid = true;
            }

            return isValid ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
        public SelectListItem[] ObtenerInsumos(int? selected = null, int[] except = null)
        {
            var query = InsumosVisibles;
            query = query.Where(w => w.activo && !w.esAgrupador);

            query = query.Where(w => !w.ConteoFisicoInsumos.Any(a => a.insumoId == w.idInsumo));
            //query = query.Where(w => !w.ConteoFisicoAgrupadorInsumos.Any(a => a.insumoId == w.idInsumo));

            if (except != null && except.Any())
            {
                query = query.Where(c => (c.aplicaInventario == true && c.esProcesado == false && c.esAgrupador == false) && !except.Contains(c.idInsumo));
            }
            else{
                query = query.Where(c => (c.aplicaInventario == true && c.esProcesado == false && c.esAgrupador == false) );
            }
            query = query.OrderBy(o => o.nombre);
            return query
                .Select(c => new SelectListItem
                {
                    Text = c.nombre,
                    Value = c.idInsumo.ToString(),
                    Selected = c.idInsumo == selected
                }).ToArray();
        }

        public SelectListItem[] ObtenerInsumosPorRubro(int? selected, int idRubro)
        {
            var query = InsumosVisibles;
            query = query.Where(w => w.activo && !w.esAgrupador && w.catRubroId == idRubro);

            query = query.Where(w => !w.ConteoFisicoInsumos.Any(a => a.insumoId == w.idInsumo));
            
            query = query.Where(c => (c.aplicaInventario == true && c.esProcesado == false && c.esAgrupador == false));
            
            query = query.OrderBy(o => o.nombre);
            return query
                .Select(c => new SelectListItem
                {
                    Text = c.nombre,
                    Value = c.idInsumo.ToString(),
                    Selected = c.idInsumo == selected
                }).ToArray();
        }

        private SelectListItem[] ObtenerUnidadesMedida()
        {
            return ComerciosFirmados
                .Include(c => c.UnidadMedida)
                .SelectMany(c => c.UnidadMedida)
                .Select(c => new SelectListItem
                {
                    Text = c.descripcion,
                    Value = c.idUnidadMedida.ToString(),
                }).ToArray();
        }   
        public SelectListItem[] ObtenerEstatusCompra(int? selected = null)
        {
            return Contexto.CatEstatusCompras
                .Select(c => new SelectListItem
                {
                    Text = c.descripcion,
                    Value = c.idCatEstatusCompras.ToString(),
                    Selected = c.idCatEstatusCompras == selected
                }).ToArray();
        }
        #endregion
    }
}