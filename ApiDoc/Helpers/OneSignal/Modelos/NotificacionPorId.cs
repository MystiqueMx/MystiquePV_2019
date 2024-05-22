using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Helpers.OneSignal.Modelos
{
    public class NotificacionPorId : NotificacionBase
    {
        public string[] include_player_ids { get; set; }
        public dynamic contents { get; set; }
        public dynamic headings { get; set; }
    }
}