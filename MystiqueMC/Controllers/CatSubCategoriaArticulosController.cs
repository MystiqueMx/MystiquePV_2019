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
    [ValidatePermissionsAttribute(false)]
    public class CatSubCategoriaArticulosController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: /CatSubCategoriaArticulos/

        [ValidatePermissionsAttribute(true)]
        public ActionResult Index()

        {


            var catsubcategoriaarticulos = Contexto.CatSubCategoriaArticulos.Include(c => c.CatCategoriaArticulos);

            return View(catsubcategoriaarticulos.ToList());


        }

        // GET: /CatSubCategoriaArticulos/Details/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult Details(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatSubCategoriaArticulos catSubCategoriaArticulos = Contexto.CatSubCategoriaArticulos.Find(id);

            if (catSubCategoriaArticulos == null)
            {
                return HttpNotFound();
            }
            return View(catSubCategoriaArticulos);
        }

        // GET: /CatSubCategoriaArticulos/Create
        [ValidatePermissionsAttribute(true)]
        public ActionResult Create()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa)
                    .Select(c => c.idComercio)
                    .First();
                ViewBag.comercioId = comercioId;

                ViewBag.catCategoriaArticulosId = new SelectList(Contexto.CatCategoriaArticulos, "idCatCategoriaArticulos", "descripcion");

                return View();
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // POST: /CatSubCategoriaArticulos/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidatePermissionsAttribute(true)]

        public ActionResult Create([Bind(Include = "idCatSubCategoriaArticulos,comercioId,catCategoriaArticulosId,descripcion")] CatSubCategoriaArticulos catSubCategoriaArticulos)
        {

            try
            {
                catSubCategoriaArticulos.descripcion = catSubCategoriaArticulos.descripcion.ToUpper();

                if (ModelState.IsValid)
                {
                    ViewBag.catCategoriaArticulosId = new SelectList(Contexto.CatCategoriaArticulos, "idCatCategoriaArticulos", "descripcion");

                    var selection = Contexto.CatSubCategoriaArticulos.Where(c => c.catCategoriaArticulosId == catSubCategoriaArticulos.catCategoriaArticulosId);

                    if (selection.Any(coincidencia => coincidencia.descripcion.Equals(catSubCategoriaArticulos.descripcion)))
                    {
                        ModelState.AddModelError("descripcion", "Este articulo ya se encuentra registrado");

                        return View(catSubCategoriaArticulos);
                    }
                    else
                    {
                        Contexto.CatSubCategoriaArticulos.Add(catSubCategoriaArticulos);

                        Contexto.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catSubCategoriaArticulos.comercioId);

                return View(catSubCategoriaArticulos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }

        }


        // GET: /CatSubCategoriaArticulos/Edit/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult Edit(int? id)

        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CatSubCategoriaArticulos catSubCategoriaArticulos = Contexto.CatSubCategoriaArticulos.Find(id);

                if (catSubCategoriaArticulos == null)
                {
                    return HttpNotFound();
                }

                ViewBag.catCategoriaArticulosId = new SelectList(Contexto.CatCategoriaArticulos, "idCatCategoriaArticulos", "descripcion", catSubCategoriaArticulos.catCategoriaArticulosId);

                return View(catSubCategoriaArticulos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");

            }
        }

        // POST: /CatSubCategoriaArticulos/Edit/5

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidatePermissionsAttribute(true)]

        public ActionResult Edit([Bind(Include = "idCatSubCategoriaArticulos,comercioId,catCategoriaArticulosId,descripcion")] CatSubCategoriaArticulos catSubCategoriaArticulos)
        {
            try
            {
                catSubCategoriaArticulos.descripcion = catSubCategoriaArticulos.descripcion.ToUpper();

                if (ModelState.IsValid)
                {

                    ViewBag.catCategoriaArticulosId = new SelectList(Contexto.CatCategoriaArticulos, "idCatCategoriaArticulos", "descripcion");

                    var selection = Contexto.CatSubCategoriaArticulos.Where(c => c.catCategoriaArticulosId == catSubCategoriaArticulos.catCategoriaArticulosId);

                    if (selection.Any(coincidencia => coincidencia.descripcion.Equals(catSubCategoriaArticulos.descripcion)))
                    {

                        ModelState.AddModelError("descripcion", "Este registro ya existe");
                        return View(catSubCategoriaArticulos);
                    }
                    else
                    {

                        Contexto.Entry(catSubCategoriaArticulos).State = EntityState.Modified;
                        Contexto.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                return View(catSubCategoriaArticulos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: /CatSubCategoriaArticulos/Delete/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult Delete(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatSubCategoriaArticulos catSubCategoriaArticulos = Contexto.CatSubCategoriaArticulos.Find(id);

            if (catSubCategoriaArticulos == null)
            {
                return HttpNotFound();
            }
            return View(catSubCategoriaArticulos);
        }

        // POST: /CatSubCategoriaArticulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteConfirmed(int id)

        {

            CatSubCategoriaArticulos catSubCategoriaArticulos = Contexto.CatSubCategoriaArticulos.Find(id);

            Contexto.CatSubCategoriaArticulos.Remove(catSubCategoriaArticulos);

            Contexto.SaveChanges();

            return RedirectToAction("Index");
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
