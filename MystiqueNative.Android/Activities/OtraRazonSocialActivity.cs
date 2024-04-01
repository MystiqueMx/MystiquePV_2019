using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Helpers;
using MystiqueNative.ViewModels;
using System;
using Result = Android.App.Result;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Información del receptor", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class OtraRazonSocialActivity : BaseActivity, TextView.IOnEditorActionListener
    {
        public const int RequestNuevaRazonSocial = 1213;
        public const int RequestEditarRazonSocial = 1214;

        public const string ExtraIntentRfc = "ExtraFacturaRFC";
        public const string ExtraIntentRz = "ExtraFacturaRZ";
        public const string ExtraIntentEmail = "ExtraFacturaEmail";
        public const string ExtraIntentDireccion = "ExtraFacturaDireccion";
        public const string ExtraIntentIdReceptor = "ExtraReceptorCliente";
        public const string ExtraIntentCp = "ExtraCodigoPostal";
        public const string ExtraIntentCfdi = "ExtraFacturaCFDI";
        protected override int LayoutResource => Resource.Layout.activity_otra_razon_social;
        #region VIEWS

        private TextInputLayout _rfcLayout;
        private TextInputLayout _cpLayout;
        private TextInputLayout _direccionLayout;
        private TextInputLayout _emailLayout;
        private TextInputLayout _razonSocialLayout;

        private TextInputEditText _entryRfc;
        private TextInputEditText _entryCp;
        private TextInputEditText _entryDireccion;
        private TextInputEditText _entryEmail;
        private TextInputEditText _entryRazonSocial;

        private AppCompatSpinner _spinnerCfdi;

        private Button _buttonEnviar;
        #endregion
        #region FIELDS

        private string _idReceptorEdicion;
        private int _cfdiPos;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            TrySetIntentParameters();

        }

        private void TrySetIntentParameters()
        {
            var idEdicion = Intent.GetStringExtra(ExtraIntentIdReceptor);
            
            if (string.IsNullOrEmpty(idEdicion)) return;
            
            _idReceptorEdicion = idEdicion;
            _entryCp.Text = Intent.GetStringExtra(ExtraIntentCp);
            _entryDireccion.Text = Intent.GetStringExtra(ExtraIntentDireccion);
            _entryEmail.Text = Intent.GetStringExtra(ExtraIntentEmail);
            _entryRfc.Text = Intent.GetStringExtra(ExtraIntentRfc);
            _entryRazonSocial.Text = Intent.GetStringExtra(ExtraIntentRz);
            _cfdiPos = Intent.GetIntExtra(ExtraIntentCfdi, -1);
            
        }

        private void GrabViews()
        {
            _rfcLayout = FindViewById<TextInputLayout>(Resource.Id.otra_razon_layout_rfc);
            _cpLayout = FindViewById<TextInputLayout>(Resource.Id.otra_razon_layout_cp);
            _direccionLayout = FindViewById<TextInputLayout>(Resource.Id.otra_razon_layout_direccion);
            _emailLayout = FindViewById<TextInputLayout>(Resource.Id.otra_razon_layout_email);
            _razonSocialLayout = FindViewById<TextInputLayout>(Resource.Id.otra_razon_layout_rz);

            _entryRfc = FindViewById<TextInputEditText>(Resource.Id.otra_razon_input_rfc);
            _entryCp = FindViewById<TextInputEditText>(Resource.Id.otra_razon_input_cp);
            _entryDireccion = FindViewById<TextInputEditText>(Resource.Id.otra_razon_input_direccion);
            _entryEmail = FindViewById<TextInputEditText>(Resource.Id.otra_razon_input_email);
            _entryRazonSocial = FindViewById<TextInputEditText>(Resource.Id.otra_razon_input_rz);

            _spinnerCfdi = FindViewById<AppCompatSpinner>(Resource.Id.otra_razon_cfdi);

            _buttonEnviar = FindViewById<Button>(Resource.Id.otra_razon_button_enviar);
   
            _buttonEnviar.Click += ButtonEnviar_Click;
          
            _entryDireccion.SetOnEditorActionListener(this);

            var adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_custom_style, FacturacionViewModel.Instance.UsosCfdiAsStringArray());
            adapter.SetDropDownViewResource(Resource.Layout.spinner_custom_dropdown_item);
            _spinnerCfdi.Adapter = adapter;
            _spinnerCfdi.Background = Android.Support.V4.Content.ContextCompat.GetDrawable(this, Resource.Drawable.abc_edit_text_material);

        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_cfdiPos >= 0)
            {
                _spinnerCfdi.SetSelection(_cfdiPos);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }
        protected override void OnStop()
        {
            base.OnStop();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home) OnBackPressed();

            return true;
        }
        public override void OnBackPressed()
        {
            SetResult(Android.App.Result.Canceled);
            base.OnBackPressed();
        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (actionId != ImeAction.Send) return false;

            ButtonEnviar_Click(null, null);
            return true;
        }

        private void ButtonEnviar_Click(object sender, EventArgs e)
        {
            if (!ValidarInputs()) return;

            Intent intent = new Intent();
            intent.PutExtra(ExtraIntentRfc, _entryRfc.Text);
            intent.PutExtra(ExtraIntentCfdi, _spinnerCfdi.SelectedItemPosition);
            intent.PutExtra(ExtraIntentCp, _entryCp.Text);
            intent.PutExtra(ExtraIntentDireccion, _entryDireccion.Text);
            intent.PutExtra(ExtraIntentIdReceptor, _idReceptorEdicion);
            intent.PutExtra(ExtraIntentEmail, _entryEmail.Text);
            intent.PutExtra(ExtraIntentRz, _entryRazonSocial.Text);
            intent.PutExtra(ExtraIntentCp, _entryCp.Text);
            SetResult(Result.Ok, intent);
            Finish();
        }
        #region Validaciones
        private bool ValidarInputs()
        {
            var canContinue = true;
            View focusView = null;

            if (string.IsNullOrEmpty(_entryRazonSocial.Text))
            {
                _razonSocialLayout.Error = GetString(Resource.String.otra_razon_error_obligatorio);
                canContinue = false;
                focusView = _entryRazonSocial;
            }
            else
            {
                _razonSocialLayout.Error = string.Empty;
            }

            if (string.IsNullOrEmpty(_entryRfc.Text) )
            {
                _rfcLayout.Error = GetString(Resource.String.otra_razon_error_obligatorio);
                canContinue = false;
                focusView = _entryRfc;
            }
            else
            {
                if (!ValidatorHelper.IsValidRfc(_entryRfc.Text))
                {
                    _rfcLayout.Error = GetString(Resource.String.otra_razon_error_rfc);
                    canContinue = false;
                    focusView = _entryRfc;
                }
                else
                {
                    _rfcLayout.Error = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(_entryCp.Text))
            {
                _cpLayout.Error = GetString(Resource.String.otra_razon_error_obligatorio);
                canContinue = false;
                if(focusView == null) focusView = _entryCp;
            }
            else
            {
                if (!ValidatorHelper.IsValidPostalCode(_entryCp.Text))
                {
                    _cpLayout.Error = GetString(Resource.String.otra_razon_error_cp);
                    canContinue = false;
                    if (focusView == null) focusView = _entryCp;
                }
                else
                {
                    _cpLayout.Error = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(_entryEmail.Text))
            {
                _emailLayout.Error = GetString(Resource.String.otra_razon_error_obligatorio);
                canContinue = false;
                if (focusView == null) focusView = _entryEmail;
            }
            else
            {
                if (!ValidatorHelper.IsValidEmail(_entryEmail.Text))
                {
                    _emailLayout.Error = GetString(Resource.String.otra_razon_error_email);
                    canContinue = false;
                    if (focusView == null) focusView = _entryEmail;
                }
                else
                {
                    _emailLayout.Error = string.Empty;
                }
            }

            focusView?.RequestFocus();

            return canContinue;
        }
        #endregion
    }
}