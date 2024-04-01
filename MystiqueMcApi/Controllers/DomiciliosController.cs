using MystiqueMC.DAL;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class DomiciliosController : BaseApiController
    {

        [Route("api/hazPedido/ObtenerDirecciones")]
        public ResponseDomicilio ObtenerDirecciones([FromBody]RequestDomicilio entradas)
        {
            ResponseDomicilio respuesta = new ResponseDomicilio();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var dirccionesConsumidor = Contexto.ConsumidorDirecciones.OrderByDescending(o => o.fechaRegistro).Where(w => w.consumidorId == entradas.consumidorId && w.activo == true).Select(c => new ResponseListaDomicilio
                        {
                            direccionId = c.idConsumidorDireccion,
                            calle = c.calle,
                            numeroInt = c.numInterior,
                            numeroExt = c.numExterior,
                            coloniaId = c.catColoniaId,
                            nombreColonia = c.nombreColonia,
                            codigoPostal = c.codigoPostal,
                            estadoId = c.catEstadoId,
                            nombreEstado = c.catEstados.estadoDescr,
                            codigoPais = c.catEstados.catPaises.codigoPais,
                            nombrePais = c.catEstados.catPaises.paisDescr,
                            referencias = c.referencia,
                            latitud = c.latitud,
                            longitud = c.longitud,
                            alias = c.alias
                        }).ToList();


                        //Remover direcciones que no esten dentro del area de entrega en base a la sucursal y la posicion de la direccion de entrada
                        //if (entradas.sucursalId >0 && dirccionesConsumidor.Count()>0)
                        //{
                        //    var outResultadoParameter = new ObjectParameter("verificado", typeof(int));
                        //    int resultado = 0;
                        //    for (int x=0; x < dirccionesConsumidor.Count(); x++)
                        //    {
                        //        resultado = 0;                             
                        //        Contexto.SP_Verificar_Cobertura((float)dirccionesConsumidor.ElementAt(x).latitud, (float)dirccionesConsumidor.ElementAt(x).longitud, entradas.sucursalId, outResultadoParameter);
                        //        resultado = Convert.ToInt32(outResultadoParameter.Value);
                        //        if(resultado<=0)
                        //        {
                        //            dirccionesConsumidor.RemoveAt(x);
                        //        }
                        //    } 
                        //}      

                        respuesta.respuesta = new List<ResponseListaDomicilio>();
                        respuesta.respuesta = dirccionesConsumidor;
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
        }

        [Route("api/hazPedido/AgregarDireccion")]
        public ResponseDomicilio AgregarDireccion([FromBody]RequestAgregarDomicilio entradas)
        {
            ResponseDomicilio respuesta = new ResponseDomicilio();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {

                        var datosColonia = Contexto.catColonias.Where(w => w.codigoPostal == entradas.codigoPostal).FirstOrDefault();

                        if (datosColonia == null)
                        {
                            datosColonia = Contexto.catColonias.Where(w => w.idCatColonia == entradas.coloniaId).FirstOrDefault();
                        }

                        using (var tx = Contexto.Database.BeginTransaction())
                        {
                            try
                            {
                                var direccion = Contexto.ConsumidorDirecciones.Add(new ConsumidorDirecciones
                                {
                                    consumidorId = entradas.consumidorId,
                                    alias = entradas.alias,
                                    calle = entradas.calle,
                                    numExterior = entradas.numeroExt,
                                    numInterior = entradas.numeroInt,
                                    catColoniaId = entradas.coloniaId != null ? entradas.coloniaId : null,
                                    catCiudadId = datosColonia.catCiudadId,
                                    catEstadoId = datosColonia.catCiudades.catEstadoId,
                                    nombreColonia = entradas.nombreColonia,
                                    referencia = entradas.referencias,
                                    latitud = entradas.latitud,
                                    longitud = entradas.longitud,
                                    fechaRegistro = DateTime.Now,
                                    activo = true,
                                    codigoPostal = datosColonia.codigoPostal
                                });

                                Contexto.SaveChanges();


                                var dirccionesConsumidor = Contexto.ConsumidorDirecciones.Where(w => w.consumidorId == entradas.consumidorId && w.idConsumidorDireccion == direccion.idConsumidorDireccion).Select(c => new ResponseListaDomicilio
                                {
                                    direccionId = c.idConsumidorDireccion,
                                    calle = c.calle,
                                    numeroInt = c.numInterior,
                                    numeroExt = c.numExterior,
                                    coloniaId = c.catColoniaId,
                                    nombreColonia = c.nombreColonia,
                                    codigoPostal = c.codigoPostal,
                                    estadoId = c.catEstadoId,
                                    nombreEstado = c.catEstados.estadoDescr,
                                    codigoPais = c.catEstados.catPaises.codigoPais,
                                    nombrePais = c.catEstados.catPaises.paisDescr,
                                    referencias = c.referencia,
                                    latitud = c.latitud,
                                    longitud = c.longitud,
                                    alias = c.alias
                                }).FirstOrDefault();

                                tx.Commit();
                                respuesta.respuesta = new List<ResponseListaDomicilio>();
                                respuesta.respuesta.Add(dirccionesConsumidor);
                                respuesta.estatusPeticion = RespuestaOk;
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                                respuesta.estatusPeticion = RespuestaErrorInterno;
                                tx.Rollback();
                            }
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
        }        

        [Route("api/hazPedido/EditarDireccion")]
        public ResponseDomicilio EditarDireccion([FromBody]RequestAgregarDomicilio entradas)
        {
            ResponseDomicilio respuesta = new ResponseDomicilio();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var datosColonia = Contexto.catColonias.Where(w => w.codigoPostal == entradas.codigoPostal).FirstOrDefault();

                        if (datosColonia == null)
                        {
                            datosColonia = Contexto.catColonias.Where(w => w.idCatColonia == entradas.coloniaId).FirstOrDefault();
                        }

                        var direccionConsumidor = Contexto.ConsumidorDirecciones.Where(w => w.consumidorId == entradas.consumidorId && w.idConsumidorDireccion == entradas.direccionId).FirstOrDefault();

                        direccionConsumidor.alias = entradas.alias;
                        direccionConsumidor.calle = entradas.calle;
                        direccionConsumidor.numExterior = entradas.numeroExt;
                        direccionConsumidor.numInterior = entradas.numeroInt;
                        direccionConsumidor.catColoniaId = entradas.coloniaId;
                        direccionConsumidor.catCiudadId = datosColonia.catCiudadId;
                        direccionConsumidor.catEstadoId = datosColonia.catCiudades.catEstadoId;
                        direccionConsumidor.referencia = entradas.referencias;
                        direccionConsumidor.latitud = entradas.latitud;
                        direccionConsumidor.longitud = entradas.longitud;
                        direccionConsumidor.fechaModificacion = DateTime.Now;
                        direccionConsumidor.activo = entradas.activo;

                        Contexto.SaveChanges();

                        var direccionesConsumidor = Contexto.ConsumidorDirecciones.Where(w => w.consumidorId == entradas.consumidorId && w.idConsumidorDireccion == entradas.direccionId).Select(c => new ResponseListaDomicilio
                        {
                            direccionId = c.idConsumidorDireccion,
                            calle = c.calle,
                            numeroInt = c.numInterior,
                            numeroExt = c.numExterior,
                            coloniaId = c.catColoniaId,
                            nombreColonia = c.catColonias.descripcion,
                            codigoPostal = c.catColonias.codigoPostal,
                            estadoId = c.catEstadoId,
                            nombreEstado = c.catEstados.estadoDescr,
                            codigoPais = c.catEstados.catPaises.codigoPais,
                            nombrePais = c.catEstados.catPaises.paisDescr,
                            referencias = c.referencia,
                            latitud = c.latitud,
                            longitud = c.longitud
                        }).FirstOrDefault();

                        respuesta.respuesta = new List<ResponseListaDomicilio>();
                        respuesta.respuesta.Add(direccionesConsumidor);
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
        }        

        [Route("api/hazPedido/ObtenerColonias")]
        public ResponseColoniaHazPedido ObtenerColonias([FromBody]RequestObtenerColonias entradas)
        {
            ResponseColoniaHazPedido respuesta = new ResponseColoniaHazPedido();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        respuesta.respuesta = Contexto.SP_Obtener_Colonias(entradas.latitud, entradas.longitud, entradas.sucursalId, entradas.codigoPostal).ToList();
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
        }

        [Route("api/hazPedido/VerificarCoberturaDireccion")]
        public ErrorObjCodeResponseBase VerificarCoberturaDireccion([FromBody]RequestVerificarCobertura entradas)
        {
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();

            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var outResultadoParameter = new ObjectParameter("verificado", typeof(int));
                        Contexto.SP_Verificar_Cobertura(entradas.latitud, entradas.longitud, entradas.sucursalId, outResultadoParameter);
                        int resultado = Convert.ToInt32(outResultadoParameter.Value);
                        if (resultado > 0)
                        {
                            respuesta.estatusPeticion = RespuestaOk;
                        }
                        else
                        {
                            respuesta.estatusPeticion = RespuestaErrorValidacion("Sin cobertura.");
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
        }        
    }
}
