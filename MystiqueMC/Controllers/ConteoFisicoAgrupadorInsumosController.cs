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
using MystiqueMC.Models.Json;

namespace MystiqueMC.Controllers
{

    public class ConteoFisicoAgrupadorInsumosController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index(string SearchOrden = "true")
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var conteofisicoagrupadorinsumos = Contexto.ConteoFisicoAgrupadorInsumos
                    .Include(c => c.comercios)
                    .Include(c => c.ConteoFisicoPeriodoCaptura)
                    .Include(c => c.UnidadMedida)
                    .Include(c => c.Insumos)
                    .Where(c => c.comercioId == comercioId);

                if (!string.IsNullOrEmpty(SearchOrden))
                {
                    ViewBag.SearchOrden = SearchOrden;
                    conteofisicoagrupadorinsumos = conteofisicoagrupadorinsumos.Where(c => c.activo.ToString() == SearchOrden);
                }

                ViewBag.sucursaId = Session.ObtenerInventarioSucursal();

                return View(conteofisicoagrupadorinsumos.ToList());
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Details(int? id, string SearchOrden = "true")
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ConteoFisicoAgrupadorInsumos conteoFisicoAgrupadorInsumos = Contexto.ConteoFisicoAgrupadorInsumos.Find(id);
                var items = Contexto.ConteoFisicoInsumos.Where(w => w.conteoFisicoAgrupadorInsumosId == id);

                if (conteoFisicoAgrupadorInsumos == null)
                {
                    return HttpNotFound();
                }

                if (!string.IsNullOrEmpty(SearchOrden))
                {
                    ViewBag.SearchOrden = SearchOrden;
                    items = items.Where(c => c.activo.ToString() == SearchOrden);
                }

                conteoFisicoAgrupadorInsumos.fechaInicio.ToShortDateString();
                ViewBag.NombreAgrupador = conteoFisicoAgrupadorInsumos.descripcion;
                ViewBag.IdAgrupador = conteoFisicoAgrupadorInsumos.idConteoFisicoAgrupadorInsumos;
                return View(items.ToList());
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        public ActionResult Create()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var categoriaInsumo = Contexto.CategoriaInsumo.Where(c => c.comercioId == comercioId).ToList();
                ViewBag.IdComercio = comercioId;
                var insumos = Contexto.Insumos.Where(c => c.comercioId == comercioId && c.conteoFisico == true).ToList();
                ViewBag.unidadMedida = insumos.Any(c => c.unidadMedidaIdMinima != null);
                ViewBag.categoriaInsumoId = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion");
                ViewBag.unidadMedidaIdConteo = new SelectList(Contexto.UnidadMedida.Where(w => w.comercioId == comercioId), "idUnidadMedida", "descripcion");
                ViewBag.unidadCompraIdConteo = new SelectList(Contexto.UnidadMedida.Where(w => w.comercioId == comercioId), "idUnidadMedida", "descripcion");
                return View();
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int conteoFisicoPeriodoCapturaId,int diaConteo, string descripcion, int unidadMedidaIdConteo, int comercioId, int unidadCompraIdConteo, decimal equivalencia, int categoriaInsumoId)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var insumo = Contexto.Insumos.Add( new Insumos
                    {
                        nombre = descripcion,
                        comercioId = comercioId,
                        unidadMedidaIdCompra = unidadCompraIdConteo,
                        unidadMedidaIdMinima = unidadMedidaIdConteo,
                        esIngrediente = false,
                        esProcesado = false,
                        conteoFisico = false,
                        usuarioRegistroId = IdUsuarioActual,
                        esAgrupador = true,
                        equivalencia = equivalencia,
                        categoriaInsumoId = categoriaInsumoId,
                        fechaRegistro = DateTime.Now,
                        activo = true
                    });


                    var conteoFisicoAgrupadorInsumos = Contexto.ConteoFisicoAgrupadorInsumos.Add(new ConteoFisicoAgrupadorInsumos
                    {
                        comercioId = comercioId,
                        descripcion = descripcion,
                        conteoFisicoPeriodoCapturaId = conteoFisicoPeriodoCapturaId,
                        fechaInicio = DateTime.Now,                      
                        unidadMedidaIdConteo = unidadCompraIdConteo,
                        diaConteo = diaConteo,
                        insumoId = insumo.idInsumo,
                        activo = true
                    });

                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View();
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }

        }

        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ConteoFisicoAgrupadorInsumos conteoFisicoAgrupadorInsumos = Contexto.ConteoFisicoAgrupadorInsumos.Find(id);
                var categoriaInsumo = Contexto.CategoriaInsumo.Where(c => c.comercioId == conteoFisicoAgrupadorInsumos.comercioId).ToList();
                if (conteoFisicoAgrupadorInsumos == null)
                {
                    return HttpNotFound();
                }

                ViewBag.conteoFisicoPeriodoCapturaId = new SelectList(Contexto.ConteoFisicoPeriodoCaptura, "idConteoFisicoPeriodoCaptura", "descripcion", conteoFisicoAgrupadorInsumos.conteoFisicoPeriodoCapturaId);
                ViewBag.unidadMedidaIdConteo = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion", conteoFisicoAgrupadorInsumos.Insumos.unidadMedidaIdMinima);
                ViewBag.unidadCompraIdConteo = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion", conteoFisicoAgrupadorInsumos.Insumos.unidadMedidaIdCompra);
                ViewBag.categoriaInsumoId = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion", conteoFisicoAgrupadorInsumos.Insumos.categoriaInsumoId);
                return View(conteoFisicoAgrupadorInsumos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idConteoFisicoAgrupadorInsumos,comercioId,descripcion,conteoFisicoPeriodoCapturaId,unidadMedidaIdConteo,diaConteo,insumoId")] ConteoFisicoAgrupadorInsumos conteoFisicoAgrupadorInsumos, 
            int unidadCompraIdConteo, decimal equivalencia, int categoriaInsumoId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var insumos = Contexto.Insumos.Find(conteoFisicoAgrupadorInsumos.insumoId);
                    insumos.nombre = conteoFisicoAgrupadorInsumos.descripcion;
                    insumos.unidadMedidaIdMinima = conteoFisicoAgrupadorInsumos.unidadMedidaIdConteo;
                    insumos.unidadMedidaIdCompra = unidadCompraIdConteo;
                    insumos.equivalencia = equivalencia;
                    insumos.categoriaInsumoId = categoriaInsumoId;
                    Contexto.Entry(insumos).State = EntityState.Modified;

                    conteoFisicoAgrupadorInsumos.fechaInicio = DateTime.Now;
                    conteoFisicoAgrupadorInsumos.activo = true;
                    Contexto.Entry(conteoFisicoAgrupadorInsumos).State = EntityState.Modified;
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", conteoFisicoAgrupadorInsumos.comercioId);
                ViewBag.conteoFisicoPeriodoCapturaId = new SelectList(Contexto.ConteoFisicoPeriodoCaptura, "idConteoFisicoPeriodoCaptura", "descripcion", conteoFisicoAgrupadorInsumos.conteoFisicoPeriodoCapturaId);
                ViewBag.unidadMedidaIdConteo = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion", conteoFisicoAgrupadorInsumos.unidadMedidaIdConteo);
                ViewBag.insumoId = new SelectList(Contexto.Insumos, "idInsumo", "nombre", conteoFisicoAgrupadorInsumos.insumoId);
                return View(conteoFisicoAgrupadorInsumos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        [HttpPost]
        public ActionResult EliminarAgrupador(int idConteoFisicoAgrupadorInsumos)
        {
            try
            {
                var conteoFisicoAgrupadorInsumos = Contexto.ConteoFisicoAgrupadorInsumos.Find(idConteoFisicoAgrupadorInsumos);

                if (conteoFisicoAgrupadorInsumos != null)
                {
                    if (!conteoFisicoAgrupadorInsumos.ConteoFisicoInsumos.Any()
                        && conteoFisicoAgrupadorInsumos.Insumos != null
                        && conteoFisicoAgrupadorInsumos.ConteoFisicoPeriodoCaptura != null
                        && !conteoFisicoAgrupadorInsumos.RegistroConteoFisicoInsumos.Any())
                    {
                        var insumo = Contexto.Insumos.Find(conteoFisicoAgrupadorInsumos.insumoId);
                        if (insumo != null)
                        {
                            if (!insumo.DetalleCompra.Any()
                                && !insumo.DetalleRecetaProcesados.Any()
                                && !insumo.DetalleRecetaProducto.Any()
                                && !insumo.InsumoProductos.Any()
                                && !insumo.Inventarios.Any()
                                && !insumo.MovimientoInventarios.Any()
                                && !insumo.RecetasProcesados.Any())
                            {
                                Contexto.Insumos.Remove(insumo);
                                Contexto.ConteoFisicoAgrupadorInsumos.Remove(conteoFisicoAgrupadorInsumos);
                            }
                        }
                        else
                        {
                            Contexto.ConteoFisicoAgrupadorInsumos.Remove(conteoFisicoAgrupadorInsumos);
                        }
                        Contexto.SaveChanges();
                    }
                    return new HttpStatusCodeResult(200,"OK");
                }
                else
                {
                    return new HttpStatusCodeResult(500);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        public ActionResult obtenerUnidadMedida(int insumo)
        {
            try
            {
                var listadoInsumos = Contexto.Insumos
                    .Where(c => c.idInsumo == insumo)
                    .Select(c => new ItemListadoInsumos
                    {
                        Idinsumo = c.idInsumo,
                        IdUnidadMedida = c.unidadMedidaIdMinima,
                    }).ToList();

                return Json(new ListadoInsumos { exito = true, resultado = listadoInsumos });
            }
            catch (Exception e)
            {
                return Json(new ListadoInsumos { exito = false });
            }
        }


        public ActionResult ActivarAgrupador(int id, int idAgrupador)
        {
            try
            {
                var agrupador = Contexto.ConteoFisicoInsumos.Find(id);
                agrupador.activo = true;
                Contexto.Entry(agrupador).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Details", "ConteoFisicoAgrupadorInsumos", new { id = idAgrupador });
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }
        public ActionResult InactivarAgrupador(int id, int idAgrupador)
        {
            try
            {
                var agrupador = Contexto.ConteoFisicoInsumos.Find(id);
                agrupador.activo = false;
                Contexto.Entry(agrupador).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Details", "ConteoFisicoAgrupadorInsumos", new { id = idAgrupador });
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        public ActionResult InactivarAgrupadorInsumo(int id)
        {
            try
            {
                var agrupador = Contexto.ConteoFisicoAgrupadorInsumos.Find(id);
                agrupador.activo = false;
                Contexto.Entry(agrupador).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        public ActionResult ActivarAgrupadorInsumo(int id)
        {
            try
            {
                var agrupador = Contexto.ConteoFisicoAgrupadorInsumos.Find(id);
                agrupador.activo = true;
                Contexto.Entry(agrupador).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
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
    }
}
