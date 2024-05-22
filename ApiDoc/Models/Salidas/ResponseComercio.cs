using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseComercio
    {
    }
    public class ResponseComercioDistanciaComerciosUsuario : ResponseBase
    {
        public int ComercioId { get; set; }
        public string NombreComercial { get; set; }
        public string Nombre { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int SucursalId { get; set; }
        public string BeneficioDesc { get; set; }
        public string BeneficioTitulo { get; set; }
        public string DireccionComercio { get; set; }
        public string Telefono { get; set; }
        public string urlLogoComercio { get; set; }

    }

    public class ResponseComercioEmpresa 
    {
        public int comercioId { get; set; }
        public string nombreComercial { get; set; }
        public string urlLogoComercio { get; set; }
    }

    public class ResponseListacomercios : ResponseBase
    {
        public List<ResponseComercioEmpresa> listaComercioEmpresa { get; set; }
    }

}