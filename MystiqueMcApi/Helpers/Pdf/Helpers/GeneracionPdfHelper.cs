using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MystiqueMcApi.Helpers.Pdf.ViewModels;
using QRCoder;

namespace MystiqueMcApi.Helpers.Pdf.Helpers
{
    public class GeneracionPdfHelper
    {
        public PdfFactura ObtenerViewModelFactura(string documentoTimbrado, string cadenaOriginal)
        {
            var datosFactura = LlenarFactura(documentoTimbrado);
            datosFactura.CadenaDigitalSAT = cadenaOriginal;
            return datosFactura;
        }

        public string GenerarCodigoBidimensional(PdfFactura datos)
        {
            if(string.IsNullOrEmpty(datos.UUID)
               ||string.IsNullOrEmpty(datos.RFCEmisor)
               ||string.IsNullOrEmpty(datos.RFCReceptor)
               ||string.IsNullOrEmpty(datos.Total))
                throw new ArgumentNullException(nameof(datos));
            #region Generar Cadena
            return $"?&id={datos.UUID}&re={datos.RFCEmisor}&rr={datos.RFCReceptor}&tt={datos.Total}";
            #endregion
        }

        public string GenerarImagenQR(string qrcode, string path = "")
        {
            #region Generar imagen QR
            var ruta = string.IsNullOrEmpty(path) ? Path.GetTempPath() : path;
            var result = $"{Guid.NewGuid()}.png";
            var code = qrcode;
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);

            var imgBarCode = new System.Web.UI.WebControls.Image
            {
                Height = 25,
                Width = 25
            };

            #region Guardar Imagen QR

            using (var bitMap = qrCode.GetGraphic(5))
            {
                using (var ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    var byteImage = ms.ToArray();
                    var img = System.Drawing.Image.FromStream(ms);
                    img.Save($"{ruta}{result}");
                    imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }

            }
            #endregion

            return result;

            #endregion

        }
        #region Helpers
        
        private static PdfFactura LlenarFactura(string xml)
        {
            #region Llenar Factura
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return new PdfFactura
            {
                Folio = GetFirstValue(doc, "cfdi:Comprobante", "Folio"),
                UUID = GetFirstValue(doc, "tfd:TimbreFiscalDigital", "UUID"),
                FechaEmision = GetFirstValue(doc, "cfdi:Comprobante", "Fecha"),
                NombreEmisor = GetFirstValue(doc, "cfdi:Emisor", "Nombre"),
                RFCEmisor = GetFirstValue(doc, "cfdi:Emisor", "Rfc"),
                Regimen = GetFirstValue(doc, "cfdi:Emisor", "RegimenFiscal"),
                //DireccionEmisor = GetFirstValue(doc, "", ""),
                //CPEmisor = GetFirstValue(doc, "cfdi:Emisor", ""),
                NombreReceptor = GetFirstValue(doc, "cfdi:Receptor", "Nombre"),
                UsoCFDI = GetFirstValue(doc, "cfdi:Receptor", "UsoCFDI"),
                RFCReceptor = GetFirstValue(doc, "cfdi:Receptor", "Rfc"),
                //DireccionReceptor = GetFirstValue(doc, "", ""),
                MetodoPago = GetFirstValue(doc, "cfdi:Comprobante", "MetodoPago"),
                SubTotal = GetFirstValue(doc, "cfdi:Comprobante", "SubTotal"),
                IVATrasladado = GetFirstValue(doc, "cfdi:Impuestos", "TotalImpuestosTrasladados"),
                Total = GetFirstValue(doc, "cfdi:Comprobante", "Total"),
                TipoPago = GetFirstValue(doc, "cfdi:Comprobante", "FormaPago"),
                CondicionesDePago = GetFirstValue(doc, "cfdi:Comprobante", "CondicionesDePago"),
                CantidadLetra = GetFirstValue(doc, "", ""),
                SelloDigital = GetFirstValue(doc, "tfd:TimbreFiscalDigital", "SelloCFD"),
                NoCertificadoSAT = GetFirstValue(doc, "tfd:TimbreFiscalDigital", "NoCertificadoSAT"),
                //CadenaDigitalSAT = GetFirstValue(doc, "", ""),
                //QR = GetFirstValue(doc, "", ""),
                SelloSAT = GetFirstValue(doc, "tfd:TimbreFiscalDigital", "SelloSAT"),
                ListaConceptos = GetConceptos(doc, "cfdi:Concepto"),
                //CPReceptor = GetFirstValue(doc, "", ""),


            };
            #endregion
        }
        private static string GetFirstValue(XmlDocument doc, string tag, string atributo)
        {
            #region GetFirstValue
            var valor = string.Empty;
            try
            {
                var xmlAttributeCollection = doc.GetElementsByTagName(tag)[0].Attributes;
                if (xmlAttributeCollection != null)
                    valor = xmlAttributeCollection[atributo].Value;
            }
            catch (Exception)
            {
                return null;
            }
            return valor;

            #endregion
        }
        private static List<PdfFactura.Conceptos> GetConceptos(XmlDocument doc, string v)
        {
            #region GetValues
            var lista = new List<PdfFactura.Conceptos>();
            var valor = string.Empty;
            try
            {
                var xmlElements = doc.GetElementsByTagName(v);
                for (var i = 0; i <= xmlElements.Count - 1; i++)
                {
                    var xmlAttributeCollection = xmlElements[i].Attributes;
                    if (xmlAttributeCollection == null) continue;
                    var dato =
                        new PdfFactura.Conceptos
                        {
                            Cantidad = xmlAttributeCollection["Cantidad"].Value,
                            Clave = xmlAttributeCollection["ClaveProdServ"].Value,
                            Descripcion = xmlAttributeCollection["Descripcion"].Value,
                            ValorUnitario = xmlAttributeCollection["ValorUnitario"].Value,
                            UM = xmlAttributeCollection["Unidad"].Value,
                            Importe = xmlAttributeCollection["Importe"].Value
                        };


                    lista.Add(dato);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex}");
                return null;
            }
            return lista;

            #endregion
        }
        
        #endregion
    }
}