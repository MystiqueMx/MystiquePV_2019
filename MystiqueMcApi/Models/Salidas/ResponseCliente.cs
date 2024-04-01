using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseCliente : ResponseBase
    {
        public long ClienteId { get; set; }
        public string ID_AspNetUsers { get; set; }
        public string IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public bool? Estatus { get; set; }
        public string Email { get; set; }
        public System.DateTime? FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public System.DateTime? FechaRegistro { get; set; }
        public int? DireccionId { get; set; }
        public int? EmpresaId { get; set; }
        public string RFC { get; set; }
        public string Empresa { get; set; }
        public string conektaId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? ZonaCitySalads { get; set; }
        public string GiroDescr { get; set; }
        public string TipoComercioDescr { get; set; }
        public string EstatusDescr { get; set; }
        public string DomicilioCompleto { get; set; }
        public string EmpresaDescr { get; set; }
        public string NombreCompleto { get; set; }
        public int? BeneficioId { get; set; }
        public string NombreComercial { get; set; }
        public string FechaPedido { get; set; }
        public string Descripcion { get; set; }
        public int? BeneficioSolicitado { get; set; }
        public int? BeneficioRealizados { get; set; }
        public string FolioMembresia { get; set; }
        public string PlayerId { get; set; }
    }
    
    public class ResponseClienteValidarMembresiaVinculada : ResponseBase
    {
        public bool? existe { get; set; }
    }

    public class ResponseClienteCallCenter : ErrorObjCodeResponseBase
    {
        public List<ListClientesCallCenter> ListaClientesCallCenter { get; set; }

        public bool existe { get; set; }
    }

    public class ListClientesCallCenter
    {
        public int ID { get; set; }

        public string nombreCompleto { get; set; }

        public string telefono { get; set; }
    }
}