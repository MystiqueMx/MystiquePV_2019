using CoreGraphics;
using Foundation;
using MaterialControls;
using System;
using System.Threading.Tasks;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class EscanearTicketViewController : UIViewController, IScannerViewController
    {
        #region Declaraciones
        ZXingScannerView scannerView;
        public bool ContinuousScanning { get; set; }
        public MobileBarcodeScanningOptions ScanningOptions { get; set; }
        public MobileBarcodeScanner Scanner { get; set; }
        private string TextCode = "";
        public event Action<Result> OnScannedResult;
        UIActivityIndicatorView loadingView;
        #endregion
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModels.FacturacionViewModel.Instance.OnValidarTicketFinished += Instance_OnValidarTicketFinished;
            
            TorchButton.MdButtonType = MaterialControls.MDButtonType.FloatingAction;
            scannerView = scannerView = new ZXingScannerView(new CGRect(0, 0, ScanView.Frame.Size.Width, ScanView.Frame.Size.Height));
            scannerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            scannerView.UseCustomOverlayView = true;
            scannerView.CustomOverlayView = new UIView();
            ScanView.Add(scannerView);

        }

        private void Instance_OnValidarTicketFinished(object sender, ViewModels.ValidarTicketEventArgs e)
        {
            if (e.Success)
            {
                BeginInvokeOnMainThread(() =>
                {
                    var Alert = UIAlertController.Create("Escanear Ticket", "Ticket escaneado correctamente", UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, ((OK) =>
                    {
                        PerformSegue("FACTURAR_CONSUMO_SEGUE", this);
                    })));
                    PresentViewController(Alert, true, null);
                });

            }
            else
            {
                BeginInvokeOnMainThread(() =>
                {
                    var Alert = UIAlertController.Create("Escanear Ticket", e.Message, UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, ((OK) =>
                    {
                        NavigationController.PopViewController(true);
                    })));
                    PresentViewController(Alert, true, null);
                });
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ResumeAnalysis();
            var opt = new ZXing.Mobile.MobileBarcodeScanningOptions
            {
                PossibleFormats = new System.Collections.Generic.List<BarcodeFormat> {
                    BarcodeFormat.QR_CODE
                },
                DelayBetweenContinuousScans = 3000,
                //   UseNativeScanning = true
            };
            scannerView.StartScanning(OnScanResultReceived, opt);
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            Cancel();
            PauseAnalysis();
            scannerView.StopScanning();
        }

        private void OnScanResultReceived(ZXing.Result code)
        {
            if (code != null && !string.IsNullOrEmpty(code.Text))
            {
                ViewModels.FacturacionViewModel.Instance.ValidarTicket(code.Text);
            }
        }

        public EscanearTicketViewController(IntPtr handle) : base(handle)
        {
        }

        public bool IsTorchOn
        {
            get { return scannerView.IsTorchOn; }
        }


        public UIViewController AsViewController()
        {
            return this;
        }

        public void Cancel()
        {
            this.InvokeOnMainThread(scannerView.StopScanning);
        }

        public void PauseAnalysis()
        {
            scannerView.PauseAnalysis();
        }

        public void ResumeAnalysis()
        {
            scannerView.ResumeAnalysis();
        }

        public void ToggleTorch()
        {
            if (scannerView != null)
                scannerView.ToggleTorch();
        }

        public void Torch(bool on)
        {
            if (scannerView != null)
                scannerView.Torch(on);
        }

        partial void TorchButton_TouchUpInside(MDButton sender)
        {
            if (IsTorchOn)
            {
                TorchButton.SetImage(UIImage.FromBundle("flashlight-off"), UIControlState.Normal);
                scannerView.Torch(false);
            }
            else
            {
                TorchButton.SetImage(UIImage.FromFile("flashlight"), UIControlState.Normal);
                scannerView.Torch(true);
            }
        }
    }
}