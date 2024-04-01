using MystiqueMC.DAL;
using MystiqueMcApi.Helpers;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class ConfiguracionController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
        private PermisosApi validar = new PermisosApi();

        [Route("api/obtenerConfiguracion")]
        public ResponseConfiguracionSistema obtenerConfiguracion([FromBody]RequestConfiguracion entradas)
        {
            ResponseConfiguracionSistema respuesta = new ResponseConfiguracionSistema();

            try
            {
                using (contextEntity)
                {
                    var resultado = contextEntity.configuracionSistema.Where(w => w.empresaId == entradas.idEmpresa).Select(n => new Models.Salidas.ResponseConfiSistema
                    {
                        empresaId = n.empresaId,
                        telefonoContacto = n.telefonoContacto,
                        correoContacto = n.correoContacto,
                        txtTerminosCondiciones = n.txtTerminosCondiciones,
                        txtSoporte = n.txtSoporte,
                        versionAndroid = n.versionAndroid,
                        versionAndroidTest = n.versionAndroidTest,
                        versioniOS = n.versioniOS,
                        versioniOSTest = n.versioniOSTest,
                        mostrarComercios = n.mostrarComercios,
                        mostrarSucursales = n.mostrarSucursales,
                        idQDC = n.idQDC,
                    }).FirstOrDefault();


                    var resultadoComercios = contextEntity.comercios.Where(w => w.empresaId == entradas.idEmpresa).Select(n => new Models.Salidas.ResponseConfiSistemaComercios
                    {
                        empresaId = n.empresaId,
                        idComercio = n.idComercio,
                        nombreComercial = n.nombreComercial,
                        logoUrl = n.logoUrl,

                    }).ToList();

                    respuesta.configuraciones = resultado;
                    respuesta.listaComercios = resultadoComercios;
                }
                respuesta.Success = true;
                respuesta.ErrorMessage = "";
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }




        [Route("api/obtenerColonias")]
        public ResponseCatColonias obtenerColonias([FromBody]RequestColonia entradas)
        {
            ResponseCatColonias respuesta = new ResponseCatColonias();

            try
            {
                var colonias = contextEntity.catColonias.Where(w => w.catCiudadId == entradas.ciudadId)
                    .Select(n => new ResponseColonia
                    {
                        coloniaId = n.idCatColonia,
                        descripcionColonia = n.descripcion

                    }).ToList();

                respuesta.Success = true;
                respuesta.ErrorMessage = "";
                respuesta.colonias = colonias;
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
