using ApiDoc.Models;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class ZonasController : BaseApiController
    {
        #region POST
        [HttpPost]
        [Route("api/zonas")]
        public ZonasResponse ObtenerZonas(RequestBase viewmodel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new ZonasResponse
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }

                var zonas = Contexto.CatZonas
                    .Where(c => c.activo)
                    .OrderBy(c => c.descripcion)
                    .Where(c => viewmodel.Empresas.Contains(c.empresaId))
                    .Select(c => new ZonasJson
                    {
                        Descripcion = c.descripcion,
                        Id = c.idCatZona,
                        Latitud = c.latitud,
                        Longitud = c.longitud
                    })
                    .ToArray();

                return new ZonasResponse
                {
                    Success = true,
                    Data = zonas,
                };
            }
            catch (Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new ZonasResponse
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }

        [HttpPost]
        [Route("api/zonas/sucursales")]
        public SucursalPorZonaResponse ObtenerSucursalesPorZona(SucursalesPorZonaFiltrosRequest viewModel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new SucursalPorZonaResponse
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }
                var girosIds = viewModel.Giros.Select(c => (int)c);
                var query = Contexto.sucursales
                    .Include(c => c.comercios)
                    .Include(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad)
                    .Include(c => c.direccion)
                    .Where(c => (c.activo == true && c.comercios.estatus == true)
                        && viewModel.Empresas.Contains(c.comercios.empresaId)
                        && c.catZonaId == viewModel.Zona
                        && c.comercios.catComercioGiroId.HasValue
                        && girosIds.Contains(c.comercios.catComercioGiroId.Value))
                    .OrderBy(c => c.nombre)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(viewModel.Filtro))
                {
                    query = query.Where(c => c.nombre.Contains(viewModel.Filtro));
                }

                if (!string.IsNullOrEmpty(viewModel.Especialidad))
                {
                    if (viewModel.Idioma == AppLanguage.Spanish)
                    {
                        query = query.Where(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad.descripcion.Contains(viewModel.Especialidad));
                    }
                    else
                    {
                        query = query.Where(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad.descripcionIngles.Contains(viewModel.Especialidad));
                    }
                }
                if (viewModel.catAseguranzaId.HasValue)
                {
                    var aseguranzasDoctores = Contexto.DoctorAseguranzas
                        .Where(w => w.catAseguranzaId == viewModel.catAseguranzaId)
                        .Select(s => s.anexoDoctorId)
                        .ToList();

                    query = query.Where(c => aseguranzasDoctores.Any(a => a == c.comercios.AnexoDoctor.idAnexoDoctor));
                }

                var sucursales = query
                    .Select(c => new
                    {
                        Id = c.idSucursal,
                        Direccion = c.direccion,
                        Latitud = c.latitud,
                        Longitud = c.longitud,
                        Nombre = c.comercios.nombreComercial,
                        Especialidad = c.comercios.AnexoDoctor.CatDoctorEspecialidad,
                        Tipo = (TipoSucursales)c.comercios.catComercioGiroId,
                    })
                    .ToArray()
                    .Select(c => new SucursalJson
                    {
                        Id = c.Id,
                        Latitud = c.Latitud,
                        Longitud = c.Longitud,
                        Tipo = c.Tipo,
                        Nombre = c.Nombre,
                        Direccion = ConvertDireccionToString(c.Direccion),
                        Especialidad = viewModel.Idioma == AppLanguage.Spanish ? c.Especialidad?.descripcion : c.Especialidad?.descripcionIngles
                    });

                return new SucursalPorZonaResponse
                {
                    Success = true,
                    Data = sucursales,
                };
            }
            catch (Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new SucursalPorZonaResponse
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }

        [HttpPost]
        [Route("api/zonas/sucursalesZonaDocEsp")]
        public SucursalPorZonaResponse ObtenerSucursalesPorZonaDocEsp(SucursalesPorZonaFiltrosRequest viewModel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new SucursalPorZonaResponse
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }
                var girosIds = viewModel.Giros.Select(c => (int)c);
                var query = Contexto.sucursales
                    .Include(c => c.comercios)
                    .Include(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad)
                    .Include(c => c.direccion)
                    .Where(c => (c.activo == true && c.comercios.estatus == true)
                        && viewModel.Empresas.Contains(c.comercios.empresaId)
                        && c.catZonaId == viewModel.Zona
                        && c.comercios.catComercioGiroId.HasValue
                        && girosIds.Contains(c.comercios.catComercioGiroId.Value))
                    .OrderBy(c => c.nombre)
                    .AsQueryable();

                var queryDoc = Contexto.sucursales
                    .Include(c => c.comercios)
                    .Include(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad)
                    .Include(c => c.direccion)
                    .Where(c => (c.activo == true && c.comercios.estatus == true)
                        && viewModel.Empresas.Contains(c.comercios.empresaId)
                        && c.catZonaId == viewModel.Zona
                        && c.comercios.catComercioGiroId.HasValue
                        && girosIds.Contains(c.comercios.catComercioGiroId.Value)
                        && c.comercios.nombreComercial.Contains(viewModel.Especialidad))
                    .OrderBy(c => c.nombre)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(viewModel.Filtro))
                {
                    query = query.Where(c => c.nombre.Contains(viewModel.Filtro));
                }

                if (!string.IsNullOrEmpty(viewModel.Especialidad))
                {
                    if (viewModel.Idioma == AppLanguage.Spanish)
                    {
                        query = query.Where(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad.descripcion.Contains(viewModel.Especialidad));
                    }
                    else
                    {
                        query = query.Where(c => c.comercios.AnexoDoctor.CatDoctorEspecialidad.descripcionIngles.Contains(viewModel.Especialidad));
                    }
                }
                if (viewModel.catAseguranzaId.HasValue)
                {
                    var aseguranzasDoctores = Contexto.DoctorAseguranzas
                        .Where(w => w.catAseguranzaId == viewModel.catAseguranzaId)
                        .Select(s => s.anexoDoctorId)
                        .ToList();

                    query = query.Where(c => aseguranzasDoctores.Any(a => a == c.comercios.AnexoDoctor.idAnexoDoctor));
                }

                var mergedQuery = query.Union(queryDoc);

                var sucursales = mergedQuery
                    .Select(c => new
                    {
                        Id = c.idSucursal,
                        Direccion = c.direccion,
                        Latitud = c.latitud,
                        Longitud = c.longitud,
                        Nombre = c.comercios.nombreComercial,
                        Especialidad = c.comercios.AnexoDoctor.CatDoctorEspecialidad,
                        Tipo = (TipoSucursales)c.comercios.catComercioGiroId,
                    })
                    .ToArray()
                    .Select(c => new SucursalJson
                    {
                        Id = c.Id,
                        Latitud = c.Latitud,
                        Longitud = c.Longitud,
                        Tipo = c.Tipo,
                        Nombre = c.Nombre,
                        Direccion = ConvertDireccionToString(c.Direccion),
                        Especialidad = viewModel.Idioma == AppLanguage.Spanish ? c.Especialidad?.descripcion : c.Especialidad?.descripcionIngles
                    });

                return new SucursalPorZonaResponse
                {
                    Success = true,
                    Data = sucursales,
                };
            }
            catch (Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new SucursalPorZonaResponse
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }
        #endregion

        #region HELPERS
        public string ConvertDireccionToString(MystiqueMC.DAL.direccion item)
        {
            if (item == null)
            {
                return "-";
            }
            else
            {
                var dir = item.calle + " " + item.numExterior;
                if (!string.IsNullOrEmpty(item.numInterior))
                {
                    dir += " " + item.numInterior;
                }
                if (!string.IsNullOrEmpty(item.colonia))
                {
                    dir += " " + item.colonia;
                }
                if (!string.IsNullOrEmpty(item.codigoPostal))
                {
                    dir += " " + item.codigoPostal;
                }
                if (item.catCiudades != null)
                {
                    dir += " " + item.catCiudades.ciudadDescr;
                }
                if (item.catEstados != null)
                {
                    dir += " " + item.catEstados.estadoDescr;
                }
                return dir;
            }
        }
        #endregion
    }
}