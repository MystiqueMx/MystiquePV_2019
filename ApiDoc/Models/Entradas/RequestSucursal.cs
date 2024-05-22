using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Entradas
{
    public class RequestSucursal
    {
    }

    public class RequestSucursaComercio : RequestBase
    {
        public int idComercio { get; set; }
    }
}