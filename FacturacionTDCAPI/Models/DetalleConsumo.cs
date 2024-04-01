using System.ComponentModel.DataAnnotations;
using Facturacion.v33;

namespace FacturacionTDCAPI.Models.Requests.Restaurante
{
    public class DetalleConsumo
    {
        
        [Required]
        public c_ClaveProdServ ClaveProdServ { get; set; }
        
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
        public string ClaveSat { get; set; }
        public string ClaveUnidadSat { get; set; }

    }
}