using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Fragments;
using Android.Support.V4.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.ViewModels;
using FFImageLoading;
using FFImageLoading.Views;
using Android.Support.Design.Widget;
using MystiqueNative.Models;
using MystiqueNative.Droid.Animations;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ParentActivity = typeof(LandingHasbroActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".LandingHasbroActivity")]
    public class EstadoCuentaActivity : BaseActivity
    {
        private TextView _labelActuales;
        private TextView _labelSumados;
        private TextView _labelCanjeados;
        private CardView _cardSumados;
        private CardView _cardCanjeados;
        private ProgressBar _progress;
        private RevealAnimation _mRevealAnimation;
        protected override int LayoutResource => Resource.Layout.activity_estado_cuenta;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GrabViews();
            // SetupMenu();
            FindViewById<Button>(Resource.Id.ic_wallet).Click += (s, e) =>
            {
                StartRevealActivity(s as View);
            };
            _cardCanjeados.Click += CardCanjeados_Click;
            _cardSumados.Click += CardSumados_Click;
        }

        private void CardSumados_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(HistorialSumadosActivity));
        }

        private void CardCanjeados_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(HistorialCanjeadosActivity));
        }

        private void GrabViews()
        {
            _labelActuales = FindViewById<TextView>(Resource.Id.estado_cuenta_actuales_value);
            _labelCanjeados = FindViewById<TextView>(Resource.Id.estado_cuenta_canjeados_value);
            _labelSumados = FindViewById<TextView>(Resource.Id.estado_cuenta_sumados_value);
            _cardCanjeados = FindViewById<CardView>(Resource.Id.estado_cuenta_canjeados);
            _cardSumados= FindViewById<CardView>(Resource.Id.estado_cuenta_sumados);
        }

        protected override void OnResume()
        {
            base.OnResume();

            HistorialViewModel.Instance.FinishLoadingHistorial += Instance_FinishLoadingHistorial;
            CitypointsViewModel.Instance.OnEstadoCuentaFinished += Instance_OnEstadoCuentaFinished;
            HistorialViewModel.Instance.ObtenerHistorial();
            CitypointsViewModel.Instance.ObtenerEstadoCuenta();
            if (SupportActionBar != null)
            {
                SupportActionBar.Title = "Mi Saldo";
            }
        }

        private void Instance_OnEstadoCuentaFinished(object sender, EstadoCuentaArgs e)
        {
            if(e.EstadoCuenta != null && e.EstadoCuenta.Success)
                _labelActuales.Text = _labelActuales.Text =
                    $"{CitypointsViewModel.Instance.EstadoCuenta.PuntosAsInt} pts";
        }

        private void Instance_FinishLoadingHistorial(object sender, HistorialViewArgs e)
        {
            if (e.Success)
            {
                
                _labelCanjeados.Text = $"{e.Canjeados} pts";
                _labelSumados.Text = $"{e.Sumados} pts";
            }
            else
            {
                SendMessage(e.ErrorMessage);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            CitypointsViewModel.Instance.OnEstadoCuentaFinished -= Instance_OnEstadoCuentaFinished;
            HistorialViewModel.Instance.FinishLoadingHistorial -= Instance_FinishLoadingHistorial;
        }
        private void StartRevealActivity(View v)
        {
            int revealX = (int)(v.GetX() + v.Width / 2);
            int revealY = (int)(v.GetY() + v.Height / 2);

            Intent intent = new Intent(this, typeof(LandingHasbroActivity));
            intent.PutExtra(RevealAnimation.ExtraCircularRevealX, revealX);
            intent.PutExtra(RevealAnimation.ExtraCircularRevealY, revealY);
            StartActivity(intent, null);

            OverridePendingTransition(0, 0);
        }
        #region DRAWER

        private Android.Support.V4.Widget.DrawerLayout _drawerLayout;
        private Android.Support.Design.Widget.NavigationView _navigationView;
        private const int NavigationItemId = Resource.Id.nav_item_historial;

        private void SetupMenu()
        {
            _drawerLayout = FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<Android.Support.Design.Widget.NavigationView>(Resource.Id.navigation_view);
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            _drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_24dp);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            //NavigationView.Menu.GetItem(ACTIVITY_INDEX_ON_MENU).SetChecked(true);
            _navigationView.Menu.FindItem(NavigationItemId).SetChecked(true);
            SetupDrawer(_navigationView);
        }
        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    switch (item.ItemId)
        //    {
        //        case Android.Resource.Id.Home:
        //            _drawerLayout.OpenDrawer(_navigationView, true);
        //            return true;
        //        default:
        //            return base.OnOptionsItemSelected(item);
        //    }
        //}
        private void SetupDrawer(Android.Support.Design.Widget.NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_item_home:
                        if (NavigationItemId != Resource.Id.nav_item_home)
                            StartActivity(typeof(LandingHasbroActivity));
                        break;
                    case Resource.Id.nav_item_membership:
                        if (NavigationItemId != Resource.Id.nav_item_membership)
                            StartActivity(typeof(MembresiaActivity));
                        break;
                    case Resource.Id.nav_item_notificaciones:
                        if (NavigationItemId != Resource.Id.nav_item_notificaciones)
                            StartActivity(typeof(NotificacionesActivity));
                        break;
                    case Resource.Id.nav_item_historial:
                        if (NavigationItemId != Resource.Id.nav_item_historial)
                            StartActivity(typeof(EstadoCuentaActivity));
                        break;
                    case Resource.Id.nav_item_soporte:
                        if (NavigationItemId != Resource.Id.nav_item_soporte)
                            StartActivity(typeof(SoporteActivity));
                        break;
                    case Resource.Id.nav_item_mi_pedido:
                        if (NavigationItemId != Resource.Id.nav_item_mi_pedido)
                            StartActivity(typeof(LandingHasbroActivity));
                        break;
                    case Resource.Id.nav_item_comentarios:
                        if (NavigationItemId != Resource.Id.nav_item_comentarios)
                            StartActivity(typeof(ComentariosActivity));
                        break;
                }
                _drawerLayout.CloseDrawers();
            };

        }
        #endregion
    }
}