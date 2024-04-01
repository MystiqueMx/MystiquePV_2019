using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class SucursalContainer : BaseModel
    {
        [JsonProperty("ListSucursalPorComercio")]
        public List<Sucursal> ListaSucursales { get; set; }
    }
}