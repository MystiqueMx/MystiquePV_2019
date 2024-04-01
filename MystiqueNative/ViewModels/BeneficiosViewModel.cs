using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MystiqueNative.Services;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class BeneficiosViewModel : BaseViewModel
    {
        #region Singleton
        public static BeneficiosViewModel Instance => _instance ?? (_instance = new BeneficiosViewModel());
        private static BeneficiosViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        //ES CON LO QUE SE VA A LLENAR
        private const string SucursalWildcard = "0";
        public ObservableCollection<Comercio> Comercios { get; set; }
        public ObservableCollection<Sucursal> Sucursales { get; set; }
        public ObservableCollection<BeneficiosSucursal> Beneficios { get;  set; }
        public DetalleBeneficio BeneficioSeleccionado { get => _beneficioSeleccionado; set { _beneficioSeleccionado = value; OnPropertyChanged(nameof(BeneficioSeleccionado)); } }
        public bool GuardarCalificacionStatus { get => _insertarCalifStatus; set { _insertarCalifStatus = value; OnPropertyChanged(nameof(GuardarCalificacionStatus)); } }
        
        private DetalleBeneficio _beneficioSeleccionado;
        private bool _insertarCalifStatus;
        //CONSTRUCTOR
        public BeneficiosViewModel()
        {
            Comercios = new ObservableCollection<Comercio>();
            Sucursales = new ObservableCollection<Sucursal>();
            Beneficios = new ObservableCollection<BeneficiosSucursal>();
            BeneficioSeleccionado = null;
        }

        // EL METODO QUE OBTIENE LAS SUCURSALES POR ID
        public async void ObtenerSucursalesPorIdComercio(string idComercio)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Beneficios.CallObtenerSucursalesPorComercio(idComercio);
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
            }
            else
            {
                Sucursales.Clear();
                foreach (var t in response.ListaSucursales)
                    Sucursales.Add(t);
            }
            ErrorStatus = !response.Success;
        }
        public async void ObtenerTodosLosBeneficios()
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Beneficios.CallObtenerBeneficiosPorSucursal(SucursalWildcard);
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
            }
            else
            {
                Beneficios.Clear();
                foreach (var t in response.BeneficiosSucursal)
                    Beneficios.Add(t);
            }
            ErrorStatus = !response.Success;

        }
        public async void ObtenerBeneficioPorIdSucursal(string idSucursal)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Beneficios.CallObtenerBeneficiosPorSucursal(idSucursal);
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
            }
            else
            {
                Beneficios.Clear();
                foreach (var t in response.BeneficiosSucursal)
                    Beneficios.Add(t);
            }
            ErrorStatus = !response.Success;

        }
        public async void ObtenerDetalleBeneficios(string idBeneficio, string idSucursal)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Beneficios.CallObtenerDetalleBeneficio(idBeneficio, idSucursal);
            IsBusy = false;
            if (response.Success)
            {
                BeneficioSeleccionado = response.Detalle;
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }
            ErrorStatus = !response.Success;

        }
        public async void ObtenerComerciosPorGiro(string idGiro)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Beneficios.CallObtenerComerciosIdGiro(idGiro);
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
            }
            else
            {
                Comercios.Clear();
                foreach (var t in response.ListaComercios)
                    Comercios.Add(t);
            }
            ErrorStatus = !response.Success;
        }

        public async void InsertarCalificacionBeneficio(string idBeneficio, int calificacion)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Beneficios.CallInsertarCalificacion(idBeneficio, calificacion.ToString());
            IsBusy = false;
            ErrorMessage = response.ErrorMessage;
            GuardarCalificacionStatus = response.Success;
        }

        
    }
}
