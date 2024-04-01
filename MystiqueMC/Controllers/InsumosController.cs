using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    public class InsumosController : BaseController
    {
        #region GET
        // GET: Insumos
        public ActionResult Index(string CategoriaInsumo, string Nombre, string Estatus)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var categoriaInsumo = Contexto.CategoriaInsumo.Where(c => c.comercioId == comercioId);
                ViewBag.CategoriaInsumo = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion");
                ViewBag.categoria = CategoriaInsumo;
                ViewBag.Nombre = Nombre;
                ViewBag.Estatus = Estatus;
                if (CategoriaInsumo != null || Nombre != null || Estatus != null)
                {
                    var insumos = Contexto.Insumos
                        .Include(i => i.CategoriaIngrediente)
                        .Include(i => i.CategoriaInsumo)
                        .Include(i => i.UnidadMedida)
                        .Include(i => i.UnidadMedida1)
                        .Include(i => i.DetalleRecetaProducto)
                        .Include(i => i.CatRubros)
                        .Where(c => c.comercioId == comercioId && c.categoriaInsumoId != 1006 && c.esProcesado == false);

                    if (!string.IsNullOrEmpty(CategoriaInsumo))
                    {
                        insumos = insumos.Where(w => w.categoriaInsumoId.ToString().Contains(CategoriaInsumo));
                    }
                    if (!string.IsNullOrEmpty(Nombre))
                    {
                        insumos = insumos.Where(w => w.nombre.ToUpper().Contains(Nombre.ToUpper()));
                    }
                    if (!string.IsNullOrEmpty(Estatus) && Estatus != "2")
                    {
                        bool estatus = Estatus == "1" ? true : false;
                        insumos = insumos.Where(w => w.activo == estatus);
                    }

                    return View(insumos.ToList());
                }

                var insumos1 = Contexto.Insumos.Where(c => c.comercioId == 0);

                ViewBag.sucursaId = Session.ObtenerInventarioSucursal();

                return View(insumos1.ToList());
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Details(int? id, string nombreInsumo, int categoria, string nombre)
        {
            try
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
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Create()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                ViewBag.comercioId = comercioId;
                var categoriaInsumo = Contexto.CategoriaInsumo.Where(c => c.comercioId == comercioId).ToList();
                var categoriaIngrediente = Contexto.CategoriaIngrediente.Where(c => c.comercioId == comercioId).ToList();
                var categoriaUnidadMedida = Contexto.UnidadMedida.Where(c => c.comercioId == comercioId).ToList();

                ViewBag.categoriaInsumoId = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion");
                ViewBag.CategoriaIngredienteId = new SelectList(categoriaIngrediente, "idCategoriaIngrediente", "descripcion");
                ViewBag.UnidadMedidaIdMinima = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion");
                ViewBag.UnidadMedidaIdCompra = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion");
                ViewBag.catRubroId = ObtenerRubros();
                return View();
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Edit(int? id, string CategoriaInsumo, string Nombreb, string Estatus)
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

                ViewBag.categoriaInsumoId = new SelectList(categoriaInsumo, "idCategoriaInsumo", "descripcion", insumos.categoriaInsumoId);
                ViewBag.CategoriaIngredienteId = new SelectList(categoriaIngrediente, "idCategoriaIngrediente", "descripcion", insumos.categoriaIngredienteId);
                ViewBag.UnidadMedidaIdMinima = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion", insumos.unidadMedidaIdMinima);
                ViewBag.UnidadMedidaIdCompra = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion", insumos.unidadMedidaIdCompra);
                ViewBag.catRubroId = ObtenerRubros(insumos.catRubroId);

                ViewBag.CategoriaInsumoSearch = CategoriaInsumo;
                ViewBag.NombreSearch = Nombreb;
                ViewBag.Estatus = Estatus;

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

        public ActionResult Menu()
        {
            return View();
        }
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idInsumo,comercioId,nombre,categoriaInsumoId,esProcesado,activo,CategoriaIngredienteId,UnidadMedidaIdMinima,UnidadMedidaIdCompra,mermaPermitida,aplicaInventario,catRubroId,equivalencia")] Insumos insumos, string esIngrediente, string Imagen, string conteoFisico)
        {
            try
            {
                /*
                insumos.aplicaInventario = true;                
                */
                if (insumos.catRubroId != null)
                {
                    insumos.aplicaInventario = true;
                    insumos.conteoFisico = true;
                }
                else
                {
                    insumos.aplicaInventario = false;
                    insumos.conteoFisico = false;
                }

                if (ModelState.IsValid)
                {
                    insumos.fechaRegistro = DateTime.Now;
                    insumos.usuarioRegistroId = IdUsuarioActual;
                    insumos.activo = true;
                    insumos.imagen = Imagen;
                    insumos.esProcesado = false;


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
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                ViewBag.comercioId = comercioId;
                var catRubroId = Contexto.CatRubros.Where(c => c.comercioId == comercioId).ToList();

                ViewBag.categoriaInsumoId = new SelectList(Contexto.CategoriaInsumo, "idCategoriaInsumo", "descripcion", insumos.categoriaInsumoId);
                ViewBag.CategoriaIngredienteId = new SelectList(Contexto.CategoriaIngrediente, "idCategoriaIngrediente", "descripcion");
                ViewBag.UnidadMedidaIdMinima = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion");
                ViewBag.UnidadMedidaIdCompra = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion");
                ViewBag.catRubroId = new SelectList(catRubroId, "idCatRubro", "descripcion", insumos.catRubroId);

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
        public ActionResult Edit([Bind] Insumos insumos,string equivalencia, string esIngrediente, string Imagen, 
            string conteoFisico, string CategoriaInsumoSearch, string NombreSearch, string Estatus)
        {
            try
            {
                if (insumos.catRubroId != null)
                {
                    insumos.aplicaInventario = true;
                    insumos.conteoFisico = true;
                }
                else
                {
                    insumos.aplicaInventario = false;
                    insumos.conteoFisico = false;
                }

                if (ModelState.IsValid)
                {
                    if (esIngrediente == "true") { insumos.esIngrediente = true; }
                    if (insumos.aplicaInventario == null) { insumos.aplicaInventario = false; };

                    insumos.fechaRegistro = DateTime.Now;
                    insumos.usuarioRegistroId = IdUsuarioActual;
                    insumos.imagen = Imagen;
                    insumos.esProcesado = false;
                    Contexto.Entry(insumos).State = EntityState.Modified;

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
                    return RedirectToAction("Index", new { CategoriaInsumo = CategoriaInsumoSearch, Nombre = NombreSearch, Estatus });
                }
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                ViewBag.comercioId = comercioId;
                var catRubroId = Contexto.CatRubros.Where(c => c.comercioId == comercioId).ToList();

                ViewBag.categoriaInsumoId = new SelectList(Contexto.CategoriaInsumo, "idCategoriaInsumo", "descripcion", insumos.categoriaInsumoId);
                ViewBag.CategoriaIngredienteId = new SelectList(Contexto.CategoriaIngrediente, "idCategoriaIngrediente", "descripcion", insumos.categoriaIngredienteId);
                ViewBag.UnidadMedidaIdMinima = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion", insumos.unidadMedidaIdMinima);
                ViewBag.UnidadMedidaIdCompra = new SelectList(Contexto.UnidadMedida, "idUnidadMedida", "descripcion", insumos.unidadMedidaIdCompra);
                ViewBag.catRubroId = new SelectList(catRubroId, "idCatRubro", "descripcion", insumos.catRubroId);
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
        [HttpPost]
        public ActionResult EliminarInsumo(int idInsumo)
        {
            try
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    try
                    {
                        var insumo = Contexto.Insumos.Find(idInsumo);
                        Contexto.Insumos.Remove(insumo);
                        var inventario = Contexto.Inventarios
                            .FirstOrDefault(f => f.insumoId == idInsumo);

                        if (inventario != null)
                        {
                            Contexto.Inventarios.Remove(inventario);
                        }
                        Contexto.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        Console.WriteLine(ex);
                    }
                }

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

        public ActionResult PuedeEliminarse(int idInsumo)
        {
            var CanContinue = true;

            if (Contexto.ConteoFisicoInsumos.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }

            if (Contexto.DetalleRecetaProducto.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }
            if (Contexto.ConteoFisicoAgrupadorInsumos.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }
            if (Contexto.DetalleCompra.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }
            if (Contexto.MovimientoInventarios.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }
            if (Contexto.DetalleRecetaProcesados.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }
            if (Contexto.InsumoProductos.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }
            //if (Contexto.Inventarios.Any(a => a.insumoId == idInsumo))
            //{
            //    CanContinue = false;
            //}
            if (Contexto.Equivalencias.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }
            if (Contexto.RecetasProcesados.Any(a => a.insumoId == idInsumo))
            {
                CanContinue = false;
            }


            return new { success = CanContinue }.ToJsonResult();
        }

        private SelectListItem[] ObtenerRubros(int? selected = null)
        {
            var gastosController = DependencyResolver.Current.GetService<GastosController>();
            gastosController.ControllerContext = new ControllerContext(this.Request.RequestContext, gastosController);

            return gastosController.ObtenerRubros(selected, esCostos: true);
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