using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models
{
    public class ControlInventariosResponse
    {
        public bool isSuccess { get; set; }
        public bool hasError { get; set; }
        public string message { get; set; }
        public bool isRedirectToAction { get; set; }
        public string actionName { get; set; }
        public string controllerName { get; set; }
        public object routeValue { get; set; }
    }
    public enum TiposMovimientosInventario
    {
        Compra = 1,
        Ajuste = 2,
        Venta = 3
    }
}