using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.SyncApi.Helpers.Base;
using WebApp.SyncApi.Models.Requests;
using WebApp.SyncApi.Models.Responses;
using MystiqueMC.DAL;
using System.Data;
using System.Data.Entity;

namespace WebApp.SyncApi.Controllers
{
    [Authorize]
    [Route("api/v1/menu")]
    public class SyncMenuController : BaseApiController
    {
        [HttpGet]
        public ResponseMenu ObtenerMenuSucursal()
        {
            ResponseMenu respuesta = new ResponseMenu();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        
                        var sucursales = Contexto.ConfUsuarioSucursales
                            .Include(i => i.sucursales)
                            .Include(i=> i.sucursales.direccion)
                            .Include(i => i.sucursales.direccion.catCiudades)
                            .Include(i => i.sucursales.direccion.catEstados)
                            .Include(i => i.sucursales.datosFiscales)
                            .Include(i => i.sucursales.datosFiscales.direccion)
                            .Include(i => i.sucursales.datosFiscales.catRegimenFiscal)
                            .Include(i => i.sucursales.datosFiscales.direccion.catCiudades)
                            .Include(i => i.sucursales.datosFiscales.direccion.catEstados)
                            .Where(w => w.usuarioId == CurrentUserId).FirstOrDefault();
                         
                        if(sucursales != null)
                        {
                            var sucursalId = sucursales.sucursalId;
                            var comercioId = sucursales.sucursales.comercioId;
                            var ciudadSucursalId = sucursales.sucursales.direccion.catCiudadId;
                            respuesta.respuesta = new ResponseEntidadesMenu();

                            respuesta.respuesta.ListadoAreaPreparacion = Contexto.AreaPreparacion.Where(w => w.comercioId == comercioId).Select(s => new EntidadAreaPreparacion
                            {
                                id = s.idAreaPreparacion,
                                nombre = s.descripcion
                            }).ToList();

                            respuesta.respuesta.ListadoCategoriaInsumo = Contexto.CategoriaInsumo.Where(w => w.comercioId == comercioId).Select(s => new EntidadCategoriaInsumo
                            {
                                id = s.idCategoriaInsumo,
                                nombre = s.descripcion
                            }).ToList();


                            respuesta.respuesta.ListadoCategoriaProductos = Contexto.CategoriaProductos.Where(w => w.comercioId == comercioId)
                                .Select(s => new EntidadCategoriaProductos
                                {
                                    id = s.idCategoriaProducto,
                                    codigo = s.codigo,
                                    nombre = s.descripcion,
                                    indice = s.indice
                                }).ToList();

                            respuesta.respuesta.ListadoVariedadProductos = Contexto.VariedadProductos
                                .Include(i => i.Productos)
                                 .Include(i => i.Productos.SucursalProductos)
                                .Where(w => w.Productos.SucursalProductos.Any(c=> c.sucursalId == sucursalId))
                                .Select(s => new EntidadVariedadProductos
                                {
                                   id = s.idVariedadProducto,
                                   nombre = s.descripcion,
                                   productoId = s.productoId,
                                   imagen = s.imagen
                                }).ToList();


                            respuesta.respuesta.ListadoProductos = Contexto.SucursalProductos
                               .Include(i => i.Productos)                             
                               .Where(w => w.sucursalId == sucursalId && w.activo == true)
                               .Select(s => new EntidadProductos
                               {
                                   id = s.Productos.idProducto,
                                   areaPreparacionId = s.Productos.areaPreparacionId,                                  
                                   categoriaProductoId = s.Productos.categoriaProductoId,
                                   esCombo = s.Productos.esCombo,                                  
                                   imagen = s.Productos.urlImagen,
                                   indice = s.Productos.indice,
                                   nombre = s.Productos.nombre,
                                   precio = s.precio,                                
                                   tieneVariedad = s.Productos.VariedadProductos.Any(),
                                   clave = s.Productos.clave,
                                   esEnsalada = s.Productos.armarCobroMostrador, 
                                   principal = s.Productos.principal,
                               }).ToList();

                            respuesta.respuesta.ListadoAgrupadorInsumos = Contexto.AgrupadorInsumos
                                 .Where(w => w.comercioId == comercioId)
                                 .Select(s => new EntidadAgrupadorInsumos
                                 {
                                     id = s.idAgrupadorInsumo,
                                     nombre = s.descripcion,
                                     agregarExtra = s.agregarExtra,
                                     confirmarPorSeparado = s.confirmarPorSeparado,
                                     indice = s.indice,
                                     productoId = s.productoId
                                 }).ToList();

                            respuesta.respuesta.ListadoInsumoProductos = Contexto.InsumoProductos
                                 .Include(i => i.Productos)
                                 .Include(i => i.Productos.SucursalProductos)
                                 .Include(i => i.Insumos)
                                 .Where(w => w.Productos.SucursalProductos.Any(c => c.sucursalId == sucursalId) || w.Insumos.comercioId== comercioId)
                                 .Select(s => new EntidadInsumoProductos
                                 {
                                     productoId = s.productoId,
                                     agrupadorInsumoId = s.agrupadorInsumoId,
                                     id = s.idInsumoProducto,
                                     insumoId = s.insumoId
                                 }).ToList();

                            respuesta.respuesta.ListadoConfiguracionArmadoProducto = Contexto.ConfiguracionArmadoProductos
                                 .Include(i => i.Productos)
                                 .Include(i => i.Productos.SucursalProductos)
                                 .Where(w => w.Productos.SucursalProductos.Any(c => c.sucursalId == sucursalId))
                                 .Select(s => new EntidadConfiguracionArmadoProducto
                                 {
                                     id = s.idConfiguracionArmadoProducto,
                                     agrupadorInsumoId = s.agrupadorInsumoId,
                                     cantidad = s.cantidad,
                                     productoId = s.productoId
                                 }).ToList();
                            
                            respuesta.respuesta.ListadoColonias = Contexto.catColonias.Where(w => w.catCiudadId == ciudadSucursalId)
                                .Select(s => new EntidadColonias
                                {
                                    id = s.idCatColonia,
                                    nombre = s.descripcion
                                }).ToList();

                            respuesta.respuesta.ListadoInsumos = Contexto.Insumos.Where(w => w.comercioId == comercioId)
                                .Select(s => new EntidadInsumos
                                {
                                    id = s.idInsumo,
                                    categoriaInsumoId = s.categoriaInsumoId,
                                    imagen = s.imagen,
                                    nombre = s.nombre
                                }).ToList();


                            var direcFiscal = "";

                            if (sucursales.sucursales.datosFiscales != null)
                            {
                                direcFiscal = String.Concat(sucursales.sucursales.datosFiscales.direccion.calle, " ", sucursales.sucursales.datosFiscales.direccion.numExterior, " ", sucursales.sucursales.datosFiscales.direccion.numInterior, " ", sucursales.sucursales.datosFiscales.direccion.codigoPostal, " ", sucursales.sucursales.datosFiscales.direccion.catCiudades.ciudadDescr, " ", sucursales.sucursales.datosFiscales.direccion.catEstados.estadoDescr);
                            }

                            var datosConfiguracionSucursal = new EntidadConfiguracionSucursal
                            {
                                id = sucursales.sucursalId,
                                nombre = sucursales.sucursales.nombre,
                                costoEnvio = sucursales.sucursales.costoEnvio != null ? sucursales.sucursales.costoEnvio : 0,
                                faceBook = sucursales.sucursales.faceBook != null ? sucursales.sucursales.faceBook : "",
                                iva = sucursales.sucursales.iva != null ? sucursales.sucursales.iva : 0,
                                maxEfectivo = sucursales.sucursales.maxEfectivo != null ? sucursales.sucursales.maxEfectivo : 0,
                                mensajeTicket = sucursales.sucursales.mensajeTicket != null ? sucursales.sucursales.mensajeTicket : "",
                                sitioWeb = sucursales.sucursales.sitioWeb != null ? sucursales.sucursales.sitioWeb : "",
                                direccion = String.Concat(sucursales.sucursales.direccion.calle, " ", sucursales.sucursales.direccion.numExterior, " ", sucursales.sucursales.direccion.numInterior, " ", sucursales.sucursales.direccion.codigoPostal, " ", sucursales.sucursales.direccion.catCiudades.ciudadDescr, " ", sucursales.sucursales.direccion.catEstados.estadoDescr),
                                razonSocial = sucursales.sucursales.datosFiscales != null ? sucursales.sucursales.datosFiscales.nombreFiscal : "",
                                rfc = sucursales.sucursales.datosFiscales != null ? sucursales.sucursales.datosFiscales.rfc : "",
                                direccionFiscal = direcFiscal ,
                                regimenFiscal = sucursales.sucursales.datosFiscales.catRegimenFiscal.descripcion,
                                logo = sucursales.sucursales.logoTickets,                                
                            };

                            respuesta.respuesta.ListadoConfiguracionSucursal = datosConfiguracionSucursal;

                            respuesta.respuesta.ListadoDescuentos = Contexto.SucursalDescuentos
                                .Include(i => i.Descuentos)
                                .Where(w => w.sucursalId == sucursalId && w.activo == true)
                                .Select(s => new EntidadDescuentos
                                {
                                    id = s.descuentoId,
                                    nombre = s.Descuentos.nombre,
                                    fechaInicio = s.Descuentos.fechaInicio,
                                    fechaFin = s.Descuentos.fechaFin,
                                    horaInicio = s.Descuentos.horaInicio,
                                    horaFin = s.Descuentos.horaFin,
                                    montoPorcentaje = s.Descuentos.montoporcentaje,
                                    lunes = s.Descuentos.lunes,
                                    martes = s.Descuentos.martes,
                                    miercoles = s.Descuentos.miercoles,
                                    jueves = s.Descuentos.jueves,
                                    viernes = s.Descuentos.viernes,
                                    sabado = s.Descuentos.sabado,
                                    domingo = s.Descuentos.domingo,
                                    productoId = s.Descuentos.productoId,

                                }).ToList();


                            respuesta.respuesta.ListadoTiposPagos = Contexto.CatTipoPagos.Where(w=> w.comercioId == comercioId || w.cajaBase==true).Select(s => new EntidadTipoPagos
                            {
                                id = s.idCatTipoPago,
                                nombre = s.descripcion,
                                activo = s.activo,                                
                            }).ToList();


                            respuesta.EstatusPeticion = RespuestaOk;
                        }
                        else
                        {
                            respuesta.EstatusPeticion = RespuestaErrorValidacion("No se encontro sucursal asociada al usuario.");
                        }                      
                    }
                    else
                    {
                        respuesta.EstatusPeticion = RespuestaErrorValidacion(ModelState);
                    }
                }
                else
                {
                    respuesta.EstatusPeticion = RespuestaNoPermisos;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.EstatusPeticion = RespuestaErrorInterno;
            }
            return respuesta;
        }
    }
}