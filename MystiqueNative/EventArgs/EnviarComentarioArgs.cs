using System;

namespace MystiqueNative.ViewModels
{
    public class EnviarComentarioArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}