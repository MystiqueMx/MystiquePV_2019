using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models.Sucursal
{
    public enum EstatusDatosFiscales
    {
        [Description("No registrados")]
        Vacio = 0,
        [Description("Datos cargados anteriormente")]
        Catalogo = 1,
        [Description("Nuevos datos fiscales")]
        Nuevos = 2,
    }
}