using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ApiDoc.Helpers.Facturacion.Models.Requests.Restaurante;
using ApiDoc.Helpers.Facturacion.Models.Responses;
using Newtonsoft.Json;
using RestSharp;

namespace ApiDoc.Helpers.Facturacion.Service
{
    public static class FacturacionApiV1
    {
        internal static readonly string AppSecret = ConfigurationManager.AppSettings.Get("FACTURACION_API_SECRET");
        internal static readonly string BaseUrl = ConfigurationManager.AppSettings.Get("FACTURACION_API_URL");

        public class Restaurantes
        {
            private const string UrlSolicitarFacturaTicket = "/api/v1/restaurantes/facturas";
            internal SolicitudFacturaResponse CallFacturarTicket(Factura factura)
            {
                try
                {
                    var url = $"{BaseUrl}{UrlSolicitarFacturaTicket}";
                    var client = new RestClient(url);
                    client.AddDefaultHeader("X-Api-Secret", AppSecret);
                    var request = new RestRequest(Method.POST);
#if DEBUG
                    Trace.WriteLine("~FacturacionApi > POST | CallFacturarTicketRestaurante > url: " + url);
                    Trace.WriteLine("~FacturacionApi > POST | CallFacturarTicketRestaurante > body:" +
                                    JsonConvert.SerializeObject(factura));
#endif
                    request.AddJsonBody(factura);

                    var response = client.Execute<SolicitudFacturaResponse>(request);

                    if (!response.IsSuccessful || response.Data == null)
                    {
                        throw new HttpException("Null response");
                    }
#if DEBUG
                    Trace.WriteLine("~FacturacionApi > POST | CallFacturarTicketRestaurante > response:" +
                                    JsonConvert.SerializeObject(response));
#endif
                    return response.Data;
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.WriteLine("~ FacturacionApi>CallFacturarTicketRestaurante | Error al contactar el servidor : " + e.Message);
                    Trace.WriteLine(e.StackTrace);
#endif
                    return new SolicitudFacturaResponse { ResponseCode = (int)ResponseTypes.CodigoNoConexion, Message = "Ocurrio un error al contactar el servidor" };
                }


            }
        }
        

        
    }
}