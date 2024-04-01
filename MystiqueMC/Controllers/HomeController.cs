using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;
using MystiqueMC.Models.Graficas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [ValidatePermissions]
    public class HomeController : BaseController
    {
        #region VARIABLES
        private readonly string HOSTNAME_IMAGENES = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");
        // Fecha minima para SQL
        private readonly DateTime SQL_MIN_DATE = new DateTime(1975, 1, 1);
        //Colores Grafica
        private string GetAColor(bool IsUnavailable = false)
        {
            if (IsUnavailable)
            {
                return "rgba(0, 0, 0, 0.1)";
            }
            else
            {
                ColorIdx++;
                if (ColorIdx == Colors.Length)
                {
                    ColorIdx = 0;
                }
                return Colors[ColorIdx];
            }
        }
        private int ColorIdx = 0;
        private readonly string[] Colors =
        {
            "rgba(255, 99, 132, 0.7)",
            "rgba(54, 162, 235, 0.7)",
            "rgba(255, 206, 86, 0.7)",
            "rgba(75, 192, 192, 0.7)",
            "rgba(153, 102, 255, 0.7)",
            "rgba(22, 192, 33, 0.7)",
            "rgba(204, 184, 38, 0.7)",
            "rgba(94, 155, 54, 0.7)",
            "rgba(73, 160, 124, 0.7)",
            "rgba(73, 86, 160, 0.7)",
            "rgba(160, 73, 119, 0.7)",
        };
        #endregion
        #region GET
        public ActionResult Index(int? sucursalId, string desde = null, string hasta = null, int resumen = 0)
        {
            HomeViewModel viewModel;
            try
            {
                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
                var Usuario = Session.ObtenerUsuario();
                var Rol = Session.ObtenerRol();
                if (!Request.IsAuthenticated || Usuario == null)
                    return RedirectToAction("Login", "Account", null);
                if (Rol == "Analista") return RedirectToAction("Index", "clientes", null);
                ViewBag.Recompensas = GetRecompensasList();
                ViewBag.Membresias = GetMembresiasList();
                var comercios = GetComerciosList();

                List<VW_ObtenerClientesConMembresia> UniversoClientesGraficas;
                UniversoClientesGraficas = Contexto.VW_ObtenerClientesConMembresia
                    .Where(c => c.IdEmpresa == Usuario.empresaId
                    && ClientesVisibles.Select(d => d.idCliente).Contains(c.Id))
                    .ToList();
                RangoFechas rangoFechasResumen = ObtenerRangoFechasParaResumen(resumen, desde, hasta);

                

                viewModel = new HomeViewModel
                {
                    Empresa = Empresa,
                    Comercios = comercios,
                    GraficaUno = new Grafica { Titulo = "Sexos", Data = CalcularDataGraficaSexos(UniversoClientesGraficas) },
                    GraficaDos = new Grafica { Titulo = "Edades", Data = CalcularDataGraficaEdades() },
                    GraficaTres = new Grafica { Titulo = "Ciudades", Data = CalcularDataGraficaCiudades(UniversoClientesGraficas) },
                    ResumenCompras = CalcularResumenDeUso(rangoFechasResumen, 0)

                };
                string rol = Session.ObtenerRol();

                if (rol.Equals("Empresa"))
                {
                    return RedirectToAction("IndexEmpresa", "comercios", new { idEmpresa = Usuario.empresaId });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Home", null);
            }

           

            return View(viewModel);
        }
        public ActionResult Fidelizacion(int? comercioEmpresa, int sucursalId = 0, string tabID = "", string desde = null, string hasta = null, int resumen = 0)
        {
            HomeViewModel viewModel = null;
            try
            {
                resumen = 4;
                ViewBag.comercioEmpresa = comercioEmpresa;
                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
                var Usuario = Session.ObtenerUsuario();
                var Rol = Session.ObtenerRol();
                if (!Request.IsAuthenticated || Usuario == null)
                    return RedirectToAction("Login", "Account", null);
                if (Rol == "Analista") return RedirectToAction("Index", "clientes", null);
                ViewBag.Recompensas = GetRecompensasList();
                ViewBag.Membresias = GetMembresiasList();
                var comercios = GetComerciosList();
                var sucursales = Contexto.sucursales
                   .Where(w => w.comercios.empresaId == Usuario.empresaId)
                   .ToList();
                ViewBag.FiltroSucursal = "TODAS LAS SUCURSALES";
                if (sucursalId != 0)
                {
                    ViewBag.FiltroSucursal = sucursales.Where(w => w.idSucursal == sucursalId).Select(s => s.nombre).FirstOrDefault().ToString();
                }

                List<VW_ObtenerClientesConMembresia> UniversoClientesGraficas;
                UniversoClientesGraficas = Contexto.VW_ObtenerClientesConMembresia
                    .Where(c => c.IdEmpresa == Usuario.empresaId
                    && ClientesVisibles.Select(d => d.idCliente).Contains(c.Id))
                    .ToList();
                RangoFechas rangoFechasResumen = ObtenerRangoFechasParaResumen(resumen, desde, hasta);

                viewModel = new HomeViewModel
                {
                    Empresa = Empresa,
                    Comercios = comercios,
                    GraficaUno = new Grafica { Titulo = "Sexos", Data = CalcularDataGraficaSexos(UniversoClientesGraficas) },
                    GraficaDos = new Grafica { Titulo = "Edades", Data = CalcularDataGraficaEdades() },
                    GraficaTres = new Grafica { Titulo = "Ciudades", Data = CalcularDataGraficaCiudades(UniversoClientesGraficas) },
                    ResumenCompras = CalcularResumenDeUso(rangoFechasResumen, sucursalId),
                    Sucursales = sucursales
                };

                if (Rol.Equals("Empresa")) {
                    tabID = "1";
                }
                ViewBag.TabID = tabID;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Home", null);
            }
            return View(viewModel);
        }
        #endregion
        #region HELPERS


        private empresas GetEmpresa()
        {
            return Empresa;
        }
        private List<comercios> GetComerciosList()
        {
            return ComerciosFirmados.ToList();
        }
        private List<recompensas> GetRecompensasList()
        {
            return RecompensasVisibles
                .Where(c => c.estatus.HasValue
                    && c.estatus.Value)
                .ToList();
        }

        private List<catTipoMembresias> GetMembresiasList()
        {
            return MembresiasVisibles
                .ToList();
        }
        private Dataset[] CalcularDataGraficaCiudades(List<VW_ObtenerClientesConMembresia> data)
        {
            var labels = data
                .Select(c => c.Ciudad)
                .Where(c => c != null)
                .Distinct()
                .ToList();
            var dataset = new List<Dataset>();
            labels.ForEach(categoria =>
            {
                dataset.Add(new Dataset
                {
                    Etiqueta = string.Format("{0} ({1})", categoria, data.Count(f => f.Ciudad == categoria)),
                    Valor = data.Count(f => f.Ciudad == categoria),
                    Color = GetAColor()
                });
            });

            dataset.Add(new Dataset
            {
                Etiqueta = string.Format("{0} ({1})", "Sin Datos", data.Count(c => c.Ciudad == null || c.Ciudad == "")),
                Valor = data.Count(c => c.Ciudad == null || c.Ciudad == ""),
                Color = GetAColor(true)
            });

            return dataset.ToArray();
        }

        private Dataset[] CalcularDataGraficaSexos(List<VW_ObtenerClientesConMembresia> data)
        {
            var labels = data
                .Select(c => c.Sexo)
                .Where(c => c != null)
                .Distinct()
                .ToList();
            var dataset = new List<Dataset>();
            labels.ForEach(categoria =>
            {
                dataset.Add(new Dataset
                {
                    Etiqueta = string.Format("{0} ({1})", categoria, data.Count(f => f.Sexo == categoria)),
                    Valor = data.Count(f => f.Sexo == categoria),
                    Color = GetAColor()
                });
            });
            dataset.Add(new Dataset
            {
                Etiqueta = string.Format("{0} ({1})", "Sin Datos", data.Count(c => c.Sexo == null || c.Sexo == "")),
                Valor = data.Count(c => c.Sexo == null || c.Sexo == ""),
                Color = GetAColor(true)
            });
            return dataset.ToArray();
        }

        private Dataset[] CalcularDataGraficaEdades()
        {
            var data = Contexto.membresias
                .Where(c => ClientesVisibles.Select(d => d.idCliente)
                .Contains(c.clienteId))
                .Select(c => c.clientes);

            var labels = Contexto.catRangoEdad
                .ToList();
            var dataset = new List<Dataset>();
            labels.ForEach(categoria =>
            {
                DateTime FechaInicio = DateTime.Now.AddYears(categoria.edadSuperior * -1);
                DateTime FechaFin = DateTime.Now.AddYears(categoria.edadInferior * -1);
                dataset.Add(new Dataset
                {
                    Etiqueta = string.Format("{0} ({1})", categoria.etiqueta, data.Count(f => f.fechaNacimiento > FechaInicio && f.fechaNacimiento < FechaFin)),
                    Valor = data.Count(f => f.fechaNacimiento > FechaInicio && f.fechaNacimiento < FechaFin),
                    Color = GetAColor()
                });
            });
            dataset.Add(new Dataset
            {
                Etiqueta = string.Format("{0} ({1})", "Sin Datos", data.Count(c => c.fechaNacimiento == null)),
                Valor = data.Count(c => c.fechaNacimiento == null),
                Color = GetAColor(true)
            });
            return dataset.ToArray();
        }

        private Resumen CalcularResumenDeUso(RangoFechas rangoFechas, int? sucursalId)
        {
            var totalCanjesEnPuntos = Contexto.canjePuntos
                    .Where(c => c.catEstatusCanjePuntoId == (int)EstatusCanjePuntos.Canjeado
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.comercios.empresaId == EmpresaUsuario
                        && c.fechaCanje.Value >= rangoFechas.Inicio
                        && c.fechaCanje.Value <= rangoFechas.Fin
                        && (sucursalId == 0 || c.sucursalId == sucursalId))
                    .Select(c => c.valorPuntos)
                    .DefaultIfEmpty(0)
                    .Sum();

            var numeroBeneficiosUsados = Contexto.beneficioAplicados
                    .Where(c => c.sucursales.comercios.empresaId == EmpresaUsuario
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.fechaCompra >= rangoFechas.Inicio
                        && c.fechaCompra <= rangoFechas.Fin
                        && (sucursalId == 0 || c.sucursalId == sucursalId))
                    .Count();

            var totalComprasEnPuntos = Contexto.cargaCompras
                    .Where(c => c.fechaCompra >= rangoFechas.Inicio
                        && c.catEstatusCargaCompraId == (int)EstatusCargaCompras.Aplicado
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.comercios.empresaId == EmpresaUsuario
                        && c.fechaCompra <= rangoFechas.Fin
                        && (sucursalId == 0 || c.sucursalId == sucursalId))
                    .Select(c => c.puntos)
                    .DefaultIfEmpty(0)
                    .Sum();

            var totalCompras = Contexto.cargaCompras
                .Where(c => c.fechaCompra >= rangoFechas.Inicio
                    && c.catEstatusCargaCompraId == (int)EstatusCargaCompras.Aplicado
                    && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                    && c.comercios.empresaId == EmpresaUsuario
                    && c.fechaCompra <= rangoFechas.Fin
                          && (sucursalId == 0 || c.sucursalId == sucursalId))
                .Select(c => c.montoCompra)
                .DefaultIfEmpty(0)
                .Sum();

            var canjes = (decimal?)Contexto.canjePuntos
                   .Where(c => c.fechaCanje.HasValue
                       && c.fechaCanje.Value >= rangoFechas.Inicio
                       && c.catEstatusCanjePuntoId == (int)EstatusCanjePuntos.Canjeado
                       && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                       && c.comercios.empresaId == EmpresaUsuario
                       && c.fechaCanje.Value <= rangoFechas.Fin
                                   && (sucursalId == 0 || c.sucursalId == sucursalId))
                    .Select(c => c.montoCompra)
                    .DefaultIfEmpty(0)
                   .Sum()
                   .GetValueOrDefault();

            var totalBeneficios = Contexto.beneficioAplicados
                    .Where(c => c.fechaCompra >= rangoFechas.Inicio
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.sucursales.comercios.empresaId == EmpresaUsuario
                        && c.fechaCompra <= rangoFechas.Fin
                             && (sucursalId == 0 || c.sucursalId == sucursalId))
                    .Select(c => c.montoCompra)
                    .DefaultIfEmpty(0)
                    .Sum();

            Resumen resumen = new Resumen
            {

                TotalCanjes = canjes ?? 0,
                TotalCanjesEnPuntos = totalCanjesEnPuntos,
                TotalCompras = totalCompras,
                TotalComprasEnPuntos = totalComprasEnPuntos,
                TotalBeneficios = totalBeneficios ?? 0,
                NumeroBeneficiosUsados = numeroBeneficiosUsados,
            };
            return resumen;
        }
        /// <summary>
        ///  Calcula rango de fechas para el resumen, o devuelve uno de los rangos predefinidos 
        /// Rango predefinido -> Default 1 semana (0)
        /// Rango predefinido -> Últimas dos semanas (1)
        /// Rango predefinido ->  Último mes (2)
        /// Rango predefinido ->  Últimos 3 meses (3)
        /// Rango predefinido ->  Todos (4)
        /// </summary>
        /// <param name="rangoPredefinido"> [0,1,2,3,4] </param>
        /// <param name="desde">31/01/2018</param>
        /// <param name="hasta">01/02/2018</param>
        /// <returns></returns>
        private RangoFechas ObtenerRangoFechasParaResumen(int rangoPredefinido, string desde, string hasta)
        {
            DateTime fechaFin = new DateTime();
            DateTime fechaInicio = new DateTime();

            bool customFin = false;
            bool customInicio = false;

            if (!string.IsNullOrEmpty(desde))
            {
                customInicio = DateTime.TryParseExact(desde,
                           format: "dd/MM/yyyy",
                           result: out fechaInicio,
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal);
                if (customInicio) ViewBag.inicio = desde;
            }
            if (!string.IsNullOrEmpty(hasta))
            {
                customFin = DateTime.TryParseExact(hasta,
                           format: "dd/MM/yyyy",
                           result: out fechaFin,
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal);
                if (customFin) ViewBag.fin = hasta;
            }

            if (customFin && customInicio)
            {
                ViewBag.FiltroGrafica = string.Format("del {0} al {1}", desde, hasta);
                ViewBag.FiltroGraficaOpcion = -1;
            }
            else
            {
                switch (rangoPredefinido)
                {
                    case 1:
                        fechaFin = DateTime.Now;
                        fechaInicio = fechaFin.AddDays(-14);
                        ViewBag.FiltroGrafica = "Últimas dos semanas";
                        ViewBag.FiltroGraficaOpcion = 1;
                        break;
                    case 2:
                        fechaFin = DateTime.Now;
                        fechaInicio = fechaFin.AddDays(-30);
                        ViewBag.FiltroGrafica = "Último mes";
                        ViewBag.FiltroGraficaOpcion = 2;
                        break;
                    case 3:
                        fechaFin = DateTime.Now;
                        fechaInicio = fechaFin.AddDays(-90);
                        ViewBag.FiltroGrafica = "Últimos 3 meses";
                        ViewBag.FiltroGraficaOpcion = 3;
                        break;
                    case 4:
                        fechaFin = DateTime.Now;
                        fechaInicio = SQL_MIN_DATE;
                        ViewBag.FiltroGrafica = "Todos";
                        ViewBag.FiltroGraficaOpcion = 4;
                        break;
                    default:
                        fechaFin = DateTime.Now;
                        fechaInicio = fechaFin.AddDays(-7);
                        ViewBag.FiltroGraficaOpcion = 0;
                        break;
                }
            }
            return new RangoFechas
            {
                Fin = fechaFin,
                Inicio = fechaInicio
            };
        }

        public ActionResult ValidarSesion()
        {
            var CanContinue = true;
            var Usuario = Session.ObtenerUsuario();
            var Rol = Session.ObtenerRol();
            if (!Request.IsAuthenticated || Usuario == null)
            {
                CanContinue = false;
            }

            return new { success = CanContinue }.ToJsonResult();
        }
        #endregion
    }
}