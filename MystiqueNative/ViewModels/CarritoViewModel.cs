using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Carrito;
using MystiqueNative.Models.Orden;


using System.Text;

namespace MystiqueNative.ViewModels
{
    public class CarritoViewModel : BaseViewModel
    {
        #region SINGLETON
        public static CarritoViewModel Instance => _instance ?? (_instance = new CarritoViewModel());
        private static CarritoViewModel _instance;

        public static void Destroy() => _instance = null;
        #endregion

        #region CTOR

        public CarritoViewModel()
        {
            Items = new ObservableCollection<ItemCarrito>();
        }

        #endregion

        #region EVENTS

        public event EventHandler<BaseEventArgs> OnCarritoUpdated;

        #endregion

        #region FIELDS

        public Carrito PedidoActual { get; private set; }
        public ObservableCollection<ItemCarrito> Items { get; set; }
        public string SubTotalActual => PedidoActual == null ? "$0.00 MXN" : $"{PedidoActual.SubTotal:C} MXN";
        public string TotalActual(TipoReparto tipoReparto) => $"{TotalActualAsDecimal(tipoReparto)} MXN";
        public decimal TotalActualAsDecimal(TipoReparto tipoReparto)
        {
            if (PedidoActual == null)
            {
                return 0M;
            }

            return tipoReparto == TipoReparto.Particular
                ? PedidoActual.SubTotal + PedidoActual.Restaurante.CostoEnvio
                : PedidoActual.SubTotal;
        }

        public bool ExistenItemsEnCarrito => PedidoActual != null && PedidoActual.SubTotal > 0;
        public int ConteoPlatillosPorId(int id) => PedidoActual?.Platillos?.Count(c => c.Id == id) ?? 0;
        public int ConteoPlatillosPorOrden() => PedidoActual?.Platillos?.Count + PedidoActual?.Ensaladas?.Count ?? 0;
        #endregion

        public bool AgregarEnsaladaACarrito(EnsaladaCarrito ensalada, bool forcePush = false)
        {
            if (PedidoActual == null
                || (forcePush && PedidoActual.Restaurante.Id !=
                    RestaurantesViewModel.Instance.RestauranteActivo.Id)) CreateNewPedido();
            if (PedidoActual.Restaurante.Id == RestaurantesViewModel.Instance.RestauranteActivo.Id)
            {
                ensalada.Hash = Guid.NewGuid();
                ensalada.TipoItem = TipoPlatillos.Ensalada;
                PedidoActual.Ensaladas.Add(ensalada);
                PedidoActual.SubTotal += ensalada.Precio;
                Items.Add(ensalada);
                OnCarritoUpdated?.Invoke(this, new BaseEventArgs
                {
                    Success = true,
                    Message = $"Tu platillo se ha agregado al carrito con éxito"
                });
                return true;
            }
            else
            {
                OnCarritoUpdated?.Invoke(this, new BaseEventArgs
                {
                    Success = false,
                    Message = $"Ya cuentas con un platillos seleccionados en {PedidoActual.Restaurante.Descripcion}, ¿deseas desecharlo y ordenar en {RestaurantesViewModel.Instance.RestauranteActivo.Descripcion}?"
                });
                return false;
            }

        }

        public void AgregarPlatilloACarrito(PlatilloCarrito platillo, bool forcePush = false)
        {
            if (PedidoActual == null
                || forcePush && PedidoActual.Restaurante.Id !=
                        RestaurantesViewModel.Instance.RestauranteActivo.Id) CreateNewPedido();
            if (PedidoActual.Restaurante.Id == RestaurantesViewModel.Instance.RestauranteActivo.Id)
            {
                platillo.Hash = Guid.NewGuid();
                platillo.TipoItem = TipoPlatillos.Platillo;
                PedidoActual.Platillos.Add(platillo);
                PedidoActual.SubTotal += platillo.Precio;
                Items.Add(platillo);
                OnCarritoUpdated?.Invoke(this, new BaseEventArgs
                {
                    Success = true,
                    Message = $"Tu platillo se ha agregado al carrito con éxito"
                });
            }
            else
            {
                OnCarritoUpdated?.Invoke(this, new BaseEventArgs
                {
                    Success = false,
                    Message = $"Ya cuentas con un pedido en {PedidoActual.Restaurante.Descripcion}, ¿deseas desecharlo y continuar tu orden en {RestaurantesViewModel.Instance.RestauranteActivo.Descripcion}?"
                });
            }
        }

        public void RemoverItemCarrito(Guid hashToRemove)
        {
            var item = Items.First(c => c.Hash == hashToRemove);
            Items.Remove(item);
            switch (item.TipoItem)
            {
                case TipoPlatillos.Ensalada:
                    var ensalada = PedidoActual.Ensaladas.First(c => c.Hash == item.Hash);
                    PedidoActual.Ensaladas.Remove(ensalada);
                    PedidoActual.SubTotal -= ensalada.Precio;
                    break;
                case TipoPlatillos.Platillo:
                    var platillo = PedidoActual.Platillos.First(c => c.Hash == item.Hash);
                    PedidoActual.Platillos.Remove(platillo);
                    PedidoActual.SubTotal -= platillo.Precio;
                    break;
                case TipoPlatillos.NoDefinido:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnCarritoUpdated?.Invoke(this, new BaseEventArgs
            {
                Success = true,
                Message = $"Tu platillo se ha removido del carrito con éxito"
            });
        }

        public void RemoverPlatilloCarrito(int idPlatillo)
        {
            var item = Items.Last(c => c.Id == idPlatillo);
            var platillo = PedidoActual.Platillos.Last(c => c.Hash == item.Hash);
            Items.Remove(item);
            PedidoActual.Platillos.Remove(platillo);
            PedidoActual.SubTotal -= platillo.Precio;
            OnCarritoUpdated?.Invoke(this, new BaseEventArgs
            {
                Success = true,
                Message = $"Tu platillo se ha removido del carrito con éxito"
            });
        }

        public void AgregarNotaPlatillo(Guid hashToUpdate, string nota)
        {
            var item = Items.First(c => c.Hash == hashToUpdate);
            switch (item.TipoItem)
            {

                case TipoPlatillos.Ensalada:
                    var ensalada = PedidoActual.Ensaladas.First(c => c.Hash == item.Hash);
                    ensalada.Notas = nota;
                    break;
                case TipoPlatillos.Platillo:
                    var platillo = PedidoActual.Platillos.First(c => c.Hash == item.Hash);
                    platillo.Notas = nota;
                    break;
                case TipoPlatillos.NoDefinido:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CreateNewPedido()
        {
            PedidoActual = new Carrito
            {
                Ensaladas = new List<EnsaladaCarrito>(),
                Platillos = new List<PlatilloCarrito>(),
                Restaurante = RestaurantesViewModel.Instance.RestauranteActivo,
                SubTotal = 0m,
                Total = null,
            };
            Items.Clear();
        }

        public void TerminarPedido()
        {
            PedidoActual = null;
        }
    }
}
