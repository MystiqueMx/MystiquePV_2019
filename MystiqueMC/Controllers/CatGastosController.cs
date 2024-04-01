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
    public class CatGastosController : BaseController
    {
	   readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: /CatGastos/

		[ValidatePermissionsAttribute(true)] 
        public ActionResult Index()

        {


            var catgastos = Contexto.CatGastos.Include(c => c.comercios);

            return View(catgastos.ToList());


        }

        // GET: /CatGastos/Details/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Details(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatGastos catGastos = Contexto.CatGastos.Find(id);

            if (catGastos == null)
            {
                return HttpNotFound();
            }
            return View(catGastos);
        }

        // GET: /CatGastos/Create
		[ValidatePermissionsAttribute(true)] 
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

        // POST: /CatGastos/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Create([Bind(Include="idCatGastos,comercioId,descripcion")] CatGastos catGastos)

        {
            try {

                catGastos.descripcion = catGastos.descripcion.ToUpper();

                if (ModelState.IsValid)
                {

                    Contexto.CatGastos.Add(catGastos);

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catGastos.comercioId);
                return View(catGastos);
            }
            catch (Exception ex){
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
            
        }

        // GET: /CatGastos/Edit/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Edit(int? id)

        {
            try {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CatGastos catGastos = Contexto.CatGastos.Find(id);

                if (catGastos == null)
                {
                    return HttpNotFound();
                }

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catGastos.comercioId);

                return View(catGastos);
            }
            catch (Exception ex) {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
            
        }

        // POST: /CatGastos/Edit/5

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Edit([Bind(Include="idCatGastos,comercioId,descripcion")] CatGastos catGastos)

        {
            try {

                catGastos.descripcion = catGastos.descripcion.ToUpper();

                if (ModelState.IsValid)
                {
                    Contexto.Entry(catGastos).State = EntityState.Modified;

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catGastos.comercioId);

                return View(catGastos);
            }
            catch (Exception ex) {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }

            
        }

        // GET: /CatGastos/Delete/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Delete(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatGastos catGastos = Contexto.CatGastos.Find(id);

            if (catGastos == null)
            {
                return HttpNotFound();
            }
            return View(catGastos);
        }

        // POST: /CatGastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteConfirmed(int id)

        {

            CatGastos catGastos = Contexto.CatGastos.Find(id);

            Contexto.CatGastos.Remove(catGastos);

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
