using System.ComponentModel.DataAnnotations;

namespace ApiDoc.Helpers.Facturacion.Models.Requests.Restaurante
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