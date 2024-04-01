using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestLogistica
    {
        public int sucursalId { get; set; }
    }

    public class RequestLogisticaPedidos
    {
        public int sucursalId { get; set; }
    }

    public class RequestLogisticaActualizarEstatus
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int pedidoId { get; set; }

        [Required]
        public string nombreUsuarioActualizo { get; set; }

        public int sucursalId { get; set; }

        public int usuarioId { get; set; }
    }
}