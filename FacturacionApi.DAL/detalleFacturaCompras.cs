//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FacturacionApi.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class detalleFacturaCompras
    {
        public int idDetalleFacturaCompra { get; set; }
        public int idFacturaCompra { get; set; }
        public int claveProdServ { get; set; }
        public decimal ivaDetalle { get; set; }
        public decimal importeDetalle { get; set; }
        public decimal valorUnitario { get; set; }
        public int cantidad { get; set; }
        public string unidad { get; set; }
        public string descripcion { get; set; }
        public string claveUnidad { get; set; }
    
        public virtual facturaCompras facturaCompras { get; set; }
    }
}
