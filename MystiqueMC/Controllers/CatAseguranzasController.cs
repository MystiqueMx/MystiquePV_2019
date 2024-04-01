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
using System.Configuration;

namespace MystiqueMC.Controllers
{

	[Authorize]
    [ValidatePermissionsAttribute(false)] 
    public class CatAseguranzasController : BaseController
    {
	   readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private string ServerPath => Server.MapPath(@"~");
        private const string IMAGE_EXTENSION = ".png";
        private readonly string HOSTNAME_IMAGENES = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");
        // GET: /CatAseguranzas/

        [ValidatePermissionsAttribute(true)] 
        public ActionResult Index()

        {



            return View(Contexto.CatAseguranzas.ToList());


        }

        // GET: /CatAseguranzas/Details/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Details(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatAseguranzas catAseguranzas = Contexto.CatAseguranzas.Find(id);

            if (catAseguranzas == null)
            {
                return HttpNotFound();
            }
            return View(catAseguranzas);
        }

        // GET: /CatAseguranzas/Create
		[ValidatePermissionsAttribute(true)] 
        public ActionResult Create()
        {
            ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;

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

        // POST: /CatAseguranzas/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Create([Bind(Include="idCatAseguranzas,descripcion,descripcionIngles,imagenAseguranza")] CatAseguranzas catAseguranzas)

        {
            try
            {
                catAseguranzas.descripcion = catAseguranzas.descripcion.ToUpper();
                catAseguranzas.descripcionIngles = catAseguranzas.descripcionIngles.ToUpper();

                if (ModelState.IsValid)
                {

                    Contexto.CatAseguranzas.Add(catAseguranzas);

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {

                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "CatAseguranzas");
            }

            return View(catAseguranzas);
        }

        // GET: /CatAseguranzas/Edit/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Edit(int? id)

        {
           try{
                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CatAseguranzas catAseguranzas = Contexto.CatAseguranzas.Find(id);

                if (catAseguranzas == null)
                {
                    return HttpNotFound();
                }

                return View(catAseguranzas);

            } catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "CatAseguranzas");
            }

            
        }

        // POST: /CatAseguranzas/Edit/5

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Edit([Bind(Include="idCatAseguranzas,descripcion,descripcionIngles,imagenAseguranza")] CatAseguranzas catAseguranzas)

        {
            try
            {
                catAseguranzas.descripcion = catAseguranzas.descripcion.ToUpper();
                catAseguranzas.descripcionIngles = catAseguranzas.descripcionIngles.ToUpper();
                if (ModelState.IsValid)
                {
                   
                    Contexto.Entry(catAseguranzas).State = EntityState.Modified;

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "CatAseguranzas");
            }

            return View(catAseguranzas);
        }

        // GET: /CatAseguranzas/Delete/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Delete(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatAseguranzas catAseguranzas = Contexto.CatAseguranzas.Find(id);

            if (catAseguranzas == null)
            {
                return HttpNotFound();
            }
            return View(catAseguranzas);
        }

        // POST: /CatAseguranzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteConfirmed(int id)

        {

            CatAseguranzas catAseguranzas = Contexto.CatAseguranzas.Find(id);

            Contexto.CatAseguranzas.Remove(catAseguranzas);

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
