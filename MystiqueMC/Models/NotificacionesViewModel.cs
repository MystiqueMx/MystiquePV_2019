using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models
{
    public class NotificacionesViewModel
    {
        public IEnumerable<catRangoEdad> RangosEdades { get; set; }
        public IEnumerable<sucursales> Sucursales { get; set; }
        public IEnumerable<catSexos> Sexos { get; set; }
        public IEnumerable<notificaciones> NotificacionesAnteriores { get; set; }
        public IEnumerable<notificaciones> NotificacionesActuales{ get; set; }
    }
}