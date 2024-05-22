using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Helpers.OneSignal.Modelos
{
    public class NotificacionTemplatePorSegmento : NotificacionBase
    {
        public string[] included_segments { get; set; }
        public string template_id { get; set; }
    }
}