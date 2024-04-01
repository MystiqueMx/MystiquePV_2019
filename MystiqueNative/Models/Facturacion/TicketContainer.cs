using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class TicketContainer : BaseModel
    {
        [JsonProperty("ticket")]
        public string Id { get; set; }
        [JsonProperty("sucursal")]
        public string Sucursal { get; set; }
        [JsonProperty("fechaCompra")]
        public string FechaCompra { get; set; }
        [JsonProperty("montoCompra")]
        public string MontoCompra { get; set; }
        [JsonProperty("sucursalId")]
        public string SucursalId { get; set; }
        [JsonProperty("PendienteTicket")]
        public bool PendienteTicket { get; set; }

        [JsonProperty("catUsoCFDI")]
        public List<UsoCfdi> CatalogoUsos { get; set; }
        [JsonProperty("listaDatosFiscalesReceptor")]
        public List<ReceptorFactura> ReceptoresGuardados { get; set; }
    }
}