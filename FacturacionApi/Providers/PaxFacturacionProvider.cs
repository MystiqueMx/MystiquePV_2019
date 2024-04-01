using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using FacturacionApi.Data.Sat.Catalogos;
using FacturacionApi.DAL;
using FacturacionApi.Helpers.Criptografia;
using FacturacionApi.Helpers.Sat;
using FacturacionApi.Models;
using FacturacionApi.Models.Facturacion;
using FacturacionApi.PaxFacturacion;
using MystiqueMC.Helpers.FileUpload;
using Newtonsoft.Json;

namespace FacturacionApi.Providers
{
    public static class PaxFacturacionProvider
    {
        private static readonly string ServicioPaxFacturacion = ConfigurationManager.AppSettings.Get("URL_PAX_FACTURACION");
        private const string PathXslt = "Data/Sat/cadenaoriginal_3_3.xslt";
        /// <summary>
        /// Proceso de facturacion -> PAX Facturacion
        /// </summary>
        /// <param name="idFactura">Referencia de la factura</param>
        /// <param name="comprobante">Comprobante fiscal a facturar</param>
        /// <param name="configuracion">Configuracion de certificados y pax facturacion</param>
        /// <param name="serverPath">Ruta del servidor, para respaldo de archivos</param>
        /// <returns></returns>
        internal static Task SolicitarFactura(Guid idFactura, Comprobante comprobante, ConfiguracionEmisor configuracion, string serverPath)
        {
            return Task.Factory.StartNew(async () =>
            {
                var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                try
                {
                    string cadenaOriginal;
                    logger.Debug(
                        $"IdFactura : {idFactura}, comprobante: { comprobante.Folio }");
                    // Agrega certificados del emisor

                    using (var delegateCertificados = new DelegateSslCertificates(configuracion.PathCertificado))
                    {
                        comprobante.NoCertificado = delegateCertificados.ObtenerNumeroCertificado();
                        comprobante.Certificado = delegateCertificados.ObtenerCertificadoComoString();
                    }
                    // Agrega sello digital ( Ultimo paso antes de enviar )
                    using (var delegateRsaHash = new DelegateRsaHashing(configuracion.PathLlavePrivada, configuracion.PasswordLlavePrivada, configuracion.PathCertificado))
                    {
                        cadenaOriginal = comprobante.ToXmlCadenaOriginal($"{serverPath}{PathXslt}");
                        var selloDigital = delegateRsaHash.FirmarConLlavePrivada(cadenaOriginal);
                        comprobante.Sello = selloDigital;
                    }

                    // Solicita timbrado
                    string factura;
                    using (var clientePax = new wcfRecepcionASMXSoapClient(HttpBindingPaxFacturacion, new EndpointAddress(ServicioPaxFacturacion)))
                    {
                        factura = clientePax.fnEnviarXML(comprobante.ToXml(),
                            configuracion.TipoDocumentoPaxFacturacion, configuracion.IdEstructuraPaxFacturacion,
                            configuracion.UsuarioPaxFacturacion, configuracion.PasswordPaxFacturacion,
                            configuracion.VersionPaxFacturacion);
                    }
                    // Si la respuesta contiene timbre fue exitosa la facturacion
                    var timbre = ObtenerTimbre(factura);
                    
                    // Actualizar estatus segun resultado de facturacion
                    if (string.IsNullOrEmpty(timbre))
                    {
                        logger.Error($"IdFactura : {idFactura}, mensajeRespuesta: { factura }");
                        ActualizarEstatusErrorFacturacion(idFactura, factura);
                    }
                    else
                    {
                        logger.Debug($"IdFactura : {idFactura}, cadenaOriginalFacturada: {factura}");
                        var rutaRespaldo = RespaldarFacturaADisco(idFactura, factura, serverPath, configuracion.PathRespaldoDocumentos);
                        ActualizarEstatusFacturacionExitosa(idFactura, factura, cadenaOriginal, rutaRespaldo);
                    }
                    
                }
                catch (Exception e)
                {
                    #if DEBUG
                    Trace.WriteLine(e);
                    #endif
                    logger.Error(e);
                }
            });
        }

        private static void ActualizarEstatusFacturacionExitosa(Guid idFactura, string factura, string cadenaOriginal, string rutaRespaldo)
        {
            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            
            // Actualizar el estatus en BD
            using (var contexto = new FacturacionContext())
            {
                try
                {
                    // Agregar bitacora de facturacion
                    contexto.bitacoraPaxFacturacion.Add(new bitacoraPaxFacturacion
                    {
                        mensajeRespuesta = factura,
                        idComprobanteFiscal = idFactura,
                        fechaRegistro = DateTime.Now
                    });
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.WriteLine(ex);
#endif
                    logger.Error(ex);
                }
                        
                // Actualizar estatus de la factura
                if (contexto.facturaComprobanteFiscal.Find(idFactura) is facturaComprobanteFiscal facturaDb)
                {
                    facturaDb.catEstatusComprobanteId = (int)EstatusFactura.Facturada;
                    facturaDb.documentoTimbrado = factura;
                    facturaDb.archivoXmlRuta = rutaRespaldo;
                    facturaDb.cadenaOriginal = cadenaOriginal;
                    contexto.SaveChanges();
                }
                else
                {
                    throw new DataException(idFactura.ToString());
                }
            }
        }
        private static void ActualizarEstatusErrorFacturacion(Guid idFactura, string factura)
        {
            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            
            using (var contexto = new FacturacionContext())
            {
                try
                {
                    // Agregar bitacora de facturacion
                    contexto.bitacoraPaxFacturacion.Add(new bitacoraPaxFacturacion
                    {
                        mensajeRespuesta = factura,
                        idComprobanteFiscal = idFactura,
                        fechaRegistro = DateTime.Now
                    });
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.WriteLine(ex);
#endif
                    logger.Error(ex);
                }
                        
                // Actualizar estatus de la factura
                if (contexto.facturaComprobanteFiscal.Find(idFactura) is facturaComprobanteFiscal facturaDb)
                {
                    facturaDb.catEstatusComprobanteId = (int)EstatusFactura.ErrorFacturacion;
                    contexto.SaveChanges();
                }
                else
                {
                    throw new DataException(idFactura.ToString());
                }
            }
        }
        private static string RespaldarFacturaADisco(Guid idFactura, string factura, string serverPath, string configuracionPathRespaldoDocumentos)
        {
            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var pathFacturaDisco = string.Empty;
            // Respalda factura
            try
            {
                
                var filesUpload = new FilesUploadDelegate();
                pathFacturaDisco = filesUpload.BackupCadenaOriginalToFile(factura, serverPath,
                    configuracionPathRespaldoDocumentos + FilesHelper.OriginalesFacturasPath, idFactura.ToString(),".xml");
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.WriteLine(e);
#endif
                logger.Error(e);
            }

            return pathFacturaDisco;
        }
        private static string ObtenerTimbre(string respuesta)
        {
            #region ObtenerTimbre

            var doc = new XmlDocument();
            var valor = string.Empty;
            try
            {
                doc.LoadXml(respuesta);

                var xmlAttributeCollection = doc.GetElementsByTagName("tfd:TimbreFiscalDigital")[0].Attributes;
                if (xmlAttributeCollection != null)
                    valor = xmlAttributeCollection["UUID"].Value;

            }
            catch (Exception)
            {
                return null;
            }
            return valor;

            #endregion
        }
        private static BasicHttpBinding HttpBindingPaxFacturacion
        {
            get
            {
                var httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                httpBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                httpBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                httpBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                return httpBinding;
            }
        }

    }
}