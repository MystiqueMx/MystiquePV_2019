// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Helpers.SecurityDbContext
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity.EntityFramework;


namespace MystiqueMC.Models.Helpers
{
  public class SecurityDbContext : IdentityDbContext<ApplicationUser>
  {
    public SecurityDbContext()
      : base("MystiqueMeDevEntities", false)
    {
    }

    public static SecurityDbContext Create() => new SecurityDbContext();
  }
}
