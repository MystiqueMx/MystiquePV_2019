using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace MystiqueMC.Helpers.FileUpload
{
    public class FilesUploadDelegate
    {
        private const string BasePath = @"Documentos\\";
        /// <summary>
        ///     Delegate para guardar archivos a un directorio
        /// </summary>
        /// <param name="file">Archivo recibido en un request</param>
        /// <param name="serverPath">Directorio del aplicativo en el servidor</param>
        /// <param name="route">Directorio relativo donde se cargara el archivo</param>
        /// <param name="extension">Extension con la que se guarda el archivo</param>
        /// <returns></returns>
        internal string BackupCadenaOriginalToFile(string fileContent, string serverPath, string route, string facturaId, string extension)
        {
            var newFileName = facturaId + extension;
            var relativeRoute =  BasePath + route + newFileName;
            var absoluteRoute = serverPath + relativeRoute;

            return FileIOHelper.SaveFile(fileContent, absoluteRoute) ? relativeRoute : string.Empty;
        }
    }
}