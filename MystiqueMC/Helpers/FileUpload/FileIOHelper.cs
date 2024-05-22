// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.FileUpload.FileIOHelper
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.IO;
using System.Web;


namespace MystiqueMC.Helpers.FileUpload
{
  public static class FileIOHelper
  {
    public static bool SaveFile(HttpPostedFileBase file, string path)
    {
      if (file.ContentLength <= 0)
        return false;
      if (!Directory.Exists(Path.GetDirectoryName(path)))
        Directory.CreateDirectory(Path.GetDirectoryName(path));
      file.SaveAs(path);
      return true;
    }

    public static string ExtensionToContentType(string extension)
    {
      switch (extension.ToLower())
      {
        case ".pdf":
          return "application/pdf";
        case ".png":
          return "image/png";
        case ".jpeg":
        case ".jpg":
          return "image/jpg";
        default:
          throw new NotImplementedException();
      }
    }

    public static string ContentTypeToExtension(string contentType)
    {
      switch (contentType.ToLower())
      {
        case "application/pdf":
          return ".pdf";
        case "image/*":
        case "image/png":
          return ".png";
        case "image/jpeg":
        case "image/jpg":
          return ".jpg";
        default:
          throw new NotImplementedException();
      }
    }

    public static bool DeleteFile(string filePath)
    {
      try
      {
        if (File.Exists(filePath))
          File.Delete(filePath);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
