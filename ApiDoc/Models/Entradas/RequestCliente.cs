using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Entradas
{
    public class RequestCliente : RequestBase
    {
        public int clienteId { get; set; }
        public string nombre { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public System.DateTime? fechaNacimiento { get; set; }
        public string telefono { get; set; }
        public int? catSexoId { get; set; }
        public int? coloniaId { get; set; }
        public string contraseniaNueva { get; set; }
        public int? rangoEdadId { get; set; }
        public int? aseguradoraId { get; set; }
    }


    public class RequestClienteValidarMenbresiaVinculada : RequestBase
    {
        public int clienteId { get; set; }
        public string folioMembresia { get; set; }

    }


    public class RequestClienteObtenerPorId : RequestBase
    {
        public int clienteId { get; set; }

    }

}