// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Startup
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MystiqueMC.Helpers.Permissions;
using MystiqueMC.Models;
using Owin;
using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;


namespace MystiqueMC
{
  public class Startup
  {
    public void ConfigureAuth(IAppBuilder app)
    {
      app.CreatePerOwinContext<ApplicationDbContext>(new Func<ApplicationDbContext>(ApplicationDbContext.Create));
      app.CreatePerOwinContext<ApplicationUserManager>(new Func<IdentityFactoryOptions<ApplicationUserManager>, IOwinContext, ApplicationUserManager>(ApplicationUserManager.Create));
      app.CreatePerOwinContext<ApplicationSignInManager>(new Func<IdentityFactoryOptions<ApplicationSignInManager>, IOwinContext, ApplicationSignInManager>(ApplicationSignInManager.Create));
      IAppBuilder app1 = app;
      CookieAuthenticationOptions options = new CookieAuthenticationOptions();
      options.AuthenticationType = "ApplicationCookie";
      options.LoginPath = new PathString("/Account/Login");
      options.Provider = (ICookieAuthenticationProvider) new CookieAuthenticationProvider()
      {
        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(TimeSpan.FromMinutes(1.0), (Func<ApplicationUserManager, ApplicationUser, Task<ClaimsIdentity>>) ((manager, user) => user.GenerateUserIdentityAsync((UserManager<ApplicationUser>) manager)))
      };
      app1.UseCookieAuthentication(options);
      app.UseExternalSignInCookie("ExternalCookie");
      app.UseTwoFactorSignInCookie("TwoFactorCookie", TimeSpan.FromMinutes(5.0));
      app.UseTwoFactorRememberBrowserCookie("TwoFactorRememberBrowser");
    }

    public void Configuration(IAppBuilder app)
    {
      this.ConfigureAuth(app);
      this.ConfigureRoles(RolesConfiguration.RolesBase);
    }

    private void ConfigureRoles(string[] rolesBase)
    {
      using (ApplicationDbContext context = new ApplicationDbContext())
      {
        RoleManager<IdentityRole> manager1 = new RoleManager<IdentityRole>((IRoleStore<IdentityRole, string>) new RoleStore<IdentityRole>((DbContext) context));
        foreach (string roleName in rolesBase)
        {
          if (!manager1.RoleExists<IdentityRole, string>(roleName))
          {
            RoleManager<IdentityRole> manager2 = manager1;
            IdentityRole role = new IdentityRole();
            role.Name = roleName;
            manager2.Create<IdentityRole, string>(role);
          }
        }
      }
    }
  }
}
