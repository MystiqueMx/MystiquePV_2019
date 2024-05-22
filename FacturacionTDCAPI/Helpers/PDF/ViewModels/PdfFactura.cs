using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Helpers.PDF.ViewModels
{
    public class PdfFactura
    {
        public List<Conceptos> ListaConceptos { get; set; }
        public string Folio { get; set; }
        public string noCertificado { get; set; }
        public string UUID { get; set; }
        public string FechaEmision { get; set; }
        public string NombreEmisor { get; set; }
        public string RFCEmisor { get; set; }
        public string Regimen { get; set; }
        public string DireccionEmisor { get; set; }
        public string CPEmisor { get; set; }
        public string NombreReceptor { get; set; }
        public string UsoCFDI { get; set; }
        public string RFCReceptor { get; set; }
        public string CPReceptor { get; set; }
        public string DireccionReceptor { get; set; }
        public string MetodoPago { get; set; }
        public string SubTotal { get; set; }
        public string IVATrasladado { get; set; }
        public string Total { get; set; }
        public string TipoPago { get; set; }
        public string CondicionesDePago { get; set; }
        public string CantidadLetra { get; set; }
        public string SelloDigital { get; set; }
        public string SelloSAT { get; set; }
        public string NoCertificadoSAT { get; set; }
        public string CadenaDigitalSAT { get; set; }
        public string QR { get; set; }

        public class Conceptos
        {
            public string Cantidad { get; set; }
            public string CUM { get; set; }
            public string UM { get; set; }
            public string Clave { get; set; }
            public string Descripcion { get; set; }
            public string ValorUnitario { get; set; }
            public string Importe { get; set; }
        }
    }
}