using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MystiqueMC.Helpers;
using MystiqueMC.DAL;
using Microsoft.AspNet.Identity.Owin;
using MystiqueMC.Models;
using Microsoft.AspNet.Identity;
using MystiqueMC.Helpers.Permissions;

namespace MystiqueMC.Controllers
{
    [Authorize]
    [ValidatePermissions]
    public class usuariosController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public ActionResult Index()
        {
            List<VW_ObtenerUsuarios> usrs;
            if (Session.ObtenerRol() == RolesConfiguration.Superuser)
                usrs = Contexto.VW_ObtenerUsuarios.ToList();
            else
                usrs = Contexto.VW_ObtenerUsuarios.Where(c => c.empresaId == EmpresaUsuario && c.role == "Empresa" 
                || c.empresaId == EmpresaUsuario && c.role == "Comercio").ToList();

            return View(usrs);
        }

        public async Task<ActionResult> Details(int? id)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            var RoleID = usuarioFirmado.AspNetUsers.AspNetRoles.Select(s => s.Id).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            usuarios usuarios = await Contexto.usuarios.FindAsync(id);

            if (usuarios == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserName = usuarioFirmado.nombre + " " + usuarioFirmado.paterno + " " + usuarioFirmado.materno;
            var permisos = Contexto.AspNetRolePermissions.Where(w => w.RoleId == RoleID);

            return View(await permisos.ToListAsync());
        }

        public ActionResult Create()
        {
            ViewBag.AspNetRoles = new SelectList(RolesAsignables, "Name", "Name");
            //if (Session.ObtenerRol() == RolesConfiguration.Superuser)
            ViewBag.Empresas = new SelectList(EmpresasAsignables, "idEmpresa", "nombre");
            return View();
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            usuarios usuarios = await Contexto.usuarios.FindAsync(id);
            ViewBag.AspNetRoles = new SelectList(RolesAsignables, "Name", "Name");
            if (Session.ObtenerRol() == RolesConfiguration.Superuser)
                ViewBag.Empresas = new SelectList(Contexto.empresas, "idEmpresa", "nombre", usuarios.empresaId);

            if (usuarios == null)
            {
                return HttpNotFound();
            }

            ViewBag.aspNetUsersId = new SelectList(Contexto.AspNetUsers, "Id", "Email", usuarios.aspNetUsersId);
            ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "guidEmpresa", usuarios.empresaId);

            var rolesAnterior = userManager.GetRoles(usuarios.aspNetUsersId);

            ViewBag.RolSelected = rolesAnterior.FirstOrDefault();
            return View(usuarios);
        } 

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            usuarios usuarios = await Contexto.usuarios.FindAsync(id);

            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        public ActionResult ConfigurarUsuario(int? id)
        {
            try
            {
                var user = Contexto.usuarios.Find(id);
                var comerciosAsignados = Contexto.confUsuarioComercio
                    .Where(c => c.usuarioId == id)
                    .Select(c => c.comercios.idComercio)
                    .ToList();
                ViewBag.Comercios = BuildComerciosSelectList(user.empresaId, comerciosAsignados);
                return View(user);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException("Ocurrió un error al buscar al usuario");
                return RedirectToAction("Index", "usuarios", new
                {
                    exception = ex
                });
            }           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idUsuario,empresaId,aspNetUsersId,nombre,paterno,materno,email,telefono,fechaNacimiento,userName,password,usuarioRegistroId,FechaRegistro")] usuarios usuarios,
            string AspRol, int Empresa = 0)
        {
            try
            {

                var usuarioFirmado = Session.ObtenerUsuario();
                ViewBag.AspNetRoles = new SelectList(Contexto.AspNetRoles, "Name", "Name");
                string aspNetRolId;

                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                bool existe = userManager.FindByEmail(usuarios.email) != null;

                if (existe)
                {
                    ViewBag.AspNetRoles = new SelectList(Contexto.AspNetRoles, "Name", "Name");
                    ShowAlertException("El usuario ya se encuentra en uso");
                    return View();
                }
                else
                {
                    var user = new ApplicationUser { UserName = usuarios.email, Email = usuarios.email };
                    IdentityResult result = userManager.Create(user, usuarios.password);
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(user.Id, AspRol);

                       if(usuarios.telefono != null) usuarios.telefono = usuarios.telefono.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        usuarios.aspNetUsersId = user.Id;
                        usuarios.userName = usuarios.email;
                        usuarios.FechaRegistro = DateTime.Now;
                        usuarios.estatus = true;
                       
                        if(Empresa == 0)
                        {
                            usuarios.empresaId = usuarioFirmado.empresaId;
                        }
                        else
                        {
                            usuarios.empresaId = Empresa;
                        }

                        Contexto.usuarios.Add(usuarios);
                        Contexto.SaveChanges();

                        if (RolesConfiguration.RolesConfigurables.Contains(AspRol))
                        {
                            return RedirectToAction("ConfigurarUsuario", new { id = usuarios.idUsuario });
                        }
                    }
                    else
                    {
                        userManager.Delete(user);
                    }
                }

                return RedirectToAction("Index");             
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException("Ocurrió un error al registrar el usuario");
                return RedirectToAction("Index", "usuarios", new
                {
                    exception = ex
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Permisos(AspNetRolePermissions permisos,
            int[] Comercios, int[] Sucursales, int[] Productos, int[] Beneficios, int[] Home)
        {
            var usuarioFirmado = Session.ObtenerUsuario();
            var RoleID = usuarioFirmado.AspNetUsers.AspNetRoles.Select(s => s.Id).FirstOrDefault();
            var ComerciosReset = Contexto.AspNetRolePermissions.Where(w => w.ControllerId == 1).ToList();
            var SucursalesReset = Contexto.AspNetRolePermissions.Where(w => w.ControllerId == 2).ToList();
            var ProductosReset = Contexto.AspNetRolePermissions.Where(w => w.ControllerId == 3).ToList();
            var BeneficiosReset = Contexto.AspNetRolePermissions.Where(w => w.ControllerId == 4).ToList();
            var HomeReset = Contexto.AspNetRolePermissions.Where(w => w.ControllerId == 5).ToList();

            var ResetData = Contexto.AspNetRolePermissions.Where(w => w.RoleId == RoleID).ToList();

            foreach (var x in ResetData)
            {
                Contexto.AspNetRolePermissions.Remove(x);
                Contexto.SaveChanges();
            }

            using (var tx = Contexto.Database.BeginTransaction())
            {
                try
                {
                    if (Comercios != null)
                    {
                        foreach (var n in Comercios)
                        {
                            Contexto.AspNetRolePermissions.Add(new AspNetRolePermissions
                            {
                                RoleId = RoleID,
                                ControllerId = 1,
                                ControllerActivityId = n
                            });
                        }
                    }
                    if (Sucursales != null)
                    {
                        foreach (var n in Sucursales)
                        {
                            Contexto.AspNetRolePermissions.Add(new AspNetRolePermissions
                            {
                                RoleId = RoleID,
                                ControllerId = 2,
                                ControllerActivityId = n
                            });
                        }
                    }
                    if (Productos != null)
                    {
                        foreach (var n in Productos)
                        {
                            Contexto.AspNetRolePermissions.Add(new AspNetRolePermissions
                            {
                                RoleId = RoleID,
                                ControllerId = 3,
                                ControllerActivityId = n
                            });
                        }
                    }
                    if (Beneficios != null)
                    {
                        foreach (var n in Beneficios)
                        {
                            Contexto.AspNetRolePermissions.Add(new AspNetRolePermissions
                            {
                                RoleId = RoleID,
                                ControllerId = 4,
                                ControllerActivityId = n
                            });
                        }
                    }
                    if (Home != null)
                    {
                        foreach (var n in Home)
                        {
                            Contexto.AspNetRolePermissions.Add(new AspNetRolePermissions
                            {
                                RoleId = RoleID,
                                ControllerId = 5,
                                ControllerActivityId = n
                            });
                        }
                    }
                    Contexto.SaveChanges();
                    tx.Commit();
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    tx.Rollback();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idUsuario,empresaId,aspNetUsersId,nombre,paterno,materno,email,telefono,fechaNacimiento,userName,password,usuarioRegistroId,FechaRegistro")] usuarios usuarios
            ,string AspRol , int Empresa = 0)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var usuarioAnterior = Contexto.usuarios.Find(usuarios.idUsuario);
                    var user = userManager.FindByEmail(usuarios.email);
                    var cambioPassword = userManager.ChangePassword(user.Id, usuarioAnterior.password, usuarios.password);

                    if (cambioPassword.Succeeded)
                    {
                        usuarioAnterior.nombre = usuarios.nombre;
                        usuarioAnterior.paterno = usuarios.paterno;
                        usuarioAnterior.materno = usuarios.materno;
                        usuarioAnterior.fechaNacimiento = usuarios.fechaNacimiento;
                        usuarioAnterior.telefono = usuarios.telefono;
                        usuarioAnterior.estatus = usuarios.estatus;
                        usuarioAnterior.password = usuarios.password;
                        usuarioAnterior.estatus = true;

                        /*
                        if(Empresa != 0)
                        {
                            usuarioAnterior.empresaId = 0;
                        }
                        */
                        usuarioAnterior.empresaId = Empresa;

                        Contexto.Entry(usuarioAnterior).State = EntityState.Modified;
                        await Contexto.SaveChangesAsync();

                        var rolesAnterior = userManager.GetRoles(usuarioAnterior.aspNetUsersId);
                        if (!rolesAnterior.Contains(AspRol))
                        {
                            userManager.RemoveFromRoles(usuarioAnterior.aspNetUsersId, rolesAnterior.ToArray());
                            userManager.AddToRole(usuarioAnterior.aspNetUsersId, AspRol);
                        }
                    }
                    else
                    {
                        ViewBag.AspNetRoles = new SelectList(Contexto.AspNetRoles, "Name", "Name");
                        ShowAlertException("Ocurrió un error al actualizar el usuario");
                        return View();
                    }
                
                    return RedirectToAction("Index");
                }

                ViewBag.aspNetUsersId = new SelectList(Contexto.AspNetUsers, "Id", "Email", usuarios.aspNetUsersId);
                ViewBag.empresaId = new SelectList(Contexto.empresas, "idEmpresa", "guidEmpresa", usuarios.empresaId);

                return View(usuarios);
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException("Ocurrió un error al actualizar el usuario");
                return RedirectToAction("Index", "usuarios", new
                {
                    exception = ex
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfigurarUsuario(int UsuarioId, int[] Comercios, bool VerSoporte = false, bool VerReportes = false, bool VerComentarios = false, bool VerNotificaciones = false, 
            bool VerArqueo = false, bool VerFacturas = false, bool VerUsuarios = false, bool VerCatalogos = false, bool VerComercioAfiliado = false)
        {
            try
            {
                if (Comercios.Length > 0)
                {
                    List<confUsuarioComercio> comerciosAsignados = new List<confUsuarioComercio>();
                    var Usuario = Contexto.usuarios.Find(UsuarioId);
                    Contexto.confUsuarioComercio.RemoveRange(Contexto.confUsuarioComercio.Where(c => c.usuarioId == UsuarioId));
                    foreach (int comercio in Comercios)
                    {
                        comerciosAsignados.Add(new confUsuarioComercio
                        {
                            comercioId = comercio,
                            permiso = true,
                            usuarioId = UsuarioId,
                            verReportes = VerReportes,
                            verSoporte = VerSoporte,
                            verComentarios = VerComentarios,
                            verNotificaciones = VerNotificaciones,
                            verArqueo = VerArqueo,
                            verCatalogos = VerCatalogos,
                            verComercioAfiliado= VerComercioAfiliado,
                            verFacturas = VerFacturas,
                            verUsuarios = VerUsuarios
                        });
                    }
                    Contexto.confUsuarioComercio.AddRange(comerciosAsignados);
                    Contexto.SaveChanges();

                    var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    userManager.UpdateSecurityStamp(Usuario.aspNetUsersId);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ShowAlertException("Ocurrió un error al configurar al usuario");
                return RedirectToAction("Index", "usuarios", new
                {
                    exception = ex
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var UsuarioSeleccionado = await Contexto.usuarios.FindAsync(id);

            if (UsuarioSeleccionado.estatus)
            {
                UsuarioSeleccionado.estatus = false;
            }
            else
            {
                UsuarioSeleccionado.estatus = true;
            }

            await Contexto.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> EstatusBloqueosAsync(int id)
        {
            var UsuarioSeleccionado = await Contexto.usuarios.FindAsync(id);

            if (UsuarioSeleccionado.estatus)
            {

                UsuarioSeleccionado.estatus = false;
            }
            else
            {
                UsuarioSeleccionado.estatus = true;
            }

            Contexto.Entry(UsuarioSeleccionado).State = EntityState.Modified;
            await Contexto.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Contexto.Dispose();
            }
            base.Dispose(disposing);
        }

        #region AJAX
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ValidarUsername(string userName)
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByName(userName);
            if (user != null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            return new HttpNotFoundResult();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ValidarEmail(string email)
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByEmail(email);
            var user2 = userManager.FindByName(email);
            if (user != null || user2 != null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            return new HttpNotFoundResult();
        }
        #endregion

        #region HELPERS

        private MultiSelectList BuildComerciosSelectList(int EmpresaId, IList<int> selected = null)
        {
            if (selected == null)
            {
                return new MultiSelectList(Contexto.comercios.Where(c=>c.empresaId == EmpresaId), "idComercio", "nombreComercial", null);
            }
            else
            {
                return new MultiSelectList(Contexto.comercios.Where(c => c.empresaId == EmpresaId), "idComercio", "nombreComercial", null, selected);
            }
        }

        #endregion
    }
}
