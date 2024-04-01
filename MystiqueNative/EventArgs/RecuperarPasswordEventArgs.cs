using System;

namespace MystiqueNative.ViewModels
{
    public class RecuperarPasswordEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}