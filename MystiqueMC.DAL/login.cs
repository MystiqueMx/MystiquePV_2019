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
    
    public partial class login
    {
        public int idLogin { get; set; }
        public int clienteId { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public bool sesionActiva { get; set; }
        public string sesionToken { get; set; }
        public string deviceId { get; set; }
        public string deviceModel { get; set; }
        public string devicePlatform { get; set; }
        public string deviceVersion { get; set; }
        public string deviceConnectionType { get; set; }
        public bool activo { get; set; }
        public bool isCliente { get; set; }
        public string playerId { get; set; }
        public System.DateTime fechaActualizacion { get; set; }
    
        public virtual clientes clientes { get; set; }
    }
}
