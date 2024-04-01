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
    
    public partial class ConsumidorDirecciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConsumidorDirecciones()
        {
            this.Pedidos1 = new HashSet<Pedidos1>();
        }
    
        public int idConsumidorDireccion { get; set; }
        public int consumidorId { get; set; }
        public string alias { get; set; }
        public string calle { get; set; }
        public string numExterior { get; set; }
        public string numInterior { get; set; }
        public Nullable<int> catColoniaId { get; set; }
        public string nombreColonia { get; set; }
        public int catCiudadId { get; set; }
        public int catEstadoId { get; set; }
        public string referencia { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public bool activo { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public Nullable<System.DateTime> fechaModificacion { get; set; }
        public string codigoPostal { get; set; }
    
        public virtual catCiudades catCiudades { get; set; }
        public virtual catColonias catColonias { get; set; }
        public virtual catEstados catEstados { get; set; }
        public virtual clientes clientes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedidos1> Pedidos1 { get; set; }
    }
}
