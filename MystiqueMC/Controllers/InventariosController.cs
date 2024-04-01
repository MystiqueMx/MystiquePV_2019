using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MystiqueMC.Helpers;
using MystiqueMC.Models;

namespace MystiqueMC.Controllers
{
    public class InventariosController : Helpers.BaseController
    {
        private Helpers.ControlInventario controlInventarioHelper = new ControlInventario();

        // GET: Inventarios
        public ActionResult Index(int? sucursaId)
        {
            var comercio = ComerciosFirmados.First().idComercio;
            var sucursales = Contexto.sucursales.Where(s => s.comercioId == comercio).ToList();
            ViewBag.sucursaId = new SelectList(sucursales, "idSucursal", "nombre", sucursaId ?? 0);
            var catInventario = Contexto.CatMovimientoInventarios.Where(c => c.comercioId == comercio && c.idCatMovimientoInventario == 5 || c.idCatMovimientoInventario == 9).ToList();
            ViewBag.CatMovimientoInventario = new SelectList(catInventario, "idCatMovimientoInventario", "descripcion");
            var inventarios = Contexto.Inventarios
                .Include(c => c.Insumos)
                .Include(c => c.Insumos.UnidadMedida)
                .Where(w => w.activo && !w.Insumos.ConteoFisicoInsumos.Any(a => a.insumoId == w.insumoId && a.activo));

            if (sucursaId != null)
            {
                inventarios = inventarios
                    .Where(w => w.sucursalId == sucursaId && w.activo);

                ViewBag.sucursal = sucursaId;
                ViewBag.sucursaId = new SelectList(sucursales, "idSucursal", "nombre", sucursaId);

                //return View(inventarios.ToList());
            }
            else
            {
                inventarios = inventarios
                .Where(w => w.sucursalId == 0 && w.activo);
            }

            Session.GuardarInventarioSucursal(sucursaId);

            return View(inventarios.ToList());
        }

        public ActionResult IndexSeleccionSucursal()
        {
            var comercio = ComerciosFirmados.First().idComercio;
            var sucursales = Contexto.sucursales.Where(s => s.comercioId == comercio).ToList();
            //ViewBag.sucursaId = new SelectList(sucursales, "idSucursal", "nombre");
            return View("SeleccionSucursal", sucursales);
        }

        public ActionResult Menu()
        {

            return View();
        }
        public ActionResult ActualizarValor(int id, decimal Valor, int campo)
        {
            try
            {
                var inventario = Contexto.Inventarios.Find(id);
                switch (campo)
                {
                    case 1:
                        inventario.maximo = Valor;
                        break;
                    case 2:
                        inventario.minimo = Valor;
                        break;
                    case 3:
                        inventario.precio = Valor;
                        break;
                    default:
                        break;
                }
                Contexto.Entry(inventario).State = EntityState.Modified;
                Contexto.SaveChanges();

                return Json(new { exito = true });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
                return Json(new { exito = false });
            }
        }

        [HttpPost]
        public ActionResult AjusteInventario(int insumoId, decimal cantidadNueva, int sucursal, decimal cantidadActual, string observaciones)
        {
            try
            {
                ControlInventariosResponse result;
                var diferencia = cantidadNueva - cantidadActual;

                var tiposMovimientosInventario = Contexto.sucursales.First(c=>c.idSucursal == sucursal).comercioId;
                
                if (diferencia != 0)
                {
                    result = controlInventarioHelper.registroMovimientoInventario(Contexto, null, diferencia, insumoId, sucursal, cantidadNueva, UsuarioActual, TiposMovimientosInventario.Ajuste, null, observaciones);
                }

                return RedirectToAction("Index", "Inventarios", new { sucursaId = sucursal });
            }
            catch (Exception e)
            {
                ShowAlertException(e);
                Logger.Error(e);
                return View("Index");
            }
        }
        /*
        public ActionResult registroMovimientoInventario(int tipoMovimiento, decimal cantidad, int insumoId, int sucursal, decimal cantidadNueva)
        {
            try
            {
                var movimiento = Contexto.MovimientoInventarios.Add(new DAL.MovimientoInventarios
                {
                    catMovimientoId = tipoMovimiento,
                    insumoId = insumoId,
                    cantidad = cantidad,
                    sucursalId = sucursal,
                    fechaRegistro = DateTime.Now,
                    usuarioRegistroId = UsuarioActual.idUsuario
                });

                var inventario = Contexto.Inventarios.Where(i => i.insumoId == insumoId).First();
                inventario.cantidad = cantidadNueva;
                Contexto.Entry(inventario).State = EntityState.Modified;

                Contexto.SaveChanges();
                return RedirectToAction("Index", "Inventarios", new { sucursaId = sucursal });
            }
            catch (Exception e)
            {
                ShowAlertException(e);
                Logger.Error(e);
                return View("Index");
            }
        }
        */
        public ActionResult MovimientoInventario(int? sucursaId, int? filtroFecha, int? movimientos, string nombre, int id, string nombreInsumo, string NombreSearch)
        {
            try
            {
                var movimientoInventario = Contexto.MovimientoInventarios.Include(c => c.CatMovimientoInventarios).Include(c => c.sucursales).Include(c => c.Insumos);
                var comercioId = ComerciosFirmados.First().idComercio;
                var sucursales = Contexto.sucursales.Where(s => s.comercioId == comercioId).ToList();

                string[] tipos = Enum.GetNames(typeof(TiposMovimientosInventario));
                var lista = tipos.Select((value, key) => new { value, key = (key + 1) }).ToList();

                ViewBag.movimientos = new SelectList(lista,"key","value");
                ViewBag.sucursaId = new SelectList(sucursales, "idSucursal", "nombre");
                ViewBag.sucursal = null;
                ViewBag.nombreSucursal = nombre;
                ViewBag.insumo = id;
                ViewBag.nombreInsumo = nombreInsumo;
                ViewBag.NombreSearch = NombreSearch;

                var filtroFechaInicio = DateTime.Now.AddDays(-filtroFecha ?? -7);
                var filtroFechaFin = DateTime.Now;

                ViewBag.fechaInicio = filtroFechaInicio.ToShortDateString();
                ViewBag.fechaFin = filtroFechaFin.ToShortDateString();


                if (sucursaId != null)
                {
                    movimientoInventario = movimientoInventario.Where(w => w.sucursalId == sucursaId && w.insumoId == id);
                    ViewBag.sucursal = sucursaId;

                    if (sucursaId != null && movimientos != null)
                    {
                        var filtroTipo = Enum.GetName(typeof(TiposMovimientosInventario), movimientos);
                        movimientoInventario = movimientoInventario.Where(w => w.tipoMovimiento == filtroTipo && w.sucursalId == sucursaId && w.insumoId == id);
                    }

                    movimientoInventario = movimientoInventario.Where(w => DbFunctions.TruncateTime(w.fechaRegistro) >= DbFunctions.TruncateTime(filtroFechaInicio));

                    movimientoInventario = movimientoInventario.Where(w => DbFunctions.TruncateTime(w.fechaRegistro) <= DbFunctions.TruncateTime(filtroFechaFin));

                    return View(movimientoInventario.ToArray());
                }
                else
                {
                    movimientoInventario = Contexto.MovimientoInventarios.Where(c => c.sucursalId == 0);
                }
                return View(movimientoInventario.ToArray());
            }
            catch (Exception e)
            {
                ShowAlertException(e);
                Logger.Error(e);
                return View("Index");
            }
        }
    }
}