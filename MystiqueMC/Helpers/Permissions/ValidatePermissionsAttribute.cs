// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.ValidatePermissionsAttribute
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.DAL;
using MystiqueMC.Helpers.Permissions;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;


namespace MystiqueMC.Helpers
{
  public class ValidatePermissionsAttribute : ActionFilterAttribute
  {
    private readonly string _superuser;
    private readonly bool isAction;

    private ActionResult RedirectAction
    {
      get
      {
        return (ActionResult) new RedirectToRouteResult(new RouteValueDictionary()
        {
          {
            "Controller",
            (object) "Autentificacion"
          },
          {
            "Action",
            (object) "Login"
          }
        });
      }
    }

    public ValidatePermissionsAttribute(bool isAction = false, string Superuser = "")
    {
      this.isAction = isAction;
      if (string.IsNullOrEmpty(Superuser))
        this._superuser = "WebMaster";
      else
        this._superuser = Superuser;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      try
      {
        if (filterContext.HttpContext.Request.IsAjaxRequest())
          return;
        string role = filterContext.HttpContext.Session.ObtenerRol();
        if (string.IsNullOrEmpty(role))
          filterContext.Result = this.RedirectAction;
        List<VW_Permisos> permisos = filterContext.HttpContext.Session.ObtenerPermisos();
        PermissionsDelegate permissionsDelegate = new PermissionsDelegate(this._superuser, permisos);
        string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        string actionName = filterContext.ActionDescriptor.ActionName;
        if (this.isAction)
        {
          if (!(role != this._superuser) || permissionsDelegate.HasPermissionForAction(role, controllerName, actionName))
            return;																													  				 
          filterContext.Result = this.RedirectAction;	 
        }
        else
        {
          if (permisos.Count <= 0 || !(role != this._superuser) || permissionsDelegate.HasPermissionForController(role, controllerName))
            return;
          filterContext.Result = this.RedirectAction;
        }
      }
      catch (Exception)
            {
        filterContext.Result = this.RedirectAction;
      }
    }
  }
}
