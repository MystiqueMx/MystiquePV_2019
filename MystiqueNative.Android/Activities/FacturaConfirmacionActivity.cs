using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Confirmar datos fiscales", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FacturaConfirmacionActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_factura_confirmacion;
        #region EXTRAS
        public const string ExtraFacturaRfc = "ExtraFacturaRFC";
        public const string ExtraFacturaRz = "ExtraFacturaRZ";
        public const string ExtraFacturaCfdi = "ExtraFacturaCFDI";
        public const string ExtraFacturaEmail = "ExtraFacturaEmail";
        public const string ExtraFacturaDireccion = "ExtraFacturaDireccion";
        public const string ExtraReceptorCliente = "ExtraReceptorCliente";
        public const string ExtraCodigoPostal = "ExtraCodigoPostal";
        #endregion
        #region VIEWS
        private FrameLayout _progressBarHolder;
        private TextView _labelTitle;
        private TextView _labelId;
        private TextView _labelFecha;
        private TextView _labelTotal;
        private TextView _labelRazonSocial;
        private TextView _labelRfc;
        private TextView _labelEmail;
        private TextView _labelDireccion;
        private TextView _labelCfdi;
        private TextView _labelCp;

        private Button _buttonEnviar;
        private Button _buttonEditar;
        #endregion
        #region VARS

        private string _receptorId;
        private string _razonSocial;
        private string _rfc;
        private string _email;
        private int _cfdiPos;
        private string _direccion;
        private string _codigoPostal;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            TrySetIntentParameters(Intent);
        }

        private void GrabViews()
        {
            _labelTitle = FindViewById<TextView>(Resource.Id.label_title);
            _labelId = FindViewById<TextView>(Resource.Id.label_item);
            _labelFecha = FindViewById<TextView>(Resource.Id.label_fecha);
            _labelTotal = FindViewById<TextView>(Resource.Id.label_total);

            _labelRazonSocial = FindViewById<TextView>(Resource.Id.label_rz);
            _labelRfc = FindViewById<TextView>(Resource.Id.label_rfc);
            _labelEmail = FindViewById<TextView>(Resource.Id.label_email);
            _labelCfdi = FindViewById<TextView>(Resource.Id.label_cfdi);
            _labelDireccion = FindViewById<TextView>(Resource.Id.label_direccion);
            _labelCp = FindViewById<TextView>(Resource.Id.label_cp);

            _buttonEnviar = FindViewById<Button>(Resource.Id.button_confirmar);
            _buttonEditar = FindViewById<Button>(Resource.Id.button_editar);

            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);

            _buttonEnviar.Click += _buttonEnviar_Click;
            _buttonEditar.Click += _buttonEditar_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
            FacturacionViewModel.Instance.OnSolicitarFacturaFinished += Instance_OnSolicitarFacturaFinished;
        }

        protected override void OnPause()
        {
            base.OnPause();
            FacturacionViewModel.Instance.OnSolicitarFacturaFinished -= Instance_OnSolicitarFacturaFinished;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                OnBackPressed();
            }

            return true;
        }

        public override void OnBackPressed()
        {
            if (Build.VERSION.SdkInt == BuildVersionCodes.Lollipop)
            {
                SupportFinishAfterTransition();
            }
            base.OnBackPressed();
        }

        private void TrySetIntentParameters(Intent intent)
        {
            _razonSocial = intent.GetStringExtra(ExtraFacturaRz);
            _rfc = intent.GetStringExtra(ExtraFacturaRfc);
            _cfdiPos = intent.GetIntExtra(ExtraFacturaCfdi, -1);
            _email = intent.GetStringExtra(ExtraFacturaEmail);
            _direccion = intent.GetStringExtra(ExtraFacturaDireccion);
            _codigoPostal =intent.GetStringExtra(ExtraCodigoPostal);
            _receptorId =intent.GetStringExtra(ExtraReceptorCliente);

            if (FacturacionViewModel.Instance.UsosCfdi.Count == 0 || _cfdiPos < 0) Finish();
            
            _labelTitle.Text = $"{FacturacionViewModel.Instance.TicketEscaneado.Sucursal}";
            _labelId.Text = $"Ticket: {FacturacionViewModel.Instance.TicketEscaneado.Id}";
            _labelFecha.Visibility = ViewStates.Gone;
            _labelTotal.Text = $"Total: {FacturacionViewModel.Instance.TicketEscaneado.MontoCompra}";

            var descCfdi =
                FacturacionViewModel.Instance.UsosCfdiAsStringList()[_cfdiPos];

            _labelRazonSocial.Text = $"Razon Social: {_razonSocial}";
            _labelCp.Text = $"Codigo Postal: {_codigoPostal}";
            _labelRfc.Text = $"RFC: {_razonSocial}";
            _labelCfdi.Text = $"CFDI: {descCfdi}";
            _labelEmail.Text = $"Email: {_email}";
            _labelDireccion.Text = $"Dirección: {_direccion}";

        }

        private void _buttonEditar_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(OtraRazonSocialActivity));
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentIdReceptor, _receptorId);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentCp, _codigoPostal);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentDireccion, _direccion);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentEmail, _email);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentRfc, _rfc);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentRz, _razonSocial);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentCfdi, _cfdiPos);
            StartActivityForResult(intent, OtraRazonSocialActivity.RequestEditarRazonSocial);
            
        }

        private void _buttonEnviar_Click(object sender, System.EventArgs e)
        {
            var cfdi =
                FacturacionViewModel.Instance.UsosCfdi[_cfdiPos];

            var solicitud = new ReceptorFactura
            {
                CodigoPostal = _codigoPostal,
                Direccion = _direccion,
                Email = _email,
                RazonSocial = _razonSocial,
                Rfc = _rfc,
                UsoCFDI = cfdi.Id,
            };

            if (!string.IsNullOrEmpty(_receptorId))
            {
                solicitud.Id = _receptorId;
            }

            FacturacionViewModel.Instance.SolicitarFactura(solicitud);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Android.App.Result resultCode,
            Intent data)
        {
            var intent = new Intent(this, typeof(FacturaConfirmacionActivity));
            switch (requestCode)
            {
                case OtraRazonSocialActivity.RequestEditarRazonSocial when resultCode == Result.Ok:
                    TrySetIntentParameters(data);
                    break;
                default:
                    break;
            }
        }
        
        private void Instance_OnSolicitarFacturaFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            if (e.Success)
            {
                SendConfirmation(e.Message, "", "Salir","", ok =>
                {
                    Intent intent = new Intent(this, typeof(FacturacionActivity));
                    intent.AddFlags(ActivityFlags.ClearTop);
                    StartActivity(intent);
                });
            }
            else
            {
                SendMessage(e.Message);
            }
        }
    }
}