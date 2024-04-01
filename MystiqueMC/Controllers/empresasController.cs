using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MystiqueMC.Helpers;
using MystiqueMC.DAL;

namespace MystiqueMC.Controllers
{

	[Authorize]
    [ValidatePermissionsAttribute(false)] 
    public class empresasController : BaseController
    {
	   readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: /empresas/

        [ValidatePermissionsAttribute(true)] 
        public async Task<ActionResult> Index()
        {
            return View(await Contexto.empresas.ToListAsync());           
        }

        // GET: /empresas/Details/5
		[ValidatePermissionsAttribute(true)] 

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            empresas empresas = await Contexto.empresas.FindAsync(id);

            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        // GET: /empresas/Create
		[ValidatePermissionsAttribute(true)] 
        public ActionResult Create()
        {
            return View();
        }

        // POST: /empresas/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)] 

        public async Task<ActionResult> Create([Bind(Include="acronimo,nombre")] empresas empresa)
        {
            #region Create
            if(empresa.acronimo == null || empresa.acronimo == string.Empty)
                ModelState.AddModelError("El acronimo es requerido", empresa.acronimo);
            if (empresa.nombre == null || empresa.nombre == string.Empty)
                ModelState.AddModelError("El nombre es requerido", empresa.nombre);
            //TODO validar el mostrar mensajes de error con valiation message
            if (ModelState.IsValid)
            {
                empresa.fechaRegistro = DateTime.Now;
                empresa.estatus = true;
                empresa.guidEmpresa = Guid.NewGuid().ToString();
                Contexto.empresas.Add(empresa);

                await Contexto.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(empresa);
            #endregion
        }

        // GET: /empresas/Edit/5
        [ValidatePermissionsAttribute(true)] 

        public async Task<ActionResult> Edit(int? id)
        {
            #region Edit
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            empresas empresas = await Contexto.empresas.FindAsync(id);

            if (empresas == null)
            {
                return HttpNotFound();
            }

            return View(empresas);
            #endregion
        }

        // POST: /empresas/Edit/5

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
		[ValidatePermissionsAttribute(true)] 

        public async Task<ActionResult> Edit([Bind(Include="idEmpresa,guidEmpresa,acronimo,nombre,fechaRegistro,estatus")] empresas empresa)
        {
            #region Edit
            if (empresa.acronimo == null || empresa.acronimo == string.Empty)
                ModelState.AddModelError("El acronimo es requerido", empresa.acronimo);
            if (empresa.nombre == null || empresa.nombre == string.Empty)
                ModelState.AddModelError("El nombre es requerido", empresa.nombre);

            if (ModelState.IsValid)
            {
                Contexto.Entry(empresa).State = EntityState.Modified;

                await Contexto.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(empresa);
            #endregion
        }

        // GET: /empresas/Delete/5
        [ValidatePermissionsAttribute(true)] 

        public async Task<ActionResult> Delete(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            empresas empresas = await Contexto.empresas.FindAsync(id);

            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        // POST: /empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> DeleteConfirmed(int id)

        {

            empresas empresas = await Contexto.empresas.FindAsync(id);

            Contexto.empresas.Remove(empresas);

            await Contexto.SaveChangesAsync();

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
