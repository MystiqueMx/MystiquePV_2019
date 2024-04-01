
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using MystiqueNative.Droid.Helpers;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using MystiqueNative.Droid.Animations;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ParentActivity = typeof(LandingHasbroActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".LandingHasbroActivity")]
    public class SoporteActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_soporte;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GrabViews();
            // SetupMenu();

            FindViewById<Button>(Resource.Id.ic_wallet).Click += (s, e) =>
            {
                StartRevealActivity(s as View);
            };
        }


        private void GrabViews()
        {
            var versionTv = FindViewById<TextView>(Resource.Id.soporte_version);
            versionTv.Text = "Versión v" + ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
            versionTv.LongClick += (s, e) =>
            {
                var mensaje = string.Format("VC:{0} VN:{2} E:{1}", PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionCode, Configuration.MystiqueApiV2Config.MystiqueAppEmpresa, PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName);
                SendConfirmation(mensaje, "Información de Fresco App", "Crash", "Salir", okLabel =>
               {
                   if (okLabel)
                   {
                       Com.Crashlytics.Android.Crashlytics.Instance.Crash();
                   }
               });
            };
            FindViewById<TextView>(Resource.Id.soporte_parte1).Text = MystiqueApp.DescripcionSoporte;
            FindViewById<TextView>(Resource.Id.soporte_parte2).Text = MystiqueApp.TelefonoSoporte;
            FindViewById<TextView>(Resource.Id.soporte_parte3).Text = MystiqueApp.EmailSoporte;
            FindViewById<TextView>(Resource.Id.soporte_parte4).Text = MystiqueApp.TerminosSoporte;
            FindViewById<TextView>(Resource.Id.soporte_parte5).Text = MystiqueApp.DescripcionSoporte;
        }
        #region DRAWER

        private Android.Support.V4.Widget.DrawerLayout _drawerLayout;
        private Android.Support.Design.Widget.NavigationView _navigationView;
        private const int NavigationItemId = Resource.Id.nav_item_soporte;

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
        #endregion
    }
}