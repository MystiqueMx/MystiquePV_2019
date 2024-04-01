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
    
    public partial class clientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public clientes()
        {
            this.beneficioAplicados = new HashSet<beneficioAplicados>();
            this.beneficioCalificaciones = new HashSet<beneficioCalificaciones>();
            this.Citas = new HashSet<Citas>();
            this.clienteNotificaciones = new HashSet<clienteNotificaciones>();
            this.comentarios = new HashSet<comentarios>();
            this.login = new HashSet<login>();
            this.membresias = new HashSet<membresias>();
            this.receptorCliente = new HashSet<receptorCliente>();
            this.wallet = new HashSet<wallet>();
            this.ConsumidorConektaTarjetas = new HashSet<ConsumidorConektaTarjetas>();
            this.ConsumidorDirecciones = new HashSet<ConsumidorDirecciones>();
            this.ConsumidorOpenPay = new HashSet<ConsumidorOpenPay>();
            this.OpenPayTransacciones = new HashSet<OpenPayTransacciones>();
            this.ConsumidoresConekta = new HashSet<ConsumidoresConekta>();
            this.ConsumidorNotificaciones = new HashSet<ConsumidorNotificaciones>();
            this.Pedidos1 = new HashSet<Pedidos1>();
        }
    
        public int idCliente { get; set; }
        public string nombre { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public bool estatus { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> fechaNacimiento { get; set; }
        public string telefono { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public Nullable<bool> zonaMystique { get; set; }
        public string urlFotoPerfil { get; set; }
        public Nullable<System.DateTime> fechaCargaFoto { get; set; }
        public Nullable<int> catSexoId { get; set; }
        public Nullable<int> catColoniaId { get; set; }
        public string facebookId { get; set; }
        public int empresaId { get; set; }
        public Nullable<int> catTipoAutenficacionId { get; set; }
        public Nullable<int> catRangoEdadId { get; set; }
        public Nullable<int> catAseguranzaId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<beneficioAplicados> beneficioAplicados { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<beneficioCalificaciones> beneficioCalificaciones { get; set; }
        public virtual CatAseguranzas CatAseguranzas { get; set; }
        public virtual catColonias catColonias { get; set; }
        public virtual catRangoEdad catRangoEdad { get; set; }
        public virtual catSexos catSexos { get; set; }
        public virtual catTipoAutentificacion catTipoAutentificacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Citas> Citas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<clienteNotificaciones> clienteNotificaciones { get; set; }
        public virtual empresas empresas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comentarios> comentarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<login> login { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<membresias> membresias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<receptorCliente> receptorCliente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<wallet> wallet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumidorConektaTarjetas> ConsumidorConektaTarjetas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumidorDirecciones> ConsumidorDirecciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumidorOpenPay> ConsumidorOpenPay { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpenPayTransacciones> OpenPayTransacciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumidoresConekta> ConsumidoresConekta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumidorNotificaciones> ConsumidorNotificaciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedidos1> Pedidos1 { get; set; }
    }
}
