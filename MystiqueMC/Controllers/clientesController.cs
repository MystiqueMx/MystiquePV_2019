using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;
using MystiqueMC.Models.Graficas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{

    [Authorize]
   [ValidatePermissions]
    public class clientesController : BaseController
    {
        #region VARIABLES
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
        public ActionResult Index(string OrdenarPor, string SearchPalabra, string desde = null, string hasta = null, int resumen = 0)
        {
            ClientesViewModel viewModel;
            try
            {
                
            #region TAB2
                var Usuario = Session.ObtenerUsuario();
                var clientes = Contexto.VW_ObtenerClientesConMembresia
                    .Where(c => c.IdEmpresa == Usuario.empresaId
                        && ClientesVisibles.Select(d=>d.idCliente).Contains(c.Id))
                    .AsQueryable();

                //BUSQUEDA 
                if (!string.IsNullOrEmpty(SearchPalabra))
                {
                    clientes = clientes.Where(w => w.NombreCompleto.ToUpper().Contains(SearchPalabra.ToUpper())
                    || w.Email.ToUpper().Contains(SearchPalabra.ToUpper())
                    || w.Telefono == SearchPalabra.ToUpper());
                    ViewBag.SearchPalabra = SearchPalabra;
                    ViewBag.VerDetalle = true;
                }
                else
                {
                    ViewBag.VerDetalle = false;
                    ViewBag.SearchPalabra = string.Empty;
                }
                //ORDENACION
                switch (OrdenarPor)
            {
                case "1":
                    clientes = clientes.OrderByDescending(o => o.Telefono);
                    break;
                case "2":
                    clientes = clientes.OrderBy(o => o.Email);
                    break;
                default:
                    clientes = clientes.OrderBy(o => o.NombreCompleto);
                    break;
            }
                #endregion
                List<VW_ObtenerClientesConMembresia> ClientesTabla = clientes.ToList();
                List<VW_ObtenerClientesConMembresia> UniversoClientesGraficas;
                if (string.IsNullOrEmpty(SearchPalabra)) // Si no hay filtros en la tabla, usa el mismo query para las graficas
                {
                    UniversoClientesGraficas = ClientesTabla;
                }
                else
                {
                    UniversoClientesGraficas = Contexto.VW_ObtenerClientesConMembresia
                        .Where(c => c.IdEmpresa == Usuario.empresaId
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.Id))
                        .ToList();
                }
                RangoFechas rangoFechasResumen = ObtenerRangoFechasParaResumen(resumen, desde, hasta);

                viewModel = new ClientesViewModel
                {
                    Clientes = ClientesTabla,
                    GraficaUno = new Grafica { Titulo = "Sexos", Data = CalcularDataGraficaSexos(UniversoClientesGraficas) },
                    GraficaDos = new Grafica { Titulo = "Edades", Data = CalcularDataGraficaEdades() },
                    GraficaTres = new Grafica { Titulo = "Ciudades", Data = CalcularDataGraficaCiudades(UniversoClientesGraficas) },
                    ResumenCompras = CalcularResumenDeUso(rangoFechasResumen)
                };
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Home", null);
            }
            
            return View(viewModel);
        }
        public ActionResult DetalleCompras(string desde = null, string hasta = null, int resumen = 0)
        {
            try
            {
                RangoFechas rangoFechas;
                if (resumen < 0)
                {
                    rangoFechas = ObtenerRangoFechasParaResumen(resumen, desde, hasta);
                }
                else
                {
                    rangoFechas = ObtenerRangoFechasParaResumen(resumen, null, null);
                }
                    
                var Compras = Contexto.cargaCompras
                    .Where(c => c.comercios.empresaId == EmpresaUsuario 
                        && c.catEstatusCargaCompraId == (int)EstatusCargaCompras.Aplicado
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.fechaCompra >= rangoFechas.Inicio 
                        && c.fechaCompra <= rangoFechas.Fin)
                    .OrderByDescending(c=>c.fechaCompra)
                    .ToList();
                return View(Compras);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return RedirectToAction("Index", "Home", null);
            }
        }
        public ActionResult DetalleCanjes(string desde = null, string hasta = null, int resumen = 0)
        {
            try
            {
                RangoFechas rangoFechas;
                if (resumen < 0)
                {
                    rangoFechas = ObtenerRangoFechasParaResumen(resumen, desde, hasta);
                }
                else
                {
                    rangoFechas = ObtenerRangoFechasParaResumen(resumen, null, null);
                }

                var Canjes = Contexto.canjePuntos
                    .Where(c => c.comercios.empresaId == EmpresaUsuario
                        && c.catEstatusCanjePuntoId == (int)EstatusCanjePuntos.Canjeado
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.fechaCanje >= rangoFechas.Inicio
                        && c.fechaCanje <= rangoFechas.Fin)
                    .OrderByDescending(c => c.fechaCanje)
                    .ToList();
                return View(Canjes);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return RedirectToAction("Index", "Home", null);
            }
        }
        public ActionResult DetalleBeneficios(string desde = null, string hasta = null, int resumen = 0)
        {
            try
            {
                RangoFechas rangoFechas;
                if (resumen < 0)
                {
                    rangoFechas = ObtenerRangoFechasParaResumen(resumen, desde, hasta);
                }
                else
                {
                    rangoFechas = ObtenerRangoFechasParaResumen(resumen, null, null);
                }

                var Beneficios = Contexto.beneficioAplicados
                    .Where(c => c.sucursales.comercios.empresaId == EmpresaUsuario
                    && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.fechaCompra >= rangoFechas.Inicio
                        && c.fechaCompra <= rangoFechas.Fin)
                    .OrderByDescending(c => c.fechaCompra)
                    .ToList();
                return View(Beneficios);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return RedirectToAction("Index", "Home", null);
            }
        }
        #endregion
        #region OVERRIDES
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Contexto.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
        #region AJAX
        // GET: /clientes/DetailsPointsPurchases/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult DetailsPointsPurchases(int? id, int? asc)
        {
            Contexto.Configuration.LazyLoadingEnabled = false;
            var Compras = Contexto.cargaCompras
             .Where(w => w.membresias.clienteId == id
                    && w.catEstatusCargaCompraId == (int)EstatusCargaCompras.Aplicado)
             .OrderByDescending(o => o.fechaRegistro)
             .ToList();
             

            switch (asc)
            {
                case 1:
                    Compras.Reverse();
                    break;

            }

            var ListaCompras = Compras.Select(s => new
            {
                Fecha = s.fechaRegistro,
                NoTicket = s.folioCompra,
                Monto = s.montoCompra.ToString("C0"),
                Puntos = s.puntos.ToString("C0").Replace("$", ""),
                PuntosAsDecimal = s.puntos

            }).ToList();
            return new { success = true, results = ListaCompras, total = ListaCompras.Sum(c => c.PuntosAsDecimal).ToString("C0").Replace("$", "") }.ToJsonResult();
        }
        // GET: /clientes/DetailsPointsProducts/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult DetailsPointsProducts(int? id, int? filtro)
        {
            Contexto.Configuration.LazyLoadingEnabled = false;

            var Canje = Contexto.canjePuntos
                .Where(w => w.membresias.clienteId == id
                    && w.catEstatusCanjePuntoId == (int)EstatusCanjePuntos.Canjeado)
                .Include(c=>c.recompensas.catProductos)
                .OrderByDescending(o => o.fechaRegistro)
                .ToList();

            switch (filtro)
            {
                case 1:
                    Canje.Reverse();
                break;
            }

             var result = Canje.Select(s => new
              {
                  fecharegistro = s.fechaRegistro,
                  s.recompensas.catProductos.descripcion,
                  valorPuntos = s.valorPuntos.ToString("C0").Replace("$", ""),
                  valorPuntosAsInt = s.valorPuntos
              }).ToList();

            return new { success = true, results = result, total = Canje.Sum(s => s.valorPuntos).ToString("C0").Replace("$", "") }.ToJsonResult();
        }

        // GET: /clientes/DetailsBenefits/5
        [ValidatePermissionsAttribute(true)]

        public ActionResult DetailsBenefits(int? id, int? asc)
        {
            Contexto.Configuration.LazyLoadingEnabled = false;

            var Beneficios = Contexto.beneficioAplicados
                .Where(w => w.membresias.clienteId == id)
                .Select(s => new
                {
                    s.fechaRegistro,
                    s.beneficios.descripcion
                }).ToList();

            switch (asc)
            {
                case 1:
                    Beneficios = Beneficios.OrderBy(o => o.fechaRegistro).ToList();
                    break;

                default:
                    Beneficios = Beneficios.OrderByDescending(o => o.fechaRegistro).ToList();
                    break;
            }

            return new { success = true, results = Beneficios.ToList() }.ToJsonResult();
        }
        #endregion
        #region HELPERS 
        private Dataset[] CalcularDataGraficaCiudades(List<VW_ObtenerClientesConMembresia> data)
        {
            var labels = data
                .Select(c => c.Ciudad)
                .Where(c=>c != null)
                .Distinct()
                .ToList();
            var dataset = new List<Dataset>();
            labels.ForEach(categoria =>
            {
                dataset.Add(new Dataset
                {
                    Etiqueta = string.Format("{0} ({1})",categoria, data.Count(f => f.Ciudad == categoria)),
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

        private Resumen CalcularResumenDeUso(RangoFechas rangoFechas)
        {
            var totalCanjesEnPuntos = Contexto.canjePuntos
                    .Where(c => c.catEstatusCanjePuntoId == (int)EstatusCanjePuntos.Canjeado
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.comercios.empresaId == EmpresaUsuario
                        && c.fechaCanje.Value >= rangoFechas.Inicio
                        && c.fechaCanje.Value <= rangoFechas.Fin)
                    .Select(c=> c.valorPuntos)
                    .DefaultIfEmpty(0)
                    .Sum();

            var numeroBeneficiosUsados = Contexto.beneficioAplicados
                    .Where(c => c.sucursales.comercios.empresaId == EmpresaUsuario
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.fechaCompra >= rangoFechas.Inicio
                        && c.fechaCompra <= rangoFechas.Fin)
                    .Count();

            var totalComprasEnPuntos = Contexto.cargaCompras
                    .Where(c => c.fechaCompra >= rangoFechas.Inicio
                        && c.catEstatusCargaCompraId == (int)EstatusCargaCompras.Aplicado
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.comercios.empresaId == EmpresaUsuario
                        && c.fechaCompra <= rangoFechas.Fin)
                    .Select(c => c.puntos)
                    .DefaultIfEmpty(0)
                    .Sum();

            var totalCompras = Contexto.cargaCompras
                .Where(c => c.fechaCompra >= rangoFechas.Inicio
                    && c.catEstatusCargaCompraId == (int)EstatusCargaCompras.Aplicado
                    && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                    && c.comercios.empresaId == EmpresaUsuario
                    && c.fechaCompra <= rangoFechas.Fin)
                .Select(c => c.montoCompra)
                .DefaultIfEmpty(0)
                .Sum();

            var canjes = (decimal?) Contexto.canjePuntos
                   .Where(c => c.fechaCanje.HasValue
                       && c.fechaCanje.Value >= rangoFechas.Inicio
                       && c.catEstatusCanjePuntoId == (int)EstatusCanjePuntos.Canjeado
                       && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                       && c.comercios.empresaId == EmpresaUsuario
                       && c.fechaCanje.Value <= rangoFechas.Fin)
                    .Select(c => c.montoCompra)
                    .DefaultIfEmpty(0)
                   .Sum()
                   .GetValueOrDefault();

            var totalBeneficios = Contexto.beneficioAplicados
                    .Where(c => c.fechaCompra >= rangoFechas.Inicio
                        && ClientesVisibles.Select(d => d.idCliente).Contains(c.membresias.clienteId)
                        && c.sucursales.comercios.empresaId == EmpresaUsuario
                        && c.fechaCompra <= rangoFechas.Fin)
                    .Select(c => c.montoCompra)
                    .DefaultIfEmpty(0)
                    .Sum();

            Resumen resumen = new Resumen
            {
                
                TotalCanjes = canjes ?? 0,
                TotalCanjesEnPuntos = totalCanjesEnPuntos,
                TotalCompras = totalCompras,
                TotalComprasEnPuntos = totalComprasEnPuntos ,
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
        #endregion
    }

}
