using System.Web;

namespace MystiqueMC.Helpers.FileUpload
{
    public static class FilesHelper
    {
        public const string OriginalesFacturasPath = @"/OriginalesFacturas/";
        public static bool IsPng(HttpPostedFileBase file) => file.ContentType == "image/png";
        public static bool IsJpg(HttpPostedFileBase file) => file.ContentType == "image/jpg";
        public static bool IsPdf(HttpPostedFileBase file) => file.ContentType == "application/pdf";
    }
}