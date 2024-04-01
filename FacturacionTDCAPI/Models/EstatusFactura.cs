using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models
{
    public enum EstatusFactura
    {
        // Flujo normal de facturacion
        Recibida = 1,
        Facturada = 2,
        EnviadaPorCorreo = 3,
        // Estatus de error
        ErrorFacturacion = 4,
        ErrorEnvioPorCorreo = 5,

        // Cancelaciones
        Cancelada = 6,
    }
}