using MystiqueMC.DAL;
using MystiqueMcApi.Helpers;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class UsuarioController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
        readonly string MENSAJE_USUARIO_YA_EXISTE = "MYSTIQUE_MENSAJE_USUARIO_YA_EXISTE";
        readonly string MENSAJE_OTRO_METODO_AUTENTIFICACION = "MYSTIQUE_MENSAJE_OTRO_METODO_AUTENTIFICACION";

        [AllowAnonymous]
        [Route("api/registrarUsuario")]
        public async Task<ResponseBase> registrarUsuario(RequestUsuarioRegistrar model)
        {
            ResponseBase respuesta = new ResponseBase();

            try
            {
                if (validar.IsAppSecretValid)
                {
                    var verificar = contextEntity.clientes.Where(w => w.email == model.email && w.empresaId == model.empresaId).AsNoTracking().FirstOrDefault();

                    if (verificar == null)
                    {
                        clientes cliente = new clientes();
                        cliente.nombre = model.nombre;
                        cliente.paterno = model.paterno;
                        cliente.materno = model.materno;
                        cliente.email = model.email;
                        cliente.telefono = model.telefono;
                        cliente.fechaNacimiento = model.fechaNacimiento != null ? model.fechaNacimiento : null;
                        cliente.userName = model.userName;
                        cliente.password = model.password;
                        cliente.estatus = true;
                        cliente.fechaRegistro = DateTime.Now;
                        cliente.catSexoId = model.sexo > 0 ? model.sexo : null;
                        cliente.catColoniaId = model.coloniaId > 0 ? model.coloniaId : null;
                        cliente.facebookId = model.facebookId;
                        cliente.empresaId = model.empresaId;
                        cliente.catTipoAutenficacionId = model.tipoAutentificacionId;
                        contextEntity.clientes.Add(cliente);

                        if (( contextEntity.SaveChanges()) <= 0)
                        {
                            respuesta.Success = false;
                            respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
                        }
                        else
                        {
                            if (generarMembresia(cliente.idCliente))
                            {
                                respuesta.Success = true;
                                respuesta.ErrorMessage = "";
                                respuesta.correoElectronico = cliente.email;
                                respuesta.contrasenia = cliente.password;
                            }
                        }
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_USUARIO_YA_EXISTE);

                        if (model.tipoAutentificacionId != verificar.catTipoAutenficacionId)
                        {
                            respuesta.ErrorMessage = string.Format(validar.ObtenerMensajeRespuesta(MENSAJE_OTRO_METODO_AUTENTIFICACION), verificar.catTipoAutentificacion.descripcion);
                        }
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

        public bool generarMembresia(int clienteId)
        {
            var guidMembresia = Guid.NewGuid().ToString();
            bool respuesta = true;
            try
            {
                membresias agregarMembresia = new membresias();
                agregarMembresia.clienteId = clienteId;
                agregarMembresia.guid = guidMembresia;
                agregarMembresia.catTipoMembresiaId = 1;
                agregarMembresia.folio = "00000";
                agregarMembresia.fechaInicio = DateTime.Now;
                //agregarMembresia.fechaFin = DateTime.Now.AddYears(1);
                agregarMembresia.estatus = true;
                agregarMembresia.fechaVinculo = DateTime.Now;
                contextEntity.membresias.Add(agregarMembresia);
                if ((contextEntity.SaveChanges()) <= 0)
                {
                    respuesta = false;
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                respuesta = false;
            }
            return respuesta;
        }


        [Route("api/actualizarUsuario")]
        public async Task<ResponseLogin> actualizarUsuario([FromBody]RequestCliente entradas)
        {
            ResponseLogin respuesta = new ResponseLogin();

            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {

                    var clientes = contextEntity.clientes.Where(w => w.idCliente == entradas.clienteId).FirstOrDefault();

                    if (clientes != null)
                    {
                        clientes.nombre = entradas.nombre;
                        clientes.paterno = entradas.paterno;
                        clientes.materno = entradas.materno;
                        clientes.telefono = entradas.telefono;
                        clientes.catSexoId = entradas.catSexoId > 0 ? entradas.catSexoId : null;
                        clientes.catColoniaId = entradas.coloniaId > 0 ? entradas.coloniaId : null;
                        clientes.fechaNacimiento = entradas.fechaNacimiento != null ? entradas.fechaNacimiento : null;
                        clientes.password = entradas.contraseniaNueva;

                        await contextEntity.SaveChangesAsync();

                        var clienteActualizado = contextEntity.clientes.Where(w => w.idCliente == entradas.clienteId).FirstOrDefault();
                        membresias membresia = clienteActualizado.membresias.FirstOrDefault();

                        respuesta.Success = true;
                        respuesta.ErrorMessage = "";
                        respuesta.clienteId = clienteActualizado.idCliente;
                        respuesta.email = clienteActualizado.email;
                        respuesta.correoElectronico = clienteActualizado.email;
                        respuesta.nombre = clienteActualizado.nombre;
                        respuesta.paterno = clienteActualizado.paterno;
                        respuesta.materno = clienteActualizado.materno;
                        respuesta.telefono = clienteActualizado.telefono;
                        respuesta.sexo = clienteActualizado.catSexoId != null ? clienteActualizado.catSexoId : 0;
                        respuesta.catColoniaId = clienteActualizado.catColoniaId != null ? clienteActualizado.catColoniaId : 0;
                        respuesta.catCiudadId = clienteActualizado.catColonias != null ? clienteActualizado.catColonias.catCiudadId : 0;
                        respuesta.fechaNacimiento = clienteActualizado.fechaNacimiento;
                        respuesta.colonia = clienteActualizado.catColoniaId != null ? clienteActualizado.catColonias.descripcion : "";
                        respuesta.membresiaId = membresia.idMembresia;
                        respuesta.folioMembresia = membresia.folio;
                        respuesta.fechaFinMembresia = membresia.fechaFin;
                        respuesta.folioGuidMembresia = membresia.guid;
                        respuesta.urlFotoPerfil = clienteActualizado.urlFotoPerfil;
                        respuesta.fechaCargaFoto = clienteActualizado.fechaCargaFoto;
                        respuesta.contrasenia = clienteActualizado.password;
                        respuesta.registroCompleto = validar.validarUsuarioDatosRequeridos(clienteActualizado);

                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = "Usuario no existe";
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



        [Route("api/recuperarPassword")]
        public async Task<ResponseBase> recuperarPasswordAsync([FromBody]RequestUsuarioRecuperarPassword emailVerificar)
        {
            ResponseBase resultado = new ResponseBase();
            SendEmail enviarCorreo = new SendEmail();
            clientes usuarioValidado = new clientes();

            try
            {
                usuarioValidado = contextEntity.clientes.FirstOrDefault(w => w.email == emailVerificar.email && w.empresaId == emailVerificar.empresaId);
                if (usuarioValidado != null)
               {
                    var configuracionSistema = contextEntity.configuracionSistema.Where(w => w.empresaId == emailVerificar.empresaId).FirstOrDefault();
                    var body = "<p>Su contraseña es: {0} </p>";
                    string.Format(body, usuarioValidado.password);
           
                await enviarCorreo.SendEmailAsync(usuarioValidado.email, ConfigurationManager.AppSettings.Get("subject"), string.Format(body, usuarioValidado.password), configuracionSistema.correoContacto, usuarioValidado.nombre + " " + usuarioValidado.paterno + " " + usuarioValidado.materno, ConfigurationManager.AppSettings.Get("smtpCorreoDe"));
                //await enviarCorreo.SendEmailAsync(emailVerificar.email, ConfigurationManager.AppSettings.Get("subject"), string.Format(body, "test"), emailVerificar.email, "test", ConfigurationManager.AppSettings.Get("smtpCorreoDe"));
                resultado.Success = true;
                }
                else
                {
                    resultado.Success = false;
                    resultado.ErrorMessage = "No existe cliente.";
                }
            }
            catch (Exception e)
            {
                logger.Error("ERROR:" + e.Message);
                resultado.Success = false;
                resultado.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_ERROR_SERVIDOR);
            }
            return resultado;
        }


      
        [Route("api/validarRecuperarContrasenia")]
        public ResponseBase validarRecuperarContrasenia([FromBody]RequestBase entrada)
        {
            ResponseBase respuesta = new ResponseBase();
            try
            {
                if (validar.UsuarioRecuperarContrasenia(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "Cliente esta registrado correctamente";
                }
                else
                {
                    respuesta.Success = false;
                    respuesta.ErrorMessage = "Intenta entrar con tu facebook y actualiza tus datos de registro";
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