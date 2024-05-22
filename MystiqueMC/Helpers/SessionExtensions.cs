// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.SessionExtensions
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.DAL;
using System.Collections.Generic;
using System.Web;


namespace MystiqueMC.Helpers
{
  public static class SessionExtensions
  {
    private const string APP_USER = "APPLICATION_USER";
    private const string USER_ROLE = "APPLICATION_USER_ROLE";
    private const string USER_PERMISSIONS = "APPLICATION_USER_PERMISSIONS";
    private const string UserComercios = "MystiqueMC.Helpers.UserComercios";
    private const string UserEmpresas = "MystiqueMC.Helpers.UserEmpresas";
    private const string UserSucursales = "MystiqueMC.Helpers.UserSucursales";
    private const string Inventario_Sucursal = "MystiqueMC.Helpers.Inventario_Sucursal";

    public static void GuardarUsuario(this HttpSessionStateBase httpSession, usuarios usuario)
    {
      httpSession.Add("APPLICATION_USER", (object) usuario);
    }

    public static usuarios ObtenerUsuario(this HttpSessionStateBase httpSession)
    {
      return httpSession["APPLICATION_USER"] as usuarios;
    }

    public static void GuardarRol(this HttpSessionStateBase httpSession, string rol)
    {
      httpSession.Add("APPLICATION_USER_ROLE", (object) rol);
    }

    public static string ObtenerRol(this HttpSessionStateBase httpSession)
    {
      return httpSession["APPLICATION_USER_ROLE"] as string;
    }

    public static void GuardarPermisos(
      this HttpSessionStateBase httpSession,
      List<VW_Permisos> permisos)
    {
      httpSession.Add("APPLICATION_USER_PERMISSIONS", (object) permisos);
    }

    public static List<VW_Permisos> ObtenerPermisos(this HttpSessionStateBase httpSession)
    {
      return httpSession["APPLICATION_USER_PERMISSIONS"] as List<VW_Permisos>;
    }

    public static bool TieneSesionActiva(this HttpSessionStateBase httpSession)
    {
      return httpSession.ObtenerUsuario() != null;
    }

    public static int EmpresaUsuarioLogueado(this HttpSessionStateBase httpSession)
    {
      return httpSession.ObtenerUsuario() == null ? 0 : httpSession.ObtenerUsuario().empresaId;
    }

    public static int IdUsuarioLogueado(this HttpSessionStateBase httpSession)
    {
      return httpSession.ObtenerUsuario() == null ? 0 : httpSession.ObtenerUsuario().idUsuario;
    }

    public static int[] ObtenerComercios(this HttpSessionStateBase httpSession)
    {
      return httpSession["MystiqueMC.Helpers.UserComercios"] is int[] numArray ? numArray : new int[0];
    }

    public static int[] ObtenerEmpresas(this HttpSessionStateBase httpSession)
    {
      return httpSession["MystiqueMC.Helpers.UserEmpresas"] is int[] numArray ? numArray : new int[0];
    }

    public static int[] ObtenerSucursales(this HttpSessionStateBase httpSession)
    {
      return httpSession["MystiqueMC.Helpers.UserSucursales"] is int[] numArray ? numArray : new int[0];
    }

    public static int? ObtenerInventarioSucursal(this HttpSessionStateBase httpSession)
    {
      return httpSession["MystiqueMC.Helpers.Inventario_Sucursal"] as int?;
    }

    public static void GuardarInventarioSucursal(
      this HttpSessionStateBase httpSession,
      int? sucursalId)
    {
      httpSession.Add("MystiqueMC.Helpers.Inventario_Sucursal", (object) sucursalId);
    }
  }
}
