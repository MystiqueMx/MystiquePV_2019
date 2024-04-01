using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.Emails;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{

    [Authorize]
    [ValidatePermissions]
    public class beneficiosController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: /beneficios/
        private readonly string HOSTNAME_IMAGENES = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");
        readonly string TITULO_NOTIFICACION_NUEVO_BENEFICIO = ConfigurationManager.AppSettings.Get("TITULO_NOTIFICACION_NUEVO_BENEFICIO");
        public async Task<ActionResult> Index(int? SearchSucursal, int? SearchComercio, int? searchItem, string SearchOrden = "true")

        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                if (!Request.IsAuthenticated || Usuario == null)
                    return new { success = false }.ToJsonResult();

                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
                string rol = Session.ObtenerRol();
                var usuarioFirmado = Session.ObtenerUsuario();


                var beneficios = Contexto.beneficios
                    .Include(b => b.confBeneficioSucursal)
                    .Include(b => b.beneficioHorarios)
                    .Include(b => b.catTipoMembresias)
                    .Include(b => b.comercios)
                    .Include(b => b.usuarios)
                    .Include(b => b.comercios.sucursales)
                    .Where(w => w.comercios.empresaId == usuarioFirmado.empresaId)
                    .AsQueryable();


                if (SearchComercio.HasValue)
                {
                    beneficios = beneficios.Where(c => c.comercioId == SearchComercio.Value);
                    var sucursal = from u in Contexto.sucursales.Where(c => c.comercioId == SearchComercio)
                                   select u;
                    //llenar grid

                    StringBuilder sc = new StringBuilder();

                    foreach (var type in sucursal)
                    {
                        sc.Append("<option value='" + type.idSucursal + "'>" + type.nombre + "</option>");
                    }

                    ViewBag.sucursal = sc.ToString();
                    ViewBag.Idcomercio = SearchComercio;
                    if (!string.IsNullOrEmpty(SearchOrden))
                    {
                        beneficios = beneficios.Where(c => c.estatus.ToString() == SearchOrden);
                    }
                    //if (SearchOrden == 1)
                    //{
                    //    beneficios = beneficios.OrderByDescending(stu => stu.fechaRegistro);
                    //}
                    //if (SearchOrden == 2)
                    //{
                    //    beneficios = beneficios.OrderBy(stu => stu.fechaRegistro);
                    //}
                    if (SearchSucursal.HasValue)
                    {
                        beneficios = beneficios.Where(stu => stu.confBeneficioSucursal.Any(c => c.sucursalId == SearchSucursal));
                    }

                }
                if (searchItem.HasValue)
                {
                    beneficios = beneficios.Where(stu => stu.idBeneficio == searchItem);

                    var comercio1 = beneficios.Where(c => c.idBeneficio == searchItem).Select(c => c.comercioId).FirstOrDefault();
                    var sucursal = from u in Contexto.sucursales.Where(c => c.comercioId == comercio1)
                                   select u;
                    //llenar grid

                    StringBuilder sc = new StringBuilder();

                    foreach (var type in sucursal)
                    {
                        sc.Append("<option value='" + type.idSucursal + "'>" + type.nombre + "</option>");
                    }

                    ViewBag.sucursal = sc.ToString();
                    ViewBag.Idcomercio = comercio1;


                    //if (SearchOrden == 2)
                    //{
                    //    beneficios = beneficios.OrderBy(stu => stu.fechaRegistro);
                    //}
                    if (SearchSucursal.HasValue)
                    {
                        beneficios = beneficios.Where(stu => stu.confBeneficioSucursal.Any(c => c.sucursalId == SearchSucursal));
                    }
                }
                if (!string.IsNullOrEmpty(SearchOrden))
                {
                    ViewBag.SearchOrden = SearchOrden;
                    beneficios = beneficios.Where(c => c.estatus.ToString() == SearchOrden);
                }
                //fin

                return PartialView(beneficios.ToList());
            }
            catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new { exception = e });
            }

        }

        // GET: /beneficios/Details/5
        public async Task<ActionResult> Details(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            beneficios beneficios = await Contexto.beneficios.FindAsync(id);

            if (beneficios == null)
            {
                return HttpNotFound();
            }
            return View(beneficios);
        }

        // GET: /beneficios/Create
        public ActionResult Create(int ss, int? IndexEmpresa, int? idEmpresa)
        {
            try
            {
                var comercio = Contexto.comercios.Find(ss);
                var usuarioFirmado = Session.ObtenerUsuario();
                var sucursal = from u in Contexto.sucursales.Where(c => c.comercios.empresaId == usuarioFirmado.empresaId)
                               select u;
                ViewBag.Idcomercio = ss;
                ViewBag.IndexEmpresa = IndexEmpresa;
                List<catTipoMembresias> menbresia = Contexto.catTipoMembresias.ToList();

                var categoriaProducto = ComerciosFirmados.Include(c => c.CategoriaProductos).SelectMany(c => c.CategoriaProductos).ToArray();
                ViewBag.CategoriaProductoId = new SelectList(categoriaProducto, "idCategoriaProducto", "descripcion");

                ViewData["sucursales"] = BuildComerciosSelectList(ss);
                ViewBag.menbresia = menbresia;
                ViewBag.idEmpresa = idEmpresa;
                ViewBag.idBeneficio = new SelectList(Contexto.beneficioHorarios, "beneficioHorarioId", "horarioInicio");
                ViewBag.tipoMembresiaId = new SelectList(Contexto.catTipoMembresias, "idcatTipoMembresia", "nombre");
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial");
                ViewBag.usuarioRegistroId = new SelectList(Contexto.usuarios, "idUsuario", "aspNetUsersId");
                ViewBag.tienePV = comercio.puntoVenta ?? false;

                Console.WriteLine(ViewBag.tienePV);
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new { SearchComercio = ss, exception = e });
            }
        }


        // POST: /beneficios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idBeneficio,comercioId,descripcion,descripcionIngles,fechaInicio,fechaFin,usuarioRegistroId,fechaRegistro,imagenDescuento,imagenExt,imagenTitulo,estatus,fondoColor,terminosCondiciones,titulo,isCupon,isPromocion,tipoCodigo,tipoMembresiaId,cadenaCodigo,nivelMembresia,urlImagenCodigo")] beneficios beneficios,
         int[] Sucursal, string horaInicio, string horaFin, int? IndexEmpresa, int? productoId, decimal montoDescuento = 0, int tipoDescuento = 0, int[] valor = null)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();

                var comercio = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                if (string.IsNullOrEmpty(horaInicio) || string.IsNullOrEmpty(horaFin))
                {
                    TempData["hora1"] = "Selecione el horario";
                }
                else
                {
                    if (valor == null)
                    {
                        TempData["notice"] = "Selecione una opcion";

                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            using (var tx = Contexto.Database.BeginTransaction())

                                try
                                {
                                    var guidBeneficio = Guid.NewGuid().ToString();
                                    int id = 0;
                                    beneficios.usuarioRegistroId = usuarioFirmado.idUsuario;
                                    beneficios.estatus = true;
                                    beneficios.fechaRegistro = DateTime.Now;
                                    beneficios.cadenaCodigo = "B" + guidBeneficio + "&";

                                    Contexto.beneficios.Add(beneficios);
                                    Contexto.SaveChanges();

                                    id = beneficios.idBeneficio;

                                    if (productoId != null)
                                    {

                                        Descuentos newDescuento = new Descuentos();

                                        newDescuento.activo = true;
                                        newDescuento.comercioId = beneficios.comercioId;
                                        newDescuento.nombre = beneficios.titulo;
                                        newDescuento.usuarioRegistroId = beneficios.usuarioRegistroId;
                                        newDescuento.fechaRegistro = beneficios.fechaRegistro;
                                        newDescuento.fechaInicio = beneficios.fechaInicio;
                                        newDescuento.fechaFin = beneficios.fechaFin;

                                        var producto = Contexto.Productos.FirstOrDefault(f => f.idProducto == productoId);
                                        newDescuento.montoporcentaje = (tipoDescuento == 1 ? montoDescuento : (producto.precio * (montoDescuento / 100)));

                                        DateTime hora;
                                        var ok = DateTime.TryParseExact(horaInicio, @"h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out hora);
                                        if (ok)
                                        {
                                            newDescuento.horaInicio = hora.TimeOfDay;
                                        }

                                        ok = DateTime.TryParseExact(horaFin, @"h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out hora);
                                        if (ok)
                                        {
                                            newDescuento.horaFin = hora.TimeOfDay;
                                        }

                                        newDescuento.lunes = valor.Any(a => a == 1);
                                        newDescuento.martes = valor.Any(a => a == 2);
                                        newDescuento.miercoles = valor.Any(a => a == 3);
                                        newDescuento.jueves = valor.Any(a => a == 4);
                                        newDescuento.viernes = valor.Any(a => a == 5);
                                        newDescuento.sabado = valor.Any(a => a == 6);
                                        newDescuento.domingo = valor.Any(a => a == 7);
                                        newDescuento.productoId = productoId;

                                        newDescuento.isPorcentaje = (tipoDescuento == 2);
                                        newDescuento.porcentaje = (tipoDescuento == 2 ? montoDescuento : (decimal?)null);

                                        newDescuento.beneficioId = beneficios.idBeneficio;
                                        Contexto.Descuentos.Add(newDescuento);

                                        foreach (var suc in Sucursal)
                                        {
                                            Contexto.SucursalDescuentos.Add(new SucursalDescuentos
                                            {
                                                descuentoId = newDescuento.idDescuento,
                                                sucursalId = Convert.ToInt32(suc),
                                                activo = true,
                                                usuarioRegistroId = newDescuento.usuarioRegistroId,
                                                fechaRegistro = newDescuento.fechaRegistro
                                            });
                                            Contexto.SaveChanges();

                                        }
                                    }
                                    foreach (int v in valor)
                                    {
                                        string a = "";
                                        if (v == 1)
                                        {
                                            a = "Lunes";
                                        }
                                        if (v == 2)
                                        {
                                            a = "Martes";
                                        }
                                        if (v == 3)
                                        {
                                            a = "Miercoles";
                                        }
                                        if (v == 4)
                                        {
                                            a = "jueves";
                                        }
                                        if (v == 5)
                                        {
                                            a = "Viernes";
                                        }
                                        if (v == 6)
                                        {
                                            a = "Sabado";
                                        }
                                        if (v == 7)
                                        {
                                            a = "Domingo";
                                        }

                                        Contexto.beneficioHorarios.Add(new beneficioHorarios
                                        {
                                            beneficioId = id,
                                            diasNum = Convert.ToInt32(v),
                                            horarioInicio = horaInicio,
                                            horarioFin = horaFin,
                                            descripcion = a
                                        });
                                        Contexto.SaveChanges();

                                      
                                        // EnviarNotificacionNuevoBeneficio(usuarioFirmado, beneficio);
                                    }

                                    foreach (var tec in Sucursal)
                                    {
                                        Contexto.confBeneficioSucursal.Add(new confBeneficioSucursal
                                        {
                                            beneficioId = id,
                                            sucursalId = Convert.ToInt32(tec)
                                        });
                                        Contexto.SaveChanges();

                                    }

                                    tx.Commit();

                                    if (IndexEmpresa == 1)
                                    {
                                        return RedirectToAction("Promociones", "comercios", new { idComercio = beneficios.comercioId, idEmpresa = usuarioFirmado.empresas.idEmpresa });
                                    }
                                    else
                                    {
                                        return RedirectToAction("Fidelizacion", "Home", new { tabID = "promociones_button", SearchComercio = beneficios.comercioId });
                                    }
                                }
                                catch (System.Data.Entity.Validation.DbEntityValidationException c)
                                {
                                    tx.Rollback();
                                }
                        }
                    }
                }

                List<catTipoMembresias> menbresia = Contexto.catTipoMembresias.ToList();
                ViewBag.menbresia = menbresia;
                ViewBag.Idcomercio = beneficios.comercioId;
                ViewData["sucursales"] = BuildComerciosSelectList(beneficios.comercioId);
                ViewBag.idBeneficio = new SelectList(Contexto.beneficioHorarios, "beneficioHorarioId", "horarioInicio", beneficios.idBeneficio);
                ViewBag.tipoMembresiaId = new SelectList(Contexto.catTipoMembresias, "idcatTipoMembresia", "nombre", beneficios.tipoMembresiaId);

                if (IndexEmpresa == 1)
                {
                    return RedirectToAction("Promociones", "comercios", new { idComercio = beneficios.comercioId, idEmpresa = usuarioFirmado.empresas.idEmpresa });
                }
                else
                {
                    return RedirectToAction("Fidelizacion", "Home", new { tabID = "promociones_button", SearchComercio = beneficios.comercioId });
                }
            }
            catch (Exception ex)
            {
                if (IndexEmpresa == 1)
                {
                    return RedirectToAction("Index", "Home", null);
                }
                else
                {
                    return RedirectToAction("Fidelizacion", "Home", new { tabID = "promociones_button", SearchComercio = beneficios.comercioId });
                }
            }
        }

        public static bool ValidateDocumentExtension(string filename)
        {
            string fileExt = System.IO.Path.GetExtension(filename);
            return fileExt.ToLower() == ".png";
        }

        // GET: /beneficios/Edit/5
        public async Task<ActionResult> Edit(int? id, int ss, int? IndexEmpresa)
        {
            try
            {
                ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var usuarioFirmado = Session.ObtenerUsuario();

                beneficios beneficios = await Contexto.beneficios.FindAsync(id);
                ViewBag.Idcomercio = ss;
                ViewBag.IndexEmpresa = IndexEmpresa;
                ViewBag.idEmpresa = usuarioFirmado.empresaId;

                if (beneficios == null)
                {
                    return HttpNotFound();
                }

                var comercio = Contexto.comercios.FirstOrDefault(c => c.empresaId == usuarioFirmado.empresas.idEmpresa);
                ViewBag.tienePV = comercio.puntoVenta ?? false;

                List<catTipoMembresias> menbresia = Contexto.catTipoMembresias.ToList();

                ViewData["sucursales"] = BuildComerciosSelectList(ss, beneficios.confBeneficioSucursal.Select(c => c.sucursalId));
                ViewBag.menbresia = menbresia;
                ViewBag.idBeneficio = new SelectList(Contexto.beneficioHorarios, "beneficioHorarioId", "horarioInicio", beneficios.idBeneficio);
                ViewBag.tipoMembresiaId = new SelectList(Contexto.catTipoMembresias, "idcatTipoMembresia", "nombre", beneficios.tipoMembresiaId);
                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", beneficios.comercioId);
                ViewBag.usuarioRegistroId = new SelectList(Contexto.usuarios, "idUsuario", "aspNetUsersId", beneficios.usuarioRegistroId);

                var categoriaProducto = ComerciosFirmados.Include(c => c.CategoriaProductos).SelectMany(c => c.CategoriaProductos).ToArray();

                Descuentos descuento = Contexto.Descuentos.FirstOrDefault(f => f.beneficioId == beneficios.idBeneficio);
                List<Productos> productos;

                if (descuento != null)
                {
                    productos = Contexto.Productos.Where(w => w.comercioId == ss && w.categoriaProductoId == descuento.Productos.categoriaProductoId).ToList();
                    ViewBag.productoList = new SelectList(productos, "idProducto", "nombre", (descuento.productoId ?? 0));
                    ViewBag.CategoriaProductoId = new SelectList(categoriaProducto, "idCategoriaProducto", "descripcion", (descuento.Productos.categoriaProductoId ?? 0));
                }
                else
                {
                    descuento = new Descuentos();
                    productos = new List<Productos>();
                    ViewBag.CategoriaProductoId = new SelectList(categoriaProducto, "idCategoriaProducto", "descripcion");
                    ViewBag.productoList = new SelectList(productos, "idProducto", "nombre");
                }

                ViewBag.descuento = descuento;

                return View(beneficios);
            }
            catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "beneficios", new { SearchComercio = ss, exception = e });
            }
        }
        private MultiSelectList BuildComerciosSelectList(int ss, IEnumerable<int> selected = null)
        {
            if (selected == null)
            {
                return new MultiSelectList(Contexto.sucursales.Where(c => c.comercioId == ss), "idSucursal", "nombre", null);
            }
            else
            {
                return new MultiSelectList(Contexto.sucursales.Where(c => c.comercioId == ss), "idSucursal", "nombre", null, selected);
            }
        }

        // POST: /beneficios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idBeneficio,comercioId,descripcion,descripcionIngles,fechaInicio,fechaFin,usuarioRegistroId,fechaRegistro,imagenDescuento,imagenExt,imagenTitulo,estatus,fondoColor,terminosCondiciones,titulo,isCupon,isPromocion,tipoCodigo,tipoMembresiaId,cadenaCodigo,nivelMembresia,urlImagenCodigo")] beneficios beneficios,
              int[] Sucursal, string horaInicio, string horaFin, int? IndexEmpresa, int? productoId, decimal montoDescuento = 0, int? tipoDescuento = 0, int[] valor = null)
        {
            try
            {
                var usuarioFirmado = Session.ObtenerUsuario();
                var comercio = Contexto.comercios.Where(c => c.empresaId == usuarioFirmado.empresas.idEmpresa).Select(c => c.idComercio).First();
                if (string.IsNullOrEmpty(horaInicio) || string.IsNullOrEmpty(horaFin))
                {
                    TempData["hora1"] = "Selecione el horario";
                }
                else
                {
                    if (valor == null)
                    {
                        TempData["notice"] = "Selecione una opcion";
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            using (var tx = Contexto.Database.BeginTransaction())
                                try
                                {

                                    beneficios.usuarioRegistroId = usuarioFirmado.idUsuario;
                                    beneficios.estatus = true;
                                    beneficios.tipoMembresiaId = beneficios.tipoMembresiaId;
                                    Contexto.Entry(beneficios).State = EntityState.Modified;
                                    confBeneficioSucursal benesucur = null;

                                    var surc = Contexto.confBeneficioSucursal.Where(c => c.beneficioId == beneficios.idBeneficio).Select(c => c.beneficioId).ToArray();

                                    if (productoId != null)
                                    {

                                        var descuento = Contexto.Descuentos.FirstOrDefault(f => f.beneficioId == beneficios.idBeneficio);
                                        if (descuento == null)
                                        {
                                            descuento = new Descuentos();
                                            descuento.activo = true;
                                            descuento.comercioId = beneficios.comercioId;
                                            descuento.usuarioRegistroId = usuarioFirmado.idUsuario;
                                            descuento.fechaRegistro = beneficios.fechaRegistro;
                                            descuento.beneficioId = beneficios.idBeneficio;
                                        }

                                        descuento.nombre = beneficios.titulo;
                                        descuento.fechaInicio = beneficios.fechaInicio;
                                        descuento.fechaFin = beneficios.fechaFin;

                                        var producto = Contexto.Productos.FirstOrDefault(f => f.idProducto == productoId);
                                        descuento.montoporcentaje = (tipoDescuento.Value == 1 ? montoDescuento : (producto.precio * (montoDescuento / 100)));
                                        descuento.productoId = productoId;
                                        descuento.isPorcentaje = (tipoDescuento == 2);
                                        descuento.porcentaje = (tipoDescuento == 2 ? montoDescuento : (decimal?)null);

                                        descuento.lunes = valor.Any(a => a == 1);
                                        descuento.martes = valor.Any(a => a == 2);
                                        descuento.miercoles = valor.Any(a => a == 3);
                                        descuento.jueves = valor.Any(a => a == 4);
                                        descuento.viernes = valor.Any(a => a == 5);
                                        descuento.sabado = valor.Any(a => a == 6);
                                        descuento.domingo = valor.Any(a => a == 7);


                                        DateTime hora;
                                        var ok = DateTime.TryParseExact(horaInicio, @"h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out hora);
                                        if (ok)
                                        {
                                            descuento.horaInicio = hora.TimeOfDay;
                                        }

                                        ok = DateTime.TryParseExact(horaFin, @"h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out hora);
                                        if (ok)
                                        {
                                            descuento.horaFin = hora.TimeOfDay;
                                        }

                                        if (descuento.idDescuento > 0)
                                        {
                                            Contexto.Entry(descuento).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            Contexto.Descuentos.Add(descuento);
                                        }

                                        Contexto.SaveChanges();

                                        var sucDescList = descuento.SucursalDescuentos.Where(w => Sucursal.Any(a => a != w.sucursalId)).ToList();
                                        foreach (var sucDesc in sucDescList)
                                        {
                                            if (!Sucursal.Any(a => a == sucDesc.sucursalId))
                                            {
                                                Contexto.SucursalDescuentos.Remove(descuento.SucursalDescuentos.FirstOrDefault(f => f.idSucursalDescuento == sucDesc.idSucursalDescuento));
                                            }
                                        }

                                        foreach (var suc in Sucursal)
                                        {
                                            if (!descuento.SucursalDescuentos.Any(a => a.sucursalId == suc))
                                            {
                                                Contexto.SucursalDescuentos.Add(new SucursalDescuentos
                                                {
                                                    descuentoId = descuento.idDescuento,
                                                    sucursalId = suc,
                                                    activo = true,
                                                    usuarioRegistroId = descuento.usuarioRegistroId,
                                                    fechaRegistro = DateTime.Now
                                                });
                                            }
                                        }
                                    }

                                    Contexto.SaveChanges();

                                    beneficioHorarios benefic = null;

                                    var benf = Contexto.beneficioHorarios.Where(c => c.beneficioId == beneficios.idBeneficio).Select(c => c.beneficioHorarioId).ToArray();
                                    foreach (int c in benf)
                                    {

                                        benefic = (from s in Contexto.beneficioHorarios
                                                   where s.beneficioId == beneficios.idBeneficio
                                                   select s).FirstOrDefault();
                                        Contexto.beneficioHorarios.Attach(benefic);
                                        Contexto.beneficioHorarios.Remove(benefic);
                                        Contexto.SaveChanges();


                                    }
                                    foreach (int v in valor)
                                    {
                                        string a = "";
                                        if (v == 1)
                                        {
                                            a = "Lunes";
                                        }
                                        if (v == 2)
                                        {
                                            a = "Martes";
                                        }
                                        if (v == 3)
                                        {
                                            a = "Miercoles";
                                        }
                                        if (v == 4)
                                        {
                                            a = "jueves";
                                        }
                                        if (v == 5)
                                        {
                                            a = "Viernes";
                                        }
                                        if (v == 6)
                                        {
                                            a = "Sabado";
                                        }
                                        if (v == 7)
                                        {
                                            a = "Domingo";
                                        }

                                        Contexto.beneficioHorarios.Add(new beneficioHorarios
                                        {
                                            beneficioId = beneficios.idBeneficio,
                                            diasNum = Convert.ToInt32(v),
                                            horarioInicio = horaInicio,
                                            horarioFin = horaFin,
                                            descripcion = a
                                        });

                                        Contexto.SaveChanges();
                                    }
                                    foreach (int c in surc)
                                    {

                                        benesucur = (from s in Contexto.confBeneficioSucursal
                                                     where s.beneficioId == beneficios.idBeneficio
                                                     select s).FirstOrDefault();
                                        Contexto.confBeneficioSucursal.Attach(benesucur);
                                        Contexto.confBeneficioSucursal.Remove(benesucur);
                                        Contexto.SaveChanges();


                                    }
                                    foreach (var tec in Sucursal)
                                    {

                                        Contexto.confBeneficioSucursal.Add(new confBeneficioSucursal
                                        {
                                            beneficioId = beneficios.idBeneficio,
                                            sucursalId = Convert.ToInt32(tec)

                                        });
                                        Contexto.SaveChanges();
                                    }
                                    tx.Commit();


                                    if (IndexEmpresa == 1)
                                    {
                                        return RedirectToAction("Promociones", "comercios", new { idComercio = beneficios.comercioId, idEmpresa = usuarioFirmado.empresas.idEmpresa });
                                    }
                                    else
                                    {
                                        return RedirectToAction("Fidelizacion", "Home", new { tabID = "promociones_button" });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    tx.Rollback();
                                }
                        }
                    }
                }
                beneficios beneficios1 = await Contexto.beneficios.FindAsync(beneficios.idBeneficio);
                ViewData["sucursales"] = BuildComerciosSelectList(beneficios.comercioId);
                List<catTipoMembresias> menbresia = Contexto.catTipoMembresias.ToList();
                ViewBag.menbresia = menbresia;
                ViewBag.Idcomercio = beneficios.comercioId;
                ViewBag.idBeneficio = new SelectList(Contexto.beneficioHorarios, "beneficioHorarioId", "horarioInicio", beneficios.idBeneficio);

                ViewBag.tipoMembresiaId = new SelectList(Contexto.catTipoMembresias, "idcatTipoMembresia", "nombre", beneficios.tipoMembresiaId);

                ViewBag.comercioId = new SelectList(Contexto.comercios, "idComercio", "nombreComercial", beneficios.comercioId);

                ViewBag.usuarioRegistroId = new SelectList(Contexto.usuarios, "idUsuario", "aspNetUsersId", beneficios.usuarioRegistroId);

                if (IndexEmpresa == 1)
                {
                    return RedirectToAction("Promociones", "comercios", new { idComercio = beneficios.comercioId, idEmpresa = usuarioFirmado.empresas.idEmpresa });
                }
                else
                {
                    return RedirectToAction("Fidelizacion", "Home", new { tabID = "promociones_button" });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ShowAlertException("");
                if (IndexEmpresa == 1)
                {
                    return RedirectToAction("Index", "Home", null);
                }
                else
                {
                    return RedirectToAction("Index", "beneficios", new { SearchComercio = beneficios.comercioId, exception = ex });
                }
            }
        }

        // GET: /beneficios/Delete/5
        //[ValidatePermissionsAttribute(true)]
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> Delete(int id, int? IdItemEmpresa)
        {
            var ben = 0;
            try
            {
                beneficios beneficios = await Contexto.beneficios.FindAsync(id);

                if (beneficios.estatus != false)
                {
                    beneficios.estatus = false;
                }
                else
                {
                    beneficios.estatus = true;
                }

                ben = beneficios.comercioId;
                Contexto.Entry(beneficios).State = EntityState.Modified;
                await Contexto.SaveChangesAsync();

                if (IdItemEmpresa == 1)
                {
                    return RedirectToAction("Promociones", "comercios", new { idComercio = beneficios.comercioId, idEmpresa = beneficios.comercios.empresaId });
                }
                else
                {
                    return RedirectToAction("Fidelizacion", "Home", new { tabID = "promociones_button", SearchComercio = beneficios.comercioId });
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "beneficios", new { SearchComercio = ben, exception = e });
            }
        }

        // GET: /beneficios/Delete/5
        //[ValidatePermissionsAttribute(true)]
        [HttpPost, ActionName("DeleteBenefit")]
        public async Task<ActionResult> DeleteBenefit(int id, int? IdItemEmpresa)
        {
            var ben = 0;
            try
            {
                bool delete = true;
                beneficios beneficios = await Contexto.beneficios.FindAsync(id);
                int comercioId = beneficios.comercioId;
                int empresaId = beneficios.comercios.empresaId;
                IEnumerable<beneficioAplicados> beneficioAplicados = Contexto.beneficioAplicados.Where(c => c.beneficioId == id);
                IEnumerable<beneficioCalificaciones> beneficioCalificaciones = Contexto.beneficioCalificaciones.Where(c => c.beneficioId == id);
                if (beneficioAplicados.Any() || beneficioCalificaciones.Any())
                {
                    ShowAlertWarning("La promoción no puede ser eliminada por que cuenta con información relacionada.");
                    delete = false;
                }

                if (delete)
                {
                    List<beneficioHorarios> beneficioHorarios = Contexto.beneficioHorarios.Where(c => c.beneficioId == id).ToList();
                    //Eliminar horarios del beneficio
                    beneficioHorarios.ForEach(item =>
                    {
                        Contexto.beneficioHorarios.Remove(item);
                    });
                    // Eliminar sucursal beneficio
                    List<confBeneficioSucursal> beneficioSucursales = Contexto.confBeneficioSucursal.Where(c => c.beneficioId == id).ToList();
                    beneficioSucursales.ForEach(item => 
                    {
                        Contexto.confBeneficioSucursal.Remove(item);
                    });
                    // Eliminar beneficio
                    Contexto.beneficios.Remove(beneficios);
                    // Guardar cambios
                    Contexto.SaveChanges();
                    ShowAlertSuccess("Promocion eliminada exitosamente.");
                }
                
                if (IdItemEmpresa == 1)
                {
                    return RedirectToAction("Promociones", "comercios", new { idComercio = comercioId, idEmpresa = empresaId });
                }
                else
                {
                    return RedirectToAction("Fidelizacion", "Home", new { tabID = "promociones_button", SearchComercio = comercioId });
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "beneficios", new { SearchComercio = ben, exception = e });
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
        private void EnviarNotificacionNuevoBeneficio(usuarios Usuario, beneficios beneficio)
        {
            var UsuariosMovil = Contexto.login
                .Where(c => c.clientes.empresaId == Usuario.empresaId)
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
                descripcion = string.Format("{0}", beneficio.descripcion),
                titulo = TITULO_NOTIFICACION_NUEVO_BENEFICIO,
                usuarioRegistro = Usuario.idUsuario,
                isBeneficio = true,
                beneficioId = beneficio.idBeneficio,
                empresaId = Usuario.empresaId,
                descripcionIngles = "-",
                tipoNotificacion = 1,
                notificacionAutomatica = false
            };

            Contexto.notificaciones.Add(notificacion);
            Contexto.SaveChanges();

            List<clienteNotificaciones> notificacionesListado = new List<clienteNotificaciones>();
            UsuariosMovil.ForEach(c =>
            {
                notificacionesListado.Add(new clienteNotificaciones
                {
                    notificaciones = notificacion,
                    clienteId = c.clienteId,
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Contexto.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
