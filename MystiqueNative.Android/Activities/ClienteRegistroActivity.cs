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
using MystiqueNative.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask)]
    public class ClienteRegistroActivity : BaseActivity
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_clientes_registro;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #endregion

        #region EXPORT

        #endregion

        #region FIELDS
        private LinearLayout linearRegistroFormulario, linearRegistroBusqueda;
        private TextInputEditText tietNombre;
        private TextInputEditText tietPaterno;
        private TextInputEditText tietMaterno;
        private TextInputEditText tietTelefono, tietTelefonoBusqueda;
        private Button btnRegistrarCliente, btnBuscarCliente;
        private FrameLayout progressBarHolder;
        #endregion

        #region LIFECYCLE
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
        }

        protected override void OnPause()
        {
            base.OnPause();
            ClientesViewModel.Instance.OnFinishRegisterCliente -= Instance_OnFinishRegisterCliente;
            ClientesViewModel.Instance.OnFinishBuscarCliente -= Instance_OnFinishBuscarCliente;
        }

        protected override void OnResume()
        {
            base.OnResume();
            ClientesViewModel.Instance.OnFinishRegisterCliente += Instance_OnFinishRegisterCliente;
            ClientesViewModel.Instance.OnFinishBuscarCliente += Instance_OnFinishBuscarCliente;
        }
        #endregion

        #region OVERRIDES
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
                default: return base.OnOptionsItemSelected(item);
            }
            return true;
        }
        #endregion

        #region EVENTS
        private async void ButtonBuscarCliente(object sender, EventArgs e)
        {
            #region ButtonBuscarCliente
            if (!validarTelefono()) return;
            StartLoading();
            await ClientesViewModel.Instance.BuscarClienteCallCenter(tietTelefonoBusqueda.Text);
            #endregion
        }

        private async void ButtonRegistrarCliente(object sender, EventArgs e)
        {
            #region ButtonRegistrarCliente
            if (!validarInputs()) return;
            StartLoading();
            await ClientesViewModel.Instance.RegistrarClienteCallCenter(tietNombre.Text, tietPaterno.Text, tietMaterno.Text, tietTelefono.Text);
            #endregion
        }
        #endregion

        #region METHODS
        private void GrabViews()
        {
            #region GrabViews
            tietNombre = FindViewById<TextInputEditText>(Resource.Id.tiet_registro_cliente_nombre);
            tietPaterno = FindViewById<TextInputEditText>(Resource.Id.tiet_registro_cliente_paterno);
            tietMaterno = FindViewById<TextInputEditText>(Resource.Id.tiet_registro_cliente_materno);
            tietTelefono = FindViewById<TextInputEditText>(Resource.Id.tiet_registro_cliente_telefono);
            tietTelefonoBusqueda = FindViewById<TextInputEditText>(Resource.Id.tiet_registro_cliente_busqueda_telefono);
            btnRegistrarCliente = FindViewById<Button>(Resource.Id.btn_registro_cliente_registrar);
            btnBuscarCliente = FindViewById<Button>(Resource.Id.btn_registro_cliente_buscar);
            progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            linearRegistroBusqueda = FindViewById<LinearLayout>(Resource.Id.linear_registro_busqueda);
            linearRegistroFormulario = FindViewById<LinearLayout>(Resource.Id.linear_registro_formulario);

            btnRegistrarCliente.Click += ButtonRegistrarCliente;
            btnBuscarCliente.Click += ButtonBuscarCliente;
            #endregion
        }

        private bool validarInputs()
        {
            #region validarInputs
            if (string.IsNullOrEmpty(tietNombre.Text))
            {
                SendMessage("El campo nombre es obligatorio");
                return false;
            }
            else
            {
                if (ValidatorHelper.IsValidName(tietNombre.Text))
                {

                }
                else
                {
                    SendMessage("El formato del nombre es inválido");
                    return false;
                }
            }

            if (string.IsNullOrEmpty(tietPaterno.Text))
            {
                SendMessage("El campo apellido paterno es obligatorio");
                return false;
            }
            else
            {
                if (ValidatorHelper.IsValidName(tietPaterno.Text))
                {

                }
                else
                {
                    SendMessage("El formato del apellido paterno es inválido");
                    return false;
                }
            }

            if (string.IsNullOrEmpty(tietTelefono.Text))
            {
                SendToast("El campo teléfono es obligatorio");
                return false;
            }
            else
            {
                if (ValidatorHelper.IsValidPhone(tietTelefono.Text))
                {

                }
                else
                {
                    SendToast("El formato del teléfono es inválido");
                    return false;
                }

            }

            return true; 
            #endregion
        }

        private bool validarTelefono()
        {
            #region validarTelefono
            if (string.IsNullOrEmpty(tietTelefonoBusqueda.Text))
            {
                SendToast("El campo teléfono es obligatorio");
                return false;
            }
            else
            {
                if (ValidatorHelper.IsValidPhone(tietTelefonoBusqueda.Text))
                {
                    return true;
                }
                else
                {
                    SendToast("El formato del teléfono es inválido");
                    return false;
                }
            } 
            #endregion
        }
        #endregion

        #region CALLBACKS
        private void Instance_OnFinishRegisterCliente(object sender, BaseEventArgs e)
        {
            #region Instance_OnFinishRegisterCliente
            SendToast(e.Message);
            StopLoading();
            if (e.Success)
            {
                tietNombre.Text = string.Empty;
                tietPaterno.Text = string.Empty;
                tietMaterno.Text = string.Empty;
                tietTelefono.Text = string.Empty;
                OnBackPressed();
            } else
            {

            }
            #endregion
        }

        private void Instance_OnFinishBuscarCliente(object sender, BaseEventArgs e)
        {
            #region Instance_OnFinishBuscarCliente            
            StopLoading();
            if (e.Success)
            {
                if (!ClientesViewModel.Instance.clienteEncontrado)
                {
                    tietTelefono.Text = tietTelefonoBusqueda.Text;
                    tietTelefono.Enabled = false;
                    tietTelefonoBusqueda.Text = string.Empty;
                    linearRegistroBusqueda.Visibility = ViewStates.Gone;
                    linearRegistroFormulario.Visibility = ViewStates.Visible;
                } else
                {
                    SendMessage(e.Message);
                }
            }
            else
            {
                SendMessage(e.Message);
            }
            #endregion
        }
        #endregion

        #region LOADING

        private void StartLoading()
        {
            #region StartAnimating
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = inAnimation;
            progressBarHolder.Visibility = ViewStates.Visible; 
            #endregion
        }

        private void StopLoading()
        {
            #region StopAnimating
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = outAnimation;
            progressBarHolder.Visibility = ViewStates.Gone; 
            #endregion
        }

        #endregion
    }
}