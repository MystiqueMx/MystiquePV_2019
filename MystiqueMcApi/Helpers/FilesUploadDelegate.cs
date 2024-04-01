using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace MystiqueMcApi.Helpers
{
    public class FilesUploadDelegate
    {
        private static int MAX_PARALLEL_UPLOADS = 5;
        private static List<Task> fileUploads = new List<Task>();
        public static async Task<string> UploadDocumentsAsync(HttpPostedFileBase file, string docType, string patientId, string serverPath)
        {
            string fName = string.Empty;
            fName = file.FileName;

            string newFileName = FilesIOHelper.ParseCreateDocumentFilename(docType, DateTime.Now.ToFileTime()) + Path.GetExtension(fName);
            string documentsPath = FilesIOHelper.ParseCreateDocumentPath(patientId, docType, serverPath);
            fName = documentsPath + newFileName;

            if (fileUploads.Count > MAX_PARALLEL_UPLOADS)
                Task.WhenAny(fileUploads).Wait();

            Task<bool> UploadTask = Task.Run(() => { return FilesIOHelper.SaveFile(file, serverPath + fName); });
            fileUploads.Add(UploadTask);

            bool isSavedSuccessfully = await UploadTask;
            fileUploads.Remove(UploadTask);

            return isSavedSuccessfully ? fName : string.Empty;
        }

        internal static string UploadProfilePicture(HttpPostedFileBase file, string usuarioId, string serverPath)
        {
            string fName = string.Empty;
            fName = file.FileName;

            string newFileName = FilesIOHelper.ParseProfilePictureFilename(usuarioId) + Path.GetExtension(fName);
            string documentsPath = FilesIOHelper.ParseProfilePicturePath(usuarioId, serverPath);
            fName = documentsPath + newFileName;

            if (fileUploads.Count > MAX_PARALLEL_UPLOADS)
                Task.WhenAny(fileUploads).Wait();

            Task<bool> UploadTask = Task.Run(() => { return FilesIOHelper.SaveFile(file, serverPath + fName); });
            fileUploads.Add(UploadTask);

            // TODO DONT RETURN TRUE 
            UploadTask.Wait();
            bool isSavedSuccessfully = true;

            fileUploads.Remove(UploadTask);

            return isSavedSuccessfully ? fName : string.Empty;
        }
    }
}