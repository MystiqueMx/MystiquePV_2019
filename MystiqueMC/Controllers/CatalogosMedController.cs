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
    public class CatalogosMedController : Controller
    {
        // GET: CatalogosMed
        public ActionResult Index()
        {
            return View();
        }
    }
}