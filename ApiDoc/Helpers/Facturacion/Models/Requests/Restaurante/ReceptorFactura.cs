using System.ComponentModel.DataAnnotations;

namespace ApiDoc.Helpers.Facturacion.Models.Requests.Restaurante
{
    public class ReceptorFactura
    {
        [Required]
        [MaxLength(300)]
        public string RazonSocial { get; set; }
        [Required]
        [RegularExpression(@"^([A-Za-zÑñ&]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([A-Z]|[0-9]){2}([A]|[0-9]){1})?$")]
        public string RFC { get; set; }
        [Required]
        [EmailAddress]
        public string CorreoElectrónico { get; set; }
        [Required]
        public string CodigoPostal { get; set; }
        [Required]
        [MaxLength(100)]
        public string Direccion { get; set; }
        [Required]
        public int UsoCFDI { get; set; }
        
    }
}