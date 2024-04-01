using System;
using System.Linq;
using System.ServiceModel;
using System.Web.Http;
using FacturacionApi.Helpers.Criptografia;

namespace FacturacionApi.Controllers
{
    [RoutePrefix("self")]
    public class HealthController : BaseApiController
    {
        [HttpGet, Route("health")]
        public ErrorCodeResponseBase Get() => IsAppSecretValid ?  RespuestaOk : RespuestaNoPermisos;
        [HttpGet, Route("demo")]
        public ErrorCodeResponseBase Demo(Models.Requests.Restaurante.Factura factura)
        {
           
            return RespuestaOk;
        }
    }
}
