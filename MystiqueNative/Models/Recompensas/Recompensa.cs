using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class Recompensa
    {
        [JsonProperty("idRecompesa")]
        public string Id { get; set; }
        [JsonProperty("descripcion")]
        public string Tipo { get; set; }
        [JsonProperty("valor")]
        public string Costo { get; set; }
        [JsonProperty("imagen")]
        public string ImgRecompensa { get; set; }
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
        [JsonProperty("descripcionProducto")]
        public string Descripcion { get; set; }
        [JsonProperty("diasCanje")]
        public string DiasVigencia { get; set; }

        public int CostoAsInt
        {
            get
            {
                if (decimal.TryParse(Costo, out decimal p))
                    return (int) p;
                else
                    throw new ArgumentException();
            }
        }
    }
}
