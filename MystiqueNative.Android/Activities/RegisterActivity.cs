using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Fragments;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Facebook;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegisterActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_register;
        protected override bool AllowNoConfiguration => true;
        protected override bool AllowNotLogged => true;
        #region VIEWS

        private Spinner _spinnerSexo;
        private Spinner _spinnerCiudad;
        private AutoCompleteTextView _acColonia;
        private Button _btnInitRegister;
        private Button _btnBack;
        private TextInputEditText _entryName;
        private TextInputEditText _entryLastname;
        private EditText _entryLastname2;
        private TextInputEditText _entryMail;
        private TextInputEditText _entryBirthday;
        private TextInputEditText _entryPhone;
        private TextInputEditText _entryPassword;
        private TextInputEditText _entryPassword2;
        private TextInputEditText _entryColonia;
        private ProgressBar _progressBar;
        private FrameLayout _progressBarHolder;
        #endregion
        #region FIELDS

        private string _sexo = string.Empty;
        private string _colonia = string.Empty;
        private string _mensajeError = string.Empty;
        #endregion
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            GrabViews();
            SetUpGenderSpinner();
            SetUpCitiesSpinner();

        }
        
        protected override void OnResume()
        {
            base.OnResume();
            
            ViewModels.AuthViewModelV2.Instance.PropertyChanged += Auth_PropertyChanged;
            ViewModels.AuthViewModelV2.Instance.OnRegistrarFailed += Instance_OnRegistrarFailed;
            ViewModels.AuthViewModelV2.Instance.OnIniciarSesionFinished += Instance_OnIniciarSesionFinished;
            ViewModels.AuthViewModelV2.Instance.OnConsultarColoniasFinished += Instance_OnConsultarColoniasFinished;
            _btnBack.Click += BtnBack_Click;
            _btnInitRegister.Click += BtnInitRegister_Click;
            _entryBirthday.Click += EntryBirthday_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModels.AuthViewModelV2.Instance.PropertyChanged -= Auth_PropertyChanged;
            ViewModels.AuthViewModelV2.Instance.OnRegistrarFailed -= Instance_OnRegistrarFailed;
            ViewModels.AuthViewModelV2.Instance.OnIniciarSesionFinished -= Instance_OnIniciarSesionFinished;
            ViewModels.AuthViewModelV2.Instance.OnConsultarColoniasFinished -= Instance_OnConsultarColoniasFinished;
            _btnBack.Click -= BtnBack_Click;
            _btnInitRegister.Click -= BtnInitRegister_Click;
            _entryBirthday.Click -= EntryBirthday_Click;
        }
        private void Instance_OnConsultarColoniasFinished(object sender, ViewModels.ConsultarColoniasEventArgs e)
        {
            if (e.Success)
            {
                SetColoniasAutocomplete(e.Colonias);
            }
        }

        private async void Instance_OnIniciarSesionFinished(object sender, ViewModels.LoginEventArgs e)
        {
            if (e.Success)
            {
                StartEnterAppIntent();
                PreferencesHelper.SetSavedLoginMethod(e.Method);
                await SecureStorageHelper.SetCredentialsAsync(e.Username, e.Password);
            }
            else
            {
                SendMessage(e.Message);
            }
            StopAnimatingLogin();
        }
        private void StartEnterAppIntent()
        {
            var intent = new Intent(this, typeof(LandingHasbroActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }
        private void Instance_OnRegistrarFailed(object sender, ViewModels.RegistrarEventArgs e)
        {
            SendMessage(e.Message);
        }

        private void Auth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBusy" when ViewModels.AuthViewModelV2.Instance.IsBusy:
                    StartAnimatingLogin();
                    break;
                case "IsBusy":
                    StopAnimatingLogin();
                    break;
            }
        }

        private void SetUpGenderSpinner()
        {
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SexosHelper.Genders);
            adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            _spinnerSexo.Adapter = adapter;
            _spinnerSexo.ItemSelected += SpinnerSexo_ItemSelected;
        }

        private void SetUpCitiesSpinner()
        {
            //acCiudad.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SelectDialogItem, CiudadesHelper.Ciudades);
            //acCiudad.Threshold = 1;
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, CiudadesHelper.Ciudades);
            adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            _spinnerCiudad.Adapter = adapter;
            _spinnerCiudad.ItemSelected += SpinnerCiudad_ItemSelected;
        }

        private void SpinnerCiudad_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position < 0) return;
            _acColonia.Text = string.Empty;
            ViewModels.AuthViewModelV2.Instance.ConsultarColonias(CiudadesHelper.Ciudades[e.Position]);
        }

        private void SetColoniasAutocomplete(List<string> colonias)
        {
            _acColonia.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SelectDialogItem, colonias);
            _acColonia.Threshold = 1;
            _acColonia.FocusChange += AcColonia_FocusChange;
            _acColonia.Enabled = colonias.Count > 0;
        }

        private void AcColonia_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus) return;
            var s = sender as AutoCompleteTextView;
            if (ViewModels.AuthViewModelV2.Instance.Colonias.Contains(s?.Text))
            {
                _colonia = s?.Text;
            }
            else
            {
                if (s != null)
                    s.Error = GetString(Resource.String.error_colonia_no_existe);
            }
        }
        private void SpinnerSexo_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _sexo = SexosHelper.Genders[e.Position];
            if (e.Position == 0)
                _sexo = string.Empty;
        }

        private void EntryBirthday_Click(object sender, EventArgs e)
        {
            var bdDialog = new BirthdayPickerDialogFragment(DateTime.Now.Date);
            bdDialog.DialogClosed += BdDialog_DialogClosed;
            bdDialog.Show(SupportFragmentManager, "BirthdayPickerDialogFragment");
        }

        private void BdDialog_DialogClosed(object sender, BirthdayDialogEventArgs args)
        {
            if (args == null) return;
            var day = args.Day > 9 ? args.Day.ToString() : "0" + args.Day;
            var mth = args.Month > 9 ? args.Month.ToString() : "0" + args.Month;
            _entryBirthday.Text = day + "/" + mth + "/" + args.Year;
        }
        private void BtnInitRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;
            if (IsInternetAvailable)
            {
                ViewModels.AuthViewModelV2.Instance.Registrar(new Models.Configuration.RegisterUser
                {
                    Nombre = _entryName.Text,
                    Paterno = _entryLastname.Text,
                    Materno = _entryLastname2.Text,
                    FechaNacimiento = _entryBirthday.Text,
                    Sexo = _sexo,
                    Telefono = _entryPhone.Text, 
                    Colonia = _colonia, 
                    Email = _entryMail.Text,
                    Password= _entryPassword.Text
                });
            }
            else
            {
                SendConfirmation(GetString(Resource.String.error_no_conexion), "Sin conexión", accept =>
                {
                    if (accept)
                    {
                        StartActivity(new Intent(Android.Provider.Settings.ActionWirelessSettings));
                    }
                });
            }

        }
        private bool ValidateFields()
        {
            var canContinue = true;
            View focusView = null;
            if (string.IsNullOrEmpty(_entryName.Text)
                    || string.IsNullOrEmpty(_entryLastname.Text)
                    || string.IsNullOrEmpty(_entryMail.Text)

                    || string.IsNullOrEmpty(_entryPassword.Text)
                    || string.IsNullOrEmpty(_entryPassword2.Text))
            {
                SendMessage(GetString(Resource.String.error_campos_vacios));
                return false;
            }
            else
            {
                if (!ValidatorHelper.IsValidName(_entryName.Text))
                {
                    _entryName.Error = GetString(Resource.String.error_validacion_nombre);
                    focusView = _entryName;
                    canContinue = false;
                }
                if (!ValidatorHelper.IsValidName(_entryLastname.Text))
                {
                    _entryLastname.Error = GetString(Resource.String.error_validacion_nombre);
                    if (focusView == null)
                        focusView = _entryLastname;
                    canContinue = false;
                }
                if (!string.IsNullOrEmpty(_entryLastname2.Text) && !ValidatorHelper.IsValidName(_entryLastname2.Text))
                {
                    _entryLastname2.Error = GetString(Resource.String.error_validacion_nombre);
                    if (focusView == null)
                        focusView = _entryLastname2;
                    canContinue = false;
                }
                if (!string.IsNullOrEmpty(_entryPhone.Text) && !ValidatorHelper.IsValidPhone(_entryPhone.Text))
                {
                    _entryPhone.Error = GetString(Resource.String.error_validacion_telefono);
                    if (focusView == null)
                        focusView = _entryPhone;
                    canContinue = false;
                }
                if (!ValidatorHelper.IsValidEmail(_entryMail.Text))
                {
                    _entryMail.Error = GetString(Resource.String.error_validacion_email);
                    if (focusView == null)
                        focusView = _entryMail;
                    canContinue = false;
                }
                if (!ValidatorHelper.IsValidPassword(_entryPassword.Text))
                {
                    _entryPassword.Error = GetString(Resource.String.error_validacion_password);
                    if (focusView == null)
                        focusView = _entryPassword;
                    canContinue = false;
                }
                if (!_entryPassword2.Text.Equals(_entryPassword.Text))
                {
                    _entryPassword.Error = GetString(Resource.String.error_validacion_password_confirmacion);
                    if (focusView == null)
                        focusView = _entryPassword;
                    canContinue = false;
                }
                if (!string.IsNullOrEmpty(_acColonia.Text) && !ViewModels.AuthViewModelV2.Instance.Colonias.Contains(_colonia))
                {
                    _acColonia.Error = GetString(Resource.String.error_colonia_no_existe);
                    canContinue = false;
                    if (focusView == null)
                        focusView = _acColonia;
                }

                focusView?.RequestFocus();
                return canContinue;
            }
           
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            Finish();
        }
        private void GrabViews()
        {
            _btnInitRegister = FindViewById<Button>(Resource.Id.btn_init_register);
            _btnBack = FindViewById<Button>(Resource.Id.btn_back);

            _spinnerSexo = FindViewById<Spinner>(Resource.Id.spinner_sexo);

            _entryColonia = FindViewById<TextInputEditText>(Resource.Id.register_entry_colonia);
            _entryName = FindViewById<TextInputEditText>(Resource.Id.login_entry_nombre);
            _entryLastname = FindViewById<TextInputEditText>(Resource.Id.registro_entry_paterno);
            _entryLastname2 = FindViewById<EditText>(Resource.Id.entry_last_name2);
            _entryMail = FindViewById<TextInputEditText>(Resource.Id.register_entry_mail);
            _entryBirthday = FindViewById<TextInputEditText>(Resource.Id.register_entry_nacimiento);
            _entryPhone = FindViewById<TextInputEditText>(Resource.Id.register_entry_phone);
            _entryPassword = FindViewById<TextInputEditText>(Resource.Id.register_entry_password);
            _entryPassword2 = FindViewById<TextInputEditText>(Resource.Id.register_entry_password_2);

            _spinnerCiudad = FindViewById<Spinner>(Resource.Id.spinner_ciudad);
            _acColonia = FindViewById<AutoCompleteTextView>(Resource.Id.ac_colonia);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
        }
        private void StartAnimatingLogin()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimatingLogin()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }
    }
    internal class FacebookUserRequest : Java.Lang.Object, GraphRequest.ICallback
    {
        private const string MeEndpoint = "/me";
        public Models.Facebook.FacebookProfile Profile { get; private set; }

        public void MakeUserRequest()
        {
            var parameters = new Bundle();
            parameters.PutString("fields", "id, email");

            var request = new GraphRequest(
                    AccessToken.CurrentAccessToken,
                    MeEndpoint,
                    parameters,
                    HttpMethod.Get,
                    this
            );
            Task.Run(()=> { request.ExecuteAndWait(); }).Wait();
            
        }
        public void OnCompleted(GraphResponse response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.RawResponse))
                {
                    return;
                }
                this.Profile = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Facebook.FacebookProfile>(response.RawResponse);

            }
            catch (Exception e)
            {
            #if DEBUG
                Console.WriteLine(e.Message);
            #endif
            }
        }
    }
}