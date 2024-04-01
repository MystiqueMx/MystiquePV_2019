using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestCanjePuntos : RequestBase
    {
        public int membresiaId { get; set; }
        public int comercioId { get; set; }
    }


    public class RequestRegistrarCanjePuntos : RequestBase
    {
        public int membresiaId { get; set; }
        public int recompensaId { get; set; }
        public string playerId { get; set; }
        public string deviceId { get; set; }

    }


    public class RequestCanjePuntosValidarCupon : RequestBase
    {
        public string codigoCupon { get; set; }
    }

    public class RequestCanjeRecompensa : RequestBase
    {
        public string codigoCupon { get; set; }
        public int sucursalId { get; set; }
        public int canjepuntoId { get; set; }
        public string folioCompra { get; set; }
        public decimal? montoCompra { get; set; }
    }

    public class RequesListaCupones : RequestBase
    {
        public int membresiaId { get; set; }
        public int comercioId { get; set; }
    }


    public class RequestEliminarRecompensa : RequestBase
    {

        public int canjepuntoId { get; set; }
        public int membresiaId { get; set; }

    }
}