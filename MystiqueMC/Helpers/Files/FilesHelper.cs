// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.Files.FilesHelper
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System.Web;


namespace MystiqueMC.Helpers.Files
{
  public static class FilesHelper
  {
    public static bool IsPng(HttpPostedFileBase file) => file.ContentType == "image/png";

    public static bool IsJpg(HttpPostedFileBase file) => file.ContentType == "image/jpg";

    public static bool IsPdf(HttpPostedFileBase file) => file.ContentType == "application/pdf";

    public class Resources
    {
      public const string CatCfdiXsd = "/Resources/SAT/catCFDI.xsd";
    }

    public class Uploads
    {
      public const string AnexosPath = "/Uploads/AnexosDoctor/";
      public const string ImagenComerciosPath = "/Uploads/Comercios/";
    }
  }
}
