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
    
    public partial class beneficioCalificaciones
    {
        public int idBeneficioCalificacion { get; set; }
        public int beneficioId { get; set; }
        public int clienteId { get; set; }
        public int calificacion { get; set; }
        public Nullable<System.DateTime> fechaRegistro { get; set; }
    
        public virtual beneficios beneficios { get; set; }
        public virtual clientes clientes { get; set; }
    }
}