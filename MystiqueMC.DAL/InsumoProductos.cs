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
    
    public partial class InsumoProductos
    {
        public int idInsumoProducto { get; set; }
        public int agrupadorInsumoId { get; set; }
        public Nullable<int> productoId { get; set; }
        public Nullable<int> insumoId { get; set; }
    
        public virtual AgrupadorInsumos AgrupadorInsumos { get; set; }
        public virtual Insumos Insumos { get; set; }
        public virtual Productos Productos { get; set; }
    }
}
