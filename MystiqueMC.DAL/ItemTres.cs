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
    
    public partial class ItemTres
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ItemTres()
        {
            this.ConfMenuPlatillosItemTres = new HashSet<ConfMenuPlatillosItemTres>();
        }
    
        public int idItemTres { get; set; }
        public int sucursalId { get; set; }
        public string nombre { get; set; }
        public Nullable<int> ordenamiento { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfMenuPlatillosItemTres> ConfMenuPlatillosItemTres { get; set; }
        public virtual sucursales sucursales { get; set; }
    }
}