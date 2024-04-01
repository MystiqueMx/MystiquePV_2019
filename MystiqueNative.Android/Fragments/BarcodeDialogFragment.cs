using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.Droid.Fragments
{
    public class BarcodeDialogFragment : AppCompatDialogFragment
    {
        private View _view;
        private Button _action;
        private ImageView _codeImageView;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (_barcodeContent == null)
                throw new ArgumentNullException("barcodeContent");

            base.OnCreate(savedInstanceState);

            _view = inflater.Inflate(Resource.Layout.dialog_fragment_barcode, container, false);
            if (!string.IsNullOrEmpty(_navigationButton))
            {
                _action = _view.FindViewById<Button>(Resource.Id.dialog_barcode_action);
                _action.Text = _navigationButton;
                _action.Click += (s, ev) =>
                {
                    DialogClosed?.Invoke(this, new BarcodeDialogEventArgs { Navigate = true });
                    Dismiss();
                };
                _action.Visibility = ViewStates.Visible;
            }
            _view.FindViewById<Button>(Resource.Id.dialog_barcode_close).Click+=(s,ev)=> 
            {
                DialogClosed?.Invoke(this, new BarcodeDialogEventArgs{ Navigate = false });
                Dismiss();
            };
            _codeImageView = _view.FindViewById<ImageView>(Resource.Id.image_barcode);

            const int w = 500;

            var barcodeWriter = new BarcodeWriter
            {
                Format = _barcodeFormat,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = w,
                    Height = _barcodeFormat == BarcodeFormat.QR_CODE ? 500 : 200
                }
            };
            using(var b = barcodeWriter.Write(_barcodeContent))
                _codeImageView.SetImageBitmap(b);

            return _view;
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            _action?.Dispose();
            _codeImageView?.Dispose();
            _view?.Dispose();
#pragma warning disable S1215 // "GC.Collect" should not be called
            GC.Collect(0,GCCollectionMode.Optimized,false,true);
#pragma warning restore S1215 // "GC.Collect" should not be called
        }

        public BarcodeDialogFragment(string barcodeContent, BarcodeFormat barcodeFormat, string navigationButton = "")
        {
            _barcodeContent = barcodeContent;
            _barcodeFormat = barcodeFormat;
            _navigationButton = navigationButton; 
        }
        private readonly string _barcodeContent;
        private readonly string _navigationButton;
        private readonly BarcodeFormat _barcodeFormat;
        public event EventHandler<BarcodeDialogEventArgs> DialogClosed;

    }
    public class BarcodeDialogEventArgs
    {
        public bool Navigate { get; set; }
    }
}