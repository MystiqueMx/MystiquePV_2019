using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestBeneficio : RequestBase
    {
    }


    public class RequestObtenerBeneficioNivelMembresia : RequestBase
    {
        public int nivelMembresia { get; set; }

    }

    public class RequestObtenerBeneficioSucursal : RequestBase
    {
        public int sucursalId { get; set; }
        public int clienteId { get; set; }
        public int membresiaId { get; set; }


    }


    public class RequestObtenerBeneficioDetalle : RequestBase
    {
        public int beneficioId { get; set; }
        public int sucursalId { get; set; }
        public int clienteId { get; set; }

    }


    public class RequestZonaCitySalads : RequestBase
    {
        public long idUsuario { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }

    }



    public class RequestInsertarBeneficioCliente : RequestBase
    {
        public int beneficioId { get; set; }
        public int sucursalId { get; set; }
        public int clienteId { get; set; }
        public int membresiaId { get; set; }
        public string folioCompra { get; set; }
        public DateTime? fechaCompra { get; set; }
        public decimal? montoCompra { get; set; }

    }
}