using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class NotificacionesContainer : BaseModel
    {
        [JsonProperty("listaNoticacionesCliente")]
        public List<Notificacion> Notificaciones { get; set; }
    }
}