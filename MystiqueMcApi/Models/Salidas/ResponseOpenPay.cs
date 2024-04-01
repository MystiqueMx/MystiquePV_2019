using Openpay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseOpenPay : ErrorObjCodeResponseBase
    {
    }

    public class ResponseListadoTarjetasOP : ErrorObjCodeResponseBase
    {
       public List<Card> respuesta { get; set; }
    }


    public class ResponseAgregarTarjetasOP : ErrorObjCodeResponseBase
    {
        public Card respuesta { get; set; }
    }
}