using System;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class ActualizarPerfilEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Profile Usuario { get; set; }
    }
}