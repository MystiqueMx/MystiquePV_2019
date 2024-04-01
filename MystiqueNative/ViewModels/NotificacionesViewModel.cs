using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MystiqueNative.Services;
using MystiqueNative.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class NotificacionesViewModel : BaseViewModel
    {
        #region Singleton
        public static NotificacionesViewModel Instance => _instance ?? (_instance = new NotificacionesViewModel());
        private static NotificacionesViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion

        #region FIELDS
        public ObservableCollection<Notificacion_HP> NotificacionesHazPedido { get; private set; }
        #endregion

        #region CTOR

        public NotificacionesViewModel()
        {
            Notificaciones = new ObservableCollection<Notificacion>();
            NotificacionesHazPedido = new ObservableCollection<Notificacion_HP>();
        }

        #endregion

        #region EVENTS
        public event EventHandler<BaseEventArgs> OnObtenerNotificacionesHPFinished;
        public event EventHandler<BaseEventArgs> OnActualizarNotificacionesHPFinished;
        #endregion

        #region API

        public async Task ObtenerNotificacionesHP()
        {
            IsBusy = true;
            var response = await QdcApi.Notificaciones.LlamarObtenerNotificaciones();
            if (response.Estatus.IsSuccessful)
            {
                NotificacionesHazPedido.Clear();
                foreach (var notificacion in response.Resultados)
                {
                    NotificacionesHazPedido.Add(notificacion);
                }
            }
            OnObtenerNotificacionesHPFinished?.Invoke(this, new BaseEventArgs()
            {
                Success = response.Estatus.IsSuccessful,
                Message = response.Estatus.Message,
            });

            IsBusy = false;
        }

        public async Task MarcarNotificacionesHPComoLeidas()
        {
            IsBusy = true;
            var response = await QdcApi.Notificaciones.LlamarActualizarNotificaciones();
            OnActualizarNotificacionesHPFinished?.Invoke(this, new BaseEventArgs()
            {
                Success = response.Estatus.IsSuccessful,
                Message = response.Estatus.Message,
            });

            IsBusy = false;
        }

        #endregion

        public event EventHandler<ObtenerNotificacionesArgs> OnObtenerNotificacionesFinished;
        public ObservableCollection<Notificacion> Notificaciones { get; private set; }
        public int NotificacionesNuevas { get; private set; }

        public async void ObtenerNotificaciones()
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Notificaciones.CallObtenerNotificacionesIdCliente();
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
                OnObtenerNotificacionesFinished?.Invoke(this, new ObtenerNotificacionesArgs { Success = false, Message = response.ErrorMessage });
            }
            else
            {
                Notificaciones.Clear();
                NotificacionesNuevas = response.Notificaciones.Count(c => !c.Leido);
                foreach (var t in response.Notificaciones)
                    Notificaciones.Add(t);

                OnObtenerNotificacionesFinished?.Invoke(this, new ObtenerNotificacionesArgs { Success = true, NotificacionesNuevas = response.Notificaciones.Count(c => !c.Leido) });
            }
            ErrorStatus = !response.Success;
        }
        public async void LimpiarNotificaciones()
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Notificaciones.CallLimpiarNotificacionesCliente();
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
            }

            ErrorStatus = !response.Success;
        }
        //public NotificacionesViewModel() => Notificaciones = new ObservableCollection<Notificacion>();
    }
    #region EventArgs

    #endregion
}
