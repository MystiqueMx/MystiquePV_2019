using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class LoginController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
        readonly string MENSAJE_USUARIO_NO_REGISTRADO = "MYSTIQUE_MENSAJE_USUARIO_NO_REGISTRADO";
        readonly string MENSAJE_USUARIO_BLOQUEADO = "MYSTIQUE_MENSAJE_USUARIO_BLOQUEADO";
        readonly string MENSAJE_OTRO_METODO_AUTENTIFICACION = "MYSTIQUE_MENSAJE_OTRO_METODO_AUTENTIFICACION";

        [Route("api/iniciarSesion")]
        public ResponseLogin iniciarSesion([FromBody]RequestLogin entradas)
        {
            ResponseLogin respuesta = new ResponseLogin();

            try
            {
                clientes verificar = contextEntity.clientes
                    .Include(c => c.catRangoEdad)
                    .FirstOrDefault(w => w.email == entradas.correoElectronico
                        && w.password == entradas.contrasenia
                        && entradas.Empresas.Contains(w.empresaId));

                if (verificar != null)
                {
                    if (entradas.tipoAutentificacionId == verificar.catTipoAutenficacionId)
                    {
                        if (verificar.estatus)
                        {
                            login loginDatos = contextEntity.login.Where(w => w.clienteId == verificar.idCliente && w.playerId == entradas.playerId).FirstOrDefault();
                            if (loginDatos != null)
                            {
                                loginDatos.sesionActiva = true;
                                loginDatos.fechaActualizacion = DateTime.Now;
                            }
                            else
                            {
                                login insertarLogin = new login();
                                insertarLogin.clienteId = verificar.idCliente;
                                insertarLogin.fechaRegistro = DateTime.Now;
                                insertarLogin.fechaActualizacion = DateTime.Now;
                                insertarLogin.sesionActiva = true;
                                insertarLogin.sesionToken = "";
                                insertarLogin.deviceId = entradas.deviceId;
                                insertarLogin.deviceModel = entradas.deviceModel;
                                insertarLogin.devicePlatform = entradas.devicePlatform;
                                insertarLogin.deviceVersion = entradas.deviceVersion;
                                insertarLogin.deviceConnectionType = entradas.deviceConnectionType;
                                insertarLogin.activo = true;
                                insertarLogin.isCliente = true;
                                insertarLogin.playerId = entradas.playerId;

                                contextEntity.login.Add(insertarLogin);
                            }

                            if ((contextEntity.SaveChanges()) <= 0)
                            {
                                respuesta.Success = false;
                                respuesta.ErrorMessage = "Problemas al autentificar";
                            }
                            else
                            {
                                respuesta.Success = true;
                                respuesta.ErrorMessage = "";
                                membresias membresia = verificar.membresias.FirstOrDefault();

                                respuesta.clienteId = verificar.idCliente;
                                respuesta.email = verificar.email;
                                respuesta.nombre = verificar.nombre;
                                respuesta.paterno = verificar.paterno;
                                respuesta.materno = verificar.materno;
                                respuesta.telefono = verificar.telefono;
                                respuesta.sexo = verificar.catSexoId != null ? verificar.catSexoId : 0;
                                respuesta.catColoniaId = verificar.catColoniaId != null ? verificar.catColoniaId : 0;
                                respuesta.catCiudadId = verificar.catColoniaId != null ? verificar.catColonias.catCiudadId : 0;
                                respuesta.colonia = verificar.catColoniaId != null ? verificar.catColonias.descripcion : "";
                                respuesta.fechaNacimiento = verificar.fechaNacimiento;
                                respuesta.membresiaId = membresia.idMembresia;
                                respuesta.folioMembresia = membresia.folio;
                                respuesta.fechaFinMembresia = membresia.fechaFin;
                                respuesta.folioGuidMembresia = membresia.guid;
                                respuesta.urlFotoPerfil = verificar.urlFotoPerfil;
                                respuesta.fechaCargaFoto = verificar.fechaCargaFoto;
                                respuesta.correoElectronico = verificar.email;
                                respuesta.contrasenia = verificar.password;
                                respuesta.facebookId = verificar.facebookId;
                                respuesta.tipoAutentificacionId = verificar.catTipoAutenficacionId;
                                respuesta.registroCompleto = validar.validarUsuarioDatosRequeridos(verificar);

                                if (verificar.catRangoEdadId.HasValue)
                                {
                                    respuesta.rangoEdadId = verificar.catRangoEdad.idCatRangoEdad;
                                    respuesta.rangoEdadDescripcion = verificar.catRangoEdad.etiqueta;
                                }
                                if (verificar.catAseguranzaId.HasValue)
                                {
                                    respuesta.catAseguranzaId = verificar.catAseguranzaId;
                                }
                            }
                        }
                        else
                        {
                            respuesta.Success = false;
                            respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_BLOQUEADO);
                        }
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = string.Format(validar.ObtenerMensajeRespuesta(MENSAJE_OTRO_METODO_AUTENTIFICACION), verificar.catTipoAutentificacion.descripcion);
                    }
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_NO_REGISTRADO);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }


        [Route("api/autentificarse")]
        public ResponseLogin autentificarse([FromBody]RequestLogin entradas)
        {
            ResponseLogin respuesta = new ResponseLogin();
            try
            {
                clientes verificar = contextEntity.clientes
                    .Include(c => c.catRangoEdad)
                    .FirstOrDefault(w => w.email == entradas.correoElectronico
                        && w.password == entradas.contrasenia
                        && w.empresaId == entradas.empresaId);

                if (verificar != null)
                {
                    if (verificar.estatus)
                    {
                        login loginDatos = contextEntity.login.Where(w => w.clienteId == verificar.idCliente && w.playerId == entradas.playerId).FirstOrDefault();
                        if (loginDatos != null)
                        {
                            loginDatos.sesionActiva = true;
                            loginDatos.fechaActualizacion = DateTime.Now;
                        }
                        else
                        {
                            login insertarLogin = new login();
                            insertarLogin.clienteId = verificar.idCliente;
                            insertarLogin.fechaRegistro = DateTime.Now;
                            insertarLogin.fechaActualizacion = DateTime.Now;
                            insertarLogin.sesionActiva = true;
                            insertarLogin.sesionToken = "";
                            insertarLogin.deviceId = entradas.deviceId;
                            insertarLogin.deviceModel = entradas.deviceModel;
                            insertarLogin.devicePlatform = entradas.devicePlatform;
                            insertarLogin.deviceVersion = entradas.deviceVersion;
                            insertarLogin.deviceConnectionType = entradas.deviceConnectionType;
                            insertarLogin.activo = true;
                            insertarLogin.isCliente = true;
                            insertarLogin.playerId = entradas.playerId;

                            contextEntity.login.Add(insertarLogin);
                        }

                        if ((contextEntity.SaveChanges()) <= 0)
                        {
                            respuesta.Success = false;
                            respuesta.ErrorMessage = "Problemas al autentificar";
                        }
                        else
                        {
                            respuesta.Success = true;
                            respuesta.ErrorMessage = "";
                            membresias membresia = verificar.membresias.FirstOrDefault();

                            respuesta.clienteId = verificar.idCliente;
                            respuesta.email = verificar.email;
                            respuesta.nombre = verificar.nombre;
                            respuesta.paterno = verificar.paterno;
                            respuesta.materno = verificar.materno;
                            respuesta.telefono = verificar.telefono;
                            respuesta.sexo = verificar.catSexoId != null ? verificar.catSexoId : 0;
                            respuesta.catColoniaId = verificar.catColoniaId != null ? verificar.catColoniaId : 0;
                            respuesta.catCiudadId = verificar.catColoniaId != null ? verificar.catColonias.catCiudadId : 0;
                            respuesta.colonia = verificar.catColoniaId != null ? verificar.catColonias.descripcion : "";
                            respuesta.fechaNacimiento = verificar.fechaNacimiento;
                            respuesta.membresiaId = membresia.idMembresia;
                            respuesta.folioMembresia = membresia.folio;
                            respuesta.fechaFinMembresia = membresia.fechaFin;
                            respuesta.folioGuidMembresia = membresia.guid;
                            respuesta.urlFotoPerfil = verificar.urlFotoPerfil;
                            respuesta.fechaCargaFoto = verificar.fechaCargaFoto;
                            respuesta.correoElectronico = verificar.email;
                            respuesta.contrasenia = verificar.password;
                            respuesta.facebookId = verificar.facebookId;
                            respuesta.registroCompleto = validar.validarUsuarioDatosRequeridos(verificar);

                            if (verificar.catRangoEdadId.HasValue)
                            {
                                respuesta.rangoEdadId = verificar.catRangoEdad.idCatRangoEdad;
                                respuesta.rangoEdadDescripcion = verificar.catRangoEdad.etiqueta;
                            }
                            if (verificar.catAseguranzaId.HasValue)
                            {
                                respuesta.catAseguranzaId = verificar.catAseguranzaId;
                            }
                        }
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_BLOQUEADO);
                    }
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_NO_REGISTRADO);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }

        [Route("api/autentificarseDoctor")]
        public ResponseLoginDoctor autentificarseDoctor([FromBody]RequestLogin entradas)
        {
            ResponseLoginDoctor respuesta = new ResponseLoginDoctor();
            try
            {
                var verificar = contextEntity.comercios
                    .Include(i => i.AnexoDoctor)
                    .FirstOrDefault(w => w.AnexoDoctor.userName == entradas.correoElectronico
                        && w.AnexoDoctor.password == entradas.contrasenia);

                if (verificar != null)
                {
                    if (verificar.estatus)
                    {
                        var anexoDoctor = verificar.AnexoDoctor.playerId = entradas.playerId;
                        contextEntity.SaveChanges();
                        //login loginDatos = contextEntity.login.Where(w => w.clienteId == verificar.anexoDoctorId.Value && w.playerId == entradas.playerId).FirstOrDefault();
                        //if (loginDatos != null)
                        //{
                        //    loginDatos.sesionActiva = true;
                        //    loginDatos.fechaActualizacion = DateTime.Now;
                        //}
                        //else
                        //{
                        //    login insertarLogin = new login();
                        //    insertarLogin.clienteId = verificar.AnexoDoctor.idAnexoDoctor;
                        //    insertarLogin.fechaRegistro = DateTime.Now;
                        //    insertarLogin.fechaActualizacion = DateTime.Now;
                        //    insertarLogin.sesionActiva = true;
                        //    insertarLogin.sesionToken = "";
                        //    insertarLogin.deviceId = entradas.deviceId;
                        //    insertarLogin.deviceModel = entradas.deviceModel;
                        //    insertarLogin.devicePlatform = entradas.devicePlatform;
                        //    insertarLogin.deviceVersion = entradas.deviceVersion;
                        //    insertarLogin.deviceConnectionType = entradas.deviceConnectionType;
                        //    insertarLogin.activo = true;
                        //    insertarLogin.isCliente = true;
                        //    insertarLogin.playerId = entradas.playerId;

                        //    contextEntity.login.Add(insertarLogin);
                        //}

                        //if ((contextEntity.SaveChanges()) <= 0)
                        //{
                        //    respuesta.Success = false;
                        //    respuesta.ErrorMessage = "Problemas al autentificar";
                        //}
                        //else
                        //{
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "";
                        respuesta.doctorId = verificar.idComercio;
                        respuesta.anexoDoctorId = verificar.anexoDoctorId.Value;
                        respuesta.email = verificar.correo;
                        respuesta.userName = verificar.AnexoDoctor.userName;
                        respuesta.nombre = verificar.nombreComercial;
                        respuesta.telefono = verificar.telefono;
                        respuesta.paginaWeb = verificar.paginaWeb;
                        respuesta.direccion = verificar.direccion;
                        respuesta.cedula = verificar.AnexoDoctor.cedula;
                        respuesta.urlFotoPerfil = verificar.logoUrl;
                        respuesta.catTipoComercioId = verificar.catTipoComercioId;
                        respuesta.catComercioGiroId = verificar.catComercioGiroId;
                        respuesta.fechaRegistro = verificar.fechaRegistro;
                        respuesta.correoElectronico = verificar.correo;
                        respuesta.contrasenia = verificar.AnexoDoctor.password;
                        respuesta.doctorEspecialidadId = verificar.AnexoDoctor.doctorEspecialidadId;
                        //}
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_BLOQUEADO);
                    }
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_NO_REGISTRADO);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }
        [Route("api/autentificarseFB")]
        public ResponseLogin autentificarseFB([FromBody]RequestLoginFB entradas)
        {
            ResponseLogin respuesta = new ResponseLogin();
            try
            {
                clientes verificar = contextEntity.clientes.FirstOrDefault(w => w.email == entradas.correoElectronico && w.facebookId == entradas.facebookId && w.empresaId == entradas.empresaId);

                if (verificar != null)
                {
                    if (verificar.estatus)
                    {

                        login loginDatos = contextEntity.login.Where(w => w.clienteId == verificar.idCliente && w.playerId == entradas.playerId).FirstOrDefault();
                        if (loginDatos != null)
                        {
                            loginDatos.sesionActiva = true;
                            loginDatos.fechaActualizacion = DateTime.Now;
                        }
                        else
                        {
                            login insertarLogin = new login();
                            insertarLogin.clienteId = verificar.idCliente;
                            insertarLogin.fechaRegistro = DateTime.Now;
                            insertarLogin.fechaActualizacion = DateTime.Now;
                            insertarLogin.sesionActiva = true;
                            insertarLogin.sesionToken = "";
                            insertarLogin.deviceId = entradas.deviceId;
                            insertarLogin.deviceModel = entradas.deviceModel;
                            insertarLogin.devicePlatform = entradas.devicePlatform;
                            insertarLogin.deviceVersion = entradas.deviceVersion;
                            insertarLogin.deviceConnectionType = entradas.deviceConnectionType;
                            insertarLogin.activo = true;
                            insertarLogin.isCliente = true;
                            insertarLogin.playerId = entradas.playerId;

                            contextEntity.login.Add(insertarLogin);
                        }
                        if ((contextEntity.SaveChanges()) <= 0)
                        {
                            respuesta.Success = false;
                            respuesta.ErrorMessage = "Problemas al autentificar";
                        }
                        else
                        {
                            respuesta.Success = true;
                            respuesta.ErrorMessage = "";
                            membresias membresia = verificar.membresias.FirstOrDefault();

                            respuesta.clienteId = verificar.idCliente;
                            respuesta.email = verificar.email;
                            respuesta.nombre = verificar.nombre;
                            respuesta.paterno = verificar.paterno;
                            respuesta.materno = verificar.materno;
                            respuesta.telefono = verificar.telefono;
                            respuesta.sexo = verificar.catSexoId != null ? verificar.catSexoId : 0;
                            respuesta.catColoniaId = verificar.catColoniaId != null ? verificar.catColoniaId : 0;
                            respuesta.catCiudadId = verificar.catColonias != null ? verificar.catColonias.catCiudadId : 0;
                            respuesta.colonia = verificar.catColoniaId != null ? verificar.catColonias.descripcion : "";
                            respuesta.fechaNacimiento = verificar.fechaNacimiento;
                            respuesta.membresiaId = membresia.idMembresia;
                            respuesta.folioMembresia = membresia.folio;
                            respuesta.fechaFinMembresia = membresia.fechaFin;
                            respuesta.folioGuidMembresia = membresia.guid;
                            respuesta.urlFotoPerfil = verificar.urlFotoPerfil;
                            respuesta.fechaCargaFoto = verificar.fechaCargaFoto;
                            respuesta.correoElectronico = verificar.email;
                            respuesta.contrasenia = verificar.password;
                            respuesta.facebookId = verificar.facebookId;
                            respuesta.registroCompleto = validar.validarUsuarioDatosRequeridos(verificar);
                            if (verificar.catAseguranzaId.HasValue)
                            {
                                respuesta.catAseguranzaId = verificar.catAseguranzaId;
                            }
                        }
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_BLOQUEADO);
                    }
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_NO_REGISTRADO);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }


        [Route("api/logout")]
        public ResponseLogout logout([FromBody]RequestLogout entradas)
        {
            ResponseLogout respuesta = new ResponseLogout();
            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    var verificar = contextEntity.clientes.Where(w => w.userName == entradas.correoElectronico && w.password == entradas.contrasenia && entradas.Empresas.Contains(w.empresaId)).FirstOrDefault();

                    if (verificar != null)
                    {
                        login loginDatos = contextEntity.login.Where(w => w.clienteId == verificar.idCliente && w.playerId == entradas.playerId && w.sesionActiva == true).FirstOrDefault();
                        if (loginDatos != null)
                        {
                            loginDatos.sesionActiva = false;

                            if ((contextEntity.SaveChanges()) <= 0)
                            {
                                respuesta.Success = false;
                                respuesta.ErrorMessage = "Problemas al cerrar sesión";
                            }
                            else
                            {
                                respuesta.Success = true;
                                respuesta.ErrorMessage = "Logout correcto";
                            }
                        }
                        else
                        {
                            respuesta.Success = false;
                            respuesta.ErrorMessage = "No estaba activo";
                        }
                    }
                    else
                    {
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "No existe cliente";
                    }
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return respuesta;
        }
    }
}