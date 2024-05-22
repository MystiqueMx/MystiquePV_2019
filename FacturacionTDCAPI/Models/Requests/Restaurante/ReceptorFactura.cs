using System.ComponentModel.DataAnnotations;

namespace FacturacionTDCAPI.Models.Requests.Restaurante
{
    public class ReceptorFactura
    {
        [Required]
        [MaxLength(300)]
        public string RazonSocial { get; set; }
        [Required]
        [RegularExpression(@"^([A-Za-zÑñ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([A-Za-z0-9]{3}))$")]
        public string Rfc { get; set; }
        [Required]
        [EmailAddress]
        public string CorreoElectrónico { get; set; }
        [Required]
        [MinLength(5), MaxLength(5)]
        public string CodigoPostal { get; set; }
        [Required]
        [MaxLength(100)]
        public string Direccion { get; set; }
        [Required]
        public int UsoCfdi { get; set; }
    }
}