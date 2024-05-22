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
using MystiqueNative.Droid.HazPedido.Carrito;
using MystiqueNative.Droid.HazPedido.Ensaladas;
using MystiqueNative.Droid.HazPedido.Platillos;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Menú")]
    public class MenuRestauranteHazPedidoActivity : BaseActivity
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_menu_restaurante;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #endregion

        #region EXPORTS
        public const string ExtraIdRestaurante = "QDC_EXTRAIDRESTAURANTE";
        public const string ExtraReentry = "QDC_EXTRAREENTRY";
        public const string ExtraModoLectura = "QDC_EXTRAREENTRY.ExtraModoLectura";
        public const string ExtraDirectorio = "QDC_EXTRAREENTRY.ExtraDirectorio";
        public const string EXTRAACTIVITYCLIENTE = "SHOWCLIENTES";
        #endregion

        #region FIELDS
        private MenuRestauranteHazPedidoAdapter _adapter;
        private int _idRestaurante;
        private bool _isReentry;
        private bool _isModoLectura;
        private bool _isDirectorio;
        private FloatingActionButton _fab;
        private FrameLayout _progressBarHolder;
        private View _layoutTotal;
        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var idRestaurante = Intent.GetIntExtra(ExtraIdRestaurante, 0);
            _isReentry = Intent.GetBooleanExtra(ExtraReentry, false);
            _isModoLectura = Intent.GetBooleanExtra(ExtraModoLectura, false);
            _isDirectorio = Intent.GetBooleanExtra(ExtraDirectorio, false);
            if (idRestaurante == 0 && !_isReentry) throw new ArgumentException(nameof(ExtraIdRestaurante));
            _idRestaurante = _isReentry
                ? RestaurantesViewModel.Instance.RestauranteActivo.Id
                : idRestaurante;
            GrabViews();
        }

        private async void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _layoutTotal = FindViewById(Resource.Id.menu_restaurante_ly_total);
            _fab = FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.fab);
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _adapter = _isDirectorio
                ? new MenuRestauranteHazPedidoAdapter(this, DirectorioViewModel.Instance.MenuDirectorioResturantes)
                : new MenuRestauranteHazPedidoAdapter(this, RestaurantesViewModel.Instance.MenuRestauranteActivo);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(_adapter);

            _adapter.ItemClick += Adapter_Button1Click;
            _adapter.NotifyDataSetChanged();

            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            recyclerView.AddItemDecoration(itemDecor);

            _fab.Click += Fab_Click;


        }

        protected override async void OnResume()
        {
            base.OnResume();
            StartAnimatingLogin();
            UpdateTotalCarrito();
            DirectorioViewModel.Instance.OnObtenerMenuRestauranteFinished += Instance_OnObtenerMenuRestauranteFinished;
            RestaurantesViewModel.Instance.OnObtenerMenuRestauranteFinished += Instance_OnObtenerMenuRestauranteFinished1;
            _layoutTotal.Visibility = !_isDirectorio ? ViewStates.Visible : ViewStates.Gone;
            _fab.Visibility = CarritoViewModel.Instance.ExistenItemsEnCarrito && !_isDirectorio && !_isModoLectura ? ViewStates.Visible : ViewStates.Gone;

            if (_isDirectorio)
            {
                await DirectorioViewModel.Instance.ObtenerMenuRestaurante(_idRestaurante);
                Title = DirectorioViewModel.Instance.RestauranteActivo.Nombre;
            }
            else
            {
                await RestaurantesViewModel.Instance.ObtenerMenuRestaurante(_idRestaurante);
                Title = RestaurantesViewModel.Instance.RestauranteActivo.Nombre;
            }

        }

        protected override void OnPause()
        {
            base.OnPause();
            DirectorioViewModel.Instance.OnObtenerMenuRestauranteFinished -= Instance_OnObtenerMenuRestauranteFinished;
            RestaurantesViewModel.Instance.OnObtenerMenuRestauranteFinished -= Instance_OnObtenerMenuRestauranteFinished1;
            StopAnimatingLogin(false);
        }

        #endregion

        #region ACTIVITY OVERRIDES
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

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        #endregion

        #region EVENTS
        private void Adapter_Button1Click(object sender, RecyclerClickEventArgs e)
        {
            UpdateTotalCarrito();
            var item = _isDirectorio
                ? DirectorioViewModel.Instance.MenuDirectorioResturantes[e.Position]
                : RestaurantesViewModel.Instance.MenuRestauranteActivo[e.Position];
            if (item.FlujoEnsalada)
            {
                var intent = new Intent(this, typeof(EnsaladasActivity));
                intent.PutExtra(EnsaladasActivity.ExtraEsDirectorio, _isDirectorio);
                intent.PutExtra(EnsaladasActivity.ExtraModoLectura, _isModoLectura || _isDirectorio);
                StartActivity(intent);
            }
            else
            {

                var intent = new Intent(this, typeof(PlatillosActivity));

                intent.PutExtra(PlatillosActivity.ExtraSoloLectura, _isModoLectura || _isDirectorio);
                intent.PutExtra(PlatillosActivity.ExtraIdMenu, item.Id);
                intent.PutExtra(PlatillosActivity.ExtraTitleMenu, item.Nombre);
                intent.PutExtra(PlatillosActivity.ExtraIdRestaurante, _idRestaurante);
                StartActivity(intent);
            }
        }

        private void Fab_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(CarritoActivity));
        } 
        #endregion

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

        #region CALLBACKS
        private void Instance_OnObtenerMenuRestauranteFinished1(object sender, BaseEventArgs e)
        {
            StopAnimatingLogin();
        }

        private void Instance_OnObtenerMenuRestauranteFinished(object sender, BaseEventArgs e)
        {
            StopAnimatingLogin();
        } 
        #endregion
    }
}