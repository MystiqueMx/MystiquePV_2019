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
    
    public partial class confSumaPuntos
    {
        public int idConfSumaPunto { get; set; }
        public int comercioId { get; set; }
        public Nullable<System.DateTime> fechaRegistro { get; set; }
        public Nullable<bool> estatus { get; set; }
        public decimal montoCompraMinima { get; set; }
        public int horaValides { get; set; }
        public int diasValides { get; set; }
        public decimal cantidadPunto { get; set; }
        public decimal equivalentePuntoPorDinero { get; set; }
        public int catTipoMembresiaId { get; set; }
        public decimal porcentajeDescuento { get; set; }
    
        public virtual catTipoMembresias catTipoMembresias { get; set; }
    }
}
