using MystiqueMC.DAL;
using MystiqueMcApi.Helpers.Hangfire.Pedidos;
using MystiqueMcApi.Helpers.OpenPay;
using MystiqueMcApi.Helpers.OpenPay.Modelos;
using MystiqueMcApi.Models.Entradas;
using MystiqueMcApi.Models.Pedidos;
using MystiqueMcApi.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace MystiqueMcApi.Controllers
{
    public class PedidoController : BaseApiController
    {
        private readonly string _hostImagenes = ConfigurationManager.AppSettings.Get("HOSTNAME_IMAGENES");
        private readonly string MENSAJE_MONTO_MINIMO_PEDIDO = ConfigurationManager.AppSettings.Get("MENSAJE_MONTO_MINIMO_PEDIDO");

        [Route("api/hazPedido/RegistrarPedido")]
        public ResponseBasePedido RegistrarPedido([FromBody]RequestPedido entradas)
        {
            #region RegistrarPedido
            ResponseBasePedido respuesta = new ResponseBasePedido();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        bool recogerSucursal = false;
                        bool conCobertura = true;

                        respuesta.respuesta = new ResponsePedido();

                        var direccionEntrega = "";
                        var montoMinimo = Contexto.ConfSucursales
                            .Where(w => w.sucursalId == entradas.pedido.restaurante.idSucursal)
                            .Select(s => s.montoMinimo)?.First();

                        if (montoMinimo == null)
                        {
                            montoMinimo = 0;
                        }
                        if (entradas.pedido.total >= montoMinimo)
                        {

                            if (entradas.pedido.tipoReparto == 1)
                            {
                                recogerSucursal = true;
                            }
                            else
                            {
                                var outResultadoParameter = new ObjectParameter("resultado", typeof(string));
                                int dia = (int)(DateTime.Now.DayOfWeek);
                                var result = Contexto.SP_Verificar_Datos_Entrega_Pedido(entradas.pedido.restaurante.idSucursal, dia, entradas.direccionEntrega.direccion.nombreColonia, entradas.direccionEntrega.direccion.codigoPostal, entradas.direccionEntrega.ubicacion.latitude, entradas.direccionEntrega.ubicacion.longitud).FirstOrDefault();

                                char delimiter = ';';
                                string[] resultados = result.resultado.Split(delimiter);//outResultadoParameter.Value.ToString().Split(delimiter);
                                if (resultados[0].Equals("ERROR"))
                                {
                                    conCobertura = false;
                                    respuesta.respuesta.pedidoId = 0;
                                    respuesta.estatusPeticion = RespuestaErrorValidacion(resultados[1]);
                                }
                                else
                                {
                                    direccionEntrega = entradas.direccionEntrega.direccion.calle + " # " + entradas.direccionEntrega.direccion.numeroExt + " Colonia: " + entradas.direccionEntrega.direccion.nombreColonia + " CP: " + entradas.direccionEntrega.direccion.codigoPostal + " " + entradas.direccionEntrega.direccion.referencia;
                                }
                            }
                        }
                        else
                        {
                            conCobertura = false;
                            respuesta.respuesta.pedidoId = 0;
                            respuesta.estatusPeticion = RespuestaErrorValidacion($"El monto mínimo de compra es: {montoMinimo}");
                        }

                        if (conCobertura)
                        {
                            RegistrarPedidoBd(entradas, direccionEntrega, recogerSucursal, respuesta);
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

        private RespuestaEjecucionAplicarCargo RealizarCargoOpenPay(int consumidorId, string customerId, string sourceId, decimal monto, string descripcion, string ordenId, string deviceSessionID)
        {
            EnviarLlamadosOpenPay ejecutarOp = new EnviarLlamadosOpenPay();
            RespuestaEjecucionAplicarCargo respuesta = new RespuestaEjecucionAplicarCargo();
            OpenPayController agregaBitacora = new OpenPayController();
            respuesta = ejecutarOp.AplicarCargoCliente(customerId, sourceId, monto, descripcion, ordenId, deviceSessionID);

            agregaBitacora.InsertarBitacoraTransaccionOpenPay(consumidorId, 4, respuesta.datosEnvio, respuesta.datosRespuesta, respuesta.codigoError, respuesta.descripcionError);
            return respuesta;
        }        

        private bool RegistrarPedidoBd(RequestPedido entradas, string direccionEntrega, bool recogerSucursal, ResponseBasePedido respuesta)
        {
            #region RegistrarPedidoBd
            bool resultado = false;
            bool guardarPedido = false;

            var consumidor = Contexto.clientes.Include(path: w => w.ConsumidoresConekta).FirstOrDefault(predicate: w => w.idCliente == entradas.consumidorId);

            var nombreCompleto = consumidor.nombre + ' ' + consumidor.paterno + ' ' + consumidor.materno;
            using (var tx = Contexto.Database.BeginTransaction())
            {
                try
                {
                    var Pedido = Contexto.Pedidos1.Add(entity: new Pedidos1
                    {
                        sucursalId = entradas.pedido.restaurante.idSucursal,
                        fechaRegistro = DateTime.Now,
                        catPedidoEstatusId = 1,
                        costoEnvio = recogerSucursal == true ? 0 : entradas.pedido.restaurante.costoEnvio,
                        subTotal = entradas.pedido.subTotal,
                        totalPagar = entradas.pedido.total,
                        catMetodoPagoId = entradas.formaPago.metodo,
                        consumidorId = entradas.consumidorId,
                        consumidorDireccionId = entradas.solicitudPorAgente == true ? null : entradas.direccionEntrega.direccion != null ? entradas.direccionEntrega.direccion.direccionId : null,
                        nombreQuienRealizo = nombreCompleto,
                        direccionEntrega = direccionEntrega,
                        quienRecibe = entradas.clienteRecibe.nombreCompleto,
                        telefonoQuienRecibe = entradas.clienteRecibe.telefono,
                        recogerEnSucursal = recogerSucursal,
                        ultimaNotificacionSucursal = DateTime.Now,
                        SolicitudPorAgente = entradas.solicitudPorAgente,
                        ClienteCallCenterID = entradas.clienteRecibe.clienteId
                    });

                    var listaPlatillos = entradas.pedido.platillos;
                    if (listaPlatillos.Count() > 0)
                    {
                        if (listaPlatillos.ElementAt(index: 0).platilloId > 0)
                        {
                            guardarPedido = true;

                            for (int x = 0; x < listaPlatillos.Count(); x++)
                            {
                                var InsertaDetallePedido = Contexto.DetallePedidos.Add(entity: new DetallePedidos
                                {
                                    Pedidos1 = Pedido,
                                    platilloId = listaPlatillos.ElementAt(index: x).platilloId,
                                    cantidad = 1,
                                    precioUnitario = listaPlatillos.ElementAt(index: x).precio,
                                    descripcion = listaPlatillos.ElementAt(index: x).descripcion + " : " + listaPlatillos.ElementAt(index: x).contenido,
                                    nota = listaPlatillos.ElementAt(index: x).notas
                                });
                                Contexto.DetallePedidos.Add(entity: InsertaDetallePedido);
                            }
                        }
                    }

                    var listaPlatillosEnsaladas = entradas.pedido.ensaladas;
                    if (listaPlatillosEnsaladas.Count() > 0)
                    {
                        if (listaPlatillosEnsaladas.ElementAt(index: 0).platilloId > 0)
                        {
                            guardarPedido = true;
                            for (int x = 0; x < listaPlatillosEnsaladas.Count(); x++)
                            {
                                var InsertaDetallePedido = Contexto.DetallePedidos.Add(entity: new DetallePedidos
                                {
                                    Pedidos1 = Pedido,
                                    platilloId = listaPlatillosEnsaladas.ElementAt(index: x).platilloId,
                                    cantidad = 1,
                                    precioUnitario = listaPlatillosEnsaladas.ElementAt(index: x).precio,
                                    descripcion = listaPlatillosEnsaladas.ElementAt(index: x).descripcion + " : " + listaPlatillosEnsaladas.ElementAt(index: x).contenido,
                                    nota = listaPlatillosEnsaladas.ElementAt(index: x).notas
                                });
                                Contexto.DetallePedidos.Add(entity: InsertaDetallePedido);
                            }
                        }
                    }

                    if (guardarPedido)
                    {
                        bool guardar = true;
                        RespuestaEjecucionAplicarCargo respuestaCargo = new RespuestaEjecucionAplicarCargo();
                        if (entradas.formaPago.metodo == 1)
                        {
                            StringBuilder generarOrden = new StringBuilder();
                            generarOrden.Append(value: entradas.formaPago.idTarjeta);
                            generarOrden.Append(value: "|");
                            generarOrden.Append(value: DateTime.Now.ToString(format: "yyyyMMddHHmm"));
                            //Realizar cargo OPENPAY
                            // respuestaCargo = RealizarCargoOpenPay(consumidor.idConsumidor,consumidor.ConsumidorOpenPay.ElementAt(0).customerId, entradas.formaPago.idTarjeta, entradas.pedido.total, "Consumo", generarOrden.ToString(), entradas.formaPago.idSesion);
                            //guardar = respuestaCargo.resultado;
                            //Realizar cargo CONEKTA
                            if (recogerSucursal)
                            {

                                var direEntrega = Contexto.sucursales.Where(predicate: w => w.idSucursal == entradas.pedido.restaurante.idSucursal).Select(selector: s => s.direccion).FirstOrDefault();
                                var controller = new ConektaController();
                                var direccionSucursal = direEntrega.calle + " " + direEntrega.numExterior + " " + direEntrega.numExterior;
                                var orden = controller.CreateOrderConekta(customerId: consumidor.ConsumidoresConekta.First().customerId,
                                    unitPrice: entradas.pedido.total,
                                    tokenId: entradas.formaPago.idTarjeta,
                                    nombreContacto: nombreCompleto,
                                    direccionContacto: direccionSucursal,
                                    telefonoContacto: consumidor.telefono,
                                    cobroEnvio: 0,
                                    portador: nombreCompleto,
                                    codigoPostal: direEntrega.codigoPostal,
                                    colonia: direEntrega.colonia,
                                    ciudad: direEntrega.catCiudades.ciudadDescr ?? "Mexicali",
                                    estado: direEntrega.catEstados.estadoDescr ?? "Baja California");

                                guardar = !string.IsNullOrEmpty(orden);

                                Contexto.PedidosConekta.Add(new PedidosConekta
                                {
                                    Pedidos1 = Pedido,
                                    estatusConekta = (int)PedidoTarjetaEstatus.PreAutorizado,
                                    fechaActualizacion = DateTime.Now,
                                    idOrdenPedido = orden
                                });
                                // guardar = controller.CreateOrderConekta(consumidor.ConsumidoresConekta.First().customerId, entradas.pedido.total, entradas.formaPago.idTarjeta, nombreCompleto, direccionEntrega, consumidor.Usuarios.telefono, 0, nombreCompleto, direEntrega.Direcciones.codigoPostal, direEntrega.Direcciones.CatColonias.descripcion, direEntrega.Direcciones.CatCiudades.ciudadDescr ?? "Mexicali", direEntrega.Direcciones.CatEstados.estadoDescr ?? "Baja California");
                            }
                            else
                            {
                                var direEntrega = Contexto.ConsumidorDirecciones
                                    .FirstOrDefault(predicate: w => w.idConsumidorDireccion == entradas.direccionEntrega.direccion.direccionId);
                                var controller = new ConektaController();
                                var orden = controller.CreateOrderConekta(
                                    customerId: consumidor.ConsumidoresConekta.First().customerId,
                                    unitPrice: entradas.pedido.subTotal, tokenId: entradas.formaPago.idTarjeta,
                                    nombreContacto: nombreCompleto, direccionContacto: direccionEntrega,
                                    telefonoContacto: consumidor.telefono,
                                    cobroEnvio: entradas.pedido.restaurante.costoEnvio, portador: nombreCompleto,
                                    codigoPostal: direEntrega.codigoPostal, colonia: direEntrega.nombreColonia,
                                    ciudad: direEntrega.catCiudades.ciudadDescr ?? "Mexicali",
                                    estado: direEntrega.catEstados.estadoDescr ?? "Baja California");

                                guardar = !string.IsNullOrEmpty(orden);

                                Contexto.PedidosConekta.Add(new PedidosConekta
                                {
                                    Pedidos1 = Pedido,
                                    estatusConekta = (int)PedidoTarjetaEstatus.PreAutorizado,
                                    fechaActualizacion = DateTime.Now,
                                    idOrdenPedido = orden
                                });
                            }


                            Pedido.ordenOP = generarOrden.ToString();
                        }

                        if (guardar)
                        {
                            var InsertarSeguimiento = Contexto.SeguimientoPedidos.Add(entity: new SeguimientoPedidos
                            {
                                Pedidos1 = Pedido,
                                fechaRegistro = DateTime.Now,
                                catPedidoEstatusId = 1
                            });

                            Contexto.SeguimientoPedidos.Add(entity: InsertarSeguimiento);

                            Contexto.SaveChanges();
                            respuesta.respuesta.pedidoId = Pedido.idPedido;
                            respuesta.estatusPeticion = RespuestaOk;

                            Hangfire.BackgroundJob.Enqueue(() => CancelacionAutomatica.NotificarPedidoNuevo(Pedido.idPedido));

                            if (int.TryParse(ConfigurationManager.AppSettings["MINUTOS_VIDA_PEDIDOS_DESATENDIDOS"],
                                out var tiempoVida))
                            {
                                Hangfire.BackgroundJob.Schedule(() => CancelacionAutomatica.CancelarPedido(Pedido.idPedido), TimeSpan.FromMinutes(tiempoVida));
                            }
                            tx.Commit();
                        }
                        else
                        {
                            respuesta.respuesta.pedidoId = Pedido.idPedido;
                            respuesta.estatusPeticion = RespuestaErrorValidacion(errors: respuestaCargo.descripcionError);
                            tx.Rollback();

                        }
                    }
                    else
                    {
                        respuesta.respuesta.pedidoId = 0;
                        respuesta.estatusPeticion = RespuestaErrorValidacion(errors: "No se encontraron platillos.");
                        tx.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(message: ex);

                    respuesta.estatusPeticion = RespuestaErrorInterno;
                    tx.Rollback();
                }
            }
            return resultado; 
            #endregion
        }
        
        [Route("api/hazPedido/ObtenerHistorialPedidos")]
        public ResponseHistorialPedido ObtenerHistorialPedidos([FromBody]RequestHistorialPedido entradas)
        {
            ResponseHistorialPedido respuesta = new ResponseHistorialPedido();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var datosPedido = Contexto.Pedidos1.OrderByDescending(o => o.fechaRegistro).Where(w => w.consumidorId == entradas.consumidorId && w.catPedidoEstatusId == 6).Select(c => new ResponseDatosHistorialPedido
                        {
                            pedidoId = c.idPedido,
                            folio = c.folio,
                            fecha = c.fechaRegistro,
                            montoCompra = c.totalPagar,
                            nombreSucursal = c.sucursales.nombre,
                            calificacionPedido = c.calificacionPedido
                        }).ToList();

                        respuesta.respuesta = new List<ResponseDatosHistorialPedido>();
                        respuesta.respuesta = datosPedido;
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
        

        [Route("api/hazPedido/ObtenerPedidosActivos")]
        public ResponsePedidoActivo ObtenerPedidosActivos([FromBody]RequestPedidoActivo entradas)
        {
            ResponsePedidoActivo respuesta = new ResponsePedidoActivo();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var datosPedido = Contexto.Pedidos1.OrderByDescending(o => o.fechaRegistro).Where(w => w.consumidorId == entradas.consumidorId && w.catPedidoEstatusId < 6).ToList();

                        var listaPedidos = new List<ResponseDatosPedidoActivo>();
                        respuesta.respuesta = listaPedidos;

                        int pedidoBuscar = 0;

                        for (int x = 0; x < datosPedido.Count(); x++)
                        {
                            var pedidoActivo = new ResponseDatosPedidoActivo();

                            pedidoActivo.pedidoId = datosPedido.ElementAt(x).idPedido;
                            pedidoActivo.folio = datosPedido.ElementAt(x).folio;
                            pedidoActivo.fecha = datosPedido.ElementAt(x).fechaRegistro;
                            pedidoActivo.montoCompra = datosPedido.ElementAt(x).totalPagar;
                            pedidoActivo.nombreSucursal = datosPedido.ElementAt(x).sucursales.nombre;
                            pedidoActivo.estatus = datosPedido.ElementAt(x).CatPedidoEstatus.etiquetaEstatus;
                            pedidoActivo.estatusId = datosPedido.ElementAt(x).CatPedidoEstatus.estatusAplicacionId;
                            pedidoActivo.metodoPagoId = datosPedido.ElementAt(x).catMetodoPagoId;

                            pedidoBuscar = datosPedido.ElementAt(x).idPedido;

                            var bitacoraPedidos = Contexto.SeguimientoBitacoraPedidos.Where(w => w.SeguimientoPedidos.pedidoId == pedidoBuscar).Select(c => new ResponseBitacoraPedidoActivo
                            {
                                fecha = c.fechaRegistro,
                                comentario = c.comentario
                            }).OrderByDescending(o => o.fecha).ToList();

                            pedidoActivo.bitacoraPedido = bitacoraPedidos;
                            listaPedidos.Add(pedidoActivo);
                        }
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


        [Route("api/hazPedido/ObtenerInformacionPedido")]
        public ResponseInfoPedido ObtenerInformacionPedido([FromBody]RequestInformacionPedido entradas)
        {
            ResponseInfoPedido respuesta = new ResponseInfoPedido();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var detallePedido = Contexto.Pedidos1.Where(w => w.idPedido == entradas.pedidoId).Select(s => new ResponseListadoInformacionPedido
                        {
                            nombreSucursal = s.sucursales.nombre,
                            fecha = s.fechaRegistro,
                            pedidoId = s.idPedido,
                            folio = s.folio,
                            total = s.totalPagar,
                            subtotal = s.subTotal,
                            costoEnvio = s.costoEnvio,
                            estatusId = s.CatPedidoEstatus.estatusAplicacionId,
                            estatus = s.CatPedidoEstatus.etiquetaEstatus,
                            detallePedido = s.DetallePedidos.Select(r => new ResponseDetalleInformacionPedido
                            {
                                platilloId = r.platilloId,
                                nombrePlatillo = r.Productos.nombre,
                                descripcion = r.descripcion,
                                precio = r.precioUnitario,
                                urlImagen = _hostImagenes + r.Productos.urlImagen
                            }).ToList()
                        }).FirstOrDefault();

                        var bitacoraPedidos = Contexto.SeguimientoBitacoraPedidos.Where(w => w.SeguimientoPedidos.pedidoId == entradas.pedidoId).Select(c => new ResponseBitacoraPedidoActivo
                        {
                            fecha = c.fechaRegistro,
                            comentario = c.comentario
                        }).OrderByDescending(o => o.fecha).ToList();

                        respuesta.respuesta = new ResponseListaInformacionPedido();

                        // respuesta.respuesta.bitacoraPedido = bitacoraPedidos;
                        detallePedido.bitacoraPedido = bitacoraPedidos;
                        respuesta.respuesta.informacion = detallePedido;
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


        [Route("api/hazPedido/CalificarPedido")]
        public ErrorObjCodeResponseBase CalificarPedido([FromBody]RequestCalificarPedido entradas)
        {
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var pedido = Contexto.Pedidos1.Where(w => w.idPedido == entradas.pedidoId).FirstOrDefault();

                        pedido.calificacionAplicacion = entradas.calMovil;
                        pedido.calificacionPedido = entradas.calProducto;
                        pedido.calificacionServicio = entradas.calReparticion;
                        pedido.calificacionComentario = entradas.comentario;

                        Contexto.SaveChanges();
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

        [Route("api/hazPedido/RegistrarMensajePedido")]
        public ErrorObjCodeResponseBase RegistrarMensajePedido([FromBody]RequestPedidoInsertarMensaje entradas)
        {
            ErrorObjCodeResponseBase respuesta = new ErrorObjCodeResponseBase();
            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var seguimientoPedido = Contexto.SeguimientoPedidos.Where(w => w.pedidoId == entradas.pedidoId).OrderByDescending(o => o.catPedidoEstatusId).FirstOrDefault();

                        InsertarSeguimientoBitacoraPedido(seguimientoPedido, entradas.consumidorId, entradas.mensaje, respuesta, true);
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

        internal bool InsertarSeguimientoBitacoraPedido(SeguimientoPedidos seguimientoPedido, int usuarioRegistro, string mensaje, ErrorObjCodeResponseBase respuesta, bool isConsumidor)
        {
            bool resultado = false;

            try
            {
                var segBitacoraPedido = new SeguimientoBitacoraPedidos
                {
                    seguimientoPedidoId = seguimientoPedido.idSeguimientoPedido,
                    comentario = mensaje,
                    isconsumidor = isConsumidor,
                    usuarioIdRegistro = seguimientoPedido.Pedidos1.clientes.idCliente,
                    fechaRegistro = DateTime.Now
                };
                Contexto.SeguimientoBitacoraPedidos.Add(segBitacoraPedido);
                Contexto.SaveChanges();

                respuesta.estatusPeticion = RespuestaOk;

                Contexto.SaveChanges();
                resultado = true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                respuesta.estatusPeticion = RespuestaErrorInterno;
            }
            return resultado;
        }
        
        internal int ObtenerNumeroPedidosActivos(int consumidorId)
        {
            int resultado = 0;

            try
            {
                var datosPedido = Contexto.Pedidos1.OrderByDescending(o => o.fechaRegistro).Where(w => w.consumidorId == consumidorId && w.catPedidoEstatusId < 6).ToList();
                resultado = datosPedido.Count();
            }
            catch (Exception e)
            {
                Logger.Error(e);

            }
            return resultado;
        }


        //TODO crear estructura que usara punto de venta para registro en automatico al aceptar pedidos levantados por la app (estatus pendientes)
        [Route("api/hazPedido/ObtenerPedidoPuntoVenta")]
        public ResponsePedidoStructPuntoVenta ObtenerPedidoStructPuntoVenta([FromBody]RequestInformacionPedidoPuntoVenta entradas)
        {
            #region ObtenerPedidoStructPuntoVenta
            ResponsePedidoStructPuntoVenta respuesta = new ResponsePedidoStructPuntoVenta();

            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid)
                    {
                        var datosPedido = Contexto.Pedidos1.OrderByDescending(o => o.fechaRegistro).Where(w => w.idPedido == entradas.pedidoId /*&& w.catPedidoEstatusId == 6*/)
                        .FirstOrDefault();

                        List<Producto> listProducto = new List<Producto>();
                        foreach (DetallePedidos item in datosPedido.DetallePedidos)
                        {
                            List<DetalleCombo> listCombo = new List<DetalleCombo>();
                            string[] ingredientesCombo = item.descripcion.Split(',');
                            bool isCombo = ingredientesCombo.Count() > 1 ? true : false;

                            if (isCombo)
                            {
                                Extra extra = new Extra()
                                {
                                    areaPreparacionId = item.Productos.areaPreparacionId,
                                    categoriaProductoId = item.Productos.categoriaProductoId,
                                    clave = item.Productos.clave,
                                    esCombo = true,
                                    esEnsalada = false,
                                    fechaRegistro = DateTime.Now,
                                    id = item.platilloId,
                                    imagen = "", //TODO Se envia un byteArray de la imagen (del producto), revisar si es necesario este valor
                                    indice = item.Productos.indice,
                                    nombre = item.Productos.nombre,
                                    precio = item.precioUnitario.ToString(),
                                    principal = false,
                                    tieneVariedad = false
                                };

                                foreach (string cadena in ingredientesCombo)
                                {
                                    List<Opciones> listOpciones = new List<Opciones>();
                                    string[] ingrediente = cadena.Split(':');
                                    listOpciones.Add(new Opciones()
                                    {
                                        id = 0, //TODO buscar de donde se obtiene este valor
                                        nombre = ingrediente[1].Trim(),
                                        seleccionados = 1
                                    });

                                    listCombo.Add(new DetalleCombo()
                                    {
                                        agrupador = 0, //TODO buscar de donde se obtiene este valor... [ads].[AgrupadorInsumos]
                                        confirmarPorSeparado = false,
                                        extra = extra,
                                        maximo = 1,
                                        opciones = listOpciones,
                                        puedeAgregarExtra = false,
                                        titulo = ingrediente[0].Trim()
                                    });
                                }

                                listProducto.Add(new Producto()
                                {
                                    cantidad = 1,
                                    nombre = item.Productos.nombre,
                                    precio = item.precioUnitario ?? 0,
                                    producto = item.platilloId,
                                    combo = isCombo,
                                    detalleCombo = listCombo,                                    
                                });
                            }
                            else
                            {
                                listProducto.Add(new Producto()
                                {
                                    cantidad = 1,
                                    nombre = item.Productos.nombre,
                                    precio = item.precioUnitario ?? 0,
                                    producto = item.platilloId,
                                    combo = isCombo
                                });
                            }
                        }

                        respuesta.productos = listProducto;
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
    }
}
