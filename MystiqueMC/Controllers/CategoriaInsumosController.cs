using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace MystiqueMC.Controllers
{
    public class CategoriaInsumosController : BaseController
    {
        #region GET
        // GET: CategoriaProductos
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var categoriaInsumo = Contexto.CategoriaInsumo
                    .Include(c => c.comercios)

                    .Where(c => c.comercioId == comercioId);

                return View(categoriaInsumo.ToList().OrderBy(c => c.descripcion));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CategoriaIsumo/Create
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

        // GET: CategoriaInsimo/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategoriaInsumo categoriaInsumos = Contexto.CategoriaInsumo.Find(id);
                if (categoriaInsumos == null)
                {
                    return HttpNotFound();
                }

                return View(categoriaInsumos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: CategoriaInsumo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaInsumo categoriaInsumos = Contexto.CategoriaInsumo.Find(id);
            if (categoriaInsumos == null)
            {
                return HttpNotFound();
            }
            return View(categoriaInsumos);
        }

        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCategoriaInsumo,comercioId,descripcion")] CategoriaInsumo categoriaInsumo)
        {
            if (ModelState.IsValid)
            {
                Contexto.CategoriaInsumo.Add(categoriaInsumo);
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoriaInsumo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCategoriaInsumo,comercioId,descripcion")] CategoriaInsumo categoriaInsumo)
        {
            if (ModelState.IsValid)
            {
                Contexto.Entry(categoriaInsumo).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoriaInsumo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CategoriaInsumo categoriaInsumo = Contexto.CategoriaInsumo.Find(id);
                Contexto.CategoriaInsumo.Remove(categoriaInsumo);
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ShowAlertDanger(" Otros insumos dependen de esta familia que desea eliminar.");

                return RedirectToAction("Index");
            }
        }

        #endregion
    }
}