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
    
    public partial class CatZonas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CatZonas()
        {
            this.sucursales = new HashSet<sucursales>();
        }
    
        public int idCatZona { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string placeId { get; set; }
        public int usuarioRegistroId { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public int empresaId { get; set; }
    
        public virtual empresas empresas { get; set; }
        public virtual usuarios usuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sucursales> sucursales { get; set; }
    }
}
