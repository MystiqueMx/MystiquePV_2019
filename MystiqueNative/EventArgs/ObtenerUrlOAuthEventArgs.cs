using System;
using MystiqueNative.Models.Twitter;

namespace MystiqueNative.ViewModels
{
    public class ObtenerUrlOAuthEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Url { get; set; }
        public RequestToken Token { get; set; }
    }
}