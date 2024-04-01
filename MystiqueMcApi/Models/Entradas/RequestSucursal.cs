using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestSucursal
    {
    }

    public class RequestSucursaComercio : RequestBase
    {
        public int idComercio { get; set; }
    }

    public class RequestHazPedidoSucursal
    {
        [Required]
        public float Latitud { get; set; }

        [Required]
        public float Longitud { get; set; }

        public TiposReparto RestauranteTiposReparto { get; set; } = TiposReparto.Todos;
    }

    public class RequestHazPedidoSucursalActivas
    {


    }
}