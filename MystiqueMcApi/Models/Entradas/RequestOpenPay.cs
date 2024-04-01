using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestOpenPay : AuthorizedRequestBase
    {
        [Required]
        public string tokenId { get; set; }
        [Required]
        public string deviceSesionId { get; set; }

        [Required]
        public string cardNumber { get; set; }

        [Required]
        public string holderName { get; set; }

        [Required]
        public string brand { get; set; }
    }


    public class RequestOpenPayListadoTarjetas : AuthorizedRequestBase
    {

    }


    public class RequestOpenPayRegistrarCargo : AuthorizedRequestBase
    {
        [Required]
        public string sourceId { get; set; }
        [Required]
        public string deviceSesionId { get; set; }

        public string descripcion { get; set; }

        public decimal monto { get; set; }

        public string ordenId { get; set; }


    }


    public class RequestOpenPayEliminarTarjeta : AuthorizedRequestBase
    {
        [Required]
        public string tokenId { get; set; }
    }
}