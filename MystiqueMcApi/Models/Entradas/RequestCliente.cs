using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
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

    public class RequestClienteCallCenter
    {
        public string telefono { get; set; }
        public string nombre { get; set; }
    }

    public class RequestRegistroClienteCallCenter
    {
        [Required]
        public string nombre { get; set; }

        [Required]
        public string apPaterno { get; set; }

        [Required]
        public string apMaterno { get; set; }

        [Required]
        public string telefono { get; set; }
    }

}