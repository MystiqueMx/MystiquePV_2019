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
    
    public partial class Proveedores1
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Proveedores1()
        {
            this.Compras = new HashSet<Compras1>();
            this.Gastos = new HashSet<Gastos1>();
            this.GastosPvDetalle = new HashSet<GastosPvDetalle1>();
            this.SucursalProveedores = new HashSet<SucursalProveedores1>();
        }
    
        public int idProveedor { get; set; }
        public int comercioId { get; set; }
        public string descripcion { get; set; }
    
        public virtual comercios comercios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compras1> Compras { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gastos1> Gastos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GastosPvDetalle1> GastosPvDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SucursalProveedores1> SucursalProveedores { get; set; }
    }
}