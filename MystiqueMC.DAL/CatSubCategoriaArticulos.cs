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
    
    public partial class CatSubCategoriaArticulos
    {
        public int idCatSubCategoriaArticulos { get; set; }
        public Nullable<int> comercioId { get; set; }
        public int catCategoriaArticulosId { get; set; }
        public string descripcion { get; set; }
    
        public virtual CatCategoriaArticulos CatCategoriaArticulos { get; set; }
    }
}
