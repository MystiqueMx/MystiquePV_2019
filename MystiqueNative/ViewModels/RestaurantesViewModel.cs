using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MystiqueNative.EventsArgs;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Menu;
using MystiqueNative.Services;

namespace MystiqueNative.ViewModels
{
    public class RestaurantesViewModel : BaseViewModel
    {
        #region SINGLETON
        public static RestaurantesViewModel Instance => _instance ?? (_instance = new RestaurantesViewModel());
        private static RestaurantesViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion

        #region CTOR

        public RestaurantesViewModel()
        {
            Restaurantes = new ObservableCollection<Restaurante>();
            MenuRestauranteActivo = new ObservableCollection<MenuRestaurante>();
        }

        #endregion
        #region EVENTS
        public event EventHandler<ObtenerRestaurantesEventArgs> OnObtenerRestaurantesFinished;
        public event EventHandler<BaseEventArgs> OnObtenerMenuRestauranteFinished;
        #endregion

        #region FIELDS

        public ObservableCollection<Restaurante> Restaurantes { get; private set; }
        public Restaurante RestauranteActivo { get; private set; }
        public ObservableCollection<MenuRestaurante> MenuRestauranteActivo { get; private set; }
        #endregion
        #region API
        public async Task ObtenerRestaurantes(FiltroRestaurante filtro = FiltroRestaurante.Todos)
        {
            IsBusy = true;
            var response = await QdcApi.Restaurantes.LlamarObtenerRestaurantes(filtro);
            if (response.Estatus.IsSuccessful)
            {
                Restaurantes.Clear();
                foreach (var resultadosRestaurante in response.Resultados.Restaurantes)
                {
                    Restaurantes.Add(resultadosRestaurante);    
                }

                if (Restaurantes.Count == 0)
                {
                    OnObtenerRestaurantesFinished?.Invoke(this, new ObtenerRestaurantesEventArgs
                    {
                        Success = true,
                        EstatusRestaurantes = EstatusMenu.FueraCobertura,
                        Restaurantes = Restaurantes.ToList(),
                    });
                }
                else
                {
                    OnObtenerRestaurantesFinished?.Invoke(this, new ObtenerRestaurantesEventArgs
                    {
                        Success = true,
                        EstatusRestaurantes = EstatusMenu.Abierto,
                        Restaurantes = Restaurantes.ToList(),
                    });
                }
                
            }
            else
            {
                if (response.Estatus.ResponseCode == ResponseTypes.CodigoInterno)
                {
                    OnObtenerRestaurantesFinished?.Invoke(this, new ObtenerRestaurantesEventArgs
                    {
                        Success = false,
                        EstatusRestaurantes = EstatusMenu.FueraCobertura,
                        Message = response.Estatus.Message,
                    });
                }
                else
                {
                    OnObtenerRestaurantesFinished?.Invoke(this, new ObtenerRestaurantesEventArgs
                    {
                        Success = false,
                        EstatusRestaurantes = EstatusMenu.NoDefinido,
                        Message = response.Estatus.Message,
                    });
                }
            }

            IsBusy = false;
        }
        public void SeleccionarRestaurante(int seleccion)
        {
            RestauranteActivo = Restaurantes.First(c=>c.Id == seleccion);
            MenuRestauranteActivo.Clear();
        }
        public void SeleccionarRestaurante(Restaurante seleccion)
        {
            RestauranteActivo = seleccion;
            MenuRestauranteActivo.Clear();
        }
        public async Task ObtenerMenuRestaurante(int idRestaurante)
        {
            IsBusy = true;

            var response = await QdcApi.Restaurantes.LlamarObtenerMenuRestaurante($"{idRestaurante}");
            if (response.Estatus.IsSuccessful)
            {
                MenuRestauranteActivo.Clear();
                foreach (var resultadosRestaurante in response.Resultados.OrderBy(c=>c.Orden))
                {
                    MenuRestauranteActivo.Add(resultadosRestaurante);
                }
                OnObtenerMenuRestauranteFinished?.Invoke(this, new BaseEventArgs()
                {
                    Success = true,
                });
            }
            else
            {
                OnObtenerMenuRestauranteFinished?.Invoke(this, new BaseEventArgs()
                {
                    Success = false,
                    Message = response.Estatus.Message
                });
            }

            IsBusy = false;
        }
        #endregion
    }
}
