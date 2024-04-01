using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class ComercioController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";


        [Route("api/obtenerComercioEmpresa")]
        public ResponseListacomercios obtenerListadoComerciosPorGiro([FromBody]RequestBase entradas)
        {
            ResponseListacomercios respuesta = new ResponseListacomercios();

            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia , entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    var comercios = contextEntity.comercios.Where(w => w.empresaId == entradas.empresaId).Select(s => new ResponseComercioEmpresa
                    {
                        comercioId = s.idComercio,
                        nombreComercial = s.nombreComercial,
                        urlLogoComercio = s.logoUrl
                    }).ToList();

                    respuesta.listaComercioEmpresa = comercios;
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }


    }
}