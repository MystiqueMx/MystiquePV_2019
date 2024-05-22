using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using conekta;
using Hangfire;
using log4net;
using MystiqueMC.DAL;
using MystiqueMC.Helpers.Emails;
using MystiqueMcApi.Controllers;
using MystiqueMcApi.Helpers.Email;
using MystiqueMcApi.Models.Pedidos;
//using Qdc.Api.Helpers.Email;
//using Qdc.Api.Helpers.OneSignal;
//using Qdc.Api.Models.Pedidos;
//using Qdc.Dal;
using RestSharp;
using RestSharp.Authenticators;

namespace MystiqueMcApi.Helpers.Hangfire.Pedidos
{
    [AutomaticRetry(Attempts = 5)]
    public static class CancelacionAutomatica
    {
        public static async Task NotificarPedidoNuevo(int idPedido)
        {
            var logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Debug($"Validando si el pedido fue atendido {idPedido}");
            using (var contexto = new MystiqueMeEntities())
            {
                var fechaActual = DateTime.Today;
                var pedido = await contexto.Pedidos1
                    .Include(c => c.sucursales)
                    .Include(c => c.clientes)
                    .FirstOrDefaultAsync(c => c.idPedido == idPedido);
                var ultimaActividadB = await contexto.BitacoraSucursalesActivas
                    .OrderByDescending(c => c.fechaRegistro)
                    .Where(c => fechaActual == DbFunctions.TruncateTime(c.fechaRegistro))
                    .FirstOrDefaultAsync();
                var ultimaActividad = ultimaActividadB?.fechaRegistro ?? (DateTime?)null;

                if (pedido == null) return;

                if (ConfigurationManager.AppSettings["EmailAdministrador"] is string email &&
                    !string.IsNullOrEmpty(email))
                {
                    var descUltimaActividad = "sin actividad registrada el día de hoy";
                    if (ultimaActividad.HasValue)
                    {
                        descUltimaActividad = $"{ultimaActividad.Value:HH:mm:ss}";
                    }

                    var mensaje =
                        $"Hay un nuevo pedido, restaurante: {pedido.sucursales.nombre}, teléfono del restaurante: {pedido.sucursales.telefono}, total del pedido: {pedido.totalPagar:C}, última actividad del restaurante: {descUltimaActividad}, comensal: {pedido.clientes.nombre} {pedido.clientes.paterno}, teléfono del comensal: {pedido.clientes.telefono}, email del comensal: {pedido.clientes.email}";
                    var @delegate = new SendEmailDelegate();
                    @delegate.SendEmailSSL(email, email, "Nuevo Pedido", mensaje, "qdc-noreply@grupored.com.mx");
                }
            }
        }
        public static async Task CancelarPedido(int idPedido)
        {
            var logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Debug($"Validando si el pedido fue atendido {idPedido}");
            using (var contexto = new MystiqueMeEntities())
            {
                var pedidoTest = contexto.Pedidos1.Find(idPedido);
                if (pedidoTest == null) return;


                if (pedidoTest.catPedidoEstatusId == (int)EstatusPedido.RECIBIDO)
                {
                    var pedido = await contexto.Pedidos1
                        .Include(c => c.sucursales)
                        .Include(c => c.PedidosConekta)
                        .Include(c => c.clientes)
                        .Include(c => c.clientes.login)
                        //.Include(c => c.Consumidores.Usuarios)
                        .FirstAsync(c => c.idPedido == idPedido);

                    logger.Info($"El pedido {idPedido} continua en estatus {(int)EstatusPedido.RECIBIDO}({EstatusPedido.RECIBIDO}) a las {DateTime.Now}, cancelando el pedido");

                    pedido.catPedidoEstatusId = (int)EstatusPedido.CANCELADO;
                    contexto.Entry(pedido).State = EntityState.Modified;
                    contexto.SaveChanges();

                    try
                    {
                        if (ConfigurationManager.AppSettings["EmailAdministrador"] is string email && !string.IsNullOrEmpty(email))
                        {
                            var mensaje = $"El pedido ha sido rechazado debido a que no fué atendido, restaurante: {pedido.sucursales.nombre}, teléfono del restaurante: {pedido.sucursales.telefono}, total del pedido: {pedido.totalPagar:C}, comensal: {pedido.clientes.nombre} {pedido.clientes.paterno}, teléfono del comensal: {pedido.clientes.telefono}, email del comensal: {pedido.clientes.email}";
                            var @delegate = new SendEmailDelegate();
                            @delegate.SendEmailSSL(email, email, "Pedido perdido", mensaje, "qdc-noreply@grupored.com.mx");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }


                    try
                    {
                        if (pedido.PedidosConekta?.Any() ?? false)
                        {
                            logger.Info($"El pedido {idPedido} fue realizado con tarjeta, realizando devolucion para {pedido.PedidosConekta.Count} ordenes en conekta");

                            foreach (var pedidosConekta in pedido.PedidosConekta)
                            {
                                var client = new RestClient("https://api.conekta.io")
                                {
                                    Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings.Get("API_KEY"), string.Empty)
                                };
                                client.AddDefaultHeader("Accept", "application/vnd.conekta-v2.0.0+json");
                                client.AddDefaultHeader("Content-Type", "application/json");

                                var request = new RestRequest($"/orders/{pedidosConekta.idOrdenPedido}/void", Method.POST);
                                var response = client.Execute(request);
                                logger.Info($"Cancelacion en conekta del pedido {pedidosConekta.idOrdenPedido}, respuesta: (StatusCode:{(int)response.StatusCode} - {response.StatusCode}){response.Content}");

                                pedidosConekta.estatusConekta = (int)PedidoTarjetaEstatus.Devuelto;
                                contexto.Entry(pedidosConekta).State = EntityState.Modified;
                            }

                            contexto.SaveChanges();
                        }

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }

                    try
                    {
                        var login = pedido.clientes.login.OrderByDescending(o => o.fechaRegistro).ToArray();

                        var listaPlyer = login.Select(s => s.playerId).Distinct().ToArray();

                        var notificacion = new NotificacionesHazPedido
                        {
                            sucursalId = pedido.sucursalId,
                            descripcion = "Lo sentimos el restaurante no pudo tomar tu pedido en este momento. Por favor vuelve a intentarlo o selecciona otro restaurante.",
                            titulo = "Pedido cancelado",
                            activo = true,
                            fechaRegistro = DateTime.Now,
                            usuarioRegistro = pedido.clientes.idCliente,
                            pedidoId = pedido.idPedido                            
                        };
                        try
                        {
                            if (listaPlyer?.Any() ?? false)
                            {
                                logger.Info($"El pedido {idPedido} fue cancelado, notificando a los dispositivos: {string.Join(", ", listaPlyer)}");

                                var @delegate = new SendNotificationDelegate();
                                @delegate.SendNotificationPorPlayerIds(listaPlyer, notificacion.titulo, notificacion.descripcion);
                            }
                        }
                        catch (Exception ee)
                        {
                            logger.Error(ee);
                        }

                        contexto.NotificacionesHazPedido.Add(notificacion);
                        contexto.ConsumidorNotificaciones.Add(new ConsumidorNotificaciones
                        {
                            consumidorId = pedido.consumidorId,
                            fechaEnviado = DateTime.Now,
                            revisado = false
                        });

                        contexto.SaveChanges();


                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }

                }
                else
                {
                    logger.Debug($"El pedido {idPedido} ha sido actualizado a estatus {(int)EstatusPedido.RECIBIDO}({EstatusPedido.RECIBIDO})");
                }
            }

        }

    }
}