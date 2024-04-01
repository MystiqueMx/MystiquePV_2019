using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class DetalleBeneficioContainer : BaseModel
    {
        [JsonProperty("beneficioDetalle")]
        public DetalleBeneficio Detalle { get; set; }
    }
}