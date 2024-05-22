// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.Permissions.RolesConfiguration
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll


namespace MystiqueMC.Helpers.Permissions
{
  public static class RolesConfiguration
  {
    private static readonly string[] rolesBase = new string[4]
    {
      "WebMaster",
      "Empresa",
      "Comercio",
      "Analista"
    };
    private static readonly string[] rolesConfigurables = new string[2]
    {
      "Comercio",
      "Analista"
    };
    public const string Superuser = "WebMaster";

    public static string[] RolesBase => RolesConfiguration.rolesBase;

    public static string[] RolesConfigurables => RolesConfiguration.rolesConfigurables;
  }
}
