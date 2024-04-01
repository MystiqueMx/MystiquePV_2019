using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models
{
    public enum EstatusFactura
    {
        Pendiente = 1,
        Procesando = 2,
        ListaParaEnviar = 3,
        Error = 4,
        Cancelado = 5,
    }
}