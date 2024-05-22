using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseLogin : ResponseBase
    {
        public int clienteId { get; set; }
        public string email { get; set; }
        public string nombre { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public string telefono { get; set; }
        public int? sexo { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public string colonia { get; set; }
        public int membresiaId { get; set; }
        public string folioMembresia { get; set; }
        public DateTime? fechaFinMembresia { get; set; }
        public string folioGuidMembresia { get; set; }
        public string urlFotoPerfil { get; set; }
        public DateTime? fechaCargaFoto { get; set; }
        public string facebookId { get; set; }
        public int? catColoniaId { get; set; }
        public int? catCiudadId { get; set; }
        public bool registroCompleto { get; set; }
        public int? tipoAutentificacionId { get; set; }
        public int? rangoEdadId { get; set; }
        public int? catAseguranzaId { get; set; }
        public string rangoEdadDescripcion { get; set; }
    }

    public class ResponseLoginDoctor : ResponseBase
    {
        public int anexoDoctorId { get; set; }
        public int doctorId { get; set; }
        public string email { get; set; }
        public string userName { get; set; }
        public string nombre { get; set; }
        public string paterno { get; set; }
        public string telefono { get; set; }
        public int? sexo { get; set; }
        public DateTime? fechaRegistro { get; set; }
        public string urlFotoPerfil { get; set; }
        public string rfc { get; set; }
        public string direccion { get; set; }
        public string cedula { get; set; }
        public int? empresaId { get; set; }
        public int? catComercioGiroId { get; set; }
        public int? catTipoComercioId { get; set; }
        public bool registroCompleto { get; set; }
        public int? tipoAutentificacionId { get; set; }
        public int? doctorEspecialidadId { get; set; }
        public int? rangoEdadId { get; set; }
        public string paginaWeb { get; set; }
    }


    public class ResponseLoginActivo : ResponseBase
    {
        public bool? isActivo { get; set; }
    }

    public class ResponseLoginRevisarNombreUsuarioDisponible : ResponseBase
    {
        public bool? isTaken { get; set; }
    }

    public class ResponseLogout : ResponseBase
    {

    }

}