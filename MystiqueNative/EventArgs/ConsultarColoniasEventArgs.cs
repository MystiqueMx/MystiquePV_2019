using System;
using System.Collections.Generic;

namespace MystiqueNative.ViewModels
{
    public class ConsultarColoniasEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public List<string> Colonias { get; set; }
    }
}