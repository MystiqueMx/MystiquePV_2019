
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Facturacion.v33;
using FacturacionTDCAPI.Helpers.Sat;
using System.Diagnostics;
using FacturacionTDCAPI.Models;
using System.Data;
using FacturacionTDCAPI.Helpers.FileUpload;
using System.Xml;
using FacturacionTDCAPI.DAL;
using System.Net;
using System.IO;
using FacturacionTDCAPI.Helpers.Criptografia;
using FacturacionTDCAPI.Helpers;
using Hangfire;
using Hangfire.SqlServer;
using Newtonsoft.Json;
using FacturacionTDCAPI.Models.Requests.Respuesta;
using FacturacionTDCAPI.Models.Cancelaciones;
using System.ServiceModel;

namespace FacturacionTDCAPI.Providers
{
    public class FacturacionProvider
    {
        private static readonly string ServicioGrupoEcoFacturacion = ConfigurationManager.AppSettings.Get("URL_FACTURACION");
        private static readonly string ServicioGrupoEcoCancelacion = ConfigurationManager.AppSettings.Get("URL_CANCELACION");
        private const string PathXslt = "Data/Sat/cadenaoriginal_3_3.xslt";
        /// <summary>
        /// Proceso de facturacion -> PAX Facturacion
        /// </summary>
        /// <param name="idFactura">Referencia de la factura</param>
        /// <param name="comprobante">Comprobante fiscal a facturar</param>
        /// <param name="configuracion">Configuracion de certificados y pax facturacion</param>
        /// <param name="serverPath">Ruta del servidor, para respaldo de archivos</param>
        /// <returns></returns>
        internal static async Task<ComprobanteFiscal> SolicitarFactura(long idFactura, Comprobante comprobante, ConfiguracionEmisor configuracion, string serverPath)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                string cadenaOriginal;
                logger.Debug("Inicio solicitud factura >>> IdFactura : {0}, comprobante: {1}", idFactura, comprobante.Folio);
                // Agrega certificados del emisor
            /*    logger.Debug("Inicio carga certificados >>> IdFactura : {0}, PathCertificado{1}", idFactura, configuracion.PathCertificado);
                using (var delegateCertificados = new DelegateSslCertificates(configuracion.PathCertificado))
                {
                    comprobante.NoCertificado = delegateCertificados.ObtenerNumeroCertificado();
                    comprobante.Certificado = delegateCertificados.ObtenerCertificadoComoString();
                }
                logger.Debug("Se agrego número de certificado >>> IdFactura : {0}, NoCertificado: {1}, Certificado: {2}", idFactura, comprobante.NoCertificado, comprobante.Certificado);
               
                // Agrega sello digital ( Ultimo paso antes de enviar )
                logger.Debug("Agrega sello digital >>> IdFactura : {0}", idFactura);
                using (var delegateRsaHash = new DelegateRsaHashing(configuracion.PathLlavePrivada,
                    configuracion.PasswordLlavePrivada, configuracion.PathCertificado))
                {
                    cadenaOriginal = comprobante.ToXmlCadenaOriginal($"{serverPath}{PathXslt}");
                    logger.Debug("Genero cadena original >>> IdFactura : {0}, cadena : {1}", idFactura, cadenaOriginal);
                    var selloDigital = delegateRsaHash.FirmarConLlavePrivada(cadenaOriginal);
                    logger.Debug("Genero sello digital >>> IdFactura : {0}, sello : {1}", idFactura, selloDigital);
                    comprobante.Sello = selloDigital;
                }
                logger.Debug("Se agrego sello digital >>> IdFactura : {0}", idFactura);
                // Solicita timbrado
                
                logger.Debug("Solicitud de timbrado  >>> IdFactura : {0}, enpoint address: {1}", idFactura, ServicioGrupoEcoFacturacion);
                */
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ServicioGrupoEcoFacturacion);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                string bytesStr;
                string xmlEnvio;
                Respuesta respuesta;
                string factura;
                //Archivo
                var xml = comprobante.ToXml();
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    
                    cadenaOriginal = xml;
                    xmlEnvio = xml.Replace("xmlns:schemaLocation", "xsi:schemaLocation");
                    //Archivo base string 64
                    bytesStr = Base64Encode(xmlEnvio);

                    StringBuilder objeto = new StringBuilder();
                    objeto.Append(bytesStr);

                    //Token de satelite
                    StringBuilder token = new StringBuilder();
                    var tokenG = GenerarToken();
                    token.Append(tokenG);

                    string json = "{\"satelite\": \"1\"," +             //Id de satelite
                                  "\"operacion\":\"1\"," +              //Id de transacción
                                  "\"token\":\"" + token + "\"," +      //Token de satelite
                                  "\"transaccion\":\"" + objeto + "\"" +//Archivo
                                  "}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                string result;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();

                }
                respuesta = JsonConvert.DeserializeObject<Respuesta>(result);
                //factura = xmlEnvio.ToString();
                // Si la respuesta contiene timbre fue exitosa la facturacion

                var timbre = ObtenerTimbre(respuesta.objeto.ToString());

                //factura = response.Body.fnEnviarXMLResult;



                factura = respuesta.objeto.ToString();
                //se agrego para pruebas de la factura que se generara en pdf
                // var timbre = "5291dc3c-b449-4cc5-b665-dd81384bc126";


                // Actualizar estatus segun resultado de facturacion
                if (string.IsNullOrEmpty(timbre))
                {
                    logger.Error("Error en la solicitud de factura >>> IdFactura : {0}, Pax no agrego timbrado", idFactura);
                    return ActualizarEstatusErrorFacturacion(idFactura, factura);
                }
                else
                {
                    logger.Info("Factura generada con exito, respaldando >>> IdFactura : {0}, factura : {1}", idFactura, factura);
                    var rutaRespaldo = RespaldarFacturaADisco(timbre, factura, serverPath,
                        configuracion.PathRespaldoDocumentos);
                    logger.Info("Factura respaldada con exito, retornando >>> IdFactura : {0}, factura : {1}, uuid : {2}, rutaRespaldo: {3}, serverPath: {4}", idFactura, factura, timbre, rutaRespaldo, serverPath);
                    return ActualizarEstatusFacturacionExitosa(idFactura, factura, cadenaOriginal, timbre, rutaRespaldo, serverPath);
                }
            }
            catch (Exception e)
            {
                #if DEBUG
                Trace.WriteLine(e);
                #endif
                logger.Error(e);
                return null;
            }
        }

        #region token   
        public static string GenerarToken()
        {
            GeneraToken.Token token = new GeneraToken.Token();
            string tokenTransaccion = token.Generar("8A64DE2B-AE54-4D1C-A40F-6C2F028D536C", "1", "");
            return tokenTransaccion;
        }
        #endregion

        #region proceso de cancelacion
        internal static bool SolicitarCancelacion(long idFactura, ConfiguracionEmisor configuracion)
        {
            #region SolicitarCancelacion
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Debug("Inicio solicitud cancelacion factura >>> IdFactura : {0}, ConfiguracionEmisor: {1}", idFactura, configuracion);
                var factura = ObtenerDatosCancelacionPorIdFactura(idFactura, configuracion.Rfc);

                logger.Debug("Factura encontrada >>> Factura : {0}", factura);


                logger.Debug("Solicitud de cancelacion  >>> IdFactura : {0}, enpoint address: {1}", idFactura);
                var cancelacion = "";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ServicioGrupoEcoCancelacion);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string uuid = factura.Uuid;   //Folio fiscal
                    string Emisor = configuracion.Rfc;                         //RFC Emisor
                    string Receptor = factura.RfcReceptor;                       //RFC Receptor
                    string total = factura.Total.ToString();                                 //Importe total facturado

                    //Token de satelite
                    StringBuilder token = new StringBuilder();
                    var tokenG = GenerarToken();
                    token.Append(tokenG);


                    string json = "{\"satelite\": \"1\"," +                             //Id Satelite
                                  "\"operacion\":\"2\"," +                              //Id de transacción
                                  "\"token\":\"" + token + "\"," +                      //Token de satelite
                                  "\"transaccion\":{ \"UUID\":\"" + uuid +              //Folio fiscal
                                                "\", \"rfcEmisor\":\"" + Emisor +       //RFC Emisor
                                                "\", \"rfcReceptor\":\"" + Receptor +   //RFC Receptor
                                                "\", \"total\":\"" + total + "\"}" +    //Importe total facturado
                                  "}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var resultado = String.Empty;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resultado = streamReader.ReadToEnd();
                }
                
                logger.Debug("Respuesta de cancelacion  >>> Response : {0}", resultado);

               var cancelacionCorrecta = ObtenerEstaCancelada(cancelacion);
               logger.Debug("Pudo ser cancelada?  >>> {0}", cancelacionCorrecta);

               ActualizarEstatusCancelada(idFactura, cancelacionCorrecta, cancelacion);

               return true;
            }
            catch (Exception e)
            {
            #if DEBUG
                Trace.WriteLine(e);
            #endif
                logger.Error(e);
                return false;
            }
            #endregion
        }

        private static bool ObtenerEstaCancelada(string cancelacion)
        {
            #region ObtenerEstaCancelada
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                XmlDocument docXml = new XmlDocument();
                docXml.LoadXml(cancelacion);
                XmlNode nodoComplemento = docXml.ChildNodes[0];
                string folios = "No es posible cancelar los siguientes folios ante el SAT:";

                var foliosXml = new List<dynamic>();
                foreach (XmlNode item in nodoComplemento.ChildNodes)
                {

                    if (item["UUID"] != null)
                    {
                        var folioXml = new
                        {
                            FolioID = item["UUID"].InnerText,
                            Estatus = item["UUIDEstatus"].InnerText.Trim(),
                            Descripcion = item["UUIDdescripcion"].InnerText
                        };
                        foliosXml.Add(folioXml);
                    }
                }

                var estaCancelada = true;
                List<string> facturasCancelarActualizado = new List<string>();
                foreach (var folio in foliosXml)
                {
                    logger.Debug("{0}  {1} : {2}", folio.FolioID, folio.Estatus, folio.Descripcion);
                    if (folio == null)
                    {
                        estaCancelada = false;
                    }
                    else if (folio.Estatus == "201" || folio.Estatus == "202" || folio.Estatus == "103" || folio.Estatus == "107")
                    {
                        estaCancelada = true;
                    }
                    else
                    {
                        estaCancelada = false;
                    }
                }
                return estaCancelada;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
            #endregion
        }

        private static FacturaCancelacion ObtenerDatosCancelacionPorIdFactura(long id, string emisor)
        {
            #region ObtenerDatosCancelacionPorIdFactura
            using (var contexto = new FacturacionTDCDevEntities())
            {
                var comprobante = contexto.ComprobanteFiscals
                    .Include("ConfiguracionEmisore")
                    .Include("ReceptorFactura")
                    .First(c => c.idComprobanteFiscal == id
                        && c.ConfiguracionEmisore.rfc == emisor
                        && c.documentoTimbrado != "");
                return new FacturaCancelacion
                {
                    Uuid = comprobante.uuid.ToString(),
                    Total = comprobante.ConsumoFactura.total,
                    RfcReceptor = comprobante.ReceptorFactura.rfc,
                };
            }
            #endregion
        }

        #endregion

        public static string Base64Encode(string xml)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(xml);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static ComprobanteFiscal ActualizarEstatusErrorFacturacion(long idFactura, string factura)
        {
            #region ActualizarEstatusErrorFacturacion
            var logger = NLog.LogManager.GetCurrentClassLogger();

            using (var contexto = new FacturacionTDCDevEntities())
            {
                try
                {
                    // Agregar bitacora de facturacion
                    contexto.BitacoraPaxFacturacions.Add(new BitacoraPaxFacturacion()
                    {
                        mensajeRespuesta = factura,
                        comprobanteFiscalId = idFactura,
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
                if (contexto.ComprobanteFiscals
                    .FirstOrDefault(c => c.idComprobanteFiscal == idFactura) is ComprobanteFiscal facturaDb)
                {
                    facturaDb.catEstatusComprobanteId = (int)EstatusFactura.ErrorFacturacion;
                    contexto.SaveChanges();
                    return facturaDb;
                }
                else
                {
                    throw new DataException(idFactura.ToString());
                }
            }
            #endregion
        }
        private static ComprobanteFiscal ActualizarEstatusFacturacionExitosa(long idFactura, string factura, string cadenaOriginal, string uuid, string rutaRespaldo, string serverPath)
        {
            #region ActualizarEstatusFacturacionExitosa
            var logger = NLog.LogManager.GetCurrentClassLogger();

            // Actualizar el estatus en BD
            using (var contexto = new FacturacionTDCDevEntities())
            {
                try
                {
                    // Agregar bitacora de facturacion
                    contexto.BitacoraPaxFacturacions.Add(new BitacoraPaxFacturacion
                    {
                        mensajeRespuesta = factura,
                        comprobanteFiscalId = idFactura,
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

                var facturaDb = contexto.ComprobanteFiscals
                     .Include("ReceptorFactura")
                     .Include("ConfiguracionEmisore")
                     .Include("BitacoraPaxFacturacions")
                     .First(c => c.idComprobanteFiscal == idFactura);

                facturaDb.catEstatusComprobanteId = (int)EstatusFactura.Facturada;
                facturaDb.documentoTimbrado = factura;
                facturaDb.rutaXml = rutaRespaldo;
                facturaDb.cadenaOriginal = cadenaOriginal;
                facturaDb.uuid = Guid.Parse(uuid);
                contexto.SaveChanges();

                try
                {
                    Hangfire.BackgroundJob.Enqueue(() => GenerarFactura.Procesar(facturaDb.idComprobanteFiscal, serverPath, serverPath + rutaRespaldo, null));
                }
                catch (Exception ex1)
                {
                    logger.Error(ex1);
                }
                return facturaDb;
            }
        }
        private static string RespaldarFacturaADisco(string uuid, string factura, string serverPath, string configuracionPathRespaldoDocumentos)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            var pathFacturaDisco = string.Empty;
            // Respalda factura
            try
            {
                var filesUpload = new FilesUploadDelegate();
                pathFacturaDisco = filesUpload.BackupCadenaOriginalToFile(factura, serverPath,
                    configuracionPathRespaldoDocumentos + FilesHelper.OriginalesFacturasPath, uuid, ".xml");
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.WriteLine(e);
#endif
                logger.Error(e);
            }

            return pathFacturaDisco;
            #endregion
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

        private static void ActualizarEstatusCancelada(long idFactura, bool cancelada, string response)
        {
            #region ActualizarEstatusCancelada
            var logger = NLog.LogManager.GetCurrentClassLogger();

            using (var contexto = new FacturacionTDCDevEntities())
            {
                try
                {
                    // Agregar bitacora de facturacion
                    contexto.BitacoraPaxFacturacions.Add(new BitacoraPaxFacturacion()
                    {
                        mensajeRespuesta = response,
                        comprobanteFiscalId = idFactura,
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
                if (cancelada)
                {
                    // Actualizar estatus de la factura
                    if (contexto.ComprobanteFiscals.Find(idFactura) is ComprobanteFiscal facturaDb)
                    {
                        facturaDb.catEstatusComprobanteId = (int)EstatusFactura.Cancelada;
                        contexto.SaveChanges();
                    }
                    else
                    {
                        throw new DataException(idFactura.ToString());
                    }
                }

            }
            #endregion
        }
    }
}

