using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionApi.Models.Facturacion
{
    public class ConfiguracionEmisor
    {
        public string PathCertificado { get; set; }
        public string PathLlavePrivada { get; set; }
        public string PasswordLlavePrivada { get; set; }

        public string SerieFactura { get; set; }
        public string UsuarioPaxFacturacion { get; set; }
        public string PasswordPaxFacturacion { get; set; }
        public string TipoDocumentoPaxFacturacion { get; set; }
        public int IdEstructuraPaxFacturacion { get; set; }
        public string VersionPaxFacturacion { get; set; }


        public string PathRespaldoDocumentos { get; set; }
    }
}