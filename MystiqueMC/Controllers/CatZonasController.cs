using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{

    [Authorize]
    [ValidatePermissionsAttribute(false)]
    public class CatZonasController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: /CatZonas/

        [ValidatePermissionsAttribute(true)]
        public ActionResult Index()

        {
            var catzonas = Contexto.CatZonas.Include(c => c.empresas).Include(c => c.usuarios);

            return View(catzonas.ToList());


        }

        // GET: /CatZonas/Details/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult Details(int? id)

        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CatZonas catZonas = Contexto.CatZonas.Find(id);

                if (catZonas == null)
                {
                    return HttpNotFound();
                }
                return View(catZonas);
            
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "CatZona");
            }
        }

        // GET: /CatZonas/Create
        [ValidatePermissionsAttribute(true)]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidatePermissionsAttribute(true)]

        public ActionResult Create([Bind(Include = "idCatZona,descripcion,activo")] CatZonas catZonas)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuarioFirmado = Session.ObtenerUsuario();

                    catZonas.usuarioRegistroId = usuarioFirmado.idUsuario;
                    catZonas.empresaId = usuarioFirmado.empresaId;
                    catZonas.fechaRegistro = DateTime.Now;

                    Contexto.CatZonas.Add(catZonas);

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(catZonas);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "CatZona");
            }

        }

        // GET: /CatZonas/Edit/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult Edit(int? id)

        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CatZonas catZonas = Contexto.CatZonas.Find(id);

                if (catZonas == null)
                {
                    return HttpNotFound();
                }
                return View(catZonas);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "CatZona");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidatePermissionsAttribute(true)]

        public ActionResult Edit([Bind(Include = "idCatZona,descripcion,activo,usuarioRegistroId,fechaRegistro,empresaId")] CatZonas catZonas)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    Contexto.Entry(catZonas).State = EntityState.Modified;

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(catZonas);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "CatZona");
            }
        }

        // GET: /CatZonas/Delete/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult Delete(int? id)

        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CatZonas catZonas = Contexto.CatZonas.Find(id);

                if (catZonas == null)
                {
                    return HttpNotFound();
                }
                return View(catZonas);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "CatZona");
            }
        }

        // POST: /CatZonas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CatZonas catZonas = Contexto.CatZonas.Find(id);

                Contexto.CatZonas.Remove(catZonas);

                Contexto.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "CatZona");
            }
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
