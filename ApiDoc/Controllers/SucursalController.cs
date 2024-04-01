using ApiDoc.Helpers;
using ApiDoc.Models;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class SucursalController : BaseApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";

        [Route("api/obtenerListaSucursalesPorComercio")]
        public ResponseListaSucursal obtenerListaSucursalesPorComercio([FromBody]RequestSucursaComercio entradas)
        {
            ResponseListaSucursal respuesta = new ResponseListaSucursal();

            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    respuesta.ListSucursalPorComercio = contextEntity.SP_Obtener_Sucursal_Comercio(entradas.idComercio).ToList();
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

        [HttpPost]
        [Route("api/sucursal")]
        public DetalleSucursalResponse ObtenerDetalleSucursal(DetalleSucursalRequest viewModel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new DetalleSucursalResponse
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }

                var sucursal = Contexto.sucursales
                            .Include(c => c.confBeneficioSucursal)
                            .Include(c => c.confBeneficioSucursal.Select(d => d.beneficios))
                            .Include(c => c.sucursalHorarios)
                            .Include(c => c.comercios)
                            .Include(c => c.comercios.AnexoDoctor)
                            .Include(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad)
                            .Include(c => c.comercios.ImagenDoctor)
                            .First(c => c.idSucursal == viewModel.Sucursal && (c.activo == true || c.comercios.estatus == true)
                                    && viewModel.Empresas.Contains(c.comercios.empresaId));


                var horariosSemanales = ObtenerHorariosSucursal(sucursal.sucursalHorarios.ToArray());
                var diaSemana = DateTime.Today.DayOfWeek;
                var hoy = DateTime.Today;
                var horarioActual = horariosSemanales.FirstOrDefault(c => c.Dia == diaSemana);

                //TODO: AGREGAR FECHA VIGENCIA AL RESPONSE DE LISTADO DE BENEFICIOS
                var sucursalBeneficios = sucursal.confBeneficioSucursal
                    .Where(c => c.beneficios.estatus
                        && c.beneficios.fechaInicio <= hoy
                        && c.beneficios.fechaFin >= hoy)
                    .Select(c => c.beneficios)
                    .ToArray();
                var beneficios = ObtenerBeneficiosPorIdioma(viewModel.Idioma, sucursalBeneficios);
                Anexo anexo = null;
                if (sucursal.comercios.catComercioGiroId == (int)TipoSucursales.Doctores)
                {
                    anexo = new Anexo
                    {
                        Cedula = sucursal.comercios.AnexoDoctor.cedula,
                        Especialidad = viewModel.Idioma == AppLanguage.Spanish
                            ? sucursal.comercios.AnexoDoctor.CatDoctorEspecialidad.descripcion
                            : sucursal.comercios.AnexoDoctor.CatDoctorEspecialidad.descripcionIngles,
                        Descripcion = viewModel.Idioma == AppLanguage.Spanish
                            ? sucursal.comercios.AnexoDoctor.descripcion
                            : sucursal.comercios.AnexoDoctor.descripcionIngles,
                        Imagenes = sucursal.comercios.ImagenDoctor.Select(c => ObtenerUrlImagen(c.ruta)).ToArray(),
                        ImagenesAseguranzas = sucursal.comercios.AnexoDoctor.DoctorAseguranzas.Select(c => ObtenerUrlImagen(c.CatAseguranzas.imagenAseguranza)).ToArray(),
                        PrecioConsulta = sucursal.comercios.AnexoDoctor?.precioConsulta,
                    };
                }
                return new DetalleSucursalResponse
                {
                    Success = true,
                    Id = sucursal.idSucursal,
                    Nombre = sucursal.comercios.nombreComercial,
                    EmailContacto = sucursal.comercios.correo,
                    Telefono = sucursal.telefono,
                    Imagen = ObtenerUrlImagen(sucursal.comercios.logoUrl),
                    Facebook = sucursal.faceBook,
                    Twitter = sucursal.twitter,
                    Instagram = sucursal.instagram,
                    HorarioHoy = horarioActual,
                    HorarioSemanal = horariosSemanales,
                    Tipo = (TipoSucursales)sucursal.comercios.catComercioGiroId,
                    Beneficios = beneficios,
                    Doctor = anexo,
                };
            }
            catch (Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new DetalleSucursalResponse
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }
        #region HELPER
        private List<HorarioSucursal> ObtenerHorariosSucursal(sucursalHorarios[] sucursalHorarios)
        {
            var horarios = new List<HorarioSucursal>();
            if (sucursalHorarios.FirstOrDefault(c => c.dayOfWeek == (int)DayOfWeek.Sunday) is sucursalHorarios horario
                && horario != null)
            {
                horarios.Add(new HorarioSucursal
                {
                    Dia = DayOfWeek.Sunday,
                    Horario = $"{horario.horarioInicio} - {horario.horarioFin}"
                });
            }
            if (sucursalHorarios.FirstOrDefault(c => c.dayOfWeek == (int)DayOfWeek.Monday) is sucursalHorarios horarioL
                && horarioL != null)
            {
                horarios.Add(new HorarioSucursal
                {
                    Dia = DayOfWeek.Monday,
                    Horario = $"{horarioL.horarioInicio} - {horarioL.horarioFin}"
                });
            }
            if (sucursalHorarios.FirstOrDefault(c => c.dayOfWeek == (int)DayOfWeek.Tuesday) is sucursalHorarios horarioT
                && horarioT != null)
            {
                horarios.Add(new HorarioSucursal
                {
                    Dia = DayOfWeek.Tuesday,
                    Horario = $"{horarioT.horarioInicio} - {horarioT.horarioFin}"
                });
            }
            if (sucursalHorarios.FirstOrDefault(c => c.dayOfWeek == (int)DayOfWeek.Wednesday) is sucursalHorarios horarioW
                && horarioW != null)
            {
                horarios.Add(new HorarioSucursal
                {
                    Dia = DayOfWeek.Wednesday,
                    Horario = $"{horarioW.horarioInicio} - {horarioW.horarioFin}"
                });
            }
            if (sucursalHorarios.FirstOrDefault(c => c.dayOfWeek == (int)DayOfWeek.Thursday) is sucursalHorarios horarioTh
                && horarioTh != null)
            {
                horarios.Add(new HorarioSucursal
                {
                    Dia = DayOfWeek.Thursday,
                    Horario = $"{horarioTh.horarioInicio} - {horarioTh.horarioFin}"
                });
            }
            if (sucursalHorarios.FirstOrDefault(c => c.dayOfWeek == (int)DayOfWeek.Friday) is sucursalHorarios horarioF
                && horarioF != null)
            {
                horarios.Add(new HorarioSucursal
                {
                    Dia = DayOfWeek.Friday,
                    Horario = $"{horarioF.horarioInicio} - {horarioF.horarioFin}"
                });
            }
            if (sucursalHorarios.FirstOrDefault(c => c.dayOfWeek == (int)DayOfWeek.Saturday) is sucursalHorarios horarioS
                && horarioS != null)
            {
                horarios.Add(new HorarioSucursal
                {
                    Dia = DayOfWeek.Saturday,
                    Horario = $"{horarioS.horarioInicio} - {horarioS.horarioFin}"
                });
            }
            return horarios;
        }
        private List<BeneficioSucursal> ObtenerBeneficiosPorIdioma(Models.AppLanguage idioma, beneficios[] sucursalBeneficios)
        {
            var beneficios = new List<BeneficioSucursal>();
            switch (idioma)
            {
                case Models.AppLanguage.English:
                    beneficios.AddRange(sucursalBeneficios
                        .Select(c => new BeneficioSucursal
                        {
                            Codigo = c.cadenaCodigo,
                            Id = c.idBeneficio,
                            Descripcion = c.descripcionIngles,
                            Imagen = ObtenerUrlImagen(c.imagenTitulo),
                            Vigencia = c.fechaFin.ToString("MM/dd/yyyy")
                            //Imagen = ObtenerUrlImagen(c.urlImagenCodigo),
                        }));
                    break;
                default:
                    beneficios.AddRange(sucursalBeneficios
                        .Select(c => new BeneficioSucursal
                        {
                            Codigo = c.cadenaCodigo,
                            Id = c.idBeneficio,
                            Descripcion = c.descripcion,
                            Imagen = ObtenerUrlImagen(c.imagenTitulo),
                            Vigencia = c.fechaFin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                            //Imagen = ObtenerUrlImagen(c.urlImagenCodigo),
                        }));
                    break;
            }
            return beneficios;
        }
        #endregion
    }
}