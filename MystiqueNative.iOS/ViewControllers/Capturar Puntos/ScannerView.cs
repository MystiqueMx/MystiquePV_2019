using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;
using ZXing;

namespace MystiqueNative.iOS
{
    public partial class ScannerView : UIView
    {
        public event EventHandler<ScannedCodeEventArgs> OnScannedItem;
        private ZXing.Mobile.ZXingScannerView scannerView;
        public ScannerView(IntPtr handle) : base(handle) { }
        public void InitScanner(nfloat width, nfloat height)
        {
            scannerView = new ZXing.Mobile.ZXingScannerView(new CGRect(0, 0, width, height));

            var opt = new ZXing.Mobile.MobileBarcodeScanningOptions
            {
                PossibleFormats = new System.Collections.Generic.List<BarcodeFormat> {
                    BarcodeFormat.QR_CODE
                },
                DelayBetweenContinuousScans = 3000,
                //   UseNativeScanning = true
            };
            //scannerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            scannerView.CancelButtonText = "";
            scannerView.FlashButtonText = "FLASH";

            scannerView.UseCustomOverlayView = true;
            scannerView.CustomOverlayView = new UIView();
            this.AddSubview(scannerView);
            scannerView.StartScanning(OnScanResultReceived, opt);
            //  scannerView.PauseAnalysis();


        }


        public void StartScanning()
        {
            scannerView.ResumeAnalysis();
        }
        public void StopScanning()
        {
            scannerView.PauseAnalysis();
        }
        public void Cancel()
        {
            this.InvokeOnMainThread(scannerView.StopScanning);
            scannerView.PauseAnalysis();
        }
        public void SetFlashlight(bool IsOn)
        {
            scannerView.Torch(IsOn);
        }
        public bool IsTorchOn()
        {
            return scannerView.IsTorchOn;
        }
        private void OnScanResultReceived(ZXing.Result code)
        {
            if (code != null && !string.IsNullOrEmpty(code.Text))
            {
                OnScannedItem?.Invoke(this, new ScannedCodeEventArgs { Success = true, Code = code?.Text, Format = code.BarcodeFormat });
            }
            else
            {
                OnScannedItem?.Invoke(this, new ScannedCodeEventArgs { Success = false });
            }
        }
    }
    public class PlayerView : UIView
    {
        public AVPlayer Player { get => PlayerLayer.Player; set { PlayerLayer.Player = value; } }
        public AVPlayerLayer PlayerLayer
        {
            get
            {
                return Layer as AVPlayerLayer;
            }
        }
        [Export("layerClass")]
        public static ObjCRuntime.Class LayerClass()
        {
            return new ObjCRuntime.Class(typeof(AVPlayerLayer));
        }
    }
    public class ScannedCodeEventArgs
    {
        public bool Success { get; set; }
        public string Code { get; set; }
        public ZXing.BarcodeFormat Format { get; set; }
    }
}