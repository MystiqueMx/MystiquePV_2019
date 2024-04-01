using MystiqueMC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [Authorize]
    [ValidatePermissions]
    public class ReportesMedController : BaseController
    {
        public ActionResult Index()
        {
            try
            {

                return View();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "ReportesMed");
            }
        }
        public ActionResult ReporteCitas(string NombreDoc, DateTime? fecha1, DateTime? fecha2, string estatus)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            ViewBag.Nombre = NombreDoc;
            var IdEstatus = Contexto.CatEstatusCita.Where(w => w.descripcion.Equals(estatus)).Select(c => c.idCatEstatusCita).FirstOrDefault();
            if (fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var ReporteCompras = Contexto.SP_Reporte_Citas(NombreDoc, fecha1, fecha2, IdEstatus, empresaId).OrderByDescending(o => o.FechaCita).ToList();

            ViewData["Estatus"] = new SelectList(ReporteCompras.Select(s => s.Estatus).Distinct());

            return View(ReporteCompras);
        }

        public ActionResult ReporteBeneficios(string NombreDoc, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            ViewBag.Nombre = NombreDoc;
            if (fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var ReporteCompras = Contexto.SP_Reporte_Beneficios(NombreDoc, fecha1, fecha2, empresaId).OrderByDescending(o => o.FechaCierre).ToList();
            
            return View(ReporteCompras);
        }

        
    }
}