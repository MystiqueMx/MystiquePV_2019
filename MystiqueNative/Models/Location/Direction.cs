using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MystiqueNative.Helpers;

namespace MystiqueNative.Models.Location
{
    public class Direction
    {
        [JsonProperty("codigoPais")]
        public string CountryCode { get; set; }

        [JsonProperty("pais")]
        public string CountryName { get; set; }

        [JsonProperty("entidad")]
        public string AdminArea { get; set; }

        [JsonProperty("municipio")]
        public string SubAdminArea { get; set; }

        [JsonProperty("ciudad")]
        public string Locality { get; set; }

        [JsonProperty("nombreColonia")]
        public string SubLocality { get; set; }

        [JsonProperty("codigoPostal")]
        public string PostalCode { get; set; }

        [JsonProperty("calle")]
        public string Thoroughfare { get; set; }

        [JsonProperty("numeroExt")]
        public string SubThoroughfare { get; set; }

        [JsonProperty("FeatureName")]
        public string FeatureName { get; set; }

        [JsonProperty("referencias")]
        public string OtherAddressLines { get; set; }

        [JsonProperty("direccionId")]
        public int Id { get; set; }

        [JsonProperty("alias")]
        public string Nombre { get; set; }
        [JsonProperty("coloniaId")]
        public int? IdColonia { get; set; }
        [JsonProperty("latitud")]
        public double Latitud { get; set; }

        [JsonProperty("longitud")]
        public double Longitud { get; set; }
    }

    public class DirectionsContainer : BaseContainer
    {
       [JsonProperty("respuesta")] 
       public List<Direction> Respuesta { get; set; }
    }
    
}