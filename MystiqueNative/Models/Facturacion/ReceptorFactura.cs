using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models
{
    public class ReceptorFactura
    {
        [JsonProperty("receptorClienteId")]
        public string Id { get; set; }
        [JsonProperty("rfc")]
        public string Rfc { get; set; }
        [JsonProperty("companiaNombreLegal")]
        public string RazonSocial { get; set; }
        [JsonProperty("prederteminada")]
        public string Predeterminada { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("claveUsoCFDI")]
        public string UsoCFDI { get; set; }
        [JsonProperty("codigoPostal")]
        public string CodigoPostal { get; set; }
        [JsonProperty("direccion")]
        public string Direccion { get; set; }
    }
}
