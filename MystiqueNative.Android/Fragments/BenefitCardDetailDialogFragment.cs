using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.Droid.Fragments
{
    public class BenefitCardDetailDialogFragment : AppCompatDialogFragment
    {
        private readonly string _qrCode;
        private readonly string _title;
        private ImageView _qrview;
        private TextView _textview;
        private View _view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _view = inflater.Inflate(Resource.Layout.dialog_fragment_benefit_card_generated, container, false);
            _qrview = _view.FindViewById<ImageView>(Resource.Id.qr_code);
            _textview = _view.FindViewById<TextView>(Resource.Id.title_template);
            _view.FindViewById<Button>(Resource.Id.dialog_barcode_close).Click += Imageview_Click;
            _textview.Text = $"{_title}";

            const int w = 150;
            const int h = w;
            var bitmapRenderer = new BitmapRenderer();
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = w,
                    Height = h
                },
                Renderer = bitmapRenderer
            };
            using(var b = barcodeWriter.Write(_qrCode))
                _qrview.SetImageBitmap(b);

            bitmapRenderer = null;
            return _view;
        }

        private void Imageview_Click(object sender, System.EventArgs e)
        {
            Dismiss();
            _qrview.Dispose();
            _textview.Dispose();
            _view.Dispose();
#pragma warning disable S1215 // "GC.Collect" should not be called
            System.GC.Collect(0);
#pragma warning restore S1215 // "GC.Collect" should not be called
        }

        public BenefitCardDetailDialogFragment(string title, string qrcode)
        {
            _qrCode = qrcode;
            _title = title;
        }
    }
}