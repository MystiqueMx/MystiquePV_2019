using MystiqueMC.DAL;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    //TODO
    public class ClienteController : BaseApiController
    {
        [Route("api/hazPedido/BuscarClienteCallCenter")]
        public ResponseClienteCallCenter BuscarClienteCallCenter([FromBody]RequestClienteCallCenter entradas)
        {
            #region BuscarClienteCallCenter
            ResponseClienteCallCenter respuesta = new ResponseClienteCallCenter();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        ClientesCallCenter clienteEncontrado = Contexto.ClientesCallCenter.Where(w => w.Telefono == entradas.telefono).FirstOrDefault();
                        respuesta.existe = Contexto.ClientesCallCenter.Where(w => w.Telefono == entradas.telefono) != null;
                        if (respuesta.existe)
                        {
                            string nombreCompleto = clienteEncontrado.Nombre
                                + " " + clienteEncontrado.Paterno
                                + " " + clienteEncontrado.Materno ?? "";
                            respuesta.estatusPeticion = RespuestaOkMensaje("El número de teléfono ya existe con: " + nombreCompleto);
                        }
                        else
                            respuesta.estatusPeticion = RespuestaOk;
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta; 
            #endregion
        }

        [Route("api/hazPedido/BuscarListaClienteCallCenter")]
        public ResponseClienteCallCenter BuscarListaClienteCallCenter([FromBody]RequestClienteCallCenter entradas)
        {
            #region BuscarListaClienteCallCenter
            ResponseClienteCallCenter respuesta = new ResponseClienteCallCenter();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        respuesta.ListaClientesCallCenter = Contexto.ClientesCallCenter
                            .Where(w => (string.IsNullOrEmpty(entradas.telefono) || w.Telefono == entradas.telefono)
                                && (string.IsNullOrEmpty(entradas.nombre) || (w.Nombre + " " + w.Paterno + " " + w.Materno ?? "").ToLower().Contains(entradas.nombre.ToLower())) )
                            .Select(s => new ListClientesCallCenter
                            {
                                ID = s.IdClienteCallCenter,
                                nombreCompleto = s.Nombre + " " + s.Paterno + " " + s.Materno ?? "",
                                telefono = s.Telefono
                            }).ToList();
                        
                            respuesta.estatusPeticion = RespuestaOk;
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta;
            #endregion
        }

        [Route("api/hazPedido/ObtenerClientesCallCenter")]
        public ResponseClienteCallCenter ObtenerClientesCallCenter()
        {
            #region BuscarClienteCallCenter
            ResponseClienteCallCenter respuesta = new ResponseClienteCallCenter();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        respuesta.ListaClientesCallCenter = Contexto.ClientesCallCenter.Select(s => new ListClientesCallCenter
                        {
                            ID = s.IdClienteCallCenter,
                            nombreCompleto = s.Nombre + " " + s.Paterno + " " + s.Materno ?? "",
                            telefono = s.Telefono
                        }).ToList();
                        respuesta.estatusPeticion = RespuestaOk;
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta;
            #endregion
        }

        [Route("api/hazPedido/GuardarClienteCallCenter")]
        public ResponseClienteCallCenter GuardarClienteCallCenter([FromBody]RequestRegistroClienteCallCenter entradas)
        {
            #region GuardarClienteCallCenter
            ResponseClienteCallCenter respuesta = new ResponseClienteCallCenter();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {

                        bool telefonoExiste = Contexto.ClientesCallCenter.Where(w => w.Telefono == entradas.telefono).Count() > 0;
                        if (telefonoExiste)
                        {
                            respuesta.estatusPeticion = RespuestaErrorValidacion("El número de teléfono ya existe");
                        } else {

                            Contexto.ClientesCallCenter.Add(new MystiqueMC.DAL.ClientesCallCenter
                            {
                                Nombre = entradas.nombre,
                                Paterno = entradas.apPaterno,
                                Materno = entradas.apMaterno,
                                Telefono = entradas.telefono
                            });
                            Contexto.SaveChanges();
                            respuesta.estatusPeticion = RespuestaOkMensaje("Registro guardado");
                        }
                    }
                    else
                    {
                        respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.estatusPeticion = RespuestaErrorValidacion(ModelState);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return respuesta; 
            #endregion
        }
    }
}
