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
    public class AreaPreparacionController : BaseController
    {
        #region GET
        // GET: AreaPreparacion
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var areaPreparacion = Contexto.AreaPreparacion.Include(a => a.comercios).Where(c => c.comercioId == comercioId);
                return View(areaPreparacion.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: AreaPreparacion/Create
        public ActionResult Create()																			
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                ViewBag.comercioId = comercioId;
                return View();
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }
		











		
        // GET: AreaPreparacion/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AreaPreparacion areaPreparacion = Contexto.AreaPreparacion.Find(id);
                if (areaPreparacion == null)
                {
                    return HttpNotFound();
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", areaPreparacion.comercioId);
                return View(areaPreparacion);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: AreaPreparacion/Delete/5
        public ActionResult Delete(int? id)																		
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaPreparacion areaPreparacion = Contexto.AreaPreparacion.Find(id);
            if (areaPreparacion == null)
            {
                return HttpNotFound();
            }
            return View(areaPreparacion);
        }

        #endregion

        #region POST
        // POST: AreaPreparacion/Create
		
		
		
		
		
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idAreaPreparacion,comercioId,descripcion")] AreaPreparacion areaPreparacion)																																																																		
        {
            if (ModelState.IsValid)
            {
                Contexto.AreaPreparacion.Add(areaPreparacion);
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", areaPreparacion.comercioId);
            return View(areaPreparacion);
        }













        // POST: AreaPreparacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idAreaPreparacion,comercioId,descripcion")] AreaPreparacion areaPreparacion)
        {
            if (ModelState.IsValid)
            {
                Contexto.Entry(areaPreparacion).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", areaPreparacion.comercioId);
            return View(areaPreparacion);
        }
		
		
		
		
		
		
		
		
		
		
		
		
		
		
        // POST: AreaPreparacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		
        public ActionResult DeleteConfirmed(int id)
        {
            AreaPreparacion areaPreparacion = Contexto.AreaPreparacion.Find(id);										   
            Contexto.AreaPreparacion.Remove(areaPreparacion);
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