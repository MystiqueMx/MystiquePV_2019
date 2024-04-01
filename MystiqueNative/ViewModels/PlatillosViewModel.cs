using MystiqueNative.EventsArgs;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Carrito;
using MystiqueNative.Models.Platillos;
using MystiqueNative.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MystiqueNative.ViewModels
{
    public class PlatillosViewModel : BaseViewModel
    {
        #region SINGLETON
        public static PlatillosViewModel Instance => _instance ?? (_instance = new PlatillosViewModel());
        private static PlatillosViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion

        #region CTOR

        public PlatillosViewModel()
        {
            Platillos = new ObservableCollection<Platillo>();
            PlatillosRandom = new ObservableCollection<Platillo>();
            _platillosSeleccionadosCombo = new List<SeleccionPlatilloMultiNivel>();
        }

        #endregion

        #region EVENTS

        public event EventHandler<BaseEventArgs> OnObtenerPlatillosMenuFinished;
        public event EventHandler<BaseEventArgs> OnObtenerPlatillosRandomFinished;
        public event EventHandler<PlatilloTerminadoEventArgs> PlatilloTerminado;
        public event EventHandler<CargarSubmenuEventArgs> CargarSubmenu;
        public event EventHandler<SubplatilloCompletado> SubplatilloCompletado;
        #endregion

        #region FIELDS
        public ObservableCollection<Platillo> PlatillosRandom { get; set; }
        public ObservableCollection<Platillo> Platillos { get; set; }
        public Platillo PlatilloSeleccionado { get; private set; }

        #region PRIVATE FIELDS
        private int _cantidadRestantePlatilloSeleccionado;
        private readonly List<SeleccionPlatilloMultiNivel> _platillosSeleccionadosCombo;
        private PlatilloCarrito _platilloCarritoSeleccionado;
        private int _ultimoIdPrimerNivelSeleccionado;
        private int _ultimoIdSegundoNivelSeleccionado;
        #endregion

        #endregion

        #region API

        public async void ObtenerPlatillosMenu(int idMenu, int idRestaurante)
        {
            IsBusy = true;

            var response = await QdcApi.Restaurantes.LlamarObtenerPlatillosMenu(idMenu.ToString(), idRestaurante.ToString());
            if (response.Estatus.IsSuccessful)
            {
                Platillos.Clear();
                foreach (var resultadosRestaurante in response.Resultados.Platillos)
                {
                    if (CarritoViewModel.Instance.PedidoActual != null &&
                        CarritoViewModel.Instance.PedidoActual.Platillos.Any(c => c.Id == resultadosRestaurante.Id))
                    {
                        resultadosRestaurante.CantidadEnCarrito =
                            CarritoViewModel.Instance.PedidoActual.Platillos
                                .Count(c => c.Id == resultadosRestaurante.Id);
                    }
                    else
                    {
                        resultadosRestaurante.CantidadEnCarrito = 0;
                    }
                    Platillos.Add(resultadosRestaurante);
                }
                OnObtenerPlatillosMenuFinished?.Invoke(this, new BaseEventArgs()
                {
                    Success = true,
                });
            }
            else
            {
                OnObtenerPlatillosMenuFinished?.Invoke(this, new BaseEventArgs()
                {
                    Success = false,
                    Message = response.Estatus.Message
                });
            }

            IsBusy = false;
        }

        public void SeleccionarPlatillo(int idPlatillo)
        {
            int indicePlatillo = 0;
            try
            {
                PlatilloSeleccionado = Platillos.First(c => c.Id == idPlatillo);
                indicePlatillo = Platillos.IndexOf(PlatilloSeleccionado);
                _platillosSeleccionadosCombo.Clear();
                _platilloCarritoSeleccionado = new PlatilloCarrito
                {
                    Id = PlatilloSeleccionado.Id,
                    Nombre = PlatilloSeleccionado.Nombre,
                    Nivel1 = new List<int>(),
                    Nivel2 = new List<int>(),
                    Nivel3 = new List<int>(),
                    Notas = string.Empty,
                    Precio = PlatilloSeleccionado.Precio,
                    Imagen = PlatilloSeleccionado.ImagenUrl,
                };
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
                throw;
#else
                //OnObtenerMenuRestauranteFinished?.Invoke(this, new BaseEventArgs
                //{
                //    Success = false,
                //    Message = "PlatilloSeleccionado fuera de rango"
                //});
#endif

            }

            _cantidadRestantePlatilloSeleccionado = PlatilloSeleccionado.ConfiguracionCantidad;
            if (PlatilloSeleccionado.EsTerminal)
            {
                var argumentos = new PlatilloTerminadoEventArgs
                {
                    Success = true,
                    Message = $"{_platilloCarritoSeleccionado.Nombre} agregado al carrito",
                    Platillo = _platilloCarritoSeleccionado,
                    Posicion = indicePlatillo,

                };
                CargarContenidoPlatilloSeleccionado();
                CarritoViewModel.Instance.AgregarPlatilloACarrito(_platilloCarritoSeleccionado, true);
                Platillos[indicePlatillo].CantidadEnCarrito =
                    CarritoViewModel.Instance.ConteoPlatillosPorId(PlatilloSeleccionado.Id);
                _platilloCarritoSeleccionado = null;
                PlatilloTerminado?.Invoke(this, argumentos);
            }
            else
            {
                _platillosSeleccionadosCombo.Clear();
                var hijos = new List<BasePlatilloMultiNivel>(PlatilloSeleccionado.Hijos);
                if (_cantidadRestantePlatilloSeleccionado != 1)
                {
                    foreach (var hijo in PlatilloSeleccionado.Hijos)
                    {
                        if (!hijo.EsTerminal) continue;

                        _platilloCarritoSeleccionado.Nivel1.Add(hijo.Id);
                        _platillosSeleccionadosCombo.Add(new SeleccionPlatilloMultiNivel
                        {
                            IdNivel1 = new KeyValuePair<int, string>(hijo.Id, hijo.Descripcion),
                            IdNivel2 = new KeyValuePair<int, string>(0, string.Empty),
                            IdNivel3 = new KeyValuePair<int, string>(0, string.Empty)
                        });
                        hijos.First(c => c.Id == hijo.Id).Completado = true;
                        _cantidadRestantePlatilloSeleccionado--;
                    }
                }
                CargarSubmenu?.Invoke(this, new CargarSubmenuEventArgs
                {
                    Contenido = hijos,
                    Nivel = NivelesPlatillo.PrimerNivel,
                    Success = true,
                });
            }
        }

        #region PROCESO DE COMBOS

        public void SeleccionarSubPlatillo(int idPlatillo, NivelesPlatillo nivel)
        {

            switch (nivel)
            {
                case NivelesPlatillo.PrimerNivel:
                    SeleccionarPlatilloNivel1(idPlatillo);
                    break;
                case NivelesPlatillo.SegundoNivel:
                    SeleccionarPlatilloNivel2(idPlatillo);
                    break;
                case NivelesPlatillo.TercerNivel:
                    SeleccionarPlatilloNivel3(idPlatillo);
                    break;
                case NivelesPlatillo.NoDefinido:
                default:
                    throw new ArgumentOutOfRangeException(nameof(nivel), nivel, null);
            }
        }

        private void SeleccionarPlatilloNivel3(int idPlatillo)
        {
            var platillo1 = PlatilloSeleccionado.Hijos
                .First(c => c.Id == _ultimoIdPrimerNivelSeleccionado);
            var platillo2 = platillo1.Hijos
                    .First(c => c.Id == _ultimoIdSegundoNivelSeleccionado);
            var platillo3 = platillo2.Hijos
                        .First(c => c.Id == idPlatillo);

            EliminarSeleccionPrevia(platillo1.Id);

            //_platilloCarritoSeleccionado.Contenido += $", {platillo1.Descripcion}, {platillo2.Descripcion}, {platillo3.Descripcion}";
            _platilloCarritoSeleccionado.Precio += platillo1.Precio + platillo2.Precio + platillo3.Precio;
            _platilloCarritoSeleccionado.Nivel1.Add(platillo1.Id);
            _platilloCarritoSeleccionado.Nivel2.Add(platillo2.Id);
            _platilloCarritoSeleccionado.Nivel3.Add(platillo3.Id);
            _cantidadRestantePlatilloSeleccionado--;
            _platillosSeleccionadosCombo.Add(new SeleccionPlatilloMultiNivel
            {
                IdNivel1 = new KeyValuePair<int, string>(platillo1.Id, platillo1.Descripcion),
                IdNivel2 = new KeyValuePair<int, string>(platillo2.Id, platillo2.Descripcion),
                IdNivel3 = new KeyValuePair<int, string>(platillo3.Id, platillo3.Descripcion)
            });
            if (_cantidadRestantePlatilloSeleccionado == 0)
            {
                var Argumentos = new PlatilloTerminadoEventArgs
                {
                    Success = true,
                    Message = $"{_platilloCarritoSeleccionado.Nombre} agregado al carrito",
                    Platillo = _platilloCarritoSeleccionado

                };
                CargarContenidoPlatilloSeleccionado();
                CarritoViewModel.Instance.AgregarPlatilloACarrito(_platilloCarritoSeleccionado, true);
                Platillos.First(c => c.Id == PlatilloSeleccionado.Id).CantidadEnCarrito =
                    CarritoViewModel.Instance.ConteoPlatillosPorId(PlatilloSeleccionado.Id);
                _platilloCarritoSeleccionado = null;
                PlatilloTerminado?.Invoke(this, Argumentos);
            }
            else
            {
                SubplatilloCompletado?.Invoke(this, new SubplatilloCompletado()
                {
                    Success = true,
                    Message = $"{platillo3.Descripcion} seleccionado",
                    Seleccion = new SeleccionPlatilloMultiNivel()
                    {
                        IdNivel1 = new KeyValuePair<int, string>(platillo1.Id, platillo1.Descripcion),
                        IdNivel2 = new KeyValuePair<int, string>(platillo2.Id, platillo2.Descripcion),
                        IdNivel3 = new KeyValuePair<int, string>(platillo3.Id, platillo3.Descripcion)
                    }

                });
            }
        }

        private void SeleccionarPlatilloNivel2(int idPlatillo)
        {
            var platillo1 = PlatilloSeleccionado.Hijos
                .First(c => c.Id == _ultimoIdPrimerNivelSeleccionado);
            var platillo2 = platillo1.Hijos
                    .First(c => c.Id == idPlatillo);


            if (platillo2.EsTerminal)
            {
                _cantidadRestantePlatilloSeleccionado--;
                EliminarSeleccionPrevia(platillo1.Id);
                _platillosSeleccionadosCombo.Add(new SeleccionPlatilloMultiNivel
                {
                    IdNivel1 = new KeyValuePair<int, string>(platillo1.Id, platillo1.Descripcion),
                    IdNivel2 = new KeyValuePair<int, string>(platillo2.Id, platillo2.Descripcion),
                    IdNivel3 = new KeyValuePair<int, string>(0, string.Empty)
                });
                _platilloCarritoSeleccionado.Nivel1.Add(platillo1.Id);
                _platilloCarritoSeleccionado.Nivel2.Add(platillo2.Id);
                //_platilloCarritoSeleccionado.Contenido += $", {platillo1.Descripcion}, {platillo2.Descripcion}";
                _platilloCarritoSeleccionado.Precio += platillo1.Precio + platillo2.Precio;
                if (_cantidadRestantePlatilloSeleccionado == 0)
                {
                    var Argumentos = new PlatilloTerminadoEventArgs
                    {
                        Success = true,
                        Message = $"{_platilloCarritoSeleccionado.Nombre} agregado al carrito",
                        Platillo = _platilloCarritoSeleccionado

                    };
                    CargarContenidoPlatilloSeleccionado();
                    CarritoViewModel.Instance.AgregarPlatilloACarrito(_platilloCarritoSeleccionado, true);
                    Platillos.First(c => c.Id == PlatilloSeleccionado.Id).CantidadEnCarrito =
                        CarritoViewModel.Instance.ConteoPlatillosPorId(PlatilloSeleccionado.Id);
                    _platilloCarritoSeleccionado = null;
                    PlatilloTerminado?.Invoke(this, Argumentos);
                }
                else
                {
                    SubplatilloCompletado?.Invoke(this, new SubplatilloCompletado()
                    {
                        Success = true,
                        Message = $"{platillo2.Descripcion} seleccionado",
                        Seleccion = new SeleccionPlatilloMultiNivel()
                        {
                            IdNivel1 = new KeyValuePair<int, string>(platillo1.Id, platillo1.Descripcion),
                            IdNivel2 = new KeyValuePair<int, string>(platillo2.Id, platillo2.Descripcion),
                            IdNivel3 = new KeyValuePair<int, string>(0, string.Empty)
                        }
                    });
                }
            }
            else
            {
                _ultimoIdSegundoNivelSeleccionado = platillo2.Id;
                CargarSubmenu?.Invoke(this, new CargarSubmenuEventArgs
                {
                    Contenido = new List<BasePlatilloMultiNivel>(platillo2.Hijos),
                    Nivel = NivelesPlatillo.TercerNivel,
                    Success = true,
                });
            }
        }

        private void SeleccionarPlatilloNivel1(int idPlatillo)
        {
            var platillo = PlatilloSeleccionado.Hijos.First(c => c.Id == idPlatillo);


            if (platillo.EsTerminal)
            {
                _cantidadRestantePlatilloSeleccionado--;
                EliminarSeleccionPrevia(platillo.Id);
                _platillosSeleccionadosCombo.Add(new SeleccionPlatilloMultiNivel
                {
                    IdNivel1 = new KeyValuePair<int, string>(platillo.Id, platillo.Descripcion),
                    IdNivel2 = new KeyValuePair<int, string>(0, string.Empty),
                    IdNivel3 = new KeyValuePair<int, string>(0, string.Empty)
                });
                _platilloCarritoSeleccionado.Nivel1.Add(platillo.Id);
                //_platilloCarritoSeleccionado.Contenido += $", {platillo.Descripcion}";
                _platilloCarritoSeleccionado.Precio += platillo.Precio;
                if (_cantidadRestantePlatilloSeleccionado == 0)
                {
                    var Argumentos = new PlatilloTerminadoEventArgs
                    {
                        Success = true,
                        Message = $"{_platilloCarritoSeleccionado.Nombre} agregado al carrito",
                        Platillo = _platilloCarritoSeleccionado

                    };
                    CargarContenidoPlatilloSeleccionado();
                    CarritoViewModel.Instance.AgregarPlatilloACarrito(_platilloCarritoSeleccionado, true);
                    Platillos.First(c => c.Id == PlatilloSeleccionado.Id).CantidadEnCarrito =
                        CarritoViewModel.Instance.ConteoPlatillosPorId(PlatilloSeleccionado.Id);
                    _platilloCarritoSeleccionado = null;
                    PlatilloTerminado?.Invoke(this, Argumentos);
                }
                else
                {
                    SubplatilloCompletado?.Invoke(this, new SubplatilloCompletado()
                    {
                        Success = true,
                        Message = $"{platillo.Descripcion} seleccionado",
                        Seleccion = new SeleccionPlatilloMultiNivel()
                        {
                            IdNivel1 = new KeyValuePair<int, string>(platillo.Id, platillo.Descripcion),
                            IdNivel2 = new KeyValuePair<int, string>(0, string.Empty),
                            IdNivel3 = new KeyValuePair<int, string>(0, string.Empty),
                        }
                    });
                }
            }
            else
            {
                _ultimoIdPrimerNivelSeleccionado = platillo.Id;
                CargarSubmenu?.Invoke(this, new CargarSubmenuEventArgs
                {
                    Contenido = new List<BasePlatilloMultiNivel>(platillo.Hijos),
                    Nivel = NivelesPlatillo.SegundoNivel,
                    Success = true,
                });
            }
        }

        private void CargarContenidoPlatilloSeleccionado()
        {
            var contenido = new List<string> { _platilloCarritoSeleccionado.Nombre };
            foreach (var seleccionPlatilloMultiNivel in _platillosSeleccionadosCombo)
            {
                var platillo = new List<string>();
                if (seleccionPlatilloMultiNivel.IdNivel1.Key > 0)
                {
                    platillo.Add(seleccionPlatilloMultiNivel.IdNivel1.Value);
                }

                if (seleccionPlatilloMultiNivel.IdNivel2.Key > 0)
                {
                    platillo.Add(seleccionPlatilloMultiNivel.IdNivel2.Value);
                }

                if (seleccionPlatilloMultiNivel.IdNivel3.Key > 0)
                {
                    platillo.Add(seleccionPlatilloMultiNivel.IdNivel3.Value);
                }
                contenido.Add(string.Join(" : ", platillo));
            }

            _platilloCarritoSeleccionado.Contenido = string.Join(", ", contenido);
        }

        private void EliminarSeleccionPrevia(int idPrimerNivel)
        {
            var seleccionPrevia = _platillosSeleccionadosCombo.FirstOrDefault(c => c.IdNivel1.Key == idPrimerNivel);
            if (seleccionPrevia == null || _platilloCarritoSeleccionado == null) return;
            _cantidadRestantePlatilloSeleccionado++;
            if (seleccionPrevia.IdNivel1.Key > 0) _platilloCarritoSeleccionado.Nivel1.Remove(seleccionPrevia.IdNivel1.Key);
            if (seleccionPrevia.IdNivel2.Key > 0) _platilloCarritoSeleccionado.Nivel2.Remove(seleccionPrevia.IdNivel2.Key);
            if (seleccionPrevia.IdNivel3.Key > 0) _platilloCarritoSeleccionado.Nivel3.Remove(seleccionPrevia.IdNivel3.Key);
            _platillosSeleccionadosCombo.Remove(seleccionPrevia);
        }
        public bool EsTerminal(int idPlatillo, NivelesPlatillo nivel)
        {
            switch (nivel)
            {
                case NivelesPlatillo.PrimerNivel:
                    return PlatilloSeleccionado.Hijos.First(c => c.Id == idPlatillo).EsTerminal;
                case NivelesPlatillo.SegundoNivel:
                case NivelesPlatillo.TercerNivel:
                case NivelesPlatillo.NoDefinido:
                default:
                    throw new ArgumentOutOfRangeException(nameof(nivel), nivel, null);
            }
        }
        #endregion

        public void RemoverPlatillo(int id)
        {
            if (Platillos.First(c => c.Id == id).CantidadEnCarrito < 1) return;
            CarritoViewModel.Instance.RemoverPlatilloCarrito(id);
            Platillos.First(c => c.Id == id).CantidadEnCarrito =
                CarritoViewModel.Instance.ConteoPlatillosPorId(PlatilloSeleccionado.Id);
        }
        //public async void ObtenerPlatillosRandom()
        //{
        //    IsBusy = true;

        //    var response = await QdcApi.Restaurantes.LlamarObtenerPlatillosRandom();
        //    if (response.Estatus.IsSuccessful)
        //    {
        //        PlatillosRandom.Clear();
        //        foreach (var platillo in response.Resultados)
        //        {
        //            PlatillosRandom.Add(platillo);
        //        }
        //        OnObtenerPlatillosRandomFinished?.Invoke(this, new BaseEventArgs()
        //        {
        //            Success = true,
        //        });
        //    }
        //    else
        //    {
        //        OnObtenerPlatillosRandomFinished?.Invoke(this, new BaseEventArgs()
        //        {
        //            Success = false,
        //            Message = response.Estatus.Message
        //        });
        //    }

        //    IsBusy = false;
        //}
        #endregion

    }
}
