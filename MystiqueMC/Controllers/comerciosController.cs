using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.FileUpload;
using MystiqueMC.Helpers.Permissions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [Authorize]
    [ValidatePermissions]
    public class comerciosController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string rolMaster = RolesConfiguration.Superuser;
        private string ServerPath => Server.MapPath(@"~");
        private const string IMAGE_EXTENSION = ".png";
        private readonly string HOSTNAME_IMAGENES = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");

        // GET: /comercios/Index
        public async Task<ActionResult> Index()
        {
            ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
            List<comercios> Comercios = ComerciosFirmados.Where(w => w.estatus).ToList();
            return View(Comercios);
        }

        // GET: /comercios/IndexEmpresa
        public ActionResult IndexEmpresa(int? idEmpresa, string Nombre, int? Giro, string SearchOrden = "true")
        {
            ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;

            if (idEmpresa == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var Comercios = Contexto.comercios.Where(w => w.empresaId == idEmpresa);

            if (!string.IsNullOrEmpty(Giro.ToString()))
            {
                Comercios = Comercios.Where(w => w.catComercioGiroId == Giro);
            }

            if (!string.IsNullOrEmpty(Nombre))
            {
                Comercios = Comercios.Where(w => w.nombreComercial.ToUpper().Contains(Nombre));
            }

            if (!string.IsNullOrEmpty(SearchOrden))
            {
                ViewBag.SearchOrden = SearchOrden;
                Comercios = Comercios.Where(c => c.estatus.ToString() == SearchOrden);
            }

            ViewBag.idEmpresa = idEmpresa;
            ViewBag.nombreEmpresa = Session.ObtenerUsuario().empresas.nombre;
            var giroComercio = Contexto.catComercioGiros.Where(c => c.cctivo);
            ViewBag.Giro = new SelectList(giroComercio, "idCatComercioGiro", "descripcion");
            return View(Comercios.ToList());
        }

        public async Task<ActionResult> UploadProductoImage()
        {
            if (Request.Files.Count == 0) return Json(new { success = false, message = "No se encontro la imagen" });
            if (Request.Files[0].ContentLength == 0) return Json(new { success = false, message = "No se encontro la imagen" });
            if (!FilesHelper.IsPNG(Request.Files[0])) return Json(new { success = false, message = "La imagen debe tener formato PNG" });
            try
            {
                FilesUploadDelegate @delegate = new FilesUploadDelegate();
                var FileName = await @delegate.UploadFileAsync(Request.Files[0], ServerPath, FilesHelper.FilesPath, IMAGE_EXTENSION);
                if (string.IsNullOrEmpty(FileName)) return Json(new { success = false, message = "Ocurrió un error al cargar la imagen" });
                return Json(new { success = true, fileUrl = FileName });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(new { success = false, message = "Ocurrió un error al cargar la imagen" });
            }
        }
        // GET: /comercios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            comercios comercios = await Contexto.comercios.FindAsync(id);

            if (comercios == null)
            {
                return HttpNotFound();
            }
            return View(comercios);
        }

        // GET: /comercios/Create
        public ActionResult Create(int? id)
        {
            ViewBag.idEmpresa = id;
            ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
            var giroComercio = Contexto.catComercioGiros.Where(c => c.cctivo);
            ViewBag.catComercioGiroId = new SelectList(giroComercio, "idCatComercioGiro", "descripcion");
            ViewBag.catTipoComercioId = new SelectList(Contexto.catTipoComercios, "idCatTipoComercio", "nombre");
            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "nombre");
            ViewBag.usuarioRegistroId = new SelectList(Contexto.usuarios, "idUsuario", "nombre");

            return View();
        }

        // POST: /comercios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idComercio,empresaId,nombreComercial,catComercioGiroId,rfc,correo,telefono,paginaWeb,logoUrl,estatus,usuarioRegistroId,fechaRegistro,direccion,catTipoComercioId,puntoVenta")] comercios comercios,
         string rfc, int IndexEmpresa, bool? puntoVenta)
        {
            if (ModelState.IsValid)
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    try
                    {
                        if (comercios.telefono != null)
                        {
                            comercios.telefono = Regex.Replace(comercios.telefono, @"[^0-9\.]", "");
                        }

                        comercios.usuarioRegistroId = Session.ObtenerUsuario().idUsuario;
                        comercios.empresaId = Session.ObtenerUsuario().empresaId;
                        comercios.fechaRegistro = DateTime.Now;
                        comercios.puntoVenta = puntoVenta ?? false;
                        comercios.rfc = rfc;
                        comercios.estatus = true;

                        Contexto.comercios.Add(comercios);

                        await Contexto.SaveChangesAsync();
                        tx.Commit();
                        if (IndexEmpresa == 1)
                        {
                            return RedirectToAction("IndexEmpresa", "comercios", new { idEmpresa = usuarioFirmado.empresas.idEmpresa });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException)
                    { tx.Rollback(); }
                }
            }

            ViewBag.catComercioGiroId = new SelectList(Contexto.catComercioGiros, "idCatComercioGiro", "descripcion", comercios.catComercioGiroId);
            ViewBag.catTipoComercioId = new SelectList(Contexto.catTipoComercios, "idCatTipoComercio", "nombre", comercios.catTipoComercioId);
            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "nombre", comercios.empresaId);
            ViewBag.usuarioRegistroId = new SelectList(Contexto.usuarios, "idUsuario", "nombre", comercios.usuarioRegistroId);

            return View(comercios);
        }

        // GET: /comercios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            comercios comercios = await Contexto.comercios.FindAsync(id);

            if (comercios == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "ACTIVO", Value = "1" });
            list.Add(new SelectListItem() { Text = "INACTIVO", Value = "0" });

            int Value;
            bool SelectedValue = Contexto.comercios.Where(w => w.idComercio == id).Select(s => s.estatus).FirstOrDefault();
            if (SelectedValue) Value = 1;
            else Value = 0;

            ViewData["estatus"] = new SelectList(list, "Value", "Text", Value);
            var giroComercio = Contexto.catComercioGiros.Where(c => c.cctivo);
            ViewBag.catComercioGiroId = new SelectList(giroComercio, "idCatComercioGiro", "descripcion", comercios.catComercioGiroId);
            ViewBag.catTipoComercioId = new SelectList(Contexto.catTipoComercios, "idCatTipoComercio", "nombre", comercios.catTipoComercioId);
            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "nombre", comercios.empresaId);
            ViewBag.usuarioRegistroId = new SelectList(Contexto.usuarios, "idUsuario", "nombre", comercios.usuarioRegistroId);

            return View(comercios);
        }

        // POST: /comercios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idComercio,empresaId,nombreComercial,catComercioGiroId,rfc,correo,telefono,paginaWeb,logoUrl,direccion,catTipoComercioId")] comercios comercios,
            string estatus, int IndexEmpresa, bool? puntoVenta)
        {
            if (ModelState.IsValid)
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    try
                    {
                        if (comercios.telefono != null)
                        {
                            comercios.telefono = Regex.Replace(comercios.telefono, @"[^0-9\.]", "");
                        }

                        var usuarioFirmado = Session.ObtenerUsuario();

                        if (!string.IsNullOrEmpty(estatus))
                        {
                            if (estatus == "1") comercios.estatus = true;
                            else comercios.estatus = false;
                        }
                        var comercioAntiguo = Contexto.comercios.Find(comercios.idComercio);
                        comercioAntiguo.rfc = comercios.rfc;
                        comercioAntiguo.correo = comercios.correo;
                        comercioAntiguo.direccion = comercios.direccion;
                        comercioAntiguo.estatus = comercios.estatus;
                        comercioAntiguo.logoUrl = comercios.logoUrl;
                        comercioAntiguo.nombreComercial = comercios.nombreComercial;
                        comercioAntiguo.paginaWeb = comercios.paginaWeb;
                        comercioAntiguo.telefono = comercios.telefono;
                        comercioAntiguo.catComercioGiroId = comercios.catComercioGiroId;
                        comercioAntiguo.puntoVenta = puntoVenta ?? false;

                        Contexto.Entry(comercioAntiguo).State = EntityState.Modified;

                        await Contexto.SaveChangesAsync();
                        tx.Commit();
                        if (IndexEmpresa == 1)
                        {
                            return RedirectToAction("IndexEmpresa", "comercios", new { idEmpresa = comercios.empresaId });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        ShowAlertException("");
                        tx.Rollback();
                    }
                }
            }

            ViewBag.estatus = comercios.estatus;
            ViewBag.catComercioGiroId = new SelectList(Contexto.catComercioGiros, "idCatComercioGiro", "descripcion", comercios.catComercioGiroId);
            ViewBag.catTipoComercioId = new SelectList(Contexto.catTipoComercios, "idCatTipoComercio", "nombre", comercios.catTipoComercioId);
            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "nombre", comercios.empresaId);
            ViewBag.usuarioRegistroId = new SelectList(Contexto.usuarios, "idUsuario", "nombre", comercios.usuarioRegistroId);

            return View(comercios);
        }

        // GET: /comercios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            comercios comercios = await Contexto.comercios.FindAsync(id);

            if (comercios == null)
            {
                return HttpNotFound();
            }
            return View(comercios);
        }

        // POST: /comercios/Delete/5
        //[ValidatePermissionsAttribute(true)]
        [HttpPost]
        public async Task<ActionResult> Delete(int idComercio)
        {
            try
            {
                comercios comercios = await Contexto.comercios.FindAsync(idComercio);

                Contexto.comercios.Remove(comercios);
                await Contexto.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Redirect(Request.Path);
            }
        }

        public ActionResult Promociones(int idComercio, int? idEmpresa, int? SearchSucursal, int? SearchComercio, int? searchItem, string SearchOrden = "true")
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                if (!Request.IsAuthenticated || Usuario == null)
                    return new { success = false }.ToJsonResult();

                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
                string rol = Session.ObtenerRol();
                var usuarioFirmado = Session.ObtenerUsuario();

                var beneficios = Contexto.beneficios
                    .Include(b => b.confBeneficioSucursal)
                    .Include(b => b.beneficioHorarios)
                    .Include(b => b.catTipoMembresias)
                    .Include(b => b.comercios)
                    .Include(b => b.usuarios)
                    .Include(b => b.comercios.sucursales)
                    .Where(w => w.comercios.idComercio == idComercio)
                    .AsQueryable();

                ViewBag.Idcomercio = idComercio;
                ViewBag.idEmpresa = usuarioFirmado.empresas.idEmpresa;

                ViewBag.nombreComercio = Contexto.comercios.Where(c => c.idComercio == idComercio).Select(c => c.nombreComercial).First();

                if (SearchComercio.HasValue)
                {
                    beneficios = beneficios.Where(c => c.comercioId == SearchComercio.Value);
                    var sucursal = from u in Contexto.sucursales.Where(c => c.comercioId == SearchComercio)
                                   select u;
                    //llenar grid

                    StringBuilder sc = new StringBuilder();

                    foreach (var type in sucursal)
                    {
                        sc.Append("<option value='" + type.idSucursal + "'>" + type.nombre + "</option>");
                    }

                    ViewBag.sucursal = sc.ToString();
                    ViewBag.Idcomercio = SearchComercio;

                    if (!string.IsNullOrEmpty(SearchOrden))
                    {
                        beneficios = beneficios.Where(c => c.estatus.ToString() == SearchOrden);
                    }

                    if (SearchSucursal.HasValue)
                    {
                        beneficios = beneficios.Where(stu => stu.confBeneficioSucursal.Any(c => c.sucursalId == SearchSucursal));
                    }

                }
                if (searchItem.HasValue)
                {
                    beneficios = beneficios.Where(stu => stu.idBeneficio == searchItem);

                    var comercio1 = beneficios.Where(c => c.idBeneficio == searchItem).Select(c => c.comercioId).FirstOrDefault();
                    var sucursal = from u in Contexto.sucursales.Where(c => c.comercioId == comercio1)
                                   select u;
                    //llenar grid

                    StringBuilder sc = new StringBuilder();

                    foreach (var type in sucursal)
                    {
                        sc.Append("<option value='" + type.idSucursal + "'>" + type.nombre + "</option>");
                    }

                    ViewBag.sucursal = sc.ToString();


                    if (SearchSucursal.HasValue)
                    {
                        beneficios = beneficios.Where(stu => stu.confBeneficioSucursal.Any(c => c.sucursalId == SearchSucursal));
                    }
                }
                if (!string.IsNullOrEmpty(SearchOrden))
                {
                    ViewBag.SearchOrden = SearchOrden;
                    beneficios = beneficios.Where(c => c.estatus.ToString() == SearchOrden);
                }
                //fin
                return View(beneficios.ToList());
            }
            catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new { exception = e });
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
