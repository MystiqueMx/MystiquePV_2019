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
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Ordenes
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Enviar mensaje de seguimiento")]
    public class OrdenSeguimientoActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_orden_seguimiento;
        protected override int BackButtonIcon => Resource.Drawable.ic_close_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const string ExtraIdPedido = "QDCNOTAEXTRAIDPEDIDO";
        public const int RequestAgregarSeguimiento = 5783;
        #endregion

        #region VIEWS

        private FloatingActionButton _fab;
        private TextInputLayout _layoutNotas;
        private TextInputEditText _entryNotas;

        #endregion

        #region FIELDS

        private int _idPedido;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabIntentParameters();
            GrabViews();

        }
        private void GrabIntentParameters()
        {
            var id = Intent.GetIntExtra(ExtraIdPedido, -1);
            if (id == -1) throw new ArgumentNullException(nameof(ExtraIdPedido));
            _idPedido = id;
        }

        private void GrabViews()
        {
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            _layoutNotas = FindViewById<TextInputLayout>(Resource.Id.orden_seguimiento_layout_mensaje);
            _entryNotas = FindViewById<TextInputEditText>(Resource.Id.orden_seguimiento_entry_mensaje);
            _entryNotas.TextChanged += (s, e) => _fab.Visibility = e.AfterCount > 0 ? ViewStates.Visible : ViewStates.Gone;
            _fab.Click += Fab_Click;
            FindViewById<TextView>(Resource.Id.item_folio).Text = $"Seguimiento al pedido #{_idPedido:D10}";
        }

        private async void Fab_Click(object sender, System.EventArgs e)
        {
            await PedidosViewModel.Instance.AgregarSeguimiento(_idPedido, _entryNotas.Text);
        }

        protected override void OnResume()
        {
            base.OnResume();
            PedidosViewModel.Instance.OnAgregarSeguimientoFinished += Instance_OnAgregarSeguimientoFinished;
        }


        protected override void OnPause()
        {
            base.OnPause();
            PedidosViewModel.Instance.OnAgregarSeguimientoFinished -= Instance_OnAgregarSeguimientoFinished;
        }

        #endregion

        private void Instance_OnAgregarSeguimientoFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            if (e.Success)
            {
                SetResult(Result.Ok);
                Finish();
            }
            else
            {
                SendMessage(e.Message);
            }
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
    }
}