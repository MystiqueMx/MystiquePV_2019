
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using System.Web;
using Hangfire;
using System.Data;
using NLog;
using FacturacionTDCAPI.DAL;
using FacturacionTDCAPI.Helpers.PDF.Helpers;
using FacturacionTDCAPI.Helpers.Email;
using FacturacionTDCAPI.Helpers.PDF;
using static FacturacionTDCAPI.Helpers.MontoALetra;

namespace FacturacionTDCAPI.Helpers
{
    public static class GenerarFactura
    {

        private const string PathFacturasPdf = @"Facturas";
        private const string PathTemplatePdf = @"Views\\Shared\\TemplateFacturas.cshtml";

        public static async Task Procesar(long id, string directorioAplicacion, string xmlFile, string overrideEmail = null)
        {
            #region Procesar
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                using (var contexto = new FacturacionTDCDevEntities())
                {
                    var dbFacturaCliente = contexto.ComprobanteFiscals
                        .Include(c => c.ReceptorFactura)
                        .Include(c => c.ConsumoFactura)
                        .Include(c => c.ConfiguracionEmisore)
                        .First(c => c.idComprobanteFiscal == id);

                    var usoCfdi = contexto.CatalogoUsoCfdis
                        .First(c => c.idCatalogoUsoCfdi == dbFacturaCliente.ReceptorFactura.catUsoCfdi);

                    var rutaPdf = string.IsNullOrEmpty(dbFacturaCliente.rutaPdf)
                        ? GenerarPdf(dbFacturaCliente, usoCfdi.descripcion, directorioAplicacion)
                        : dbFacturaCliente.rutaPdf;
                    logger.Debug($"IdFactura : {id}, Pdf Generado : `{rutaPdf}`");

                    if (string.IsNullOrEmpty(rutaPdf)) throw new ApplicationException();
                    if (string.IsNullOrEmpty(dbFacturaCliente.rutaPdf))
                    {
                        dbFacturaCliente.rutaPdf = rutaPdf;
                        contexto.Entry(dbFacturaCliente).State = EntityState.Modified;
                        contexto.SaveChanges();
                    }

                    var correo = string.IsNullOrEmpty(overrideEmail)
                        ? dbFacturaCliente.ReceptorFactura.correo
                        : overrideEmail;

                    if (string.IsNullOrEmpty(correo)) return;

                    var emailDelegate = new SendEmailDelegate();
                    var body = $"Adjunto a este correo se encuentra el comprobante fiscal solicitado el día {dbFacturaCliente.fechaRegistro:dd/MM/yyyy}";
                    await emailDelegate.SendEmail(correo, "Comprobante fiscal", body, files: new[]
                    {
                        directorioAplicacion + rutaPdf,
                        xmlFile
                    });
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            #endregion
        }
        public static string GenerarPdf(ComprobanteFiscal facturaDb, string usoCfdi, string directorioAplicacion)
        {
            #region GenerarPdf
            var helper = new GeneracionPdfHelper();
            var pdfFactura = helper.ObtenerViewModelFactura(facturaDb.documentoTimbrado, facturaDb.cadenaOriginal);
            pdfFactura.DireccionEmisor = facturaDb.ConfiguracionEmisore.direccion;
            pdfFactura.DireccionReceptor = facturaDb.ReceptorFactura.direccion;
            pdfFactura.CPEmisor = facturaDb.ConfiguracionEmisore.codigoPostal.ToString();// Revisar cual debe de llevar
            pdfFactura.CPReceptor = facturaDb.ReceptorFactura.codigoPostal;
            pdfFactura.CantidadLetra = new Numalet { LetraCapital = true, Decimales = 2 }.ToCustomCardinal(facturaDb.ConsumoFactura.total);
            pdfFactura.Regimen = facturaDb.ConfiguracionEmisore.descRegimenFiscal;
            pdfFactura.UsoCFDI = usoCfdi;

            var codigoBidimensional = helper.GenerarCodigoBidimensional(pdfFactura);
            pdfFactura.QR = helper.GenerarImagenQR(codigoBidimensional);

            var generador = new GeneracionPdfDelegate(directorioAplicacion, $"{PathFacturasPdf}\\{facturaDb.ConfiguracionEmisore.rfc}", $"{directorioAplicacion}{PathTemplatePdf}");
            var pdf = generador.GenerarPdfFactura(pdfFactura);
            return pdf;
            #endregion
        }
    }
}