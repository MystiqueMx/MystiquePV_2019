using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestMenu
    {
        [Required]
        public int sucursalId { get; set; }
    }

    public class RequestMenuEnsalada
    {
        [Required]
        public int menuId { get; set; }
    }

    public class RequestPlatillos
    {
        [Required]
        public int menuId { get; set; }

        [Required]
        public int sucursalId { get; set; }
    }
}