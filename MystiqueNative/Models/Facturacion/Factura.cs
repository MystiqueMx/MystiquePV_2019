using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class Factura 
    {
        [JsonProperty("facturaClienteId")]
        public string Id { get; set; }
        [JsonProperty("numeroFactura")]
        public string Folio { get; set; }
        [JsonProperty("fecha")]
        public string FechaRegistro { get; set; }
        [JsonProperty("fechaCompra")]
        public string FechaCompra { get; set; }
        [JsonProperty("montoCompra")]
        public string MontoCompra { get; set; }
        [JsonProperty("sucursal")]
        public string Sucursal { get; set; }
        [JsonProperty("estatus")]
        public string Estatus { get; set; }
        [JsonProperty("PuedeReenviar")]
        public bool PuedeReenviar { get; set; }
        [JsonProperty("rfc")]
        public string RfcReceptor { get; set; }
        [JsonProperty("razonSocial")]
        public string RazonSocialReceptor { get; set; }
        [JsonProperty("email")]
        public string EmailReceptor { get; set; }
        /// <summary>
        /// //////////////////////////////////////
        /// </summary>
        public string FechaCompraConFormatoEspanyol
        {
            get
            {
                if (string.IsNullOrEmpty(FechaCompra))
                    return FechaCompra;
                else
                {
                    if (1900 > FechaCompraAsDateTime.Value.Year)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return DateTime.Parse(FechaCompra).ToString("dd/MM/yyyy");
                    }
                }
            }
        }
        public DateTime? FechaCompraAsDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(FechaCompra))
                    return null;
                else
                    return DateTime.Parse(FechaCompra);
            }
        }
        public string MontoCompraConFormatoMoneda
        {
            get
            {
                if (decimal.TryParse(MontoCompra, out decimal p))
                {
                    return p.ToString("C");
                }
                else
                    return "-";
            }
        }
    }
}
