using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MystiqueNative.Helpers;

namespace MystiqueNative.Models.Location
{
    public class ColoniasContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public List<Colonia> Colonias { get; set; }
    }

    public class Colonia
    {
        [JsonProperty("idColonia")]
        public int Id { get; set; }

        [JsonProperty("nombreColonia")]
        public string Nombre { get; set; }

        [JsonProperty("codigoPostal")]
        public string CodigoPostal { get; set; }
    }
}
