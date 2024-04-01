using MystiqueNative.Configuration;
using MystiqueNative.EventsArgs;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Orden;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class PedidosViewModel : BaseViewModel
    {
        #region SINGLETON
        public static PedidosViewModel Instance => _instance ?? (_instance = new PedidosViewModel());
        private static PedidosViewModel _instance;

        public static void Destroy() => _instance = null;
        #endregion

        #region CTOR

        public PedidosViewModel()
        {
            OrdenesActivas = new ObservableCollection<Orden>();
        }

        #endregion

        #region EVENTS
        public event EventHandler<RegistrarPedidoEventArgs> OnRegistrarOrdenFinished;
        public event EventHandler<BaseEventArgs> OnActualizarPedidosActivosFinished;
        public event EventHandler<DetallePedidoEventArgs> OnObtenerDetallePedidoFinished;
        public event EventHandler<BaseEventArgs> OnAgregarSeguimientoFinished;
        #endregion

        #region FIELDS

        public ObservableCollection<Orden> OrdenesActivas { get; set; }
        public Orden OrdenSeleccionada { get; set; }
        public MetodoPago MetodoPagoOrdenSeleccionada { get; set; }
        private CancellationTokenSource _cancellationTokenSource;
        private const int OrdenesRefreshRateInMilliseconds = 5_000;
        #endregion

        public async Task TerminarPedido(TipoReparto tipoReparto, FormaPago formaPago, DireccionOrden direccionEntrega = null)
        {
            IsBusy = true;
            var total = CarritoViewModel.Instance.TotalActualAsDecimal(tipoReparto);

            if (total < CarritoViewModel.Instance.PedidoActual.Restaurante.CompraMinima)
            {
                OnRegistrarOrdenFinished?.Invoke(this,
                 new RegistrarPedidoEventArgs
                 {
                     Success = false,
                     Message = $"El monto mínimo de compra es: {CarritoViewModel.Instance.PedidoActual.Restaurante.CompraMinima:C}",
                 });
                return;
            }

            CarritoViewModel.Instance.PedidoActual.Total = total;

            CarritoViewModel.Instance.PedidoActual.TipoReparto = tipoReparto;

            if (formaPago.Metodo == MetodoPago.Tarjeta
                && !string.IsNullOrEmpty(formaPago.IdTarjeta))
            {
                formaPago.IdSesion = await ServiceLocator.Instance.Get<Interfaces.IOpenPaySessionId>()
                    .CreateDeviceSessionIdInternal(OpenPayApiConfig.MerchantId, OpenPayApiConfig.PublicKey, OpenPayApiConfig.UrlApi);
            }
            var orden = new NuevaOrden
            {
                Pedido = CarritoViewModel.Instance.PedidoActual,
                FormaPago = formaPago,
                cliente = ClientesViewModel.Instance.ClienteSelected,
                solicitudPorAgente = true
            };

            if (direccionEntrega != null)
            {
                orden.DireccionEntrega = direccionEntrega;
            }

            var response = await Services.QdcApi.Orden.LlamarAgregarPedido(orden);
            if (response.Estatus.IsSuccessful)
            {
                CarritoViewModel.Instance.TerminarPedido();
                var numeroPedidos = await ObtenerPedidosActivos();
                OnRegistrarOrdenFinished?.Invoke(this,
                    new RegistrarPedidoEventArgs
                    {
                        Success = response.Estatus.IsSuccessful,
                        Message = response.Estatus.Message,
                        NumeroOrdenesActivas = numeroPedidos,
                        TipoDeReparto = tipoReparto,
                        MetodoDePago = formaPago.Metodo,
                        Total = orden.Pedido.Total ?? -1,
                    });
            }
            else
            {
                OnRegistrarOrdenFinished?.Invoke(this,
                    new RegistrarPedidoEventArgs
                    {
                        Success = response.Estatus.IsSuccessful,
                        Message = response.Estatus.Message,
                    });
            }

            IsBusy = false;
        }
        public async Task<int> ObtenerPedidosActivos()
        {
            IsBusy = true;
            var response = await Services.QdcApi.Orden.LlamarObtenerPedidosActivos();
            if (response.Estatus.IsSuccessful)
            {
                MystiqueApp.PedidosActivos = response.Respuesta.Count;
                OrdenesActivas.Clear();
                foreach (var orden in response.Respuesta)
                {
                    OrdenesActivas.Add(orden);
                }

                OnActualizarPedidosActivosFinished?.Invoke(this, new BaseEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });

            }
            else
            {
                OnActualizarPedidosActivosFinished?.Invoke(this, new BaseEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }

            IsBusy = false;
            return OrdenesActivas.Count;
        }

        public async Task AgregarSeguimiento(int idPedido, string seguimiento)
        {
            IsBusy = true;
            var response = await Services.QdcApi.Orden.LlamarAgregarSeguimiento(idPedido, seguimiento);

            OnAgregarSeguimientoFinished?.Invoke(this, new BaseEventArgs
            {
                Success = response.Estatus.IsSuccessful,
                Message = response.Estatus.Message
            });

            IsBusy = false;
        }

        public async Task SeleccionarOrden(int idPedido)
        {
            OrdenSeleccionada = OrdenesActivas.First(c => c.Id == idPedido);
            MetodoPagoOrdenSeleccionada = OrdenSeleccionada.FormaPago;
            await ObtenerDetalleOrden(OrdenSeleccionada);
        }
        private async Task ObtenerDetalleOrden(Orden pedido)
        {
            IsBusy = true;

            var response = await Services.QdcApi.Orden.LlamarObtenerDetallePedido(pedido.Id);
            if (response.Estatus.IsSuccessful)
            {
                OrdenSeleccionada = response.Respuesta.Detalle;
                OnObtenerDetallePedidoFinished?.Invoke(this, new DetallePedidoEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    OrdenSeleccionada = OrdenSeleccionada
                });
            }
            else
            {
                OnObtenerDetallePedidoFinished?.Invoke(this, new DetallePedidoEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }


            IsBusy = false;
        }

        #region AUTO REFRESH

        public async Task IniciarActualizacionPedido()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                while (!_cancellationTokenSource.Token.IsCancellationRequested && OrdenSeleccionada != null)
                {
                    await ObtenerDetalleOrden(OrdenSeleccionada);
                    await Task.Delay(OrdenesRefreshRateInMilliseconds, _cancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
            }
        }

        public void DetenerActualizacionPedido()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
            }
        }

        #endregion
    }
}
