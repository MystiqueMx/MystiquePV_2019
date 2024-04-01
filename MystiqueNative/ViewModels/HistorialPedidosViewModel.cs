using MystiqueNative.EventsArgs;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Orden;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class HistorialPedidosViewModel : BaseViewModel
    {
        #region SINGLETON
        public static HistorialPedidosViewModel Instance => _instance ?? (_instance = new HistorialPedidosViewModel());
        private static HistorialPedidosViewModel _instance;

        public static void Destroy() => _instance = null;
        #endregion

        #region CTOR

        public HistorialPedidosViewModel()
        {
            HistorialOrdenes = new ObservableCollection<OrdenHistorial>();
        }

        #endregion

        #region EVENTS

        public event EventHandler<BaseEventArgs> OnActualizarHistorialPedidosFinished;
        public event EventHandler<BaseEventArgs> OnCalificarPedidosFinished;
        public event EventHandler<DetallePedidoEventArgs> OnObtenerDetallePedidoFinished;

        #endregion

        #region FIELDS

        public ObservableCollection<OrdenHistorial> HistorialOrdenes { get; set; }
        public Orden HistorialSeleccionado { get; set; }
        #endregion

        #region API

        public async Task ObtenerHistorialPedidos()
        {
            IsBusy = true;
            var response = await Services.QdcApi.Orden.LlamarObtenerHistorialPedidos();
            if (response.Estatus.IsSuccessful)
            {
                HistorialOrdenes.Clear();
                foreach (var orden in response.Respuesta)
                {
                    HistorialOrdenes.Add(orden);
                }

                OnActualizarHistorialPedidosFinished?.Invoke(this, new BaseEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                });

            }
            else
            {
                OnActualizarHistorialPedidosFinished?.Invoke(this, new BaseEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }

            IsBusy = false;
        }

        public async Task ObtenerDetalleOrden(int idPedido)
        {
            IsBusy = true;

            var response = await Services.QdcApi.Orden.LlamarObtenerDetallePedido(idPedido);
            if (response.Estatus.IsSuccessful)
            {
                HistorialSeleccionado = response.Respuesta.Detalle;
                OnObtenerDetallePedidoFinished?.Invoke(this, new DetallePedidoEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    OrdenSeleccionada = HistorialSeleccionado
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
        public async Task CalificarPedido(int idPedido, int calificacionProducto, int calificacionReparticion, int calificacionMovil)
        {
            IsBusy = true;
            var response = await Services.QdcApi.Orden.LlamarCalificarPedido(idPedido, calificacionProducto, calificacionReparticion, calificacionMovil);
            await ObtenerHistorialPedidos();
            OnCalificarPedidosFinished?.Invoke(this, new BaseEventArgs
            {
                Success = response.Estatus.IsSuccessful,
                Message = response.Estatus.Message
            });

            IsBusy = false;
        }
        #endregion
    }
}
