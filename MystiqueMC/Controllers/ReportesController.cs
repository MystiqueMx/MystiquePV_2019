using MystiqueMC.Helpers;
using MystiqueMC.Models;
using MystiqueMC.Models.Graficas;
//using Humanizer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [Authorize]
    [ValidatePermissions]
    public class ReportesController : BaseController
    {
        // GET: Reportes
        #region Reportes

        public ActionResult ReporteClientes(string Sucursales, string SearchNombre, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;


            //REGRESA A LA VISTA TODOS LOS REGISTROS
            var ReporteClientes = Contexto.SP_Reporte_Clientes(null, null, empresaId).AsQueryable();

            ReporteClientes = ReporteClientes.OrderByDescending(c => c.FechaRegistro);

            //BUSCA LOS REGISTROS EN UN RANGO DE FECHA DETERMINADO

            if (fecha1.HasValue)
            {
                ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                ReporteClientes = ReporteClientes.Where(w => w.FechaRegistro > fecha1.Value);
            }

            if (fecha2.HasValue)
            {
                ViewBag.fechaFin = fecha2.Value.ToShortDateString();
                ReporteClientes = ReporteClientes.Where(w => w.FechaRegistro < fecha2.Value);
            }
            if (!string.IsNullOrEmpty(SearchNombre))
            {
                ReporteClientes = ReporteClientes.Where(w => w.Nombre.ToUpper().Contains(SearchNombre.ToUpper())
                || w.Paterno.ToUpper().Contains(SearchNombre.ToUpper())
                || w.Materno.ToUpper().Contains(SearchNombre.ToUpper())
                );
            }
            return View(ReporteClientes.ToList());
        }

        public ActionResult ReporteCanjeados(string Canjes, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            if (fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Canjes)).Select(c => c.idSucursal).FirstOrDefault();
            var ReporteCanjeados = Contexto.SP_Reporte_Canje_Productos(IdSucursal, fecha1, fecha2, empresaId).OrderByDescending(o => o.FechaCanje).ToList();
            ViewData["CanjesSelect"] = new SelectList(Contexto.SP_Reporte_Canje_Productos(0, null, null, empresaId).Select(s => s.Sucursal).Distinct());

            return View(ReporteCanjeados);
        }

        public ActionResult ReporteCompras(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            if (fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
            var ReporteCompras = Contexto.SP_Reporte_Registro_Compras(IdSucursal, fecha1, fecha2, empresaId).OrderByDescending(o => o.FechaCompra).ToList();
            ViewData["ComprasSelect"] = new SelectList(ReporteCompras.Select(s => s.Sucursal).Distinct());

            return View(ReporteCompras);
        }

        public ActionResult ReporteBeneficios(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            int empresaId = usuarioFirmado.empresaId;
            if (fecha1 != null) ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
            if (fecha2 != null) ViewBag.fechaFin = fecha2.Value.ToShortDateString();
            var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
            var ReporteBeneficios = Contexto.SP_Reporte_Canje_Beneficios(IdSucursal, fecha1, fecha2, empresaId).OrderByDescending(o => o.fechaCompra).ToList();
            ViewData["BeneficiosSelect"] = new SelectList(Contexto.SP_Reporte_Canje_Beneficios(0, null, null, empresaId).Select(s => s.Sucursal).Distinct());

            return View(ReporteBeneficios);
        }

        public ActionResult ReporteVentasGraficas(DateTime? fecha1, DateTime? fecha2, bool? ventaPor = false)
        {
            try
            {
                ViewData["Sucursales"] = GetSucursales();
                var usuarioFirmado = Session.ObtenerUsuario();
                var comercioId =  usuarioFirmado.confUsuarioComercio.FirstOrDefault();

                List<SelectListItem> lst = new List<SelectListItem>();
                if (ventaPor != null)
                {
                    var x = ventaPor.Value ? ViewBag.Ticket = true : ViewBag.Ticket = false;

                    lst.Add(new SelectListItem() { Text = "TICKET", Value = "false", Selected = ViewBag.Ticket });
                    lst.Add(new SelectListItem() { Text = "IMPORTE", Value = "true", Selected = !ViewBag.Ticket });
                }
                else
                {
                    lst.Add(new SelectListItem() { Text = "TICKET", Value = "false" });
                    lst.Add(new SelectListItem() { Text = "IMPORTE", Value = "true" });
                }
                ViewBag.ventaPor = new SelectList(lst, "Value", "Text");


                var data = Contexto.SP_PV_Reporte_VentasGD(fecha1, fecha2, comercioId != null ? comercioId.comercioId : 0).Select(s => new GraficaVentas
                {
                    IdSucursal = s.cveSucursal.HasValue ? s.cveSucursal.Value : 0,
                    Etiqueta = s.isemana.Value.ToShortDateString() + " - " + s.fsemana.Value.ToShortDateString(),
                    Valor = ventaPor.Value ? (s.ventasTotales ?? 0) : (s.comensales ?? 0),
                    Nombre = s.sucursal,
                    Color = s.color
                }).ToList();

                var dataGrouped = data.GroupBy(s => s.IdSucursal).Select(g => g.ToList()).ToList();

                var labelsG = data.Select(g => g.Etiqueta).Distinct()
                    .ToList();

                var colorsG = data.Select(g => g.Color).Distinct()
                   .ToList();

                var dataset = new List<GraficaLines>();

                dataGrouped.ForEach(sucursal => dataset.Add(new GraficaLines
                {
                    Titulo = sucursal.Select(s => s.Nombre).First(),
                    Data = sucursal.ToArray()
                }));

                ViewBag.Ticket = true;
                ViewBag.fechaInicio = fecha1.HasValue ? fecha1.Value.ToShortDateString() : null;
                ViewBag.fechaFin = fecha2.HasValue ? fecha2.Value.ToShortDateString() : null;
                ViewBag.labelsG = labelsG.ToList();
                ViewBag.colorsG = colorsG.ToList();
                return View(dataset.ToArray());
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }
        #endregion

        #region ReportesL        

        public ActionResult Index()
        {
            try
            {

                return View();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteVentas(string Sucursales, DateTime? fecha1, DateTime? fecha2, bool? ventaPor)
        {
            try
            {
                string viewModel = "[]";
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();

                ViewBag.Ticket = true;
                List<SelectListItem> lst = new List<SelectListItem>();

                if (ventaPor != null)
                {
                    var x = ventaPor.Value ? ViewBag.Ticket = true : ViewBag.Ticket = false;

                    lst.Add(new SelectListItem() { Text = "TICKET", Value = "false", Selected = ViewBag.Ticket });
                    lst.Add(new SelectListItem() { Text = "IMPORTE", Value = "true", Selected = !ViewBag.Ticket });
                }
                else
                {
                    lst.Add(new SelectListItem() { Text = "TICKET", Value = "false" });
                    lst.Add(new SelectListItem() { Text = "IMPORTE", Value = "true" });
                }
                ViewBag.ventaPor = new SelectList(lst, "Value", "Text");



                DataSet dataSet = new DataSet();
                if (fecha1 != null && fecha2 != null)
                {
                    var promedio = Contexto.SP_PV_Reporte_Ventas_promedioTicket(fecha1, fecha2, IdSucursal);
                    ViewBag.promedio = promedio.First();
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringDirecta"].ToString()))
                    {
                        cn.Open();

                        SqlTransaction transaction = cn.BeginTransaction();

                        try
                        {
                            SqlCommand cmd = new SqlCommand("ads.SP_PV_Reporte_VentasP", cn);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@inFechaInicial", fecha1);
                            cmd.Parameters.AddWithValue("@inFechaFinal", fecha2);
                            cmd.Parameters.AddWithValue("@inHoraInicio", "");
                            cmd.Parameters.AddWithValue("@inHoraFin", "");
                            cmd.Parameters.AddWithValue("@deVentas", ventaPor);
                            cmd.Parameters.AddWithValue("@inScucursalId", IdSucursal);

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Transaction = transaction;

                            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                            {
                                #region logica

                                adapter.TableMappings.Add("Table", "##MyTempTableDatos");

                                adapter.Fill(dataSet);

                                #endregion
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                        finally
                        {
                            cn.Close();
                        }
                    }
                    viewModel = JsonConvert.SerializeObject(dataSet.Tables[0]);
                }

                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                ViewData["Sucursales"] = GetSucursales();

                return View(model: viewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteVentasPorHora(string Sucursales, DateTime? fecha1, DateTime? fecha2, bool? ventaPor)
        {
            try
            {
                string viewModel = "[]";
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();

                ViewBag.Ticket = true;
                List<SelectListItem> lst = new List<SelectListItem>();

                if (ventaPor != null)
                {
                    var x = ventaPor.Value ? ViewBag.Ticket = true : ViewBag.Ticket = false;

                    lst.Add(new SelectListItem() { Text = "TICKET", Value = "false", Selected = ViewBag.Ticket });
                    lst.Add(new SelectListItem() { Text = "IMPORTE", Value = "true", Selected = !ViewBag.Ticket });
                }
                else
                {
                    lst.Add(new SelectListItem() { Text = "TICKET", Value = "false" });
                    lst.Add(new SelectListItem() { Text = "IMPORTE", Value = "true" });
                }
                ViewBag.ventaPor = new SelectList(lst, "Value", "Text");



                DataSet dataSet = new DataSet();
                if (fecha1 != null && fecha2 != null)
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringDirecta"].ToString()))
                    {
                        cn.Open();

                        SqlTransaction transaction = cn.BeginTransaction();

                        try
                        {
                            SqlCommand cmd = new SqlCommand("ads.SP_PV_Reporte_Ventas_Por_HorasP", cn);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@inFechaInicial", fecha1);
                            cmd.Parameters.AddWithValue("@inFechaFinal", fecha2);
                            cmd.Parameters.AddWithValue("@deVentas", ventaPor);
                            cmd.Parameters.AddWithValue("@inScucursalId", IdSucursal);

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Transaction = transaction;

                            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                            {
                                #region logica

                                adapter.TableMappings.Add("Table", "##MyTempTableDatos");

                                adapter.Fill(dataSet);

                                #endregion
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                        finally
                        {
                            cn.Close();
                        }
                    }
                    viewModel = JsonConvert.SerializeObject(dataSet.Tables[0]);
                }

                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                ViewData["Sucursales"] = GetSucursales();

                return View(model: viewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteCorteZ(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            try
            {
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
                var ReporteCorteZ = Contexto.SP_PV_Reporte_Cortes_Z(IdSucursal, fecha1, fecha2).ToList();

                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                ViewData["Sucursales"] = GetSucursales();

                return View(ReporteCorteZ);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteCorteDetallado(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            try
            {
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();

                var ReporteConsumoAgrupadoDetalle = Contexto.SP_PV_Reporte_Cortes_Detallado(IdSucursal, fecha1, fecha2).ToList();

                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                var usuarioFirmado = Session.ObtenerUsuario();
                ViewData["Sucursales"] = GetSucursales();

                return View(ReporteConsumoAgrupadoDetalle);
            }
            catch (Exception e)
            {

                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteConsumoAgrupado(string Sucursales, string familia, DateTime? fecha1, DateTime? fecha2)
        {
            try
            {
                
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
                var IdFamilia = Contexto.CategoriaProductos.Where(w => w.descripcion.Equals(familia)).Select(c => c.idCategoriaProducto).FirstOrDefault();

                var ReporteConsumoAgrupado = Contexto.SP_PV_Reporte_Consumo_Agrupado(IdSucursal, fecha1, fecha2, IdFamilia).ToList();//

               

                if (IdFamilia != 0) ViewBag.bools = 1;
                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                ViewData["Sucursales"] = GetSucursales();

                ViewBag.SucursalId = Sucursales;
                if (fecha1 != null)
                    ViewBag.Iniciofecha = fecha1.Value;
                if (fecha2 != null)
                    ViewBag.finFecha = fecha2.Value;

                var usuarioFirmado = Session.ObtenerUsuario();
                int idComercio = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                ViewData["Familias"] = new SelectList(Contexto.CategoriaProductos.Where(s => s.comercioId == idComercio).ToList().Distinct(), "descripcion", "descripcion");

                return View(ReporteConsumoAgrupado);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteConsumoAgrupadoDetalle(string Sucursales, string familia, DateTime? fechaInicio, DateTime? fechaFin, decimal? ventas, int? frecuencia, decimal? porcentaje, int? familias)
        {
            try
            {
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
                var IdFamilia = Contexto.CategoriaProductos.Where(w => w.descripcion.Equals(familia)).Select(c => c.idCategoriaProducto).FirstOrDefault();

                var ReporteConsumoAgrupadoDetalle = Contexto.SP_PV_Reporte_Consumo_Agrupado_Detalle(IdSucursal, fechaInicio, fechaFin, IdFamilia).ToList();//

                if (fechaInicio != null)
                    ViewBag.fechaInicio = fechaInicio.Value.ToShortDateString();
                if (fechaFin != null)
                    ViewBag.fechaFin = fechaFin.Value.ToShortDateString();
                ViewBag.Sucursales = Sucursales;
                ViewBag.familia = familia;

                ViewBag.ventas = ventas;
                ViewBag.frecuencia = frecuencia;
                ViewBag.porcentaje = porcentaje;
                ViewBag.fechaFin = fechaFin;
                ViewBag.fechaInicio = fechaInicio;
                if (familias == 1) ViewBag.familias = familia;

                return View(ReporteConsumoAgrupadoDetalle);
            }
            catch (Exception e)
            {

                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteCancelados(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            try
            {
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
                var ReporteCorteZ = Contexto.SP_PV_Cancelados(IdSucursal, fecha1, fecha2).ToList();

                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                ViewData["Sucursales"] = GetSucursales();

                return View(ReporteCorteZ);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteCxC(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            try
            {
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
                var ReporteCorteZ = Contexto.SP_PV_Reporte_Consumo_Empleado(IdSucursal, fecha1, fecha2).OrderByDescending(o => o.idConsumoEmpleado).ToList();

                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                ViewData["Sucursales"] = GetSucursales();

                return View("ReporteConsumoEmpleado", ReporteCorteZ);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteGastosCaja(string Sucursales, DateTime? fecha1, DateTime? fecha2)
        {
            try
            {
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
                var ReporteCorteZ = Contexto.SP_PV_Reporte_Gastos_Caja(IdSucursal, fecha1, fecha2).ToList();

                if (fecha1 != null)
                    ViewBag.fechaInicio = fecha1.Value.ToShortDateString();
                if (fecha2 != null)
                    ViewBag.fechaFin = fecha2.Value.ToShortDateString();

                ViewData["Sucursales"] = GetSucursales();

                return View(ReporteCorteZ);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Reportes");
            }
        }

        public ActionResult ReporteRecetaProductoConteoManual(string Sucursales, DateTime? fecha1, DateTime? fecha2, string d, bool? unidadMedida)
        {
            try
            {
                ViewBag.suc = Sucursales;
                var IdSucursal = Contexto.sucursales.Where(w => w.nombre.Equals(Sucursales)).Select(c => c.idSucursal).FirstOrDefault();
                //     var reporteRecetaManual = Contexto.SP_PV_Reporte_Receta_Producto_Conteo_Manual(IdSucursal, fecha1, fecha2, unidadMedida).ToList();


                ViewData["Sucursales"] = GetSucursales(Sucursales);
                ViewBag.sucursalId = IdSucursal;
                ViewBag.fecha1 = fecha1?.ToString("dd/MM/yyyy");
                ViewBag.fecha2 = fecha2?.ToString("dd/MM/yyyy");
                ViewBag.unidadMedida = unidadMedida;
                if (Sucursales != null && fecha1.HasValue && fecha2.HasValue)
                {
                    var reporteRecetaManual = Contexto.SP_PV_Reporte_Receta_Producto_Conteo_Manual(IdSucursal, fecha1, fecha2, unidadMedida).ToList();
                    ViewBag.anio = DateTime.Now.Year;
                    ViewBag.SucursalId = Sucursales;

                    return View(reporteRecetaManual.AsEnumerable().Select(c => new ReporteRecetaConteoManual
                    {

                        FechaFinalSemana = c.fechaFinalSemana,
                        FechaInicialSemana = c.fechaInicialSemana,
                        UnidadMedida = c.unidadMedida,
                        IdInsumo = c.idInsumo,
                        Nombre = c.nombre,
                        TotalInventarioManual = c.totalInventarioManual,
                        TotalProductosVenta = c.totalProductosVenta ?? 0,
                        Diferencia = c.diferencia ?? 0
                    }));
                }

                return View(Enumerable.Empty<ReporteRecetaConteoManual>());

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ReporteRecetaProductoConteoManualDetalle(int? Sucursales, DateTime? fecha1, DateTime? fecha2, int? insumoId, string nombre, decimal? TotalProductosVenta, decimal? TotalInventarioManual, decimal? Diferencia, bool? unidadMedida, string suc)
        {
            try
            {
                var reporteRecetaManualDetalle = Contexto.SP_PV_Reporte_Receta_Producto_Conteo_Manual_Detalle(Sucursales, fecha1, fecha2, insumoId).ToList();
                ViewBag.fecha1 = fecha1;
                ViewBag.fecha2 = fecha2;
                ViewBag.SucursalId = suc;
                ViewBag.totalVenta = TotalProductosVenta;
                ViewBag.TotalConteo = TotalInventarioManual;
                ViewBag.TotalDiferencia = Diferencia;
                ViewBag.Nombre = nombre;
                ViewBag.unidadMedida = unidadMedida;

                return View(reporteRecetaManualDetalle);

            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "Home");
            }
        }

        private SelectList GetSucursales(string selected = null)
        {
            #region GetSucursales

            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                return new SelectList(Contexto.sucursales.Where(w => w.comercios.empresaId == usuarioFirmado.empresaId).ToList().Distinct(), "nombre", "nombre", selected);
            }
            catch (Exception e)
            {

                throw e;
            }

            #endregion
        }

        #endregion
    }
}