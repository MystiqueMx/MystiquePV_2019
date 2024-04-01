using System;
using System.Linq;
using System.Web.Http;
using System.Web;
using WebApi.OutputCache.V2;
using FacturacionTDCAPI.Models.Responses.Catalogos;
using System.Net;
using System.IO;
using System.Text;
using FacturacionTDCAPI.Providers;
using FacturacionTDCAPI.Models.Requests.Respuesta;
using Newtonsoft.Json;
using System.Diagnostics;
using FacturacionTDCAPI.DAL;

namespace FacturacionTDCAPI.Controllers
{
    [RoutePrefix("api/v1/TiempoAire")]
    [CacheOutput(ClientTimeSpan = 60 * 60 * 12, ServerTimeSpan = 0)]
    public class TiempoAireController : BaseApiController
    {
        ///Realiza la compra de tiempo aire
        [HttpGet]
        [Route("PruebaRecarga")]      
        public object PruebaRecarga(string sku, string telefono)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://servicios.grupoeco.com.mx/CalidadTransac/api/ApiTransac/recarga");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                Respuesta respuesta;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    StringBuilder token = new StringBuilder();
                    token.Append(FacturacionProvider.GenerarToken());

                    ////Objeto Prueba

                    string objeto = "{\"sku\":\"" + sku + "\"," +
                                  "\"telefono\":\"" + telefono + "\"" +
                                  "}";


                    string json = "{\"satelite\": \"1\"," +
                                  "\"sucursal\":\"1\"," +
                                 "\"operacion\":\"4\"," +
                                 "\"token\":\"" + token + "\"," +
                                 "\"transaccion\":" + objeto + "" +
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

                InsertBitacoraServicio(respuesta.solicitud, result);

                return respuesta;
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

        private static void InsertBitacoraServicio(int? solicitud, string result)
        {
            #region InsertBitacoraServicio
            var logger = NLog.LogManager.GetCurrentClassLogger();

            using (var contexto = new FacturacionTDCDevEntities())
            {
                try
                {
                    // Agregar bitacora de facturacion
                    contexto.BitacoraServicios.Add(new BitacoraServicio()
                    {
                        tipoServicioId = 1,
                        solicitud = solicitud,
                        mensajeRespuesta = result,
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
            }
            #endregion
        }

        //Realiza la consulta del estatus de la compra de tiempo aire
        //Se debe enviar el la solicitud de transacción con la cual se realizó la recarga.
        [HttpGet]
        [Route("PruebaRecargaConsulta")]
        public object PruebaRecargaConsulta(long solicitud)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://servicios.grupoeco.com.mx/CalidadTransac/api/ApiTransac/recargaConsulta");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                Respuesta respuesta;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    StringBuilder token = new StringBuilder();
                    token.Append(FacturacionProvider.GenerarToken());

              

                    string objeto = "{\"solicitud\":\"" + solicitud.ToString() + "\"}";

                    string json = "{\"satelite\": \"1\"," +
                                  "\"sucursal\":\"1\"," +
                                  "\"operacion\":\"5\"," +
                                  "\"token\":\"" + token + "\"," +
                                  "\"transaccion\":" + objeto + "" +
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
                
                return respuesta;
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
    }
}