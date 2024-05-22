using System.ComponentModel.DataAnnotations;

namespace FacturacionTDCAPI.Models
{
    public class ImpuestoTraslados
    {
        [Required]
        public int ClaveImpuesto { get; set; }
        [Required]
        public decimal Tasa { get; set; }
        [Required]
        public decimal Impuesto { get; set; }

    }
}