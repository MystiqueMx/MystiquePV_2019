using System;
using MystiqueNative.Models.Twitter;

namespace MystiqueNative.ViewModels
{
    public class SolicitarUsuarioTwitterEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public RequestToken Token { get; set; }
        public TwitterProfile User { get;set; }
    }
}