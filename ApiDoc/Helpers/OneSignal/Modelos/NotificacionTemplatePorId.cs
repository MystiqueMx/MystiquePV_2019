using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Helpers.OneSignal.Modelos
{
    public class NotificacionTemplatePorIds : NotificacionBase
    {
        public string[] include_player_ids { get; set; }
        public string template_id { get; set; }
    }
}