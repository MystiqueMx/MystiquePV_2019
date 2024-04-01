using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using MystiqueNative.Droid.Helpers;
using Android.Support.V4.Widget;
using MystiqueNative.ViewModels;
using Android.Support.Design.Widget;
using MystiqueNative.Helpers;
using FFImageLoading;
using FFImageLoading.Views;
using MystiqueNative.Droid.Animations;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ParentActivity = typeof(LandingHasbroActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".LandingHasbroActivity")]
    public class ComentariosActivity : BaseActivity
    {
        private ProgressBar _progress;
        private Spinner _spinner;
        private Button _btnSend;
        private Button _btnEncuesta;
        private EditText _entryComentario;
        private TextView _charCount;
        private string _tipoComentario = TipoComentariosHelper.Descripcion[0];
        protected override int LayoutResource => Resource.Layout.activity_comentarios;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GrabViews();
            SetupSpinner();
           // SetupMenu();

            FindViewById<Button>(Resource.Id.ic_wallet).Click += (s, e) =>
            {
                StartRevealActivity(s as View);
            };
        }

        private void EntryComentario_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            _charCount.Text = $"{e.Editable.Length()}/255";
        }

        private void GrabViews()
        {
            _progress = FindViewById<ProgressBar>(Resource.Id.progressbar);
            _btnSend = FindViewById<Button>(Resource.Id.btn_send_comment);
            _btnEncuesta = FindViewById<Button>(Resource.Id.btn_realizar_encuesta);
            _entryComentario = FindViewById<EditText>(Resource.Id.entry_comentario);
            _spinner = FindViewById<Spinner>(Resource.Id.spinner_tipo_comentario);
            _charCount = FindViewById<TextView>(Resource.Id.comentario_lenght_count);
        }

        private void SetupSpinner()
        {
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, TipoComentariosHelper.Descripcion);
            adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            _spinner.Adapter = adapter;
            _spinner.ItemSelected += Spinner_ItemSelected;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _tipoComentario = TipoComentariosHelper.Descripcion[e.Position];
        }

        private void ViewModelComentarios_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBusy" when MainApplication.ViewModelComentarios.IsBusy:
                    StartAnimatingLogin();
                    break;
                case "IsBusy":
                    StopAnimatingLogin();
                    break;
                case "ErrorStatus":
                    if (!string.IsNullOrEmpty(MainApplication.ViewModelComentarios.ErrorMessage))
                    {
                        SendMessage(MainApplication.ViewModelComentarios.ErrorMessage);
                        MainApplication.ViewModelComentarios.ErrorMessage = string.Empty;
                    }
                    else
                    {
                        SendToast(MainApplication.ViewModelComentarios.ErrorMessage);
                        MainApplication.ViewModelComentarios.ErrorMessage = string.Empty;
                    }
                    if (!MainApplication.ViewModelComentarios.ErrorStatus)
                    {
                        _entryComentario.Text = string.Empty;
                    }

                    break;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            
            if (SupportActionBar != null)
            {
                SupportActionBar.Title = GetString(Resource.String.comentario_title);
            }
            _btnSend.Click += BtnSend_Click;
            _btnEncuesta.Click += _btnEncuesta_Click;
            MainApplication.ViewModelComentarios.PropertyChanged += ViewModelComentarios_PropertyChanged;
            _entryComentario.AfterTextChanged += EntryComentario_AfterTextChanged;
        }

        private void _btnEncuesta_Click(object sender, EventArgs e)
        {
           
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_entryComentario.Text))
            {
                SendMessage(GetString(Resource.String.error_campos_vacios));
            }
            else
            {
                if (IsInternetAvailable)
                    MainApplication.ViewModelComentarios.EnviarComentario(_tipoComentario, _entryComentario.Text);
                else
                {
                    SendConfirmation(GetString(Resource.String.error_no_conexion), "Sin conexión", accept =>
                    {
                        if (accept)
                        {
                            StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionWirelessSettings));
                        }
                    });
                }
            }
                
        }

        protected override void OnPause()
        {
            base.OnPause();
            _btnSend.Click -= BtnSend_Click;
            _btnEncuesta.Click -= _btnEncuesta_Click;
            MainApplication.ViewModelComentarios.PropertyChanged -= ViewModelComentarios_PropertyChanged;
            _entryComentario.AfterTextChanged -= EntryComentario_AfterTextChanged;
            //btnHomeAboutUs.Click -= BtnHomeAboutUs_Click;
            //ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
        #region DRAWER

        private Android.Support.V4.Widget.DrawerLayout _drawerLayout;
        private Android.Support.Design.Widget.NavigationView _navigationView;
        private const int NavigationItemId = Resource.Id.nav_item_comentarios;

        private void SetupMenu()
        {
            _drawerLayout = FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
           // _navigationView = FindViewById<Android.Support.Design.Widget.NavigationView>(Resource.Id.navigation_view);
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            _drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_24dp);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            //NavigationView.Menu.GetItem(ACTIVITY_INDEX_ON_MENU).SetChecked(true);
            //_navigationView.Menu.FindItem(NavigationItemId).SetChecked(true);
            //SetupDrawer(_navigationView);
        }
        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    switch (item.ItemId)
        //    {
        //        case Android.Resource.Id.Home:
        //            //_drawerLayout.OpenDrawer(_navigationView, true);
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
        private void StartAnimatingLogin()
        {
            _btnSend.Enabled = false;
            _btnSend.Alpha = 0.5F;

            _entryComentario.Enabled = false;
            _spinner.Enabled = false;

            _progress.Visibility = ViewStates.Visible;
        }
        private void StopAnimatingLogin()
        {
            _btnSend.Enabled = true;
            _btnSend.Alpha = 1F;

            _entryComentario.Enabled = true;
            _spinner.Enabled = true;

            _progress.Visibility = ViewStates.Gone;
        }
    }
}