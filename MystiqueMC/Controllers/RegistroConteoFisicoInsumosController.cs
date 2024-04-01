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

namespace MystiqueMC.Controllers
{
    public class RegistroConteoFisicoInsumosController : BaseController
    {
	   readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GET
        public ActionResult Index(int? id, string nombre)
        {
            var conteo = Contexto.RegistroConteoFisicoInsumos
                .Include(r => r.ConteoFisicoAgrupadorInsumos)
                .Include(r => r.ConteoFisicoInsumos)
                .Include(r => r.ConteoFisicoTipoRegistro)
                .Where(r => r.conteoFisicoAgrupadorInsumosId == id && DbFunctions.TruncateTime(r.fechaRegistro) == DbFunctions.TruncateTime(DateTime.Now))
                .ToList();

            ViewBag.nombre = nombre;
            return View(conteo);
        }
        #endregion

        #region POST
        public ActionResult RegistroConteoFisicoInsumos(int[] idRegistroConteoFisicoLlegar, int[] idRegistroConteoFisicoMerma, int[] idRegistroConteoFisicoEntrada, int[] idRegistroConteoFisicoCerrar, string[] CantidadAllegar, string[] CantidadMerma, string[] CantidadEntrada, string[] CantidadAlCerrar)
        {
            try
            {
                int idLlegar = 0;
                string cantidadLlegar = "0";
                foreach (var item in idRegistroConteoFisicoLlegar.Zip(CantidadAllegar, (x, y) => (Num: x, Text: y)))
                {
                    idLlegar = item.Num;
                    cantidadLlegar = item.Text;
                    var conteoLlegar = Contexto.RegistroConteoFisicoInsumos.Find(idLlegar);
                    if (cantidadLlegar != "") conteoLlegar.cantidad = Convert.ToDecimal(cantidadLlegar); else conteoLlegar.cantidad = 0;
                    conteoLlegar.usuarioActualizaId = UsuarioActual.idUsuario;
                    conteoLlegar.ultimaActualizacion = DateTime.Now;
                }

                int idMerma = 0;
                string cantidadMerma = "0";
                foreach (var item in idRegistroConteoFisicoMerma.Zip(CantidadMerma, (x, y) => (Num: x, Text: y)))
                {
                    idMerma = item.Num;
                    cantidadMerma = item.Text;
                    var conteoMerma = Contexto.RegistroConteoFisicoInsumos.Find(idMerma);
                    if (cantidadMerma != "") conteoMerma.cantidad = Convert.ToDecimal(cantidadMerma); else conteoMerma.cantidad = 0;
                    conteoMerma.usuarioActualizaId = UsuarioActual.idUsuario;
                    conteoMerma.ultimaActualizacion = DateTime.Now;
                }

                int idEntrada = 0;
                string cantidadEntrada = "0";
                foreach (var item in idRegistroConteoFisicoEntrada.Zip(CantidadEntrada, (x, y) => (Num: x, Text: y)))
                {
                    idEntrada = item.Num;
                    cantidadEntrada = item.Text;
                    var conteoEntrada = Contexto.RegistroConteoFisicoInsumos.Find(idEntrada);
                    if (cantidadEntrada != "") conteoEntrada.cantidad = Convert.ToDecimal(cantidadEntrada); else conteoEntrada.cantidad = 0;
                    conteoEntrada.usuarioActualizaId = UsuarioActual.idUsuario;
                    conteoEntrada.ultimaActualizacion = DateTime.Now;
                }

                int idCerrar = 0;
                string cantidadCerrar = "0";
                foreach (var item in idRegistroConteoFisicoCerrar.Zip(CantidadAlCerrar, (x, y) => (Num: x, Text: y)))
                {
                    idCerrar = item.Num;
                    cantidadCerrar = item.Text;
                    var conteoCerrar = Contexto.RegistroConteoFisicoInsumos.Find(idCerrar);
                    if (cantidadCerrar != "") conteoCerrar.cantidad = Convert.ToDecimal(cantidadCerrar); else conteoCerrar.cantidad = 0;
                    conteoCerrar.usuarioActualizaId = UsuarioActual.idUsuario;
                    conteoCerrar.ultimaActualizacion = DateTime.Now;
                }

                Contexto.SaveChanges();
                ShowAlertSuccess("La información se guardo correctamente.");
                return RedirectToAction("Index", "ConteoFisicoAgrupadorInsumos", null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException(ex);
            }

            return RedirectToAction("Index", "RegistroConteoFisicoInsumos", null);
        }
        #endregion

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
