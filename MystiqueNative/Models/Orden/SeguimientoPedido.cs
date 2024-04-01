using Humanizer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MystiqueNative.Models.Orden
{
    public class SeguimientoPedido
    {
        [JsonProperty("fecha")]
        public DateTime Fecha { get; set; }

        [JsonProperty("comentario")]
        public string Comentario { get; set; }

        public string HoraRelativa => Fecha.Humanize(utcDate: false, culture: new CultureInfo("es-ES"));
    }
}
