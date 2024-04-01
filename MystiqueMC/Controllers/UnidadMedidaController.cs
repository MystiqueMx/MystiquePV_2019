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
    public class UnidadMedidaController : BaseController
    {
        #region GET
        // GET: UnidadMedida
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var unidadMedida = Contexto.UnidadMedida.Include(u => u.comercios).Where(c => c.comercioId == comercioId);
                return View(unidadMedida.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: UnidadMedida/Create
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

        // GET: UnidadMedida/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UnidadMedida unidadMedida = Contexto.UnidadMedida.Find(id);
                if (unidadMedida == null)
                {
                    return HttpNotFound();
                }
                return View(unidadMedida);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: UnidadMedida/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadMedida unidadMedida = Contexto.UnidadMedida.Find(id);
            if (unidadMedida == null)
            {
                return HttpNotFound();
            }
            return View(unidadMedida);
        }
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idUnidadMedida,comercioId,descripcion")] UnidadMedida unidadMedida)
        {
            if (ModelState.IsValid)
            {
                Contexto.UnidadMedida.Add(unidadMedida);
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unidadMedida);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idUnidadMedida,comercioId,descripcion")] UnidadMedida unidadMedida)
        {
            if (ModelState.IsValid)
            {
                Contexto.Entry(unidadMedida).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unidadMedida);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnidadMedida unidadMedida = Contexto.UnidadMedida.Find(id);
            Contexto.UnidadMedida.Remove(unidadMedida);
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