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
    public class CatMovimientosInventariosController : BaseController
    {
        #region GET
        // GET: CatMovimientosInventarios
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var catMovimientoInventarios = Contexto.CatMovimientoInventarios.Include(c => c.comercios).Include(c => c.usuarios).Where(c => c.comercioId == comercioId);
                return View(catMovimientoInventarios.ToList());
            }
            catch (Exception ex)
            {
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
                return View();
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
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
                CatMovimientoInventarios catMovimientoInventarios = Contexto.CatMovimientoInventarios.Find(id);
                if (catMovimientoInventarios == null)
                {
                    return HttpNotFound();
                }
                return View(catMovimientoInventarios);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatMovimientoInventarios catMovimientoInventarios = Contexto.CatMovimientoInventarios.Find(id);
            if (catMovimientoInventarios == null)
            {
                return HttpNotFound();
            }
            return View(catMovimientoInventarios);
        }

        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatMovimientoInventario,comercioId,descripcion")] CatMovimientoInventarios catMovimientoInventarios)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    catMovimientoInventarios.fechaRegistro = DateTime.Now;
                    catMovimientoInventarios.usuarioRegistroId = IdUsuarioActual;
                    Contexto.CatMovimientoInventarios.Add(catMovimientoInventarios);
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return View(catMovimientoInventarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatMovimientoInventario,comercioId,descripcion")] CatMovimientoInventarios catMovimientoInventarios)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    catMovimientoInventarios.fechaRegistro = DateTime.Now;
                    catMovimientoInventarios.usuarioRegistroId = IdUsuarioActual;
                    Contexto.Entry(catMovimientoInventarios).State = EntityState.Modified;
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return View(catMovimientoInventarios);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatMovimientoInventarios catMovimientoInventarios = Contexto.CatMovimientoInventarios.Find(id);
            Contexto.CatMovimientoInventarios.Remove(catMovimientoInventarios);
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