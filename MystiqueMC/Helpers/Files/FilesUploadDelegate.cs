// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.Files.FilesUploadDelegate
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.Helpers.FileUpload;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;


namespace MystiqueMC.Helpers.Files
{
  public class FilesUploadDelegate
  {
    private const int MaxParallelUploads = 3;
    private static readonly List<Task> FileUploads = new List<Task>();

    public FilesUploadDelegate(string serverPath) => this.serverPath = serverPath;

    private string serverPath { get; }

    internal async Task<string> UploadFileAsync(
      HttpPostedFileBase file,
      string route,
      string extension,
      string serverPathOverwrite = null)
    {
      if (string.IsNullOrEmpty(serverPathOverwrite))
        serverPathOverwrite = this.serverPath;
      string relativeRoute = route + (Guid.NewGuid().ToString() + extension);
      string absoluteRoute = serverPathOverwrite + relativeRoute;
      if (FilesUploadDelegate.FileUploads.Count > 3)
        Task.WhenAny((IEnumerable<Task>) FilesUploadDelegate.FileUploads).Wait();
      Task<bool> uploadTask = Task.Run<bool>((Func<bool>) (() => FileIOHelper.SaveFile(file, absoluteRoute)));
      FilesUploadDelegate.FileUploads.Add((Task) uploadTask);
      bool flag = await uploadTask;
      FilesUploadDelegate.FileUploads.Remove((Task) uploadTask);
      return flag ? relativeRoute : string.Empty;
    }
  }
}
