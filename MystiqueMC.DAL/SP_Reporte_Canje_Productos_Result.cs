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
    
    public partial class SP_Reporte_Canje_Productos_Result
    {
        public string Sucursal { get; set; }
        public string Nombre { get; set; }
        public string ProductoCanjeado { get; set; }
        public Nullable<System.DateTime> FechaCanje { get; set; }
        public decimal ValorPuntos { get; set; }
        public decimal MontoCompra { get; set; }
    }
}