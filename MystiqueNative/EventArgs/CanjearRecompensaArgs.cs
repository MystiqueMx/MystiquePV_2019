using System;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class CanjearRecompensaArgs : EventArgs
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public RecompensaCanjeada CodigoCanje { get; set; }
        public Recompensa Recompensa { get; internal set; }
    }
}