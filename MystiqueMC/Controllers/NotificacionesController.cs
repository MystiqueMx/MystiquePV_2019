using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.Emails;
using MystiqueMC.Models;
using MystiqueMC.Models.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    public class NotificacionesController : BaseController
    {
        #region GET
        public ActionResult Index()
        {
            try
            {
                // TODO Implementar envio de sms
                //Helpers.SMS.SendSmsDelegate @delegate = new Helpers.SMS.SendSmsDelegate();
                //var msg = @delegate.SendSms(new Helpers.SMS.Modelos.Sms { Number = "6861162815", Body = "Mensaje C#" });
                var Usuario = Session.ObtenerUsuario();
                if (!Request.IsAuthenticated || Usuario == null)
                    return new { success = false }.ToJsonResult();

                return PartialView(new NotificacionesViewModel
                {
                    RangosEdades = ObtenerRangosEdades(),
                    Sexos = ObtenerSexos(),
                    Sucursales = ObtenerSucursales(),
                    NotificacionesAnteriores = ObtenerNotificacionesAnteriores(),
                    NotificacionesActuales = ObtenerNotificacionesActuales()
                });            
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error en la aplicación");
                return RedirectToAction("Index", "Home", null);
            }
        }
        #endregion
        #region POST
        public ActionResult NotificacionNueva(string Titulo, string Contenido, string[] sector, int[] sucursales, int? IdNotificacion)
        {
            try
            {
                var clientes = ObtenerClientesFiltradosPorSector(sector, sucursales).ToList();
                if (clientes.Count == 0)
                {
                    ShowAlertWarning("Los filtros seleccionados contienen un total de 0 clientes");
                    ViewBag.Titulo = Titulo;
                    ViewBag.Contenido = Contenido;
                    ViewBag.sector = sector;
                    ViewBag.sucursales = sucursales;
                    return PartialView("Index", new NotificacionesViewModel
                    {
                        RangosEdades = ObtenerRangosEdades(),
                        Sexos = ObtenerSexos(),
                        Sucursales = ObtenerSucursales(),
                    });
                }
                else
                {
                    var notificaciones = Contexto.notificaciones.Find(IdNotificacion);
                    if (notificaciones != null)
                    {
                        notificaciones.descripcion = Contenido;
                        notificaciones.titulo = Titulo;
                        Contexto.Entry(notificaciones).State = EntityState.Modified;

                        Contexto.SucursalNotificaciones.RemoveRange(Contexto.SucursalNotificaciones.Where(c => c.notificacionId == IdNotificacion));
                        var sucursal = sucursales.Select(s => new SucursalNotificaciones
                        {
                            notificaciones = notificaciones,
                            sucursalId = s
                        });
                        Contexto.SucursalNotificaciones.AddRange(sucursal);

                        Contexto.SectoresNotificaciones.RemoveRange(Contexto.SectoresNotificaciones.Where(c => c.notificacionId == IdNotificacion));
                        var sectores = sector.Select(s => new SectoresNotificaciones
                        {
                            notificaciones = notificaciones,
                            sectores = s
                        });
                        Contexto.SectoresNotificaciones.AddRange(sectores);
                    }
                    else
                    {
                        notificaciones n = new notificaciones
                        {
                            activo = true,
                            descripcion = Contenido,
                            titulo = Titulo,
                            isBeneficio = false,
                            empresaId = EmpresaUsuario,
                            usuarioRegistro = Session.IdUsuarioLogueado(),
                            fechaRegistro = DateTime.Now,
                            descripcionIngles = "",
                            notificacionAutomatica = false,
                            tipoNotificacion = 1
                        };
                        Contexto.notificaciones.Add(n);

                        var sucursal = sucursales.Select(s => new SucursalNotificaciones
                        {
                            notificaciones = n,
                            sucursalId = s
                        });

                        Contexto.SucursalNotificaciones.AddRange(sucursal);

                        var sectores = sector.Select(se => new SectoresNotificaciones
                        {
                            notificaciones = n,
                            sectores = se
                        });
                        Contexto.SectoresNotificaciones.AddRange(sectores);

                        List<clienteNotificaciones> listaClientes = new List<clienteNotificaciones>();

                        clientes.ForEach(c =>
                        {
                            listaClientes.Add(new clienteNotificaciones
                            {
                                clienteId = c.idCliente,
                                notificaciones = n,
                                fechaEnviado = DateTime.Now,
                                empresaId = n.empresaId,
                                revisado = false,
                            });
                        });
                        Contexto.clienteNotificaciones.AddRange(listaClientes);
                    }

                    Contexto.SaveChanges();
                    List<login> logins = new List<login>();
                    clientes.Select(c => c.login).ToList().ForEach(c => {
                        logins.AddRange(c);
                    });
                    EnviarNotificacionAClientes(Titulo, Contenido, logins);
                    ShowAlertSuccess("La notificacion ha sido enviada a un total de " + clientes.Count + " clientes");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error al enviar las notificaciones");
                ViewBag.Titulo = Titulo;
                ViewBag.Contenido = Contenido;
                ViewBag.sector = sector;
                ViewBag.sucursales = sucursales;
                return PartialView("Index", new NotificacionesViewModel
                {
                    RangosEdades = ObtenerRangosEdades(),
                    Sexos = ObtenerSexos(),
                    Sucursales = ObtenerSucursales(),
                });
            }
            
        }

        public ActionResult Edit(int idNotificacion)
        {
            try
            {
                var notificacion = Contexto.notificaciones
                    .Where(n => n.idNotificacion == idNotificacion)
                    .Select(n => new ItemNotificaciones
                    {
                        IdNotificacion = n.idNotificacion,
                        Titulo = n.titulo,
                        Descripcion = n.descripcion,
                        SucursalesId = n.SucursalNotificaciones.Where(c => c.notificacionId == idNotificacion).Select(c => c.sucursalId).ToList(),
                        Sectores = n.SectoresNotificaciones.Where(C => C.notificacionId == idNotificacion).Select(c => c.sectores).ToList(),
                    }).ToList();


               return Json(new Notificaciones { exito = true, resultado = notificacion });
            }
            catch (Exception e)
            {
                return Json(new Notificaciones { exito = false });
            }
        }

        #endregion
        #region HELPERS
        //public ActionResult ValidarClientesEnSector(string[] sector)
        //{

        //    return PartialView();
        //}
        private IEnumerable<catRangoEdad> ObtenerRangosEdades() =>
            Contexto.catRangoEdad.OrderBy(c => c.indice).ToList();
        private IEnumerable<catSexos> ObtenerSexos() =>
            Contexto.catSexos.OrderBy(c => c.idCatSexo).ToList();
        private IEnumerable<sucursales> ObtenerSucursales() =>
            SucursalesFirmadas.OrderBy(c=>c.nombre).ToList();
        private IQueryable<clientes> ObtenerClientesFiltradosPorSector(string[] sector, int[] sucursales)
        {
            if (sector.Any(c => c.Contains("az")))
            {
                return ClientesVisibles.Include(c=>c.login).Where(d => d.membresias.Any(e => e.cargaCompras.Any(f => sucursales.Contains(f.sucursalId))));
            }
            else
            {
                var results = ClientesVisibles;
                sector.Where(c => c.Contains("sx")).ToList().ForEach(c =>
                {
                    if (int.TryParse(c.Split('-')[1], out int SexoId))
                        results = results.Where(d => d.catSexoId == SexoId);
                    else
                        throw new ArgumentException(nameof(sector));
                });

                List<int> EdadesId = new List<int>();
                sector.Where(c => c.Contains("ed")).ToList().ForEach(c =>
                {
                    if (int.TryParse(c.Split('-')[1], out int EdadId))
                        EdadesId.Add(EdadId);
                    else
                        throw new ArgumentException(nameof(sector));
                });

                Contexto.catRangoEdad.Where(c => EdadesId.Contains(c.idCatRangoEdad)).ToList().ForEach(c=> {
                    var DateTimeInicial = DateTime.Now.AddYears(c.edadInferior * -1);
                    var DateTimeFinal = DateTime.Now.AddYears(c.edadSuperior * -1);
                    results = results.Where(d => d.fechaNacimiento > DateTimeInicial
                        && d.fechaNacimiento < DateTimeFinal);
                });

                results = results.Where(d => d.membresias.Any(e => e.cargaCompras.Any(f => sucursales.Contains(f.sucursalId))));

                return results.Include(c=>c.login);
            }
        }
        private void EnviarNotificacionAClientes(string titulo, string descripcion, ICollection<login> login)
        {
            var PlayerIds = login
                .Where(c => c.playerId != null 
                    && c.playerId.Length == 36)
                .Select(c => c.playerId)
                .Distinct()
                .ToArray();
            SendNotificationDelegate @delegate = new SendNotificationDelegate();
            @delegate.SendNotificationPorPlayerIds(PlayerIds, titulo, descripcion);
        }

        private IEnumerable<notificaciones> ObtenerNotificacionesAnteriores()
        {
            var clientes = ClientesVisibles.Select(c => c.idCliente).ToArray();
            var idUsuario = Session.ObtenerUsuario().idUsuario;
            return Contexto.clienteNotificaciones
                .Where(c => clientes.Contains(c.clienteId) && c.notificaciones.usuarioRegistro == idUsuario)
                .OrderBy(c=>c.fechaEnviado)
                .Select(c=>c.notificaciones)
                .Take(30)
                .Distinct()
                .ToList();
        }

        private IEnumerable<notificaciones> ObtenerNotificacionesActuales()
        {
            var clientes = ClientesVisibles.Select(c => c.idCliente).ToArray();
            var idUsuario = Session.ObtenerUsuario().idUsuario;
            return Contexto.notificaciones
                .Where(c => c.usuarioRegistro == idUsuario && c.notificacionAutomatica == false)
                .OrderBy(c => c.fechaRegistro)
                .Take(30)
                .Distinct()
                .ToList();
        }
        #endregion
    }
}