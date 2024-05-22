using Microsoft.CSharp.RuntimeBinder;
using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [Authorize]
    [ValidatePermissions]
    public class ArqueosController : BaseController  																  
    {
        #region GET
        // GET: Arqueos
        public ActionResult Index(int? sucursaId, int? mes, int? año)
        {
            try
            {


                var sucursales = SucursalesFirmadas.ToList();
                ViewBag.sucursaId = new SelectList(sucursales, "idSucursal", "nombre", sucursaId);
                ViewBag.YearSelected = año ?? DateTime.Now.Year;
                ViewBag.mes = mes ?? DateTime.Now.Month;
                var arqueo = Contexto.Arqueo
                    .Include(a => a.sucursales)
                    .Include(a => a.usuarios)
                    .Include(a => a.Aperturas)
                    .Include(a => a.Ventas).AsEnumerable();



































                if (sucursaId != null)
                {
                    arqueo = arqueo
                        .Where(w => w.sucursalId == sucursaId)
                        .AsEnumerable();
                    
					
					
					
					if (mes != null)
                    {
                        arqueo = arqueo
                            .Where(w => w.fechaRegistroVenta.Month == mes)
                            .AsEnumerable();
                    }
                    
					
					if (año != null)
                    {
                        arqueo = arqueo
                            .Where(w => w.fechaRegistroVenta.Year == año)
                            .AsEnumerable();
                    }


                    return View(arqueo.OrderBy(o => o.fechaRegistroVenta));
                }
                else
                {
                    arqueo = arqueo
                    .Where(w => w.sucursalId == 0)
                    .AsEnumerable();
                }

                return View(arqueo.OrderBy(o => o.fechaRegistroVenta));
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Arqueos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var arqueo = Contexto.Arqueo.Find(id);
            if (arqueo == null)
            {
                return HttpNotFound();
            }
            return View(arqueo);
        }

        // GET: Arqueos/Create
        public ActionResult Create()
        {
            ViewBag.sucursalId = new SelectList(Contexto.sucursales, "idSucursal", "nombre");
            ViewBag.usuarioActualizo = new SelectList(Contexto.usuarios, "idUsuario", "nombre");
            ViewBag.aperturaId = new SelectList(Contexto.Aperturas, "idApertura", "uuidApertura");
            ViewBag.ventaId = new SelectList(Contexto.Ventas, "idVenta", "uuidVenta");
            return View();
        }


















































        // GET: Arqueos/Edit/5
        public ActionResult Edit(int? id)														 																		
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var arqueo = Contexto.Arqueo.Find(id);
            if (arqueo == null)
            {
                return HttpNotFound();
            }
            ViewBag.sucursalId = new SelectList(Contexto.sucursales, "idSucursal", "nombre", arqueo.sucursalId);
            ViewBag.usuarioActualizo = new SelectList(Contexto.usuarios, "idUsuario", "nombre", arqueo.usuarioActualizo);
            ViewBag.aperturaId = new SelectList(Contexto.Aperturas, "idApertura", "uuidApertura", arqueo.aperturaId);
            ViewBag.ventaId = new SelectList(Contexto.Ventas, "idVenta", "uuidVenta", arqueo.ventaId);
            return View(arqueo);
        }
















































        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var arqueo = Contexto.Arqueo.Find(id);
            if (arqueo == null)
            {
                return HttpNotFound();
            }
            return View(arqueo);
        }

        #endregion
		
		
        #region POST
        public ActionResult ActualizarArqueo(int id, decimal totalRecibido, string Observacion)																																																																
        {
            try
            {
                var arqueo = Contexto.Arqueo.Find(id);
                arqueo.totalRecibido = totalRecibido;
                arqueo.observacion = Observacion;
                arqueo.diferencia = arqueo.total - arqueo.totalRecibido;
                arqueo.fechaActualizacion = DateTime.Now;
                arqueo.usuarioActualizo = UsuarioActual.idUsuario;
                Contexto.Entry(arqueo).State = EntityState.Modified;
                Contexto.SaveChanges();

                return RedirectToAction("Index", new 
				{ sucursaId = arqueo.sucursalId,
				mes = arqueo.fechaRegistroVenta.Month,
				año = arqueo.fechaRegistroVenta.Year });
            }
			
			
			
			
			
			
			
			
			
			
			
			
			
			
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
                return RedirectToAction("Index");
            }
        }
		
        public ActionResult ActualizarObservacion(int id, string Observacion)
        {
            try
            {
                var arqueo = Contexto.Arqueo.Find(id);
                arqueo.observacion = Observacion;
                arqueo.fechaActualizacion = DateTime.Now;
                arqueo.usuarioActualizo = UsuarioActual.idUsuario;
                Contexto.Entry(arqueo).State = EntityState.Modified;
                Contexto.SaveChanges();

                return Json(new Ordenamiento { exito = true });
            }
			
			
			
			
			
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
                return Json(new Ordenamiento { exito = false });
            }																							  	
        }
        // POST: Arqueos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idArqueo,sucursalId,ventaId,aperturaId,concepto,saldoDlls,arqueoDlls,efectivo,cxc,tarjeta,gasto,total,totalRecibido,diferencia,observacion,fechaRegistro,fechaActualizacion,usuarioActualizo")] Arqueo arqueo)																																																																	
        {
            if (ModelState.IsValid)																		  
            {														   
                Contexto.Arqueo.Add(arqueo);
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.sucursalId = new SelectList(Contexto.sucursales, "idSucursal", "nombre", arqueo.sucursalId);
            ViewBag.usuarioActualizo = new SelectList(Contexto.usuarios, "idUsuario", "nombre", arqueo.usuarioActualizo);
            ViewBag.aperturaId = new SelectList(Contexto.Aperturas, "idApertura", "uuidApertura", arqueo.aperturaId);
            ViewBag.ventaId = new SelectList(Contexto.Ventas, "idVenta", "uuidVenta", arqueo.ventaId);
            return View(arqueo);																								   
        }












































        // POST: Arqueos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idArqueo,sucursalId,ventaId,aperturaId,concepto,saldoDlls,arqueoDlls,efectivo,cxc,tarjeta,gasto,total,totalRecibido,diferencia,observacion,fechaRegistro,fechaActualizacion,usuarioActualizo")] Arqueo arqueo)
        {
            if (ModelState.IsValid)
            {
                Contexto.Entry(arqueo).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.sucursalId = new SelectList(Contexto.sucursales, "idSucursal", "nombre", arqueo.sucursalId);
            ViewBag.usuarioActualizo = new SelectList(Contexto.usuarios, "idUsuario", "nombre", arqueo.usuarioActualizo);
            ViewBag.aperturaId = new SelectList(Contexto.Aperturas, "idApertura", "uuidApertura", arqueo.aperturaId);
            ViewBag.ventaId = new SelectList(Contexto.Ventas, "idVenta", "uuidVenta", arqueo.ventaId);
            return View(arqueo);													 
        }



















        // POST: Arqueos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)																																																																
        {
            var arqueo = Contexto.Arqueo.Find(id);
            Contexto.Arqueo.Remove(arqueo);										   
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
        #endregion
    }   
}