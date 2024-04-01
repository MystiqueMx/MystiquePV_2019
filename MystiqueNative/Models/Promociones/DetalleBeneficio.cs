using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class DetalleBeneficio
    {
        [JsonProperty("BeneficioDescripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("UrlImagenCodigo")]
        public string CodigoQRUrl { get; set; }
        [JsonProperty("contentBarQRCode")]
        public string CodigoQRString { get; set; }
        [JsonProperty("hasCode")]
        public bool TieneCodigo { get; set; }
        [JsonProperty("SucursalNombre")]
        public string Sucursal { get; set; }
        [JsonProperty("typeCode")]
        public string TipoCodigo { get; set; }
        [JsonProperty("cantidadCalificados")]
        public string NumeroCalificaciones { get; set; }
        [JsonProperty("urlImgBeneficio")]
        public string ImagenBeneficioUrl { get; set; }
        [JsonProperty("calificadoPorCliente")]
        public bool EstaCalificado { get; set; }
        [JsonProperty("savedInWallet")]
        public bool EstaEnWallet { get; set; }
        [JsonProperty("Dias")]
        public string DiasDisponible { get; set; }
        [JsonProperty("calificacionPromedio")]
        public string CalificacionAsString { get; set; }
        /// <summary>
        /// //////////////////////////////////////
        /// </summary>
        public int CalificacionAsInt
        {
            get
            {
                return (int) System.Math.Round(CalificacionAsDecimal);
            }
        }
        public decimal CalificacionAsDecimal
        {
            get
            {
                if (decimal.TryParse(CalificacionAsString, out decimal d))
                {
                    return d;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
