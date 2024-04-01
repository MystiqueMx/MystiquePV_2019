using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class ColoniaContainer : BaseModel
    {
        [JsonProperty("colonias")]
        public List<Colonia> colonias { get; set; }
    }
}