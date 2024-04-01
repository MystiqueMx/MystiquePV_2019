using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.Files;
using MystiqueMC.Helpers.FileUpload;
using MystiqueMC.Helpers.SAT;
using MystiqueMC.Models;
using MystiqueMC.Models.Responses;
using MystiqueMC.Models.Sucursal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [Authorize]
    public class SucursalesController : BaseController
    {
        // GET: Sucursales
        public async Task<ActionResult> Index(int? item, int? IdComercio, int? idEmpresa)
        {
            string rol = Session.ObtenerRol();
            ViewBag.rol = rol;

            var usuarioFirmado = Session.ObtenerUsuario();
            ViewBag.idcomercio = IdComercio;



            var sucursales = SucursalesFirmadas;

            if (IdComercio.HasValue)
            {
                var comercio = Contexto.comercios.Find(IdComercio.Value);
                sucursales = sucursales.Where(c => c.comercioId == IdComercio.Value);
                ViewBag.Comercio = comercio;
            }
            if (item.HasValue)
                sucursales = sucursales.Where(c => c.idSucursal == item.Value);
            return View(await sucursales.ToListAsync());
        }

        [HttpGet]
        public ActionResult Create(int cc)
        {
            ViewBag.idcomecio = cc;

            ViewBag.ciudadId = new SelectList(Contexto.catCiudades, "idCatCiudad", "ciudadDescr");
            ViewBag.estadoId = new SelectList(Contexto.catEstados, "idCatEstado", "estadoDescr");
            ViewBag.catRegimenFiscalId = new SelectList(Contexto.catRegimenFiscal, "idCatRegimenFiscal", "descripcion");

            var comercio = Contexto.comercios.FirstOrDefault(f => f.idComercio == cc);

            if (comercio != null) {
                ViewBag.catZonaId = new SelectList(Contexto.CatZonas.Where(w => w.empresaId == comercio.empresaId ), "idCatZona", "descripcion");
            }

            return View(new SucursalViewModel
            {
                comercioId = cc,
                idDatoFiscal = 0,
                idSucursal = 0,
                idDireccion = 0,
                idSucursalHorarioDomingo = 0,
                idSucursalHorarioJueves = 0,
                idSucursalHorarioLunes = 0,
                idSucursalHorarioMartes = 0,
                idSucursalHorarioMiercoles = 0,
                idSucursalHorarioSabado = 0,
                idSucursalHorarioViernes = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SucursalViewModel sucursalModel, string picker)
        {
            if (ModelState.IsValid)
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    try
                    {
                        var dir =
                        Contexto.direccion.Add(new DAL.direccion
                        {
                            calle = sucursalModel.calle,
                            colonia = sucursalModel.colonia,
                            numExterior = sucursalModel.NoExterior,
                            numInterior = sucursalModel.NoInterior,
                            codigoPostal = sucursalModel.codigoPostal,
                            catCiudadId = sucursalModel.ciudadId,
                            catEstadoId = sucursalModel.estadoId,
                            entreCalles = sucursalModel.entreCalles
                        });

                        var suc =
                        Contexto.sucursales.Add(new DAL.sucursales
                        {
                            comercioId = sucursalModel.comercioId,
                            activo = true,
                            fechaRegistro = DateTime.Now,
                            usuarioRegistroId = Convert.ToInt32(Session.ObtenerUsuario().idUsuario),
                            nombre = sucursalModel.nombre,
                            telefono = sucursalModel.telefono,
                            latitud = sucursalModel.latitud,
                            longitud = sucursalModel.longitud,
                            placeId = sucursalModel.placeId,
                            radioZonaMystique = sucursalModel.radioZonaMystique,
                            sucursalPuntoVenta = sucursalModel.sucursalPuntoVenta,
                            catZonaId =  sucursalModel.catZonaId,
                            direccion = dir,
                            colorIndicador = picker
                        });

                        #region Sucursal Horario
                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = suc,
                            diasNum = sucursalModel.diaLunes,
                            horarioInicio = sucursalModel.HorarioInicioLunes,
                            horarioFin = sucursalModel.HorarioFinLunes,
                            descripcion = sucursalModel.descripcionLunes,
                            dayOfWeek = 1
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = suc,
                            diasNum = sucursalModel.diaMartes,
                            horarioInicio = sucursalModel.HorarioInicioMartes,
                            horarioFin = sucursalModel.HorarioFinMartes,
                            descripcion = sucursalModel.descripcionMartes,
                            dayOfWeek = 2
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = suc,
                            diasNum = sucursalModel.diaMiercoles,
                            horarioInicio = sucursalModel.HorarioInicioMiercoles,
                            horarioFin = sucursalModel.HorarioFinMiercoles,
                            descripcion = sucursalModel.descripcionMiercoles,
                            dayOfWeek = 3
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = suc,
                            diasNum = sucursalModel.diaJueves,
                            horarioInicio = sucursalModel.HorarioInicioJueves,
                            horarioFin = sucursalModel.HorarioFinJueves,
                            descripcion = sucursalModel.descripcionJueves,
                            dayOfWeek = 4
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = suc,
                            diasNum = sucursalModel.diaViernes,
                            horarioInicio = sucursalModel.HorarioInicioViernes,
                            horarioFin = sucursalModel.HorarioFinViernes,
                            descripcion = sucursalModel.descripcionViernes,
                            dayOfWeek = 5
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = suc,
                            diasNum = sucursalModel.diaSabado,
                            horarioInicio = sucursalModel.HorarioInicioSabado,
                            horarioFin = sucursalModel.HorarioFinSabado,
                            descripcion = sucursalModel.descripcionSabado,
                            dayOfWeek = 6
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = suc,
                            diasNum = sucursalModel.diaDomingo,
                            horarioInicio = sucursalModel.HorarioInicioDomingo,
                            horarioFin = sucursalModel.HorarioFinDomingo,
                            descripcion = sucursalModel.descripcionDomingo,
                            dayOfWeek = 0
                        });
                        #endregion

                        Contexto.SaveChanges();
                        tx.Commit();

                        var outResultadoParameter = new ObjectParameter("resultado", typeof(string));
                        ErrorCodeResponseBase respuesta = new ErrorCodeResponseBase();
                        Contexto.SP_PV_Copiar_Productos_Comercio_Sucursal(suc.idSucursal, suc.comercioId, UsuarioActual.idUsuario, outResultadoParameter);
                        char delimiter = ';';
                        string[] resultados = outResultadoParameter.Value.ToString().Split(delimiter);
                        if (resultados[0].Equals("ERROR"))
                        {
                            respuesta = RespuestaErrorValidacion(resultados[1]);
                            ShowAlertDanger("Ocurrio un error al actualizar el menu de la sucursal.");
                        }

                        return RedirectToAction("Index", "sucursales", new { IdComercio = sucursalModel.comercioId });
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException)
                    {
                        tx.Rollback();
                    }
                }
            }
            ViewBag.ciudadId = new SelectList(Contexto.catCiudades, "idCatCiudad", "ciudadDescr", sucursalModel.idDireccion);
            ViewBag.estadoId = new SelectList(Contexto.catEstados, "idCatEstado", "estadoDescr", sucursalModel.idDireccion);
            ViewBag.catRegimenFiscalId = new SelectList(Contexto.catRegimenFiscal, "idCatRegimenFiscal", "descripcion", sucursalModel.catRegimenFiscalId);

            return View(sucursalModel);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id, int cc)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SucursalViewModel sucursalView = new SucursalViewModel();
            sucursales sucursales = await Contexto.sucursales.FindAsync(id);
            ViewBag.idcomecio = cc;
            if (sucursales == null)
            {
                return HttpNotFound();
            }

            #region Sucursal
            sucursalView.idSucursal = sucursales.idSucursal;
            sucursalView.comercioId = sucursales.comercioId;
            sucursalView.nombre = sucursales.nombre;
            sucursalView.telefono = sucursales.telefono;
            sucursalView.direccionSucursal = sucursales.direccionId;
            sucursalView.latitud = sucursales.latitud;
            sucursalView.longitud = sucursales.longitud;
            sucursalView.placeId = sucursales.placeId;
            sucursalView.radioZonaMystique = Convert.ToInt32(sucursales.radioZonaMystique);
            sucursalView.sucursalPuntoVenta = Convert.ToInt32(sucursales.sucursalPuntoVenta);
            sucursalView.catZonaId = Convert.ToInt32(sucursales.catZonaId);
            sucursalView.ColorIndicador = sucursales.colorIndicador;
            #endregion

            #region Datos Fiscales
            var sc = sucursales.confDatosFiscalesSucursal.Where(w => w.datoFiscalId == w.datosFiscales.idDatoFiscal).Select(w => w.datosFiscales).FirstOrDefault();

            if (sc != null)
            {
                sucursalView.idDatoFiscal = sc.idDatoFiscal;
                sucursalView.razonFiscal = sc.nombreFiscal;
                sucursalView.rfc = sc.rfc;
                sucursalView.direccionFiscal = sc.direccionId.HasValue ? sc.direccionId.Value : 0;
                sucursalView.codigoP = sc.cp;
                sucursalView.catRegimenFiscalId = sc.catRegimenFiscalId.HasValue ? sc.catRegimenFiscalId.Value : 0;
            }
            #endregion

            #region Direccion
            sucursalView.idDireccion = sucursales.direccion.idDireccion;
            sucursalView.calle = sucursales.direccion.calle;
            sucursalView.colonia = sucursales.direccion.colonia;
            sucursalView.NoExterior = sucursales.direccion.numExterior;
            sucursalView.NoInterior = sucursales.direccion.numInterior;
            sucursalView.codigoPostal = sucursales.direccion.codigoPostal;
            sucursalView.ciudadId = sucursales.direccion.catCiudadId.HasValue ? sucursales.direccion.catCiudadId.Value : 0;
            sucursalView.estadoId = sucursales.direccion.catEstadoId.HasValue ? sucursales.direccion.catEstadoId.Value : 0;
            sucursalView.entreCalles = sucursales.direccion.entreCalles;
            #endregion

            #region Sucursal Horario
            var horarioDiaLunes = sucursales.sucursalHorarios.FirstOrDefault(c => c.diasNum?.ToUpper() == "LUNES");
            if (horarioDiaLunes != null)
            {
                sucursalView.idSucursalHorarioLunes = horarioDiaLunes.idSucursalHorario;
                sucursalView.diaLunes = horarioDiaLunes.diasNum;
                sucursalView.HorarioInicioLunes = horarioDiaLunes.horarioInicio;
                sucursalView.HorarioFinLunes = horarioDiaLunes.horarioFin;
                sucursalView.descripcionLunes = horarioDiaLunes.descripcion;
            }

            var horarioDiaMartes = sucursales.sucursalHorarios.FirstOrDefault(c => c.diasNum?.ToUpper() == "MARTES");
            if (horarioDiaMartes != null)
            {
                sucursalView.idSucursalHorarioMartes = horarioDiaMartes.idSucursalHorario;
                sucursalView.diaMartes = horarioDiaMartes.diasNum;
                sucursalView.HorarioInicioMartes = horarioDiaMartes.horarioInicio;
                sucursalView.HorarioFinMartes = horarioDiaMartes.horarioFin;
                sucursalView.descripcionMartes = horarioDiaMartes.descripcion;
            }

            var horarioDiaMiercoles = sucursales.sucursalHorarios.FirstOrDefault(c => c.diasNum?.ToUpper() == "MIERCOLES");
            if (horarioDiaMiercoles != null)
            {
                sucursalView.idSucursalHorarioMiercoles = horarioDiaMiercoles.idSucursalHorario;
                sucursalView.diaMiercoles = horarioDiaMiercoles.diasNum;
                sucursalView.HorarioInicioMiercoles = horarioDiaMiercoles.horarioInicio;
                sucursalView.HorarioFinMiercoles = horarioDiaMiercoles.horarioFin;
                sucursalView.descripcionMiercoles = horarioDiaMiercoles.descripcion;
            }

            var horarioDiaJueves = sucursales.sucursalHorarios.FirstOrDefault(c => c.diasNum?.ToUpper() == "JUEVES");
            if (horarioDiaJueves != null)
            {
                sucursalView.idSucursalHorarioJueves = horarioDiaJueves.idSucursalHorario;
                sucursalView.diaJueves = horarioDiaJueves.diasNum;
                sucursalView.HorarioInicioJueves = horarioDiaJueves.horarioInicio;
                sucursalView.HorarioFinJueves = horarioDiaJueves.horarioFin;
                sucursalView.descripcionJueves = horarioDiaJueves.descripcion;
            }

            var horarioDiaViernes = sucursales.sucursalHorarios.FirstOrDefault(c => c.diasNum?.ToUpper() == "VIERNES");
            if (horarioDiaViernes != null)
            {
                sucursalView.idSucursalHorarioViernes = horarioDiaViernes.idSucursalHorario;
                sucursalView.diaViernes = horarioDiaViernes.diasNum;
                sucursalView.HorarioInicioViernes = horarioDiaViernes.horarioInicio;
                sucursalView.HorarioFinViernes = horarioDiaViernes.horarioFin;
                sucursalView.descripcionViernes = horarioDiaViernes.descripcion;
            }

            var horarioDiaSabado = sucursales.sucursalHorarios.FirstOrDefault(c => c.diasNum?.ToUpper() == "SABADO");
            if (horarioDiaSabado != null)
            {
                sucursalView.idSucursalHorarioSabado = horarioDiaSabado.idSucursalHorario;
                sucursalView.diaSabado = horarioDiaSabado.diasNum;
                sucursalView.HorarioInicioSabado = horarioDiaSabado.horarioInicio;
                sucursalView.HorarioFinSabado = horarioDiaSabado.horarioFin;
                sucursalView.descripcionSabado = horarioDiaSabado.descripcion;
            }

            var horarioDiaDomingo = sucursales.sucursalHorarios.FirstOrDefault(c => c.diasNum?.ToUpper() == "DOMINGO");
            if (horarioDiaDomingo != null)
            {
                sucursalView.idSucursalHorarioDomingo = horarioDiaDomingo.idSucursalHorario;
                sucursalView.diaDomingo = horarioDiaDomingo.diasNum;
                sucursalView.HorarioInicioDomingo = horarioDiaDomingo.horarioInicio;
                sucursalView.HorarioFinDomingo = horarioDiaDomingo.horarioFin;
                sucursalView.descripcionDomingo = horarioDiaDomingo.descripcion;
            }
            #endregion

           

            int Value;
            bool SelectedValue = Contexto.sucursales.Where(w => w.idSucursal == id).Select(s => s.activo).FirstOrDefault();
            if (SelectedValue) Value = 1;
            else Value = 0;

            sucursalView.activo = Value;
            var Estatus = new EstatusModel[] { new EstatusModel { Id = 1, Text = "ACTIVO" }, new EstatusModel { Id = 0, Text = "INACTIVO" } };
            sucursalView.Statuses = new SelectList(Estatus, "Id", "Text", Value);
         
            //new SelectList(list, "Value", "Text", Value);
            //ViewBag.ciudadId = new SelectList(Contexto.catCiudades, "idCatCiudad", "ciudadDescr", sucursalView.idDireccion);
            //ViewBag.estadoId = new SelectList(Contexto.catEstados, "idCatEstado", "estadoDescr", sucursalView.idDireccion);
            ViewBag.estados = Contexto.catEstados.ToList();
            ViewBag.ciudads = Contexto.catCiudades.ToList();
            ViewBag.catRegimenFiscalId = new SelectList(Contexto.catRegimenFiscal, "idCatRegimenFiscal", "descripcion", sucursalView.catRegimenFiscalId);

            var comercio = Contexto.comercios.FirstOrDefault(f => f.idComercio == cc);

            if (comercio != null)
            {
                ViewBag.zonas = Contexto.CatZonas.Where(w => w.empresaId == comercio.empresaId).ToList();
                //ViewBag.catZonaId = new SelectList(Contexto.CatZonas.Where(w => w.empresaId == comercio.empresaId), "idCatZona", "descripcion");
            }
            return View(sucursalView);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SucursalViewModel sucursalModel, string longitud, string Lat, string place, string picker)
        {
         
            if (ModelState.IsValid)
            {
                using (var tx = Contexto.Database.BeginTransaction())
                {
                    try
                    {
                        Lat = "32.62453889999999";
                        longitud = "-115.45226230000003";
                        place = "ChIJ0913qAxw14ARmvXN5aAzANQ";

                        if (string.IsNullOrEmpty(sucursalModel.longitud))
                        { longitud = sucursalModel.longitud; }
                        if (string.IsNullOrEmpty(sucursalModel.latitud))
                        { Lat = sucursalModel.latitud; }
                        if (string.IsNullOrEmpty(sucursalModel.placeId))
                        { place = sucursalModel.placeId; }

                        direccion direc = Contexto.direccion.Find(sucursalModel.idDireccion);
                        direc.colonia = sucursalModel.colonia;
                        direc.calle = sucursalModel.calle;
                        direc.numExterior = sucursalModel.NoExterior;
                        direc.numInterior = sucursalModel.NoInterior;
                        direc.codigoPostal = sucursalModel.codigoPostal;
                        direc.catCiudadId = sucursalModel.ciudadId;
                        direc.catEstadoId = sucursalModel.estadoId;
                        direc.entreCalles = sucursalModel.entreCalles;

                        Contexto.Entry(direc).State = EntityState.Modified;

                        sucursales sc = Contexto.sucursales.Find(sucursalModel.idSucursal);
                        sc.nombre = sucursalModel.nombre;
                        sc.telefono = sucursalModel.telefono;
                        sc.latitud = sucursalModel.latitud;
                        sc.longitud = sucursalModel.longitud;
                        sc.placeId = sucursalModel.placeId;
                        sc.colorIndicador = picker;

                        if (sucursalModel.activo == 1)
                        {
                            sc.activo = true;
                        }
                        else
                        {
                            sc.activo = false;
                        }

                        sc.radioZonaMystique = sucursalModel.radioZonaMystique;
                        sc.sucursalPuntoVenta = sucursalModel.sucursalPuntoVenta;
                        sc.usuarioRegistroId = Session.ObtenerUsuario().idUsuario;
                        sc.fechaRegistro = DateTime.Now;

                        sc.catZonaId = sucursalModel.catZonaId;

                        Contexto.Entry(sc).State = EntityState.Modified;

                        #region Sucursal Horario
                        Contexto.sucursalHorarios.RemoveRange(Contexto.sucursalHorarios.Where(c => c.sucursalId == sc.idSucursal));

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = sc,
                            diasNum = sucursalModel.diaLunes,
                            horarioInicio = sucursalModel.HorarioInicioLunes,
                            horarioFin = sucursalModel.HorarioFinLunes,
                            descripcion = sucursalModel.descripcionLunes,
                            dayOfWeek = 1
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = sc,
                            diasNum = sucursalModel.diaMartes,
                            horarioInicio = sucursalModel.HorarioInicioMartes,
                            horarioFin = sucursalModel.HorarioFinMartes,
                            descripcion = sucursalModel.descripcionMartes,
                            dayOfWeek = 2
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = sc,
                            diasNum = sucursalModel.diaMiercoles,
                            horarioInicio = sucursalModel.HorarioInicioMiercoles,
                            horarioFin = sucursalModel.HorarioFinMiercoles,
                            descripcion = sucursalModel.descripcionMiercoles,
                            dayOfWeek = 3
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = sc,
                            diasNum = sucursalModel.diaJueves,
                            horarioInicio = sucursalModel.HorarioInicioJueves,
                            horarioFin = sucursalModel.HorarioFinJueves,
                            descripcion = sucursalModel.descripcionJueves,
                            dayOfWeek = 4
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = sc,
                            diasNum = sucursalModel.diaViernes,
                            horarioInicio = sucursalModel.HorarioInicioViernes,
                            horarioFin = sucursalModel.HorarioFinViernes,
                            descripcion = sucursalModel.descripcionViernes,
                            dayOfWeek = 5
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = sc,
                            diasNum = sucursalModel.diaSabado,
                            horarioInicio = sucursalModel.HorarioInicioSabado,
                            horarioFin = sucursalModel.HorarioFinSabado,
                            descripcion = sucursalModel.descripcionSabado,
                            dayOfWeek = 6
                        });

                        Contexto.sucursalHorarios.Add(new DAL.sucursalHorarios
                        {
                            sucursales = sc,
                            diasNum = sucursalModel.diaDomingo,
                            horarioInicio = sucursalModel.HorarioInicioDomingo,
                            horarioFin = sucursalModel.HorarioFinDomingo,
                            descripcion = sucursalModel.descripcionDomingo,
                            dayOfWeek = 0
                        });
                        #endregion

                        Contexto.SaveChanges();
                        tx.Commit();
                        return RedirectToAction("Index", new { IdComercio = sucursalModel.comercioId });
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException)
                    {
                        tx.Rollback();
                    }
                }
            }

            var Estatus = new EstatusModel[]{ new EstatusModel {Id = 1, Text = "ACTIVO"}, new EstatusModel {Id = 0, Text = "INACTIVO"} };
            sucursalModel.Statuses = new SelectList(Estatus, "Id", "Text", sucursalModel.activo);
            ViewBag.idcomecio = sucursalModel.comercioId;
            //ViewData["activo"] = new SelectList(list, "Value", "Text", activo);
            ViewBag.ciudadId = new SelectList(Contexto.catCiudades, "idCatCiudad", "ciudadDescr", sucursalModel.idDireccion);
            ViewBag.estadoId = new SelectList(Contexto.catEstados, "idCatEstado", "estadoDescr", sucursalModel.idDireccion);
            ViewBag.catRegimenFiscalId = new SelectList(Contexto.catRegimenFiscal, "idCatRegimenFiscal", "descripcion", sucursalModel.catRegimenFiscalId);

            return View(sucursalModel);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            sucursales sucursales = await Contexto.sucursales.FindAsync(id);

            if (sucursales == null)
            {
                return HttpNotFound();
            }
            return View(sucursales);
        }

        // POST: /sucursales/Delete/5
        //[ValidatePermissionsAttribute(true)]
        [HttpPost]
        public async Task<ActionResult> Delete(int idSucursal)
        {
            try
            {
                sucursales sucursales = await Contexto.sucursales.FindAsync(idSucursal);

                Contexto.sucursales.Remove(sucursales);
                await Contexto.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Redirect(Request.Path);
            }
        }

        [HttpPost]
        public void Regresar()
        {
            try
            {
                string rol = Session.ObtenerRol();
                switch (rol)
                {
                    case "Empresa":
                        RedirectToAction("IndexEmpresa", "comercios");
                        break;
                    case "Comercio":
                        RedirectToAction("Index","Home");
                        break;
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                Redirect(Request.Path);
            }
        }

        #region MENU SUCURSAL
        [HttpGet]
        public ActionResult Menu(int id, int idC, int? idF, int? enabled)
        {
            try
            {
                var sucursal = Contexto.sucursales.Where(c => c.idSucursal == id).Select(c => c.nombre).FirstOrDefault();
                ViewBag.NombreSucursal = sucursal;
                ViewBag.sucursal = id;
                ViewBag.comercio = idC;
                ViewBag.Categorias = ObtenerCategoriaProducto(idC, idF);
                ViewBag.familiaId = idF;
                // Obtener los productos
                var productos = Contexto.Productos
                .Include(w => w.CategoriaProductos)
                .Include(w => w.AgrupadorInsumos)
                .Where(w => w.CategoriaProductos != null);
                // Filtrar por familia
                if (idF.HasValue)
                {
                    productos = productos.Where(w => w.categoriaProductoId == idF.Value);
                }
                // Obtener los productos de la sucursal
                var sucursalProductos = Contexto.SucursalProductos
                     .Where(c => c.sucursalId == id)
                     .AsQueryable();
                // Filtrar por estatus
                if (enabled.HasValue)
                {
                    ViewBag.enabled = enabled.Value;
                    switch (enabled)
                    {
                        case 1:
                            sucursalProductos = sucursalProductos.Where(w => w.activo);
                            break;
                        case 0:
                            sucursalProductos = sucursalProductos.Where(w => !w.activo);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ViewBag.enabled = 1;
                    sucursalProductos = sucursalProductos.Where(w => w.activo);
                }
                // Agrupar productos
                var viewmodel = productos.Where(w => sucursalProductos.Select(c => c.productoId).Contains(w.idProducto)).GroupBy(w => w.CategoriaProductos.descripcion).OrderBy(w => w.Key).ToDictionary(w => w.Key);
                // Actualizar precio y estatus por la sucursal solicitada
                foreach (var group in viewmodel)
                {
                    for (var i = 0; i < group.Value.Count(); i++)
                    {
                        var item = group.Value.ElementAt(i);
                        var sucursalProducto = sucursalProductos.Where(c => c.productoId == item.idProducto).FirstOrDefault();
                        if (sucursalProducto != null)
                        {
                            group.Value.ElementAt(i).precio = sucursalProducto.precio;
                            group.Value.ElementAt(i).activo = sucursalProducto.activo;
                        }
                    }
                }
                
                return View(viewmodel);
                #region OLD CODE
                /*return View(SucursalProducto);
                List<SucursalProductos> SucursalProducto = null;

                if (idF != null && idF > 0)
                {
                    SucursalProducto = Contexto.SucursalProductos
                        .Include(s => s.sucursales)
                        .Include(s => s.Productos)
                        .Where(c => c.sucursalId == id && c.Productos.categoriaProductoId == idF)
                        .OrderBy(o => o.Productos.nombre)
                        .ToList();
                }
                else
                {
                    SucursalProducto = Contexto.SucursalProductos
                        .Include(s => s.sucursales)
                        .Include(s => s.Productos)
                        .Where(c => c.sucursalId == id)
                        .OrderBy(o => o.Productos.nombre)
                        .ToList();
                }*/
                #endregion
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                Logger.Error(ex);
                return RedirectToAction("Index", "sucursales");
            }
        }
        

        public ActionResult GuardarNuevoPrecio(int id, string precio)
        {
            try
            {
                var SucursalProductos = Contexto.SucursalProductos.Find(id);
                SucursalProductos.precio = Convert.ToDecimal(precio);
                Contexto.Entry(SucursalProductos).State = EntityState.Modified;
                Contexto.SaveChanges();

                return Json(new Ordenamiento { exito = true });
            }
            catch (Exception e)
            {
                return Json(new Ordenamiento { exito = false });
            }
        }

        [HttpPost]
        public ActionResult ActivarProducto(int productoId, int sucursalId, int IdC, int? IdF)
        {
            try
            {
                //var SucursalProducto = Contexto.SucursalProductos.Find(idSucursalProducto);
                var SucursalProducto = Contexto.SucursalProductos.Where(c => c.sucursalId == sucursalId && c.productoId == productoId).FirstOrDefault();
                if (SucursalProducto != null)
                {
                    SucursalProducto.activo = true;
                    Contexto.Entry(SucursalProducto).State = EntityState.Modified;
                    Contexto.SaveChanges();
                    ShowAlertSuccess("Producto activado exitosamente.");
                }
                else
                {
                    ShowAlertDanger("No se encontro el Producto.");
                }
             
                return RedirectToAction("Menu", "sucursales", new { id = sucursalId, idC = IdC, idF = IdF });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Index", "sucursales");
            }
        }

        [HttpPost]
        public ActionResult InactivarProducto(int productoId, int sucursalId, int IdC, int? IdF)
        {
            try
            {
                //var SucursalProducto = Contexto.SucursalProductos.Find(idSucursalProducto);
                var SucursalProducto = Contexto.SucursalProductos.Where(c => c.sucursalId == sucursalId && c.productoId == productoId).FirstOrDefault();
                if (SucursalProducto != null)
                {
                    SucursalProducto.activo = false;
                    Contexto.Entry(SucursalProducto).State = EntityState.Modified;
                    Contexto.SaveChanges();
                    ShowAlertSuccess("Producto inactivado exitosamente.");
                }
                else
                {
                    ShowAlertDanger("No se encontro el Producto.");
                }
                
                return RedirectToAction("Menu", "sucursales", new { id = sucursalId, idC = IdC, idF = IdF });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Index", "sucursales");
            }
        }

        public ActionResult Generar(int IdS, int IdC)
        {
            try
            {
                var outResultadoParameter = new ObjectParameter("resultado", typeof(string));
                ErrorCodeResponseBase respuesta = new ErrorCodeResponseBase();

                Contexto.SP_PV_Copiar_Productos_Comercio_Sucursal(IdS, IdC, UsuarioActual.idUsuario, outResultadoParameter);

                char delimiter = ';';
                string[] resultados = outResultadoParameter.Value.ToString().Split(delimiter);
                if (resultados[0].Equals("ERROR"))
                {
                    respuesta = RespuestaErrorValidacion(resultados[1]);
                    ShowAlertDanger(respuesta.Message);
                    return RedirectToAction("Menu", "sucursales", new { id = IdS, idC = IdC });
                }
                else
                {
                    ShowAlertSuccess("Se actualizo correctamente el menu.");
                    return RedirectToAction("Menu", "sucursales", new { id = IdS, idC = IdC });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException(e);
                return RedirectToAction("Index", "sucursales");
            }
        }
        #endregion

        #region CONFIGURACION CAJA
        public ActionResult ConfiguracionCaja(decimal MaximoEfectivo, decimal CostoEnvio, decimal Iva, string MensajeTicket, string Imagen, int sucursalId)
        {
            try
            {
                var sucursal = Contexto.sucursales.Find(sucursalId);
                sucursal.maxEfectivo = MaximoEfectivo;
                sucursal.costoEnvio = CostoEnvio;
                sucursal.iva = Iva;
                sucursal.mensajeTicket = MensajeTicket;
                sucursal.logoTickets = Imagen;

                Contexto.Entry(sucursal).State = EntityState.Modified;
                Contexto.SaveChanges();

                return RedirectToAction("Index", "sucursales", null);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Redirect(Request.Path);
            }
        }
        #endregion

        #region DATOS FISCALES
        [HttpGet]
        public async Task<ActionResult> DatosFiscales(int id, int ids)
        {
            try
            {
                var sucursal = Contexto.sucursales
                .Include(c => c.comercios)
                .Include(c => c.comercios.empresas)
                .Include(c => c.datosFiscales)
                .Include(c => c.datosFiscales.direccion)
                .Where(c => c.comercioId == id && c.idSucursal == ids).FirstOrDefault();

                var comercio = Contexto.comercios.Where(c => c.idComercio == id).FirstOrDefault();
                if (sucursal.datosFiscales != null)
                {
                    ViewBag.regimenFiscal = sucursal.datosFiscales.catRegimenFiscalId;
                    ViewBag.estado = sucursal.datosFiscales.direccion.catEstadoId;
                    ViewBag.ciudad = sucursal.datosFiscales.direccion.catCiudadId;

                    ViewBag.FacturacionRegimenFiscal = new SelectList(Contexto.catRegimenFiscal, "idCatRegimenFiscal", "descripcion", sucursal.datosFiscales.catRegimenFiscalId);
                    ViewBag.FacturacionIdEstado = new SelectList(Contexto.catEstados, "idCatEstado", "estadoDescr", sucursal.datosFiscales.direccion.catEstadoId);
                    ViewBag.FacturacionIdCiudad = new SelectList(Contexto.catCiudades, "idCatCiudad", "ciudadDescr", sucursal.datosFiscales.direccion.catCiudadId);

                    var viewModel = new DatosFacturacionViewModel()
                    {
                        IdSucursal = sucursal.idSucursal,
                        EmpresaId = comercio.empresaId,
                        LabelEmpresa = comercio.empresas.nombre,
                        ComercioId = sucursal.comercioId,
                        LabelComercio = comercio.nombreComercial,
                        Nombre = sucursal.nombre,
                        FacturacionCalle = sucursal.datosFiscales.direccion.calle,
                        FacturacionCodigoPostal = sucursal.datosFiscales.cp,
                        FacturacionColonia = sucursal.datosFiscales.direccion.colonia,
                        FacturacionNombre = sucursal.datosFiscales.nombreFiscal,
                        FacturacionRfc = sucursal.datosFiscales.rfc,
                        FacturacionNumero = sucursal.datosFiscales.direccion.numExterior,
                        datosFiscalesId = sucursal.datosFiscales.idDatoFiscal,
                        direccionId = sucursal.datosFiscales.direccionId ?? 0
                    };

                    viewModel.EstatusDatosFiscalesId = (int)EstatusDatosFiscales.Catalogo;

                    ViewBag.EstatusDatosFiscales = ObtenerEstatusDatosFiscales((int)EstatusDatosFiscales.Catalogo);
                    ViewBag.IdDatosFiscales = await ObtenerDatosFiscalesCargados(sucursal.datosFiscalesId);
                    //ViewBag.FacturacionRegimenFiscal = await ObtenerRegimenFacturacion();
                    ViewBag.FacturacionIdEstado = await ObtenerEstados();

                    return View(viewModel);
                }
                else
                {
                    var viewModel = new DatosFacturacionViewModel()
                    {
                        IdSucursal = sucursal.idSucursal,
                        EmpresaId = comercio.empresaId,
                        LabelEmpresa = comercio.empresas.nombre,
                        ComercioId = comercio.idComercio,
                        LabelComercio = comercio.nombreComercial,
                        Nombre = sucursal.nombre,
                        direccionId = 0,
                        datosFiscalesId = 0
                    };

                    viewModel.EstatusDatosFiscalesId = (int)EstatusDatosFiscales.Vacio;
                    ViewBag.EstatusDatosFiscales = ObtenerEstatusDatosFiscales((int)EstatusDatosFiscales.Vacio);
                    ViewBag.IdDatosFiscales = await ObtenerDatosFiscalesCargados();
                    ViewBag.FacturacionRegimenFiscal = await ObtenerRegimenFacturacion();
                    ViewBag.FacturacionIdEstado = await ObtenerEstados();

                    return View(viewModel);
                }               
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                return RedirectToAction("Index", "sucursales");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DatosFiscales(int id, int? FacturacionIdCiudad, DatosFacturacionViewModel viewModel)
        {
            try
            {
                if (id != viewModel.IdSucursal) throw new DataException("id != viewModel.IdSucursal");

                var sucursal = Contexto.sucursales
                    .Include(c => c.comercios)
                    .Include(c => c.comercios.empresas)
                    .Include(c => c.datosFiscales)
                    .Include(c => c.datosFiscales.direccion)
                    .Where(c => c.comercioId == viewModel.ComercioId
                     && c.idSucursal == viewModel.IdSucursal)
                    .FirstOrDefault();

                var comercio = Contexto.comercios.Where(c => c.idComercio == sucursal.comercioId);

                switch ((EstatusDatosFiscales)viewModel.EstatusDatosFiscalesId)
                {
                    case EstatusDatosFiscales.Nuevos:
                        if (!viewModel.FacturacionRegimenFiscal.HasValue) throw new ArgumentNullException(nameof(viewModel.FacturacionRegimenFiscal));
                        using (var tx = Contexto.Database.BeginTransaction())
                        {
                            if (viewModel.datosFiscalesId != 0 && viewModel.direccionId != 0)
                            {
                                var direcciones = Contexto.direccion.Find(viewModel.direccionId);
                                direcciones.calle = viewModel.FacturacionCalle;
                                direcciones.numExterior = viewModel.FacturacionNumero;
                                direcciones.colonia = viewModel.FacturacionColonia;
                                direcciones.catEstadoId = viewModel.FacturacionIdEstado;
                                direcciones.catCiudadId = viewModel.FacturacionIdCiudad;
                                direcciones.codigoPostal = viewModel.FacturacionCodigoPostal;
                                Contexto.Entry(direcciones).State = EntityState.Modified;

                                var dato = Contexto.datosFiscales.Find(viewModel.datosFiscalesId);
                                dato.nombreFiscal = viewModel.FacturacionNombre;
                                dato.cp = viewModel.FacturacionCodigoPostal;
                                dato.rfc = viewModel.FacturacionRfc;
                                dato.serieFactura = string.Empty;
                                dato.fechaRegistro = DateTime.Now;
                                dato.catRegimenFiscalId = viewModel.FacturacionRegimenFiscal.Value;
                                dato.usuarioRegistroId = IdUsuarioActual;
                                Contexto.Entry(dato).State = EntityState.Modified;

                                sucursal.fechaActualizacion = DateTime.Now;
                                sucursal.usuarioActualizoId = IdUsuarioActual;
                                Contexto.Entry(sucursal).State = EntityState.Modified;
                                await Contexto.SaveChangesAsync();
                                tx.Commit();
                            }
                            else
                            {
                            
                                var direccion = Contexto.direccion.Add(new direccion
                                {
                                    calle = viewModel.FacturacionCalle,
                                    numExterior = viewModel.FacturacionNumero,
                                    colonia = viewModel.FacturacionColonia,
                                    catEstadoId = viewModel.FacturacionIdEstado,
                                    catCiudadId = viewModel.FacturacionIdCiudad,
                                    codigoPostal = viewModel.FacturacionCodigoPostal,
                                });

                                var datos = Contexto.datosFiscales.Add(new datosFiscales
                                {
                                    direccion = direccion,
                                    nombreFiscal = viewModel.FacturacionNombre,
                                    cp = viewModel.FacturacionCodigoPostal,
                                    rfc = viewModel.FacturacionRfc,
                                    serieFactura = string.Empty,
                                    fechaRegistro = DateTime.Now,
                                    catRegimenFiscalId = viewModel.FacturacionRegimenFiscal.Value,
                                    usuarioRegistroId = IdUsuarioActual,
                                });

                                

                                sucursal.datosFiscales = datos;
                                sucursal.fechaActualizacion = DateTime.Now;
                                sucursal.usuarioActualizoId = IdUsuarioActual;
                                Contexto.Entry(sucursal).State = EntityState.Modified;

                                Contexto.SaveChanges();
                                tx.Commit();
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return RedirectToAction("Index", "sucursales", new { id = sucursal.comercioId });
            }
            catch (Exception ex)
            {
                ShowAlertException(ex);
                return RedirectToAction("Index", "sucursales");
            }
        }
        #endregion
        #region AJAX

        [HttpPost]
        public async Task<ActionResult> CiudadesPorEstado(int id, int? seleccion = null)
        {
            try
            {
                var data = await Contexto.catCiudades
                    .AsNoTracking()
                    .Where(c => c.catEstadoId == id)
                    .OrderBy(c => c.ciudadDescr)
                    .Select(c => new SelectListItem
                    {
                        Value = c.idCatCiudad.ToString(),
                        Text = c.ciudadDescr,
                        Selected = seleccion == c.idCatCiudad
                    }).ToArrayAsync();

                return Json(new { data, any = data.Any() });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }


        public ActionResult CodigosPostalesSat(string cp)
        {
            try
            {
                var catalogos = HostingEnvironment.ApplicationPhysicalPath + Helpers.Files.FilesHelper.Resources.CatCfdiXsd;
                var isValid = ValidarCatalogosSAT.ValidarCodigoPostal(catalogos, cp);
                if (isValid.HasValue)
                {
                    return isValid.Value ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                else
                {
                    throw new ApplicationException($"ValidarCatalogosSat.ValidarCodigoPostal(\"{catalogos}\", \"{cp}\")");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public ActionResult RfcUnicos(string rfc)
        {
            try
            {
                var existe = Contexto.datosFiscales.Any(c => c.rfc == rfc);
                return existe
                    ? new HttpStatusCodeResult(HttpStatusCode.BadRequest)
                    : new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region HELPERS

        private  IEnumerable<SelectListItem> ObtenerCategoriaProducto(int idComercio, int? seleccionado = null)
        {                       
            return Contexto.CategoriaProductos
                .Where(c => c.comercioId == idComercio)
                .AsNoTracking()
                .OrderBy(c => c.descripcion)
                .Select(c => new SelectListItem
                {
                    Text = c.descripcion,
                    Value = c.idCategoriaProducto.ToString(),
                    Selected = c.idCategoriaProducto == seleccionado
                }).ToArray();
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerEstados(int? seleccionado = null)
        {
            return await Contexto.catEstados
                .AsNoTracking()
                .OrderBy(c => c.estadoDescr)
                .Select(c => new SelectListItem
                {
                    Text = c.estadoDescr,
                    Value = c.idCatEstado.ToString(),
                    Selected = c.idCatEstado == seleccionado
                }).ToArrayAsync();
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerRegimenFacturacion(int? seleccionado = null)
        {
            return await Contexto.catRegimenFiscal
                .AsNoTracking()
                .OrderBy(d => d.idCatRegimenFiscal)
                .Select(c => new SelectListItem
                {
                    Text = c.descripcion,
                    Value = c.idCatRegimenFiscal.ToString(),
                    Selected = c.idCatRegimenFiscal == seleccionado
                }).ToArrayAsync();
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerDatosFiscalesCargados(int? seleccionado = null)
        {
            return await Contexto.datosFiscales
                .AsNoTracking()
                .Where(c => SucursalesActuales.Any(d => d.datosFiscalesId == c.idDatoFiscal))
                .OrderByDescending(d => d.fechaRegistro)
                .Select(c => new SelectListItem
                {
                    Text = c.nombreFiscal + " (" + c.rfc + ")",
                    Value = c.idDatoFiscal.ToString(),
                    Selected = c.idDatoFiscal == seleccionado
                }).ToArrayAsync();
        }

        private IEnumerable<SelectListItem> ObtenerEstatusDatosFiscales(int? seleccionado = null)
        {
            return new[]
            {
                new SelectListItem
                {
                    Selected = (int?) EstatusDatosFiscales.Vacio == seleccionado,
                    Value = $"{(int) EstatusDatosFiscales.Vacio}",
                    Text = "Sin información de datos fiscales"
                },
                new SelectListItem
                {
                    Selected = (int?) EstatusDatosFiscales.Catalogo == seleccionado,
                    Value = $"{(int) EstatusDatosFiscales.Catalogo}",
                    Text = "Selecciónar datos cargados previamente"
                },
                new SelectListItem
                {
                    Selected = (int?) EstatusDatosFiscales.Nuevos == seleccionado,
                    Value = $"{(int) EstatusDatosFiscales.Nuevos}",
                    Text = "Cargar nuevos datos fiscales"
                },
            };
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Contexto.Dispose();
            }
            base.Dispose(disposing);
        }

        protected static ErrorCodeResponseBase RespuestaErrorValidacion(string errors) => new ErrorCodeResponseBase
        {
            Message = errors,
            ResponseCode = (int)ResponseTypes.CodigoValidacion,
        };
        #endregion
    }
}