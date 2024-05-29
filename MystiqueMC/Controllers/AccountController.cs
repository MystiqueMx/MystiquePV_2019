

//prueba git hub

//prueba git hub

//prueba git hub


using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Owin.Security;
using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
						   
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
							





namespace MystiqueMC.Controllers
{
    [Authorize]
    public class AccountController : BaseController	        					  
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private string DefaultRole => "Administrador";
        private const string XsrfKey = "XsrfId";
		
        public AccountController()
        {
        }
        public AccountController(
		ApplicationUserManager userManager, 
		ApplicationSignInManager signInManager )																																																																						   
        {
            UserManager = userManager;																									
            SignInManager = signInManager;								
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager											   	 																		   																								 													   																	   	   														 																																																																								   											   	 																		   																								  													   																	   	   														 																																																																								   
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]							  
        public ActionResult Login(string returnUrl)	 												 		 	   												  																	   																																	 												 		 	   												  																	   																																
        {
            if (Request.IsAuthenticated && Session.TieneSesionActiva())
                return RedirectToAction("Index", "Home", null);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }













        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
      AccountController accountController = this;
      try
      {
        if (!accountController.ModelState.IsValid)
          return (ActionResult) accountController.View((object) model);
        switch (await accountController.SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
        {
          case SignInStatus.Success:
            ApplicationUser User = accountController.UserManager.Find<ApplicationUser, string>(model.Email, model.Password);
            string Rol = accountController.UserManager.GetRoles<ApplicationUser, string>(User.Id).FirstOrDefault<string>();
            usuarios usuario = accountController.Contexto.usuarios.Where<usuarios>((Expression<Func<usuarios, bool>>) (w => w.aspNetUsersId == User.Id)).FirstOrDefault<usuarios>();
          if (usuario.estatus)		  							   				  														  
            {
              List<VW_Permisos> list = accountController.Contexto.VW_Permisos.Where<VW_Permisos>((Expression<Func<VW_Permisos, bool>>) (c => c.nombreRol.Equals(Rol))).ToList<VW_Permisos>();				 									   				 																																																																															  							   				 																 																				 																				 																													   											   					 																										 
              accountController.Session.GuardarUsuario(usuario);
              accountController.Session.GuardarRol(Rol);
              accountController.Session.GuardarPermisos(list);
              return accountController.RedirectToLocal(returnUrl);					 											 																														  																																		   					 																							   														   																														  											  																																									   				 
            }
            accountController.HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            accountController.ModelState.AddModelError("", "Intento de inicio de sesión inválido.");
            return (ActionResult) accountController.View((object) model);
          case SignInStatus.LockedOut:
            return (ActionResult) accountController.View("Lockout");
          case SignInStatus.RequiresVerification:
            return (ActionResult) accountController.RedirectToAction("SendCode", (object) new
            {
              ReturnUrl = returnUrl,
              RememberMe = model.RememberMe
            });
          default:
            accountController.ModelState.AddModelError("", "Intento de inicio de sesión inválido.");
            return (ActionResult) accountController.View((object) model);
        }
      }
              catch (Exception ex)
      {
        accountController.logger.Error((object) ex);
        accountController.ShowAlertException("");
        return accountController.RedirectToLocal(returnUrl);
      }
    }
	 
	 
	 
	 
	 
	 
	 
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
    public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
    {
      AccountController accountController = this;
      if (!await accountController.SignInManager.HasBeenVerifiedAsync())
			 
        return (ActionResult) accountController.View("Error");
      return (ActionResult) accountController.View((object) new VerifyCodeViewModel()
      {
        Provider = provider,
        ReturnUrl = returnUrl,
        RememberMe = rememberMe
      });
    }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
    {
      AccountController accountController = this;
      if (!accountController.ModelState.IsValid)
        return (ActionResult) accountController.View((object) model);
      switch (await accountController.SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser))
      {
        case SignInStatus.Success:
          return accountController.RedirectToLocal(model.ReturnUrl);
        case SignInStatus.LockedOut:
          return (ActionResult) accountController.View("Lockout");
        default:
          accountController.ModelState.AddModelError("", "Invalid code.");
          return (ActionResult) accountController.View((object) model);
      }
    }

    [AllowAnonymous]
    public ActionResult Register() => (ActionResult) this.View();

        //
        // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]     
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
      AccountController accountController = this;
      if (accountController.ModelState.IsValid)
      {
        ApplicationUser applicationUser = new ApplicationUser();
        applicationUser.UserName = model.Email;
        applicationUser.Email = model.Email;
        ApplicationUser user = applicationUser;
        IdentityResult result = await accountController.UserManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          await accountController.SignInManager.SignInAsync(user, false, false);
          return (ActionResult) accountController.RedirectToAction("Index", "Home");
        }
        accountController.AddErrors(result);
        user = (ApplicationUser) null;
        result = (IdentityResult) null;
      }
      return (ActionResult) accountController.View((object) model);
    }


        //
        // GET: /Account/ConfirmEmail
       [AllowAnonymous]
								  
    public async Task<ActionResult> ConfirmEmail(string userId, string code)
		 
								   
    {
      AccountController accountController = this;
      if (userId == null || code == null)
        return (ActionResult) accountController.View("Error");
      IdentityResult identityResult = await accountController.UserManager.ConfirmEmailAsync(userId, code);
      return (ActionResult) accountController.View(identityResult.Succeeded ? nameof (ConfirmEmail) : "Error");
    }
					
 
        //
        // GET: /Account/ForgotPassword
     [AllowAnonymous]
    public ActionResult ForgotPassword() => (ActionResult) this.View();
				

        // POST: /Account/ForgotPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      AccountController accountController = this;
      try
      {
                if (!accountController.ModelState.IsValid)
          return (ActionResult) accountController.View((object) model);
        ApplicationUserManager _userManager = accountController.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        ApplicationUser byNameAsync = await _userManager.FindByNameAsync(model.Email);
        string To = model.Email;
        if (byNameAsync == null)
          return (ActionResult) accountController.View(nameof (ForgotPassword));
        string passwordResetTokenAsync = await _userManager.GeneratePasswordResetTokenAsync(byNameAsync.Id);
        string str = accountController.Url.Action("ResetPassword", "Account", (object) new
        {
          email = model.Email,
          code = passwordResetTokenAsync
        }, accountController.Request.Url.Scheme);
        string Subject = "Link para cambio de contraseña MystiqueMC ";
        string Body = "<p>Hemos recibido una petición para restablecer la contraseña de tu cuenta.</p>" + model.Email + "<p>Si hiciste esta petición, haz clic en el siguiente enlace, si no hiciste esta petición puedes ignorar este correo.</p><b>Porfavor click en enlace para restablecer.</b><br/><a href=\"" + str + "\">Restablecer contraseña</a>";
        string UserID;
        string Password;
        string SMTPPort;
        string Host;
        AccountController.EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
        AccountController.EmailManager.SendEmail(UserID, Subject, Body, To, UserID, Password, SMTPPort, Host);
        return (ActionResult) accountController.RedirectToAction("ForgotPasswordConfirmation", "Account");
      }
      catch (Exception ex)
      {
        accountController.logger.Error((object) ex);
        return (ActionResult) accountController.RedirectToAction("Index", "Home", (object) ex);
      }
    }

		  
									   
    [AllowAnonymous]
    public ActionResult ForgotPasswordConfirmation() => (ActionResult) this.View();
		 
						  
		 

		  
										
				  
    [AllowAnonymous]
								  
    public ActionResult ResetPassword(string code)
    {
      return code != null ? (ActionResult) this.View() : (ActionResult) this.View("Error");
    }
									   
				 
																											 
																			   
																			  
									 
					 
																	  

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      AccountController accountController = this;
      if (!accountController.ModelState.IsValid)
        return (ActionResult) accountController.View((object) model);
      ApplicationUser byNameAsync = await accountController.UserManager.FindByNameAsync(model.Email);
      if (byNameAsync == null)
        return (ActionResult) accountController.RedirectToAction("ResetPasswordConfirmation", "Account");
      IdentityResult result = await accountController.UserManager.ResetPasswordAsync(byNameAsync.Id, model.Code, model.Password);
      if (result.Succeeded)
      {
        usuarios entity = await accountController.Contexto.usuarios.Where<usuarios>((Expression<Func<usuarios, bool>>) (c => c.email == model.Email)).FirstAsync<usuarios>();
        entity.password = model.Password;
        accountController.Contexto.Entry<usuarios>(entity).State = EntityState.Modified;
        int num = await accountController.Contexto.SaveChangesAsync();
        return (ActionResult) accountController.RedirectToAction("ResetPasswordConfirmation", "Account");
      }
      accountController.AddErrors(result);
      return (ActionResult) accountController.View();
    }

    [AllowAnonymous]
    public ActionResult ResetPasswordConfirmation() => (ActionResult) this.View();
					 
												 

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
      return (ActionResult) new AccountController.ChallengeResult(provider, this.Url.Action("ExternalLoginCallback", "Account", (object) new
      {
        ReturnUrl = returnUrl
      }));
    }

    [AllowAnonymous]
    public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
    {
      AccountController accountController = this;
      string verifiedUserIdAsync = await accountController.SignInManager.GetVerifiedUserIdAsync();
      if (verifiedUserIdAsync == null)
        return (ActionResult) accountController.View("Error");
      List<SelectListItem> list = (await accountController.UserManager.GetValidTwoFactorProvidersAsync(verifiedUserIdAsync)).Select<string, SelectListItem>((Func<string, SelectListItem>) (purpose => new SelectListItem()
      {
        Text = purpose,
        Value = purpose
      })).ToList<SelectListItem>();
      return (ActionResult) accountController.View((object) new SendCodeViewModel()
      {
        Providers = (ICollection<SelectListItem>) list,
        ReturnUrl = returnUrl,
        RememberMe = rememberMe
      });
    }
	
	[HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
         public async Task<ActionResult> SendCode(SendCodeViewModel model)
    {
      AccountController accountController = this;
      return !accountController.ModelState.IsValid ? (ActionResult) accountController.View() : (await accountController.SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider) ? (ActionResult) accountController.RedirectToAction("VerifyCode", (object) new
      {
        Provider = model.SelectedProvider,
        ReturnUrl = model.ReturnUrl,
        RememberMe = model.RememberMe
      }) : (ActionResult) accountController.View("Error"));
    }

    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    {
      AccountController accountController = this;
      ExternalLoginInfo loginInfo = await accountController.AuthenticationManager.GetExternalLoginInfoAsync();
      if (loginInfo == null)
        return (ActionResult) accountController.RedirectToAction("Login");
      switch (await accountController.SignInManager.ExternalSignInAsync(loginInfo, false))
      {
        case SignInStatus.Success:
          return accountController.RedirectToLocal(returnUrl);
        case SignInStatus.LockedOut:
          return (ActionResult) accountController.View("Lockout");
        case SignInStatus.RequiresVerification:
          return (ActionResult) accountController.RedirectToAction("SendCode", (object) new																							  
          {
            ReturnUrl = returnUrl,
            RememberMe = false
          });
		  
		  
		  
		 
        default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }


























        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(
		ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");	
            }

            if (ModelState.IsValid)																																																																							   
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };																						   
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
				













        //
        // POST: /Account/LogOff
         [HttpPost]
						
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
      this.AuthenticationManager.SignOut("ApplicationCookie");
												
      return (ActionResult) this.RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public ActionResult ExternalLoginFailure() => (ActionResult) this.View();
							  
						  
		 

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this._userManager != null)
        {
          this._userManager.Dispose();
          this._userManager = (ApplicationUserManager) null;
        }
        if (this._signInManager != null)
		  
									   
				  
						
								  
																			
        {
          this._signInManager.Dispose();
          this._signInManager = (ApplicationSignInManager) null;
        }
      }
      base.Dispose(disposing);
    }

    private Microsoft.Owin.Security.IAuthenticationManager AuthenticationManager
								 
						
																				   
		 
																	  
							   
    {
      get => this.HttpContext.GetOwinContext().Authentication;
			 
																						
																															   
																															 
    }

    private void AddErrors(IdentityResult result)
    {
      foreach (string error in result.Errors)
        this.ModelState.AddModelError("", error);
								  
																		 
		 
									
			 
							  
    }

    private ActionResult RedirectToLocal(string returnUrl)
																					
    {
      return this.Url.IsLocalUrl(returnUrl) ? (ActionResult) this.Redirect(returnUrl) : (ActionResult) this.RedirectToAction("Index", "Home");
			 
																																						 
    }

    public class EmailManager
    {
      public static void AppSettings(
        out string UserID,
        out string Password,
        out string SMTPPort,
        out string Host)
      {
        UserID = ConfigurationManager.AppSettings.Get("SMTP_USER");
        Password = ConfigurationManager.AppSettings.Get("SMTP_PASS");
        SMTPPort = ConfigurationManager.AppSettings.Get("SMTP_PORT");
        Host = ConfigurationManager.AppSettings.Get("SMTP_SERVER");
      }

      public static void SendEmail(
        string From,
        string Subject,
        string Body,
        string To,
        string UserID,
        string Password,
        string SMTPPort,
													   
																										   
										  
						
																									  
												  
																		  
																																 
			 
		 

		  
												   
				  
						
								  
        string Host)
      {
        new SmtpClient()
        {
          Host = Host,
          Port = ((int) Convert.ToInt16(SMTPPort)),
          Credentials = ((ICredentialsByHost) new NetworkCredential(UserID, Password)),
          EnableSsl = false
        }.Send(new MailMessage()
									
        {
          To = {
            To
          },
          From = new MailAddress(From),
          Subject = Subject,
          Body = Body,
          IsBodyHtml = true
        });
      }
    }

    internal class ChallengeResult : HttpUnauthorizedResult
    {
      public ChallengeResult(string provider, string redirectUri)
        : this(provider, redirectUri, (string) null)
      {
						  
      }

      public ChallengeResult(string provider, string redirectUri, string userId)
      {
        this.LoginProvider = provider;
        this.RedirectUri = redirectUri;
        this.UserId = userId;
				 
										   
										
      }

      public string LoginProvider { get; set; }


      public string RedirectUri { get; set; }
		 

      public string UserId { get; set; }
															   
												

      public override void ExecuteResult(ControllerContext context)
      {
        AuthenticationProperties properties = new AuthenticationProperties()
        {
          RedirectUri = this.RedirectUri
        };
                if (this.UserId != null) ;
		properties.Dictionary["XsrfId"] = this.UserId;
        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);												
      }
    }
    }
}

