using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Content.Res;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using Firebase.Analytics;
using MystiqueNative.Droid.Fragments;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Analytics;
using System;
using System.Collections.Generic;
using JavaFile = Java.IO.File;
using JavaUri = Android.Net.Uri;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PerfilActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_profile;
        private const int GalleryRequest = 123;
        private const int CameraRequest = 124;

        #region VIEWS

        private Spinner _spinnerSexo;
        private Spinner _spinnerCiudad;
        private AutoCompleteTextView _acColonia;
        private TextView _labelNombre;
        private TextView _labelCiudad;
        private TextView _labelColonia;
        private ImageButton _btnEdit;
        private ImageButton _btnFoto;
        private ImageButton _btnGaleria;
        private Drawable _editDrawable;
        private Drawable _cancelDrawable;
        private Button _btnSave;
        private TextInputEditText _entryName;
        private EditText _entrySexo;
        private TextInputEditText _entryLastname;
        private EditText _entryLastname2;
        private TextInputEditText _entryBirthday;
        private TextInputEditText _entryPhone;
        private TextInputEditText _entryPassword;
        private TextInputEditText _entryPassword2;
        private EditText _entryColonia;
        private ProgressBar _progressBar;
        private ImageViewAsync _image;
        private FloatingActionButton _fab;
        private FrameLayout _progressBarHolder;
        private TextInputLayout _layoutNombre;
        private TextInputLayout _layoutApellido;
        #endregion
        #region FIELDS

        private FirebaseAnalytics _firebaseAnalytics;
        private Bitmap _profilePictureAsBitmap;
        private JavaFile _javaPictureFile;
        private DateTime _fechaNacimiento;
        private bool _isEditing;
        private string _profilePictureToLoad;
        private string _sexo;
        private string _colonia;
        private bool _pendingToUpdate;
        #endregion

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            GrabViews();
            SetUpGenderSpinner();
            SetUpCitiesSpinner();
            //SetupMenu();
            _editDrawable = AppCompatResources.GetDrawable(this, Resource.Drawable.account_edit);
            _cancelDrawable = AppCompatResources.GetDrawable(this, Resource.Drawable.cancel);


        }
        protected override void OnResume()
        {
            base.OnResume();
            if (!_pendingToUpdate)
            {
                FillPresetFields();
            }
            ViewModels.AuthViewModelV2.Instance.OnConsultarColoniasFinished += Instance_OnConsultarColoniasFinished;
            ViewModels.AuthViewModelV2.Instance.OnActualizarPerfilFinished += Instance_OnActualizarPerfilFinished;
            ViewModels.AuthViewModelV2.Instance.OnPictureUploadFailed += Instance_OnPictureUploadFailed;
            SupportActionBar.Title = "Mi Perfil";
            EnableEdit();
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModels.AuthViewModelV2.Instance.OnConsultarColoniasFinished -= Instance_OnConsultarColoniasFinished;
            ViewModels.AuthViewModelV2.Instance.OnActualizarPerfilFinished -= Instance_OnActualizarPerfilFinished;
            ViewModels.AuthViewModelV2.Instance.OnPictureUploadFailed -= Instance_OnPictureUploadFailed;
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

        private void Instance_OnPictureUploadFailed(object sender, ViewModels.PictureUploadEventArgs e)
        {
            SendToast("Ocurrió un error al actualizar la imagen de perfil, intente de nuevo mas tarde");
        }

        private async void Instance_OnActualizarPerfilFinished(object sender, ViewModels.ActualizarPerfilEventArgs e)
        {
            if (e.Success)
            {
                if (!string.IsNullOrEmpty(_entryPassword.Text))
                {
                    var username = await Utils.SecureStorageHelper.GetUsernameAsync();
                    await Utils.SecureStorageHelper.SetCredentialsAsync(username, _entryPassword.Text);
                }

                _pendingToUpdate = false;
                _btnEdit.SetImageDrawable(_editDrawable);
                FillPresetFields();
                _isEditing = false;
                //DisableEdit();
                SendToast(GetString(Resource.String.label_exito_actualizar));
                Finish();
            
            }
            else
            {
                if (!string.IsNullOrEmpty(e.Message))
                {
                    SendMessage(e.Message);
                }

            }
        }
        private static void SpinnerCiudad_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position >= 0)
            {
                //acColonia.Text = string.Empty;
                ViewModels.AuthViewModelV2.Instance.ConsultarColonias(CiudadesHelper.Ciudades[e.Position]);
            }
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
        private void SetColoniasAutocomplete(List<string> colonias)
        {
            _acColonia.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SelectDialogItem, colonias);
            _acColonia.Threshold = 1;
            _acColonia.FocusChange += AcColonia_FocusChange;
            _acColonia.Enabled = colonias.Count > 0;
        }

        private void EntryBirthday_Click(object sender, EventArgs e)
        {
            var bdDialog = new BirthdayPickerDialogFragment(_fechaNacimiento);
            bdDialog.DialogClosed += BdDialog_DialogClosed;
            bdDialog.Show(SupportFragmentManager, "BirthdayPickerDialogFragment");
        }
        private async void BtnSave_ClickAsync(object sender, EventArgs e)
        {
            if (ViewModels.AuthViewModelV2.Instance.Colonias.Contains(_acColonia.Text))
            {
                _colonia = _acColonia.Text;
            }
            else
            {
                _acColonia.Error = "Por favor seleccione una de las colonias disponibles";
            }

            if (!ValidateFields()) return;

            if (IsInternetAvailable)
            {
                if (_profilePictureAsBitmap == null)
                {
                    ViewModels.AuthViewModelV2.Instance.ActualizarPerfil(_entryName.Text,
                        _entryLastname.Text,
                        _entryLastname2.Text,
                        _entryBirthday.Text,
                        _sexo,
                        _entryPhone.Text,
                        _colonia,
                        _entryPassword.Text);
                }
                else
                {
                    ViewModels.AuthViewModelV2.Instance.ActualizarPerfil(_entryName.Text,
                        _entryLastname.Text,
                        _entryLastname2.Text,
                        _entryBirthday.Text,
                        _sexo,
                        _entryPhone.Text,
                        _colonia,
                        _entryPassword.Text,
                        await System.Threading.Tasks.Task.Run(() => _profilePictureAsBitmap.LoadBitmapToMemoryAsJpg()),
                        ".jpg");
                }
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
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_isEditing)
            {

                SendConfirmation(GetString(Resource.String.label_cancelar_edicion), "Confirmación", accept =>
                {
                    if (!accept) return;
                    _btnEdit.SetImageDrawable(_editDrawable);
                    FillPresetFields();
                    _isEditing = false;
                    DisableEdit();
                });
            }
            else
            {
                _btnEdit.SetImageDrawable(_cancelDrawable);
                _isEditing = true;
                EnableEdit();
            }
        }
        private void BtnGaleria_Click(object sender, EventArgs e)
        {
            if (PermissionsHelper.ValidatePermissionsForStorageRw())
            {
                InitGalleryIntent();
                _pendingToUpdate = true;
            }
            else
            {
                RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(), PermissionsHelper.RequestStorageId);
            }

        }

        private void BtnFoto_Click(object sender, EventArgs e)
        {
            if (PermissionsHelper.ValidatePermissionsForCamera())
            {
                InitCameraIntent();
                _pendingToUpdate = true;
            }
            else
            {
                RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(), PermissionsHelper.RequestCameraId);
            }
        }
        private void InitGalleryIntent()
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(//TODO CHANGE STRING
                Intent.CreateChooser(imageIntent, "Selecciona la foto de perfil"), GalleryRequest);
        }
        private void InitCameraIntent()
        {
            _javaPictureFile = JavaFile.CreateTempFile("profile_picture_", ".jpg", GetExternalFilesDir(Android.OS.Environment.DirectoryDcim));

            var i = new Intent(Android.Provider.MediaStore.ActionImageCapture);
            i.PutExtra(Android.Provider.MediaStore.ExtraOutput, ContentResolverHelper.AbsoluteToContent(_javaPictureFile));
            i.AddFlags(ActivityFlags.GrantReadUriPermission);
            i.AddFlags(ActivityFlags.GrantWriteUriPermission);

            var pictureApps = PackageManager.QueryIntentActivities(i, PackageInfoFlags.MatchDefaultOnly);
            if (pictureApps != null && pictureApps.Count > 0)
            {
                StartActivityForResult(i, CameraRequest);
            }
            else
            {
                //DO SOMETHING
            }
        }

        private void Instance_OnConsultarColoniasFinished(object sender, ViewModels.ConsultarColoniasEventArgs e)
        {
            if (e.Success)
            {
                SetColoniasAutocomplete(e.Colonias);
            }
        }

        private void ChangeImageViewAsync(string absolutePathToImage)
        {
            try
            {
                _profilePictureAsBitmap = BitmapHelper.DecodeAndResize(absolutePathToImage, 800)
                                                        .RotateImageBasedOnExif(absolutePathToImage);
                _image.SetImageBitmap(_profilePictureAsBitmap);
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                _profilePictureAsBitmap = null;
            }

        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent resultData)
        {
            base.OnActivityResult(requestCode, resultCode, resultData);

            try
            {
                switch (requestCode)
                {
                    case CameraRequest:
                        if (resultCode == Result.Ok)
                        {
                            ChangeImageViewAsync(_javaPictureFile.AbsolutePath);
                        }
                        else
                        {
                            _profilePictureToLoad = string.Empty;
                        }
                        break;

                    case GalleryRequest:
                        if (resultCode == Result.Ok && resultData.Data != null)
                        {
                            _profilePictureToLoad = ContentResolverHelper.ContentResolverToAbsolute(ContentResolver, resultData.Data);
                            ChangeImageViewAsync(_profilePictureToLoad);
                        }
                        else
                        {
                            _profilePictureToLoad = string.Empty;
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Ex: ", ex.Message);
#endif
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            var gotRequestedPermissions = true;
            switch (requestCode)
            {
                case PermissionsHelper.RequestCameraId:
                    gotRequestedPermissions = true;
                    foreach (var p in grantResults)
                        if (p != Permission.Granted)
                            gotRequestedPermissions = false;
                    if (gotRequestedPermissions)
                    {
                        InitCameraIntent();
                    }
                    else { }
                    //messageDialog.SendMessage(GetString(Resource.String.error_no_permisos_storage), "Error");
                    break;
                case PermissionsHelper.RequestStorageId:
                    gotRequestedPermissions = true;
                    foreach (var p in grantResults)
                        if (p != Permission.Granted)
                            gotRequestedPermissions = false;
                    if (gotRequestedPermissions)
                    {
                        InitGalleryIntent();
                    }
                    else { }
                    //messageDialog.SendMessage(GetString(Resource.String.error_no_permisos_storage), "Error");
                    break;
                default:
                    break;
            }
        }
        private void FillPresetFields()
        {
            _labelNombre.Text = ViewModels.AuthViewModelV2.Instance.Usuario.NombreCompleto;
            _entryName.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Nombre;
            //entrySexo.Text = MainApplication.Auth.Usuario.SexoAsString;
            _sexo = string.Empty;
            if (ViewModels.AuthViewModelV2.Instance.Usuario.SexoAsInt != 0)
            {
                _sexo = ViewModels.AuthViewModelV2.Instance.Usuario.SexoAsString;
            }
            _entrySexo.Text = _sexo;
            _fechaNacimiento = ViewModels.AuthViewModelV2.Instance.Usuario.FechaNacimientoAsDateTime ?? DateTime.Now.Date;
            _entryLastname.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Paterno;
            _entryLastname2.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Materno;
            _entryBirthday.Text = ViewModels.AuthViewModelV2.Instance.Usuario.FechaNacimientoConFormatoEspanyol;
            _entryPhone.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Telefono;
            _entryColonia.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Colonia;
            _colonia = ViewModels.AuthViewModelV2.Instance.Usuario.Colonia;
            _acColonia.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Colonia;
            _spinnerCiudad.SetSelection(ViewModels.AuthViewModelV2.Instance.Usuario.CiudadAsInt);
            if (!string.IsNullOrEmpty(ViewModels.AuthViewModelV2.Instance.ProfilePictureUrl))
            {
                ImageService.Instance.LoadUrl(ViewModels.AuthViewModelV2.Instance.ProfilePictureUrl)
                        .Retry(retryCount: 5, millisecondDelay: 500)
                        .DownSampleInDip(height: 100)
                        .Transform(new CircleTransformation())
                        .Into(_image);
            }
            else
            {
                _image.SetImageDrawable(AppCompatResources.GetDrawable(this, Resource.Drawable.account_circle));
            }

            if (_fechaNacimiento.Year < 1900)
                _fechaNacimiento = DateTime.Now.Date;

            if (ViewModels.AuthViewModelV2.Instance.Usuario.SexoAsInt != 0)
            {
                _spinnerSexo.SetSelection(SexosHelper.Genders.FindIndex(c => c.Equals(ViewModels.AuthViewModelV2.Instance.Usuario.SexoAsString)));
            }

        }

        private void Auth_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBusy" when ViewModels.AuthViewModelV2.Instance.IsBusy:
                    StartAnimatingLogin();
                    break;
                case "IsBusy":
                    StopAnimatingLogin();
                    break;
                case "ProfileEditStatus":
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

        private void SpinnerSexo_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _sexo = SexosHelper.Genders[e.Position];
            if (e.Position == 0)
                _sexo = string.Empty;
        }

        private void BdDialog_DialogClosed(object sender, BirthdayDialogEventArgs args)
        {
            if (args == null) return;
            var day = args.Day > 9 ? args.Day.ToString() : "0" + args.Day;
            var mth = args.Month > 9 ? args.Month.ToString() : "0" + args.Month;
            _entryBirthday.Text = day + "/" + mth + "/" + args.Year;
        }

        private bool ValidateFields()
        {
            var canContinue = true;
            View focusView = null;
            if (string.IsNullOrEmpty(_entryName.Text)

                    || string.IsNullOrEmpty(_entryBirthday.Text)
                    || string.IsNullOrEmpty(_acColonia.Text)
                    || string.IsNullOrEmpty(_entryPhone.Text)
                    || _spinnerSexo.SelectedItemPosition == 0

                    || string.IsNullOrEmpty(_entryLastname.Text))
            {
                SendMessage(GetString(Resource.String.error_campos_vacios));
                return false;
            }
            else
            {
                if (!ValidatorHelper.IsValidName(_entryName.Text))
                {
                    _entryName.Error = GetString(Resource.String.error_validacion_nombre);
                    if (focusView == null)
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
                if (!string.IsNullOrEmpty(_entryPassword.Text))
                {
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
                }
                if (!string.IsNullOrEmpty(_acColonia.Text) && !ViewModels.AuthViewModelV2.Instance.Colonias.Contains(_colonia) && _colonia != ViewModels.AuthViewModelV2.Instance.Usuario.Colonia)
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
        private void GrabViews()
        {
            _labelNombre = FindViewById<TextView>(Resource.Id.label_nombre);

            _btnSave = FindViewById<Button>(Resource.Id.btn_save_register);
            _btnEdit = FindViewById<ImageButton>(Resource.Id.ic_account_edit);
            _btnFoto = FindViewById<ImageButton>(Resource.Id.btn_take_picture);
            _btnGaleria = FindViewById<ImageButton>(Resource.Id.btn_choose_gallery);

            _image = FindViewById<ImageViewAsync>(Resource.Id.imagen_perfil);

            _spinnerSexo = FindViewById<Spinner>(Resource.Id.spinner_sexo);

            _entryColonia = FindViewById<EditText>(Resource.Id.entry_colonia);
            _entryName = FindViewById<TextInputEditText>(Resource.Id.profile_entry_name);
            _entryLastname = FindViewById<TextInputEditText>(Resource.Id.profile_entry_last_name_a);
            _entryLastname2 = FindViewById<TextInputEditText>(Resource.Id.profile_entry_last_name_a2);
            _entrySexo = FindViewById<EditText>(Resource.Id.entry_sexo);
            _entryBirthday = FindViewById<TextInputEditText>(Resource.Id.profile_entry_birthday);
            _entryPhone = FindViewById<TextInputEditText>(Resource.Id.profile_entry_phone);
            _entryPassword = FindViewById<TextInputEditText>(Resource.Id.profile_entry_password);
            _entryPassword2 = FindViewById<TextInputEditText>(Resource.Id.profile_entry_password_2);

            
            _layoutNombre = FindViewById<TextInputLayout>(Resource.Id.profile_layout_name);
            _layoutApellido = FindViewById<TextInputLayout>(Resource.Id.profile_layout_apellido_a);

            _spinnerCiudad = FindViewById<Spinner>(Resource.Id.spinner_ciudad);
            _acColonia = FindViewById<AutoCompleteTextView>(Resource.Id.ac_colonia);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar_loading);

            _btnEdit.Click += BtnEdit_Click;
            _btnFoto.Click += BtnFoto_Click;
            _btnGaleria.Click += BtnGaleria_Click;

            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            _fab.Click += (s, e) =>
             {
                 SendConfirmation("Usted esta a punto de cerrar su sesión", "Confirmar", "Cerrar Sesión", "Cancelar", ok =>
                    {
                        if (ok)
                        {
                            Logout();
                        }
                    });
             };
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);

        }
        private void EnableEdit()
        {
            //btnInitRegister.Enabled = false;
            //btnInitRegister.Alpha = 0.5F;
            //btnBack.Enabled = false;
            //btnBack.Alpha = 0.5F;
            //btnFacebookConnect.Enabled = false;
            //btnFacebookConnect.Alpha = 0.5F;
            //_layoutNombre.Visibility = ViewStates.Visible;
            //_layoutApellido.Visibility = ViewStates.Visible;

            //_entryName.Visibility = ViewStates.Visible;
            //_entryLastname.Visibility = ViewStates.Visible;
            ////entryLastname2.Visibility = ViewStates.Visible;
            //_entryBirthday.Enabled = true;
            //_entrySexo.Visibility = ViewStates.Gone;
            //_spinnerSexo.Visibility = ViewStates.Visible;
            _entryColonia.Enabled = true;
            //_spinnerCiudad.Visibility = ViewStates.Visible;
            //_acColonia.Visibility = ViewStates.Visible;
            _entryPhone.Enabled = true;
            if (MystiqueApp.Usuario.AuthMethod != AuthMethods.Twitter && MystiqueApp.Usuario.AuthMethod != AuthMethods.Email)
            {
                _entryName.Enabled = false;
                _entryLastname.Enabled = false;
            }
            if (MystiqueApp.Usuario.AuthMethod != AuthMethods.Email)
            {
                _entryPassword.Visibility = ViewStates.Gone;
                _entryPassword2.Visibility = ViewStates.Gone;
            }
            //_entryPassword.Visibility = ViewStates.Visible;
            //_entryPassword2.Visibility = ViewStates.Visible;
            _btnSave.Click += BtnSave_ClickAsync;
            _entryBirthday.Click += EntryBirthday_Click;
            //_btnSave.Visibility = ViewStates.Visible;

            //_entryPhone.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Telefono;

            //_btnFoto.Visibility = ViewStates.Visible;
            //_btnGaleria.Visibility = ViewStates.Visible;

            _fab.Visibility = ViewStates.Gone;
        }
        private void DisableEdit()
        {
            //btnInitRegister.Enabled = false;
            //btnInitRegister.Alpha = 0.5F;
            //btnBack.Enabled = false;
            //btnBack.Alpha = 0.5F;
            //btnFacebookConnect.Enabled = false;
            //btnFacebookConnect.Alpha = 0.5F;
            _layoutNombre.Visibility = ViewStates.Gone;
            _layoutApellido.Visibility = ViewStates.Gone;

            _entryName.Visibility = ViewStates.Gone;
            _entryLastname.Visibility = ViewStates.Gone;
            //entryLastname2.Visibility = ViewStates.Gone;
            _entryBirthday.Enabled = false;
            _entrySexo.Visibility = ViewStates.Visible;
            _spinnerSexo.Visibility = ViewStates.Gone;
            _spinnerCiudad.Visibility = ViewStates.Gone;
            _acColonia.Visibility = ViewStates.Gone;
            _entryColonia.Enabled = false;
            _entryPhone.Enabled = false;

            _btnSave.Click -= BtnSave_ClickAsync;
            _entryBirthday.Click -= EntryBirthday_Click;
            _btnSave.Visibility = ViewStates.Gone;

            _entryPassword.Visibility = ViewStates.Gone;
            _entryPassword2.Visibility = ViewStates.Gone;

            _btnFoto.Visibility = ViewStates.Gone;
            _btnGaleria.Visibility = ViewStates.Gone;

            _fab.Visibility = ViewStates.Visible;
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

        private async void Logout()
        {
            var bundle = new Bundle();
            bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
            bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.MetodosLogin.LOGOUT);
            _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.Login, bundle);

            //Com.Crashlytics.Android.Crashlytics.SetUserIdentifier("-1");
            Xamarin.Facebook.Login.LoginManager.Instance.LogOut();
            ViewModels.AuthViewModelV2.Instance.CerrarSesion();
            PreferencesHelper.SetSavedLoginMethod(AuthMethods.NotLogged);
            await Utils.SecureStorageHelper.SetCredentialsAsync("", "");
            ViewModels.AuthViewModelV2.Destroy();
            ViewModels.CitypointsViewModel.Destroy();
            ViewModels.HistorialViewModel.Destroy();
            ViewModels.NotificacionesViewModel.Destroy();
            ViewModels.TwitterLoginViewModel.Destroy();
            RestartMystique();
            //StartActivity(typeof(LoginActivity));
        }
        public void OpenQuieroDeComer()
        {
            var intent = Application.Context.PackageManager.GetLaunchIntentForPackage("com.QuieroDeComer");
            if (intent != null)
            {
                intent.PutExtra("p", MystiqueApp.IdQDC);
                intent.AddFlags(ActivityFlags.NewTask);
                StartActivity(intent);
            }
            else
            {
                intent = new Intent(Intent.ActionView);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.SetData(JavaUri.Parse("market://details?id=com.QuieroDeComer"));
                StartActivity(intent);
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
                default:
                    break;
            }
            return true;
        }
    }

}