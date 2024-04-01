
using MystiqueMC.DAL;
using MystiqueMC.Helpers.Emails;
using ApiDoc.Models;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class CitasController : BaseApiController
    {
        private const int USUARIO_NOTIFICACION = 1;
       // private readonly string _hostImagenes = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");
        #region POST
        [HttpPost]
        [Route("api/citas")]
        public CitasResponse CitasCliente(RequestMisCitas viewmodel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new CitasResponse
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }

                var citas = Contexto.Citas
                   .Include(c => c.sucursales)
                   .Include(c => c.sucursales.direccion)
                   .Where(c => c.fechaCita > DateTime.Now
                       && viewmodel.Empresas.Contains(c.sucursales.comercios.empresaId)
                       && c.clienteId == viewmodel.Cliente)
                    .OrderBy(c => c.fechaCita)
                   .ToArray()
                   .Select(c => new CitaCliente
                   {
                       Doctor = c.sucursales.nombre,
                       Estatus = c.estatus,
                       Fecha = c.fechaCita,
                       Id = c.idCita,
                       Direccion = ConvertDireccionToString(c.sucursales.direccion),
                       Observacion = c.observaciones,
                       TelefonoContacto = c.sucursales.telefono,
                   });

                return new CitasResponse
                {
                    Success = true,
                    Data = citas
                };
            }
            catch (System.Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new CitasResponse
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }
        [HttpPost]
        [Route("api/citas/doctor")]
        public CitasDoctorResponse CitasDoctor(RequestMisCitasDoctor viewmodel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new CitasDoctorResponse
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }
                var comercios = Contexto.comercios
                    .Where(w => w.anexoDoctorId == viewmodel.Doctor).AsQueryable();

                var sucursales = Contexto.sucursales
                    .Where(c => comercios
                    .Select(s => s.idComercio).Contains(c.comercioId));

                var allCitas = Contexto.Citas
                    .Where(c => sucursales
                    .Select(s => s.idSucursal).Contains(c.sucursalId)
                        && c.estatus == (int)viewmodel.estatus);

                var citas = allCitas
                   .Include(c => c.clientes)
                   .Include(c => c.sucursales)
                   .Where(c => viewmodel.Empresas.Contains(c.sucursales.comercios.empresaId))
                   .OrderBy(c => c.fechaCita)
                   .ToArray()
                   .Select(c => new CitaDoctor
                   {
                       Cliente = $"{c.clientes.nombre} {c.clientes.paterno}",
                       FotoCliente = ObtenerUrlDomainImagen(c.clientes.urlFotoPerfil),
                       Email = c.clientes.email,
                       Estatus = c.estatus,
                       Fecha = c.fechaCita,
                       Id = c.idCita,
                       RangoEdad = c.clientes.catRangoEdad?.etiqueta,
                       Sexo = c.clientes.catSexoId,
                       Telefono = c.telefonoContacto,
                   });

                if (viewmodel.fechaFin.HasValue)
                {

                    citas = allCitas
                   .Include(c => c.clientes)
                   .Where(c => DbFunctions.TruncateTime(c.fechaCita) >= viewmodel.fechaInicio.Value
                       && DbFunctions.TruncateTime(c.fechaCita) <= viewmodel.fechaFin.Value
                       && viewmodel.Empresas.Contains(c.sucursales.comercios.empresaId))
                   .OrderBy(c => c.fechaCita)
                   .ToArray()
                   .Select(c => new CitaDoctor
                   {
                       Cliente = $"{c.clientes.nombre} {c.clientes.paterno}",
                       Email = c.clientes.email,
                       FotoCliente = ObtenerUrlDomainImagen(c.clientes.urlFotoPerfil),
                       Estatus = c.estatus,
                       Fecha = c.fechaCita,
                       Id = c.idCita,
                       RangoEdad = c.clientes.catRangoEdad?.etiqueta,
                       Sexo = c.clientes.catSexoId,
                       Telefono = c.telefonoContacto,
                   });
                }


                return new CitasDoctorResponse
                {
                    Success = true,
                    Data = citas
                };
            }
            catch (System.Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new CitasDoctorResponse
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }
        [HttpPost]
        [Route("api/citas/solicitar")]
        public ResponseBase SolicitarCita(RequestCita viewmodel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new ResponseBase
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }

                if (!Contexto.sucursales.Any(c => c.idSucursal == viewmodel.Doctor
                     && viewmodel.Empresas.Contains(c.comercios.empresaId)
                     && c.comercios.catComercioGiroId == (int)TipoSucursales.Doctores))
                {
                    return new ResponseBase
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }
                
                var zonas = Contexto.Citas.Add(new MystiqueMC.DAL.Citas
                {
                    clienteId = viewmodel.Cliente,
                    estatus = (int)EstatusCitas.Solicitud,
                    fechaCita = viewmodel.Fecha,
                    observaciones = string.Empty,
                    playerIdCliente = viewmodel.PlayerId,
                    telefonoContacto = viewmodel.Telefono,
                    sucursalId = viewmodel.Doctor,
                    fechaRegistro = DateTime.Now,
                    fechaUltimaActualizacion = DateTime.Now,
                });
                Contexto.SaveChanges();
                // ENVIAR SOLICITUD DE CITA A APP DOCTOR 
                var doctor = Contexto.sucursales
                    .Where(w => w.idSucursal == viewmodel.Doctor)
                    .Select(s => s.comercios)?.FirstOrDefault();
                var playerIds = Contexto.AnexoDoctor
                    .Where(w => w.idAnexoDoctor == doctor.anexoDoctorId)?.Select(s => s.playerId).ToArray();
                var cliente = Contexto.clientes
                    .Where(w => w.idCliente == viewmodel.Cliente)?.FirstOrDefault();

                SendNotificationDelegate @delegate = new SendNotificationDelegate();

                var culture = new CultureInfo("es");
                @delegate.EnviarNuevaSolicitudDoctor(playerIds, "Tiene una nueva solicitud de cita", $"{cliente.nombre} {cliente.paterno}. {cliente.telefono}, Fecha cita: {viewmodel.Fecha.ToString("dd MMMM yyyy, hh:mm tt", culture)}");

                return new ResponseBase
                {
                    Success = true,
                };
            }
            catch (System.Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new ResponseBase
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }

        [HttpPost]
        [Route("api/citas/actualizarDoctor")]
        public ResponseBase ActualizarCita(RequestDoctorCita viewmodel)
        {
            try
            {
                Logger.Error("VIEWMODEL:   " + viewmodel);
                if (!Validador.IsAppSecretValid)
                {
                    return new ResponseBase
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }


                //ACTUALIZAR DATOS DE LA CITA
                var cita = Contexto.Citas
                .Include(c => c.clientes)
                .First(c => c.idCita == viewmodel.Cita
                && viewmodel.Empresas.Contains(c.sucursales.comercios.empresaId));
                cita.fechaUltimaActualizacion = DateTime.Now;
                cita.fechaCita = viewmodel.Fecha;
                cita.observaciones = viewmodel.Observacion;
                Contexto.SaveChanges();


                return new ResponseBase
                {
                    Success = true,
                };
            }
            catch (Exception e)
            {
                Logger.Error("ERROR ACTUALIZAR DOCTOR ...:" + e.Message + "\n" + e.InnerException + e);
                return new ResponseBase
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }
        [HttpPost]
        [Route("api/citas/actualizarEstatusCita")]
        public ResponseBase ActualizarEstatusCita(RequestDoctorCita viewmodel)
        {
            try
            {
                if (!Validador.IsAppSecretValid)
                {
                    return new ResponseBase
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }

                var cita = Contexto.Citas?.First(c => c.idCita == viewmodel.Cita
                     && viewmodel.Empresas.Contains(c.sucursales.comercios.empresaId));

                cita.estatus = (int)viewmodel.estatus;

                Contexto.SaveChanges();

                var doctor = Contexto.sucursales
                    .Where(w => w.idSucursal == cita.sucursalId)?.FirstOrDefault();

                var culture = new CultureInfo("es");
                //ACEPTAR SOLICITUD (notificacion)
                string[] ids = new string[] { cita.playerIdCliente };
                if (viewmodel.estatus == EstatusCitas.Aprobada)
                {
                    //envio notificacion
                    string titulo = "La solicitud de su cita ha sido aprobada";
                    string contenido = $"{doctor.nombre}. {doctor.telefono}, Fecha cita: {cita.fechaCita.ToString("dd MMMM yyyy, hh:mm tt", culture)}";
                    SendNotificationDelegate @delegate = new SendNotificationDelegate();

                    @delegate.EnviarRespuestaSolicitudCliente(ids, titulo, contenido);

                    var notificacion = new notificaciones
                    {
                        activo = true,
                        fechaRegistro = DateTime.Now,
                        descripcion = contenido,
                        titulo = titulo,
                        usuarioRegistro = USUARIO_NOTIFICACION,
                        isBeneficio = false,
                        descripcionIngles = "-",
                        tipoNotificacion = 1,
                        empresaId = cita.clientes.empresaId,
                    };
                    Contexto.notificaciones.Add(notificacion);

                    Contexto.clienteNotificaciones.Add(new clienteNotificaciones
                    {
                        notificaciones = notificacion,
                        clienteId = cita.clienteId,
                        fechaEnviado = notificacion.fechaRegistro,
                        revisado = false,
                        empresaId = notificacion.empresaId,
                    });
                }

                //DENEGAR SOLICITUD (notificacion)
                if (viewmodel.estatus == EstatusCitas.Denegada)
                {
                    //envio notificacion
                    string titulo = "La solicitud de su cita ha sido rechazada";
                    string contenido = $"Por el momento el doctor {doctor.nombre} no puede atenderlo(a) en la hora y fecha solicitada. Fecha cita: {cita.fechaCita.ToString("dd MMMM yyyy, hh:mm tt", culture)}";
                    SendNotificationDelegate @delegate = new SendNotificationDelegate();

                    @delegate.EnviarRespuestaSolicitudCliente(ids, titulo, contenido);

                    var notificacion = new notificaciones
                    {
                        activo = true,
                        fechaRegistro = DateTime.Now,
                        descripcion = contenido,
                        titulo = titulo,
                        usuarioRegistro = USUARIO_NOTIFICACION,
                        isBeneficio = false,
                        empresaId = cita.clientes.empresaId,
                        descripcionIngles = "-",
                        tipoNotificacion = 1
                    };
                    Contexto.notificaciones.Add(notificacion);

                    Contexto.clienteNotificaciones.Add(new clienteNotificaciones
                    {
                        notificaciones = notificacion,
                        clienteId = cita.clienteId,
                        fechaEnviado = notificacion.fechaRegistro,
                        revisado = false,
                        empresaId = notificacion.empresaId,
                    });
                }

                Contexto.SaveChanges();
                return new ResponseBase
                {
                    Success = true,
                };
            }
            catch (System.Exception e)
            {
                Logger.Error("ERROR:" + e.Message);
                return new ResponseBase
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }

        [HttpPost]
        [Route("api/citas/terminarCita")]
        public ResponseBase TerminarCita(RequestDoctorCita viewmodel)
        {
            try
            {
                Logger.Error("VIEWMODEL:   " + viewmodel);
                if (!Validador.IsAppSecretValid)
                {
                    return new ResponseBase
                    {
                        Success = false,
                        ErrorMessage = MensajeNoPermisos
                    };
                }

                //TERMINAR LA CITA
                var cita = Contexto.Citas
                .Include(c => c.clientes)
                .First(c => c.idCita == viewmodel.Cita
                //&& viewmodel.Empresas.Contains(c.sucursales.comercios.empresaId)
                );

                cita.fechaUltimaActualizacion = DateTime.Now;
                cita.estatus = (int)viewmodel.estatus;
                cita.observaciones = viewmodel.Observacion;
                cita.IsBeneficioEscaneado = viewmodel.IsBeneficioEscaneado;
                cita.cadenaCodigo = viewmodel.CadenaCodigo;

                Contexto.SaveChanges();

                return new ResponseBase
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                Logger.Error("ERROR TERMINAR CITA...:" + e.Message + "\n" + e.InnerException + e);
                return new ResponseBase
                {
                    Success = false,
                    ErrorMessage = MensajeErrorServidor
                };
            }
        }
        #endregion
        #region HELPERS
        public string ConvertDireccionToString(MystiqueMC.DAL.direccion item)
        {
            if (item == null)
            {
                return "-";
            }
            else
            {
                var dir = item.calle + " " + item.numExterior;
                if (!string.IsNullOrEmpty(item.numInterior))
                {
                    dir += " " + item.numInterior;
                }
                if (!string.IsNullOrEmpty(item.colonia))
                {
                    dir += " " + item.colonia;
                }
                if (!string.IsNullOrEmpty(item.codigoPostal))
                {
                    dir += " " + item.codigoPostal;
                }
                if (item.catCiudades != null)
                {
                    dir += " " + item.catCiudades.ciudadDescr;
                }
                if (item.catEstados != null)
                {
                    dir += " " + item.catEstados.estadoDescr;
                }
                return dir;
            }
        }
        #endregion
    }
}