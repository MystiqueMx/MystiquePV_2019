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
    
    public partial class ConteoFisicoInsumos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConteoFisicoInsumos()
        {
            this.RegistroConteoFisicoInsumos = new HashSet<RegistroConteoFisicoInsumos>();
        }
    
        public int idConteoFisicoInsumo { get; set; }
        public string descripcion { get; set; }
        public int insumoId { get; set; }
        public bool activo { get; set; }
        public int conteoFisicoAgrupadorInsumosId { get; set; }
    
        public virtual ConteoFisicoAgrupadorInsumos ConteoFisicoAgrupadorInsumos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistroConteoFisicoInsumos> RegistroConteoFisicoInsumos { get; set; }
        public virtual Insumos Insumos { get; set; }
    }
}
