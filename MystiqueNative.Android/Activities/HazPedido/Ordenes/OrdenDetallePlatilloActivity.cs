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
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MystiqueNative.Droid.Helpers;

namespace MystiqueNative.Droid.HazPedido.Ordenes
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Descripcion del platillo")]
    public class OrdenDetallePlatilloActivity : BaseActivity
    {

        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_orden_detalle_platillo;
        protected override int BackButtonIcon => Resource.Drawable.ic_close_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const string ExtraTitle = "QDCEXTRATITLEPlatillo";
        public const string ExtraImagen = "QDCEXTRAIMAGENPlatillo";
        public const string ExtraPrecio = "QDCEXTRAPRECIOPlatillo";
        public const string ExtraContent = "QDCEXTRACONTENTPlatillo";

        #endregion

        #region VIEWS

        private TextView _labelTitle;
        private ImageViewAsync _imageHeader;
        private TextView _labelContent;
        private TextView _labelPrecio;

        #endregion

        #region FIELDS


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

    }
}