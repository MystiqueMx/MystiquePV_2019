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
    
    public partial class CatConceptosGastos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CatConceptosGastos()
        {
            this.Gastos = new HashSet<Gastos>();
            this.GastosPvDetalle = new HashSet<GastosPvDetalle>();
        }
    
        public int idCatConceptoGasto { get; set; }
        public int comercioId { get; set; }
        public int catRubroId { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public decimal ponderacion { get; set; }
    
        public virtual comercios comercios { get; set; }
        public virtual CatRubros CatRubros { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gastos> Gastos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GastosPvDetalle> GastosPvDetalle { get; set; }
    }
}
