using System;
using MystiqueNative.Helpers;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class LoginEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Profile Usuario { get; set; }
        public AuthMethods Method { get; internal set; }
        public string Username { get; internal set; }
        public string Password { get; internal set; }
    }
}