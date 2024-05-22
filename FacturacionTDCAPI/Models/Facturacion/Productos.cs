using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacturacionTDCApi.Models.Facturacion
{
    public class Productos
    {
        [Required]
        public string DescripcionP { get; set; }
        [Required]
        public int ClaveSat { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Iva { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }
    }
}