using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    public class ConteoFisicoInsumosController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            var conteofisicoinsumos = Contexto.ConteoFisicoInsumos.Include(c => c.ConteoFisicoAgrupadorInsumos).Include(c => c.Insumos);
            return View(conteofisicoinsumos.ToList());
        }

        public ActionResult Create(int? id)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                ViewBag.IdAgrupador = id;

                ViewBag.conteoFisicoAgrupadorInsumosId = new SelectList(Contexto.ConteoFisicoAgrupadorInsumos, "idConteoFisicoAgrupadorInsumos", "descripcion", id);
                ViewBag.insumoId = new SelectList(Contexto.Insumos.Where(w => w.comercioId == comercioId), "idInsumo", "nombre");
                ViewBag.categoriaInsumoId = new SelectList(Contexto.CategoriaInsumo.Where(w => w.comercioId == comercioId), "idCategoriaInsumo", "descripcion");
                ViewBag.NombreAgrupador = Contexto.ConteoFisicoAgrupadorInsumos.Where(w => w.idConteoFisicoAgrupadorInsumos == id)?.FirstOrDefault().descripcion;
                return View();
            }
            catch (System.Exception ex)
            {
                ShowAlertException(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idConteoFisicoInsumo,descripcion,insumoId,activo,conteoFisicoAgrupadorInsumosId")] ConteoFisicoInsumos conteoFisicoInsumos, int[] indumosIds, string existentes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var id in indumosIds)
                    {
                        var insumo = Contexto.Insumos.Where(w => w.idInsumo == id)?.FirstOrDefault();
                        var insumoExistente = Contexto.ConteoFisicoInsumos.Any(c => c.insumoId == id && c.conteoFisicoAgrupadorInsumosId == conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId);
                   
                        if (!insumoExistente)
                        {
                            if (insumo != null)
                            {
                                var conteoinsumo = new ConteoFisicoInsumos()
                                {
                                    descripcion = insumo.nombre,
                                    insumoId = insumo.idInsumo,
                                    conteoFisicoAgrupadorInsumosId = conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId,
                                    activo = true,
                                };
                                Contexto.ConteoFisicoInsumos.Add(conteoinsumo);
                                Contexto.SaveChanges();
                            }
                        }
                        else
                        {
                            existentes += insumo.nombre.ToString() + ", ";
                        }
                    }

                    if (existentes != null)
                    {
                        string mensaje = existentes.Substring(0, existentes.Length - 1);
                        ShowAlertInformation("Ya existen registros de los insumos: " + mensaje);
                    }

                    return RedirectToAction("Details", "ConteoFisicoAgrupadorInsumos", new { id = conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId });
                }

                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                ViewBag.conteoFisicoAgrupadorInsumosId = new SelectList(Contexto.ConteoFisicoAgrupadorInsumos, "idConteoFisicoAgrupadorInsumos", "descripcion", conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId);
                ViewBag.insumoId = new SelectList(Contexto.Insumos.Where(w => w.comercioId == comercioId), "idInsumo", "nombre", conteoFisicoInsumos.insumoId);

                return RedirectToAction("Details", "ConteoFisicoAgrupadorInsumos", new { id = conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId });
            }
            catch (System.Exception ex)
            {
                ShowAlertException(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }

        // GET: /ConteoFisicoInsumos/Edit/5
        public ActionResult Edit(int? id, int? idAgrupador)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ConteoFisicoInsumos conteoFisicoInsumos = Contexto.ConteoFisicoInsumos.Find(id);

            if (conteoFisicoInsumos == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdAgrupador = idAgrupador;
            ViewBag.conteoFisicoAgrupadorInsumosId = new SelectList(Contexto.ConteoFisicoAgrupadorInsumos, "idConteoFisicoAgrupadorInsumos", "descripcion", conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId);
            ViewBag.insumoId = new SelectList(Contexto.Insumos, "idInsumo", "nombre", conteoFisicoInsumos.insumoId);
            return View(conteoFisicoInsumos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idConteoFisicoInsumo,descripcion,insumoId,activo,conteoFisicoAgrupadorInsumosId")] ConteoFisicoInsumos conteoFisicoInsumos, string existente)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var insumoExistente = Contexto.ConteoFisicoInsumos.Any(c => c.insumoId == conteoFisicoInsumos.insumoId && c.conteoFisicoAgrupadorInsumosId == conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId);

                existente = conteoFisicoInsumos.descripcion;

                if (!insumoExistente)
                {
                    if (ModelState.IsValid)
                    {
                        conteoFisicoInsumos.activo = true;
                        Contexto.Entry(conteoFisicoInsumos).State = EntityState.Modified;
                        Contexto.SaveChanges();
                        return RedirectToAction("Details", "ConteoFisicoAgrupadorInsumos", new { id = conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId });
                    }
                }
                else
                {
                    ShowAlertInformation("Ya existe un registro del insumo: " + existente);
                }

                ViewBag.conteoFisicoAgrupadorInsumosId = new SelectList(Contexto.ConteoFisicoAgrupadorInsumos, "idConteoFisicoAgrupadorInsumos", "descripcion", conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId);
                ViewBag.insumoId = new SelectList(Contexto.Insumos.Where(w => w.comercioId == comercioId), "idInsumo", "nombre", conteoFisicoInsumos.insumoId);

                return RedirectToAction("Details", "ConteoFisicoAgrupadorInsumos", new { id = conteoFisicoInsumos.conteoFisicoAgrupadorInsumosId });
            }
            catch (System.Exception ex)
            {
                ShowAlertException(ex);
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos");
            }
        }
        public ActionResult ObtenerInsumos(int id, int idAgrupador)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var productosAux = Contexto.Insumos.Where(c => c.comercioId == comercioId && c.categoriaInsumoId == id && c.activo).ToList();

                List<Productos> productosList = new List<Productos>();

                foreach (var productos in productosAux)
                {
                    Productos producto = new Productos();
                    producto.nombre = productos.nombre;
                    producto.idProducto = productos.idInsumo;
                    productosList.Add(producto);
                }

                return Json(new { exito = true, data = productosList, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return HttpNotFound();
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
