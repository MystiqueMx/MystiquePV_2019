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
    public class FacturacionComercioController : BaseController
    {
        public ActionResult Reenviar()
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                ViewBag.DatosFiscales = Contexto.datosFiscales.Include("Comercios")
                    
                    .Where(c => c.comercios.empresaId == usuarioFirmado.empresaId)
                    .OrderByDescending(d => d.fechaRegistro)
                    .Select(c=>c.rfc)
                    .Distinct()
                    .Select(c => new SelectListItem
                    {
                        Text = c,
                        Value = c,
                        Selected = false
                    }).ToArray();
                return View();
            }
            catch (Exception e)
            {
                ShowAlertException(e);
                return RedirectToAction("Index", "Home");
            }

        }
    }
}