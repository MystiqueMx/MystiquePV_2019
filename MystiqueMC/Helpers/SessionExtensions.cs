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
            httpSession.Add(APP_USER, usuario);
        }
        public static usuarios ObtenerUsuario(this HttpSessionStateBase httpSession)
        {
            return httpSession[APP_USER] as usuarios;
        }
        public static void GuardarRol(this HttpSessionStateBase httpSession, string rol)
        {
            httpSession.Add(USER_ROLE, rol);
        }
        public static string ObtenerRol(this HttpSessionStateBase httpSession)
        {
            return httpSession[USER_ROLE] as string;
        }
        public static void GuardarPermisos(this HttpSessionStateBase httpSession, List<VW_Permisos> permisos)
        {
            httpSession.Add(USER_PERMISSIONS, permisos);
        }
        public static List<VW_Permisos> ObtenerPermisos(this HttpSessionStateBase httpSession)
        {
            return httpSession[USER_PERMISSIONS] as List<VW_Permisos>;
        }
        public static bool TieneSesionActiva(this HttpSessionStateBase httpSession)
        {
            return httpSession.ObtenerUsuario() != null;
        }
        public static int EmpresaUsuarioLogueado(this HttpSessionStateBase httpSession)
        {
            return httpSession.ObtenerUsuario() != null ? httpSession.ObtenerUsuario().empresaId : 0;
        }
        public static int IdUsuarioLogueado(this HttpSessionStateBase httpSession)
        {
            return httpSession.ObtenerUsuario() != null ? httpSession.ObtenerUsuario().idUsuario : 0;
        }
        public static int[] ObtenerComercios(this HttpSessionStateBase httpSession)
        {
            return httpSession[UserComercios] as int[] ?? new int[] { };
        }
        public static int[] ObtenerEmpresas(this HttpSessionStateBase httpSession)
        {
            return httpSession[UserEmpresas] as int[] ?? new int[] { };
        }
        public static int[] ObtenerSucursales(this HttpSessionStateBase httpSession)
        {
            return httpSession[UserSucursales] as int[] ?? new int[] { };
        }

        public static int? ObtenerInventarioSucursal(this HttpSessionStateBase httpSession)
        {
            return httpSession[Inventario_Sucursal] as int?;
        }
        public static void GuardarInventarioSucursal(this HttpSessionStateBase httpSession, int? sucursalId)
        {
            httpSession.Add(Inventario_Sucursal, sucursalId);
        }
    }
}