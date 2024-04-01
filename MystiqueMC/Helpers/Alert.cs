using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Helpers
{
    public class Alert
    {
        public const string TempDataKey = "TemDataAlerts";
        public string AlertStyle { get; set; }
        public string Message { get; set; }
        public bool Visible { get; set; }
        
    }
    public static class AlertStyles
    {
        public const string Success = "success";
        public const string Information = "info";
        public const string Warning = "warning";
        public const string Danger = "danger";

    }
}