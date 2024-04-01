//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MystiqueMC.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Arqueo
    {
        public int idArqueo { get; set; }
        public int sucursalId { get; set; }
        public int ventaId { get; set; }
        public int aperturaId { get; set; }
        public System.DateTime fechaRegistroVenta { get; set; }
        public string concepto { get; set; }
        public decimal saldoDlls { get; set; }
        public decimal arqueoDlls { get; set; }
        public decimal efectivo { get; set; }
        public decimal cxc { get; set; }
        public decimal tarjeta { get; set; }
        public decimal gasto { get; set; }
        public decimal total { get; set; }
        public Nullable<decimal> totalRecibido { get; set; }
        public Nullable<decimal> diferencia { get; set; }
        public string observacion { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public Nullable<System.DateTime> fechaActualizacion { get; set; }
        public Nullable<int> usuarioActualizo { get; set; }
    
        public virtual usuarios usuarios { get; set; }
        public virtual Aperturas Aperturas { get; set; }
        public virtual Ventas Ventas { get; set; }
        public virtual sucursales sucursales { get; set; }
    }
}
