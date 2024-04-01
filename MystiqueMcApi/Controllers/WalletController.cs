using MystiqueMC.DAL;
using MystiqueMcApi.Helpers;
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
    public class WalletController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";


        [Route("api/insertarWallet")]
        public ResponseBase insertarWallet([FromBody]RequestWallet entradas)
        {
            ResponseBase respuesta = new ResponseBase();

            try
            {
                // if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    wallet walletRegistrar = new wallet();

                    walletRegistrar.beneficioId = entradas.beneficioId;
                    //walletRegistrar.sucursalId = wallet.sucursalId;
                    walletRegistrar.clienteId = entradas.clienteId;
                    walletRegistrar.fechaRegistro = DateTime.Now;
                    walletRegistrar.estatus = true;
                    contextEntity.wallet.Add(walletRegistrar);

                    if ((contextEntity.SaveChanges()) <= 0)
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = "Error";
                    }
                    else
                    {
                        respuesta.Success = true;
                        respuesta.ErrorMessage = "Wallet registrado correctamente";
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
                respuesta.ErrorMessage = "Ocurrio un error en el servidor.";
            }
            return respuesta;
        }



        [Route("api/eliminarWallet")]
        public ResponseBase eliminarWallet([FromBody]RequestWallet entradas)
        {
            ResponseBase respuesta = new ResponseBase();

            try
            {
                // if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia , entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    wallet walletRegistrar = contextEntity.wallet.Where(w => w.clienteId == entradas.clienteId && w.beneficioId == entradas.beneficioId).FirstOrDefault();

                    if (walletRegistrar != null)
                    {
                        contextEntity.wallet.Remove(walletRegistrar);
                        if ((contextEntity.SaveChanges()) <= 0)
                        {
                            respuesta.Success = false;
                            respuesta.ErrorMessage = "Error";
                        }
                        else
                        {
                            respuesta.Success = true;
                            respuesta.ErrorMessage = "";
                        }
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.ErrorMessage = "No se encontro beneficio para borrar";
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
                respuesta.ErrorMessage = "Ocurrio un error en el servidor.";
            }
            return respuesta;
        }

        [Route("api/obtenerWalletPorCliente")]
        public ResponseListaWallet obtenerWalletPorCliente([FromBody]RequestWalletObtener entradas)
        {
            ResponseListaWallet respuesta = new ResponseListaWallet();

            try
            {
                //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
                if (validar.IsAppSecretValid)
                {
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                    respuesta.listaWallet = contextEntity.SP_Obtener_ListaWallet_Cliente(entradas.clienteId).ToList();
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
                respuesta.ErrorMessage = "Ocurrio un error en el servidor.";
            }
            return respuesta;
        }


        [Route("api/eliminarColeccionWallet")]
        public ResponseWallet eliminarColeccionWallet([FromBody]RequestWalletColeccion entradas)
        {
            ResponseWallet respuesta = new ResponseWallet();
            //if (validar.UsuarioExiste(entradas.correoElectronico, entradas.contrasenia, entradas.empresaId))
            if (validar.IsAppSecretValid)
            {
                using (var transaction = contextEntity.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in entradas.ListWalletId)
                        {
                            wallet walletEliminar = new wallet();
                            walletEliminar.idWallet = item;

                            contextEntity.wallet.Remove(walletEliminar);
                            contextEntity.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error("ERROR:" + e.Message);
                        transaction.Rollback();
                        respuesta.Success = false;
                        respuesta.ErrorMessage = e.Message;
                    }
                    transaction.Commit();
                    respuesta.Success = true;
                    respuesta.ErrorMessage = "";
                }
            }
            else
            {
                respuesta.Success = false;
                respuesta.ErrorMessage = validar.ObtenerMensajeRespuesta(MENSAJE_NO_PERMISOS);
            }
            return respuesta;
        }

    }
}