using CoreGraphics;
using FFImageLoading;
using MystiqueNative.Models;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class RecompensasCell : UITableViewCell
    {
        private string labelnombre;
        private string labelpuntos;
        private string image;
        UIActivityIndicatorView indicator;
        public string Id { get; set; }
        public nint tag;
        public Recompensa RecompensaSeleccionada { get; set; }
        public static UIStoryboard Storyboard = UIStoryboard.FromName("Canjear", null);

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);


            #region SET Loading Activity
            indicator = new UIActivityIndicatorView(new CGRect(0, 0, 40, 40));
            indicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
            indicator.Color = UIColor.FromRGB(50, 50, 50);
            indicator.Center = Window.Center;
            Window.AddSubview(indicator);
            #endregion
            NombreRecompensa.AdjustsFontSizeToFitWidth = true;
            //RecompensaView.AddGestureRecognizer(new UITapGestureRecognizer((MostrarCodigo)));
        }

        private void CityPoints_OnCanjearRecompensaFinished(object sender, ViewModels.CanjearRecompensaArgs e)
        {

            // indicator.StopAnimating();
            ViewController.ActivityIndicator.StopAnimating();
            ViewController.ActivityIndicator.Hidden = true;
            AppDelegate.CityPoints.OnCanjearRecompensaFinished -= CityPoints_OnCanjearRecompensaFinished;
            if (e.Success)
            {
                ModalRecompensa Modal = Storyboard.InstantiateViewController("ModalRecompensaID") as ModalRecompensa;
                Modal.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                Modal.codigoQR = e.CodigoCanje.CodigoQR;
                Modal.NombreRecompensaD = e.Recompensa.Nombre.ToUpper();
                ViewController.PresentViewController(Modal, true, null);
            }
            else
            {
                var controller = GetVisibleViewController();
                var okAlertController = UIAlertController.Create("Canjear Puntos", e.ErrorMessage, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                ViewController.PresentViewController(okAlertController, true, null);
            }
        }

        public string LabelNombre { get => labelnombre; set { labelnombre = value; NombreRecompensa.Text = value; } }
        public string LabelPuntos { get => labelpuntos; set { labelpuntos = value; PuntosRecompensa.Text = value; } }
        public string Image { get => image; set { image = value; ImageService.Instance.LoadUrl(value).DownSample(height: 200).Into(RecompensaImg); } }

        public CanjearPuntosViewController ViewController { get; internal set; }

        public RecompensasCell(IntPtr handle) : base(handle)
        {
        }

        private void CityPoints_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "CanjearStatus")
            {
                if (AppDelegate.CityPoints.CanjearStatus)
                {

                }
                else
                {
                    AppDelegate.CityPoints.PropertyChanged -= CityPoints_Changed;
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    {

                        AppDelegate.CityPoints.PropertyChanged -= CityPoints_Changed;
                        var controller = GetVisibleViewController();
                        var okAlertController = UIAlertController.Create("Canjear Puntos", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        controller.PresentViewController(okAlertController, true, null);

                    }
                }
            }
            if (e.PropertyName == "ErrorStatus")
            {
                if (AppDelegate.CityPoints.EstadoCuenta != null)
                {
                    //PointsLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                }
            }

            if (e.PropertyName == "IsBusy")
            {
                if (AppDelegate.CityPoints.IsBusy)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        //  indicator.StartAnimating();
                    });
                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        //  indicator.StopAnimating();
                    });
                }
            }

        }

        private UIViewController GetVisibleViewController(UIViewController controller = null)
        {
            controller = controller ?? UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (controller.PresentedViewController == null)
                return controller;

            if (controller.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)controller.PresentedViewController).VisibleViewController;
            }

            if (controller.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)controller.PresentedViewController).SelectedViewController;
            }

            return GetVisibleViewController(controller.PresentedViewController);
        }

        partial void RecompensaButton_TouchUpInside(UIButton sender)
        {
            this.RecompensaButton.Tag = tag;
            AppDelegate.CityPoints.PropertyChanged += CityPoints_Changed;
            AppDelegate.CityPoints.OnCanjearRecompensaFinished += CityPoints_OnCanjearRecompensaFinished;

            if (AppDelegate.Auth.Usuario.RegistroCompleto)
            {
                if (AppDelegate.CityPoints.EstadoCuenta == null)
                {
                    var controller = GetVisibleViewController();
                    var okAlertController = UIAlertController.Create("Canjear Puntos", "No tienes los suficientes puntos", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    ViewController.PresentViewController(okAlertController, true, null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.EstadoCuenta.Puntos))
                    {
                        if (RecompensaSeleccionada.CostoAsInt > AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt)
                        {
                            var controller = GetVisibleViewController();
                            var okAlertController = UIAlertController.Create("Canjear Puntos", "No tienes los suficientes puntos", UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            ViewController.PresentViewController(okAlertController, true, null);
                        }
                        else
                        {
                            ViewController.ActivityIndicator.Hidden = false;
                            ViewController.ActivityIndicator.StartAnimating();
                            AppDelegate.CityPoints.CanjearRecompensa(RecompensaSeleccionada.Id);
                        }
                    }
                    else
                    {
                        var controller = GetVisibleViewController();
                        var okAlertController = UIAlertController.Create("Canjear Puntos", "No tienes los suficientes puntos", UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        ViewController.PresentViewController(okAlertController, true, null);
                    }
                }
            }
            else
            {
                var controller = GetVisibleViewController();
                var okAlertController = UIAlertController.Create("Registro", "Completa tu registro para continuar", UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                var PerfilAction = UIAlertAction.Create("Completar Registro", UIAlertActionStyle.Default, (OK) =>
                {
                    ViewController.PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                });
                okAlertController.AddAction(okAction);
                okAlertController.AddAction(PerfilAction);
                okAlertController.PreferredAction = PerfilAction;

                ViewController.PresentViewController(okAlertController, true, null);
            }
        }
    }
}