// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.FileUpload.FilesHelper
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System.Web;


namespace MystiqueMC.Helpers.FileUpload
{
  public static class FilesHelper
  {
    public const string FilesPath = "/Uploads/Images/";
    public const string AnexosPath = "Uploads/AnexosDoctor/";

    public static bool IsPNG(HttpPostedFileBase file) => file.ContentType == "image/png";

    public static bool IsJPG(HttpPostedFileBase file) => file.ContentType == "image/jpg";

    public static bool IsPDF(HttpPostedFileBase file) => file.ContentType == "application/pdf";
  }
}
