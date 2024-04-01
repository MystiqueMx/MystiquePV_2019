using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Factura", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FacturaDetalleActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_factura_detalle;
        public const string ExtraFacturaPosition = "EXTRA_FACTURA_POSITION";
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

        private Button _buttonReenviar;
        private Factura _item;
        private int _pos;

        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            TrySetIntentParameters();
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

            _buttonReenviar = FindViewById<Button>(Resource.Id.button_solicitar);
            _buttonReenviar.Click += _buttonReenviar_Click;

            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
        }

        
        protected override void OnResume()
        {
            base.OnResume();
            FacturacionViewModel.Instance.OnReenviarFacturaFinished += Instance_OnReenviarFacturaFinished;
        }

        protected override void OnPause()
        {
            base.OnPause();
            FacturacionViewModel.Instance.OnReenviarFacturaFinished -= Instance_OnReenviarFacturaFinished;
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
            if (Build.VERSION.SdkInt == BuildVersionCodes.LollipopMr1)
            {
                SupportFinishAfterTransition();
            }
            base.OnBackPressed();
        }

        private void TrySetIntentParameters()
        {
            _pos = Intent.GetIntExtra(ExtraFacturaPosition, -1);
            
            if (_pos < 0)
            {
                _buttonReenviar.Visibility = ViewStates.Gone;
                return;
            };

            _item = FacturacionViewModel.Instance.Facturas[_pos];

            _labelTitle.Text = $"{_item.Sucursal} - {_item.Estatus}";
            _labelId.Text = $"Ticket: {_item.Folio}";
            _labelFecha.Text = $"Fecha: {_item.FechaCompraConFormatoEspanyol}";
            _labelTotal.Text = $"Total: {_item.MontoCompraConFormatoMoneda}";

            _labelRazonSocial.Text = $"Razon Social: {_item.RazonSocialReceptor}";
            _labelRfc.Text = $"RFC: {_item.RfcReceptor}";
            _labelEmail.Text = $"Email: {_item.EmailReceptor}";

            _buttonReenviar.Visibility = _item.PuedeReenviar ? ViewStates.Visible : ViewStates.Gone;
            

        }

        private void _buttonReenviar_Click(object sender, System.EventArgs e)
        {
            FacturacionViewModel.Instance.ReenviarFactura(_pos, _item.EmailReceptor);
        }

        private void Instance_OnReenviarFacturaFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            SendMessage(e.Message);
        }

    }
}