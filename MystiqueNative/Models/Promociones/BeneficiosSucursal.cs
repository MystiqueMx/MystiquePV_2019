using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class BeneficiosSucursal
    {
        [JsonProperty("idComercio")]
        public string IdComercio { get; set; }

        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }
        
        [JsonProperty("idBeneficio")]
        public string IdBeneficio { get; set; }

        [JsonProperty("HorarioActivo")]
        public string HorarioActivo { get; set; }

        [JsonProperty("HorarioInicio")]
        public string HorarioInicio { get; set; }

        [JsonProperty("HorarioFin")]
        public string HorarioFin { get; set; }

        [JsonProperty("Dias")]
        public string Dias { get; set; }

        [JsonProperty("DiasSucursal")]
        public string DiasSucursal { get; set; }

        [JsonProperty("TerminosYCondiciones")]
        public string Terminos { get; set; }

        [JsonProperty("tipoCodigo")]
        public string TipoCodigo { get; set; }

        [JsonProperty("CadenaCodigo")]
        public string CadenaCodigo { get; set; }

        [JsonProperty("urlImgBeneficio")]
        public string ImgBeneficio { get; set; }

        [JsonProperty("isDiaValido")]
        public string DiaValido { get; set; }

    }
}
