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
using MystiqueNative.Droid.Utils.Tarjetas;
using MystiqueNative.Helpers;
using MystiqueNative.Models.OpenPay;

namespace MystiqueNative.Droid.HazPedido.Tarjetas
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Información de la tarjeta", WindowSoftInputMode = SoftInput.AdjustResize)]
    public class EdicionTarjetaActivity : BaseActivity
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_edicion_tarjeta; 
        #endregion

        #region EXPORTS

        public const int RequestAgregarTarjeta = 1484;
        public const string ExtraIdEdicion = "QdcApp.Droid.Tarjetas.EdicionTarjetaActivity.ExtraIdEdicion";

        #endregion
       
        #region VIEWS

        private TextInputLayout _layoutTitular;
        private TextInputLayout _layoutCvv;
        private TextInputLayout _layoutTarjeta;
        private TextInputLayout _layoutVencimiento;

        private TextInputEditText _entryTitular;
        private TextInputEditText _entryVencimiento;
        private TextInputEditText _entryTarjeta;
        private TextInputEditText _entryCvv;

        private Button _buttonGuardar;

        #endregion

        #region FIELDS

        private bool _addSlashFlag;
        private FrameLayout _progressBarHolder;

        #endregion
        
        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
        }

        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _layoutTitular = FindViewById<TextInputLayout>(Resource.Id.editar_tarjeta_layout_nombre);
            _layoutVencimiento = FindViewById<TextInputLayout>(Resource.Id.editar_tarjeta_layout_expiracion);
            _layoutTarjeta = FindViewById<TextInputLayout>(Resource.Id.editar_tarjeta_layout_tarjeta);
            _layoutCvv = FindViewById<TextInputLayout>(Resource.Id.editar_tarjeta_layout_cvv);

            _entryTitular = FindViewById<TextInputEditText>(Resource.Id.editar_tarjeta_entry_nombre);
            _entryVencimiento = FindViewById<TextInputEditText>(Resource.Id.editar_tarjeta_entry_expiracion);
            _entryTarjeta = FindViewById<TextInputEditText>(Resource.Id.editar_tarjeta_entry_tarjeta);
            _entryCvv = FindViewById<TextInputEditText>(Resource.Id.editar_tarjeta_entry_cvv);

            _buttonGuardar = FindViewById<Button>(Resource.Id.button_guardar);
            _buttonGuardar.Click += ButtonGuardar_Click;
            _entryVencimiento.BeforeTextChanged += EntryVencimiento_BeforeTextChanged;
            _entryVencimiento.AfterTextChanged += EntryVencimiento_AfterTextChanged;

            _entryTarjeta.AddTextChangedListener(new FourDigitCardTextWatcher());

            //var keyListener = DigitsKeyListener.GetInstance("1234567890");
            //_entryTarjeta.Set(keyListener);

            //_entryTarjeta.InputType = InputTypes.ClassPhone;
            //_entryTarjeta.SetFilters(new IInputFilter[] { new CreditCardInputFilter() });
        }

        protected override void OnResume()
        {
            base.OnResume();
            ViewModels.TarjetasViewModel.Instance.OnAgregarTarjetaFinished += Instance_OnAgregarTarjetaFinished;
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModels.TarjetasViewModel.Instance.OnAgregarTarjetaFinished -= Instance_OnAgregarTarjetaFinished;
        }

        #endregion

        private void EntryVencimiento_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            if (!_addSlashFlag) return;
            _addSlashFlag = false;

            e.Editable.Append('/');

        }

        private void EntryVencimiento_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (e.Text.Count() == 1 && e.BeforeCount < e.AfterCount)
            {
                _addSlashFlag = true;
            }
        }
        
        private async void ButtonGuardar_Click(object sender, System.EventArgs e)
        {
            if (!ValidarInputs()) return;
            StartAnimating();
            var expiracion = _entryVencimiento.Text.Split('/');
            if (expiracion.Length != 2) throw new ApplicationException();
            await ViewModels.TarjetasViewModel.Instance.AgregarTarjeta(new Card
            {
                CardNumber = _entryTarjeta.Text.Replace(" ", ""),
                Cvv = _entryCvv.Text,
                HolderName = _entryTitular.Text,
                ExpirationMonth = expiracion[0],
                ExpirationYear = expiracion[1]
            });
        }

        private bool ValidarInputs()
        {
            var canContinue = true;
            View focusView = null;

            if (string.IsNullOrEmpty(_entryTitular.Text))
            {
                _layoutTitular.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = _entryTitular;
            }
            else
            {
                _layoutTitular.Error = string.Empty;

            }

            if (string.IsNullOrEmpty(_entryTarjeta.Text.Replace(" ", "")))
            {
                _layoutTarjeta.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = focusView ?? _entryTarjeta;
            }
            else
            {
                if (ValidatorHelper.IsValidCreditCard(_entryTarjeta.Text.Replace(" ", "")))
                {
                    _layoutTarjeta.Error = string.Empty;
                }
                else
                {
                    _layoutTarjeta.Error = GetString(Resource.String.error_campos_vacios);
                    canContinue = false;
                    focusView = focusView ?? _entryTarjeta;
                }


            }

            if (string.IsNullOrEmpty(_entryVencimiento.Text))
            {
                _layoutVencimiento.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = _entryVencimiento;
            }
            else
            {

                if (ValidatorHelper.IsValidCreditCardExpiration(_entryVencimiento.Text))
                {
                    _layoutVencimiento.Error = string.Empty;
                }
                else
                {
                    _layoutVencimiento.Error = GetString(Resource.String.error_vencimiento_tarjeta_invalido_HP);
                    canContinue = false;
                    focusView = focusView ?? _entryVencimiento;
                }

            }

            if (string.IsNullOrEmpty(_entryCvv.Text))
            {
                _layoutCvv.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = _entryCvv;
            }
            else
            {
                _layoutCvv.Error = string.Empty;

            }

            focusView?.RequestFocus();

            return canContinue;
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

        private void Instance_OnAgregarTarjetaFinished(object sender, ViewModels.AgregarTarjetaEventArgs e)
        {
            if (e.Success)
            {
                var intent = new Intent();
                intent.PutExtra(ExtraIdEdicion, e.EditedId);
                SetResult(Result.Ok, intent);
                Finish();
                StopAnimating();
            }
            else
            {
                SendConfirmation(e.Message, "", "Volver", "", (ok) =>
                {
                    StopAnimating();
                });
            }
        }
       
        #region LOADING

        private void StartAnimating()
        {
            _buttonGuardar.Enabled = false;
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 100
            };
            //_progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimating()
        {
            _buttonGuardar.Enabled = true;
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 100
            };
            //_progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }

        #endregion
    }
}