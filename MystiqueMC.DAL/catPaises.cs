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
    
    public partial class catPaises
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public catPaises()
        {
            this.catEstados = new HashSet<catEstados>();
        }
    
        public int idCatPais { get; set; }
        public string paisDescr { get; set; }
        public int usuarioRegistroId { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public string codigoPais { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<catEstados> catEstados { get; set; }
        public virtual usuarios usuarios { get; set; }
    }
}
