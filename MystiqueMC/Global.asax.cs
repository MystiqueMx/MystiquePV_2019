using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MystiqueMC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
        }
        //protected void Application_EndRequest()
        //{
        //    var context = new HttpContextWrapper(Context);
        //    if (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
        //    {
        //        Context.Response.Clear();
        //        Context.Response.StatusCode = 400;
        //    }
        //}
    }
}
