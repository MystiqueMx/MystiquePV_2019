using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.Emails;
using MystiqueMC.Helpers.Permissions;
using MystiqueMC.Models;

namespace MystiqueMC.Controllers
{
    public class AutentificacionController : BaseController
    {
        #region GET
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult ValidarEmail(string Email)
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByEmail(Email);
            if (user != null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            return new HttpNotFoundResult();
        }
        public ActionResult ForgotPassword() => View();
        [AllowAnonymous]
        public ActionResult ResetPassword(string code) => code == null ? View("Error") : View();
        #endregion
        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = Request.GetOwinContext().Get<ApplicationSignInManager>();

                var result = signInManager.PasswordSignIn(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var User = userManager.Find(model.Email, model.Password);
                        
                        var usuarioFirmado = Contexto.usuarios.Where(w => w.aspNetUsersId == User.Id).FirstOrDefault();
                        if (usuarioFirmado.estatus)
                        {
                            GuardarUsuarioEnSesion(usuarioFirmado, userManager);
                            //if (usuarioFirmado.empresaId)
                            //{

                            //}
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            ModelState.AddModelError("", "Intento de inicio de sesión inválido.");
                            return View(model);
                        }
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                    default:
                        ModelState.AddModelError("", "Intento de inicio de sesión inválido.");
                        return View(model);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToLocal(returnUrl);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View("ForgotPassword");

                var _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = _userManager.FindByEmail(model.Email);
                if (user == null)
                {
                    return View("ForgotPassword");
                }
                else
                {
                    string resetToken = _userManager.GeneratePasswordResetToken(user.Id);
                    var href = Url.Action("ResetPassword", "Autentificacion", new { email = model.Email, code = resetToken }, protocol: Request.Url.Scheme);
                    string subject = "Link para cambio de contraseña MystiqueMC ";
                    string body = "<p>Hemos recibido una petición para restablecer la contraseña de tu cuenta.</p>" +
                        model.Email +
                        "<p>Si hiciste esta petición, haz clic en el siguiente enlace, si no hiciste esta petición puedes ignorar este correo.</p>" +
                        "<b>Enlace para restablecer contraseña.</b><br/>"
                        + "<a href=\"" + href + "\">Restablecer contraseña</a>";

                    SendEmailDelegate @delegate = new SendEmailDelegate();
                    @delegate.SendEmail(model.Email, subject, body, "no-reply@mystique.com");

                    ShowAlertSuccess("Se ha enviado la información para recuperar contraseña a su correo electrónico");
                    return RedirectToAction("ForgotPassword");

                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return RedirectToAction("Index", "Home", e);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout(LoginViewModel model, string returnUrl)
        {
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByEmail(model.Email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            var result = userManager.ResetPassword(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                try
                {
                    usuarios users = Contexto.usuarios.Where(c => c.email == model.Email).FirstOrDefault();
                    users.password = model.Password;
                    Contexto.Entry(users).State = EntityState.Modified;
                    Contexto.SaveChanges();
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                }
                ShowAlertSuccess("Su contraseña ha sido actualizada");
                return RedirectToAction("Login");
            }
            ShowAlertDanger(result.Errors.First());
            return View();
        }
        #endregion
        #region HELPERS
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        private void GuardarUsuarioEnSesion(usuarios usuarioFirmado, ApplicationUserManager userManager)
        {
            var Rol = userManager
                .GetRoles(usuarioFirmado.aspNetUsersId)
                .FirstOrDefault();
            var PermisosDefault = Contexto.VW_Permisos
                .Where(c => c.nombreRol.Equals(Rol))
                .ToList();
            if (RolesConfiguration.RolesConfigurables.Contains(Rol))
            {
                var configuracionUsuario = usuarioFirmado.confUsuarioComercio.FirstOrDefault( c => c.usuarioId == usuarioFirmado.idUsuario && c.comercioId == c.comercioId);
                if(configuracionUsuario == null)
                {
                    PermisosDefault = PermisosDefault
                        .Except(PermisosDefault
                            .Where(c => c.controlador == "Comentarios"
                                || c.controlador == "Notificaciones"
                                || c.controlador == "Soporte"
                                || c.controlador == "comercios"
                                || c.controlador == "Arqueos"
                                || c.controlador == "Reportes"
                                || c.controlador == "FacturacionComercio"
                                || c.controlador == "usuarios"
                                || c.controlador == "Catalogos"
                        ).ToList())
                    .ToList();
                }
                else
                {
                    if (!configuracionUsuario.verComentarios)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "Comentarios").ToList()).ToList();
                    if (!configuracionUsuario.verNotificaciones)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "Notificaciones").ToList()).ToList();
                    if (!configuracionUsuario.verReportes)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "Reportes").ToList()).ToList();
                    if (!configuracionUsuario.verSoporte)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "Soporte").ToList()).ToList();
                    if (!configuracionUsuario.verComercioAfiliado)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "comercios").ToList()).ToList();
                    if (!configuracionUsuario.verArqueo)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "Arqueos").ToList()).ToList();
                    if (!configuracionUsuario.verFacturas)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "FacturacionComercio").ToList()).ToList();
                    if (!configuracionUsuario.verUsuarios)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "usuarios").ToList()).ToList();
                    if (!configuracionUsuario.verCatalogos)
                        PermisosDefault = PermisosDefault.Except(PermisosDefault.Where(c => c.controlador == "Catalogos").ToList()).ToList();
                }
            }
            Session.GuardarUsuario(usuarioFirmado);
            Session.GuardarRol(Rol);
            Session.GuardarPermisos(PermisosDefault);
        }
        #endregion
    }
}