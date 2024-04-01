using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models
{
    public class Notificacion
    {
        public int idNotificacion { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public long usuarioRegistro { get; set; }
        public Nullable<bool> isBeneficio { get; set; }
        public Nullable<int> beneficioId { get; set; }
        public bool activo { get; set; }
        public string titulo { get; set; }
    }


    public class NotificacionPush
    {
        public string headings { get; set; }
        public string contents { get; set; }
        public string playerId { get; set; }
    }
}