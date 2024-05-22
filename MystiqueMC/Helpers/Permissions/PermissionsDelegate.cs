// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.Permissions.PermissionsDelegate
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MystiqueMC.Helpers.Permissions
{
  public class PermissionsDelegate
  {
    private readonly string _superuser;
    private const string _controllerAutentificacion = "Autentificacion";
    private readonly List<VW_Permisos> _permisos;

    public PermissionsDelegate(List<VW_Permisos> permisos)
    {
      this._superuser = "WebMaster";
      this._permisos = permisos;
    }

    public PermissionsDelegate(string superuser, List<VW_Permisos> permisos)
    {
      this._superuser = superuser;
      this._permisos = permisos;
    }

    public bool HasPermissionForController(string role, string controller)
    {
      if (controller == "Autentificacion")
        return true;
      if (role == null)
        return false;
      return this._permisos.Any<VW_Permisos>((Func<VW_Permisos, bool>) (c => c.controlador.Equals(controller))) || this._superuser.Equals(role);
    }

    public bool HasPermissionForAction(string role, string controller, string action)
    {
      if (controller == "Autentificacion")
        return true;
      if (role == null)
        return false;
      return this._permisos.Any<VW_Permisos>((Func<VW_Permisos, bool>) (c => c.controlador.Equals(controller) && c.accion.Equals(action))) || this._superuser.Equals(role);
    }

    public bool HasPermission(string role, string controller, string action)
    {
      return controller == "Autentificacion" || role != null && this._permisos.Exists((Predicate<VW_Permisos>) (c => c.controlador.Equals(controller) && c.accion.Equals(action))) || this._superuser.Equals(role);
    }
  }
}
