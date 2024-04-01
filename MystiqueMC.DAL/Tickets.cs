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
    
    public partial class Tickets
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tickets()
        {
            this.PedidoProductos = new HashSet<PedidoProductos>();
            this.TicketCancelados = new HashSet<TicketCancelados>();
            this.TicketDescuentos = new HashSet<TicketDescuentos>();
            this.TicketReimpresos = new HashSet<TicketReimpresos>();
            this.TicketPagos = new HashSet<TicketPagos>();
        }
    
        public int idTicket { get; set; }
        public int pedidoId { get; set; }
        public int ticketIdSucursal { get; set; }
        public System.DateTime fechaCobro { get; set; }
        public int numeroTicket { get; set; }
        public string folioTicket { get; set; }
        public string usuarioRegistro { get; set; }
        public bool facturado { get; set; }
        public decimal subtotal { get; set; }
        public decimal iva { get; set; }
        public decimal importe { get; set; }
        public Nullable<System.DateTime> fechaCancelacion { get; set; }
        public string uuidTicket { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public bool estaCancelado { get; set; }
        public decimal tasaIva { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PedidoProductos> PedidoProductos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketCancelados> TicketCancelados { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketDescuentos> TicketDescuentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketReimpresos> TicketReimpresos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketPagos> TicketPagos { get; set; }
        public virtual Pedidos Pedidos { get; set; }
    }
}
