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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace MystiqueMC.Controllers
{

	[Authorize]
    [ValidatePermissionsAttribute(false)] 
    public class CatCategoriaArticulosController : BaseController
    {
	   readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: /CatCategoriaArticulos/

		[ValidatePermissionsAttribute(true)] 
        public ActionResult Index()

        {


            var catcategoriaarticulos = Contexto.CatCategoriaArticulos.Include(c => c.comercios);

            return View(catcategoriaarticulos.ToList());


        }

        // GET: /CatCategoriaArticulos/Details/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Details(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatCategoriaArticulos catCategoriaArticulos = Contexto.CatCategoriaArticulos.Find(id);

            if (catCategoriaArticulos == null)
            {
                return HttpNotFound();
            }
            return View(catCategoriaArticulos);
        }
        

        // GET: /CatCategoriaArticulos/Create
        [ValidatePermissionsAttribute(true)] 
        public ActionResult Create(int bandera)
        {

            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                ViewBag.comercioId = comercioId;
                ViewBag.ban = bandera;
                //ViewBag.ruta = Request.UrlReferrer.ToString();
                return View();
                
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // POST: /CatCategoriaArticulos/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)]

        public ActionResult Create( [Bind(Include="idCatCategoriaArticulos,comercioId,descripcion")] CatCategoriaArticulos catCategoriaArticulos, int ban)
        {
            try
            {
                
                catCategoriaArticulos.descripcion = catCategoriaArticulos.descripcion.ToUpper();
                
                if (ModelState.IsValid)
                {
                    if (Contexto.CatCategoriaArticulos.Any(coincidencia => coincidencia.descripcion.Equals(catCategoriaArticulos.descripcion)))
                       {
                        ModelState.AddModelError("descripcion", "Este articulo ya se encuentra registrado");
                        return View(catCategoriaArticulos);
                    }
                    else {
                        Contexto.CatCategoriaArticulos.Add(catCategoriaArticulos);
                           
                        Contexto.SaveChanges();


                        if (ban == -1) { 
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return RedirectToAction("Index","CatSubCategoriaArticulos");
                        }
                    }
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catCategoriaArticulos.comercioId);

                return View(catCategoriaArticulos);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
        }

        // GET: /CatCategoriaArticulos/Edit/5
        [ValidatePermissionsAttribute(true)] 

        public ActionResult Edit(int? id)

        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CatCategoriaArticulos catCategoriaArticulos = Contexto.CatCategoriaArticulos.Find(id);

                if (catCategoriaArticulos == null)
                {
                    return HttpNotFound();
                }

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catCategoriaArticulos.comercioId);

                return View(catCategoriaArticulos);
            } catch(Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }
            
        }

        // POST: /CatCategoriaArticulos/Edit/5

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Edit([Bind(Include="idCatCategoriaArticulos,comercioId,descripcion")] CatCategoriaArticulos catCategoriaArticulos)

        {
            try {

                catCategoriaArticulos.descripcion = catCategoriaArticulos.descripcion.ToUpper();

                if (ModelState.IsValid)
                {
                    if (Contexto.CatCategoriaArticulos.Any(model => model.descripcion.Equals(catCategoriaArticulos.descripcion)))
                    {
                        if (Contexto.CatCategoriaArticulos.Any(model => model.idCatCategoriaArticulos.Equals(catCategoriaArticulos.idCatCategoriaArticulos)))
                        {

                            ModelState.AddModelError("descripcion", "Se ingreso el nombre ya registrado");
                            return View(catCategoriaArticulos);
                        }
                        else
                        {
                            ModelState.AddModelError("descripcion", "Este articulo ya se encuentra registrado");
                            return View(catCategoriaArticulos);
                        }
                    }
                    else
                    {

                        Contexto.Entry(catCategoriaArticulos).State = EntityState.Modified;

                        Contexto.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", catCategoriaArticulos.comercioId);

                return View(catCategoriaArticulos);
            } catch(Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
            }

            
        }

        // GET: /CatCategoriaArticulos/Delete/5
		[ValidatePermissionsAttribute(true)] 

        public ActionResult Delete(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatCategoriaArticulos catCategoriaArticulos = Contexto.CatCategoriaArticulos.Find(id);

            if (catCategoriaArticulos == null)
            {
                return HttpNotFound();
            }
            return View(catCategoriaArticulos);
        }

        // POST: /CatCategoriaArticulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteConfirmed(int id)

        {
            try
            {
                
                CatCategoriaArticulos catCategoriaArticulos = Contexto.CatCategoriaArticulos.Find(id);

                if (Contexto.CatSubCategoriaArticulos.Any(m => m.catCategoriaArticulosId.Equals(catCategoriaArticulos.idCatCategoriaArticulos)))
                {
                    ModelState.AddModelError("descripcion", "Esta categoria cuenta con subcategorias, no se puede eliminar.");

                    return View(catCategoriaArticulos);
                }
                else
                {
                    Contexto.CatCategoriaArticulos.Remove(catCategoriaArticulos);

                    Contexto.SaveChanges();

                    return RedirectToAction("Index");
                }
            }catch(Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Catalogos");
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
