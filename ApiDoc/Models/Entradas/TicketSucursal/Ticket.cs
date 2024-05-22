using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ApiDoc.Helpers.Facturacion.Models.Requests.Restaurante;

namespace ApiDoc.Models.Entradas.TicketSucursal
{
    public class Ticket
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdSucursal { get; set; }
        [Required]
        public DateTime FechaCompra { get; set; }
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Subtotal { get; set; }
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Total { get; set; }
        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Iva { get; set; }
        [Required]
        public int ClaveMetodoPago { get; set; }
        [Required]
        public int ClaveFormaPago { get; set; }
        [Required]
        public int ClaveImpuesto { get; set; }
        [Required]
        public int ClaveTipoComprobante { get; set; }
        [Required]
        [MaxLength(10)]
        public string Folio { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Descuento { get; set; }
        [MaxLength(100)]
        public string Referencia { get; set; }

        [Required]
        [MaxLength(255)]
        public string LugarExpedicion { get; set; }
        [Required]
        [MaxLength(100)]
        public string SerieFactura { get; set; }
        [Required]
        [MaxLength(100)]
        public string FolioFactura { get; set; }
        [Required]
        [MaxLength(255)]
        public string CondicionesPago { get; set; }

        [Range(0, int.MaxValue)]
        public int? Moneda { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal? TipoCambio { get; set; }

        public List<DetalleConsumo> ConceptosConsumo { get; set; }
        public List<ImpuestoTraslados> ImpuestoTraslados { get; set; }
    }
}