using System;
using System.Linq;
using System.Web.Http;
using System.Web;
using WebApi.OutputCache.V2;
using FacturacionTDCAPI.Models.Responses.Catalogos;

namespace FacturacionTDCAPI.Controllers
{
    [RoutePrefix("api/v1/Catalogos")]
    [CacheOutput(ClientTimeSpan = 60 * 60 * 12, ServerTimeSpan = 0)]
    public class CatalogosController : BaseApiController
    {
        #region CONSULTAS
        [HttpGet]
        [Route("usosCfdi")]
        public UsosCfdiResponse GetUsosCfdi()
        {
            try
            {
                if (!IsAppSecretValid) return new UsosCfdiResponse { EstatusPeticion = RespuestaNoPermisos };
                if (!ModelState.IsValid) return new UsosCfdiResponse { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("CatalogosController GetUsosCfdi from: {0}", Request.Headers.UserAgent);

                return new UsosCfdiResponse
                {
                    EstatusPeticion = RespuestaOk,
                    Data = Contexto.CatalogoUsoCfdis.ToDictionary(k => k.idCatalogoUsoCfdi, v => v.descripcion)
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new UsosCfdiResponse { EstatusPeticion = RespuestaErrorInterno };
            }
        }

        [HttpGet]
        [Route("formasPago")]
        public MetodosPagoResponse GetFormasPago()
        {
            try
            {
                if (!IsAppSecretValid) return new MetodosPagoResponse { EstatusPeticion = RespuestaNoPermisos };
                if (!ModelState.IsValid) return new MetodosPagoResponse { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("CatalogosController GetFormasPago from: {0}", Request.Headers.UserAgent);

                return new MetodosPagoResponse
                {
                    EstatusPeticion = RespuestaOk,
                    Data = Contexto.CatalogoFormaPagoes.ToDictionary(k => k.idCatalogoFormaPago, v => v.descripcion)
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new MetodosPagoResponse { EstatusPeticion = RespuestaErrorInterno };
            }
        }
        #endregion
    }
}