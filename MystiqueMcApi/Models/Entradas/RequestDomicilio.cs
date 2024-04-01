using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestDomicilio : AuthorizedRequestBase
    {
        public int sucursalId { get; set; }
    }

    public class RequestAgregarDomicilio : AuthorizedRequestBase
    {
        public int direccionId { get; set; }
        [Required]
        [MinLength(3), MaxLength(150)]
        public string calle { get; set; }
        public string numeroInt { get; set; }
        public string numeroExt { get; set; }
        public int? coloniaId { get; set; }
        public string nombreColonia { get; set; }
        public string referencias { get; set; }
        [Required]
        public double latitud { get; set; }
        [Required]
        public double longitud { get; set; }
        public string alias { get; set; }
        public bool activo { get; set; }
        public string codigoPostal { get; set; }
    }

    public class RequestObtenerColonias : AuthorizedRequestBase
    {
        public int sucursalId { get; set; }
        [Required]
        public float latitud { get; set; }
        [Required]
        public float longitud { get; set; }
        public string codigoPostal { get; set; }
    }

    public class RequestVerificarCobertura : AuthorizedRequestBase
    {
        public int sucursalId { get; set; }
        [Required]
        public float latitud { get; set; }
        [Required]
        public float longitud { get; set; }
        [Required]
        public string colonia { get; set; }
    }
}