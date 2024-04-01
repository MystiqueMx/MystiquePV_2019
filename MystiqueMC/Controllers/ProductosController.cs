using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.Emails;
using MystiqueMC.Helpers.FileUpload;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{

    [ValidatePermissions]
    public class ProductosController : BaseController
    {
        private const string IMAGE_EXTENSION = ".png";
        private const int TIPO_MEMBRESIA = 1;
        private const int TIPO_RECOMPENSA = 2;
        private string ServerPath => Server.MapPath(@"~");
        private readonly string HOSTNAME_IMAGENES = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");
        readonly string TITULO_NOTIFICACION_NUEVO_PRODUCTO = ConfigurationManager.AppSettings.Get("TITULO_NOTIFICACION_NUEVO_PRODUCTO");
        #region GET
        public ActionResult Index(int? item, int? IdComercio, int? SearchComercio, decimal? porcentaje, decimal? equivalentePuntoPorDinero, decimal? cantidadPuntos, string SearchEstatus = "true" )
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                if (!Request.IsAuthenticated || Usuario == null)
                    return new { success = false }.ToJsonResult();

                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;

                if (!IdComercio.HasValue)
                {
                    var comercios = ObtenerComercios();
                    ViewData["Comercios"] = comercios;

                    //llenar grid
                    StringBuilder sc = new StringBuilder();
                    foreach (var type in comercios)
                    {
                        sc.Append("<option value='" + type.idComercio + "'>" + type.nombreComercial + "</option>");
                    }
                    ViewBag.comercio = sc.ToString();
                    ViewBag.idComercio = IdComercio;


                    ViewBag.porcentaje = porcentaje;
                    ViewBag.equivalentePuntoPorDinero = equivalentePuntoPorDinero;
                    ViewBag.cantidadPuntos = cantidadPuntos;
                }
                else
                {
                    ViewBag.idComercio = IdComercio;

                    var configuracionPuntos = Contexto.confSumaPuntos.FirstOrDefault(f => f.comercioId == IdComercio);
                    if (configuracionPuntos != null)
                    {
                        ViewBag.porcentaje = configuracionPuntos.porcentajeDescuento;
                        ViewBag.equivalentePuntoPorDinero = configuracionPuntos.equivalentePuntoPorDinero;
                        ViewBag.cantidadPuntos = configuracionPuntos.cantidadPunto;
                    }
                }

                ViewBag.SearchOrden = SearchEstatus;
                return PartialView(GetRecompensasList(IdComercio, item, SearchComercio, SearchEstatus));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }


        public ActionResult Create()
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                ViewData["IdComercios"] = BuildComerciosSelectList();
                var categoriaProducto = ComerciosFirmados.Include(c => c.CategoriaProductos).SelectMany(c => c.CategoriaProductos).ToArray();
                ViewBag.CategoriaProductoId = new SelectList(categoriaProducto, "idCategoriaProducto", "descripcion");
                return View();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }
        public ActionResult Edit(int? id, int? IdComercio)
        {
            try
            {
                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;

                if (IdComercio.HasValue)
                {
                    var configuracionPuntos = Contexto.confSumaPuntos.FirstOrDefault(f => f.comercioId == IdComercio);
                    if (configuracionPuntos != null)
                    {
                        ViewBag.porcentaje = configuracionPuntos.porcentajeDescuento;
                        ViewBag.equivalentePuntoPorDinero = configuracionPuntos.equivalentePuntoPorDinero;
                        ViewBag.cantidadPuntos = configuracionPuntos.cantidadPunto;
                    }
                }

                if (!id.HasValue || id == 0) throw new ArgumentException(nameof(id));
                return View(Contexto.recompensas.Find(id));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }
        public ActionResult Puntos(int ComercioId)
        {
            try
            {
                ViewBag.NombreComercio = Contexto.comercios.Where(c => c.idComercio == ComercioId).Select(c => c.nombreComercial).First();
                var Usuario = Session.ObtenerUsuario();
                return View(ObtenerConfiguracionesSumaPuntos(ComercioId).FirstOrDefault());
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }
        #endregion
        #region POST
        [HttpPost]
        public async Task<ActionResult> UploadProductoImage()
        {
            if (Request.Files.Count == 0) return Json(new { success = false, message = "No se encontro la imagen" });
            if(Request.Files[0].ContentLength == 0 ) return Json(new { success = false, message = "No se encontro la imagen" });
            if (!FilesHelper.IsPNG(Request.Files[0])) return Json(new { success = false, message = "La imagen debe tener formato PNG" });
            try
            {
                FilesUploadDelegate @delegate = new FilesUploadDelegate();
                var FileName = await @delegate.UploadFileAsync(Request.Files[0], ServerPath, FilesHelper.FilesPath, IMAGE_EXTENSION);
                if(string.IsNullOrEmpty(FileName)) return Json(new { success = false, message = "Ocurrió un error al cargar la imagen" });
                return Json(new { success = true, fileUrl = FileName });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(new { success = false, message = "Ocurrió un error al cargar la imagen" });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Create(string Nombre, string Descripcion, string ImageUrl, string VigenciaStart, string VigenciaEnd, int Precio, int Configuracion, int productoId, int CategoriaProductoId)
        {
            try
            {

                var Usuario = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == Usuario.empresas.idEmpresa).Select(c => c.idComercio).First();
                int[] Comercios = { comercioId };

               catProductos ProductoNotificaciones = null;
                var vigenciaStart = DateTime.ParseExact(VigenciaStart,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal);
                var vigenciaEnd = DateTime.ParseExact(VigenciaEnd,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal);
                //@Math.Round(((((recompensa.catProductos.Productos.precio) / (ViewBag.porcentaje / 100)) * ViewBag.cantidadPuntos) / ViewBag.equivalentePuntoPorDinero), MidpointRounding.ToEven) pts
                                

                var configuracionPuntos = Contexto.confSumaPuntos.FirstOrDefault(f => f.comercioId == comercioId);
                decimal totalPuntos = Precio;
                var productoCaja = Contexto.Productos.FirstOrDefault(f => f.idProducto == productoId);

                if (configuracionPuntos != null && productoCaja != null)
                {
                    totalPuntos = Math.Round(((( Convert.ToDecimal(productoCaja.precio) /(configuracionPuntos.porcentajeDescuento / 100)) * configuracionPuntos.cantidadPunto) / configuracionPuntos.equivalentePuntoPorDinero), MidpointRounding.ToEven);
                }

                foreach (var comercio in Comercios)
                {
                    var Producto = Contexto.catProductos.Add(new catProductos
                    {
                        comercioId = comercio,
                        nombre = Nombre,
                        imagen = ImageUrl,
                        descripcion = Descripcion,
                        productoId = productoId
                    });
                    if (ProductoNotificaciones == null)
                    {
                        ProductoNotificaciones = Producto;
                    }
                    Contexto.recompensas.Add(new recompensas
                    {
                        catProductos = Producto,
                        catTipoMembresiaId = TIPO_MEMBRESIA,
                        catTipoRecompensaId = TIPO_RECOMPENSA,
                        fechaInicio = vigenciaStart,
                        fechaFin = vigenciaEnd,
                        estatus = true,
                        valor = totalPuntos,
                        categoriaProductoId = CategoriaProductoId,
                        consumirCanje = @Math.Round(totalPuntos / configuracionPuntos.cantidadPunto * 100, MidpointRounding.ToEven)

                    });
                    Contexto.confProductoPVEquivalencia.Add(new confProductoPVEquivalencia
                    {
                        productoPuntoVentaId = Configuracion,
                        catProductos = Producto,
                    });
                }
                Contexto.SaveChanges();
                EnviarNotificacionNuevoProducto(Usuario, ProductoNotificaciones);
                return RedirectToAction("Fidelizacion", "Home", new { tabID = "canjes_button" });
                //return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Logger.Error(e);
                return Redirect(Request.Path);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int IdRecompensa, int IdProducto, string Nombre, string Descripcion, string ImageUrl, string VigenciaStart, string VigenciaEnd, int Precio, int Configuracion)
        {
            try
            {
                var vigenciaStart = DateTime.ParseExact(VigenciaStart,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal);
                var vigenciaEnd = DateTime.ParseExact(VigenciaEnd,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal);

                var Recompensa = Contexto.recompensas.Find(IdRecompensa);
                var Producto = Contexto.catProductos.Find(IdProducto);
                var Conf = Contexto.confProductoPVEquivalencia.Find(Producto.confProductoPVEquivalencia.FirstOrDefault()?.idConfProductoPVEquivalencia);


                var configuracionPuntos = Contexto.confSumaPuntos.FirstOrDefault(f => f.comercioId == Producto.comercioId);
                decimal totalPuntos = Precio;

                if (configuracionPuntos != null && Producto.Productos != null)
                {
                    totalPuntos = Math.Round((((Convert.ToDecimal(Producto.Productos.precio) / (configuracionPuntos.porcentajeDescuento / 100)) * configuracionPuntos.cantidadPunto) / configuracionPuntos.equivalentePuntoPorDinero), MidpointRounding.ToEven);
                }

                Recompensa.valor = totalPuntos;
                Recompensa.fechaInicio = vigenciaStart;
                Recompensa.fechaFin = vigenciaEnd;

                Producto.nombre = Nombre;
                Producto.descripcion = Descripcion;
                Producto.imagen = ImageUrl;

                

                if (Conf == null)
                {
                    Contexto.confProductoPVEquivalencia.Add(new confProductoPVEquivalencia
                    {
                        catProductoId = Producto.idCatProducto,
                        productoPuntoVentaId = Configuracion
                    });
                }
                else
                {
                    Conf.productoPuntoVentaId = Configuracion;
                    Contexto.Entry(Conf).State = EntityState.Modified;
                }

                Contexto.Entry(Recompensa).State = EntityState.Modified;
                Contexto.Entry(Producto).State = EntityState.Modified;
                Contexto.SaveChanges();

                return RedirectToAction("Fidelizacion", "Home", new { tabID = "canjes_button" });
                //return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Redirect(Request.Path);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Puntos(int IdConfSumaPunto, decimal Monto, int Dias, int Horas, int Cantidad, decimal porcentajeDescuento, int ComercioId, int CatMembresiaId)
        {
            try
            {
                if(IdConfSumaPunto == 0)
                {
                    Contexto.confSumaPuntos.Add(new confSumaPuntos
                    {
                        comercioId = ComercioId,
                        catTipoMembresiaId = CatMembresiaId,
                        cantidadPunto = Cantidad,
                        montoCompraMinima = Monto,
                        horaValides = Horas,
                        diasValides = Dias,
                        equivalentePuntoPorDinero = 100,
                        porcentajeDescuento = porcentajeDescuento
                    });

                }
                else
                {
                    var Conf = Contexto.confSumaPuntos.Find(IdConfSumaPunto);
                    Conf.cantidadPunto = Cantidad;
                    Conf.montoCompraMinima = Monto;
                    Conf.horaValides = Horas;
                    Conf.diasValides = Dias;
                    Conf.equivalentePuntoPorDinero = 100;
                    Conf.porcentajeDescuento = porcentajeDescuento;
                    Contexto.Entry(Conf).State = EntityState.Modified;
                }

                Contexto.SaveChanges();
                return RedirectToAction("Fidelizacion", "Home", new { tabID = "canjes_button" });
                //return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Redirect(Request.Path);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int IdRecompensa, decimal ViewBagPorcentaje, decimal ViewBagCantidadPuntos, decimal ViewBagEquivalentePuntoPorDinero)
        {
            try
            {
                var Recompensa = Contexto.recompensas.Find(IdRecompensa);
               if(Recompensa.estatus != false)
                {
                    Recompensa.estatus = false;
                }
                else
                {
                    Recompensa.estatus = true;
                }

                ViewBag.porcentaje = ViewBagPorcentaje;
                ViewBag.equivalentePuntoPorDinero = ViewBagEquivalentePuntoPorDinero;
                ViewBag.cantidadPuntos = ViewBagCantidadPuntos;

                Contexto.Entry(Recompensa).State = EntityState.Modified;
                Contexto.SaveChanges();

                return RedirectToAction(nameof(Index), new { porcentaje = ViewBagPorcentaje, equivalentePuntoPorDinero = ViewBagEquivalentePuntoPorDinero, cantidadPuntos = ViewBagCantidadPuntos });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Redirect(Request.Path);
            }
        }

        public ActionResult ObtenerPoductos(int id)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                var productosAux = Contexto.Productos.Where(c => c.comercioId == comercioId && c.categoriaProductoId == id).ToList();

                List<Productos> productosList = new List<Productos>();

                foreach (var productos in productosAux)
                {
                    Productos producto = new Productos();
                    producto.nombre = productos.nombre;
                    producto.idProducto = productos.idProducto;
                    productosList.Add(producto);
                }

                return Json(new { exito = true, data = productosList, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return HttpNotFound();
            }
        }
        #endregion
        #region Helpers
        private MultiSelectList BuildComerciosSelectList(IEnumerable<comercios> selected = null)
        {
            if (selected == null)
            {
                return new MultiSelectList(ComerciosFirmados, "idComercio", "nombreComercial", null);
            }
            else
            {
                return new MultiSelectList(ComerciosFirmados, "idComercio", "nombreComercial", null, selected);
            }
        }

        private List<comercios> ObtenerComercios()
        {
            return ComerciosFirmados.ToList();
        }
        private List<recompensas> GetRecompensasList(int? IdComercio, int? item, int? SearchComercio, string SearchEstatus)
        {
            var recompensas = RecompensasVisibles;

            if(SearchComercio.HasValue)
                recompensas = recompensas.Where(c => c.catProductos.comercioId == SearchComercio.Value);

            if (!string.IsNullOrEmpty(SearchEstatus))
                recompensas = recompensas.Where(c => c.estatus.ToString() == SearchEstatus);

            if (IdComercio.HasValue)
                recompensas = recompensas.Where(c => c.catProductos.comercioId == IdComercio.Value);

            if (item.HasValue)
                recompensas = recompensas.Where(c => c.idRecompesa == item.Value);

            return recompensas.ToList();
        }
        private List<confSumaPuntos> ObtenerConfiguracionesSumaPuntos(int ComercioId)
        {
            var Configuracion = Contexto.confSumaPuntos.Where(c => c.comercioId == ComercioId).ToList();
            if (Configuracion == null || Configuracion.Count == 0)
            {
                var Membresia = Contexto.catTipoMembresias.FirstOrDefault();
                Configuracion = new List<confSumaPuntos> { new confSumaPuntos { comercioId = ComercioId, catTipoMembresiaId = Membresia.idcatTipoMembresia, catTipoMembresias = Membresia } };
            }
            return Configuracion;
        }
        private void EnviarNotificacionNuevoProducto(usuarios Usuario, catProductos productoNotificaciones)
        {
            var UsuariosMovil = Contexto.login
                .Where(c => ClientesVisibles.Select(d=>d.idCliente).Contains(c.clienteId)) 
                .Select(c => new { c.playerId, c.clienteId })
                .ToList();
            if (UsuariosMovil.Count == 0) return;
            var PlayerIds = UsuariosMovil
                .Where(c => c.playerId != null && c.playerId.Length == 36) // limpia los player ids invalidos
                .Select(c => c.playerId)
                .Distinct()
                .ToArray();

            notificaciones notificacion = new notificaciones
            {
                activo = true,
                fechaRegistro = DateTime.Now,
                descripcion = string.Format("{0} - {1} puntos", productoNotificaciones.nombre, productoNotificaciones.recompensas.FirstOrDefault().valor),
                titulo = TITULO_NOTIFICACION_NUEVO_PRODUCTO,
                usuarioRegistro = Usuario.idUsuario,
                isBeneficio = false,
                empresaId = Usuario.empresaId,
                descripcionIngles = "-",
                tipoNotificacion = 1,
            };

            Contexto.notificaciones.Add(notificacion);

            List<clienteNotificaciones> notificacionesListado = new List<clienteNotificaciones>();
            UsuariosMovil.Select(c=>c.clienteId).Distinct().ToList().ForEach(id =>
            {
                notificacionesListado.Add(new clienteNotificaciones
                {
                    notificaciones = notificacion,
                    clienteId = id,
                    fechaEnviado = notificacion.fechaRegistro,
                    revisado = false,
                    empresaId = notificacion.empresaId,
                });
            });

            Contexto.clienteNotificaciones.AddRange(notificacionesListado);
            Contexto.SaveChanges();

            SendNotificationDelegate @delegate = new SendNotificationDelegate();
            @delegate.SendNotificationPorPlayerIds(PlayerIds, notificacion.titulo, notificacion.descripcion);
        }


        public ActionResult ObtenerPrecioProducto(int productoId)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                int comercioId = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();

                var producto = Contexto.Productos.FirstOrDefault(f => f.comercioId == comercioId && f.idProducto == productoId);
                var confSumaPunto = Contexto.confSumaPuntos.FirstOrDefault(f => f.comercioId == comercioId);

                var productoPrecio = new
                {
                    precio = producto.precio,
                    PuntosCanje = ((producto.precio / (confSumaPunto.porcentajeDescuento/100)) * confSumaPunto.cantidadPunto) / confSumaPunto.equivalentePuntoPorDinero,
                    GastoCanje = (producto.precio / (confSumaPunto.porcentajeDescuento / 100)),
                    productoId = productoId
                };

                return Json(new { exito = true, data = productoPrecio, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return HttpNotFound();
            }
        }
        #endregion
    }
}