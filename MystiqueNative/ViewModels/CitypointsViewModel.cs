using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MystiqueNative.Services;
using MystiqueNative.Models;
using System.Linq;

namespace MystiqueNative.ViewModels
{
    public class CitypointsViewModel : BaseViewModel
    {
        #region Singleton
        public static CitypointsViewModel Instance => _instance ?? (_instance = new CitypointsViewModel());
        private static CitypointsViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        public event EventHandler<AgregarPuntosArgs> OnAgregarPuntosFinished;
        public event EventHandler<CanjearRecompensaArgs> OnCanjearRecompensaFinished;
        public event EventHandler<EstadoCuentaArgs> OnEstadoCuentaFinished;
        public ObservableCollection<Recompensa> Recompensas { get; set; }
        public ObservableCollection<MovimientoCitypoints> Movimientos { get; set; }
        public ObservableCollection<RecompensaCanjeada> RecompensasActivas { get; set; }
        public bool AgregarStatus { get => _agregarStatus; private set { _agregarStatus = value; OnPropertyChanged("AgregarStatus"); } }
        public bool PuedeCanjear { get => _puedeCanjear; private set { _puedeCanjear = value; OnPropertyChanged("PuedeCanjear"); } }
        public bool MovimientosStatus { get => _movimientosStatus; private set { _movimientosStatus = value; OnPropertyChanged("MovimientosStatus"); } }
        public bool CanjearStatus { get => _canjearStatus; private set { _canjearStatus = value; OnPropertyChanged("CanjearStatus"); } }
        public bool EliminarStatus { get => _eliminarStatus; private set { _eliminarStatus = value; OnPropertyChanged("EliminarStatus"); } }
        public EstadoCuenta EstadoCuenta { get => _estadoCuenta; private set { _estadoCuenta = value; OnPropertyChanged("EstadoCuenta"); } }
        public RecompensaCanjeada CodigoCanje { get => _codigoCanje; private set { _codigoCanje = value; OnPropertyChanged("CodigoCanje"); } }
        public string MensajePuedeCanjear;
        private bool _agregarStatus;
        private bool _puedeCanjear;
        private bool _movimientosStatus;
        private bool _canjearStatus;
        private bool _eliminarStatus;
        private EstadoCuenta _estadoCuenta;
        private RecompensaCanjeada _codigoCanje;
        public async void AgregarPuntos(string codigo)
        {
            //if (IsBusy) return;
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallRegistarPuntos(codigo);
            var puntos = await MystiqueApiV2.CityPoints.CallObtenerPuntos();
            {
                EstadoCuenta = puntos;
                OnEstadoCuentaFinished?.Invoke(this, new EstadoCuentaArgs { EstadoCuenta = puntos });
            }
            IsBusy = false;
            OnAgregarPuntosFinished?.Invoke(this, new AgregarPuntosArgs { Success = response.Success, Message = response.ErrorMessage });
            ErrorMessage = response.ErrorMessage;
            AgregarStatus = response.Success;
        }
        public async void ObtenerEstadoCuenta()
        {
            if (IsBusy) return;
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallObtenerPuntos();
            OnEstadoCuentaFinished?.Invoke(this, new EstadoCuentaArgs { EstadoCuenta = response });
            IsBusy = false;
            if (response.Success)
            {
                EstadoCuenta = response;
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }
            
            ErrorStatus = !response.Success;
        }
        public async void ObtenerRecompensas()
        {
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallObtenerRecompensas();
            var puntos = await MystiqueApiV2.CityPoints.CallObtenerPuntos();
            IsBusy = false;
            if (puntos.Success)
            {
                EstadoCuenta = puntos;
                OnEstadoCuentaFinished?.Invoke(this, new EstadoCuentaArgs { EstadoCuenta = puntos });
            }
            if (response.Success)
            {
                Recompensas.Clear();
                foreach (var t in response.Recompensas)
                    Recompensas.Add(t);
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }

            ErrorStatus = !response.Success;
        }
        public async void CanjearRecompensa(string IdRecompensa)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallCanjearPuntos(IdRecompensa);
            IsBusy = false;
            if (response.Success)
            {
                CodigoCanje = response;
                OnCanjearRecompensaFinished?.Invoke(this, new CanjearRecompensaArgs { Success = true, CodigoCanje = CodigoCanje, Recompensa = Recompensas.FirstOrDefault(c => c.Id == IdRecompensa) });
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
                OnCanjearRecompensaFinished?.Invoke(this, new CanjearRecompensaArgs { Success = false, ErrorMessage = response.ErrorMessage });
            }

            CanjearStatus = response.Success;
        }
        public async void ObtenerMovimientosCitypoints()
        {
            if (IsBusy) return;
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallObtenerMovimientos();
            IsBusy = false;
            if (response.Success)
            {
                Movimientos.Clear();
                foreach (var t in response.ListaMovimientos)
                    Movimientos.Add(t);
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }

            ErrorStatus = !response.Success;
        }
        #region Deprecated
        public async void EliminarRecompensa(string CanjePuntoId)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallEliminarRecompensa(CanjePuntoId);
            IsBusy = false;
            if (!response.Success)
                ErrorMessage = response.ErrorMessage;
            EliminarStatus = response.Success;

        }
        public async void ObtenerRecompensasActivas()
        {
            if (IsBusy) return;
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallObtenerRecompensasActivas();
            IsBusy = false;
            if (response.Success)
            {
                RecompensasActivas.Clear();
                foreach (var t in response.RecompensasActivas)
                    RecompensasActivas.Add(t);
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }

            ErrorStatus = !response.Success;
        }
        #endregion



        public CitypointsViewModel()
        {
            Recompensas = new ObservableCollection<Recompensa>();
            Movimientos = new ObservableCollection<MovimientoCitypoints>();
            RecompensasActivas = new ObservableCollection<RecompensaCanjeada>();
        }

    }
    #region EventArgs

    #endregion
}
