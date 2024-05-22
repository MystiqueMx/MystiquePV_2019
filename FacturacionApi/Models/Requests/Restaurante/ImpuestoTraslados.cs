using System.ComponentModel.DataAnnotations;
using FacturacionApi.Data.Sat.Catalogos;

namespace FacturacionApi.Models.Requests.Restaurante
{
    public class ImpuestoTraslados
    {
        [Required]
        public c_Impuesto ClaveImpuesto { get; set; }
        [Required]
        public decimal Tasa { get; set; }
        [Required]
        public decimal Impuesto { get; set; }

    }
}