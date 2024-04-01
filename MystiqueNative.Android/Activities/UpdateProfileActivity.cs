using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Label = "Completa tu registro")]
    public class UpdateProfileActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_update_profile;
        
        #region RESULT CODE
        public const int UpdateRequestCode = 00993;
        #endregion
        #region VIEWS
        private Button _btnSave;
        private Spinner _spinnerSexo;
        private EditText _entryColonia;
        private EditText _entryBirthday;
        private EditText _entryPhone;
        private Spinner _spinnerCiudad;
        private AutoCompleteTextView _acColonia;
        private ProgressBar _progressBar;
        private FrameLayout _progressBarHolder;
        #endregion
        #region FIELDS

        private string _sexo = "0";
        private string _colonia = "";
        private DateTime _fechaNacimiento;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            SetUpGenderSpinner();
            SetUpCitiesSpinner();
            if (AuthViewModelV2.Instance.Usuario != null)
                FillPresetFields();
            // Create your application here
        }
        protected override void OnResume()
        {
            base.OnResume();
            ViewModels.AuthViewModelV2.Instance.OnConsultarColoniasFinished += Instance_OnConsultarColoniasFinished;
            ViewModels.AuthViewModelV2.Instance.OnActualizarPerfilFinished += Instance_OnActualizarPerfilFinished;
            _entryBirthday.Click += EntryBirthday_Click;
            _btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateFields())
            {
                if (IsInternetAvailable)
                {
                    StartAnimatingLogin();
                    var user = AuthViewModelV2.Instance.Usuario;
                    AuthViewModelV2.Instance.ActualizarPerfil(user.Nombre,
                        user.Paterno,
                        user.Materno,
                        _entryBirthday.Text,
                        _sexo,
                        _entryPhone.Text,
                        _colonia,
                        string.Empty);
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
        private bool ValidateFields()
        {
            var canContinue = true;
            View focusView = null;
            if (string.IsNullOrEmpty(_entryPhone.Text)
                || string.IsNullOrEmpty(_colonia)
                || string.IsNullOrEmpty(_sexo)
                || string.IsNullOrEmpty(_entryBirthday.Text))
            {
                SendMessage(GetString(Resource.String.error_campos_vacios));
                return false;
            }
            else
            { 
                if (!string.IsNullOrEmpty(_entryPhone.Text) && !ValidatorHelper.IsValidPhone(_entryPhone.Text))
                {
                    _entryPhone.Error = GetString(Resource.String.error_validacion_telefono);
                    canContinue = false;
                }
                else
                {
                    _entryPhone.Error = null;
                }
                if (!string.IsNullOrEmpty(_acColonia.Text) && !ViewModels.AuthViewModelV2.Instance.Colonias.Contains(_colonia))
                {
                    _acColonia.Error = GetString(Resource.String.error_colonia_no_existe);
                    canContinue = false;
                    if (focusView == null)
                        focusView = _acColonia;
                }
                else
                {
                    _acColonia.Error = null;
                }

                focusView?.RequestFocus();
                return canContinue;
            }
        }

        private void EntryBirthday_Click(object sender, EventArgs e)
        {
            var bdDialog = new Fragments.BirthdayPickerDialogFragment(DateTime.Now);
            bdDialog.DialogClosed += BdDialog_DialogClosed;
            bdDialog.Show(SupportFragmentManager, "BirthdayPickerDialogFragment");
        }
        private void BdDialog_DialogClosed(object sender, Fragments.BirthdayDialogEventArgs args)
        {
            if (args == null) return;
            var day = args.Day > 9 ? args.Day.ToString() : "0" + args.Day;
            var mth = args.Month > 9 ? args.Month.ToString() : "0" + args.Month;
            _entryBirthday.Text = day + "/" + mth + "/" + args.Year;
        }
        protected override void OnPause()
        {
            base.OnPause();
            ViewModels.AuthViewModelV2.Instance.OnConsultarColoniasFinished -= Instance_OnConsultarColoniasFinished;
            ViewModels.AuthViewModelV2.Instance.OnActualizarPerfilFinished -= Instance_OnActualizarPerfilFinished;
        }

        private void Instance_OnActualizarPerfilFinished(object sender, ActualizarPerfilEventArgs e)
        {
            SetResult(e.Success ? Result.Ok : Result.Canceled);
            StopAnimatingLogin();
            Finish();
        }
        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            SendConfirmation(GetString(Resource.String.error_abandonar_registro), "", "Continuar", "Cancelar", accept =>
            {
                if (accept)
                {
                    base.OnBackPressed();
                }
            });
            
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
            }
            return true;
        }
        private void Instance_OnConsultarColoniasFinished(object sender, ConsultarColoniasEventArgs e)
        {
            if (e.Success)
            {
                SetColoniasAutocomplete(e.Colonias);
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
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, CiudadesHelper.Ciudades);
            adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            _spinnerCiudad.Adapter = adapter;
            _spinnerCiudad.ItemSelected += SpinnerCiudad_ItemSelected;
        }
        private void SpinnerSexo_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _sexo = SexosHelper.Genders[e.Position];
            if (e.Position == 0)
                _sexo = string.Empty;
        }
        private static void SpinnerCiudad_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position >= 0)
            {
                ViewModels.AuthViewModelV2.Instance.ConsultarColonias(CiudadesHelper.Ciudades[e.Position]);
            }
        }
        private void SetColoniasAutocomplete(IList<string> colonias)
        {
            _acColonia.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SelectDialogItem, colonias);
            _acColonia.Threshold = 1;
            _acColonia.FocusChange += AcColonia_FocusChange;
            _acColonia.Enabled = colonias.Count > 0;
        }
        private void AcColonia_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus) return;
            if (ViewModels.AuthViewModelV2.Instance.Colonias.Contains(_acColonia.Text))
            {
                _colonia = _acColonia.Text;
            }
            else
            {
                _acColonia.Error = "Por favor seleccione una de las colonias disponibles";
            }
        }
        private void GrabViews()
        {
            _btnSave = FindViewById<Button>(Resource.Id.btn_save_register);
            _spinnerSexo = FindViewById<Spinner>(Resource.Id.spinner_sexo);
            _entryColonia = FindViewById<EditText>(Resource.Id.entry_colonia);
            _entryBirthday = FindViewById<EditText>(Resource.Id.entry_birthday);
            _entryPhone = FindViewById<EditText>(Resource.Id.entry_phone);
            _spinnerCiudad = FindViewById<Spinner>(Resource.Id.spinner_ciudad);
            _acColonia = FindViewById<AutoCompleteTextView>(Resource.Id.ac_colonia);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
        }
        private void FillPresetFields()
        {
            _fechaNacimiento = AuthViewModelV2.Instance.Usuario.FechaNacimientoAsDateTime ?? DateTime.Now.Date;

            _sexo = AuthViewModelV2.Instance.Usuario.SexoAsString;
            _colonia = AuthViewModelV2.Instance.Usuario.Colonia;
            _entryBirthday.Text = AuthViewModelV2.Instance.Usuario.FechaNacimientoConFormatoEspanyol;
            _entryPhone.Text = AuthViewModelV2.Instance.Usuario.Telefono;

            if (_fechaNacimiento.Year < 1900)
                _fechaNacimiento = DateTime.Now.Date;

            if(AuthViewModelV2.Instance.Usuario.CiudadAsInt != 0)
            {
                AuthViewModelV2.Instance.ConsultarColonias(CiudadesHelper.IntToCiudades.FirstOrDefault(c=>c.Key == AuthViewModelV2.Instance.Usuario.CiudadAsInt).Value);
            }

            if (!string.IsNullOrEmpty(_sexo))
            {
                _spinnerSexo.SetSelection(SexosHelper.Genders.FindIndex(c => c.Equals(ViewModels.AuthViewModelV2.Instance.Usuario.SexoAsString)));
                _spinnerSexo.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrEmpty(_colonia))
            {
                _acColonia.Text = _colonia;
                _spinnerCiudad.SetSelection(AuthViewModelV2.Instance.Usuario.CiudadAsInt);
                _acColonia.Visibility = ViewStates.Gone;
                _spinnerCiudad.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrEmpty(_entryBirthday.Text))
            {
                
                _entryBirthday.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrEmpty(_entryPhone.Text))
            {
                
                _entryPhone.Visibility = ViewStates.Gone;
            }

        }
    }
}