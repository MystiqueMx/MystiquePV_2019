using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.EventsArgs
{
    public class ObtenerColoniasEventArgs : BaseListEventArgs
    {
        public List<string> Colonias { get; set; }
    }
}
