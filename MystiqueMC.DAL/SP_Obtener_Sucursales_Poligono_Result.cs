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
    
    public partial class SP_Obtener_Sucursales_Poligono_Result
    {
        public int idSucursal { get; set; }
        public string nombre { get; set; }
        public string logoUrl { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.TimeSpan> horaInicio { get; set; }
        public Nullable<System.TimeSpan> horaFin { get; set; }
        public decimal montoMinimo { get; set; }
        public decimal costoEnvio { get; set; }
        public int abierto { get; set; }
        public string direccion { get; set; }
        public bool activoPlataforma { get; set; }
        public bool ReparteDomicilio { get; set; }
        public bool tieneDriveThru { get; set; }
    }
}