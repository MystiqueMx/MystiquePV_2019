using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.Droid.HazPedido.Tarjetas;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Carrito
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Selección de metodo de pago")]
    public class CarritoMetodoPagoActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_carrito_metodo_pago;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const int RequestMetodoPago = 6729;
        public const string ExtraTarjeta = "Qdec.CarritoMetodoPagoActivity.ExtraTarjeta";

        #endregion

        #region VIEWS

        private CardView _cardEfectivo;
        private CardView _cardAgregarNueva;

        #endregion

        #region FIELDS

        private TarjetaAdapter _adapter;
        private int _selectedItemIndex;
        private FrameLayout _progressBarHolder;
        private TextView _labelEfectivo;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            SetUpRecyclerView();
        }
        private void SetUpRecyclerView()
        {
            var recyclerview = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _adapter = new TarjetaAdapter(this, ViewModels.TarjetasViewModel.Instance.Tarjetas);
            _labelEfectivo = FindViewById<TextView>(Resource.Id.carrito_label_efectivo);
            recyclerview.HasFixedSize = true;
            recyclerview.SetAdapter(_adapter);
            _adapter.NotifyDataSetChanged();
            _adapter.ItemClick += Adapter_ItemClick;
            _adapter.ItemLongClick += Adapter_ItemLongPress;
        }


        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);

            _cardEfectivo = FindViewById<CardView>(Resource.Id.card_efectivo);
            _cardAgregarNueva = FindViewById<CardView>(Resource.Id.card_agregar_nueva);
            _cardEfectivo.Click += CardEfectivo_Click;
            _cardAgregarNueva.Click += CardAgregarNueva_Click;
        }

        protected override async void OnResume()
        {
            base.OnResume();
            StartAnimating();
            TarjetasViewModel.Instance.OnObtenerTarjetasFinished += Instance_OnObtenerTarjetasFinished;
            TarjetasViewModel.Instance.OnRemoverTarjetaFinished += Instance_OnRemoverTarjetaFinished;
            _labelEfectivo.Text = CarritoViewModel.Instance.PedidoActual.Restaurante.TieneRepartoADomicilio
                ? "El cobro será realizado por el repartidor."
                : "El cobro será realizado en sucursal";
            await ViewModels.TarjetasViewModel.Instance.ObtenerTarjetas();
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModels.TarjetasViewModel.Instance.OnObtenerTarjetasFinished -= Instance_OnObtenerTarjetasFinished;
            ViewModels.TarjetasViewModel.Instance.OnRemoverTarjetaFinished -= Instance_OnRemoverTarjetaFinished;
        }

        #endregion

        private void Adapter_ItemLongPress(object sender, RecyclerClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            var bs = TarjetasBottomSheet.Instance;
            bs.OnDeleteSelected += Bs_OnDeleteSelected;
            bs.Show(SupportFragmentManager, "tbs");
        }

        private async void Bs_OnDeleteSelected(object sender, System.EventArgs e)
        {
            var item = TarjetasViewModel.Instance.Tarjetas[_selectedItemIndex];
            await ViewModels.TarjetasViewModel.Instance.RemoverTarjeta(item.Id);
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var item = TarjetasViewModel.Instance.Tarjetas[e.Position];
            var intent = new Intent();
            intent.PutExtra(ExtraTarjeta, item.Id);
            SetResult(Result.Ok, intent);
            Finish();
            OverridePendingTransition(0, 0);
        }
        private void CardAgregarNueva_Click(object sender, System.EventArgs e)
        {
            StartActivityForResult(typeof(EdicionTarjetaActivity), EdicionTarjetaActivity.RequestAgregarTarjeta);
            OverridePendingTransition(0, 0);
        }

        private void CardEfectivo_Click(object sender, System.EventArgs e)
        {
            SetResult(Result.FirstUser);
            Finish();
            OverridePendingTransition(0, 0);
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
        private void Instance_OnObtenerTarjetasFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            StopAnimating();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok && requestCode == EdicionTarjetaActivity.RequestAgregarTarjeta)
            {
                var id = data.GetStringExtra(EdicionTarjetaActivity.ExtraIdEdicion);
                if (string.IsNullOrEmpty(id)) return;
                var intent = new Intent();
                intent.PutExtra(ExtraTarjeta, id);
                SetResult(Result.Ok, intent);
                Finish();
                OverridePendingTransition(0, 0);
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void Instance_OnRemoverTarjetaFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            if (!e.Success && !string.IsNullOrEmpty(e.Message))
            {
                SendMessage(e.Message);
            }
        }
    }
}