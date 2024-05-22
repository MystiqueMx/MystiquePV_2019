using FacturacionTDCApi.Models.Facturacion;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FacturacionTDCAPI.Models
{
    public class DetalleConsumo
    {
        
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal IvaDetalle { get; set; }
        
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal ImporteDetalle { get; set; }
        
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal ValorUnitario { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Cantidad { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ClaveUnidadSat { get; set; }

        [Required]
        public string Unidad { get; set; }

        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string ClaveSat { get; set; }

    

    }
}