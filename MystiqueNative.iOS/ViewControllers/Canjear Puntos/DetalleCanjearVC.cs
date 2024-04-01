using Foundation;
using System;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using UIKit;
using FFImageLoading;
using CoreGraphics;
using System.ComponentModel;
using MystiqueNative.iOS.View;
using System.Threading.Tasks;

namespace MystiqueNative.iOS
{
    public partial class DetalleCanjearVC : UIViewController
    {
        LoadingOverlay loadPop;
        private NSTimer timer;

        public Recompensa RecompensaSeleccionada { get; set; }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NombreRecompensa.AdjustsFontSizeToFitWidth = true;

            #region CONSTRAINTS SCREENSIZES
            if (UIScreen.MainScreen.Bounds.Size.Height >= 1024)
            {
                NSLayoutConstraint.Create(ImagenRecompensa, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 400).Active = true;


            }
            if (UIScreen.MainScreen.Bounds.Size.Height == 736 || UIScreen.MainScreen.Bounds.Size.Height == 736)
            {
                //ImagenRecompensa.Frame = new CoreGraphics.CGRect(ImagenRecompensa.Frame.X, ImagenRecompensa.Frame.X,
                //                                                        ImagenRecompensa.Frame.Width,350);
                NSLayoutConstraint.Create(GenerarButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 45).Active = true;
                NSLayoutConstraint.Create(CanjearButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 45).Active = true;
                GenerarButton.TitleLabel.Font.WithSize(17);
                NSLayoutConstraint.Create(ImagenRecompensa, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 220).Active = true;


            }
            else
            {
                //GenerarButton
                NSLayoutConstraint.Create(CanjearButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 30).Active = true;
                NSLayoutConstraint.Create(GenerarButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 30).Active = true;
                NSLayoutConstraint.Create(ImagenRecompensa, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 142).Active = true;
            }
            #endregion

            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds, "Canjeando recompensa...");
            CanjeoStackl.UserInteractionEnabled = false;
            CanjeoStackl.Hidden = true;

            GenerarButton.Hidden = true;
            GenerarButton.UserInteractionEnabled = false;

            this.NavigationController.NavigationItem.Title = RecompensaSeleccionada.Nombre;
            NavigationItem.Title = RecompensaSeleccionada.Nombre;
            if (!string.IsNullOrEmpty(RecompensaSeleccionada.ImgRecompensa))
            {
                ImageService.Instance.LoadUrl(RecompensaSeleccionada.ImgRecompensa).DownSample(height: 200).Into(ImagenRecompensa);
            }
            NombreRecompensa.Text = RecompensaSeleccionada.Nombre;
            CostoRecompensa.Text = RecompensaSeleccionada.Costo + " pts";
            DescripcionRecompensa.Text = RecompensaSeleccionada.Descripcion;

           // AppDelegate.CityPoints.PropertyChanged += CityPoints_Changed;

            if (AppDelegate.CityPoints.EstadoCuenta != null)
            {
                
                PointsLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                SaldoRestante.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
            }

        }

        private void CityPoints_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "CanjearStatus")
            {
                if (AppDelegate.CityPoints.CanjearStatus)

                {

                    //CanjeoStackl.UserInteractionEnabled = true;
                    //CanjeoStackl.Hidden = false;

                    //CanjearButton.Hidden = true;
                    //CanjearButton.UserInteractionEnabled = false;

                    //GenerarButton.Hidden = false;
                    //GenerarButton.UserInteractionEnabled = true;

                    //AppDelegate.CityPoints.ObtenerEstadoCuenta();
                    
                    //Console.WriteLine("\n\n\n\n\n CODIGO GENERADO EQUISDE" + AppDelegate.CityPoints.CodigoCanje.CodigoQR);

                    //ModalRecompensa Modal = Storyboard.InstantiateViewController("ModalRecompensaID") as ModalRecompensa;
                    //Modal.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;

                    //Modal.codigoQR =
                    //AppDelegate.CityPoints.CodigoCanje.CodigoQR;
                    //PointsLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                    //SaldoRestante.Text = (AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt - RecompensaSeleccionada.CostoAsInt).ToString();
                    //NavigationController.PresentViewController(Modal, true, null);

                }
                else
                {
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    {
                        RecompensaStack.UserInteractionEnabled = true;
                        RecompensaStack.Hidden = false;
                        var okAlertController = UIAlertController.Create("Canjear Puntos", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        PresentViewController(okAlertController, true, null);
                        AppDelegate.CityPoints.ErrorMessage = string.Empty;
                    }
                }
            }
            if (e.PropertyName == "ErrorStatus")
            {
                if (AppDelegate.CityPoints.EstadoCuenta != null)
                {
                    PointsLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString(); 
                }
            }

            if (e.PropertyName == "IsBusy")
            {
                if (AppDelegate.CityPoints.IsBusy)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        View.EndEditing(true);


                    });
                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                   {

                       View.Hidden = false;
                       loadPop.Hide();
                   });
                }
            }

        }


        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            #region SCREENSIZES
            /*   if phone && maxLength < 568 {
            return .iphone4
        }
        else if phone && maxLength == 568 {
            return .iphone5
        }
        else if phone && maxLength == 667 {
            return .iphone6
        }
        else if phone && maxLength == 736 {
            return .iphone6plus
        }
        else if phone && maxLength == 812 {
            return .iphoneX
        }
        return .unknown*/
            #endregion

            NombreCanjeo.Text = RecompensaSeleccionada.Nombre;
            CostoRecompensaCanjeo.Text = RecompensaSeleccionada.Costo;

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.CityPoints.PropertyChanged -= CityPoints_Changed;

        }


        public DetalleCanjearVC(IntPtr handle) : base(handle)
        {
        }

        partial void CanjearButton_TouchUpInside(UIButton sender)
        {
            if (AppDelegate.CityPoints.EstadoCuenta != null)
            {
                if (AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt < RecompensaSeleccionada.CostoAsInt ||
                    AppDelegate.CityPoints.EstadoCuenta.Puntos == null)
                {
                    #region SALDO INSUFICIENTE MSG
                    var okAlertController = UIAlertController.Create("Canjear Recompensa", "No cuentas con Citypoints suficientes", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(okAlertController, true, null);
                    #endregion

                }
                else
                {
                    View.Add(loadPop);
                    SaldoDisponible.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                    AppDelegate.CityPoints.CanjearRecompensa(RecompensaSeleccionada.Id);

                    RecompensaStack.UserInteractionEnabled = false;
                    RecompensaStack.Hidden = true;
                }
            }
            else
            {
                #region SALDO INSUFICIENTE MSG
                var okAlertController = UIAlertController.Create("Canjear Puntos", "No cuentas con Citypoints suficientes", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(okAlertController, true, null);
                #endregion

            }
        }

    
    }
}