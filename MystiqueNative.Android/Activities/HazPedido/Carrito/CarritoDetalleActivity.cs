using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Carrito
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "@string/carrito_detalle_title")]
    public class CarritoDetalleActivity : BaseActivity
    {

        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_carrito_detalle;
        protected override int BackButtonIcon => Resource.Drawable.ic_close_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const string ExtraTitle = "QDCEXTRATITLE";
        public const string ExtraImagen = "QDCEXTRAIMAGEN";
        public const string ExtraPrecio = "QDCEXTRAPRECIO";
        public const string ExtraContent = "QDCEXTRACONTENT";
        public const string ExtraHash = "QDCEXTRAHASH";

        #endregion

        #region VIEWS

        private TextView _labelTitle;
        private ImageViewAsync _imageHeader;
        private TextView _labelContent;
        private TextView _labelPrecio;
        private Button _buttonRemover;

        #endregion

        #region FIELDS

        private string _hash;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            GrabIntentParameters();
        }

        private void GrabIntentParameters()
        {
            _hash = Intent.GetStringExtra(ExtraHash);
            if (string.IsNullOrEmpty(_hash)) throw new ArgumentNullException(nameof(ExtraHash));
            _labelTitle.Text = Intent.GetStringExtra(ExtraTitle);
            _labelPrecio.Text = Intent.GetStringExtra(ExtraPrecio);
            _labelContent.Text = Intent.GetStringExtra(ExtraContent);
            ImageService.Instance
                .LoadUrl(Intent.GetStringExtra(ExtraImagen))
                .Into(_imageHeader);
        }

        private void GrabViews()
        {
            _imageHeader = FindViewById<ImageViewAsync>(Resource.Id.carrito_detalle_imagen);
            _labelTitle = FindViewById<TextView>(Resource.Id.carrito_detalle_label_title);
            _labelContent = FindViewById<TextView>(Resource.Id.carrito_detalle_label_content);
            _labelPrecio = FindViewById<TextView>(Resource.Id.carrito_detalle_label_precio);
            _buttonRemover = FindViewById<Button>(Resource.Id.button_remove);

            _buttonRemover.Click += ButtonRemover_Click;
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

        #region ACTIVITY OVERRIDES

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.menu_nota, menu);

        //    return true;
        //}
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home) OnBackPressed();
            return true;
        }
        #endregion

        private void ButtonRemover_Click(object sender, System.EventArgs e)
        {
            SendConfirmation("¿Estás seguro que deseas eliminarlo de tu carrito?", "", "Eliminar", "Cancelar", ok =>
            {
                if (!ok) return;
                CarritoViewModel.Instance.RemoverItemCarrito(Guid.Parse(_hash));
                Finish();
            });
        }
    }
}