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
using System.Text;

namespace MystiqueMC.Controllers
{

	[Authorize]
    [ValidatePermissionsAttribute(false)] 
    public class facturaClientesController : BaseController
    {
	 //  readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  //      // GET: /facturaClientes/
		//[ValidatePermissionsAttribute(true)] 
  //      public ActionResult Index(string SearchRFC, string SearchFolio, string SearchSucursal, DateTime? Inicio, DateTime? Fin)
  //      {
  //          //var facturacliente = Contexto.facturaCliente.Include(f => f.catEstatusFactura).Include(f => f.datosFiscales).Include(f => f.receptorCliente).Include(f => f.ticketSucursal);
  //          //ViewBag.Select = new SelectList(Contexto.sucursales, "idSucursal", "nombre").Distinct();
  //          ////ViewData["Select"] = new SelectList(Contexto.sucursales, "idSucursal", "nombre");

  //          //StringBuilder sa = new StringBuilder();
  //          //foreach (var type in facturacliente)
  //          //{
  //          //    sa.Append("<option >" + type.ticketSucursal.sucursales.nombre + "</option>");
  //          //}
  //          //ViewBag.Select = sa.ToString();

  //          //if (!string.IsNullOrEmpty(SearchRFC))
  //          //{
  //          //    facturacliente = facturacliente.Where(w => w.receptorCliente.datosReceptor.rfc.ToUpper().Contains(SearchRFC.ToUpper()));
  //          //}

  //          //if (!string.IsNullOrEmpty(SearchFolio))
  //          //{
  //          //    facturacliente = facturacliente.Where(w => w.ticketSucursal.folio.ToUpper().Contains(SearchFolio.ToUpper()));
  //          //}

  //          //if (!string.IsNullOrEmpty(SearchSucursal))
  //          //{
  //          //    facturacliente = facturacliente.Where(w => w.ticketSucursal.sucursales.nombre.ToUpper().Contains(SearchSucursal.ToUpper()));
  //          //}

  //          //if (Inicio.HasValue)
  //          //{
  //          //    facturacliente = facturacliente.Where(w => w.fechaRegistro > Inicio.Value);
  //          //}

  //          //if (Fin.HasValue)
  //          //{
  //          //    facturacliente = facturacliente.Where(w => w.fechaRegistro < Fin.Value);
  //          //}

  //          //facturacliente = facturacliente.OrderByDescending(c => c.fechaRegistro);

  //          return View(facturacliente.ToList());
  //      }
       
  //      protected override void Dispose(bool disposing)
  //      {
  //          if (disposing)
  //          {
  //              Contexto.Dispose();
  //          }
  //          base.Dispose(disposing);
  //      }
    }
}
