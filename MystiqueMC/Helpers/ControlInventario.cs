using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MystiqueMC.DAL;
using log4net;
using System.Data.Entity;
using MystiqueMC.Models;

namespace MystiqueMC.Helpers
{
    public class ControlInventario
    {
        #region CONTEXT
        private MystiqueMeEntities _context;
        public MystiqueMeEntities Contexto
        {
            get
            {
                if (_context != null) return _context;

                _context = new MystiqueMeEntities();
                _context.Database.Log = sql => Logger.Debug(sql);
                return _context;
            }
        }
        #endregion

        #region LOGS
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ILog Logger { get => _logger; }
        #endregion
        /// <summary>
        /// registroMovimientoInventario
        /// </summary>
        /// <param name="Contexto"></param>
        /// <param name="catMovimientoId"></param>
        /// <param name="cantidad"></param>
        /// <param name="insumoId"></param>
        /// <param name="sucursalId"></param>
        /// <param name="cantidadNueva"></param>
        /// <param name="UsuarioActual"></param>
        /// <param name="tipoMovimiento"></param>
        /// <param name="detalleCompraId"></param>
        /// <param name="observaciones"></param>
        /// <param name="saveChanges">indica si se debe persistir el movimiento, o el guardado de la información ocurre en otro proceso como en la actualizacion de movimientos por reapertura</param>
        /// <returns></returns>
        public ControlInventariosResponse registroMovimientoInventario(MystiqueMeEntities Contexto, int? catMovimientoId, decimal cantidad, int insumoId, int sucursalId, decimal cantidadNueva, usuarios UsuarioActual, TiposMovimientosInventario tipoMovimiento, int? detalleCompraId, string observaciones, bool saveChanges = true)
        {
            ControlInventariosResponse result = new ControlInventariosResponse();
            try
            {
                //TiposMovimientosInventario tipo_movimiento = (TiposMovimientosInventario)tipoMovimiento;
                Insumos insumo = Contexto.Insumos.Where(i => i.idInsumo == insumoId).FirstOrDefault();

                var movimiento = Contexto.MovimientoInventarios.Add(new DAL.MovimientoInventarios
                {
                    catMovimientoId = catMovimientoId,
                    insumoId = insumoId,
                    cantidad = cantidad,
                    sucursalId = sucursalId,
                    fechaRegistro = DateTime.Now,
                    usuarioRegistroId = UsuarioActual.idUsuario,
                    tipoMovimiento = tipoMovimiento.ToString(),
                    detalleCompraId = detalleCompraId,
                    observaciones = observaciones,
                    unidadMedidaCompraId = insumo.UnidadMedida1.idUnidadMedida,
                    unidadMedidaCompraDesc = insumo.UnidadMedida1.descripcion,
                    unidadMedidaConsumoId = insumo.UnidadMedida.idUnidadMedida,
                    unidadMedidaConsumoaDesc = insumo.UnidadMedida.descripcion
                    
                });               

                var inventario = Contexto.Inventarios.Where(i => i.insumoId == insumoId && i.sucursalId == sucursalId).FirstOrDefault();
               
                //Si no existe registro de inventario se inserta, la actualizacion del inventario se hace por base de datos.
                if (inventario == null)
                {                   
                    Contexto.Inventarios.Add(new Inventarios
                    {
                        activo = true,
                        cantidad = cantidadNueva,
                        fechaRegistro = DateTime.Now,
                        insumoId = insumoId,
                        maximo = 0,
                        minimo = 0,
                        precio = 0,
                        sucursalId = sucursalId
                    });
                }
                //if (inventario != null)
                //{
                //    inventario.cantidad = cantidadNueva;
                //    Contexto.Entry(inventario).State = EntityState.Modified;
                //}
                //else
                //{
                //    Contexto.Inventarios.Add(new Inventarios
                //    {
                //        activo = true,
                //        cantidad = cantidadNueva,
                //        fechaRegistro = DateTime.Now,
                //        insumoId = insumoId,
                //        maximo = 0,
                //        minimo = 0,
                //        precio = 0,
                //        sucursalId = sucursalId                        
                //    });
                //}

                if(saveChanges)
                {
                    Contexto.SaveChanges();
                }                    

                result.isSuccess = true;
                result.hasError = false;
                result.isRedirectToAction = true;
                result.actionName = "Index";
                result.controllerName = "Inventarios";
                result.routeValue = new { sucursaId = sucursalId };
            }
            catch (Exception e)
            {
                Logger.Error(e);

                result.isSuccess = false;
                result.hasError = true;
                result.isRedirectToAction = true;
                result.actionName = "Index";
                result.controllerName = "Inventarios";
                result.routeValue = new { sucursaId = sucursalId };
            }

            return result;
        }

    }
}