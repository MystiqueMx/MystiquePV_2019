using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class AuthorizedRequestBase
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int consumidorId { get; set; }
    }
}