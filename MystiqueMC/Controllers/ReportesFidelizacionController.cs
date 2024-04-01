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
using MystiqueMC.Models;
using Newtonsoft.Json;
using Humanizer;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using MystiqueMC.Helpers.Permissions;

namespace MystiqueMC.Controllers
{
    [Authorize]
    [ValidatePermissions]
    public class ReportesFidelizacionController : BaseController
    {

        #region ReportesL        

        public ActionResult Index()
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                var Rol = Session.ObtenerRol();
                if (!Request.IsAuthenticated || Usuario == null)
                    return RedirectToAction("Login", "Account", null);
                return PartialView();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "ReportesFidelizacion");
            }
        }

        #region Reportes

        public ActionResult ReporteClientes(string Sucursales, string SearchNombre, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;


            //REGRESA A LA VISTA TODOS LOS REGISTROS
            var ReporteClientes = Contexto.SP_Reporte_Clientes(null, null, empresaId).AsQueryable();

            ReporteClientes = ReporteClientes.OrderByDescending(c => c.FechaRegistro);

            //BUSCA LOS REGISTROS EN UN RANGO DE FECHA DETERMINADO

            if (fecha1.HasValue)
            {
                ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                ReporteClientes = ReporteClientes.Where(w => w.FechaRegistro > fecha1.Value);
            }

            if (fecha2.HasValue)
            {
                ViewBag.fechaFin = fecha2.Value.ToShortDateString();
                ReporteClientes = ReporteClientes.Where(w => w.FechaRegistro < fecha2.Value);
            }
            if (!string.IsNullOrEmpty(SearchNombre))
            {
                ReporteClientes = ReporteClientes.Where(w => w.Nombre.ToUpper().Contains(SearchNombre.ToUpper())
                || w.Paterno.ToUpper().Contains(SearchNombre.ToUpper())
                || w.Materno.ToUpper().Contains(SearchNombre.ToUpper())
                );
            }
            return View(ReporteClientes.ToList());
        }

        public ActionResult ReporteCanjeados(string Canjes, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            if(fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Canjes)).Select(c => c.idSucursal).FirstOrDefault();
            var ReporteCanjeados = Contexto.SP_Reporte_Canje_Productos(IdSucursal, fecha1, fecha2, empresaId).OrderByDescending(o => o.FechaCanje).ToList();
            ViewData["CanjesSelect"] = new SelectList(Contexto.SP_Reporte_Canje_Productos(0, null, null, empresaId).Select(s => s.Sucursal).Distinct());

            return View(ReporteCanjeados);
        }

        public ActionResult ReporteCompras(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            if (fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
            var ReporteCompras = Contexto.SP_Reporte_Registro_Compras(IdSucursal, fecha1, fecha2, empresaId).OrderByDescending(o => o.FechaCompra).ToList();
            ViewData["ComprasSelect"] = new SelectList(ReporteCompras.Select(s => s.Sucursal).Distinct());

            return View(ReporteCompras);
        }

        public ActionResult ReporteBeneficios(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            if (fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
            var ReporteBeneficios = Contexto.SP_Reporte_Canje_Beneficios(IdSucursal, fecha1, fecha2, empresaId).OrderByDescending(o => o.fechaCompra).ToList();
            ViewData["BeneficiosSelect"] = new SelectList(Contexto.SP_Reporte_Canje_Beneficios(0, null, null, empresaId).Select(s => s.Sucursal).Distinct());

            return View(ReporteBeneficios);
        }

        #endregion

        #endregion
    }
}