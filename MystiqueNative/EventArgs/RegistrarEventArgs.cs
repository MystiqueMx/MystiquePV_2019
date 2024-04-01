using System;

namespace MystiqueNative.ViewModels
{
    public class RegistrarEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}