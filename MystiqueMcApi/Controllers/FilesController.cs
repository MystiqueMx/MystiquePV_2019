using MystiqueMC.DAL;
using MystiqueMcApi.Helpers;
using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using MystiqueMcApi.Models.Salidas;

namespace MystiqueMcApi.Controllers
{
    public class FilesController : Controller
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        private string ServerPath => Server.MapPath(@"~");

        [HttpPost]
        public string UploadProfilePicture(string correoElectronico, string contrasenia)
        {
            bool isSavedSuccessfully = true;
            string FileUrl = string.Empty;

            try
            {
                clientes cliente = contextEntity.clientes.FirstOrDefault(w => w.email == correoElectronico && w.password == contrasenia);

                if (cliente == null)
                {
                    return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The file upload failed, try again later" });
                }
                if (Request.Files[0] == null || !(Request.Files[0].ContentLength > 0))
                {
                    return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The picture is empty" });
                }
                if (!HelperValidaciones.ValidatePicturesExtension(Request.Files[0].FileName))
                {
                    return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The picture's extension is not supported" });
                }

                if (string.IsNullOrEmpty
                    (FileUrl = FilesUploadDelegate.UploadProfilePicture(Request.Files[0], correoElectronico, ServerPath)))
                {
                    isSavedSuccessfully = false;
                }
                else
                {
                    try
                    {
                        System.IO.File.Delete(ServerPath + cliente.urlFotoPerfil);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error:" + e.Message);
                    }

                    cliente.urlFotoPerfil = FileUrl;
                    cliente.fechaCargaFoto = DateTime.Now;
                    contextEntity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;

                logger.Error("Error:" + ex.Message);

            }
            if (!isSavedSuccessfully)
            {
                return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The file upload failed, try again later" });
            }

            return JsonConvert.SerializeObject(new ResponseBase() { Success = true, ErrorMessage = "Ok" });
        }
        [HttpGet]
        public FileResult ProfilePictures(string p)
        {
            //Dec
            string Params = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(p));

            string url = Params;
            if (string.IsNullOrEmpty(url) || !System.IO.File.Exists(ServerPath + url))
            {
                url = "/Images/default_profile_picture.png";
            }
            string ContentType = FilesIOHelper.ExtensionToContentType(Path.GetExtension(url));
            return File(ServerPath + url, ContentType);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }
    }
}
