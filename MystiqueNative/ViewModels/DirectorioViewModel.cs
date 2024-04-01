using MystiqueNative.Helpers;
using MystiqueNative.Models.Menu;
using MystiqueNative.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class DirectorioViewModel : BaseViewModel
    {
        #region SINGLETON
        public static DirectorioViewModel Instance => _instance ?? (_instance = new DirectorioViewModel());
        private static DirectorioViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion

        #region CTOR
        public DirectorioViewModel()
        {
            DirectorioResturantes = new ObservableCollection<Restaurante>();
            MenuDirectorioResturantes = new ObservableCollection<MenuRestaurante>();
        }

        #endregion

        #region EVENTS
        public event EventHandler<BaseEventArgs> OnObtenerDirectorioFinished;
        public event EventHandler<BaseEventArgs> OnObtenerMenuRestauranteFinished;
        #endregion

        #region FIELDS
        public ObservableCollection<Restaurante> DirectorioResturantes { get; }
        public ObservableCollection<MenuRestaurante> MenuDirectorioResturantes { get; }
        public Restaurante RestauranteActivo { get; set; }
        #endregion

        #region API

        public async Task ObtenerDirectorio()
        {
            IsBusy = true;
            var response = await QdcApi.Restaurantes.LlamarObtenerDirectorio();
            if (response.Estatus.IsSuccessful)
            {
                DirectorioResturantes.Clear();
                foreach (var resultadosRestaurante in response.Resultados.Restaurantes)
                {
                    DirectorioResturantes.Add(resultadosRestaurante);
                }
            }
            OnObtenerDirectorioFinished?.Invoke(this, new BaseEventArgs()
            {
                Success = response.Estatus.IsSuccessful,
                Message = response.Estatus.Message,
            });

            IsBusy = false;
        }
        public async Task ObtenerMenuRestaurante(int idRestaurante)
        {
            IsBusy = true;

            try
            {
                RestauranteActivo = DirectorioResturantes.First(c => c.Id == idRestaurante);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
                throw;
#else
                OnObtenerMenuRestauranteFinished?.Invoke(this, new BaseEventArgs
                {
                    Success = false,
                    Message = "Restaurante fuera de rango"
                });
#endif

            }

            var response = await QdcApi.Restaurantes.LlamarObtenerMenuRestaurante($"{idRestaurante}");
            if (response.Estatus.IsSuccessful)
            {
                MenuDirectorioResturantes.Clear();
                foreach (var resultadosRestaurante in response.Resultados.OrderBy(c => c.Orden))
                {
                    MenuDirectorioResturantes.Add(resultadosRestaurante);
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
