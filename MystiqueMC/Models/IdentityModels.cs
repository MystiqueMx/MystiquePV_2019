// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ApplicationUser
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;


namespace MystiqueMC.Models
{
  public class ApplicationUser : IdentityUser
  {
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
      ApplicationUser user = this;
      return await manager.CreateIdentityAsync(user, "ApplicationCookie");
    }
  }
}
