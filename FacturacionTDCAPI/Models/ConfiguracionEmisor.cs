using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facturacion.v33;

namespace FacturacionTDCAPI.Models
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
        public string RazonSocial { get; set; }
        public string Rfc { get; set; }
        public c_CodigoPostal CodigoPostal { get; set; }
        public c_RegimenFiscal RegimenFiscal { get; set; }
        public string Direccion { get; set; }

        public string PathRespaldoDocumentos { get; set; }
    }
}