using System.ComponentModel.DataAnnotations;

namespace MystiqueMcApi.Helpers.Facturacion.Models.Requests.Restaurante
{
    public class DetalleConsumo
    {
        
        [Required]
        public int ClaveProdServ { get; set; }
        
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
        public string Unidad { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ClaveUnidad { get; set; }

        [Required]
        public string Descripcion { get; set; }
        
    }
}