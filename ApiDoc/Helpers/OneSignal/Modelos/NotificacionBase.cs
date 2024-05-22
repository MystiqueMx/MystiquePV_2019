using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Helpers.OneSignal.Modelos
{
    public class NotificacionBase
    {

        public string app_id { get; set; }
        public string android_channel_id { get; set; }
        public string ios_badgeType { get; set; }
        public int ios_badgeCount { get; set; }
        public int ttl { get; set; }
    }
}