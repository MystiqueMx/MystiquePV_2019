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

namespace MystiqueNative.Droid.HazPedido.Carrito
{
    [Activity(Label = "@string/carrito_nota_title", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CarritoNotaActivity : BaseActivity
    {

        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_carrito_nota;
        protected override int BackButtonIcon => Resource.Drawable.ic_close_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const string ExtraHash = "QDCNOTAEXTRAHASH";

        #endregion

        #region VIEWS

        private FloatingActionButton _fab;
        private TextInputLayout _layoutNotas;
        private TextInputEditText _entryNotas;

        #endregion

        #region FIELDS

        private Guid _hash;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            GrabIntentParameters();
            ObtenerNotaItem();
        }

        private void ObtenerNotaItem()
        {

            var item = CarritoViewModel.Instance.Items.First(c => c.Hash == _hash);
            _entryNotas.Text = item.Notas;
            if (_entryNotas.Text.Length > 0)
            {
                _fab.Visibility = ViewStates.Visible;
            }
        }

        private void GrabIntentParameters()
        {
            var hash = Intent.GetStringExtra(ExtraHash);
            if (string.IsNullOrEmpty(hash)) throw new ArgumentNullException(nameof(ExtraHash));
            _hash = Guid.Parse(hash);
        }

        private void GrabViews()
        {
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            _layoutNotas = FindViewById<TextInputLayout>(Resource.Id.carrito_nota_layout_texto);
            _entryNotas = FindViewById<TextInputEditText>(Resource.Id.carrito_nota_entry_texto);
            _entryNotas.TextChanged += (s, e) => _fab.Visibility = e.AfterCount > 0 ? ViewStates.Visible : ViewStates.Gone;
            _fab.Click += Fab_Click;
        }

        private void Fab_Click(object sender, System.EventArgs e)
        {
            ViewModels.CarritoViewModel.Instance.AgregarNotaPlatillo(_hash, _entryNotas.Text);
            Finish();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
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
    }
}