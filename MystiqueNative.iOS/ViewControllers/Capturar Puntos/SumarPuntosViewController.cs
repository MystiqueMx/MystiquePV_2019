using AVFoundation;
using Foundation;
using MaterialControls;
using System;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class SumarPuntosViewController : UIViewController
    {
        #region Declaraciones
        bool IsOn = false;
        #endregion
        #region FIELDS
        #endregion
        #region VIEWS
        #endregion
        #region CTOR
        public SumarPuntosViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion
        #region LIFECYCLE
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TorchButton.MdButtonType = MaterialControls.MDButtonType.FloatingAction;

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ScanWindow.InitScanner(ScanWindow.Frame.Size.Width, ScanWindow.Frame.Size.Height);

            ViewModels.CitypointsViewModel.Instance.OnAgregarPuntosFinished += Instance_OnAgregarPuntosFinished;

            ScanWindow.OnScannedItem += ScanWindow_OnScannedItem;
            ScanWindow.StartScanning();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            ViewModels.CitypointsViewModel.Instance.OnAgregarPuntosFinished -= Instance_OnAgregarPuntosFinished;
            ScanWindow.OnScannedItem -= ScanWindow_OnScannedItem;
            ScanWindow.StopScanning();
            ScanWindow.Cancel();
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ScanWindow.SetFlashlight(false);
            ScanWindow.Cancel();
        }
        #endregion
        #region EVENT HANDLERS

        private void ScanWindow_OnScannedItem(object sender, ScannedCodeEventArgs e)
        {
            if (e.Success)
            {
                AppDelegate.CityPoints.AgregarPuntos(e.Code);
                ViewModels.CitypointsViewModel.Instance.AgregarPuntos(e.Code);
            }
            else
            {
                ScanWindow.StopScanning();
               
            }
        }

        private void Instance_OnAgregarPuntosFinished(object sender, ViewModels.AgregarPuntosArgs e)
        {
            if (e.Success)
            {
                BeginInvokeOnMainThread(() =>
                {
                    var Alert = UIAlertController.Create("Canjear Puntos", e.Message, UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("Escanear otro código", UIAlertActionStyle.Default, null));
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, ((OK) =>
                    {
                        NavigationController.PopViewController(true);
                    })));

                    PresentViewController(Alert, true, null);
                });
            }
            else
            {
                BeginInvokeOnMainThread(() =>
                {
                    var Alert = UIAlertController.Create("Canjear Puntos", e.Message, UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("Intentar de nuevo", UIAlertActionStyle.Default, null));
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, ((OK) =>
                    {
                        NavigationController.PopViewController(true);
                    })));

                    PresentViewController(Alert, true, null);
                });
            }
        }
        #endregion
        #region SPINNER
        private void StartSpinner()
        {
            // TODO Agregar spinner mientras registra ticket
            ScanWindow.StopScanning();
        }
        private void StopSpinner()
        {
            ScanWindow.StartScanning();
        }

        partial void TorchButton_TouchUpInside(MDButton sender)
        {
            if (ScanWindow.IsTorchOn())
            {
                TorchButton.SetImage(UIImage.FromBundle("flashlight-off"), UIControlState.Normal);
                ScanWindow.SetFlashlight(false);
            }
            else
            {
                TorchButton.SetImage(UIImage.FromFile("flashlight"), UIControlState.Normal);
                ScanWindow.SetFlashlight(true);
            }

        }
        #endregion

    }
}