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

namespace MystiqueNative.Droid.HazPedido.Historial
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Calificar el servicio")]
    public class HistorialCalificarActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_historial_calificar;
        protected override int BackButtonIcon => Resource.Drawable.ic_close_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const string ExtraIdPedido = "QDC.HistorialCalificarActivity.ExtraIdPedido";

        #endregion

        #region VIEWS

        private readonly Dictionary<int, ImageView> _starsPedido = new Dictionary<int, ImageView>();
        private readonly Dictionary<int, ImageView> _starsReparto = new Dictionary<int, ImageView>();
        private readonly Dictionary<int, ImageView> _starsApp = new Dictionary<int, ImageView>();
        private FloatingActionButton _fab;

        #endregion

        #region FIELDS

        private int _calificacionPedido;
        private int _calificacionReparto;
        private int _calificacionApp;
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
            var idPedido = Intent.GetIntExtra(ExtraIdPedido, -1);
            if (idPedido == -1) throw new ArgumentNullException(nameof(ExtraIdPedido));
            _idPedido = idPedido;
        }

        private void GrabViews()
        {
            FindViewById<TextView>(Resource.Id.item_folio).Text = $"Califica al pedido #{_idPedido:D10}";

            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            _fab.Click += Fab_Click;

            _starsPedido.Add(1, FindViewById<ImageView>(Resource.Id.platillo_star1));
            _starsPedido.Add(2, FindViewById<ImageView>(Resource.Id.platillo_star2));
            _starsPedido.Add(3, FindViewById<ImageView>(Resource.Id.platillo_star3));
            _starsPedido.Add(4, FindViewById<ImageView>(Resource.Id.platillo_star4));
            _starsPedido.Add(5, FindViewById<ImageView>(Resource.Id.platillo_star5));

            _starsReparto.Add(1, FindViewById<ImageView>(Resource.Id.servicio_star1));
            _starsReparto.Add(2, FindViewById<ImageView>(Resource.Id.servicio_star2));
            _starsReparto.Add(3, FindViewById<ImageView>(Resource.Id.servicio_star3));
            _starsReparto.Add(4, FindViewById<ImageView>(Resource.Id.servicio_star4));
            _starsReparto.Add(5, FindViewById<ImageView>(Resource.Id.servicio_star5));

            _starsApp.Add(1, FindViewById<ImageView>(Resource.Id.app_star1));
            _starsApp.Add(2, FindViewById<ImageView>(Resource.Id.app_star2));
            _starsApp.Add(3, FindViewById<ImageView>(Resource.Id.app_star3));
            _starsApp.Add(4, FindViewById<ImageView>(Resource.Id.app_star4));
            _starsApp.Add(5, FindViewById<ImageView>(Resource.Id.app_star5));

            foreach (var image in _starsPedido.Values)
            {
                image.Click += (s, e) =>
                {
                    var selectedStar = s as ImageView;
                    UpdatePedidoUi(selectedStar);
                };
            }
            foreach (var image in _starsReparto.Values)
            {
                image.Click += (s, e) =>
                {
                    var selectedStar = s as ImageView;
                    UpdateRepartoUi(selectedStar);
                };
            }
            foreach (var image in _starsApp.Values)
            {
                image.Click += (s, e) =>
                {
                    var selectedStar = s as ImageView;
                    UpdateAppUi(selectedStar);
                };
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            HistorialPedidosViewModel.Instance.OnCalificarPedidosFinished += Instance_OnCalificarPedidosFinished;
        }

        protected override void OnPause()
        {
            base.OnPause();
            HistorialPedidosViewModel.Instance.OnCalificarPedidosFinished -= Instance_OnCalificarPedidosFinished;
        }

        #endregion

        private void UpdateAppUi(ImageView selectedStar)
        {
            var calificacion = _starsApp.First(c => c.Value == selectedStar);
            _calificacionApp = calificacion.Key;
            foreach (var imageKeyValue in _starsApp)
            {
                imageKeyValue.Value.SetImageResource(imageKeyValue.Key > _calificacionApp
                    ? Resource.Drawable.ic_star_outline
                    : Resource.Drawable.ic_star);
            }

            UpdateFabVisibility();
        }

        private void UpdateRepartoUi(ImageView selectedStar)
        {
            var calificacion = _starsReparto.First(c => c.Value == selectedStar);
            _calificacionReparto = calificacion.Key;
            foreach (var imageKeyValue in _starsReparto)
            {
                imageKeyValue.Value.SetImageResource(imageKeyValue.Key > _calificacionReparto
                    ? Resource.Drawable.ic_star_outline
                    : Resource.Drawable.ic_star);
            }
            UpdateFabVisibility();
        }

        private void UpdatePedidoUi(ImageView selectedStar)
        {
            var calificacion = _starsPedido.First(c => c.Value == selectedStar);
            _calificacionPedido = calificacion.Key;
            foreach (var imageKeyValue in _starsPedido)
            {
                imageKeyValue.Value.SetImageResource(imageKeyValue.Key > _calificacionPedido
                    ? Resource.Drawable.ic_star_outline
                    : Resource.Drawable.ic_star);
            }
            UpdateFabVisibility();
        }



        private void UpdateFabVisibility()
        {
            _fab.Visibility = _calificacionPedido > 0 && _calificacionApp > 0 && _calificacionReparto > 0
                ? ViewStates.Visible
                : ViewStates.Gone;
        }

        private async void Fab_Click(object sender, System.EventArgs e)
        {
            await HistorialPedidosViewModel.Instance.CalificarPedido(_idPedido, _calificacionPedido, _calificacionReparto, _calificacionApp);
        }
        private void Instance_OnCalificarPedidosFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            Finish();
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