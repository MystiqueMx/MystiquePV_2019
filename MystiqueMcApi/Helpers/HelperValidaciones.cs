
namespace MystiqueMcApi.Helpers
{
    public class HelperValidaciones
    {
        public static bool ValidatePicturesExtension(string filename)
        {
            string fileExt = System.IO.Path.GetExtension(filename);
            return (fileExt.ToLower() == ".jpeg" || fileExt.ToLower() == ".jpg" ||
                    fileExt.ToLower() == ".png");

        }
        public static bool ValidateDocumentExtension(string filename)
        {
            string fileExt = System.IO.Path.GetExtension(filename);
            return fileExt.ToLower() == ".pdf";
        }
    }
}