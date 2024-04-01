using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class BeneficiosSContainer : BaseModel
    {
        [JsonProperty("listaBeneficiosSucursal")]
        public List<BeneficiosSucursal> BeneficiosSucursal { get; set; }
    }
}