// Decompiled with JetBrains decompiler
// Type: MystiqueMC.RouteConfig
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using System.Web.Mvc;
using System.Web.Routing;


namespace MystiqueMC
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapRoute("Default", "{controller}/{action}/{id}", (object) new
      {
        controller = "Autentificacion",
        action = "Login",
        id = UrlParameter.Optional
      });
    }
  }
}
