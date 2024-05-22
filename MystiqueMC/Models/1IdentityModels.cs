// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ApplicationDbContext
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity.EntityFramework;


namespace MystiqueMC.Models
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext()
      : base("SecurityContext", false)
    {
    }

    public static ApplicationDbContext Create() => new ApplicationDbContext();
  }
}
