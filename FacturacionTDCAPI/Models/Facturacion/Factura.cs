using System;
using System.ComponentModel.DataAnnotations;

namespace FacturacionTDCAPI.Models.Requests.Restaurante
{
    public class Factura
    {
        [Required]
        public int TipoComprobante { get; set; }
        
        
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