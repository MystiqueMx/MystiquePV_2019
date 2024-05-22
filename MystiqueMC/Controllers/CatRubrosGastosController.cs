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
    public class CatRubrosGastosController : BaseController
    {
        #region GET
        // GET: CatRubrosGastos
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var catRubrosGastos = Contexto.CatRubrosGastos.Include(c => c.comercios)
                    .Where(w => w.comercioId == comercioId);
                return View(catRubrosGastos.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }


        // GET: CatRubrosGastos/Create
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

        // GET: CatRubrosGastos/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {

                if (id == null)
                {

                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CatRubrosGastos catRubrosGastos = Contexto.CatRubrosGastos.Find(id);
                if (catRubrosGastos == null)
                {
                    return HttpNotFound();
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catRubrosGastos.comercioId);
                return View(catRubrosGastos);

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
                CatRubrosGastos catRubrosGastos = Contexto.CatRubrosGastos.Find(id);
                if (catRubrosGastos == null)
                {
                    return HttpNotFound();
                }
                return View(catRubrosGastos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region POST

        // POST: CatRubros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatRubroGasto,comercioId,decripcion")] CatRubrosGastos catRubrosGastos)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                catRubrosGastos.comercioId = comercioId;
                if (ModelState.IsValid)
                {
                    Contexto.CatRubrosGastos.Add(catRubrosGastos);
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catRubrosGastos.comercioId);
                return View(catRubrosGastos);

            }
            catch (Exception ex)
            {

                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CatRubros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatRubroGasto,comercioId,decripcion")] CatRubrosGastos catRubrosGasto)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                catRubrosGasto.comercioId = comercioId;
                if (ModelState.IsValid)
                {
                    Contexto.Entry(catRubrosGasto).State = EntityState.Modified;
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catRubrosGasto.comercioId);
                return View(catRubrosGasto);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CatRubros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatRubrosGastos catRubrosGastos = Contexto.CatRubrosGastos.Find(id);
            Contexto.CatRubrosGastos.Remove(catRubrosGastos);
            Contexto.SaveChanges();
            return RedirectToAction("Index");
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