using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MystiqueNative.Services;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class HistorialViewModel : BaseViewModel
    {
        #region Singleton
        public static HistorialViewModel Instance => _instance ?? (_instance = new HistorialViewModel());
        private static HistorialViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        public event EventHandler<HistorialViewArgs> FinishLoadingHistorial;
        public ObservableCollection<MovimientoCitypoints> MovimientosSumados { get; set; }
        public ObservableCollection<MovimientoCitypoints> MovimientosCanjeados { get; set; }
        public HistorialViewModel()
        {
            MovimientosSumados = new ObservableCollection<MovimientoCitypoints>();
            MovimientosCanjeados = new ObservableCollection<MovimientoCitypoints>();
        }
        public string Sumados { get; set; }
        public string Canjeados { get; set; }
        public string Actuales { get; set; }
        public async void ObtenerHistorial()
        {
            IsBusy = true;
            var response = await MystiqueApiV2.CityPoints.CallObtenerMovimientos();
            
            if (response.Success)
            {
                MovimientosSumados.Clear();
                MovimientosCanjeados.Clear();
                var sumados = 0;
                var restados = 0;
                response.ListaMovimientos.ForEach(c =>
                {
                    if (c.IsUp)
                    {
                        MovimientosSumados.Add(c);
                        sumados += c.PuntosAsInt;
                    }
                    else
                    {
                        MovimientosCanjeados.Add(c);
                        restados += c.PuntosAsInt;
                    }
                });
                var actuales = sumados - restados;

                Sumados = sumados.ToString();
                Canjeados = restados.ToString();
                Actuales = actuales.ToString();
                FinishLoadingHistorial?.Invoke(this, new HistorialViewArgs { Success = true, Sumados = Sumados, Canjeados = Canjeados, Actuales = Actuales });
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
                FinishLoadingHistorial?.Invoke(this, new HistorialViewArgs { Success = false, ErrorMessage = response.ErrorMessage });
            }

            ErrorStatus = !response.Success;
            IsBusy = false;
        }
    }
}
