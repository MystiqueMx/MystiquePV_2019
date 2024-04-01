using System;
using System.IO;
using System.Web;

namespace ApiDoc.Helpers
{
    public class FilesIOHelper
    {
        public static string ParseTempPDFFilename => Guid.NewGuid().ToString() + ".pdf";

        public static bool SaveFile(HttpPostedFileBase file, string path)
        {
            if (file.ContentLength > 0)
            {
                file.SaveAs(path);
                return true;
            }
            return false;
        }

        public static byte[] ReadFile(string path)
        {
            if (!File.Exists(path))
                return null;
            byte[] file = File.ReadAllBytes(path);
            return file;
        }

        public static string ParseTempPDFPath(string FileName)
        {
            return Path.GetTempPath() + FileName;
        }
        internal static string ParseCreateDocumentPath(string IdPaciente, string DescripcionTipoDocumento, string BasePath)
        {
            string path = "\\Uploads\\" + IdPaciente + "\\" + DescripcionTipoDocumento.Replace(" ", "_").Replace("'", "") + "\\";
            if (!Directory.Exists(BasePath + path))
                Directory.CreateDirectory(BasePath + path);
            return path;
        }
        internal static string ParseCreateDocumentFilename(string DescripcionTipoDocumento, long Hash)
        {
            return DescripcionTipoDocumento.Replace(" ", "_").Replace("'", "") + "-" + Hash;
        }
        internal static string ParseProfilePictureFilename(string usuarioId)
        {
            return DateTime.Now.ToFileTime().ToString();
        }
        internal static string ParseProfilePicturePath(string usuarioId, string BasePath)
        {
            string path = "\\Uploads\\Mobile\\" + usuarioId.Replace(" ", "_").Replace("@", "_").Replace(".", "_") + "\\";
            if (!Directory.Exists(BasePath + path))
                Directory.CreateDirectory(BasePath + path);
            return path;
        }
        internal static string ExtensionToContentType(string extension)
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