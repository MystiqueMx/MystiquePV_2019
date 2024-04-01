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

namespace MystiqueMC.Controllers
{
	[Authorize]      
    public class CatConceptosGastosController : BaseController
    {
       // GET: /CatConceptosGastos/		
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var conceptosGastos = Contexto.CatConceptosGastos
                                                .Include(c => c.CatRubros)
                                                .Where(w => w.comercioId == comercioId);

                return View(conceptosGastos.ToList());

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }
        }       

        // GET: /CatConceptosGastos/Create
        public ActionResult Create()
        {           
            try
            {
                ViewBag.Rubros = ObtenerRubros();

                return View();

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }
        }

        // POST: /CatConceptosGastos/Create   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="idCatConceptoGasto,catRubroId,descripcion,activo,ponderacion")] CatConceptosGastos catConceptosGastos)
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

                    return RedirectToAction("Index", "CatRubros", null);
                }

                ViewBag.Rubros = ObtenerRubros();


                return View(catConceptosGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }

        }

        // GET: /CatConceptosGastos/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {               
                if (id == null)
                {
                    ShowAlertDanger("No se encontró el concepto seleccionado.");
                    RedirectToAction("Index");
                }

                CatConceptosGastos catConceptosGastos = Contexto.CatConceptosGastos.Find(id);

                if (catConceptosGastos == null)
                {
                    ShowAlertDanger("No se encontró el concepto seleccionado.");
                    RedirectToAction("Index");
                }

                ViewBag.Rubros = ViewBag.Rubros = ObtenerRubros(catConceptosGastos.catRubroId); 

                return View(catConceptosGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }           
        }

        // POST: /CatConceptosGastos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]	
        public ActionResult Edit([Bind(Include= "idCatConceptoGasto,comercioId,catRubroId,descripcion,activo,ponderacion")] CatConceptosGastos catConceptosGastos)
        {
            try
            {
                Validar(catConceptosGastos);

                if (ModelState.IsValid)
                {
                    catConceptosGastos.descripcion = catConceptosGastos.descripcion.ToUpper();
                    Contexto.Entry(catConceptosGastos).State = EntityState.Modified;

                    Contexto.SaveChanges();

                    return RedirectToAction("Index", "CatRubros", null);
                }

                ViewBag.Rubros = ViewBag.Rubros = ObtenerRubros(catConceptosGastos.catRubroId);

                return View(catConceptosGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }
            
        }

        // GET: /CatConceptosGastos/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    ShowAlertDanger("No se encontró el concepto seleccionado.");
                    RedirectToAction("Index");
                }

                CatConceptosGastos catConceptosGastos = Contexto.CatConceptosGastos.Find(id);

                if (catConceptosGastos == null)
                {
                    ShowAlertDanger("No se encontró el concepto seleccionado.");
                    RedirectToAction("Index");
                }
                
                return View(catConceptosGastos);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
            }
        }

        // POST: /CatConceptosGastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CatConceptosGastos catConceptosGastos = Contexto.CatConceptosGastos.Find(id);

                Contexto.CatConceptosGastos.Remove(catConceptosGastos);

                Contexto.SaveChanges();

                return RedirectToAction("Index", "CatRubros", null);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Menu", "Egresos");
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

        private SelectListItem[] ObtenerRubros(int? selected = null)
        {
            var gastosController = DependencyResolver.Current.GetService<GastosController>();
            gastosController.ControllerContext = new ControllerContext(this.Request.RequestContext, gastosController);

            return gastosController.ObtenerRubros(selected);
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
