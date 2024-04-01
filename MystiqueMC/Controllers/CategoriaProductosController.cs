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
    public class CategoriaProductosController : BaseController
    {
        #region GET
        // GET: CategoriaProductos
        public ActionResult Index()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var categoriaProductos = Contexto.CategoriaProductos
                    .Include(c => c.comercios)
                    .Include(c => c.usuarios)
                    .Include(c => c.Productos)
                    .Where(c => c.comercioId == comercioId);

                categoriaProductos = categoriaProductos.OrderBy(c => c.indice);

                return View(categoriaProductos.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CategoriaProductos/Create
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

        // GET: CategoriaProductos/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CategoriaProductos categoriaProductos = Contexto.CategoriaProductos.Find(id);
                if (categoriaProductos == null)
                {
                    return HttpNotFound();
                }

                return View(categoriaProductos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: CategoriaProductos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaProductos categoriaProductos = Contexto.CategoriaProductos.Find(id);
            if (categoriaProductos == null)
            {
                return HttpNotFound();
            }
            return View(categoriaProductos);
        }

        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCategoriaProducto,comercioId,descripcion,codigo,indice,usuarioRegistroId,fechaRegistro")] CategoriaProductos categoriaProductos)
        {
            if (ModelState.IsValid)
            {
                categoriaProductos.usuarioRegistroId = IdUsuarioActual;
                categoriaProductos.fechaRegistro = DateTime.Now;
                Contexto.CategoriaProductos.Add(categoriaProductos);
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoriaProductos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCategoriaProducto,comercioId,descripcion,codigo,indice,usuarioRegistroId,fechaRegistro")] CategoriaProductos categoriaProductos)
        {
            if (ModelState.IsValid)
            {
                categoriaProductos.fechaRegistro = DateTime.Now;
                Contexto.Entry(categoriaProductos).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoriaProductos);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoriaProductos categoriaProductos = Contexto.CategoriaProductos.Find(id);
            Contexto.CategoriaProductos.Remove(categoriaProductos);
            Contexto.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        public ActionResult ordenamiento(int id, int ordenamiento)
        {
            try
            {
                var orden = Contexto.CategoriaProductos.Find(id);
                orden.indice = ordenamiento;
                Contexto.Entry(orden).State = EntityState.Modified;
                Contexto.SaveChanges();

                return Json(new Ordenamiento { exito = true });
            }
            catch (Exception e)
            {
                return Json(new Ordenamiento { exito = false });
            }
        }

    }
}