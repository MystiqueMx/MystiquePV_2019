using System.ComponentModel.DataAnnotations;
using FacturacionApi.Data.Sat.Catalogos;

namespace FacturacionApi.Models.Requests.Restaurante
{
    public class EmisorFactura
    {
        [Required]
        [MaxLength(300)]
        public string RazonSocial { get; set; }
        [Required]
        [RegularExpression(@"^([A-Za-zÑñ&]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([A-Z]|[0-9]){2}([A]|[0-9]){1})?$")]
        public string RFC { get; set; }
        [Required]
        [MinLength(5), MaxLength(5)]
        public string CodigoPostal { get; set; }
        [Required]
        public c_RegimenFiscal RegimenFiscal { get; set; }
        [Required]
        [MaxLength(80)]
        public string Sucursal { get; set; }
        [MaxLength(110)]
        public string Direccion { get; set; }
    }
}