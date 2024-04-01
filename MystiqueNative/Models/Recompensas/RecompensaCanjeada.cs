using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class RecompensaCanjeada : BaseModel
    {
        [JsonProperty("codigoGenerado")]
        public string CodigoQR { get; set; }
        [JsonProperty("idCanjePunto")]
        public string Id { get; set; }
        [JsonProperty("folioCanje")]
        public string FolioCanje { get; set; }
        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("estatusCanje")]
        public string Estatus { get; set; }
        [JsonProperty("imagen")]
        public string ImgRecompensa { get; set; }
        [JsonProperty("DiasRestantes")]
        public string DiasRestantes { get; set; }
        [JsonProperty("HorasRestantes")]
        public string HorasRestantes { get; set; }
        [JsonProperty("MinutosRestantes")]
        public string MinutosRestantes { get; set; }
        public int DiasAsInt
        {
            get
            {
                if (int.TryParse(DiasRestantes, out int p))
                    return p;
                else
                    return 0;
            }
        }
        public int HorasAsInt
        {
            get
            {
                if (int.TryParse(HorasRestantes, out int p))
                    return p;
                else
                    return 0;
            }
        }
        public int MinutosAsInt
        {
            get
            {
                if (int.TryParse(MinutosRestantes, out int p))
                    return p;
                else
                    return 0;
            }
        }
    }
}
