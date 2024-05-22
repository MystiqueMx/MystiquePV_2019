using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ApiDoc.Helpers.Facturacion.Models.Requests.Restaurante
{
    public class Factura
    {
        [Required]
        public int TipoComprobante { get; set; }
        
        [Required]
        [MaxLength(80)]
        public string Serie { get; set; }

        [Required]
        [MaxLength(100)]
        public string LugarExpedicion { get; set; }
        
        [Required]
        [MaxLength(80)]
        public string Folio { get; set; }
        
        [Required]
        public EmisorFactura Emisor { get; set; }
        
        [Required]
        public ReceptorFactura Receptor { get; set; }
        
        [Required]
        public ConsumoFactura Consumo { get; set; }
        
        public Guid Id { get; set; }
        public string SerializedId { get; set; }
        public string DescripcionRegimenFiscal { get; set; }
        public string DescripcionUsoCFDI { get; set; }
    }
}