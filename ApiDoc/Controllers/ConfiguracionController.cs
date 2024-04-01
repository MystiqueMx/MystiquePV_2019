using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class ConfiguracionController : BaseApiController
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
                    var resultado = contextEntity.configuracionSistema.Where(w => w.empresaId.HasValue && entradas.Empresas.Contains(w.empresaId.Value)).Select(n => new Models.Salidas.ResponseConfiSistema
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
                        soporteLinea1Ingles = n.soporteLinea1Ingles,
                        soporteLinea2Ingles = n.soporteLinea2Ingles,
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

        [HttpPost]
        [Route("api/especialidades")]
        public EspecialidadesResponse ObtenerEspecialidades(EspecialidadesRequest viewmodel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new EspecialidadesResponse
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }
                var query = Contexto.CatDoctorEspecialidad
                        .Select(c => new { c.idCatDoctorEspecialidad, c.descripcion, c.descripcionIngles })
                        .AsQueryable();


                var rangos = viewmodel.Idioma == Models.AppLanguage.Spanish
                    ? query
                    .OrderBy(c => c.descripcion)
                    .Select(c => new CustomSelectItem
                        {
                            Id = c.idCatDoctorEspecialidad,
                            Descripcion = c.descripcion
                        })
                        .ToArray()
                    : query
                    .OrderBy(c => c.descripcionIngles)
                    .Select(c => new CustomSelectItem
                        {
                            Id = c.idCatDoctorEspecialidad,
                            Descripcion = c.descripcionIngles
                        })
                        .ToArray();

                return new EspecialidadesResponse
                {
                    Success = true,
                    Data = rangos,
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new EspecialidadesResponse
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
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

        [Route("api/obtenerConfiguracionDoctor")]
        public ResponseConfiguracionSistema obtenerConfiguracionDoctor([FromBody]RequestConfiguracion entradas)
        {
            ResponseConfiguracionSistema respuesta = new ResponseConfiguracionSistema();

            try
            {
                using (contextEntity)
                {
                    var resultado = contextEntity.configuracionSistema.Where(w => w.empresaId.HasValue && entradas.Empresas.Contains(w.empresaId.Value)).Select(n => new Models.Salidas.ResponseConfiSistema
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
                        soporteLinea1Ingles = n.soporteLinea1Ingles,
                        soporteLinea2Ingles = n.soporteLinea2Ingles,
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

    }
}
