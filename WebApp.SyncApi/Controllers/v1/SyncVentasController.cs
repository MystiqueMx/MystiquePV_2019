using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MystiqueMC.DAL;
using WebApp.SyncApi.Helpers.Base;
using WebApp.SyncApi.Models.Requests;
using WebApp.SyncApi.Models.Responses;
using System.Data;
using System.Data.Entity;


namespace WebApp.SyncApi.Controllers
{
    [Authorize]
    [Route("api/v1/ventas")]
    public class SyncVentasController : BaseApiController
    {

        [HttpPost]
        public ResponseVentas RegistrarVentasSucursal(RequestVentas entradas)
        {
            ResponseVentas respuesta = new ResponseVentas();
            List<int> ventaIdList = new List<int>();

            try
            {
                if (IsAppSecretValid)
                {
                    if (ModelState.IsValid && entradas != null)
                    {
                        var sucursales = Contexto.ConfUsuarioSucursales.Include(i => i.sucursales).Where(w => w.usuarioId == CurrentUserId).FirstOrDefault();

                        if (sucursales != null)
                        {
                            var valorSucursalId = sucursales.sucursalId;
                            List<EntidadVentas> ListadoVentas = new List<EntidadVentas>();
                            List<EntidadAperturas> ListadoAperturas = new List<EntidadAperturas>();
                            respuesta.respuesta = new ResponseEntidadesVentas();
                            bool insertar = false;

                            using (var tx = Contexto.Database.BeginTransaction())
                            {
                                try
                                {
                                    DateTime fechaRegistroNube = DateTime.Now;
                                    var listaVentas = entradas.venta;

                                    if (listaVentas.Count() > 0)
                                    {
                                        foreach (var itemVentas in listaVentas)
                                        {
                                            var insertarVenta = Contexto.Ventas.Where(w => w.sucursalId == valorSucursalId && w.ventaIdSucursal == itemVentas.id).FirstOrDefault();
                                            var uidVenta = "";

                                            if (insertarVenta == null)
                                            {
                                                insertar = true;
                                                uidVenta = Guid.NewGuid().ToString();
                                                insertarVenta = Contexto.Ventas.Add(new Ventas
                                                {
                                                    sucursalId = valorSucursalId,
                                                    ventaIdSucursal = itemVentas.id,
                                                    activo = itemVentas.activo,
                                                    fechaRegistroVenta = itemVentas.fechaRegistro,
                                                    fechaInicial = itemVentas.fechaInicial,
                                                    fechaFinal = itemVentas.fechaFinal,
                                                    fechaRegistro = fechaRegistroNube,
                                                    uuidVenta = uidVenta
                                                });
                                            }
                                            else
                                            {
                                                uidVenta = insertarVenta.uuidVenta;
                                            }

                                            var insertarEntidadVentas = new EntidadVentas
                                            {
                                                id = itemVentas.id,
                                                uuidVenta = uidVenta,
                                            };

                                            ListadoVentas.Add(insertarEntidadVentas);

                                            if (itemVentas.Aperturas.Count() > 0)
                                            {
                                                foreach (var itemAperturas in itemVentas.Aperturas.Where(w => w.activo == false).OrderBy(o => o.fechaRegistro))
                                                {
                                                    var insertarApertura = Contexto.Aperturas.Where(w => w.aperturaIdSucursal == itemAperturas.id && w.ventaId == insertarVenta.idVenta).FirstOrDefault();
                                                    var uidApertura = "";

                                                    if (insertarApertura == null)
                                                    {
                                                        insertar = true;
                                                        uidApertura = Guid.NewGuid().ToString();
                                                        insertarApertura = Contexto.Aperturas.Add(new Aperturas
                                                        {
                                                            aperturaIdSucursal = itemAperturas.id,
                                                            activo = itemAperturas.activo,
                                                            fechaRegistroApertura = itemAperturas.fechaRegistro,
                                                            fechaInicial = itemAperturas.fechaInicial,
                                                            fechaFinal = itemAperturas.fechaFinal,
                                                            tipoCambio = itemAperturas.tipoCambio,
                                                            fondo = itemAperturas.fondo,
                                                            uuidApertura = uidApertura,
                                                            Ventas = insertarVenta,
                                                            usuarioRegistro = String.Concat(itemAperturas.empleados.nombre, " ", itemAperturas.empleados.apellidos),
                                                            usuarioAutorizo = String.Concat(itemAperturas.autorizo.nombre, " ", itemAperturas.autorizo.apellidos),
                                                            fechaRegistro = fechaRegistroNube
                                                        });

                                                        var insertarEntidadApertura = new EntidadAperturas
                                                        {
                                                            id = itemAperturas.id,
                                                            uuidApertura = uidApertura,
                                                        };
                                                        ListadoAperturas.Add(insertarEntidadApertura);


                                                        if (itemAperturas.Retiros != null)
                                                            if (itemAperturas.Retiros.Count() > 0)
                                                            {
                                                                foreach (var itemRetiros in itemAperturas.Retiros)
                                                                {
                                                                    var insertarRetiros = Contexto.Retiros.Add(new Retiros
                                                                    {
                                                                        Aperturas = insertarApertura,
                                                                        retiroIdSucursal = itemRetiros.id,
                                                                        fechaRegistro = itemRetiros.fechaRegistro,
                                                                        monto = itemRetiros.monto,
                                                                        observaciones = itemRetiros.observaciones,
                                                                        usuarioRegistro = String.Concat(itemRetiros.Usuario.nombre, " ", itemRetiros.Usuario.apellidos),
                                                                    });
                                                                }
                                                            }


                                                        if (itemAperturas.Gastos != null)
                                                            if (itemAperturas.Gastos.Count() > 0)
                                                            {
                                                                foreach (var itemGastos in itemAperturas.Gastos)
                                                                {
                                                                    var insertarGastos = Contexto.GastosPv.Add(new GastosPv
                                                                    {
                                                                        Aperturas = insertarApertura,
                                                                        gastoIdSucursal = itemGastos.id,
                                                                        fechaRegistro = itemGastos.fechaRegistro,
                                                                        monto = itemGastos.monto,
                                                                        observaciones = itemGastos.observaciones,
                                                                        usuarioRegistro = String.Concat(itemGastos.Usuario.nombre, " ", itemGastos.Usuario.apellidos),
                                                                        tipoGasto = itemGastos.TiposGasto.nombre,
                                                                    });
                                                                }
                                                            }

                                                        if (itemAperturas.Pedidos != null)
                                                            if (itemAperturas.Pedidos.Count() > 0)
                                                            {
                                                                foreach (var itemPedidos in itemAperturas.Pedidos)
                                                                {

                                                                    var insertarPedidos = Contexto.Pedidos.Add(new Pedidos
                                                                    {
                                                                        Aperturas = insertarApertura,
                                                                        pedidoIdSucursal = itemPedidos.id,
                                                                        estatusPedido = itemPedidos.PedidoEstatus.nombre,
                                                                        tipoPedido = itemPedidos.PedidoTipo.nombre,
                                                                        fechaRegistroPedido = itemPedidos.fechaRegistro,
                                                                        usuarioRegistro = String.Concat(itemPedidos.Usuario.nombre, " ", itemPedidos.Usuario.apellidos),
                                                                        fechaRegistro = fechaRegistroNube,
                                                                        uuidPedido = Guid.NewGuid().ToString(),
                                                                        mesero_numero = itemPedidos.mesero_numero,
                                                                        mesero_nombre = itemPedidos.mesero_nombre,
                                                                        mesa_nombre = itemPedidos.mesa_nombre
                                                                    });


                                                                    if (itemPedidos.ConsumoEmpleados != null)
                                                                        if (itemPedidos.ConsumoEmpleados.Count() > 0)
                                                                            foreach (var itemConsumoEmpleado in itemPedidos.ConsumoEmpleados)
                                                                            {
                                                                                var insertarConsumoEmpleado = Contexto.ConsumoEmpleados.Add(new MystiqueMC.DAL.ConsumoEmpleados
                                                                                {
                                                                                    Pedidos = insertarPedidos,
                                                                                    usuarioConsume = String.Concat(itemConsumoEmpleado.Empleado.nombre, " ", itemConsumoEmpleado.Empleado.apellidos),
                                                                                });
                                                                            }

                                                                    if (itemPedidos.SeguimientoPedidos != null)
                                                                        if (itemPedidos.SeguimientoPedidos.Count() > 0)
                                                                            foreach (var itemConsumoEmpleado in itemPedidos.SeguimientoPedidos)
                                                                            {
                                                                                var existeCliente = Contexto.Clientespv.Where(w => w.telefono == itemConsumoEmpleado.Clientes.telefono).FirstOrDefault();

                                                                                if(existeCliente != null)
                                                                                {
                                                                                    existeCliente.nombre = itemConsumoEmpleado.Clientes.nombre;
                                                                                    existeCliente.sucursalId = valorSucursalId;
                                                                                    existeCliente.coloniaid = itemConsumoEmpleado.Clientes.coloniaId;
                                                                                    existeCliente.direccionEntrega = itemConsumoEmpleado.Clientes.direccionEntrega;
                                                                                    existeCliente.otraColonia = itemConsumoEmpleado.Clientes.otraColonia;
                                                                                }
                                                                                else
                                                                                {
                                                                                    var insertarCliente = Contexto.Clientespv.Add(new Clientespv
                                                                                    {
                                                                                        sucursalId = valorSucursalId,
                                                                                        nombre = itemConsumoEmpleado.Clientes.nombre,
                                                                                        coloniaid = itemConsumoEmpleado.Clientes.coloniaId,
                                                                                        direccionEntrega = itemConsumoEmpleado.Clientes.direccionEntrega,
                                                                                        otraColonia = itemConsumoEmpleado.Clientes.otraColonia,
                                                                                        telefono = itemConsumoEmpleado.Clientes.telefono
                                                                                    });
                                                                                }                                                                                
                                                                            }


                                                                    if (itemPedidos.Tickets != null)
                                                                        if (itemPedidos.Tickets.Count() > 0)
                                                                            foreach (var itemPedidosTicket in itemPedidos.Tickets)
                                                                            {
                                                                                var insertarTicket = Contexto.Tickets.Add(new Tickets
                                                                                {
                                                                                    Pedidos = insertarPedidos,
                                                                                    ticketIdSucursal = itemPedidosTicket.id,
                                                                                    fechaCobro = itemPedidosTicket.fechaRegistro,
                                                                                    numeroTicket = itemPedidosTicket.numeroTicket,
                                                                                    folioTicket = itemPedidosTicket.folioTicket,
                                                                                    usuarioRegistro = String.Concat(itemPedidosTicket.Usuario.nombre, " ", itemPedidosTicket.Usuario.apellidos),
                                                                                    facturado = itemPedidosTicket.facturado,
                                                                                    subtotal = itemPedidosTicket.subTotal,
                                                                                    iva = itemPedidosTicket.iva,
                                                                                    importe = itemPedidosTicket.importe,
                                                                                    fechaCancelacion = itemPedidosTicket.fechaCancelacion,
                                                                                    uuidTicket = Guid.NewGuid().ToString(),
                                                                                    fechaRegistro = fechaRegistroNube,
                                                                                    estaCancelado = itemPedidosTicket.estaCancelado,
                                                                                    tasaIva = itemPedidosTicket.tasaIva,
                                                                                    
                                                                                });

                                                                                if (itemPedidosTicket.TicketsPagos != null)
                                                                                    if (itemPedidosTicket.TicketsPagos.Count() > 0)
                                                                                        foreach (var itemTicketPagos in itemPedidosTicket.TicketsPagos)
                                                                                        {
                                                                                            var insertarTicketPagos = Contexto.TicketPagos.Add(new TicketPagos
                                                                                            {
                                                                                                Tickets = insertarTicket,
                                                                                                tipoPago = itemTicketPagos.TiposPago.nombre,
                                                                                                importe = itemTicketPagos.importe,
                                                                                                confirmacion = itemTicketPagos.confirmacion,
                                                                                                banco = itemTicketPagos.Banco != null ? itemTicketPagos.Banco.nombre : "",
                                                                                                tipoPagoId = itemTicketPagos.catTipoPagoId,
                                                                                               importeOriginal = itemTicketPagos.importeOriginal,
                                                                                            });

                                                                                        }

                                                                                if (itemPedidosTicket.TicketsReimpresos != null)
                                                                                    if (itemPedidosTicket.TicketsReimpresos.Count() > 0)
                                                                                        foreach (var itemTicketReimpresos in itemPedidosTicket.TicketsReimpresos)
                                                                                        {
                                                                                            var insrtarTicketReimpresos = Contexto.TicketReimpresos.Add(new TicketReimpresos
                                                                                            {
                                                                                                Tickets = insertarTicket,
                                                                                                fechaRegistro = itemTicketReimpresos.fechaRegistro,
                                                                                                fechaRegistroTicketImpreso = fechaRegistroNube,
                                                                                                usuarioAutorizo = String.Concat(itemTicketReimpresos.autorizoTicketReimpreso.nombre, " ", itemTicketReimpresos.autorizoTicketReimpreso.apellidos),
                                                                                                usuarioRegistro = String.Concat(itemTicketReimpresos.empleadosTicketReimpreso.nombre, " ", itemTicketReimpresos.empleadosTicketReimpreso.apellidos),
                                                                                            });

                                                                                        }

                                                                                if (itemPedidosTicket.TicketCancelados != null)
                                                                                    if (itemPedidosTicket.TicketCancelados.Count() > 0)
                                                                                        foreach (var itemTicketCancelados in itemPedidosTicket.TicketCancelados)
                                                                                        {
                                                                                            var insrtarTicketCancelados = Contexto.TicketCancelados.Add(new MystiqueMC.DAL.TicketCancelados
                                                                                            {
                                                                                                Tickets = insertarTicket,
                                                                                                fechaRegistro = itemTicketCancelados.fechaRegistro,
                                                                                                motivo = itemTicketCancelados.motivo,
                                                                                                usuarioRegistro = String.Concat(itemTicketCancelados.usuario.nombre, " ", itemTicketCancelados.usuario.apellidos),
                                                                                                usuarioAutorizo = String.Concat(itemTicketCancelados.autorizoTicketCancelado.nombre, " ", itemTicketCancelados.autorizoTicketCancelado.apellidos),                                                                                                
                                                                                            });

                                                                                        }

                                                                                if (itemPedidosTicket.TicketDescuentos != null)
                                                                                    if (itemPedidosTicket.TicketDescuentos.Count() > 0)
                                                                                        foreach (var itemTicketDescuentos in itemPedidosTicket.TicketDescuentos)
                                                                                        {
                                                                                            var insrtarTicketDescuentos = Contexto.TicketDescuentos.Add(new MystiqueMC.DAL.TicketDescuentos
                                                                                            {
                                                                                                Tickets = insertarTicket,
                                                                                                fechaRegistro = itemTicketDescuentos.fechaRegistro,
                                                                                                monto = itemTicketDescuentos.monto,
                                                                                                descuentoId = itemTicketDescuentos.descuentoId,
                                                                                            });

                                                                                        }

                                                                                if (itemPedidos.PedidosProductos != null)
                                                                                    if (itemPedidos.PedidosProductos.Count() > 0)
                                                                                        foreach (var itemPedidosProductos in itemPedidos.PedidosProductos)
                                                                                        {

                                                                                            var insertarPedidosProductos = Contexto.PedidoProductos.Add(new PedidoProductos
                                                                                            {
                                                                                                Pedidos = insertarPedidos,
                                                                                                productoId = itemPedidosProductos.productoId,
                                                                                                cantidad = itemPedidosProductos.cantidad,
                                                                                                Tickets = insertarTicket,
                                                                                                usuarioRegistro = String.Concat(itemPedidosProductos.Usuario.nombre, " ", itemPedidosProductos.Usuario.apellidos),
                                                                                                nota = itemPedidosProductos.notas,
                                                                                                precioUnitario = itemPedidosProductos.precioUnitario
                                                                                            });

                                                                                            if (itemPedidosProductos.PedidosProductoDetalles != null)
                                                                                                if (itemPedidosProductos.PedidosProductoDetalles.Count() > 0)
                                                                                                    foreach (var itemPedidosProductosDetalle in itemPedidosProductos.PedidosProductoDetalles)
                                                                                                    {
                                                                                                        var insertarPedidosProductosDetalle = Contexto.PedidoProductoDetalle.Add(new PedidoProductoDetalle
                                                                                                        {
                                                                                                            PedidoProductos = insertarPedidosProductos,
                                                                                                            insumoProductoId = itemPedidosProductosDetalle.insumoProductoId,
                                                                                                            descripcionVariedad = itemPedidosProductosDetalle.variedad,
                                                                                                            pedidoProductoId = itemPedidosProductosDetalle.pedidoProductoId
                                                                                                        });

                                                                                                        if (itemPedidosProductosDetalle.InsumosProducto != null)
                                                                                                        {
                                                                                                            var insertarInsumoProducto = Contexto.InsumoProducto.Add(new InsumoProducto
                                                                                                            {
                                                                                                                PedidoProductos = insertarPedidosProductos,
                                                                                                                PedidoProductoDetalle = insertarPedidosProductosDetalle,
                                                                                                                pedidoProductoDetalleId = insertarPedidosProductosDetalle.idPedidoProductoDetalle,
                                                                                                                insumoId = itemPedidosProductosDetalle.InsumosProducto.insumoId,
                                                                                                                pedidoProductoId = itemPedidosProductosDetalle.pedidoProductoId,
                                                                                                                productoId = itemPedidosProductosDetalle.InsumosProducto.productoId

                                                                                                            });
                                                                                                        }
                                                                                                    }
                                                                                        }
                                                                            }
                                                                }
                                                            }



                                                    } // if (insertarApertura == null)
                                                    else
                                                    {  // Apertura ya estaba registrada                                                        
                                                        var insertarEntidadApertura = new EntidadAperturas
                                                        {
                                                            id = itemAperturas.id,
                                                            uuidApertura = insertarApertura.uuidApertura,
                                                        };
                                                        ListadoAperturas.Add(insertarEntidadApertura);
                                                    }
                                                }
                                            }
                                            if (insertar)
                                            {
                                                Contexto.SaveChanges();
                                                if (insertarVenta.Aperturas.Any())
                                                {
                                                    ventaIdList.Add(insertarVenta.idVenta);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    { // no existen ventas
                                        respuesta.EstatusPeticion = RespuestaErrorValidacion("No vienen Ventas en los datos.");
                                    }
                                    if (insertar)
                                    {
                                        Contexto.SaveChanges();
                                        tx.Commit();
                                    }
                                    respuesta.EstatusPeticion = RespuestaOk;
                                    respuesta.respuesta.ListadoEntidadVentas = ListadoVentas;
                                    respuesta.respuesta.ListadoEntidadAperturas = ListadoAperturas;
                                }
                                catch (System.Data.Entity.Validation.DbEntityValidationException)
                                {
                                    tx.Rollback();
                                    respuesta.EstatusPeticion = RespuestaErrorInterno;
                                }
                            }
                            /*
                            using (var tx = Contexto.Database.BeginTransaction())
                            {
                                try
                                {
                                    foreach (int id in ventaIdList)
                                    {
                                        Contexto.SP_ActualizarInventarios_Ventas(id);
                                    }
                                    Contexto.SaveChanges();
                                    tx.Commit();
                                }
                                catch (Exception exx)
                                {
                                    tx.Rollback();
                                }
                            }
                            */
                            string result = "";
                            try
                            {
                                foreach (int id in ventaIdList)
                                {
                                    var execute = Contexto.SP_ActualizarInventarios_Ventas(id);
                                    result += execute.FirstOrDefault() + "\n\n";
                                    result += id.ToString() + "\n\n";
                                }
                                Contexto.SaveChanges();
                            }
                            catch (Exception exx)
                            {
                                var error = result;
                            }
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