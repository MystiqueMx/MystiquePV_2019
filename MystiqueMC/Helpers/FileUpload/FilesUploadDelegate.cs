// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.FileUpload.FilesUploadDelegate
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;


namespace MystiqueMC.Helpers.FileUpload
{
  public class FilesUploadDelegate
  {
    private const int MAX_PARALLEL_UPLOADS = 3;
    private static List<Task> fileUploads = new List<Task>();
    private string serverPath { get; }

	

    internal async Task<string> UploadFileAsync(
      HttpPostedFileBase file,
      string serverPath,
      string route,
      string extension)
    {
      string relativeRoute = route + (Guid.NewGuid().ToString() + extension);
      string absoluteRoute = serverPath + relativeRoute;
      if (FilesUploadDelegate.fileUploads.Count > 3)
        Task.WhenAny((IEnumerable<Task>) FilesUploadDelegate.fileUploads).Wait();
      Task<bool> UploadTask = Task.Run<bool>((Func<bool>) (() => FileIOHelper.SaveFile(file, absoluteRoute)));
      FilesUploadDelegate.fileUploads.Add((Task) UploadTask);
      bool flag = await UploadTask;
      FilesUploadDelegate.fileUploads.Remove((Task) UploadTask);
      return flag ? relativeRoute : string.Empty;
    }















    internal async Task<string> UploadFileAsyncs(
      HttpPostedFileBase file,
      string route,
      string extension)
    {
      string relativeRoute = route + (Guid.NewGuid().ToString() + extension);
      string absoluteRoute = this.serverPath + relativeRoute;
      if (FilesUploadDelegate.fileUploads.Count > 3)
        Task.WhenAny((IEnumerable<Task>) FilesUploadDelegate.fileUploads).Wait();
      Task<bool> UploadTask = Task.Run<bool>((Func<bool>) (() => FileIOHelper.SaveFile(file, absoluteRoute)));
      FilesUploadDelegate.fileUploads.Add((Task) UploadTask);
      bool flag = await UploadTask;
      FilesUploadDelegate.fileUploads.Remove((Task) UploadTask);
      return flag ? relativeRoute : string.Empty;
    }
  }
}
