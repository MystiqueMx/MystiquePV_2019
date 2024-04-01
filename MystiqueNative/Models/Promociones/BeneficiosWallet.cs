using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class BeneficiosWallet
    {
        [JsonProperty("idWallet")]
        public string IdWallet { get; set; }
        [JsonProperty("BeneficioId")]
        public string IdBeneficio { get; set; }
        [JsonProperty("BeneficioDescripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("UrlImagenCodigo")]
        public string CodigoQRUrl { get; set; }
        [JsonProperty("DiasRestantes")]
        public string DiasRestantes { get; set; }
        [JsonProperty("HorasRestantes")]
        public string HorasRestantes { get; set; }
        [JsonProperty("MinutosRestantes")]
        public string MinutosRestantes { get; set; }
        [JsonProperty("urlImgBeneficio")]
        public string ImagenBeneficioUrl { get; set; }
        [JsonProperty("contentBarQRCode")]
        public string CodigoQRString { get; set; }
        [JsonProperty("hasCode")]
        public bool TieneCodigo { get; set; }
        public int DiasAsInt
        {
            get
            {
                if (int.TryParse(DiasRestantes, out int p))
                    return p;
                else
                    throw new ArgumentException();
            }
        }
        public int HorasAsInt
        {
            get
            {
                if (int.TryParse(HorasRestantes, out int p))
                    return p;
                else
                    throw new ArgumentException();
            }
        }
        public int MinutosAsInt
        {
            get
            {
                if (int.TryParse(MinutosRestantes, out int p))
                    return p;
                else
                    throw new ArgumentException();
            }
        }
    }
}
