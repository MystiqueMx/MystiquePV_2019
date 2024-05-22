using System;
using System.Data;
using System.Data.Entity;
using System.Text;
using Hangfire;
using MystiqueMC.DAL;
using ApiDoc.Helpers.Facturacion.Models.Requests.Restaurante;
using ApiDoc.Helpers.Pdf;
using ApiDoc.Helpers.Pdf.Helpers;

namespace ApiDoc.Helpers.Facturacion
{
    public static class PostFacturacionProvider
    {
        //private const string PathFacturasPdf = @"Facturas";
        //private const string PathTemplatePdf = @"Views\\Shared\\TemplateFacturas.cshtml";
        //[AutomaticRetry(Attempts = 5)]

        //public static void Procesar(Factura factura, string directorioAplicacion)
        //{
        //    factura.Id = Guid.Parse(factura.SerializedId);
        //    var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //    try
        //    {
        //        using (var contexto = new MystiqueMeEntities())
        //        {
        //            var dbFacturaCliente = contexto.facturaCliente.Find(factura.Id);
        //            if(dbFacturaCliente == null) throw new DataException(factura.Id.ToString());

        //            if (string.IsNullOrEmpty(dbFacturaCliente.archivoPdfRuta))
        //            {
        //                logger.Debug($"IdFactura : {factura.Id}, Generando PDF");
        //                var rutaPdf = GenerarPdf(factura, dbFacturaCliente.cadenaOriginal, dbFacturaCliente.documentoTimbrado, directorioAplicacion);
        //                logger.Debug($"IdFactura : {factura.Id}, Pdf Generado : `{rutaPdf}`");

        //                if (string.IsNullOrEmpty(rutaPdf)) throw new ApplicationException();
        //                dbFacturaCliente.archivoPdfRuta = rutaPdf;
        //            }

        //            if (string.IsNullOrEmpty(dbFacturaCliente.archivoXmlRuta))
        //            {
        //                logger.Debug($"IdFactura : {factura.Id}, Generando XML");
        //                var rutaXml = GenerarXml(factura, dbFacturaCliente.cadenaOriginal, dbFacturaCliente.documentoTimbrado, directorioAplicacion);
        //                logger.Debug($"IdFactura : {factura.Id}, Xml Generado : `{rutaXml}`");

        //                if (string.IsNullOrEmpty(rutaXml)) throw new ApplicationException();

                    
        //                dbFacturaCliente.archivoXmlRuta = rutaXml;
        //            }
                    
        //            contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
        //            contexto.SaveChanges();

        //            BackgroundJob.Enqueue(() => EnvioPostFacturacionProvider.EnviarFactura(dbFacturaCliente.idFacturaClienteId, directorioAplicacion, false));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e);
        //        throw;
        //    }
        //}
        //public static string GenerarPdf(Factura factura, string cadenaOriginal, string documentoTimbrado, string directorioAplicacion)
        //{
        //    var helper = new GeneracionPdfHelper();
        //    var pdfFactura = helper.ObtenerViewModelFactura(documentoTimbrado, cadenaOriginal);
        //    pdfFactura.DireccionEmisor = factura.Emisor.Direccion;
        //    pdfFactura.DireccionReceptor = factura.Receptor.Direccion;
        //    pdfFactura.CPEmisor = factura.Emisor.CodigoPostal;
        //    pdfFactura.CPReceptor = factura.Receptor.CodigoPostal;
        //    pdfFactura.CantidadLetra = factura.Consumo.ImporteLetra;
        //    pdfFactura.Regimen = factura.DescripcionRegimenFiscal;
        //    pdfFactura.UsoCFDI = factura.DescripcionUsoCFDI;

        //    var codigoBidimensional = helper.GenerarCodigoBidimensional(pdfFactura);
        //    pdfFactura.QR = helper.GenerarImagenQR(codigoBidimensional);

        //    var generador = new GeneracionPdfDelegate(directorioAplicacion, $"{PathFacturasPdf}\\{factura.Emisor.RFC}", $"{directorioAplicacion}{PathTemplatePdf}" );
        //    var pdf = generador.GenerarPdfFactura(pdfFactura);
        //    return pdf;
        //}
        //public static string GenerarXml(Factura factura, string cadenaOriginal, string documentoTimbrado, string directorioAplicacion)
        //{
        //    var outputRelativePath = $"{PathFacturasPdf}\\{factura.Emisor.RFC}\\Xml";
        //    var outputAbsolutePath = $"{directorioAplicacion}{outputRelativePath}";

        //    var helper = new GeneracionPdfHelper();
        //    var pdfFactura = helper.ObtenerViewModelFactura(documentoTimbrado, cadenaOriginal);
            
        //    if (!System.IO.Directory.Exists(outputAbsolutePath))
        //    {
        //        System.IO.Directory.CreateDirectory(outputAbsolutePath);
        //    }

        //    var xml = $"{outputAbsolutePath}\\{pdfFactura.UUID}.xml";
        //    var xmlContent = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{documentoTimbrado}";
        //    System.IO.File.WriteAllText(xml, xmlContent, Encoding.UTF8);
        //    return $"{outputRelativePath}\\{pdfFactura.UUID}.xml";
        //}
    }
}
