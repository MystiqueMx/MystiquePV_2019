using System;

namespace MystiqueNative.ViewModels
{
    public class AgregarPuntosArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}