using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Facturacion.v33;
using FacturacionTDCAPI.Helpers.DataAnnotations;
using FacturacionTDCAPI.Models;

namespace FacturacionTDCAPI.Models
{
    public class ConsumoFactura
    {
        [Required]
        [MaxLength(15)]
        public string FolioConsumo { get; set; }
        
        [Required]
        public System.DateTime FechaConsumo { get; set; }
        
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Subtotal { get; set; }
        
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Total { get; set; }
        
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Iva { get; set; }

        [Required]
        [MaxLength(100)]
        public string ImporteLetra { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal TipoCambio { get; set; }
        
        [Required]
        public c_Moneda Moneda { get; set; }
        
        [Required]
        public c_MetodoPago ClaveMetodoPago { get; set; }
        
        [Required]
        public c_FormaPago ClaveFormaPago { get; set; }
        
        [Required]
        public c_TipoDeComprobante ClaveTipoComprobante { get; set; }
        
        [Range(0.0, double.MaxValue)]
        public decimal Descuento { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string CondicionesPago{ get; set; }
        
        [MaxLength(100)]
        public string Referencia { get; set; }
        
        [MaxLength(100)]
        public string Tarjeta { get; set; }
        
        [Required]
        [ListNoEmpty(ErrorMessage = "List must contain an element")]
        public List<DetalleConsumo> Detalle { get; set; }
        
        [Required]
        [ListNoEmpty(ErrorMessage = "List must contain an element")]
        public List<ImpuestoTraslados> Traslados { get; set; }
    }
}