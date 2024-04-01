using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class ComentarioController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";

        [Route("api/insertarComentario")]
        public Models.Salidas.ResponseComentario insertarComentario([FromBody]RequestComentarioInsertar entrada)
        {
            ResponseComentario respuesta = new ResponseComentario();
            try
            {
                //if (validar.UsuarioExiste(entrada.correoElectronico, entrada.contrasenia, entrada.empresaId))
                if (validar.IsAppSecretValid)
                {
                    comentarios comentarioRegistrar = new comentarios();

                    comentarioRegistrar.mensaje = entrada.mensaje;
                    comentarioRegistrar.catTipoComentarioId = entrada.tipoComentarioId;
                    comentarioRegistrar.clienteId = entrada.clienteId;
                    comentarioRegistrar.fechaRegistro = DateTime.Now;
                    comentarioRegistrar.activo = true;
                    comentarioRegistrar.fromComercio = entrada.fromComercio;
                    comentarioRegistrar.fromCliente = entrada.fromCliente;

                    contextEntity.comentarios.Add(comentarioRegistrar);

                    if ((contextEntity.SaveChanges()) <= 0)
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = "Error";
                    }
                    else
                    {
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "Comentario registrado correctamente";
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