using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models.Json;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Web.Models.Recetas.ViewModels;

namespace MystiqueMC.Controllers
{
    public class RecetasController : BaseController
    {
        #region GET
        public async Task<ActionResult> Index(string Nombre, string Estatus)
        {
            try
            {
                var recetaProcesado = Contexto.RecetasProcesados
                    .Include(r => r.Insumos)
                    .Where(c => c.activo);
                   
                ViewBag.UnidadMedida = ObtenerUnidadesMedida();
                ViewBag.Productos = ObtenerProductos();
                ViewBag.Insumos = ObtenerInsumos();
                ViewBag.Procesados = ObtenerProcesados();
                ViewBag.TipoProcesados = ObtenerTiposProcesados();
                ViewBag.FamiliaInsumo = ObtenerFamiliaInsumos();
                ViewBag.Nombre = Nombre;
                ViewBag.estatus = true;
                 var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                ViewBag.comercioId = comercioId;
                var categoriaIngrediente = Contexto.CategoriaIngrediente.Where(c => c.comercioId == comercioId).ToList();
                var categoriaUnidadMedida = Contexto.UnidadMedida.Where(c => c.comercioId == comercioId).ToList();
                ViewBag.UnidadMedidaIdMinima = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion");
                ViewBag.UnidadMedidaIdCompra = new SelectList(categoriaUnidadMedida, "idUnidadMedida", "descripcion");

                if (!string.IsNullOrEmpty(Estatus) && Estatus != "2")
                {
                    recetaProcesado = Contexto.RecetasProcesados.Include(r => r.Insumos);

                    bool estatus = Estatus == "1" ? true : false;
                    recetaProcesado = recetaProcesado.Where(w => w.activo == estatus);
                    ViewBag.estatus = estatus;
                }

                if (!string.IsNullOrEmpty(Nombre))
                {
                    recetaProcesado = recetaProcesado.Where(w => w.descripcion.Contains(Nombre));
                }

                ViewBag.sucursaId = Session.ObtenerInventarioSucursal();

                return View(recetaProcesado.ToList());
            }
            catch (Exception e)
            {
                ShowAlertException(e);
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region POST
        public ActionResult EliminarReceta(int id)
        {
            try
            {
                var receta = Contexto.Recetas.Find(id);
                receta.activo = false;
                Contexto.Entry(receta).State = EntityState.Modified;
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
        }

        public ActionResult EliminarProcesado(int id)
        {
            try
            {
                var receta = Contexto.RecetasProcesados.Find(id);
                receta.activo = false;
                Contexto.Entry(receta).State = EntityState.Modified;
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ActivarReceta(int idReceta)
        {
            try
            {
                var recetas = Contexto.RecetasProcesados.Find(idReceta);
                recetas.activo = true;
                Contexto.Entry(recetas).State = EntityState.Modified;
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
        }

        public ActionResult InactivarReceta(int idReceta)
        {
            try
            {
                var recetas = Contexto.RecetasProcesados.Find(idReceta);
                recetas.activo = false;
                Contexto.Entry(recetas).State = EntityState.Modified;
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region AJAX
        // Actualizar receta
        public ActionResult RecetaProducto(int? id, string descripcion, int productoId, int caducidad, decimal merma)
        {
            try
            {
                Recetas receta;
                if (id.HasValue)
                {
                    receta = Contexto.Recetas.First(c => c.idReceta == id.Value);
                    receta.descripcion = descripcion;
                    receta.productoId = productoId;
                    receta.diasCaducidad = caducidad;
                    receta.mermaPermitida = merma;
                    Contexto.Entry(receta).State = EntityState.Modified;
                }
                else
                {
                    receta = Contexto.Recetas.Add(new Recetas
                    {
                        activo = true,
                        descripcion = descripcion,
                        diasCaducidad = caducidad,
                        fechaRegistro = DateTime.Now,
                        productoId = productoId,
                        usuarioRegistroId = IdUsuarioActual,
                        mermaPermitida = merma,
                    });
                }
                Contexto.SaveChanges();
                return Json(new { id = receta.idReceta });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }

        // Obtener receta
        public ActionResult Receta(int id)
        {
            try
            {
                var receta = Contexto.Recetas
                    .Include(c => c.DetalleRecetaProducto)
                    .Include(c => c.DetalleRecetaProducto.Select(d => d.UnidadMedida))
                    .Include(c => c.DetalleRecetaProducto.Select(d => d.Insumos))
                    .First(c => c.idReceta == id);

                return Json(new
                {
                    id,
                    receta.descripcion,
                    receta.diasCaducidad,
                    receta.mermaPermitida,
                    receta.productoId,
                    insumos = receta.DetalleRecetaProducto.Select(c => new
                    {
                        id = c.insumoId,
                        c.cantidad,
                        c.unidadMedidaId,
                        unidadMedida = c.UnidadMedida.descripcion,
                        insumo = c.Insumos.nombre,
                        c.insumoId,
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }

        // Actualizar detalle receta
        public ActionResult DetalleReceta(int idReceta, int insumoDetalle, decimal cantidad, int unidad)
        {
            try
            {

                if (Contexto.DetalleRecetaProducto.Any(c => c.recetaId == idReceta && c.insumoId == insumoDetalle))
                {
                    var detalle = Contexto.DetalleRecetaProducto.Find(idReceta, insumoDetalle);
                    if (detalle == null) throw new DataException($" Contexto.DetalleRecetaProducto.Find({idReceta}, {insumoDetalle})");
                    detalle.cantidad = cantidad;
                    detalle.unidadMedidaId = unidad;
                }
                else
                {
                    Contexto.DetalleRecetaProducto.Add(new DetalleRecetaProducto
                    {
                        cantidad = cantidad,
                        insumoId = insumoDetalle,
                        recetaId = idReceta,
                        unidadMedidaId = unidad,
                    });
                }

                Contexto.SaveChanges();

                var detalleReceta = Contexto.DetalleRecetaProducto
                    .Include(c => c.UnidadMedida)
                    .Include(c => c.Insumos)
                    .First(c => c.recetaId == idReceta && c.insumoId == insumoDetalle);

                return Json(new
                {
                    detalleReceta.cantidad,
                    detalleReceta.unidadMedidaId,
                    unidadMedida = detalleReceta.UnidadMedida.descripcion,
                    insumo = detalleReceta.Insumos.nombre,
                    detalleReceta.insumoId,
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }

        // Actualizar detalle receta 2
        public ActionResult DetalleReceta2(int? idReceta, int productoId, int insumoDetalle, decimal cantidad, int unidad)
        {
            try
            {

                if (Contexto.DetalleRecetaProducto.Any(c => c.recetaId == idReceta && c.insumoId == insumoDetalle))
                {
                    var detalle = Contexto.DetalleRecetaProducto.Find(idReceta, insumoDetalle);
                    if (detalle == null) throw new DataException($" Contexto.DetalleRecetaProducto.Find({idReceta}, {insumoDetalle})");
                    detalle.cantidad = cantidad;
                    detalle.unidadMedidaId = unidad;
                }
                else
                {
                    if (idReceta.HasValue || Contexto.Recetas.Any(c=>c.productoId == productoId))
                    {
                        var id = idReceta ?? Contexto.Recetas.First(c => c.productoId == productoId).idReceta;
                        Contexto.DetalleRecetaProducto.Add(new DetalleRecetaProducto
                        {
                            cantidad = cantidad,
                            insumoId = insumoDetalle,
                            recetaId = id,
                            unidadMedidaId = unidad,
                        });
                        var insumo = Contexto.Insumos.FirstOrDefault(f => f.idInsumo == insumoDetalle);
                        if (insumo != null)
                        {
                            insumo.esIngrediente = true;
                        }
                    }
                    else
                    {
                        var receta = Contexto.Recetas.Add(new Recetas
                        {
                            activo = true,
                            diasCaducidad = 0,
                            mermaPermitida = 0,
                            usuarioRegistroId = IdUsuarioActual,
                            fechaRegistro = DateTime.Now,
                            productoId = productoId,
                        });
                        Contexto.DetalleRecetaProducto.Add(new DetalleRecetaProducto
                        {
                            cantidad = cantidad,
                            insumoId = insumoDetalle,
                            Recetas = receta,
                            unidadMedidaId = unidad,
                        });
                        var insumo = Contexto.Insumos.FirstOrDefault(f => f.idInsumo == insumoDetalle);
                        if (insumo != null)
                        {
                            insumo.esIngrediente = true;
                        }
                    }
                   
                }

                Contexto.SaveChanges();
                

                return Json("OK");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }

        // Eliminar detalle receta
        public ActionResult EliminarDetalleReceta(int idReceta, int insumoId)
        {
            try
            {

                if (Contexto.DetalleRecetaProducto.Any(c => c.recetaId == idReceta && c.insumoId == insumoId))
                {
                    Contexto.DetalleRecetaProducto.Remove(Contexto.DetalleRecetaProducto.Find(idReceta, insumoId));
                    Contexto.SaveChanges();
                    return Json("Ok");
                }
                else
                {
                    throw new DataException($"Contexto.DetalleRecetaProducto.Any(c => c.recetaId == {idReceta} && c.insumoId == {insumoId})");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }

        // Actualizar procesado
        public ActionResult RecetaProcesado(int? id, string descripcionProcesado, int? productoProcesado, int unidadMedidaIdCompra, int equivalencia, int unidadMedidaIdMinima, decimal cantidadProcesados, int insumoFamilia)
        {
            try
            {
                RecetasProcesados receta;
                if (id.HasValue)
                {
                    receta = Contexto.RecetasProcesados.First(c => c.idRecetasProcesado == id.Value);
                    receta.descripcion = descripcionProcesado.ToUpper();
                    receta.diasCaducidad = 0;
                    receta.mermaPermitida = 0;
                    receta.tipoRecetaId = 7;
                    receta.activo = true;
                    receta.cantidadProcesado = cantidadProcesados;
                    Contexto.Entry(receta).State = EntityState.Modified;


                    Insumos insumo = Contexto.Insumos.First(c => c.idInsumo == receta.insumoId);
                    insumo.nombre = descripcionProcesado.ToUpper();
                    insumo.activo = true;
                    insumo.categoriaInsumoId = insumoFamilia;
                    insumo.equivalencia = equivalencia;
                    insumo.unidadMedidaIdCompra = unidadMedidaIdCompra;
                    insumo.unidadMedidaIdMinima = unidadMedidaIdMinima;
                    Contexto.Entry(insumo).State = EntityState.Modified;
                }
                else
                {
                    var usuarioFirmado = Session.ObtenerUsuario();
                    int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                    Insumos insumo = Contexto.Insumos.Add(new Insumos
                    {
                        nombre = descripcionProcesado.ToUpper(),
                        esProcesado = true,
                        activo = true,
                        unidadMedidaIdCompra = unidadMedidaIdCompra,
                        unidadMedidaIdMinima = unidadMedidaIdMinima,
                        equivalencia = equivalencia,
                        conteoFisico = false,
                        aplicaInventario = false,
                        esIngrediente = false,
                        esAgrupador = false,
                        comercioId = comercioId,
                        fechaRegistro = DateTime.Now,
                        categoriaInsumoId = insumoFamilia,
                        usuarioRegistroId = UsuarioActual.idUsuario
                    });
                    Contexto.Insumos.Add(insumo);

                    receta = Contexto.RecetasProcesados.Add(new RecetasProcesados
                    {
                        activo = true,
                        descripcion = descripcionProcesado.ToUpper(),
                        diasCaducidad = 0,
                        fechaRegistro = DateTime.Now,
                        Insumos = insumo,
                        usuarioRegistroId = IdUsuarioActual,
                        mermaPermitida = 0,
                        tipoRecetaId = 7,
                        cantidadProcesado = cantidadProcesados
                    });
                }
                Contexto.SaveChanges();
                return Json(new { id = receta.idRecetasProcesado });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }
        // Obtener procesado
        public ActionResult Procesado(int id)
        {
            try
            {
                var receta = Contexto.RecetasProcesados
                    .Include(c => c.DetalleRecetaProcesados)
                    .Include(c => c.DetalleRecetaProcesados.Select(d => d.UnidadMedida))
                    .Include(c => c.DetalleRecetaProcesados.Select(d => d.Insumos))
                    .First(c => c.idRecetasProcesado == id);

                return Json(new
                {
                    id,
                    receta.descripcion,
                    receta.insumoId,
                    receta.Insumos.unidadMedidaIdCompra,
                    receta.Insumos.unidadMedidaIdMinima,
                    receta.Insumos.equivalencia,
                    receta.cantidadProcesado,
                    receta.Insumos.categoriaInsumoId,
                    insumos = receta.DetalleRecetaProcesados.Select(c => new
                    {
                        id = c.insumoId,
                        c.cantidad,
                        c.unidadMedidaId,
                        unidadMedida = c.UnidadMedida.descripcion,
                        insumo = c.Insumos.nombre,                       
                        c.insumoId,
                        receta.descripcion
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }
        // Actualizar detalle procesado
        public ActionResult DetalleProcesado(int idProcesadoInsumo, int insumoDetalleProcesado, decimal cantidadProcesado, int unidadProcesado)
        {
            try
            {

                if (Contexto.DetalleRecetaProcesados.Any(c => c.recetasProcesadoId == idProcesadoInsumo && c.insumoId == insumoDetalleProcesado))
                {
                    var detalle = Contexto.DetalleRecetaProcesados.Find(idProcesadoInsumo, insumoDetalleProcesado);
                    if (detalle == null) throw new DataException($" Contexto.DetalleRecetaProducto.Find({idProcesadoInsumo}, {insumoDetalleProcesado})");
                    detalle.cantidad = cantidadProcesado;
                    detalle.unidadMedidaId = unidadProcesado;
                }
                else
                {
                    Contexto.DetalleRecetaProcesados.Add(new DetalleRecetaProcesados
                    {
                        cantidad = cantidadProcesado,
                        insumoId = insumoDetalleProcesado,
                        recetasProcesadoId = idProcesadoInsumo,
                        unidadMedidaId = unidadProcesado,
                    });
                }

                Contexto.SaveChanges();

                var detalleReceta = Contexto.DetalleRecetaProcesados
                    .Include(c => c.UnidadMedida)
                    .Include(c => c.Insumos)
                    .First(c => c.recetasProcesadoId == idProcesadoInsumo && c.insumoId == insumoDetalleProcesado);

                return Json(new
                {
                    detalleReceta.cantidad,
                    detalleReceta.unidadMedidaId,
                    unidadMedida = detalleReceta.UnidadMedida.descripcion,
                    insumo = detalleReceta.Insumos.nombre,
                    detalleReceta.insumoId,
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }

        // Eliminar detalle procesado
        public ActionResult EliminarDetalleProcesado(int idProcesado, int insumoId)
        {
            try
            {

                if (Contexto.DetalleRecetaProcesados.Any(c => c.recetasProcesadoId == idProcesado && c.insumoId == insumoId))
                {
                    Contexto.DetalleRecetaProcesados.Remove(Contexto.DetalleRecetaProcesados.Find(idProcesado, insumoId));
                    Contexto.SaveChanges();
                    return Json("Ok");
                }
                else
                {
                    throw new DataException($"Contexto.DetalleRecetaProducto.Any(c => c.recetaId == {idProcesado} && c.insumoId == {insumoId})");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return HttpNotFound();
            }
        }

        public ActionResult ObtenerUnidadProcesado(int idInsumo)
        {
            try
            {
                var insumo = Contexto.Insumos
                    .Where(c => c.idInsumo == idInsumo)
                    .Select(c => new ItemListadoInsumos
                    {
                        Idinsumo = c.idInsumo,
                        IdUnidadMedida = c.unidadMedidaIdMinima,
                        Descripcion = c.UnidadMedida.descripcion
                    })
                    .ToList();

                return Json(new ListadoInsumos { exito = true, resultado = insumo });
            }
            catch (Exception e)
            {
                return Json(new ListadoInsumos { exito = false });
            }
        }
        #endregion

        #region HELPERS
        private SelectListItem[] ObtenerProcesados()
        {
            return InsumosVisibles
                .Where(c => c.esProcesado)
                .Select(c => new SelectListItem
                {
                    Text = c.nombre,
                    Value = c.idInsumo.ToString(),
                }).ToArray();
        }
        private SelectListItem[] ObtenerInsumos()
        {
            return InsumosVisibles
                .Where(c =>c .esProcesado == false)
                .Select(c => new SelectListItem
                {
                    Text = c.nombre,
                    Value = c.idInsumo.ToString(),
                }).ToArray();
        }
        private SelectListItem[] ObtenerProductos()
        {
            return ProductosVisibles2
                .Select(c => new SelectListItem
                {
                    Text = c.nombre,
                    Value = c.idProducto.ToString(),
                }).ToArray();
        }
        private SelectListItem[] ObtenerUnidadesMedida()
        {
            return ComerciosFirmados
                .Include(c => c.UnidadMedida)
                .SelectMany(c => c.UnidadMedida)
                .Select(c => new SelectListItem
                {
                    Text = c.descripcion,
                    Value = c.idUnidadMedida.ToString(),
                }).ToArray();
        }

        private SelectListItem[] ObtenerTiposProcesados()
        {
            return ComerciosFirmados
                .Include(c => c.TipoRecetas)
                .SelectMany(c => c.TipoRecetas)
                .Select(c => new SelectListItem
                {
                    Value = c.idTipoReceta.ToString(),
                    Text = c.descripcion
                })
                .ToArray();
        }

        public SelectListItem[] ObtenerFamiliaInsumos()
        {
            return Contexto.CategoriaInsumo
                .Select(c => new SelectListItem
                {
                    Value = c.idCategoriaInsumo.ToString(),
                    Text = c.descripcion
                }).ToArray();
        }
        #endregion


    }
}