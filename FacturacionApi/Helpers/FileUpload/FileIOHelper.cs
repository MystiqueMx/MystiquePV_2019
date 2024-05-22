using System;
using System.IO;
using System.Web;

namespace MystiqueMC.Helpers.FileUpload
{
    public static class FileIOHelper
    {
        public static bool SaveFile(HttpPostedFileBase file, string path)
        {
            if (file.ContentLength <= 0) return false;

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            file.SaveAs(path);
            return true;
        }
        public static bool SaveFile(string content, string path)
        {
            if (content.Length <= 0) return false;

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
            }

            using (var fileStreamWriter = File.CreateText(path))
            {
                fileStreamWriter.Write(content);
            }
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
                default: throw new NotImplementedException();
            }
        }
    }
}