using MystiqueNative.EventsArgs;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Location;
using MystiqueNative.Models.Menu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class DireccionesViewModel : BaseViewModel
    {
        #region SINGLETON
        public static DireccionesViewModel Instance => _instance ?? (_instance = new DireccionesViewModel());
        private static DireccionesViewModel _instance;

        public static void Destroy() => _instance = null;
        #endregion

        #region CTOR

        public DireccionesViewModel()
        {
            Colonias = new ObservableCollection<string>();
            Direcciones = new ObservableCollection<Direction>();
            _colonias = new List<Colonia>();
        }

        #endregion

        #region EVENTS
        public event EventHandler<BaseEventArgs> OnObtenerDireccionesFinished;
        public event EventHandler<BaseUpdateArgs> OnAgregarDireccionFinished;
        public event EventHandler<BaseUpdateArgs> OnEditarDireccionFinished;
        public event EventHandler<ObtenerColoniasEventArgs> OnObtenerColoniasFinished;
        #endregion

        #region FIELDS

        public ObservableCollection<Direction> Direcciones { get; set; }
        public ObservableCollection<string> Colonias { get; set; }
        public Restaurante RestauranteActivo { get; private set; }
        private List<Colonia> _colonias { get; set; }
        #endregion

        #region API
        public async void ObtenerDirecciones(int idRestaurante = 0)
        {
            IsBusy = true;
            var response = await Services.QdcApi.Direccion.LlamarObtenerDirecciones(idRestaurante);

            if (response.Estatus.IsSuccessful)
            {
                Direcciones.Clear();
                foreach (var resultadosRestaurante in response.Respuesta)
                {
                    Direcciones.Add(resultadosRestaurante);
                }
            }

            OnObtenerDireccionesFinished?.Invoke(this,
                new BaseEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            IsBusy = false;
        }


        public async Task AgregarDireccion(Direction direccion, int idRestaurante = 0)
        {
            IsBusy = true;
            if (string.IsNullOrEmpty(direccion.PostalCode))
            {
                if (Colonias.Contains(direccion.SubLocality))
                {
                    direccion.IdColonia = _colonias.First(c => c.Nombre == direccion.SubLocality).Id;
                }
                else
                {
                    OnAgregarDireccionFinished?.Invoke(this,
                        new BaseUpdateArgs
                        {
                            Success = false,
                            Message = $"Debes seleccionar la colonia"
                        });
                    IsBusy = false;
                    return;
                }

            }


            var config = await Services.QdcApi.Direccion.LlamarAgregarDireccion(direccion);


            var response = await Services.QdcApi.Direccion.LlamarObtenerDirecciones(idRestaurante);

            if (response.Estatus.IsSuccessful)
            {
                Direcciones.Clear();
                foreach (var resultadosRestaurante in response.Respuesta)
                {
                    Direcciones.Add(resultadosRestaurante);
                }
            }

            if (config.Estatus.IsSuccessful)
            {
                OnEditarDireccionFinished?.Invoke(this,
                    new BaseUpdateArgs()
                    {
                        Success = config.Estatus.IsSuccessful,
                        Message = config.Estatus.Message,
                        EditedId = config.Respuesta.FirstOrDefault()?.Id ?? 0,
                    });
            }
            else
            {
                OnEditarDireccionFinished?.Invoke(this,
                    new BaseUpdateArgs
                    {
                        Success = config.Estatus.IsSuccessful,
                        Message = config.Estatus.Message
                    });
            }

            IsBusy = false;
        }

        public async Task EditarDireccion(Direction direccion, int idRestaurante = 0)
        {
            IsBusy = true;
            var config = await Services.QdcApi.Direccion.LlamarEditarDireccion(direccion, true);

            var response = await Services.QdcApi.Direccion.LlamarObtenerDirecciones(idRestaurante);

            if (response.Estatus.IsSuccessful)
            {
                Direcciones.Clear();
                foreach (var resultadosRestaurante in response.Respuesta)
                {
                    Direcciones.Add(resultadosRestaurante);
                }
            }

            if (config.Estatus.IsSuccessful)
            {
                OnEditarDireccionFinished?.Invoke(this,
                    new BaseUpdateArgs()
                    {
                        Success = config.Estatus.IsSuccessful,
                        Message = config.Estatus.Message,
                        EditedId = config.Respuesta.FirstOrDefault()?.Id ?? 0,
                    });
            }
            else
            {
                OnEditarDireccionFinished?.Invoke(this,
                    new BaseUpdateArgs
                    {
                        Success = config.Estatus.IsSuccessful,
                        Message = config.Estatus.Message
                    });
            }

            IsBusy = false;
        }
        public async Task EliminarDireccion(Direction direccion)
        {
            IsBusy = true;

            var response = await Services.QdcApi.Direccion.LlamarEditarDireccion(direccion, false);
            if (response.Estatus.IsSuccessful)
            {
                Direcciones.RemoveAt(Direcciones.IndexOf(direccion));
            }


            OnEditarDireccionFinished?.Invoke(this,
                new BaseUpdateArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message,

                });
            IsBusy = false;
        }
        public async void ObtenerColonias(Point ubicacion, string codigoPostal, int idRestaurante = 0)
        {
            IsBusy = true;
            var response = await Services.QdcApi.Direccion.LlamarObtenerColonias(ubicacion, codigoPostal, idRestaurante);
            Colonias.Clear();
            _colonias.Clear();
            if (response.Estatus.IsSuccessful)
            {
                foreach (var colonia in response.Colonias)
                {
                    Colonias.Add(colonia.Nombre);
                    _colonias.Add(colonia);
                }
            }

            OnObtenerColoniasFinished?.Invoke(this,
                new ObtenerColoniasEventArgs
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message,
                    Colonias = Colonias.ToList(),
                    IsEmpty = Colonias.Count == 0,
                    ResultCount = Colonias.Count
                });
            IsBusy = false;
        }
        #endregion
    }
}
