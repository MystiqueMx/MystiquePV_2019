using FacturacionTDCAPI.Models.Responses.Receptores;
using Microsoft.Ajax.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Utility.WebApi.OutputCache.V2;

namespace FacturacionTDCAPI.Controllers
{
    [RoutePrefix("api/v1/Receptores")]
    public class ReceptoresController : BaseApiController
    {
        #region CONSULTAS

        [HttpGet, Route("receptor/{rfc:regex(^([A-Za-zÑñ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([A-Z]|[0-9]){2}([A]|[0-9]){1})?$)}")]
        public ReceptorResponse Get(string rfc)
        {
            try
            {
                if (!IsAppSecretValid) return new ReceptorResponse { EstatusPeticion = RespuestaNoPermisos };
                if (!ModelState.IsValid) return new ReceptorResponse { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("ReceptoresController Get from: {0}", Request.Headers.UserAgent);

                if (Contexto.ReceptorFacturas.Any(c => c.rfc == rfc))
                {
                    var receptor = Contexto.ReceptorFacturas
                        .OrderByDescending(c => c.idReceptorFactura)
                        .First(c => c.rfc == rfc);
                    //TODO VALIDAR EL USOCFDI LO MODIFIQUE PARA QUE FUNCIONARA
                    return new ReceptorResponse
                    {
                        EstatusPeticion = RespuestaOk,
                        Receptor = new Models.Requests.Restaurante.ReceptorFactura
                        {
                            CodigoPostal = receptor.codigoPostal,
                            Direccion = receptor.direccion,
                            Rfc = receptor.rfc,
                            RazonSocial = receptor.razonSocial,
                            UsoCfdi = receptor.catUsoCfdi,
                            CorreoElectrónico = receptor.correo,
                        }
                    };
                }
                else if (Contexto.ReceptorFactura_Legado.Any(c => c.rfc == rfc))
                {
                    var receptor = Contexto.ReceptorFactura_Legado
                        .OrderByDescending(c => c.idReceptorFactura_Legado)
                        .First(c => c.rfc == rfc);
                    return new ReceptorResponse
                    {
                        EstatusPeticion = RespuestaOk,
                        Receptor = new Models.Requests.Restaurante.ReceptorFactura
                        {
                            CodigoPostal = receptor.codigoPostal,
                            Direccion = receptor.direccion,
                            Rfc = receptor.rfc,
                            RazonSocial = receptor.razonSocial,
                            UsoCfdi = receptor.catUsoCfdi,
                            CorreoElectrónico = receptor.correo,
                        }
                    };
                }
                else
                {
                    return new ReceptorResponse { EstatusPeticion = RespuestaOkMensaje("No registrado") };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ReceptorResponse { EstatusPeticion = RespuestaErrorInterno };
            }
        }

        [HttpGet, Route("receptor/search")]
        public async Task<ReceptorSearchResponse> Search(string term)
        {
            try
            {
                if (!IsAppSecretValid) return new ReceptorSearchResponse { EstatusPeticion = RespuestaNoPermisos };
                if (!ModelState.IsValid) return new ReceptorSearchResponse { EstatusPeticion = RespuestaErrorValidacion(ModelState) };

                Logger.Debug("ReceptoresController Get from: {0}", Request.Headers.UserAgent);

                var receptores = Contexto.ReceptorFacturas
                    .OrderByDescending(c => c.idReceptorFactura)
                    .Where(c => c.rfc.Contains(term))
                    .GroupBy(c => c.rfc, c => c.razonSocial, (key, rz) => new { key, val = rz.FirstOrDefault() })
                    .ToDictionary(c => c.key, c => c.val);
                var rfcsEncontrados = receptores.Keys.ToArray();
                var receptoresLegado = Contexto.ReceptorFactura_Legado
                    .OrderByDescending(c => c.idReceptorFactura_Legado)
                    .Where(c => c.rfc.Contains(term) && !rfcsEncontrados.Contains(c.rfc))
                    .GroupBy(c => c.rfc, c => c.razonSocial, (key, rz) => new { key, val = rz.FirstOrDefault() })
                    .ToDictionary(c => c.key, c => c.val);
                if (receptoresLegado?.Any() ?? false)
                {
                    receptoresLegado.ForEach(c =>
                    {
                        receptores.Add(c.Key, c.Value);
                    });
                }
                return new ReceptorSearchResponse
                {
                    EstatusPeticion = RespuestaOk,
                    Receptores = receptores
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ReceptorSearchResponse { EstatusPeticion = RespuestaErrorInterno };
            }
        }
        #endregion
    }
}