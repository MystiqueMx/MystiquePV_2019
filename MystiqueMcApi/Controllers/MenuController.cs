using MystiqueMC.DAL;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class MenuController : BaseApiController
    {
        private readonly string _hostImagenes = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");

        [Route("api/hazPedido/ObtenerMenuSucursal")]
        public ResponseMenu ObtenerMenuSucursal([FromBody]RequestMenu entradas)
        {
            ResponseMenu respuesta = new ResponseMenu();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        respuesta.respuesta = Contexto.SucursalCategoriaProducto
                            .Where(w => w.SucursalID == entradas.sucursalId
                                        && w.Activo)
                            .Select(s => new ResponseMenuListado
                            {
                                menuId = s.CategoriaProductos.idCategoriaProducto,
                                descripcion = s.CategoriaProductos.descripcion,
                                imagen = _hostImagenes + s.CategoriaProductos.ImagenApp,
                                orden = s.CategoriaProductos.indice,
                                esEnsalada = s.CategoriaProductos.esEnsalada ?? false
                            }).OrderByDescending(o => o.orden).ToList();

                        //respuesta.respuesta = Contexto.Menus
                        //    .Where(w => w.sucursalId == entradas.sucursalId
                        //                && w.estatus)
                        //    .Select(s => new ResponseMenuListado
                        //    {
                        //        menuId = s.idMenu,
                        //        descripcion = s.nombre,
                        //        imagen = _hostImagenes + s.imagenAPP,
                        //        orden = s.ordenamiento,
                        //        esEnsalada = s.esEnsalada
                        //    }).OrderByDescending(o => o.orden).ToList();
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


        [Route("api/hazPedido/ObtenerPlatillosEnsaladas")]
        public ResponseListasMenuEnsalada ObtenerPlatillosEnsaladas([FromBody]RequestMenuEnsalada entradas)
        {
            ResponseListasMenuEnsalada respuesta = new ResponseListasMenuEnsalada();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {

                        respuesta.respuesta = new ResponseMenuEnsalada();
                        respuesta.respuesta.ingredientesEnsalada = Contexto.CatIngredienteEnsaladaPrecios.Select(s => new CatEnsaladaPlatilloPrecios
                        {
                            id = s.idCatIngredienteEnsaladaPrecio,
                            descripcion = s.descripcion,
                            precio = s.precio,
                            catTipoIngredienteEnsalada = s.catTipoIngredienteEnsaladaId
                        }).ToList();

                        respuesta.respuesta.configuracionEnsalada = Contexto.ConfEnsaladaPlatillos.Select(s => new EnsaladaPlatillo
                        {
                            idPresentacion = s.idConfEnsaladaPlatillo,
                            platilloId = s.platilloId,
                            tamanio = s.tamanio,
                            precioBase = s.precioBase,
                            proteina = s.proteina,
                            barraFria = s.barraFria,
                            aderezos = s.aderezo,
                            complementos = s.complemento,
                            cortesia = s.cortesia
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
        }

        [Route("api/hazPedido/ObtenerPlatillosMenu")]
        public ResponseListasMenu ObtenerPlatillosMenu([FromBody]RequestPlatillos entradas)
        {
            ResponseListasMenu respuesta = new ResponseListasMenu();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var configuracionMenuPlatillos = Contexto.SucursalProductos.Where(w => w.sucursalId == entradas.sucursalId
                            && w.Productos.categoriaProductoId == entradas.menuId
                            && w.activo).Select(c => new ResponseMenuPlatillo
                            {
                                idPlatillo = c.Productos.idProducto,
                                nombre = c.Productos.nombre,
                                desPlatillo = c.Productos.descripcion,
                                isCombo = c.Productos.esCombo,
                                urlImagen = c.Productos.urlImagenApp,
                                precio = c.precio,
                                orden = c.Productos.indice
                            }).OrderByDescending(o => o.orden).ToList();

                        respuesta.respuesta = new listaResponseMenuPlatillo();
                        respuesta.respuesta.listaPlatillos = new List<ResponseMenuPlatillo>();

                        var configuracionPlatillosItemUno = Contexto.ConfMenuPlatillosItemUno.Where(w => w.menuId == entradas.menuId
                            && w.SucursalID == entradas.sucursalId).ToList();

                        for (int r = 0; r < configuracionMenuPlatillos.Count; r++)
                        {
                            respuesta.respuesta.listaPlatillos.Add(obtenerNivelesPlatillo(configuracionMenuPlatillos, r, entradas.menuId, configuracionPlatillosItemUno));
                        }

                        //var configuracionMenuPlatillos = Contexto.ConfMenuPlatillos.Where(w => w.menuId == entradas.menuId && w.Platillos.estatus == true).Select(c => new ResponseMenuPlatillo
                        //{
                        //    idPlatillo = c.platilloId,
                        //    nombre = c.Platillos.nombre,
                        //    desPlatillo = c.Platillos.descripcionApp,
                        //    isCombo = c.Platillos.isCombo,
                        //    urlImagen = c.Platillos.imagenApp,
                        //    precio = c.Platillos.precio,
                        //    orden = c.Platillos.ordenamiento,
                        //}).OrderByDescending(o => o.orden).ToList();

                        //respuesta.respuesta = new listaResponseMenuPlatillo();
                        //respuesta.respuesta.listaPlatillos = new List<ResponseMenuPlatillo>();

                        //var configuracionPlatillosItemUno = Contexto.ConfMenuPlatillosItemUno.Where(w => w.menuId == entradas.menuId).ToList();

                        //for (int r = 0; r < configuracionMenuPlatillos.Count; r++)
                        //{
                        //    respuesta.respuesta.listaPlatillos.Add(obtenerNivelesPlatillo(configuracionMenuPlatillos, r, entradas.menuId, configuracionPlatillosItemUno));
                        //}
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



        private ResponseMenuPlatillo obtenerNivelesPlatillo(List<ResponseMenuPlatillo> datos, int r, int menuId, List<ConfMenuPlatillosItemUno> configuracionPlatillosItemUno)
        {
            ResponseMenuPlatillo platilo = new ResponseMenuPlatillo();
            int idPlatillo = datos.ElementAt(r).idPlatillo;
            var datosMenu = configuracionPlatillosItemUno.Where(w => w.menuId == menuId && w.platilloId == idPlatillo).ToList();

            platilo.desPlatillo = datos.ElementAt(r).desPlatillo;
            platilo.idPlatillo = datos.ElementAt(r).idPlatillo;
            platilo.precio = datos.ElementAt(r).precio;
            platilo.nombre = datos.ElementAt(r).nombre;
            platilo.urlImagen = _hostImagenes + datos.ElementAt(r).urlImagen;

            if (datos.ElementAt(r).isCombo)
            {
                platilo.cantidad = datosMenu.Count();
            }
            else
            {
                platilo.cantidad = 1;
            }

            platilo.configuracionUno = new List<PlatilloNivelUno>();

            for (int x = 0; x < datosMenu.Count; x++)
            {
                PlatilloNivelUno plaNivelUno = new PlatilloNivelUno();
                if (datosMenu.ElementAt(x).itemUnoId != null)
                {

                    plaNivelUno.id = datosMenu.ElementAt(x).itemUnoId;
                    plaNivelUno.descripcion = datosMenu.ElementAt(x).ItemUno.nombre;
                    plaNivelUno.precio = datosMenu.ElementAt(x).precio;
                    plaNivelUno.cantidad = datosMenu.ElementAt(x).cantidadSeleccion;
                    plaNivelUno.configuracionDos = new List<PlatilloNivelDos>();

                    var datos3 = datosMenu.ElementAt(x).ConfMenuPlatillosItemDos.Where(w => w.confMenuPlatillosItemUnoId == datosMenu.ElementAt(x).idConfMenuPlatillosItemUno).ToList();
                    for (int y = 0; y < datos3.Count; y++)
                    {
                        PlatilloNivelDos plaNivelDos = new PlatilloNivelDos();
                        plaNivelDos.id = datos3.ElementAt(y).itemDosId;
                        plaNivelDos.descripcion = datos3.ElementAt(y).ItemDos.nombre;
                        plaNivelDos.precio = datos3.ElementAt(y).precio;
                        plaNivelDos.cantidad = datos3.ElementAt(y).cantidadSeleccion;
                        plaNivelDos.configuracionTres = new List<PlatilloNivelTres>();

                        var datos4 = datos3.ElementAt(y).ConfMenuPlatillosItemTres.Where(w => w.confMenuPlatillosItemDosId == datos3.ElementAt(y).idConfMenuPlatillosItemDos).ToList();

                        for (int z = 0; z < datos4.Count; z++)
                        {
                            PlatilloNivelTres plaNivelTres = new PlatilloNivelTres();
                            plaNivelTres.id = datos4.ElementAt(z).itemTresId;
                            plaNivelTres.descripcion = datos4.ElementAt(z).ItemTres.nombre;
                            plaNivelTres.precio = datos4.ElementAt(z).precio;
                            plaNivelDos.configuracionTres.Add(plaNivelTres);
                        }
                        plaNivelUno.configuracionDos.Add(plaNivelDos);
                    }

                    platilo.configuracionUno.Add(plaNivelUno);
                }
            }
            return platilo;
        }
    }
}
