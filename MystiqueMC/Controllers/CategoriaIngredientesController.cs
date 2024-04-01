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
    public class CategoriaIngredientesController : BaseController
    {
        #region GET
        // GET: CategoriaIngredientes
        public ActionResult Index()
        {
            try
            { 
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var categoriaIngrediente = Contexto.CategoriaIngrediente.Include(c => c.comercios).Where(c => c.comercioId == comercioId);
                return View(categoriaIngrediente.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CategoriaIngredientes/Create
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


        // GET: CategoriaIngredientes/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategoriaIngrediente categoriaIngrediente = Contexto.CategoriaIngrediente.Find(id);
                if (categoriaIngrediente == null)
                {
                    return HttpNotFound();
                }
                return View(categoriaIngrediente);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: CategoriaIngredientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaIngrediente categoriaIngrediente = Contexto.CategoriaIngrediente.Find(id);
            if (categoriaIngrediente == null)
            {
                return HttpNotFound();
            }
            return View(categoriaIngrediente);
        }
        #endregion

        #region POST
        // POST: CategoriaIngredientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCategoriaIngrediente,comercioId,descripcion")] CategoriaIngrediente categoriaIngrediente)
        {
            if (ModelState.IsValid)
            {
                Contexto.CategoriaIngrediente.Add(categoriaIngrediente);
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoriaIngrediente);
        }


        // POST: CategoriaIngredientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCategoriaIngrediente,comercioId,descripcion")] CategoriaIngrediente categoriaIngrediente)
        {
            if (ModelState.IsValid)
            {
                Contexto.Entry(categoriaIngrediente).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoriaIngrediente);
        }

        // POST: CategoriaIngredientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoriaIngrediente categoriaIngrediente = Contexto.CategoriaIngrediente.Find(id);
            Contexto.CategoriaIngrediente.Remove(categoriaIngrediente);
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