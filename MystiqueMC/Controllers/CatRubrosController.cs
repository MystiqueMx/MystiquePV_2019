using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;
using MystiqueMC.Models.Graficas;
using MystiqueMC.Models.Rubros;
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
    [Authorize]
    public class CatRubrosController : BaseController
    {
        #region GET
        // GET: CatRubros
        public ActionResult Index(string SearchOrden = "true")
        {
            try
            {                               
                //var usuarioFirmado = Session.ObtenerUsuario();
                //int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                //bool filtroActivo = SearchOrden == "true";
                //var egresosCostos = Contexto.CatRubros.Include(c => c.comercios)
                //    .Where(w => w.comercioId == comercioId && w.esCosto && w.activo == filtroActivo)
                //    .ToList();

                //var egresosGastos = Contexto.CatRubros.Include(c => c.comercios)
                //    .Where(w => w.comercioId == comercioId && !w.esCosto)
                //    .ToList();

                //if (!string.IsNullOrEmpty(SearchOrden))
                //{
                //    ViewBag.SearchOrden = SearchOrden;
                //}

                //var rubrosVW = new RubrosIndexViewModel
                //{
                //    RubrosVM = egresosCostos,
                //    TotalPonderacion = egresosCostos.Sum(x => x.ponderacion),
                //};
                
                return View();
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        public ActionResult Egresos(bool esCostos, bool filtroActivo)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var rubros = Contexto.CatRubros.Include(c => c.comercios)
                    .Where(w => w.comercioId == comercioId && w.esCosto == esCostos && w.activo == filtroActivo)
                    .Select(r => new RubrosViewModel
                    {
                        IdCatRubro = r.idCatRubro,
                        ComercioId = r.comercioId,
                        Descripcion = r.descripcion,
                        Ponderacion = r.ponderacion,
                        EsCosto = r.esCosto,
                        Activo = r.activo
                    }).ToList();

                var rubrosVW = new RubrosIndexViewModel
                {
                    RubrosVM = rubros,
                    TotalPonderacion = rubros.Sum(x => x.Ponderacion),
                };

                ViewBag.esCostos = esCostos;
                return PartialView("_Egresos", rubrosVW);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        public ActionResult Conceptos(int catRubroId)
        {
            try
            {
                var ConceptosGastosVM = Contexto.CatConceptosGastos.Where(c => c.catRubroId == catRubroId)
                    .Select(c => new ConceptosGastosViewModel
                    {
                        IdCatConceptoGasto = c.idCatConceptoGasto,
                        ComercioId = c.comercioId,
                        CatRubroId = c.catRubroId,
                        Descripcion = c.descripcion,
                        Activo = c.activo,
                        Ponderacion = c.ponderacion
                    }).ToList();

                return PartialView("_Conceptos", ConceptosGastosVM);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: CatRubros/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial");
                return View();
            }
            catch (Exception ex)
            {

                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: CatRubros/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CatRubros catRubros = Contexto.CatRubros.Find(id);
                if (catRubros == null)
                {
                    return HttpNotFound();
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catRubros.comercioId);
                return View(catRubros);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: CatRubros/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CatRubros catRubros = Contexto.CatRubros.Find(id);
                if (catRubros == null)
                {
                    return HttpNotFound();
                }
                return View(catRubros);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        public ActionResult ModalCreateConcepto()
        {
            try
            {
                ViewBag.Rubros = ObtenerRubros();
                return PartialView("_ModalCreateConcepto");
            }
            catch (Exception ex)
            {

                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        public ActionResult ModalEditConcepto(int id)
        {
            try
            {
                CatConceptosGastos catConceptosGastos = Contexto.CatConceptosGastos.Find(id);

                if (catConceptosGastos == null)
                {
                    ShowAlertDanger("No se encontró el concepto seleccionado.");
                    //RedirectToAction("Index");
                }

                ViewBag.Rubros = ObtenerRubros(catConceptosGastos.catRubroId);

                return PartialView("_ModalEditConcepto", catConceptosGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModalCreateConcepto([Bind(Include = "idCatConceptoGasto,catRubroId,descripcion,activo,ponderacion")] CatConceptosGastos catConceptosGastos)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                catConceptosGastos.comercioId = comercioId;

                Validar(catConceptosGastos);

                if (ModelState.IsValid)
                {
                    catConceptosGastos.descripcion = catConceptosGastos.descripcion.ToUpper();

                    Contexto.CatConceptosGastos.Add(catConceptosGastos);

                    Contexto.SaveChanges();

                    return Json(new { success = true, message = "Concepto agregado exitosamente." }, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Index");
                }

                ViewBag.Rubros = new SelectList(Contexto.CatRubros, "idCatRubro", "descripcion", catConceptosGastos.catRubroId);


                return PartialView("_ModalCreateConcepto", catConceptosGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModalEditConcepto([Bind(Include = "idCatConceptoGasto,comercioId,catRubroId,descripcion,activo,ponderacion")] CatConceptosGastos catConceptosGastos)
        {
            try
            {
                Validar(catConceptosGastos);

                if (ModelState.IsValid)
                {
                    catConceptosGastos.descripcion = catConceptosGastos.descripcion.ToUpper();
                    Contexto.Entry(catConceptosGastos).State = EntityState.Modified;

                    Contexto.SaveChanges();

                    return Json(new { success = true, message = "Concepto modificado exitosamente." }, JsonRequestBehavior.AllowGet);
                }

                ViewBag.Rubros = ObtenerRubros(catConceptosGastos.catRubroId);

                return PartialView("_ModalEditConcepto", catConceptosGastos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }

        }

        private void Validar(CatConceptosGastos catConceptosGastos)
        {
            //Validar duplicados
            CatConceptosGastos existe = null;

            if (catConceptosGastos.idCatConceptoGasto > 0)
            {
                existe = Contexto.CatConceptosGastos.FirstOrDefault(r => r.comercioId == catConceptosGastos.comercioId
                                                                     && r.catRubroId == catConceptosGastos.catRubroId
                                                                     && r.descripcion.ToUpper() == catConceptosGastos.descripcion.ToUpper()
                                                                     && r.idCatConceptoGasto != catConceptosGastos.idCatConceptoGasto);
            }
            else
            {
                existe = Contexto.CatConceptosGastos.FirstOrDefault(r => r.comercioId == catConceptosGastos.comercioId && r.catRubroId == catConceptosGastos.catRubroId && r.descripcion.ToUpper() == catConceptosGastos.descripcion.ToUpper());
            }

            if (existe != null)
            {
                ModelState.AddModelError("descripcion", "Ya existe un concepto de gasto con esa descripción y rubro.");
            }

            if (catConceptosGastos.ponderacion <= 0)
            {
                ModelState.AddModelError("ponderacion", "Debe especificar un valor mayor a 0.");
            }

            //Validar sumatoria de ponderaciones no sea mayor a ponderación del rubro
            decimal totalPonderaciones = 0;
            var conceptos = Contexto.CatConceptosGastos.Where(c => c.catRubroId == catConceptosGastos.catRubroId &&
                                                                        c.idCatConceptoGasto != catConceptosGastos.idCatConceptoGasto);
            if (conceptos.Count() > 0)
            {
                totalPonderaciones = conceptos.Sum(c => c.ponderacion);
            }
            totalPonderaciones += catConceptosGastos.ponderacion;

            decimal ponderacionRubro = Contexto.CatRubros.FirstOrDefault(r => r.idCatRubro == catConceptosGastos.catRubroId).ponderacion;

            if (totalPonderaciones > ponderacionRubro)
            {
                ModelState.AddModelError("ponderacion", $"La suma de ponderaciones excede el { ponderacionRubro }%");
            }

        }
        #endregion

        #region POST

        // POST: CatRubros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatRubro,comercioId,descripcion,ponderacion,esCosto")] CatRubros catRubros)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                
                catRubros.comercioId = comercioId;
                catRubros.descripcion = catRubros.descripcion.ToUpper();
                catRubros.activo = true;

                Validar(catRubros);

                if (ModelState.IsValid)
                {
                    Contexto.CatRubros.Add(catRubros);
                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catRubros.comercioId);
                return View(catRubros);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // POST: CatRubros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatRubro,comercioId,descripcion,ponderacion,esCosto,activo")] CatRubros catRubros)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();

                Validar(catRubros);

                if (ModelState.IsValid)
                {
                    Contexto.Entry(catRubros).State = EntityState.Modified;
                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catRubros.comercioId);
                return View(catRubros);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        public ActionResult InactivarRubro(int id)
        {
            try
            {
                var rubro = Contexto.CatRubros.Find(id);
                rubro.activo = false;
                Contexto.Entry(rubro).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index", "CatRubros");
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "CatRubros");
            }
        }

        public ActionResult ActivarRubro(int id)
        {
            try
            {
                var rubro = Contexto.CatRubros.Find(id);
                rubro.activo = true;
                Contexto.Entry(rubro).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index", "CatRubros");
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "CatRubros");
            }
        }

        // POST: CatRubros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CatRubros catRubros = Contexto.CatRubros.Find(id);
                Contexto.CatRubros.Remove(catRubros);
                Contexto.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                if (EntidadTieneRelacion(ex))
                {
                    var catRubros = Contexto.CatRubros.Find(id);
                    catRubros.activo = false;
                    Contexto.Entry(catRubros).State = EntityState.Modified;
                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlertException(ex);
                    Logger.Error(ex);
                    return RedirectToAction("Index", "Catalogos");
                }
            }
        }

        private void Validar(CatRubros catRubros)
        {
            //Validar duplicados
            CatRubros existe = null;

            if (catRubros.idCatRubro > 0)
            {
                existe = Contexto.CatRubros.FirstOrDefault(r => r.comercioId == catRubros.comercioId 
                                                            && r.descripcion.ToUpper() == catRubros.descripcion.ToUpper() 
                                                            && r.idCatRubro != catRubros.idCatRubro);
            }
            else
            {
                existe = Contexto.CatRubros.FirstOrDefault(r => r.comercioId == catRubros.comercioId
                                                            && r.descripcion.ToUpper() == catRubros.descripcion.ToUpper());
            }             

            if (existe != null)
            {
                ModelState.AddModelError("descripcion", "Ya existe un rubro con esa descripción.");
            }

            if (catRubros.ponderacion <= 0)
            {
                ModelState.AddModelError("ponderacion", "Debe especificar un valor mayor a 0.");
            }

            //Validar sumatoria de ponderaciones no sea mayor a 100
            
            decimal totalPonderaciones = Contexto.CatRubros.Where(r => r.comercioId == catRubros.comercioId 
                                                                    && r.idCatRubro != catRubros.idCatRubro)
                                                                    .Sum(r => r.ponderacion) + catRubros.ponderacion;

            if (totalPonderaciones > 100)
            {
                ModelState.AddModelError("ponderacion", "La suma de ponderaciones excede el 100%");
            }

        }

        private SelectListItem[] ObtenerRubros(int? selected = null)
        {
            var gastosController = DependencyResolver.Current.GetService<GastosController>();
            gastosController.ControllerContext = new ControllerContext(this.Request.RequestContext, gastosController);

            return gastosController.ObtenerRubros(selected, esCostos: false);
        }

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