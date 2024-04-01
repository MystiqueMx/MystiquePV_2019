using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class FilesController : BaseApiController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();

        [Route("api/PutUserImage/{idCliente}")]
        [HttpPut]
        public ErrorObjCodeResponseBase PutUserImage(int idCliente)
        {
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();
            try
            {
                if (validar.IsAppSecretValid)
                {
                    var httpRequest = HttpContext.Current.Request;

                    if (httpRequest.ContentLength > 0)
                    {
                        var nombre = Guid.NewGuid();

                        var cliente = Contexto.clientes.FirstOrDefault(w => w.idCliente == idCliente);

                        using (var mc = new FileStream(HttpRuntime.AppDomainAppPath + "Uploads\\Mobile\\" + idCliente + "@" + nombre + ".JPG", FileMode.OpenOrCreate))
                        {
                            httpRequest.InputStream.CopyTo(mc);
                            cliente.fechaCargaFoto = DateTime.Now;
                            cliente.urlFotoPerfil = "\\Uploads\\Mobile\\" + idCliente + "@" + nombre + ".JPG";
                            Contexto.SaveChanges();
                            respuesta.estatusPeticion = RespuestaOk;
                        }
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion("La imagen viene vacia.");
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaNoPermisos;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta;
        }


        [Route("api/GetUserImage/{idCliente}")]
        [HttpGet]
        public IHttpActionResult GetUserImage(int? idCliente)
        {
            try
            {
                string ruta = HttpRuntime.AppDomainAppPath;
                FileResult respuesta = new FileResult(@ruta + "/Images/default_profile_picture.png", "image/png");

                if (validar.IsAppSecretValid && idCliente > 0)
                {
                    var cliente = Contexto.clientes.FirstOrDefault(w => w.idCliente == idCliente);
                    ruta = HttpRuntime.AppDomainAppPath + cliente.urlFotoPerfil;
                    if (!string.IsNullOrEmpty(cliente.urlFotoPerfil))
                    {
                        respuesta = new FileResult(@ruta, "image/jpg");
                    }
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return InternalServerError();
            }

        }
        //[HttpPost]
        //public string UploadProfilePicture(string correoElectronico, string contrasenia)
        //{
        //    bool isSavedSuccessfully = true;
        //    string FileUrl = string.Empty;

        //    try
        //    {
        //        clientes cliente = contextEntity.clientes.FirstOrDefault(w => w.email == correoElectronico && w.password == contrasenia);

        //        if (cliente == null)
        //        {
        //            return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The file upload failed, try again later" });
        //        }
        //        if (Request.Files[0] == null || !(Request.Files[0].ContentLength > 0))
        //        {
        //            return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The picture is empty" });
        //        }
        //        if (!HelperValidaciones.ValidatePicturesExtension(Request.Files[0].FileName))
        //        {
        //            return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The picture's extension is not supported" });
        //        }

        //        if (string.IsNullOrEmpty
        //            (FileUrl = FilesUploadDelegate.UploadProfilePicture(Request.Files[0], correoElectronico, ServerPath)))
        //        {
        //            isSavedSuccessfully = false;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                System.IO.File.Delete(ServerPath + cliente.urlFotoPerfil);
        //            }
        //            catch (Exception e)
        //            {
        //                logger.Error("Error:" + e.Message);
        //            }

        //            cliente.urlFotoPerfil = FileUrl;
        //            cliente.fechaCargaFoto = DateTime.Now;
        //            contextEntity.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        isSavedSuccessfully = false;

        //        logger.Error("Error:" + ex.Message);

        //    }
        //    if (!isSavedSuccessfully)
        //    {
        //        return JsonConvert.SerializeObject(new ResponseBase() { Success = false, ErrorMessage = "The file upload failed, try again later" });
        //    }

        //    return JsonConvert.SerializeObject(new ResponseBase() { Success = true, ErrorMessage = "Ok" });
        //}
        //[HttpGet]
        //public FileResult ProfilePictures(string p)
        //{
        //    //Dec
        //    string Params = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(p));

        //    string url = Params;
        //    if (string.IsNullOrEmpty(url) || !System.IO.File.Exists(ServerPath + url))
        //    {
        //        url = "/Images/default_profile_picture.png";
        //    }
        //    string ContentType = FilesIOHelper.ExtensionToContentType(Path.GetExtension(url));
        //    return File(ServerPath + url, ContentType);
        //}


    }
}
