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
    
    public partial class ImagenDoctor
    {
        public int idImagenDoctor { get; set; }
        public string ruta { get; set; }
        public int comercioId { get; set; }
    
        public virtual comercios comercios { get; set; }
    }
}
