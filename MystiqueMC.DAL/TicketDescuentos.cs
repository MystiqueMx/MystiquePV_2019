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
    
    public partial class TicketDescuentos
    {
        public int idTicketDescuento { get; set; }
        public int ticketId { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public decimal monto { get; set; }
        public int descuentoId { get; set; }
    
        public virtual Tickets Tickets { get; set; }
        public virtual Descuentos Descuentos { get; set; }
    }
}
