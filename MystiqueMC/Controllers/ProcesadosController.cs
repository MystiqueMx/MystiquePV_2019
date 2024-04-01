using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;
using MystiqueMC.Models.Graficas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    public class ProcesadosController : BaseController
    {
        #region GET
        // GET: Insumos
        public ActionResult Index(string CategoriaInsumo, string Nombre, string Estatus)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
            var categoriaInsumo = Contexto.CategoriaInsumo.Where(c => c.comercioId == comercioId);
            ViewBag.CategoriaInsumo = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion");
            ViewBag.categoria = CategoriaInsumo;
            ViewBag.Nombre = Nombre;
            if (CategoriaInsumo != null || Nombre != null || Estatus != null)
            {
                var insumos = Contexto.Insumos
                    .Include(i => i.CategoriaIngrediente)
                    .Include(i => i.CategoriaInsumo)
                    .Include(i => i.UnidadMedida)
                    .Include(i => i.UnidadMedida1)
                    .Include(i => i.DetalleRecetaProducto)
                    .Where(c => c.comercioId == comercioId && c.categoriaInsumoId.ToString().Contains("1006"));

                if (!string.IsNullOrEmpty(CategoriaInsumo))
                {
                    insumos = insumos.Where(w => w.categoriaInsumoId.ToString().Contains("1006"));
                }
                if (!string.IsNullOrEmpty(Nombre))
                {
                    insumos = insumos.Where(w => w.nombre.Contains(Nombre));
                }
                if (!string.IsNullOrEmpty(Estatus) && Estatus != "2")
                {
                    bool estatus = Estatus == "1" ? true : false;
                    insumos = insumos.Where(w => w.activo == estatus);
                }

                return View(insumos.ToList());
            }

            var insumos1 = Contexto.Insumos.Where(c => c.comercioId == 0);

            return View(insumos1.ToList());
        }

        public ActionResult Details(int? id, string nombreInsumo, int categoria, string nombre)
        {
            var equivalencia = Contexto.Equivalencias
                .Include(e => e.Insumos)
                .Include(e => e.UnidadMedida)
                .Where(c => c.insumoId == id).ToList();

            ViewBag.insumo = id;
            ViewBag.nombre = nombreInsumo;
            ViewBag.categoria = categoria;
            ViewBag.nombreSearch = nombre;
            ViewBag.unidadMedidaId = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion");
            return View(equivalencia);
        }

        public ActionResult Create()
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
            ViewBag.comercioId = comercioId;
            var categoriaInsumo = Contexto.CategoriaInsumo.Where(c => c.comercioId == comercioId).ToList();
            var categoriaIngrediente = Contexto.CategoriaIngrediente.Where(c => c.comercioId == comercioId).ToList();
            var categoriaUnidadMedida = Contexto.UnidadMedida.Where(c => c.comercioId == comercioId).ToList();
            var catRubro = Contexto.CatRubros.Where(c => c.comercioId == comercioId).ToList();

            ViewBag.categoriaInsumoId = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion");
            ViewBag.CategoriaIngredienteId = new SelectList(categoriaIngrediente, "idCategoriaIngrediente", "descripcion");
            ViewBag.UnidadMedidaIdMinima = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion");
            ViewBag.UnidadMedidaIdCompra = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion");
            ViewBag.catRubroId = new SelectList(catRubro, "idCatRubro", "descripcion");
            return View();
        }

        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Insumos insumos = Contexto.Insumos.Find(id);

                if (insumos.esIngrediente == true) { insumos.esIngrediente = true; }
                else { insumos.esIngrediente = false; }

                if (insumos == null)
                {
                    return HttpNotFound();
                }
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                ViewBag.comercioId = comercioId;
                var categoriaInsumo = Contexto.CategoriaInsumo.Where(c => c.comercioId == comercioId).ToList();
                var categoriaIngrediente = Contexto.CategoriaIngrediente.Where(c => c.comercioId == comercioId).ToList();
                var categoriaUnidadMedida = Contexto.UnidadMedida.Where(c => c.comercioId == comercioId).ToList();
                var catRubroId = Contexto.CatRubros.Where(c => c.comercioId == comercioId).ToList();

                ViewBag.categoriaInsumoId = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion", insumos.categoriaInsumoId);
                ViewBag.CategoriaIngredienteId = new SelectList(categoriaIngrediente, "idCategoriaIngrediente", "descripcion", insumos.categoriaIngredienteId);
                ViewBag.UnidadMedidaIdMinima = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion", insumos.UnidadMedida);
                ViewBag.UnidadMedidaIdCompra = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion", insumos.unidadMedidaIdCompra);
                ViewBag.catRubroId = new SelectList(catRubroId, "idCatRubro", "descripcion", insumos.catRubroId);
                return View(insumos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insumos insumos = Contexto.Insumos.Find(id);
            if (insumos == null)
            {
                return HttpNotFound();
            }
            return View(insumos);
        }

        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idInsumo,comercioId,nombre,categoriaInsumoId,esProcesado,activo,CategoriaIngredienteId,UnidadMedidaIdMinima,UnidadMedidaIdCompra,mermaPermitida,aplicaInventario,catRubroId,equivalencia")] Insumos insumos, string esIngrediente, string Imagen, string conteoFisico)
        {
            try
            {
                if (conteoFisico == "on") { insumos.conteoFisico = true; } else { insumos.conteoFisico = false; }
                if (ModelState.IsValid)
                {
                    if (insumos.aplicaInventario == null) { insumos.aplicaInventario = false; };
                    insumos.fechaRegistro = DateTime.Now;
                    insumos.usuarioRegistroId = IdUsuarioActual;
                    insumos.activo = true;
                    insumos.imagen = Imagen;
                    insumos.categoriaInsumoId = 1006;
                    insumos.esProcesado = true;
                    if (esIngrediente == "on") { insumos.esIngrediente = true; }
                    Contexto.Insumos.Add(insumos);

                    var sucursal = Contexto.sucursales.Where(c => c.comercioId == insumos.comercioId).Select(c => c.idSucursal).First();
                    var inventario = Contexto.Inventarios.Add(new Inventarios
                    {
                        insumoId = insumos.idInsumo,
                        fechaRegistro = DateTime.Now,
                        activo = true,
                        precio = 0,
                        maximo = 0,
                        minimo = 0,
                        sucursalId = sucursal
                    });
                    if (insumos.aplicaInventario == true) { inventario.activo = true; } else { inventario.activo = false; }
                    Contexto.Inventarios.Add(inventario);

                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.categoriaInsumoId = new SelectList(Contexto.CategoriaInsumo, "idCategoriaInsumo", "descripcion", insumos.categoriaInsumoId);
                ViewBag.CategoriaIngredienteId = new SelectList(Contexto.CategoriaIngrediente, "idCategoriaIngrediente", "descripcion");
                ViewBag.UnidadMedidaIdMinima = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion");
                ViewBag.UnidadMedidaIdCompra = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion");

            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return View(insumos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idInsumo,comercioId,nombre,categoriaInsumoId,esProcesado,activo,CategoriaIngredienteId,UnidadMedidaIdMinima,UnidadMedidaIdCompra,mermaPermitida,aplicaInventario,catRubroId,equivalencia")] Insumos insumos, string esIngrediente, string Imagen, string conteoFisico)
        {
            try
            {
                if (conteoFisico == "on") { insumos.conteoFisico = true; } else { insumos.conteoFisico = false; }
                if (ModelState.IsValid)
                {
                    if (esIngrediente == "true") { insumos.esIngrediente = true; }
                    if (insumos.aplicaInventario == null) { insumos.aplicaInventario = false; };

                    insumos.fechaRegistro = DateTime.Now;
                    insumos.usuarioRegistroId = IdUsuarioActual;
                    insumos.imagen = Imagen;
                    Contexto.Entry(insumos).State = EntityState.Modified;
                    insumos.categoriaInsumoId = 1006;
                    insumos.esProcesado = true;

                    var inventarioRegistrado = Contexto.Inventarios.Where(c => c.insumoId == insumos.idInsumo).FirstOrDefault();
                    if (inventarioRegistrado != null)
                    {
                        if (insumos.aplicaInventario == true)
                        {
                            inventarioRegistrado.activo = true;
                        }
                        else
                        {
                            inventarioRegistrado.activo = false;
                        }
                    }
                    else
                    {
                        var sucursal = Contexto.sucursales.Where(c => c.comercioId == insumos.comercioId).Select(c => c.idSucursal).First();
                        var inventario = Contexto.Inventarios.Add(new Inventarios
                        {
                            insumoId = insumos.idInsumo,
                            fechaRegistro = DateTime.Now,
                            activo = true,
                            precio = 0,
                            maximo = 0,
                            minimo = 0,
                            sucursalId = sucursal
                        });
                        if (insumos.aplicaInventario == true) { inventario.activo = true; } else { inventario.activo = false; }
                        Contexto.Inventarios.Add(inventario);
                    }
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.categoriaInsumoId = new SelectList(Contexto.CategoriaInsumo, "idCategoriaInsumo", "descripcion", insumos.categoriaInsumoId);
                ViewBag.CategoriaIngredienteId = new SelectList(Contexto.CategoriaIngrediente, "idCategoriaIngrediente", "descripcion", insumos.categoriaIngredienteId);
                ViewBag.UnidadMedidaIdMinima = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion", insumos.unidadMedidaIdMinima);
                ViewBag.UnidadMedidaIdCompra = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion", insumos.unidadMedidaIdCompra);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return View(insumos);
        }

        [HttpPost]
        public ActionResult ActivarInsumo(int idInsumo)
        {
            try
            {
                var insumo = Contexto.Insumos.Find(idInsumo);
                insumo.activo = true;
                Contexto.Entry(insumo).State = EntityState.Modified;
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult InactivarInsumo(int idInsumo)
        {
            try
            {
                var insumo = Contexto.Insumos.Find(idInsumo);
                insumo.activo = false;
                Contexto.Entry(insumo).State = EntityState.Modified;
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
        }

        #region EQUIVALENCIA
        public ActionResult AgregarEquivalencia(int unidadMedidaId, int idInsumo, decimal cantidad, string nombre, string categoria, string nombreSearch)
        {
            try
            {
                var equivalencia = Contexto.Equivalencias.Add(new Equivalencias
                {
                    insumoId = idInsumo,
                    unidadMedidaId = unidadMedidaId,
                    cantidad = cantidad
                });
                Contexto.SaveChanges();
                return RedirectToAction("Details", "Insumos", new { id = idInsumo, nombreInsumo = nombre, categoria = categoria, nombre = nombreSearch });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }

            return RedirectToAction("Details", "Insumos", new { id = idInsumo });
        }
        public ActionResult EditarEquivalencia(int unidadMedidaId, int idInsumo, decimal cantidad, int idEquivalencia, string nombre, string categoria, string nombreSearch)
        {
            try
            {
                var equivalente = Contexto.Equivalencias.Find(idEquivalencia);
                equivalente.unidadMedidaId = unidadMedidaId;
                equivalente.cantidad = cantidad;
                Contexto.Entry(equivalente).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Details", "Insumos", new { id = idInsumo, nombreInsumo = nombre, categoria = categoria, nombre = nombreSearch });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }

            return RedirectToAction("Details", "Insumos", new { id = idInsumo });
        }
        #endregion

        #endregion

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