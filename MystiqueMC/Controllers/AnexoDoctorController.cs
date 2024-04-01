using MystiqueMC.Helpers;
using MystiqueMC.Helpers.FileUpload;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    public class AnexoDoctorController : BaseController
    {
        private readonly string HOSTNAME_IMAGENES = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");
        // GET: AnexoDoctor
        public ActionResult Index(int idEmpresa, int idComercio)
        {
            ViewBag.idEmpresa = idEmpresa;
            ViewBag.idComercio = idComercio;
            ViewBag.catAseguranzas = new SelectList(Contexto.CatAseguranzas, "idCatAseguranzas", "descripcion");
            ViewBag.doctorEspecialidad = new SelectList(Contexto.CatDoctorEspecialidad, "idCatDoctorEspecialidad", "descripcion");
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int? id, int idEmpresa, int idComercio)
        {
            var anexoDoctor = Contexto.AnexoDoctor
                .Where(w => w.idAnexoDoctor == id)
                .FirstOrDefault();

            ViewData["HOSTNAME_IMAGENES"] = HOSTNAME_IMAGENES;
            ViewBag.imagenesAny = Contexto.ImagenDoctor.Any(c => c.comercioId == idComercio);
            var Imagenes = Contexto.ImagenDoctor.Where(c => c.comercioId == idComercio).ToList();
            ViewBag.imagenesExtras = Contexto.ImagenDoctor.Where(c => c.comercioId == idComercio).ToList();
            if (Imagenes.Count >= 5)
            {
                ViewBag.puedeSubir = false;
            }
            else
            {
                ViewBag.puedeSubir = true;
            }
            ViewBag.idEmpresa = idEmpresa;
            ViewBag.idComercio = idComercio;
            var aseguranzasDoctor = Contexto.DoctorAseguranzas
                                .Where(w => w.anexoDoctorId == anexoDoctor.idAnexoDoctor)
                                .Select(s => s.catAseguranzaId)
                                .ToList();

            ViewBag.catAseguranzas = new MultiSelectList(Contexto.CatAseguranzas, "idCatAseguranzas", "descripcion", aseguranzasDoctor);
            ViewBag.doctorEspecialidad = new SelectList(Contexto.CatDoctorEspecialidad, "idCatDoctorEspecialidad", "descripcion", anexoDoctor.doctorEspecialidadId);

            return View(anexoDoctor);
        }


        [HttpPost]
        public ActionResult Edit(int? idAnexoDoctor, int doctorEspecialidad, string descripcion, string descripcionIngles, string cedula, string usuario, string contrasena,decimal precioConsulta, int idComercio, int[] catAseguranzas)
        {
            try
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    try
                    {
                        var comercio = Contexto.comercios.Find(idComercio);
                        var datosDoctor = Contexto.AnexoDoctor.Find(idAnexoDoctor);

                        if (datosDoctor != null)
                        {
                            //ANEXO DOCTOR:
                            datosDoctor.cedula = cedula;
                            datosDoctor.descripcion = descripcion;
                            datosDoctor.descripcionIngles = descripcionIngles;
                            datosDoctor.userName = usuario;
                            datosDoctor.password = contrasena;
                            datosDoctor.estatus = true;
                            datosDoctor.precioConsulta = precioConsulta;
                            datosDoctor.doctorEspecialidadId = doctorEspecialidad;
                            Contexto.Entry(datosDoctor).State = EntityState.Modified;

                            //ASEGURANZAS DEL DOCTOR:
                            var aseguranzasDoctor = Contexto.DoctorAseguranzas
                                .Where(w => w.anexoDoctorId == datosDoctor.idAnexoDoctor).ToArray();
                            Contexto.DoctorAseguranzas.RemoveRange(aseguranzasDoctor);

                            foreach (var item in catAseguranzas)
                            {
                                var aseguranza = Contexto.CatAseguranzas
                                    .Where(w => w.idCatAseguranzas == item)?.FirstOrDefault();
                                if (aseguranza != null)
                                {
                                    Contexto.DoctorAseguranzas.Add(new DAL.DoctorAseguranzas
                                    {
                                        anexoDoctorId = datosDoctor.idAnexoDoctor,
                                        catAseguranzaId = aseguranza.idCatAseguranzas,
                                        CatAseguranzas = aseguranza,
                                        AnexoDoctor = datosDoctor
                                    });
                                }
                            }
                            Contexto.SaveChanges();
                            tx.Commit();
                            return RedirectToAction("Edit", "AnexoDoctor", new { id = datosDoctor.idAnexoDoctor, idEmpresa = comercio.empresaId, idComercio = comercio.idComercio });
                        }
                        else
                        {
                            datosDoctor = Contexto.AnexoDoctor.Add(new DAL.AnexoDoctor
                            {
                                cedula = cedula,
                                descripcion = descripcion,
                                descripcionIngles = descripcionIngles,
                                userName = usuario,
                                password = contrasena,
                                estatus = true,
                                precioConsulta = precioConsulta,
                                doctorEspecialidadId = doctorEspecialidad
                            });

                            Contexto.AnexoDoctor.Add(datosDoctor);
                            Contexto.SaveChanges();

                            comercio.anexoDoctorId = datosDoctor.idAnexoDoctor;
                            Contexto.Entry(comercio).State = EntityState.Modified;

                            //ASEGURANZAS DEL DOCTOR:
                            var aseguranzasDoctor = Contexto.DoctorAseguranzas
                                .Where(w => w.anexoDoctorId == datosDoctor.idAnexoDoctor).ToArray();
                            Contexto.DoctorAseguranzas.RemoveRange(aseguranzasDoctor);

                            foreach (var item in catAseguranzas)
                            {
                                var aseguranza = Contexto.CatAseguranzas
                                    .Where(w => w.idCatAseguranzas == item)?.FirstOrDefault();
                                if (aseguranza != null)
                                {
                                    Contexto.DoctorAseguranzas.Add(new DAL.DoctorAseguranzas
                                    {
                                        anexoDoctorId = datosDoctor.idAnexoDoctor,
                                        catAseguranzaId = aseguranza.idCatAseguranzas,
                                        CatAseguranzas = aseguranza,
                                        AnexoDoctor = datosDoctor
                                    });
                                }
                            }

                            Contexto.SaveChanges();
                            tx.Commit();
                            return RedirectToAction("Edit", "AnexoDoctor", new { id = datosDoctor.idAnexoDoctor, idEmpresa = comercio.empresaId, idComercio = comercio.idComercio });
                        }


                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        Logger.Error(ex);
                        Console.WriteLine(ex);
                    }

                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error, favor de contactar a su administrador.");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SubirImagenDoctor(string name, string type, HttpPostedFileBase file, int idComercio)
        {
            try
            {
                var extension = FileIOHelper.ContentTypeToExtension(type);
                var filesDelegate = new Helpers.Files.FilesUploadDelegate(HostingEnvironment.ApplicationPhysicalPath);

                var result = await filesDelegate.UploadFileAsync(file, Helpers.Files.FilesHelper.Uploads.AnexosPath, extension);
                if (string.IsNullOrEmpty(result))
                {
                    throw new ApplicationException($"await filesDelegate.UploadFileAsync(file, {Helpers.Files.FilesHelper.Uploads.AnexosPath}, {extension})");
                }
                else
                {
                    try
                    {
                        var doc = Contexto.ImagenDoctor.Add(new DAL.ImagenDoctor
                        {
                            comercioId = idComercio,
                            ruta = result,
                        });
                        Contexto.SaveChanges();
                        return Json(doc.idImagenDoctor);

                    }
                    catch (Exception ee)
                    {
                        Logger.Error(ee);
                        FileIOHelper.DeleteFile(HostingEnvironment.ApplicationPhysicalPath + result);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new HttpStatusCodeResult(500);
            }
        }

        public ActionResult EliminarImagen(int id, int idComercios, int idEmpresa, int idAnexo)
        {
            try
            {
                var imagen = Contexto.ImagenDoctor.Find(id);
                Contexto.ImagenDoctor.Remove(imagen);
                Contexto.SaveChanges();

                return RedirectToAction("Edit", "AnexoDoctor", new { id = idAnexo, idEmpresa = idEmpresa, idComercio = idComercios });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("Ocurrio un error, favor de contactar a su administrador.");
                return RedirectToAction("Edit", "AnexoDoctor");
            }
        }
    }
}