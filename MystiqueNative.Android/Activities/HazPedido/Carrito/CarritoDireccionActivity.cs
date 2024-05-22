using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location.Places.UI;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.Droid.HazPedido.Direccion;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Carrito
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Selección de lugar de entrega")]
    public class CarritoDireccionActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_carrito_direccion;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const int RequestDireccion = 6723;
        public const int RequestMaps = 1093;

        public const string ExtraDirection = "com.qdc.extradirection";
        #endregion

        #region VIEWS

        private CardView _cardRecogerSucursal;
        private CardView _cardAgregarDireccionNueva;


        #endregion

        #region FIELDS

        private DireccionAdapter _adapter;
        private int _selectedItemIndex;
        private FrameLayout _progressBarHolder;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
        }

        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _cardRecogerSucursal = FindViewById<CardView>(Resource.Id.card_recoger_sucursal);
            _cardAgregarDireccionNueva = FindViewById<CardView>(Resource.Id.card_agregar_nueva);
            _cardRecogerSucursal.Click += CardRecogerSucursal_Click;
            _cardAgregarDireccionNueva.Click += CardAgregarDireccionNueva_Click;

            SetUpRecyclerView();
        }

        private void SetUpRecyclerView()
        {
            var recyclerview = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _adapter = new DireccionAdapter(this, ViewModels.DireccionesViewModel.Instance.Direcciones);
            recyclerview.HasFixedSize = true;
            recyclerview.SetAdapter(_adapter);
            _adapter.NotifyDataSetChanged();
            _adapter.ItemClick += Adapter_ItemClick;
            _adapter.ItemLongClick += Adapter_ItemLongPress;
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartAnimating();
            _cardAgregarDireccionNueva.Enabled = true;
            DireccionesViewModel.Instance.OnObtenerDireccionesFinished += Instance_OnObtenerDireccionesFinished;
            DireccionesViewModel.Instance.ObtenerDirecciones(CarritoViewModel.Instance.PedidoActual.Restaurante.Id);
            DireccionesViewModel.Instance.OnEditarDireccionFinished += Instance_OnEditarDireccionFinished;
        }


        protected override void OnPause()
        {
            base.OnPause();
            DireccionesViewModel.Instance.OnEditarDireccionFinished -= Instance_OnEditarDireccionFinished;
            DireccionesViewModel.Instance.OnObtenerDireccionesFinished -= Instance_OnObtenerDireccionesFinished;
        }

        #endregion

        #region ACTIVITY OVERRIDES

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == RequestMaps && resultCode == Result.Ok)
            {
                var place = PlacePicker.GetPlace(this, data);
#if DEBUG
                var toastMsg = $"NameFormatted: {place.NameFormatted}, AddressFormatted: {place.AddressFormatted}, LatLng:({place.LatLng.Latitude}, {place.LatLng.Longitude}), PhoneNumberFormatted: {place.PhoneNumberFormatted}, AttributionsFormatted:{place.AttributionsFormatted}";
                SendToast(toastMsg);
#endif
                var intent = new Intent(this, typeof(EdicionDireccionActivity));
                intent.PutExtra(EdicionDireccionActivity.ExtraLatitud, place.LatLng.Latitude);
                intent.PutExtra(EdicionDireccionActivity.ExtraLongitud, place.LatLng.Longitude);
                StartActivityForResult(intent, EdicionDireccionActivity.RequestNuevaDireccion);
                OverridePendingTransition(0, 0);

            }

            if ((requestCode == EdicionDireccionActivity.RequestNuevaDireccion
                 || requestCode == EdicionDireccionActivity.RequestEditarDireccion)
                && resultCode == Result.Ok)
            {
                var id = data.GetIntExtra(EdicionDireccionActivity.ExtraIdEdicion, 0);
                if (id == 0) return;
                var intent = new Intent();
                intent.PutExtra(ExtraDirection, id);
                SetResult(Result.Ok, intent);
                Finish();
                OverridePendingTransition(0, 0);
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
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
        #endregion

        private void CardAgregarDireccionNueva_Click(object sender, System.EventArgs e)
        {
            _cardAgregarDireccionNueva.Enabled = false;
            var builder = new PlacePicker.IntentBuilder();

            StartActivityForResult(builder.Build(this).AddFlags(ActivityFlags.ReorderToFront), RequestMaps);
            OverridePendingTransition(0, 0);
        }

        private void CardRecogerSucursal_Click(object sender, System.EventArgs e)
        {
            SetResult(Result.FirstUser);
            Finish();
        }

        private void Adapter_ItemLongPress(object sender, RecyclerClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            var bs = DireccionesBottomSheet.Instance;
            bs.OnEditSelected += Bs_OnEditSelected;
            bs.OnDeleteSelected += Bs_OnDeleteSelected;
            bs.Show(SupportFragmentManager, "bsd");
        }

        private async void Bs_OnDeleteSelected(object sender, System.EventArgs e)
        {
            var item = DireccionesViewModel.Instance.Direcciones[_selectedItemIndex];
            await DireccionesViewModel.Instance.EliminarDireccion(item);
        }

        private void Bs_OnEditSelected(object sender, System.EventArgs e)
        {
            var item = DireccionesViewModel.Instance.Direcciones[_selectedItemIndex];
            var intent = new Intent(this, typeof(EdicionDireccionActivity));
            intent.PutExtra(EdicionDireccionActivity.ExtraIdEdicion, item.Id);
            StartActivityForResult(intent, EdicionDireccionActivity.RequestEditarDireccion);
            OverridePendingTransition(0, 0);
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var item = DireccionesViewModel.Instance.Direcciones[e.Position];
            var intent = new Intent();
            intent.PutExtra(ExtraDirection, item.Id);
            SetResult(Result.Ok, intent);
            Finish();
            OverridePendingTransition(0, 0);
        }

        private void Instance_OnEditarDireccionFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {

            if (e.Success || string.IsNullOrEmpty(e.Message)) return;
            SendMessage(e.Message);
        }

        #region LOADING

        private void StartAnimating()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimating()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }

        #endregion

        private void Instance_OnObtenerDireccionesFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            StopAnimating();
        }
    }
}