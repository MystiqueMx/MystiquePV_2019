using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.Files;
using WebApp.Web.Models.Productos;
using WebApp.Web.Models.Productos.Requests;
using WebApp.Web.Models.Productos.ViewModels;

namespace MystiqueMC.Controllers
{
    public class MenuController : BaseController
    {
        private string ServerPath => Server.MapPath(@"~");
        private const string IMAGE_EXTENSION = ".png";

        #region GET
        // GET: Productos
        public ActionResult Index(int? CategoriaProductoId)
        {
            return RedirectToAction("Home");
            var productos = Contexto.Productos
                .Include(c => c.CategoriaProductos)
                .Include(c => c.VariedadProductos)
                .Include(c => c.ProductoTieneReceta)
                .Include(c => c.Recetas)
                .Where(c => c.CategoriaProductos != null && ComerciosFirmados.Contains(c.comercios));
            ViewBag.familia = CategoriaProductoId;

            productos = productos.Where(w => true).OrderBy(c => c.CategoriaProductos.descripcion);

            if (CategoriaProductoId.HasValue)
            {
                productos = productos.Where(w => w.categoriaProductoId == CategoriaProductoId);
            }

            var categoriaProducto = ComerciosFirmados
                .Include(c => c.CategoriaProductos)
                .SelectMany(c => c.CategoriaProductos)
                .ToArray();

            ViewBag.CategoriaProductoId = new SelectList(categoriaProducto.OrderBy(c=>c.descripcion), "idCategoriaProducto", "descripcion", CategoriaProductoId);
            ViewBag.UnidadMedida = ObtenerUnidadesMedida();
            ViewBag.Productos = ObtenerProductos();
            ViewBag.Insumos = ObtenerInsumos();
            ViewBag.Procesados = ObtenerProcesados();
            ViewBag.TipoProcesados = ObtenerTiposProcesados();

            var viewmodel = productos.ToArray().OrderBy(c=>c.clave).OrderBy(c => c.CategoriaProductos.descripcion).ToArray();
            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult Home(int? enabled, int? f)
        {
            var productos = Contexto.Productos
                .Include(w => w.CategoriaProductos)
                .Include(w => w.VariedadProductos)
                .Include(w => w.ProductoTieneReceta)
                .Include(w => w.AgrupadorInsumos)
                .Include(w => w.ConfiguracionArmadoProductos)
                .Include(w => w.ConfiguracionArmadoProductos.Select(d => d.AgrupadorInsumos))
                .Include(w => w.Recetas)
                .Include(w => w.ProductoEstaConfigurado)
                .Include("AgrupadorInsumos.ConfiguracionArmadoProductos")
                .Include("Recetas.DetalleRecetaProducto.Insumos")
                .Include("Recetas.DetalleRecetaProducto.UnidadMedida")
                .Where(w => w.CategoriaProductos != null 
                    && ComerciosFirmados.Contains(w.comercios));

            productos = productos.Where(w => true).OrderBy(w => w.CategoriaProductos.descripcion);

            if (enabled.HasValue)
            {
                ViewBag.enabled = enabled.Value;
                switch (enabled)
                {
                    case 1:
                        productos = productos.Where(w => w.activo);
                        break;
                    case 0:
                        productos = productos.Where(w => !w.activo);
                        break;
                    default:
                        break;
                }
                
            }
            else
            {
                ViewBag.enabled = 1;
                productos = productos.Where(w => w.activo);
            }

            if (f.HasValue)
            {
                ViewBag.f = f.Value;
                ViewBag.familia = f.Value;
                productos = productos.Where(w => w.categoriaProductoId == f.Value);
            }

            var categoriaProducto = ComerciosFirmados
                .Include(w => w.CategoriaProductos)
                .SelectMany(w => w.CategoriaProductos)
                .ToArray();

            ViewBag.f = new SelectList(categoriaProducto.OrderBy(c => c.descripcion), "idCategoriaProducto", "descripcion", f);
            ViewBag.UnidadMedida = ObtenerUnidadesMedida();
            ViewBag.Productos = ObtenerProductos();
            ViewBag.Insumos = ObtenerInsumos();
            ViewBag.Procesados = ObtenerProcesados();
            ViewBag.TipoProcesados = ObtenerTiposProcesados();

            var viewmodel = productos.GroupBy(w => w.CategoriaProductos.descripcion).OrderBy(w => w.Key).ToDictionary(w => w.Key);
            return View(viewmodel);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            #region Create
            var catalogos = ComerciosFirmados
                .Include(c => c.AreaPreparacion)
                .Include(c => c.CategoriaProductos)
                .ToArray();
            var areaPreparacion = catalogos.SelectMany(c => c.AreaPreparacion).ToArray();
            var categoriaProducto = catalogos.SelectMany(c => c.CategoriaProductos).ToArray();
            ViewBag.AreaPreparacionId = new SelectList(areaPreparacion.OrderBy(c=>c.descripcion), "idAreaPreparacion", "descripcion");
            ViewBag.CategoriaProductoId = new SelectList(categoriaProducto.OrderBy(c => c.descripcion), "idCategoriaProducto", "descripcion");
            return View(new ProductosViewModel());
            #endregion
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int? id, int familia)
        {
            #region Edit
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductosViewModel viewModel = new ProductosViewModel();
            var productos = Contexto.Productos
                .Include(c => c.ProductoTieneReceta)
                .Include(c => c.VariedadProductos)
                .Include(c => c.Recetas)
                .Include(c=>c.Recetas.Select(d=>d.DetalleRecetaProducto))
                .Include("Recetas.DetalleRecetaProducto.UnidadMedida")
                .Include("Recetas.DetalleRecetaProducto.Insumos")
                .Include(c => c.ConfiguracionArmadoProductos)
                .Include(c => c.ConfiguracionArmadoProductos
                        .Select(d => d.AgrupadorInsumos))
                .Include(c => c.ConfiguracionArmadoProductos
                        .Select(d => d.AgrupadorInsumos)
                        .Select(e => e.Productos))
                .Include(c => c.ConfiguracionArmadoProductos
                        .Select(d => d.AgrupadorInsumos)
                        .Select(e => e.InsumoProductos))
                .First(c => c.idProducto == id);
            ViewBag.familia = familia;
            ViewBag.NombreProducto = productos.nombre;
            viewModel.IdProducto = productos.idProducto;
            viewModel.Nombre = productos.nombre;
            viewModel.Nombre = productos.nombre;
            viewModel.Precio = productos.precio;
            viewModel.Activo = productos.activo;
            viewModel.MermaPermitida = productos.mermaPermitida;
            viewModel.Clave = productos.clave;
            viewModel.Principal = productos.principal;
            viewModel.ArmarCobro = !productos.armarCobroMostrador;
            viewModel.AreaPreparacionId = productos.areaPreparacionId;
            viewModel.CategoriaProductoId = productos.categoriaProductoId ?? 0;
            viewModel.indice = productos.indice;
            viewModel.Imagen = productos.urlImagen;
            viewModel.imagenApp = productos.urlImagenApp;
            viewModel.IdReceta = productos.Recetas.FirstOrDefault()?.idReceta;
            viewModel.DetalleRecetas = productos.Recetas.FirstOrDefault()?.DetalleRecetaProducto.ToList() ?? new List<DetalleRecetaProducto>();
            viewModel.TieneReceta = productos.ProductoTieneReceta?.Any(c => c.tieneReceta) ?? false;
            if(productos.esCombo && productos.esGeneral.GetValueOrDefault(false))
            {
                viewModel.Tipo = 2;
            }
            else if (productos.esCombo)
            {
                viewModel.Tipo = 3;
            }
            else
            {
                viewModel.Tipo = 1;
            }

            VariedadesViewModel variedad;
            List<VariedadesViewModel> Variedades = new List<VariedadesViewModel>();
            foreach (var varAux in productos.VariedadProductos)
            {
                variedad = new VariedadesViewModel();
                variedad.Nombre = varAux.descripcion;
                variedad.Id = varAux.idVariedadProducto;
                variedad.Imagen = varAux.imagen;
                Variedades.Add(variedad);
            }
            viewModel.Variedades = Variedades;

            if (productos.esCombo)
            {
                viewModel.TipoProducto = productos.esGeneral == true ? 2 : 3;
            }
            else
            {
                viewModel.TipoProducto = 1;
            }
            var catalogos = ComerciosFirmados
                .Include(c => c.AreaPreparacion)
                .Include(c => c.CategoriaProductos)
                .ToArray();
            var areaPreparacion = catalogos.SelectMany(c => c.AreaPreparacion).ToArray();
            var categoriaProducto = catalogos.SelectMany(c => c.CategoriaProductos).ToArray();
            ViewBag.AreaPreparacionId = new SelectList(areaPreparacion.OrderBy(c => c.descripcion), "idAreaPreparacion", "descripcion", viewModel.AreaPreparacionId);
            ViewBag.CategoriaProductoId = new SelectList(categoriaProducto.OrderBy(c => c.descripcion), "idCategoriaProducto", "descripcion", viewModel.CategoriaProductoId);
            ViewBag.UnidadMedida = ObtenerUnidadesMedida();
            ViewBag.Productos = ObtenerProductos();
            ViewBag.Insumos = ObtenerInsumos();
            ViewBag.Procesados = ObtenerProcesados();
            ViewBag.TipoProcesados = ObtenerTiposProcesados();

            ViewBag.ProductoId = id;
            SelectListItem[] opcionesInsumosProductos;
                opcionesInsumosProductos = Contexto.Productos
                    .Where(c => c.categoriaProductoId.HasValue && ComerciosFirmados.Contains(c.comercios))
                    .Select(c => new SelectListItem
                    {
                        Value = c.idProducto.ToString(),
                        Text = c.nombre,
                    }).ToArray();
            if (productos.esCombo)
            {
                var config = new ConfigurarProductoViewModel
                {
                    IdProducto = productos.idProducto,
                    Nombre = productos.nombre,
                    Tipo = productos.esGeneral ?? true ? 2 : 3,
                    Agrupadores = productos.ConfiguracionArmadoProductos.Select(c => new Agrupador
                    {
                        Id = c.idConfiguracionArmadoProducto,
                        Descripcion = c.AgrupadorInsumos.descripcion,
                        Cantidad = c.cantidad,
                        CostoExtra = c.AgrupadorInsumos?.Productos?.precio,
                        DebeConfirmarPorSeparado = c.AgrupadorInsumos.confirmarPorSeparado,
                        PuedeAgregarExtra = c.AgrupadorInsumos.agregarExtra,
                        Indice = c.AgrupadorInsumos.indice,
                        Opciones = c.AgrupadorInsumos.InsumoProductos.Select(d => new OpcionAgrupador
                        {
                            Id = d.idInsumoProducto,
                            Insumo = d.Insumos,
                            Producto = d.Productos,

                        }).ToArray()
                    }).ToArray()
                };
                viewModel.Configuracion = config;
            }
            else
            {
                viewModel.Configuracion = new ConfigurarProductoViewModel();
            }
            ViewBag.OpcionesInsumosProductos = opcionesInsumosProductos;
            // Obtener precios por sucursal
            List<PreciosSucursalesViewModel> preciosPorSucursal;
            preciosPorSucursal = Contexto.SucursalProductos
                .Where(c => c.productoId == id && c.sucursales.comercios.empresaId == UsuarioActual.empresaId)
                .Select(c => new PreciosSucursalesViewModel
                {
                    idSucursalProducto = c.idSucursalProducto,
                    NombreSucursal = c.sucursales.nombre,
                    Precio = c.precio,
                    activo = c.activo,
                    fechaRegistro = c.fechaRegistro
                }).ToList();

            if (preciosPorSucursal.Any())
            {
                viewModel.PreciosSucursales = preciosPorSucursal;
            }
            return View(viewModel);
            #endregion
        }

        public ActionResult Variedades(int id)
        {
            #region Variedades
            KardexProductoViewModel viewModel = new KardexProductoViewModel();
            try
            {
                var producto = ProductosVisibles2
                    .Include(c => c.VariedadProductos)
                    .First(c => c.idProducto == id);

                viewModel.VariedadProductos = producto.VariedadProductos.ToList();
                viewModel.ProductoId = producto.idProducto;
                viewModel.Descripción = producto.nombre;
                return View(viewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Index");
            }
            #endregion
        }
        public ActionResult Configurar(int id, int type)
        {
            #region  Configurar
            try
            {
                var producto = Contexto.Productos
                        .Include(c => c.ConfiguracionArmadoProductos)
                        .Include(c => c.ConfiguracionArmadoProductos
                                .Select(d => d.AgrupadorInsumos))
                        .Include(c => c.ConfiguracionArmadoProductos
                                .Select(d => d.AgrupadorInsumos)
                                .Select(e => e.Productos))
                        .Include(c => c.ConfiguracionArmadoProductos
                                .Select(d => d.AgrupadorInsumos)
                                .Select(e => e.InsumoProductos))
                        .First(c => c.idProducto == id);

                SelectListItem[] opcionesInsumosProductos;
                if (type == 2)
                {
                    opcionesInsumosProductos = Contexto.Productos
                        .Where(c => c.categoriaProductoId.HasValue && ComerciosFirmados.Contains(c.comercios))
                        .Select(c => new SelectListItem
                        {
                            Value = c.idProducto.ToString(),
                            Text = c.nombre,
                        }).ToArray();
                }
                else
                {
                    opcionesInsumosProductos = InsumosVisibles
                        .Select(c => new SelectListItem
                        {
                            Value = c.idInsumo.ToString(),
                            Text = c.nombre,
                        }).ToArray();
                }

                var viewmodel = new ConfigurarProductoViewModel
                {
                    IdProducto = producto.idProducto,
                    Nombre = producto.nombre,
                    Tipo = type,
                    Agrupadores = producto.ConfiguracionArmadoProductos.Select(c => new Agrupador
                    {
                        Id = c.idConfiguracionArmadoProducto,
                        Descripcion = c.AgrupadorInsumos.descripcion,
                        Cantidad = c.cantidad,
                        CostoExtra = c.AgrupadorInsumos?.Productos?.precio,
                        DebeConfirmarPorSeparado = c.AgrupadorInsumos.confirmarPorSeparado,
                        PuedeAgregarExtra = c.AgrupadorInsumos.agregarExtra,
                        Indice = c.AgrupadorInsumos.indice,
                        Opciones = c.AgrupadorInsumos.InsumoProductos.Select(d => new OpcionAgrupador
                        {
                            Id = d.idInsumoProducto,
                            Insumo = d.Insumos,
                            Producto = d.Productos,

                        }).ToArray()
                    }).ToArray()
                };

                ViewBag.OpcionesInsumosProductos = opcionesInsumosProductos;

                if (viewmodel.Tipo == 3)
                {
                    var usuarioFirmado = Session.ObtenerUsuario();
                    int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                    var categoriaIngrediente = Contexto.CategoriaIngrediente.Where(c => c.comercioId == comercioId).ToList();

                    var agrupadorInsumo = Contexto.AgrupadorInsumos.Where(w => w.productoId == id && w.comercioId == comercioId);
                    if(agrupadorInsumo != null && agrupadorInsumo.Count() >= 1)
                    {
                        foreach (var item in agrupadorInsumo)
                        {
                            categoriaIngrediente = categoriaIngrediente.Where(p => p.descripcion != item.descripcion).ToList();
                        }
                    }

                    
                    ViewBag.CategoriaIngredienteId = new SelectList(categoriaIngrediente, "idCategoriaIngrediente", "descripcion");
                }
           

                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                return RedirectToAction("Index");
            }
            #endregion
        }
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductosViewModel viewModel, int? indicador)
        {
            #region
            try
            {
                if (ModelState.IsValid)
                {
                    bool combo = false;
                    bool general = true;
                    if (viewModel.TipoProducto == 1)
                    {
                        combo = false;
                        general = true;
                    }
                    if (viewModel.TipoProducto == 2)
                    {
                        combo = true;
                        general = true;
                    }
                    if (viewModel.TipoProducto == 3)
                    {
                        combo = true;
                        general = false;
                    }


                    var producto = Contexto.Productos.Add(new Productos
                    {
                        comercioId = ComerciosFirmados.First().idComercio,
                        nombre = viewModel.Nombre,
                        precio = viewModel.Precio,
                        activo = true,
                        mermaPermitida = viewModel.MermaPermitida,
                        esCombo = combo,
                        esGeneral = general,
                        usuarioRegistroId = IdUsuarioActual,
                        fechaRegistro = DateTime.Now,
                        armarCobroMostrador = !viewModel.ArmarCobro,
                        clave = viewModel.Nombre.Substring(0, viewModel.Nombre.Length > 20 ? 20 : viewModel.Nombre.Length),
                        areaPreparacionId = viewModel.AreaPreparacionId,
                        categoriaProductoId = viewModel.CategoriaProductoId,
                        indice = 0,
                        urlImagen = viewModel.Imagen,
                        urlImagenApp = viewModel.imagenApp,
                        principal = viewModel.Principal,
                    });

                    Contexto.ProductoTieneReceta.Add(new ProductoTieneReceta
                    {
                        Productos = producto,
                        tieneReceta = true,
                    });

                   /* var sucursales = SucursalesActuales
                        .Select(c => c.idSucursal)
                        .ToArray();*/
                    
                    var sucursales = ComercioSucursales
                        .Select(c => c.idSucursal)
                        .ToArray();
                    var sucursalProductos = sucursales.Select(c => new SucursalProductos
                    {
                        activo = true,
                        fechaRegistro = producto.fechaRegistro,
                        precio = producto.precio,
                        Productos = producto,
                        sucursalId = c,
                        usuarioRegistroId = producto.usuarioRegistroId,

                    }).ToArray();
                    if (sucursalProductos is SucursalProductos[] array && array.Any())
                    {
                        Contexto.SucursalProductos.AddRange(sucursalProductos);
                    }
                    Contexto.SaveChanges();


                    return RedirectToAction("Edit", new { id = producto.idProducto, familia = producto.categoriaProductoId });
                    
                }
                else
                {
                    var catalogos = ComerciosFirmados
                        .Include(c => c.AreaPreparacion)
                        .Include(c => c.CategoriaProductos)
                        .ToArray();
                    var areaPreparacion = catalogos.SelectMany(c => c.AreaPreparacion).ToArray();
                    var categoriaProducto = catalogos.SelectMany(c => c.CategoriaProductos).ToArray();
                    ViewBag.AreaPreparacionId = new SelectList(areaPreparacion.OrderBy(c => c.descripcion), "idAreaPreparacion", "descripcion", viewModel.AreaPreparacionId);
                    ViewBag.CategoriaProductoId = new SelectList(categoriaProducto.OrderBy(c => c.descripcion), "idCategoriaProducto", "descripcion", viewModel.CategoriaProductoId);
                    return View(viewModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return View(viewModel);
            #endregion
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductosViewModel viewModel, int familiaId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var producto = ProductosVisibles2
                        .Include(c => c.ProductoTieneReceta)
                        .First(c => c.idProducto == viewModel.IdProducto);

                   
                    bool esCombo = false;
                    bool esGeneral = true;
                    if (viewModel.TipoProducto == 1)
                    {
                        esCombo = false;
                        esGeneral = true;
                    }
                    if (viewModel.TipoProducto == 2)
                    {
                        esCombo = true;
                        esGeneral = true;
                    }
                    if (viewModel.TipoProducto == 3)
                    {
                        esCombo = true;
                        esGeneral = false;
                    }


                    producto.nombre = viewModel.Nombre;
                    producto.precio = viewModel.Precio;
                    producto.mermaPermitida = viewModel.MermaPermitida;
                    producto.clave = viewModel.Nombre.Substring(0, viewModel.Nombre.Length > 20 ? 20 : viewModel.Nombre.Length);
                    producto.armarCobroMostrador = !viewModel.ArmarCobro;
                    producto.areaPreparacionId = viewModel.AreaPreparacionId;
                    producto.categoriaProductoId = viewModel.CategoriaProductoId;
                    producto.indice = 0;
                    producto.urlImagen = viewModel.Imagen;
                    producto.urlImagenApp = viewModel.imagenApp;
                    producto.principal = false;

                    if (producto.esCombo != esCombo)
                    {
                        producto.esCombo = esCombo;
                        if (esCombo)
                        {
                            var variedades = Contexto.VariedadProductos.Where(c => c.productoId == viewModel.IdProducto);
                            if(variedades?.Any() ?? false)
                            {
                                Contexto.VariedadProductos.RemoveRange(variedades);
                            }
                        }
                        else
                        {
                            var config = Contexto.ConfiguracionArmadoProductos
                            .Where(c => c.productoId == viewModel.IdProducto)
                            .ToArray();
                            if (config?.Any() ?? false)
                            {
                                Contexto.ConfiguracionArmadoProductos.RemoveRange(config);
                            }
                        }
                    }
                    if (producto.esGeneral != esGeneral)
                    {
                        producto.esGeneral = esGeneral;
                    }

                    if (producto.ProductoTieneReceta?.Any() ?? false)
                    {
                        producto.ProductoTieneReceta.First().tieneReceta = viewModel.TieneReceta;
                        Contexto.Entry(producto.ProductoTieneReceta.First()).State = EntityState.Modified;
                    }
                    else
                    {
                        Contexto.ProductoTieneReceta.Add(new ProductoTieneReceta
                        {
                            Productos = producto,
                            tieneReceta = viewModel.TieneReceta,
                        });
                    }

                    Contexto.Entry(producto).State = EntityState.Modified;
                    Contexto.SaveChanges();
                    return viewModel.ForceRefresh 
                        ? RedirectToAction("Edit", "Menu", new { id = viewModel.IdProducto, familia = familiaId }) 
                        : RedirectToAction("Index", "Menu", new { CategoriaProductoId = familiaId });
                }
                else
                {
                    var catalogos = ComerciosFirmados
                       .Include(c => c.AreaPreparacion)
                       .Include(c => c.CategoriaProductos)
                       .ToArray();
                    var areaPreparacion = catalogos.SelectMany(c => c.AreaPreparacion).ToArray();
                    var categoriaProducto = catalogos.SelectMany(c => c.CategoriaProductos).ToArray();
                    ViewBag.AreaPreparacionId = new SelectList(areaPreparacion.OrderBy(c => c.descripcion), "idAreaPreparacion", "descripcion", viewModel.AreaPreparacionId);
                    ViewBag.CategoriaProductoId = new SelectList(categoriaProducto.OrderBy(c => c.descripcion), "idCategoriaProducto", "descripcion", viewModel.CategoriaProductoId);
                    return View(viewModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult ActivarProducto(int idProducto)
        {
            #region ActivarProducto
            try
            {
                // Activar producto general
                var producto = Contexto.Productos.Find(idProducto);
                producto.activo = true;
                Contexto.Entry(producto).State = EntityState.Modified;
                // Obtener las sucursales del comercio/Empresa
                var sucursales = ComercioSucursales
                       .Select(c => c.idSucursal)
                       .ToArray();
                // Obtener los productos por sucursal para activarlos
                var SucursalProducto = Contexto.SucursalProductos.Where(c => c.productoId == idProducto && sucursales.Contains(c.sucursalId)).ToList();
                foreach (var item in SucursalProducto)
                {
                    item.activo = true;
                    Contexto.Entry(item).State = EntityState.Modified;
                }
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
            #endregion
        }

        [HttpPost]
        public ActionResult InactivarProducto(int idProducto)
        {
            #region InactivarProducto
            try
            {
                // Inactivar producto general
                var producto = Contexto.Productos.Find(idProducto);
                producto.activo = false;
                Contexto.Entry(producto).State = EntityState.Modified;
                // Obtener las sucursales del comercio/Empresa
                var sucursales = ComercioSucursales
                       .Select(c => c.idSucursal)
                       .ToArray();
                // Obtener los productos por sucursal para inactivarlos
                var SucursalProducto = Contexto.SucursalProductos.Where(c => c.productoId == idProducto && sucursales.Contains(c.sucursalId)).ToList();
                foreach (var item in SucursalProducto)
                {
                    item.activo = false;
                    Contexto.Entry(item).State = EntityState.Modified;
                }
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return RedirectToAction("Index");
            #endregion
        }

        [HttpPost]
        public ActionResult ActivarProductoSucursal(int idSucursalProducto, int familiaRedirect)
        {
            #region ActivarProductoSucursal
            try
            {
                var SucursalProducto = Contexto.SucursalProductos.Find(idSucursalProducto);
                SucursalProducto.activo = true;
                Contexto.Entry(SucursalProducto).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Edit", "Menu", new { id = SucursalProducto.productoId, familia = familiaRedirect });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Index", "Menu");
            }
            #endregion
        }

        [HttpPost]
        public ActionResult InactivarProductoSucursal(int idSucursalProducto, int familiaRedirect)
        {
            #region InactivarProductoSucursal
            try
            {
                var SucursalProducto = Contexto.SucursalProductos.Find(idSucursalProducto);
                SucursalProducto.activo = false;
                Contexto.Entry(SucursalProducto).State = EntityState.Modified;
                Contexto.SaveChanges();
                return RedirectToAction("Edit", "Menu", new { id = SucursalProducto.productoId, familia = familiaRedirect });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Index", "Menu");
            }
            #endregion
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AgregarVariedad(int familiaid, int productoId, string ImagenAgregarVariedad, string Descripcion)
        {
            try
            {
                Contexto.VariedadProductos.Add(new VariedadProductos
                {
                    productoId = productoId,
                    descripcion = Descripcion,
                    imagen = ImagenAgregarVariedad
                });

                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
            }
            return RedirectToAction("Edit", new { id = productoId, familia = familiaid });
        }
        [HttpPost]
        public ActionResult EditarVariedad(string ImagenEdit, string DescripcionEditar, int IdVariedadProducto, int IdProducto, int CategoriaProductoId)
        {
            try
            {
                var variedad = Contexto.VariedadProductos.Find(IdVariedadProducto);
                variedad.imagen = ImagenEdit;
                variedad.descripcion = DescripcionEditar;
                Contexto.Entry(variedad).State = EntityState.Modified;

                Contexto.SaveChanges();
                return RedirectToAction("Edit", new { id = IdProducto, familia = CategoriaProductoId });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Edit", new { id = IdProducto, familia = CategoriaProductoId });
            }

        }
        [HttpPost]
        public ActionResult EliminarVariedad(int CategoriaProductoId, int IdProducto, int idVariedadEliminar)
        {
            try
            {
                var variedadProductos = Contexto.VariedadProductos.First(c => c.idVariedadProducto == idVariedadEliminar);
                Contexto.VariedadProductos.Remove(variedadProductos);
                Contexto.SaveChanges();
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            return RedirectToAction("Edit", new { id = IdProducto, familia = CategoriaProductoId });
        }
        [HttpPost]
        public ActionResult ConfigurarAgrupador(AgrupadorNuevo agrupador)
        {
            try
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    if (agrupador.Id.HasValue) // Editar
                    {
                        var agrupadorBd = Contexto.ConfiguracionArmadoProductos
                            .Include(c => c.AgrupadorInsumos)
                            .Include(c => c.AgrupadorInsumos.Productos)
                            .Include(c => c.AgrupadorInsumos.InsumoProductos)
                            .First(c => c.idConfiguracionArmadoProducto == agrupador.Id);

                        agrupadorBd.AgrupadorInsumos.agregarExtra = agrupador.PuedeAgregarExtra == 1;
                        agrupadorBd.AgrupadorInsumos.confirmarPorSeparado = agrupador.DebeConfirmarPorSeparado == 1;
                        agrupadorBd.AgrupadorInsumos.descripcion = agrupador.Descripcion;
                        agrupadorBd.AgrupadorInsumos.indice = agrupador.Indice;
                        agrupadorBd.AgrupadorInsumos.productoId = agrupador.IdProducto;
                        agrupadorBd.cantidad = agrupador.Cantidad;

                        if (agrupadorBd.AgrupadorInsumos.productoId.HasValue)
                        {
                            if (agrupadorBd.AgrupadorInsumos.agregarExtra
                                && agrupadorBd.AgrupadorInsumos.Productos.precio != agrupador.CostoExtra)
                            {
                                agrupadorBd.AgrupadorInsumos.Productos.precio = agrupador.CostoExtra.GetValueOrDefault(0);
                                var sProductos = Contexto.SucursalProductos
                                    .Where(c => c.productoId == agrupadorBd.AgrupadorInsumos.productoId)
                                    .ToArray();
                                if (sProductos?.Any() ?? false)
                                {
                                    foreach (var sucursalProductos in sProductos)
                                    {
                                        sucursalProductos.precio = agrupador.CostoExtra.GetValueOrDefault(0);
                                        Contexto.Entry(sucursalProductos).State = EntityState.Modified;
                                    }
                                }
                                Contexto.Entry(agrupadorBd.AgrupadorInsumos.Productos).State = EntityState.Modified;
                            }
                            else
                            {
                               // agrupadorBd.AgrupadorInsumos.productoId = null;
                            }
                        }
                        else if (agrupadorBd.AgrupadorInsumos.agregarExtra)
                        {
                            var producto = Contexto.Productos.Add(new Productos
                            {
                                comercioId = ComerciosFirmados.First().idComercio,
                                nombre = $"Extra de { agrupador.Descripcion }",
                                clave = $"Ex. { agrupador.Descripcion }",
                                precio = agrupador.CostoExtra.GetValueOrDefault(0),
                                activo = true,
                                mermaPermitida = 0,
                                esCombo = false,
                                esGeneral = true,
                                usuarioRegistroId = IdUsuarioActual,
                                fechaRegistro = DateTime.Now,
                                armarCobroMostrador = false,
                                indice = 0,
                                urlImagen = "data:image/png;base64,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",
                                principal = false
                            });
                            var sucursales = SucursalesActuales
                                .Select(c => c.idSucursal)
                                .ToArray();
                            var sucursalProductos = sucursales.Select(c => new SucursalProductos
                            {
                                activo = true,
                                fechaRegistro = producto.fechaRegistro,
                                precio = producto.precio,
                                Productos = producto,
                                sucursalId = c,
                                usuarioRegistroId = producto.usuarioRegistroId,
                            }).ToArray();
                            if (sucursalProductos is SucursalProductos[] array && array.Any())
                            {
                                Contexto.SucursalProductos.AddRange(sucursalProductos);
                            }
                            agrupadorBd.Productos = producto;
                        }

                        var opcionesAnteriores = agrupadorBd.AgrupadorInsumos.InsumoProductos
                            .Select(c => c.productoId ?? c.insumoId).ToArray();
                        var opcionesPorEliminar = opcionesAnteriores
                            .Where(c => c.HasValue && !agrupador.Opciones.Contains(c.Value)).ToArray();
                        var opcionesPorAgregar = agrupador.Opciones
                            .Where(c => !opcionesAnteriores.Contains(c)).ToArray();

                        if (opcionesPorEliminar?.Any() ?? false)
                        {
                            if (agrupador.Tipo == 2)
                            {
                                var toDelete = agrupadorBd.AgrupadorInsumos.InsumoProductos
                                    .Where(c => opcionesPorEliminar.Contains(c.productoId));
                                Contexto.InsumoProductos.RemoveRange(toDelete);
                            }
                            else
                            {
                                var toDelete = agrupadorBd.AgrupadorInsumos.InsumoProductos
                                    .Where(c => opcionesPorEliminar.Contains(c.insumoId));
                                Contexto.InsumoProductos.RemoveRange(toDelete);
                            }

                        }

                        if (opcionesPorAgregar?.Any() ?? false)
                        {
                            InsumoProductos[] insumosProductos;
                            if (agrupador.Tipo == 2)
                            {
                                insumosProductos = opcionesPorAgregar.Select(c => new InsumoProductos
                                {
                                    agrupadorInsumoId = agrupadorBd.agrupadorInsumoId,
                                    productoId = c,
                                }).ToArray();
                            }
                            else
                            {
                                insumosProductos = opcionesPorAgregar.Select(c => new InsumoProductos
                                {
                                    agrupadorInsumoId = agrupadorBd.agrupadorInsumoId,
                                    insumoId = c,
                                }).ToArray();
                            }

                            Contexto.InsumoProductos.AddRange(insumosProductos);
                        }

                        Contexto.Entry(agrupadorBd).State = EntityState.Modified;
                        Contexto.Entry(agrupadorBd.AgrupadorInsumos).State = EntityState.Modified;
                    }
                    else // Agregar
                    {

                        if (agrupador.PuedeAgregarExtra == 1 && !agrupador.CostoExtra.HasValue)
                            throw new ApplicationException("agrupador.PuedeAgregarExtra == 1 && !agrupador.CostoExtra.HasValue");

                        var agrupadorInsumos = new AgrupadorInsumos
                        {
                            agregarExtra = agrupador.PuedeAgregarExtra == 1,
                            confirmarPorSeparado = agrupador.DebeConfirmarPorSeparado == 1,
                            comercioId = ComerciosFirmados.First().idComercio,
                            descripcion = agrupador.Descripcion,
                            indice = agrupador.Indice,
                            productoId = agrupador.IdProducto,
                        };

                        var configuracion = new ConfiguracionArmadoProductos
                        {
                            cantidad = agrupador.Cantidad,
                            AgrupadorInsumos = agrupadorInsumos,
                            productoId = agrupador.IdProducto,
                        };
                        if (agrupadorInsumos.agregarExtra)
                        {
                            var producto = Contexto.Productos.Add(new Productos
                            {
                                comercioId = ComerciosFirmados.First().idComercio,
                                nombre = $"Extra de { agrupador.Descripcion }",
                                clave = $"Ex. { agrupador.Descripcion }",
                                precio = agrupador.CostoExtra.GetValueOrDefault(0),
                                activo = true,
                                mermaPermitida = 0,
                                esCombo = false,
                                esGeneral = true,
                                usuarioRegistroId = IdUsuarioActual,
                                fechaRegistro = DateTime.Now,
                                armarCobroMostrador = false,
                                indice = 0,
                                urlImagen = "data:image/png;base64,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",
                                principal = false
                            });
                            var sucursales = SucursalesActuales
                                .Select(c => c.idSucursal)
                                .ToArray();
                            var sucursalProductos = sucursales.Select(c => new SucursalProductos
                            {
                                activo = true,
                                fechaRegistro = producto.fechaRegistro,
                                precio = producto.precio,
                                Productos = producto,
                                sucursalId = c,
                                usuarioRegistroId = producto.usuarioRegistroId,
                            }).ToArray();
                            if (sucursalProductos is SucursalProductos[] array && array.Any())
                            {
                                Contexto.SucursalProductos.AddRange(sucursalProductos);
                            }
                            agrupadorInsumos.Productos = producto;
                        }

                        InsumoProductos[] insumosProductos;
                        if (agrupador.Tipo == 2)
                        {
                            insumosProductos = agrupador.Opciones.Select(c => new InsumoProductos
                            {
                                AgrupadorInsumos = agrupadorInsumos,
                                productoId = c,
                            }).ToArray();
                        }
                        else
                        {
                            insumosProductos = agrupador.Opciones.Select(c => new InsumoProductos
                            {
                                AgrupadorInsumos = agrupadorInsumos,
                                insumoId = c,
                            }).ToArray();
                        }

                        // validacion para que no se repita la seccion
                        //if(agrupador.Tipo == 3)
                        //{
                        //    var usuarioFirmado = Session.ObtenerUsuario();
                        //    int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                        //    var validar = Contexto.AgrupadorInsumos.Where(c => c.comercioId == comercioId && c.descripcion == agrupador.Descripcion && c.productoId == agrupador.IdProducto);

                        //    if(validar.Count() >= 1)
                        //    {
                        //        ShowAlertWarning("Ya existe esa seccion");
                        //        return RedirectToAction("Configurar", new { id = agrupador.IdProducto, type = agrupador.Tipo });
                        //    } 
                        //}
                     

                        Contexto.AgrupadorInsumos.Add(agrupadorInsumos);
                        Contexto.ConfiguracionArmadoProductos.Add(configuracion);
                        Contexto.InsumoProductos.AddRange(insumosProductos);
                    }

                    Contexto.SaveChanges();
                    tx.Commit();

                }
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            
            return RedirectToAction("Configurar", new { id = agrupador.IdProducto, type = agrupador.Tipo });
        }

        [HttpPost]
        public ActionResult EliminarAgrupador(int id, int IdProducto, int Tipo)
        {
            try
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    var porEliminar = Contexto.ConfiguracionArmadoProductos.First(c =>
                        c.productoId == IdProducto && c.idConfiguracionArmadoProducto == id);
                    Contexto.ConfiguracionArmadoProductos.Remove(porEliminar);
                    Contexto.SaveChanges();
                    tx.Commit();

                    //var porEliminarAgrupador = Contexto.AgrupadorInsumos.First(w => w.idAgrupadorInsumo == porEliminar.agrupadorInsumoId);
                    //Contexto.AgrupadorInsumos.Remove(porEliminarAgrupador);

                    //Contexto.SaveChanges();
                    //tx.Commit();
                }
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            return RedirectToAction("Configurar", new { id = IdProducto, type = Tipo });
        }
        [HttpPost]
        public ActionResult ConfigurarAgrupador2(AgrupadorNuevo agrupador)
        {
            try
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    if (agrupador.Id.HasValue) // Editar
                    {
                        var agrupadorBd = Contexto.ConfiguracionArmadoProductos
                            .Include(c => c.AgrupadorInsumos)
                            .Include(c => c.AgrupadorInsumos.Productos)
                            .Include(c => c.AgrupadorInsumos.InsumoProductos)
                            .First(c => c.idConfiguracionArmadoProducto == agrupador.Id);

                        agrupadorBd.AgrupadorInsumos.agregarExtra = agrupador.PuedeAgregarExtra == 1;
                        agrupadorBd.AgrupadorInsumos.confirmarPorSeparado = agrupador.DebeConfirmarPorSeparado == 1;
                        agrupadorBd.AgrupadorInsumos.descripcion = agrupador.Descripcion;
                        agrupadorBd.AgrupadorInsumos.indice = agrupador.Indice;
                        agrupadorBd.AgrupadorInsumos.productoId = agrupador.IdProducto;
                        agrupadorBd.cantidad = agrupador.Cantidad;

                        if (agrupadorBd.AgrupadorInsumos.productoId.HasValue)
                        {
                            if (agrupadorBd.AgrupadorInsumos.agregarExtra
                                && agrupadorBd.AgrupadorInsumos.Productos.precio != agrupador.CostoExtra)
                            {
                                agrupadorBd.AgrupadorInsumos.Productos.precio = agrupador.CostoExtra.GetValueOrDefault(0);
                                var sProductos = Contexto.SucursalProductos
                                    .Where(c => c.productoId == agrupadorBd.AgrupadorInsumos.productoId)
                                    .ToArray();
                                if (sProductos?.Any() ?? false)
                                {
                                    foreach (var sucursalProductos in sProductos)
                                    {
                                        sucursalProductos.precio = agrupador.CostoExtra.GetValueOrDefault(0);
                                        Contexto.Entry(sucursalProductos).State = EntityState.Modified;
                                    }
                                }
                                Contexto.Entry(agrupadorBd.AgrupadorInsumos.Productos).State = EntityState.Modified;
                            }
                            else
                            {
                                // agrupadorBd.AgrupadorInsumos.productoId = null;
                            }
                        }
                        else if (agrupadorBd.AgrupadorInsumos.agregarExtra)
                        {
                            var producto = Contexto.Productos.Add(new Productos
                            {
                                comercioId = ComerciosFirmados.First().idComercio,
                                nombre = $"Extra de { agrupador.Descripcion }",
                                clave = $"Ex. { agrupador.Descripcion }",
                                precio = agrupador.CostoExtra.GetValueOrDefault(0),
                                activo = true,
                                mermaPermitida = 0,
                                esCombo = false,
                                esGeneral = true,
                                usuarioRegistroId = IdUsuarioActual,
                                fechaRegistro = DateTime.Now,
                                armarCobroMostrador = false,
                                indice = 0,
                                urlImagen = "data:image/png;base64,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",
                                principal = false
                            });
                            var sucursales = SucursalesActuales
                                .Select(c => c.idSucursal)
                                .ToArray();
                            var sucursalProductos = sucursales.Select(c => new SucursalProductos
                            {
                                activo = true,
                                fechaRegistro = producto.fechaRegistro,
                                precio = producto.precio,
                                Productos = producto,
                                sucursalId = c,
                                usuarioRegistroId = producto.usuarioRegistroId,
                            }).ToArray();
                            if (sucursalProductos is SucursalProductos[] array && array.Any())
                            {
                                Contexto.SucursalProductos.AddRange(sucursalProductos);
                            }
                            agrupadorBd.Productos = producto;
                        }

                        var opcionesNuevas = agrupador.Opciones;
                        var opcionesAnteriores = agrupadorBd.AgrupadorInsumos.InsumoProductos.ToArray();

                        var opcionesPorEliminar = opcionesAnteriores
                            .Where(c => !opcionesNuevas.Contains(c.productoId ?? 0)).ToArray();
                        var opcionesPorAgregar = agrupador.Opciones
                            .Where(c => !opcionesAnteriores.Select(d => d.productoId).Contains(c)).ToArray();

                        if (opcionesPorEliminar?.Any() ?? false)
                        {
                            Contexto.InsumoProductos.RemoveRange(opcionesPorEliminar);
                        }

                        if (opcionesPorAgregar?.Any() ?? false)
                        {
                            
                            var insumosProductos = opcionesPorAgregar.Select(c => new InsumoProductos
                                {
                                    agrupadorInsumoId = agrupadorBd.agrupadorInsumoId,
                                    productoId = c,
                                }).ToArray();

                            Contexto.InsumoProductos.AddRange(insumosProductos);
                        }

                        Contexto.Entry(agrupadorBd).State = EntityState.Modified;
                        Contexto.Entry(agrupadorBd.AgrupadorInsumos).State = EntityState.Modified;
                    }
                    else // Agregar
                    {

                        if (agrupador.PuedeAgregarExtra == 1 && !agrupador.CostoExtra.HasValue)
                            throw new ApplicationException("agrupador.PuedeAgregarExtra == 1 && !agrupador.CostoExtra.HasValue");

                        var agrupadorInsumos = new AgrupadorInsumos
                        {
                            agregarExtra = agrupador.PuedeAgregarExtra == 1,
                            confirmarPorSeparado = agrupador.DebeConfirmarPorSeparado == 1,
                            comercioId = ComerciosFirmados.First().idComercio,
                            descripcion = agrupador.Descripcion,
                            indice = agrupador.Indice,
                            productoId = agrupador.IdProducto,
                        };

                        var configuracion = new ConfiguracionArmadoProductos
                        {
                            cantidad = agrupador.Cantidad,
                            AgrupadorInsumos = agrupadorInsumos,
                            productoId = agrupador.IdProducto,
                        };
                        if (agrupadorInsumos.agregarExtra)
                        {
                            var producto = Contexto.Productos.Add(new Productos
                            {
                                comercioId = ComerciosFirmados.First().idComercio,
                                nombre = $"Extra de { agrupador.Descripcion }",
                                clave = $"Ex. { agrupador.Descripcion }",
                                precio = agrupador.CostoExtra.GetValueOrDefault(0),
                                activo = true,
                                mermaPermitida = 0,
                                esCombo = false,
                                esGeneral = true,
                                usuarioRegistroId = IdUsuarioActual,
                                fechaRegistro = DateTime.Now,
                                armarCobroMostrador = false,
                                indice = 0,
                                urlImagen = "data:image/png;base64,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",
                                principal = false
                            });
                            var sucursales = SucursalesActuales
                                .Select(c => c.idSucursal)
                                .ToArray();
                            var sucursalProductos = sucursales.Select(c => new SucursalProductos
                            {
                                activo = true,
                                fechaRegistro = producto.fechaRegistro,
                                precio = producto.precio,
                                Productos = producto,
                                sucursalId = c,
                                usuarioRegistroId = producto.usuarioRegistroId,
                            }).ToArray();
                            if (sucursalProductos is SucursalProductos[] array && array.Any())
                            {
                                Contexto.SucursalProductos.AddRange(sucursalProductos);
                            }
                            agrupadorInsumos.Productos = producto;
                        }

                        InsumoProductos[] insumosProductos;
                        
                        insumosProductos = agrupador.Opciones.Select(c => new InsumoProductos
                        {
                            AgrupadorInsumos = agrupadorInsumos,
                            productoId = c,
                        }).ToArray();
                       
                        // validacion para que no se repita la seccion
                        //if(agrupador.Tipo == 3)
                        //{
                        //    var usuarioFirmado = Session.ObtenerUsuario();
                        //    int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                        //    var validar = Contexto.AgrupadorInsumos.Where(c => c.comercioId == comercioId && c.descripcion == agrupador.Descripcion && c.productoId == agrupador.IdProducto);

                        //    if(validar.Count() >= 1)
                        //    {
                        //        ShowAlertWarning("Ya existe esa seccion");
                        //        return RedirectToAction("Configurar", new { id = agrupador.IdProducto, type = agrupador.Tipo });
                        //    } 
                        //}


                        Contexto.AgrupadorInsumos.Add(agrupadorInsumos);
                        Contexto.ConfiguracionArmadoProductos.Add(configuracion);
                        Contexto.InsumoProductos.AddRange(insumosProductos);
                    }

                    Contexto.SaveChanges();
                    tx.Commit();

                }
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }

            return RedirectToAction("Edit", new { id = agrupador.IdProducto, familia = agrupador.familia });
        }

        [HttpPost]
        public ActionResult EliminarAgrupador2(int id, int IdProducto, int Tipo, int familia)
        {
            try
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    var porEliminar = Contexto.ConfiguracionArmadoProductos.First(c =>
                        c.productoId == IdProducto && c.idConfiguracionArmadoProducto == id);
                    Contexto.ConfiguracionArmadoProductos.Remove(porEliminar);
                    Contexto.SaveChanges();
                    tx.Commit();

                    //var porEliminarAgrupador = Contexto.AgrupadorInsumos.First(w => w.idAgrupadorInsumo == porEliminar.agrupadorInsumoId);
                    //Contexto.AgrupadorInsumos.Remove(porEliminarAgrupador);

                    //Contexto.SaveChanges();
                    //tx.Commit();
                }
            }
            catch (Exception e)
            {
                ShowAlertException(e);
            }
            return RedirectToAction("Edit", new { id = IdProducto, familia = familia });
        }
        #endregion

        #region AJAX
        public async Task<ActionResult> ObtenerAgrupador(int id)
        {
            try
            {
                var agrupador = Contexto.ConfiguracionArmadoProductos
                    .Include(c => c.AgrupadorInsumos)
                    .Include(c => c.AgrupadorInsumos.Productos)
                    .Include(c => c.AgrupadorInsumos.InsumoProductos)
                    .First(c => c.idConfiguracionArmadoProducto == id);
                return Json(new
                {
                    Id = id,
                    Descripcion = agrupador.AgrupadorInsumos.descripcion,
                    Cantidad = agrupador.cantidad,
                    Indice = agrupador.AgrupadorInsumos.indice,
                    Extras = agrupador.AgrupadorInsumos.agregarExtra ? 1 : 0,
                    CostoExtra = agrupador.AgrupadorInsumos?.Productos?.precio ?? 0,
                    PorSeparado = agrupador.AgrupadorInsumos.confirmarPorSeparado ? 1 : 0,
                    Opciones = agrupador.AgrupadorInsumos.InsumoProductos.Select(c => c.productoId ?? c.insumoId).ToArray()
                });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return HttpNotFound();
            }
        }

        public ActionResult ObtenerDatosSeccion(int id)
        {
            try
            {

                var secciones = Contexto.Insumos.Where(c => c.categoriaIngredienteId == id);
               
                List<Insumos> seccionesList = new List<Insumos>();

                foreach (var seccion in secciones)
                {
                    Insumos insumo = new Insumos();
                    insumo.nombre = seccion.nombre;
                    insumo.idInsumo = seccion.idInsumo;
                    seccionesList.Add(insumo);
                }

                return Json(new { exito = true, data = seccionesList, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return HttpNotFound();
            }
        }

        public ActionResult ObtenerDatosSeccion2(string nombre)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var secciones = Contexto.Insumos.Where(w => w.CategoriaIngrediente.descripcion == nombre && w.comercioId == comercioId);
                
                List<Insumos> seccionesList = new List<Insumos>();

                foreach (var seccion in secciones)
                {
                    Insumos insumo = new Insumos();
                    insumo.nombre = seccion.nombre;
                    insumo.idInsumo = seccion.idInsumo;
                    seccionesList.Add(insumo);
                }

                return Json(new { exito = true, data = seccionesList, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return HttpNotFound();
            }
        }

        public ActionResult BuscarUnidadMinima(int insumo)
        {
            try
            {
                var unidad = Contexto.Insumos.Include(c=>c.UnidadMedida).First(c => c.idInsumo == insumo);
                return Json(new { exito = true, data = unidad.unidadMedidaIdMinima, nombre = unidad.UnidadMedida?.descripcion, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new HttpStatusCodeResult(500);
            }
        }

        public async Task<ActionResult> UploadProductoImage()
        {
            if (Request.Files.Count == 0) return Json(new { success = false, message = "No se encontro la imagen" });
            if (Request.Files[0].ContentLength == 0) return Json(new { success = false, message = "No se encontro la imagen" });
            if (!FilesHelper.IsPng(Request.Files[0])) return Json(new { success = false, message = "La imagen debe tener formato PNG" });
            try
            {
                FilesUploadDelegate @delegate = new FilesUploadDelegate();
                var FileName = await @delegate.UploadFileAsync2(Request.Files[0], ServerPath, FilesHelper.FilesPath, IMAGE_EXTENSION);
                if (string.IsNullOrEmpty(FileName)) return Json(new { success = false, message = "Ocurrió un error al cargar la imagen" });
                return Json(new { success = true, fileUrl = FileName });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(new { success = false, message = "Ocurrió un error al cargar la imagen" });
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Contexto.Dispose();
            }
            base.Dispose(disposing);
        }

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
            var insumos = InsumosVisibles;

            insumos = insumos.Where(w => w.activo && !w.ConteoFisicoInsumos.Any(a => a.insumoId == w.idInsumo));
            insumos = insumos.OrderBy(o => o.nombre);

            return insumos.AsEnumerable().Select(c => {
                var procesado = c.esProcesado ? " - Es procesado" : "";
                return new SelectListItem
                {
                    Text = $"{c.nombre}{procesado}",
                    Value = c.idInsumo.ToString(),
                };
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
        #endregion

    }
}