using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Helpers.Analytics
{
    public static class AnalyticsActions
    {
        public static class Flujos
        {
            public const string FLUJO_BOTON_PROMOCIONES = "Flujos.Promociones";
            public const string FLUJO_BOTON_QDC = "Flujos.QuieroDeComer";
            public const string FLUJO_BOTON_SUMAR_PUNTOS = "Flujos.SumarPuntos";
            public const string FLUJO_BOTON_HISTORIAL_PUNTOS = "Flujos.VerHistorial";
            public const string FLUJO_BOTON_RECOMPENSAS = "Flujos.Recompensas";
            public const string FLUJO_BOTON_MEMBRESIA = "Flujos.Membresias";
            public const string FLUJO_BOTON_AYUDA = "Flujos.Ayuda";
            public const string FLUJO_BOTON_COMENTARIOS = "Flujos.Comentarios";
            public const string FLUJO_BOTON_NOTIFICACIONES = "Flujos.Notificaciones";
            public const string FLUJO_BOTON_CLIENTES = "Flujos.Clientes";
        }
        public static class MetodosLogin
        {
            public const string LOGOUT = "Logout";
            public const string LOGIN_GOOGLE = "Login.Google";
            public const string LOGIN_FACEBOOK = "Login.Facebook";
            public const string LOGIN_TWITTER = "Login.Twitter";
            public const string LOGIN_CORREO = "Login.Correo";
        }

        public static class Acciones
        {
            public const string VerRestaurante = "ver_restaurante";
        }
        public static class Errores
        {
            public const string TerminarPedido = "error_solicitar_pedido";
        }
    }
}
