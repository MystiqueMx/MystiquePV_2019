using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacturacionTDCAPI.Controllers
{
    public class FacturaViewController : Controller
    {
        // GET: FacturaView
        public ActionResult Index()
        {
            return View();
        }
    }
}