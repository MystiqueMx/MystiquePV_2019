using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using Firebase.Analytics;
using MystiqueNative.Droid.HazPedido.Carrito;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;
using Newtonsoft.Json;
using ViewModel = MystiqueNative.ViewModels.PlatillosViewModel;

namespace MystiqueNative.Droid.HazPedido.Platillos
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Platillos")]
    public class PlatillosActivity : BaseActivity
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_platillos;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #endregion

        #region EXPORTS
        public const string ExtraIdRestaurante = "QDC_EXTRAIDRESTAURANTE";
        public const string ExtraIdMenu = "QDC_EXTRAIDMENU";
        public const string ExtraTitleMenu = "QDC_EXTRATITLEMENU";
        public const string ExtraSoloLectura = "QDC_EXTRATITLEMENU.ExtraSoloLectura";
        #endregion

        #region FIELDS
        private FirebaseAnalytics _firebaseAnalytics;
        private int _idMenu, _idRestaurante;
        private PlatillosRestauranteAdapter _adapter;
        private string _title;
        private int _resultRequestId;
        private FloatingActionButton _fab;
        private int _ultimaPosicionSeleccionada;
        private bool _isSoloLectura;
        private View _layoutTotal;
        private FrameLayout _progressBarHolder;
        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            var idMenu = Intent.GetIntExtra(ExtraIdMenu, 0);
            _title = Title = Intent.GetStringExtra(ExtraTitleMenu);
            _idRestaurante = Intent.GetIntExtra(ExtraIdRestaurante, 0);
            _isSoloLectura = Intent.GetBooleanExtra(ExtraSoloLectura, false);
            if (idMenu == 0) throw new ArgumentException(nameof(ExtraIdMenu));
            _idMenu = idMenu;
            GrabViews();
        }

        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _fab = FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.fab);
            _layoutTotal = FindViewById(Resource.Id.menu_restaurante_ly_total);
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _adapter = new PlatillosRestauranteAdapter(this, ViewModel.Instance.Platillos, _isSoloLectura);
            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(_adapter);

            _adapter.ItemClick += Adapter_ButtonMinusClick;
            _adapter.ItemClick2 += Adapter_ButtonPlusClick;
            _adapter.ItemLongClick += Adapter_ItemNotesClick;
            _adapter.NotifyDataSetChanged();

            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            recyclerView.AddItemDecoration(itemDecor);

            _fab.Click += Fab_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
            ViewModel.Instance.OnObtenerPlatillosMenuFinished += Instance_OnObtenerPlatillosMenuFinished;
            StartAnimatingLogin();
            ViewModel.Instance.ObtenerPlatillosMenu(_idMenu, _idRestaurante);
            ViewModel.Instance.PlatilloTerminado += Instance_PlatilloTerminado;
            ViewModel.Instance.CargarSubmenu += Instance_CargarSubmenu;
            Title = _title;
            UpdateTotalCarrito();
            _layoutTotal.Visibility = !_isSoloLectura ? ViewStates.Visible : ViewStates.Gone;
            _fab.Visibility = CarritoViewModel.Instance.ExistenItemsEnCarrito && !_isSoloLectura ? ViewStates.Visible : ViewStates.Gone;
        }


        protected override void OnPause()
        {
            base.OnPause();
            ViewModel.Instance.PlatilloTerminado -= Instance_PlatilloTerminado;
            ViewModel.Instance.CargarSubmenu -= Instance_CargarSubmenu;
            ViewModel.Instance.OnObtenerPlatillosMenuFinished -= Instance_OnObtenerPlatillosMenuFinished;
            StopAnimatingLogin(false);
        }

        #endregion

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return true;
            }
        }

        private void Adapter_ItemNotesClick(object sender, RecyclerClickEventArgs e)
        {
            SendToast("@TODO");
        }

        private void Adapter_ButtonPlusClick(object sender, RecyclerClickEventArgs e)
        {
            if (this.IsFinishing) return;
            _ultimaPosicionSeleccionada = e.Position;
            var id = ViewModel.Instance.Platillos[_ultimaPosicionSeleccionada].Id;
            if (CarritoViewModel.Instance.ExistenItemsEnCarrito &&
                CarritoViewModel.Instance.PedidoActual.Restaurante.Id !=
                RestaurantesViewModel.Instance.RestauranteActivo.Id)
            {
                SendConfirmation($"Ya cuentas con un platillos seleccionados en {CarritoViewModel.Instance.PedidoActual.Restaurante.Nombre}, ¿deseas desecharlo y ordenar en {RestaurantesViewModel.Instance.RestauranteActivo.Nombre}?", "", "Continuar", "Salir",
                    continuar =>
                    {
                        if (continuar)
                        {
                            ViewModel.Instance.SeleccionarPlatillo(id);
                            UpdateTotalCarrito();
                        }
                        else
                        {
                            Finish();
                        }
                    });
            }
            else
            {
                ViewModel.Instance.SeleccionarPlatillo(id);
                UpdateTotalCarrito();
            }

            _fab.Visibility = CarritoViewModel.Instance.ExistenItemsEnCarrito ? ViewStates.Visible : ViewStates.Gone;
        }

        private void Adapter_ButtonMinusClick(object sender, RecyclerClickEventArgs e)
        {
            if (this.IsFinishing) return;
            ViewModel.Instance.RemoverPlatillo(ViewModel.Instance.Platillos[e.Position].Id);
            _adapter.NotifyItemChanged(e.Position);
            UpdateTotalCarrito();
            _fab.Visibility = CarritoViewModel.Instance.ExistenItemsEnCarrito ? ViewStates.Visible : ViewStates.Gone;
        }

        private void Instance_CargarSubmenu(object sender, EventsArgs.CargarSubmenuEventArgs e)
        {
            _resultRequestId = new Random().Next(0, 2000);
            var intent = new Intent(this, typeof(SubPlatillosActivity));
            var items = JsonConvert.SerializeObject(e.Contenido);
            intent.PutExtra(SubPlatillosActivity.ExtraTitleMenu, "");
            intent.PutExtra(SubPlatillosActivity.ExtraContenidosMenu, items);
            intent.PutExtra(SubPlatillosActivity.ExtraNivelMenu, (int)e.Nivel);
            StartActivityForResult(intent, _resultRequestId);
        }

        private void Instance_PlatilloTerminado(object sender, EventsArgs.PlatilloTerminadoEventArgs e)
        {
            //if (MainApplication.LogAnalytics)
            //{
            //    var bundle = new Bundle();
            //    bundle.PutString(FirebaseAnalytics.Param.ItemName, e.Platillo.Nombre);
            //    bundle.PutString(FirebaseAnalytics.Param.ItemCategory, $"{CarritoViewModel.Instance.PedidoActual.Restaurante.Nombre}");
            //    _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.AddToCart, bundle);
            //}
            _adapter.NotifyItemChanged(e.Posicion);
            SendToast(e.Message);
            UpdateTotalCarrito();
        }

        private void Fab_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(CarritoActivity));
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == _resultRequestId && resultCode == Result.Ok)
            {
                _fab.Visibility = CarritoViewModel.Instance.ExistenItemsEnCarrito ? ViewStates.Visible : ViewStates.Gone;
                _adapter.NotifyItemChanged(_ultimaPosicionSeleccionada);
            }
            else
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }
        }

        #region LOADING
        private void StartAnimatingLogin()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 100
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }

        private void StopAnimatingLogin(bool animate = true)
        {
            if (animate)
            {

                var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
                {
                    Duration = 100
                };
                _progressBarHolder.Animation = outAnimation;
            }
            _progressBarHolder.Visibility = ViewStates.Gone;
        }
        #endregion

        private void Instance_OnObtenerPlatillosMenuFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            StopAnimatingLogin();
        }
    }
}