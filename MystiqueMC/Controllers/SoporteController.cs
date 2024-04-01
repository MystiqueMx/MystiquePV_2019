using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{

    [Authorize]
    [ValidatePermissions]
    public class SoporteController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: /Soporte/
        public ActionResult Index()
        {
            var Usuario = Session.ObtenerUsuario();
            if (!Request.IsAuthenticated || Usuario == null)
                return new { success = false }.ToJsonResult();

            string rol = Session.ObtenerRol();
            var usuarioFirmado = Session.ObtenerUsuario();

            var configuracionsistema = Contexto.configuracionSistema
                .Where(w => w.empresaId == usuarioFirmado.empresaId)
                .FirstOrDefault();

            return PartialView(configuracionsistema);
        }

        // GET: /Soporte/Details/5
        public ActionResult Details(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            configuracionSistema configuracionSistema = Contexto.configuracionSistema.Find(id);

            if (configuracionSistema == null)
            {
                return HttpNotFound();
            }
            return View(configuracionSistema);
        }

        // GET: /Soporte/Create
        public ActionResult Create()
        {

            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "guidEmpresa");

            return View();
        }

        // POST: /Soporte/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "configuracionSistemaId,empresaId,diasFechasFiltros,txtVincularMembresiaApp,urlTerminosCondiciones,telefonoContacto,correoContacto,txtSolicitarBeneficioAbajo,txtTerminosCondiciones,txtSoporte,costoMembresia,conektaPublicKey,conektaPrivateKey,conektaApiVersion,fechaInicioMembresia,fechaFinMembresia,ftpPerfil,ftpUser,ftpPassword,versionAndroid,versionAndroidTest,versioniOS,versioniOSTest,vrlPublicacion,dedicadoAComercio,tienePaginaVinculacion,tienePaginaConekta,tienePaginaPuntos,modoTiempo,tiempoProgramado,tiempoIntervalo,intervaloZonaCS,tituloCompartir,contenidoCompartir,idQDC,mostrarComercios,mostrarSucursales")] configuracionSistema configuracionSistema)

        {
            if (ModelState.IsValid)
            {

                Contexto.configuracionSistema.Add(configuracionSistema);

                Contexto.SaveChanges();

                return RedirectToAction("Index");
            }


            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "guidEmpresa", configuracionSistema.empresaId);

            return View(configuracionSistema);
        }

        // GET: /Soporte/Edit/5
        public ActionResult Edit(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            configuracionSistema configuracionSistema = Contexto.configuracionSistema.Find(id);

            if (configuracionSistema == null)
            {
                return HttpNotFound();
            }

            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "guidEmpresa", configuracionSistema.empresaId);

            return View(configuracionSistema);
        }

        // POST: /Soporte/Edit/5

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "configuracionSistemaId,empresaId,diasFechasFiltros,txtVincularMembresiaApp,urlTerminosCondiciones,telefonoContacto,correoContacto,txtSolicitarBeneficioAbajo,txtTerminosCondiciones,txtSoporte,costoMembresia,conektaPublicKey,conektaPrivateKey,conektaApiVersion,fechaInicioMembresia,fechaFinMembresia,ftpPerfil,ftpUser,ftpPassword,versionAndroid,versionAndroidTest,versioniOS,versioniOSTest,vrlPublicacion,dedicadoAComercio,tienePaginaVinculacion,tienePaginaConekta,tienePaginaPuntos,modoTiempo,tiempoProgramado,tiempoIntervalo,intervaloZonaCS,tituloCompartir,contenidoCompartir,idQDC,mostrarComercios,mostrarSucursales")] configuracionSistema configuracionSistema,
            string mostrarComercios, string mostrarSucursales)
        {
            if (ModelState.IsValid)
            {
                Contexto.Entry(configuracionSistema).State = EntityState.Modified;

                Contexto.SaveChanges();

                return RedirectToAction("Fidelizacion", "Home", new { tabID = "soporte_button" });
                //return RedirectToAction("Index");
            }

            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "guidEmpresa", configuracionSistema.empresaId);

            return RedirectToAction("Fidelizacion", "Home", new { tabID = "soporte_button" });

        }

        // GET: /Soporte/Delete/5
        public ActionResult Delete(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            configuracionSistema configuracionSistema = Contexto.configuracionSistema.Find(id);

            if (configuracionSistema == null)
            {
                return HttpNotFound();
            }
            return View(configuracionSistema);
        }

        // POST: /Soporte/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)

        {

            configuracionSistema configuracionSistema = Contexto.configuracionSistema.Find(id);

            Contexto.configuracionSistema.Remove(configuracionSistema);

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
