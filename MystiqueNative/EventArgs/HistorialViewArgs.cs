using System;

namespace MystiqueNative.ViewModels
{
    public class HistorialViewArgs : EventArgs
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Sumados { get; set; }
        public string Canjeados { get; set; }
        public string Actuales { get; set; }
    }
}