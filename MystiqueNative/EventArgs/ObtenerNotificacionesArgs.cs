using System;

namespace MystiqueNative.ViewModels
{
    public class ObtenerNotificacionesArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int NotificacionesNuevas { get; set; }
    }
}