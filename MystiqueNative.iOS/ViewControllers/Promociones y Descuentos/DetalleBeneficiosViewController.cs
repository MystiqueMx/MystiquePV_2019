using CoreGraphics;
using FFImageLoading;
using Foundation;
using MaterialControls;
using MystiqueNative.iOS.Helpers;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;



namespace MystiqueNative.iOS
{
    public partial class DetalleBeneficiosViewController : UIViewController
    {

        private static string NBSP = " ";
        static bool NoMostrar = false;
        public BeneficiosSucursal Beneficios { get; set; }
        public int Calificacion { get; set; }
        public string Sucursal { get; set; }
        public string SucursalID { get; set; }

        ModalQR sendCode = new ModalQR();

        ModalImagenBeneficio URL = new ModalImagenBeneficio();
        private NSTimer timer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        

            CalificarButton.Hidden = true;
            AddWaller.Hidden = true;

            AddWaller.MdButtonType = MaterialControls.MDButtonType.Raised;
            CalificarButton.MdButtonType = MaterialControls.MDButtonType.Raised;
            SolicitarCodeImage.MdButtonType = MaterialControls.MDButtonType.Raised;
            ImageService.Instance.LoadUrl(Beneficios.ImgBeneficio).DownSample(height: 200).Into(imageBeneficio);
            AppDelegate.Beneficios.PropertyChanged += Beneficios_PropertyChanged;
            AppDelegate.Beneficios.ObtenerDetalleBeneficios(Beneficios.IdBeneficio, SucursalID);

           
            labelNombreSucursal.Text = Sucursal;
            labelBeneficio.Text = Beneficios.Descripcion;
            labelHorarioBeneficio.Text = "Válido: " + Beneficios.Dias;

            labelNombreSucursal.AdjustsFontSizeToFitWidth = true;
            labelBeneficio.AdjustsFontSizeToFitWidth = true;
            labelHorarioBeneficio.AdjustsFontSizeToFitWidth = true;
            cantidadLabel.AdjustsFontSizeToFitWidth = true;

        }
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "IR_WALLET")
            {
                var tabVC = segue.DestinationViewController as UITabBarController;
                tabVC.SelectedIndex = 3;
            }
        }

        private void Wallet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AgregarStatus")
            {
                AppDelegate.Beneficios.ObtenerDetalleBeneficios(Beneficios.IdBeneficio, SucursalID);
            }
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            
            AppDelegate.Wallet.PropertyChanged += Wallet_PropertyChanged;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(6,181,158);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.Beneficios.PropertyChanged -= Beneficios_PropertyChanged;
            AppDelegate.Wallet.PropertyChanged -= Wallet_PropertyChanged;

        }


        private void Beneficios_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BeneficioSeleccionado")
            {
                if (AppDelegate.Beneficios.BeneficioSeleccionado != null)
                {
                    this.timer = NSTimer.CreateRepeatingScheduledTimer(0.1, (_) =>
                    {
                        if (!AppDelegate.Beneficios.BeneficioSeleccionado.EstaCalificado)
                        {
                            CalificarButton.Hidden = false;
                        }
                        else
                        {
                            CalificarButton.Hidden = true;
                            AddWaller.UserInteractionEnabled = false;
                            AddWaller.Enabled = false;
                        }

                        if (AppDelegate.Beneficios.BeneficioSeleccionado.EstaEnWallet)
                        {
                            AddWaller.Hidden = true;
                            AddWaller.UserInteractionEnabled = false;
                            AddWaller.Enabled = false;
                        }
                        else
                        {
                            AddWaller.Hidden = false;
                            AddWaller.UserInteractionEnabled = true;
                            AddWaller.Enabled = true;

                        }
                    });

                    calificacionLabel.Text = AppDelegate.Beneficios.BeneficioSeleccionado.CalificacionAsString;
                    EstrellasImg.Image = UIImage.FromBundle(promedioPuntuacionEstrellas(AppDelegate.Beneficios.BeneficioSeleccionado.CalificacionAsInt));
                    cantidadLabel.Text = AppDelegate.Beneficios.BeneficioSeleccionado.NumeroCalificaciones + " Calificaron";


                }
            }
        }

        private string promedioPuntuacionEstrellas(int calificacionPromedio)
        {
            switch (calificacionPromedio)
            {
                case 1:
                    return "onestar";
                case 2:
                    return "twostar";
                case 3:
                    return "threestar";
                case 4:
                    return "fourstar";
                case 5:
                    return "fivestar";
                default:
                    return "zerostar";
            }
        }

        private void Sucursales_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void Beneficios_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        public DetalleBeneficiosViewController(IntPtr handle) : base(handle)
        {
        }
        public static UIStoryboard StoryboardModales = UIStoryboard.FromName("Modales", null);
        partial void SolicitarCodeImage_TouchUpInside(MDButton sender)
        {
            ModalImagenBeneficio popupVC = StoryboardModales.InstantiateViewController("CanjearBeneficioQR") as ModalImagenBeneficio;
            popupVC.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            popupVC.URLImagenBeneficioCode =
                AppDelegate.Beneficios.BeneficioSeleccionado.CodigoQRString;

            popupVC.DescripcionBeneficioString =
                 AppDelegate.Beneficios.BeneficioSeleccionado.Descripcion;

            NavigationController.PresentModalViewController(popupVC, true);
        }

        partial void CalificarButton_TouchUpInside(MDButton sender)
        {
            CalificarBeneficio popupVC = Storyboard.InstantiateViewController("CalificarBeneficioID") as CalificarBeneficio;

            popupVC.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;

            popupVC.IdBeneficio = Beneficios.IdBeneficio;
            popupVC.IdSucursal = SucursalID;
            NavigationController.PresentViewController(popupVC, true, () =>
             {
                 AppDelegate.Beneficios.ObtenerDetalleBeneficios(Beneficios.IdBeneficio, SucursalID);
             });


        }

        partial void AddWaller_TouchUpInside(MDButton sender)
        {
            AppDelegate.Wallet.AgregarBeneficioWallet(SucursalID, Beneficios.IdBeneficio);
            if ( PreferencesHelper.GetMostrar()) 
            {
                var alert1 = UIAlertController.Create("Beneficios", "Al Guardar tu beneficio se irá a Mis Cupones", UIAlertControllerStyle.Alert);
                var WalletAction1 = UIAlertAction.Create("Aceptar", UIAlertActionStyle.Default, (OK1) =>
                {

                    var alert = UIAlertController.Create("Agregado", "Su beneficio ha sido agregado a Mis Cupones", UIAlertControllerStyle.Alert);
                    var WalletAction = UIAlertAction.Create("Ir a mis cupones", UIAlertActionStyle.Default, (OK) =>
                    {
                        PerformSegue("IR_AWALLET", this);
                    });
                    var OkAction = UIAlertAction.Create("Cerrar", UIAlertActionStyle.Default, null);
                    //   action.SetValueForKey(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), new NSString("image"));
                    // alert.AddAction(action);
                    alert.AddAction(OkAction);
                    alert.AddAction(WalletAction);
                    alert.PreferredAction = WalletAction;

                    PresentViewController(alert, true, null);

                });
                var OkAction1 = UIAlertAction.Create("No volver a mostrar", UIAlertActionStyle.Default, (NoMostrar) =>
                {
                PreferencesHelper.SetMostrarMensaje(false);

                });
                alert1.AddAction(OkAction1);
                alert1.AddAction(WalletAction1);
                alert1.PreferredAction = WalletAction1;

                PresentViewController(alert1, true, null);

            }
            else
            {
                var alert = UIAlertController.Create("Agregado", "Su beneficio ha sido agregado a Mis Cupones", UIAlertControllerStyle.Alert);
                var WalletAction = UIAlertAction.Create("Ir a mis cupones", UIAlertActionStyle.Default, (OK) =>
                {
                    PerformSegue("IR_AWALLET", this);
                });
                var OkAction = UIAlertAction.Create("Cerrar", UIAlertActionStyle.Default, null);
                //   action.SetValueForKey(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), new NSString("image"));
                // alert.AddAction(action);
                alert.AddAction(OkAction);
                alert.AddAction(WalletAction);
                alert.PreferredAction = WalletAction;

                PresentViewController(alert, true, null);
            }
        }

        partial void ShareButton_Activated(UIBarButtonItem sender)
        {
            var item = UIActivity.FromObject("Obten el beneficio "+ AppDelegate.Beneficios.BeneficioSeleccionado.Descripcion + " descargando la aplicacion:" );
            var activityItems = new NSObject[] { item };
            UIActivity[] applicationActivities = null;

            var activityController = new UIActivityViewController(activityItems, applicationActivities);

            PresentViewController(activityController, true, null);
        }

       
    }
}