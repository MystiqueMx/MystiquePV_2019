using System;
using System.ComponentModel.DataAnnotations;
using FacturacionApi.Data.Sat.Catalogos;

namespace FacturacionApi.Models.Requests.Restaurante
{
    public class Factura
    {
        [Required]
        public c_TipoDeComprobante TipoComprobante { get; set; }
        
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
    }
}