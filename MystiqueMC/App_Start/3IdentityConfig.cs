// Decompiled with JetBrains decompiler
// Type: MystiqueMC.ApplicationSignInManager
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MystiqueMC.Models;
using System.Security.Claims;
using System.Threading.Tasks;


namespace MystiqueMC
{
  public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
  {
    public ApplicationSignInManager(
      ApplicationUserManager userManager,
      IAuthenticationManager authenticationManager)
      : base((Microsoft.AspNet.Identity.UserManager<ApplicationUser, string>) userManager, authenticationManager)
    {
    }

    public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
    {
      return user.GenerateUserIdentityAsync((Microsoft.AspNet.Identity.UserManager<ApplicationUser>) this.UserManager);
    }

    public static ApplicationSignInManager Create(
      IdentityFactoryOptions<ApplicationSignInManager> options,
      IOwinContext context)
    {
      return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
    }
  }
}
