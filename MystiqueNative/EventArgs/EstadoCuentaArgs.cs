using System;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class EstadoCuentaArgs : EventArgs
    {
        public EstadoCuenta EstadoCuenta { get; internal set; }
    }
}