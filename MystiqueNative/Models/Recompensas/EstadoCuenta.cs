using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models
{
    public class EstadoCuenta : BaseModel
    {
        [JsonProperty("puntosActuales")]
        public string Puntos { get; set; }
        [JsonProperty("puntosAnteriores")]
        public string HistoricoPuntos { get; set; }
        [JsonProperty("visitasActuales")]
        public string Visitas { get; set; }
        [JsonProperty("visitasAnteriores")]
        public string HistoricoVisitas { get; set; }
        public int PuntosAsInt
        {
            get
            {
                if (float.TryParse(Puntos, out float p))
                    return (int)p;
                else
                    throw new ArgumentException();
            }
        }
    }
}
