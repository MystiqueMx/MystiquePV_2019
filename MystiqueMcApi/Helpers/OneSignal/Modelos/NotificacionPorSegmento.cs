using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Helpers.OneSignal.Modelos
{
    public class NotificacionPorSegmento : NotificacionBase
    {
        public string[] included_segments { get; set; }
        public dynamic contents { get; set; }
        public dynamic headings { get; set; }
    }
}