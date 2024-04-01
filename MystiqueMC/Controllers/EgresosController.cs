using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [Authorize]
    public class EgresosController : Controller
    {
        #region GET

        // GET: Menu
        public ActionResult Menu()
        {
            return View();
        }

        // GET: Egresos
        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}