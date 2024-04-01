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
    public class TipoPagosController : BaseController
    {
        #region GET
        // GET: TipoPagos
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var catTipoPagos = Contexto.CatTipoPagos
                    .Include(c => c.comercios)
                    .Where(w => w.comercioId == comercioId || w.comercioId == null);
                return View(catTipoPagos.AsEnumerable());
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
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CatTipoPagos catTipoPagos = Contexto.CatTipoPagos.Find(id);
                if (catTipoPagos == null)
                {
                    return HttpNotFound();
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catTipoPagos.comercioId);
                return View(catTipoPagos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }

        }

        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatTipoPago,descripcion,comercioId,activo,cajaBase,aplicaCxc")] CatTipoPagos catTipoPagos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (catTipoPagos.aplicaCxc == null) catTipoPagos.aplicaCxc = false;
                    var usuarioFirmado = Session.ObtenerUsuario();
                    int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                    catTipoPagos.comercioId = comercioId;
                    Contexto.CatTipoPagos.Add(catTipoPagos);
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catTipoPagos.comercioId);
                return View(catTipoPagos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatTipoPago,descripcion,comercioId,activo,cajaBase,aplicaCxc")] CatTipoPagos catTipoPagos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (catTipoPagos.aplicaCxc == null) catTipoPagos.aplicaCxc = false;
                    Contexto.Entry(catTipoPagos).State = EntityState.Modified;
                    Contexto.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catTipoPagos.comercioId);
                return View(catTipoPagos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }

        }
        #endregion
    }
}