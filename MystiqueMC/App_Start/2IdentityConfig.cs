// Decompiled with JetBrains decompiler
// Type: MystiqueMC.ApplicationUserManager
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using MystiqueMC.Models;
using System;
using System.Data.Entity;


namespace MystiqueMC
{
  public class ApplicationUserManager : UserManager<ApplicationUser>
  {
    public ApplicationUserManager(IUserStore<ApplicationUser> store)
      : base(store)
    {
    }

    public static ApplicationUserManager Create(
      IdentityFactoryOptions<ApplicationUserManager> options,
      IOwinContext context)
    {
      ApplicationUserManager manager = new ApplicationUserManager((IUserStore<ApplicationUser>) new UserStore<ApplicationUser>((DbContext) context.Get<ApplicationDbContext>()));
      ApplicationUserManager applicationUserManager1 = manager;
      Microsoft.AspNet.Identity.UserValidator<ApplicationUser> userValidator = new Microsoft.AspNet.Identity.UserValidator<ApplicationUser>((UserManager<ApplicationUser, string>) manager);
      userValidator.AllowOnlyAlphanumericUserNames = false;
      userValidator.RequireUniqueEmail = true;
      applicationUserManager1.UserValidator = (IIdentityValidator<ApplicationUser>) userValidator;
      manager.PasswordValidator = (IIdentityValidator<string>) new Microsoft.AspNet.Identity.PasswordValidator()
      {
        RequiredLength = 6,
        RequireNonLetterOrDigit = false,
        RequireDigit = false,
        RequireLowercase = false,
        RequireUppercase = false
      };
      manager.UserLockoutEnabledByDefault = true;
      manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5.0);
      manager.MaxFailedAccessAttemptsBeforeLockout = 5;
      ApplicationUserManager applicationUserManager2 = manager;
      PhoneNumberTokenProvider<ApplicationUser> provider1 = new PhoneNumberTokenProvider<ApplicationUser>();
      provider1.MessageFormat = "Your security code is {0}";
      applicationUserManager2.RegisterTwoFactorProvider("Phone Code", (IUserTokenProvider<ApplicationUser, string>) provider1);
      ApplicationUserManager applicationUserManager3 = manager;
      EmailTokenProvider<ApplicationUser> provider2 = new EmailTokenProvider<ApplicationUser>();
      provider2.Subject = "Security Code";
      provider2.BodyFormat = "Your security code is {0}";
      applicationUserManager3.RegisterTwoFactorProvider("Email Code", (IUserTokenProvider<ApplicationUser, string>) provider2);
      manager.EmailService = (IIdentityMessageService) new MystiqueMC.EmailService();
      manager.SmsService = (IIdentityMessageService) new MystiqueMC.SmsService();
      IDataProtectionProvider protectionProvider = options.DataProtectionProvider;
      if (protectionProvider != null)
        manager.UserTokenProvider = (IUserTokenProvider<ApplicationUser, string>) new DataProtectorTokenProvider<ApplicationUser>(protectionProvider.Create("ASP.NET Identity"));
      return manager;
    }
  }
}
