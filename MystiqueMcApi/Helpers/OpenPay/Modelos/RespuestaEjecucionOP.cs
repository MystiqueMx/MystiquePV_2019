using Openpay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Helpers.OpenPay.Modelos
{
    public class RespuestaEjecucionOP
    {


    }


    public class RespuestaEjecucionRegistraClienteOP : BaseRespuestaOpenPay
    {

        public string customerId { get; set; }


    }
    public class RespuestaEjecucionCrearTarjetaToken : BaseRespuestaOpenPay
    {
        public Card datosTarjeta { get; set; }
    }



    public class RespuestaEjecucionListadoTarjeta : BaseRespuestaOpenPay
    {
        public List<Card> listaTarjetas { get; set; }
    }


    public class RespuestaEjecucionAplicarCargo : BaseRespuestaOpenPay
    {

    }

    public class RespuestaEjecucionEliminarTarjeta : BaseRespuestaOpenPay
    {

    }
}