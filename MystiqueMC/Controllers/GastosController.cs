using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MystiqueMC.Helpers;
using MystiqueMC.DAL;
using MystiqueMC.Models.Gastos;
using WebApp.Web.Models.Compras;
using MystiqueMC.Models;

namespace MystiqueMC.Controllers
{
    [Authorize]
    public class GastosController : BaseController
    {
        private ControlInventario controlInventarioHelper;
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">sucursalId</param>
        /// <param name="p">proveedorId</param>
        /// <param name="cg">categoria gastoId</param>
        /// <param name="fgi">fecha gasto inicio</param>
        /// <param name="fgf">fecha gasto fin</param>
        /// <returns></returns>
        /// 


        // GET: /Gastos/
        public ActionResult Index(int? s, int? p, int? cg, string fgi, string fgf)
        {
            try
            {
                var gastos = SucursalesFirmadas
                                               .Include(c => c.Gastos)
                                               .Include(c => c.Gastos.Select(g => g.CatConceptosGastos))
                                               .Include(c => c.Gastos.Select(g => g.sucursales))
                                               .Include(c => c.Gastos.Select(g => g.Proveedores))
                                               .SelectMany(c => c.Gastos)
                                               .AsQueryable();

                //Filtro por sucursal 
                if (s.HasValue)
                {
                    gastos = gastos.Where(c => c.sucursalId == s);
                }

                //Filtro por proveedor
                if (p.HasValue)
                {
                    gastos = gastos.Where(c => c.proveedorId == p);
                }

                //Filtro por concepto de gasto
                if (cg.HasValue)
                {
                    gastos = SucursalesFirmadas
                                                .Include(c => c.Gastos)
                                                .Include(c => c.Gastos.Select(g => g.CatConceptosGastos))
                                                .Include(c => c.Gastos.Select(g => g.sucursales))
                                                .Include(c => c.Gastos.Select(g => g.Proveedores))
                                                .SelectMany(c => c.Gastos)
                                                .AsQueryable();

                    gastos = gastos.Where(c => c.catConceptoGastoId == cg);
                }

                //Filtro por fecha de gasto
                if (!string.IsNullOrEmpty(fgi) && DateTime.TryParseExact(fgi,
                           format: "dd/MM/yyyy",
                           provider: System.Globalization.CultureInfo.InvariantCulture,
                           style: System.Globalization.DateTimeStyles.AssumeLocal,
                           result: out DateTime fechaGastoInicio))
                {
                    ViewBag.fgi = fechaGastoInicio;
                    gastos = gastos.Where(c => DbFunctions.TruncateTime(c.fechaGasto) >= DbFunctions.TruncateTime(fechaGastoInicio));
                }

                if (!string.IsNullOrEmpty(fgf) && DateTime.TryParseExact(fgf,
                           format: "dd/MM/yyyy",
                           provider: System.Globalization.CultureInfo.InvariantCulture,
                           style: System.Globalization.DateTimeStyles.AssumeLocal,
                           result: out DateTime fechaGastoFin))
                {
                    ViewBag.fgf = fechaGastoFin;
                    gastos = gastos.Where(c => DbFunctions.TruncateTime(c.fechaGasto) <= DbFunctions.TruncateTime(fechaGastoFin));
                }

                ViewBag.ConceptosGasto = ObtenerConceptosGasto(cg);
                ViewBag.Sucursales = ObtenerSucursales(s);
                ViewBag.Proveedores = ObtenerProveedores(p);

                var listadoGastos = gastos
                    .OrderByDescending(g => g.fechaGasto)
                    .Select(g => new GastosIndexViewModel
                    {
                        IdGasto = g.idGasto,
                        ProveedorId = g.proveedorId.Value,
                        Proveedor = g.proveedorId.HasValue ? g.Proveedores.descripcion : null,
                        SucursalId = g.sucursalId,
                        Sucursal = g.sucursales.nombre,
                        catConceptoGastoId = g.catConceptoGastoId,
                        catConceptoGasto = g.CatConceptosGastos.descripcion,
                        Monto = g.monto,
                        Observacion = g.obervacion,
                        FechaGasto = g.fechaGasto,
                    }).ToArray();

                return View(listadoGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }

        }

        // GET: /Gastos/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.ConceptosGasto = ObtenerConceptosGasto();
                ViewBag.Sucursales = ObtenerSucursales();
                ViewBag.Proveedores = ObtenerProveedores();

                return View(new Gastos());

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
                return RedirectToAction("Menu", "Egresos");
            }
        }

        // POST: /Gastos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idGasto,sucursalId,catConceptoGastoId,proveedorId,monto,obervacion,fechaGasto")] Gastos gasto)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();

                gasto.fechaRegistro = DateTime.Now;
                gasto
                    .usuarioRegistroId = usuarioFirmado.idUsuario;

                if (ModelState.IsValid)
                {
                    Contexto.Gastos.Add(gasto);

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.ConceptosGasto = ObtenerConceptosGasto();
                ViewBag.Sucursales = ObtenerSucursales();
                ViewBag.Proveedores = ObtenerProveedores();

                return View(gasto);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
                return RedirectToAction("Menu", "Egresos");
            }

        }

        // GET: /Gastos/Edit/5
        // GET: /Gastos/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    ShowAlertDanger("No se encontró el gasto seleccionado.");
                    return RedirectToAction("Index");
                }

                Gastos gastos = Contexto.Gastos.Find(id);

                if (gastos == null)
                {
                    ShowAlertDanger("No se encontró el gasto seleccionado.");
                    return RedirectToAction("Index");
                }

                ViewBag.catConceptoGastoId = new SelectList(ObtenerConceptosGasto(), "Value", "Text", gastos.catConceptoGastoId);
                ViewBag.Sucursales = new SelectList(ObtenerSucursales(), "Value", "Text", gastos.sucursalId);
                ViewBag.Proveedores = new SelectList(ObtenerProveedores(), "Value", "Text", gastos.proveedorId);

                return View(gastos);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
                return RedirectToAction("Menu", "Egresos");
            }
        }



        // POST: /Gastos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idGasto,sucursalId,catConceptoGastoId,proveedorId,monto,obervacion,fechaGasto,usuarioRegistroId,fechaRegistro")] Gastos gasto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Contexto.Entry(gasto).State = EntityState.Modified;

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.ConceptosGasto = ObtenerConceptosGasto();
                ViewBag.Sucursales = ObtenerSucursales();
                ViewBag.Proveedores = ObtenerProveedores();

                return View(gasto);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
                return RedirectToAction("Menu", "Egresos");
            }

        }

        // GET: /Gastos/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    ShowAlertDanger("No se encontró el gasto seleccionado.");
                    RedirectToAction("Index");
                }

                Gastos gastos = Contexto.Gastos.Find(id);

                if (gastos == null)
                {
                    return HttpNotFound();
                }
                return View(gastos);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
                return RedirectToAction("Menu", "Egresos");
            }
        }

        // POST: /Gastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Gastos gastos = Contexto.Gastos.Find(id);

                Contexto.Gastos.Remove(gastos);

                Contexto.SaveChanges();

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
                return RedirectToAction("Menu", "Egresos");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">sucursalId</param>
        /// <param name="fgi">fecha gasto venta inicio</param>
        /// <param name="fgf">fecha gasto venta fin</param>
        /// <returns></returns>
        // GET: /Gastos/
        public ActionResult IndexValidar(int? s, string fgi, string fgf)
        {
            try
            {
                var gastosVentaValidar = SucursalesFirmadas
                                                    .Include(c => c.Ventas)
                                                    .Include(c => c.Ventas.Select(v => v.Aperturas))
                                                    .Include(c => c.Ventas.Select(v => v.sucursales))
                                                    .SelectMany(c => c.Ventas)
                                                    .Where(v => v.Aperturas.SelectMany(gv => gv.GastosPv).Any(g => !g.aplicado))
                                                    .AsQueryable();

                //Filtro por sucursal 
                if (s.HasValue)
                {
                    gastosVentaValidar = gastosVentaValidar.Where(c => c.sucursalId == s);
                }

                //Filtro por fecha de gasto
                if (!string.IsNullOrEmpty(fgi) && DateTime.TryParseExact(fgi,
                           format: "dd/MM/yyyy",
                           provider: System.Globalization.CultureInfo.InvariantCulture,
                           style: System.Globalization.DateTimeStyles.AssumeLocal,
                           result: out DateTime fechaGastoInicio))
                {
                    ViewBag.fgi = fechaGastoInicio;
                    gastosVentaValidar = gastosVentaValidar.Where(c => DbFunctions.TruncateTime(c.fechaRegistroVenta) >= DbFunctions.TruncateTime(fechaGastoInicio));
                }

                if (!string.IsNullOrEmpty(fgf) && DateTime.TryParseExact(fgf,
                           format: "dd/MM/yyyy",
                           provider: System.Globalization.CultureInfo.InvariantCulture,
                           style: System.Globalization.DateTimeStyles.AssumeLocal,
                           result: out DateTime fechaGastoFin))
                {
                    ViewBag.fgf = fechaGastoFin;
                    gastosVentaValidar = gastosVentaValidar.Where(c => DbFunctions.TruncateTime(c.fechaRegistroVenta) <= DbFunctions.TruncateTime(fechaGastoFin));
                }

                ViewBag.Sucursales = ObtenerSucursales(s);

                var listadoGastos = gastosVentaValidar
                    .OrderByDescending(v => v.fechaRegistroVenta)
                    .Select(v => new GastosIndexValidarViewModel
                    {
                        IdVenta = v.idVenta,
                        FechaRegistroVenta = v.fechaRegistroVenta,
                        SucursalId = v.sucursalId,
                        Sucursal = v.sucursales.nombre,
                        Folio = v.folio,
                        Monto = v.Aperturas.SelectMany(gv => gv.GastosPv).Sum(gpv => gpv.monto)
                    })

                    .ToArray();

                return View(listadoGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">idVenta</param>
        /// <returns></returns>
        // GET: IndexValidarDetalle
        public ActionResult IndexValidarDetalle(int id)
        {
            try
            {
                if (id <= 0)
                {
                    ShowAlertDanger("No se encontró la venta seleccionada.");
                    RedirectToAction("Index");
                }

                ValidarGastosDetallesViewModel ventaVM = Contexto.Ventas
                                            .Include(v => v.sucursales)
                                            .Include(v => v.Aperturas)
                                            .Include(v => v.Aperturas.Select(g => g.GastosPv))
                                            .Where(v => v.idVenta == id)
                                            .Select(v => new ValidarGastosDetallesViewModel
                                            {
                                                IdVenta = v.idVenta,
                                                FechaRegistroVenta = v.fechaRegistroVenta,
                                                SucursalId = v.sucursalId,
                                                Sucursal = v.sucursales.nombre,
                                                Folio = v.folio,
                                                Monto = v.Aperturas.SelectMany(gv => gv.GastosPv).Sum(gpv => gpv.monto),

                                                Aperturas = v.Aperturas.Select(ap => new AperturaViewModel
                                                {
                                                    IdApertura = ap.idApertura,
                                                    AperturaIdSucursal = ap.aperturaIdSucursal,
                                                    Activo = ap.activo,
                                                    FechaRegistroApertura = ap.fechaRegistroApertura,
                                                    FechaInicial = ap.fechaInicial,
                                                    FechaFinal = ap.fechaFinal,
                                                    TipoCambio = ap.tipoCambio,
                                                    Fondo = ap.fondo,
                                                    uuidApertura = ap.uuidApertura,
                                                    VentaId = ap.ventaId,
                                                    UsuarioRegistro = ap.usuarioRegistro,
                                                    UsuarioAutorizo = ap.usuarioAutorizo,
                                                    FechaRegistro = ap.fechaRegistro,

                                                    GastosPv = ap.GastosPv.Select(gpv => new GastosPvViewModel
                                                    {
                                                        IdGastoPv = gpv.idGastoPv,
                                                        VentaId = v.idVenta,
                                                        SucursalId = v.sucursalId,
                                                        GastoIdSucursal = gpv.gastoIdSucursal,
                                                        AperturaId = gpv.aperturaId,
                                                        UsuarioRegistro = gpv.usuarioRegistro,
                                                        Monto = gpv.monto,
                                                        FechaRegistro = gpv.fechaRegistro,
                                                        Observaciones = gpv.observaciones,
                                                        TipoGasto = gpv.tipoGasto,
                                                        Aplicado = gpv.aplicado
                                                    }).ToList()

                                                }).ToList()

                                            }).First();

                if (ventaVM == null)
                {
                    ShowAlertDanger("No se encontró la venta seleccionada.");
                }


                return View(ventaVM);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
                return RedirectToAction("Menu", "Egresos");
            }
        }

        public ActionResult ValidarDetalle(int id, int idv)
        {
            try
            {
                GastosPv gasto = Contexto.GastosPv.Find(id);

                if (gasto == null)
                {
                    return Json(new { success = false, hasException = true, message = "Gasto no encontrado." }, JsonRequestBehavior.AllowGet);
                }

                Ventas venta = Contexto.Ventas.Find(idv);

                GastoPvValidarViewModel gastoPvValidarViewModel = new GastoPvValidarViewModel
                {
                    IdGastoPv = gasto.idGastoPv,
                    VentaId = idv,
                    FechaVenta = venta.fechaRegistroVenta,
                    FolioVenta = venta.folio,
                    SucursalId = venta.sucursalId,
                    Sucursal = venta.sucursales.nombre,
                    UsuarioRegistro = gasto.usuarioRegistro,
                    Monto = gasto.monto,
                    FechaRegistro = gasto.fechaRegistro,
                    Observaciones = gasto.observaciones,
                    TipoGasto = gasto.tipoGasto,
                    Aplicado = gasto.aplicado,
                    ObservacionesValidar = $"{gasto.observaciones} {gasto.tipoGasto}"
                };

                //if (gasto.aplicado)
                //{                    
                //    Gastos gastoAplicado = Contexto.Gastos.FirstOrDefault(g => g.GastoPvId == gasto.idGastoPv);

                //    gastoPvValidarViewModel.MontoValidado = gastoAplicado.monto;
                //    gastoPvValidarViewModel.FechaGasto = gastoAplicado.fechaGasto;
                //    gastoPvValidarViewModel.ProveedorId = gastoAplicado.proveedorId;
                //    gastoPvValidarViewModel.NoRemision = gastoAplicado.noRemision;
                //    gastoPvValidarViewModel.NoFactura = gastoAplicado.noFactura;
                //    gastoPvValidarViewModel.ObservacionesValidar = gastoAplicado.obervacion;
                //    gastoPvValidarViewModel.catConceptoGastoId = gastoAplicado.catConceptoGastoId;
                //    gastoPvValidarViewModel.catRubroId = gastoAplicado.CatConceptosGastos.catRubroId;

                //}               

                ViewBag.Rubros = ObtenerRubros(gastoPvValidarViewModel.catRubroId, null, true);
                ViewBag.catRubros = ObtenerCatRubros();
                ViewBag.ConceptosGasto = ObtenerConceptosGasto(gastoPvValidarViewModel.catConceptoGastoId);
                ViewBag.Proveedores = ObtenerProveedores(gastoPvValidarViewModel.ProveedorId);

                return View(gastoPvValidarViewModel);

                //return PartialView("_ModalValidarDetalle", gastoVM);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");

                //return Json(new { success = false, hasException = true, message = ObtenerExInfo(ex) }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidarDetalle(GastoPvValidarViewModel gastoPvValidarViewModel, bool? showNext)
        {
            try
            {
                bool hasError = false;
                int gastosPvId;
                //ValidarGasto(gastoPvValidarViewModel);

                if (ModelState.IsValid)
                {
                    //Obtener gastosPv y GastosPvDetalles

                    GastosPv gastosPv = Contexto.GastosPv.Find(gastoPvValidarViewModel.IdGastoPv);
                    gastosPvId = gastosPv.idGastoPv;

                    //Agrupar, grupo True = costo, false = Gasto

                    //Gastos
                    List<GastosPvDetalle> gastos = gastosPv.GastosPvDetalle.Where(d => !d.CatConceptosGastos.CatRubros.esCosto).OrderBy(d => d.idGastoPvDetalle).ToList();
                    //Costos
                    List<GastosPvDetalle> costos = gastosPv.GastosPvDetalle.Where(d => d.CatConceptosGastos.CatRubros.esCosto).OrderBy(d => d.idGastoPvDetalle).ToList();

                    //Crear gastos

                    foreach (GastosPvDetalle gasto in gastos)
                    {
                        Gastos nuevoGasto = new Gastos
                        {
                            sucursalId = gasto.sucursalId,
                            catConceptoGastoId = gasto.catConceptoGastoId,
                            GastoPvId = gastosPv.idGastoPv,
                            noRemision = !string.IsNullOrEmpty(gastoPvValidarViewModel.NoRemision) ? gastoPvValidarViewModel.NoRemision : null,
                            noFactura = !string.IsNullOrEmpty(gastoPvValidarViewModel.NoFactura) ? gastoPvValidarViewModel.NoFactura : null,
                            monto = gasto.monto,
                            obervacion = gastoPvValidarViewModel.ObservacionesValidar,
                            fechaGasto = gastoPvValidarViewModel.FechaGasto,
                            usuarioRegistroId = IdUsuarioActual,
                            fechaRegistro = DateTime.Now
                        };

                        //gasto

                        Contexto.Gastos.Add(nuevoGasto);

                    }

                    //Crear compra con detalles segun los insumos
                    if (costos.Any())
                    {
                        decimal totalCompra = costos.Sum(c => c.monto);
                        //crear compra
                        Compras compra = new Compras
                        {
                            activo = true,
                            descuento = gastoPvValidarViewModel.Descuento ?? 0,
                            fechaRegistro = DateTime.Now,
                            iva = gastoPvValidarViewModel.IVA ?? 0,
                            noFactura = gastoPvValidarViewModel.NoFactura,
                            noRemision = gastoPvValidarViewModel.NoRemision,
                            observaciones = gastoPvValidarViewModel.Observaciones,
                            sucursalId = gastoPvValidarViewModel.SucursalId,
                            proveedorId = gastoPvValidarViewModel.ProveedorId.Value,
                            total = totalCompra,
                            usuarioRegistroId = IdUsuarioActual,
                            estatusCompraId = (int)EstatusCompras.Cerrada,
                            fechaCompra = gastoPvValidarViewModel.FechaGasto
                        };


                        //crear detalles compra
                        controlInventarioHelper = new ControlInventario();

                        foreach (GastosPvDetalle costo in costos)
                        {
                            var inventarioInsumo = Contexto.Inventarios.FirstOrDefault(f => f.sucursalId == compra.sucursalId && f.insumoId == costo.insumoId.Value);

                            DetalleCompra detalleCompra = new DetalleCompra
                            {
                                cantidad = costo.cantidad.Value,
                                insumoId = costo.insumoId.Value,
                                importe = costo.monto,
                                precioUnitario = costo.monto / costo.cantidad,
                                unidadCompraId = inventarioInsumo.Insumos.UnidadMedida1.idUnidadMedida //Unidad de Medida compra                                                                       
                            };

                            compra.DetalleCompra.Add(detalleCompra);

                            //Modificar inventario, generar movimientos con los detalles de la compra

                            hasError = controlInventarioHelper.registroMovimientoInventario(Contexto, null, costo.cantidad.Value, costo.insumoId.Value,
                                compra.sucursalId, (inventarioInsumo != null ? inventarioInsumo.cantidad : 0) + costo.cantidad.Value, UsuarioActual, TiposMovimientosInventario.Compra, detalleCompra.idDetalleCompra,
                                "Fecha Compra:" + compra.fechaCompra.Value.ToShortDateString() +
                                " Costo: " + costo.monto +
                                (String.IsNullOrEmpty(compra.noRemision) ? "" : " Remision: " + compra.noRemision) +
                                (String.IsNullOrEmpty(compra.noFactura) ? "" : " Factura: " + compra.noFactura) +
                                (String.IsNullOrEmpty(compra.observaciones) ? "" : " Observaciones: " + compra.observaciones), false).hasError;

                            compra.estatusCompraId = (int)EstatusCompras.Cerrada;
                            Contexto.Entry(compra).State = EntityState.Modified;

                        }

                        Contexto.Compras.Add(compra);

                        if (hasError)
                        {
                            string msg = $"No fue posible generar compra ni movimientos de inventario para el GastoPV: {gastosPvId}.";

                            ShowAlertDanger(msg);
                            Logger.Error(msg);
                        }

                    }

                    //Modificar GastosPv como aplicado
                    gastosPv.aplicado = true;

                    Contexto.Entry(gastosPv).State = EntityState.Modified;

                    if (!hasError)
                    {
                        Contexto.SaveChanges();

                        return RedirectToAction("IndexValidar"); //, new { id = gastoPvValidarViewModel.VentaId });

                    }

                    //CatRubros rubro = Contexto.CatRubros.Find(gastoPvValidarViewModel.catRubroId);
                    //if (rubro != null)
                    //{
                    //    //Si ya se registro un gasto con este IdGastoPv, verificar si es costo o gasto
                    //    bool esCostoRegistrado = false;
                    //    Gastos gastoAplicado = null;
                    //    Compras compraAplicada = null;

                    //    if (rubro.esCosto)
                    //    {
                    //        //Registrar compra y afectar inventario. 

                    //    }
                    //    else
                    //    {
                    //        //if (gastoPvValidarViewModel.Aplicado)
                    //        //{
                    //        //    gastoAplicado = Contexto.Gastos.FirstOrDefault(g => g.GastoPvId == gastosPv.idGastoPv);

                    //        //    esCostoRegistrado = gastoAplicado.CatConceptosGastos.CatRubros.esCosto;
                    //        //}


                    //        //if (esCostoRegistrado)
                    //        //{
                    //        //    //Actualizar


                    //        //    sucursalId = gastoPvValidarViewModel.SucursalId,
                    //        //    catConceptoGastoId = gastoPvValidarViewModel.catConceptoGastoId,
                    //        //    GastoPvId = gastosPv.idGastoPv,
                    //        //    noRemision = !string.IsNullOrEmpty(gastoPvValidarViewModel.NoRemision) ? gastoPvValidarViewModel.NoRemision : null,
                    //        //    noFactura = !string.IsNullOrEmpty(gastoPvValidarViewModel.NoFactura) ? gastoPvValidarViewModel.NoFactura : null,
                    //        //    monto = gastoPvValidarViewModel.MontoValidado,
                    //        //    obervacion = gastoPvValidarViewModel.ObservacionesValidar,
                    //        //    fechaGasto = gastoPvValidarViewModel.FechaGasto,
                    //        //    usuarioRegistroId = IdUsuarioActual,
                    //        //    fechaRegistro = DateTime.Now

                    //        //}
                    //        //else
                    //        //{
                    //            //Registrar Gasto                       
                    //            Gastos gasto = new Gastos
                    //            {
                    //                sucursalId = gastoPvValidarViewModel.SucursalId,
                    //                catConceptoGastoId = gastoPvValidarViewModel.catConceptoGastoId,
                    //                GastoPvId = gastosPv.idGastoPv,
                    //                noRemision = !string.IsNullOrEmpty(gastoPvValidarViewModel.NoRemision) ? gastoPvValidarViewModel.NoRemision : null,
                    //                noFactura = !string.IsNullOrEmpty(gastoPvValidarViewModel.NoFactura) ? gastoPvValidarViewModel.NoFactura : null,
                    //                monto = gastoPvValidarViewModel.MontoValidado,
                    //                obervacion = gastoPvValidarViewModel.ObservacionesValidar,
                    //                fechaGasto = gastoPvValidarViewModel.FechaGasto,
                    //                usuarioRegistroId = IdUsuarioActual,
                    //                fechaRegistro = DateTime.Now
                    //            };

                    //            Contexto.Gastos.Add(gasto);
                    //            gastosPv.aplicado = true;
                    //            Contexto.Entry(gastosPv).State = EntityState.Modified;

                    //        //}

                    //        Contexto.SaveChanges();
                    //    }                      

                }

                ViewBag.Rubros = ObtenerRubros(gastoPvValidarViewModel.catRubroId, null, true);
                ViewBag.catRubros = ObtenerCatRubros();
                ViewBag.ConceptosGasto = ObtenerConceptosGasto(null, gastoPvValidarViewModel.catRubroId);

                return RedirectToAction("IndexValidarDetalle", new { id = gastoPvValidarViewModel.VentaId });

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGastoPVDetalle(GastoPvDetalleViewModel gastoPvDetalleViewModel)
        {
            try
            {
                ValidarGastoDetalle(gastoPvDetalleViewModel);

                if (ModelState.IsValid)
                {
                    CatRubros rubro = Contexto.CatRubros.Find(gastoPvDetalleViewModel.catRubroId);

                    if (rubro != null)
                    {
                        GastosPv gastosPv = Contexto.GastosPv.Find(gastoPvDetalleViewModel.GastoPvId);

                        if (gastoPvDetalleViewModel.IdGastoPvDetalle > 0)
                        {
                            GastosPvDetalle gastoPvDetalle = Contexto.GastosPvDetalle.Find(gastoPvDetalleViewModel.IdGastoPvDetalle);

                            gastoPvDetalle.sucursalId = gastoPvDetalleViewModel.SucursalId;
                            gastoPvDetalle.proveedorId = gastoPvDetalleViewModel.ProveedorId;
                            gastoPvDetalle.catConceptoGastoId = gastoPvDetalleViewModel.catConceptoGastoId;
                            gastoPvDetalle.insumoId = gastoPvDetalleViewModel.InsumoId;
                            gastoPvDetalle.cantidad = gastoPvDetalleViewModel.Cantidad;
                            gastoPvDetalle.monto = gastoPvDetalleViewModel.Monto;

                            Contexto.Entry(gastosPv).State = EntityState.Modified;
                        }
                        else
                        {
                            GastosPvDetalle gastoPvDetalle = new GastosPvDetalle
                            {
                                sucursalId = gastoPvDetalleViewModel.SucursalId,
                                proveedorId = gastoPvDetalleViewModel.ProveedorId,
                                catConceptoGastoId = gastoPvDetalleViewModel.catConceptoGastoId,
                                insumoId = gastoPvDetalleViewModel.InsumoId,
                                cantidad = gastoPvDetalleViewModel.Cantidad,
                                monto = gastoPvDetalleViewModel.Monto,
                                usuarioRegistroId = IdUsuarioActual,
                                fechaRegistro = DateTime.Now
                            };

                            gastosPv.GastosPvDetalle.Add(gastoPvDetalle);
                            Contexto.Entry(gastosPv).State = EntityState.Modified;
                        }

                        Contexto.SaveChanges();

                    }
                    else
                    {
                        ShowAlertWarning("No se encontró el rubro seleccionado.");
                    }
                }

                return RedirectToAction("IndexValidarDetalle", new { id = gastoPvDetalleViewModel.VentaId });
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("IndexValidarDetalle", new { id = gastoPvDetalleViewModel.VentaId });
            }

        }

        private void ValidarGastoDetalle(GastoPvDetalleViewModel gastoPvDetalleViewModel)
        {
            //TODO: Implementar
        }

        private void ValidarGasto(GastoPvValidarViewModel gasto)
        {
            if (gasto.IdGastoPv <= 0)
                ModelState.AddModelError("IdGastoPv", "Campo requerido.");

            if (gasto.MontoValidado <= 0)
                ModelState.AddModelError("Monto", "Debe ser mayor a 0.");

            if (gasto.FechaGasto.Date > DateTime.Now.Date || gasto.FechaGasto.Date < DateTime.Now.Date.AddDays(-30))
                ModelState.AddModelError("FechaGasto", "La fecha del gasto no debe ser mayor al día actual o menor a 30 días.");
        }

        public ActionResult ObtenerConceptoGastoPorRubro(int idRubro)
        {
            try
            {
                return Json(ObtenerConceptosGasto(null, idRubro));
            }
            catch (Exception ex)
            {
                return Json(new { hasException = true, message = ObtenerExInfo(ex) }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ObtenerInsumosPorRubro(int idRubro)
        {
            try
            {
                var comprasController = DependencyResolver.Current.GetService<ComprasController>();
                comprasController.ControllerContext = new ControllerContext(this.Request.RequestContext, comprasController);

                return Json(comprasController.ObtenerInsumosPorRubro(null, idRubro));
            }
            catch (Exception ex)
            {
                return Json(new { hasException = true, message = ObtenerExInfo(ex) }, JsonRequestBehavior.AllowGet);
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Contexto.Dispose();
            }
            base.Dispose(disposing);
        }

        #region HELPERS

        [ValidateInput(false)]
        public ActionResult ValidarFechaGasto(string fecha)
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

        public SelectListItem[] ObtenerRubros(int? selected = null, bool? esCostos = null, bool descripcionLarga = false)
        {
            return ComerciosFirmados
                    .Include(c => c.CatRubros)
                    .SelectMany(c => c.CatRubros)
                    .Where(c => c.activo &&
                        (esCostos == null || c.esCosto == esCostos))
                    .ToArray() //necesario para poder armar la descripcion concatenada
                    .Select(c => new SelectListItem
                    {
                        Text = (descripcionLarga) ? string.Format("{0} - {1}", c.descripcion, c.esCosto ? "COSTO" : "GASTO") : c.descripcion,
                        Value = c.idCatRubro.ToString(),
                        Selected = c.idCatRubro == selected,
                    }).ToArray();
        }

        public SelectListItem[] ObtenerConceptosGasto(int? selected = null, int? idRubro = null)
        {
            if (idRubro != null)
            {
                return ComerciosFirmados
               .Include(c => c.CatConceptosGastos)
               .SelectMany(c => c.CatConceptosGastos.Where(cg => cg.activo))
               .Where(c => c.activo && c.catRubroId == idRubro)
               .Select(c => new SelectListItem
               {
                   Text = c.descripcion,
                   Value = c.idCatConceptoGasto.ToString(),
                   Selected = c.idCatConceptoGasto == selected,
               }).ToArray();
            }
            else
            {
                return ComerciosFirmados
               .Include(c => c.CatConceptosGastos)
               .SelectMany(c => c.CatConceptosGastos.Where(cg => cg.activo))
               .Where(c => c.activo)
               .Select(c => new SelectListItem
               {
                   Text = c.descripcion,
                   Value = c.idCatConceptoGasto.ToString(),
                   Selected = c.idCatConceptoGasto == selected,
               }).ToArray();
            }

        }

        public List<CatRubros> ObtenerCatRubros()
        {
            return ComerciosFirmados
               .Include(c => c.CatRubros)
               .SelectMany(c => c.CatRubros)
               .Where(c => c.activo)
               .ToList();
        }

        #endregion
    }
}
