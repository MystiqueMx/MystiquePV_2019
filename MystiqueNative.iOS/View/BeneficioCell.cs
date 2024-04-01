using CoreGraphics;
using FFImageLoading;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class BeneficioCell : UITableViewCell
    {
        private string label;
        private string image;

        public nint tag;
        public string ImagenSolicitar { get; set; }
        public string Descripcion { get; set; }
        public string IDBeneficio { get; set; }
        public string IdSucursal { get; set; }
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);


            AgregarFavoritos.Hidden = true;
            labelBeneficios.AdjustsFontSizeToFitWidth = true;
        }
        public BeneficioCell(IntPtr handle) : base(handle)
        {
        }

        public string Label { get => label; set { label = value; labelBeneficios.Text = value; } }
        public string Image { get => image; set { image = value; ImageService.Instance.LoadUrl(value).DownSample(height: 200).Into(ImageBeneficio); } }

        public BeneficiosTableViewController ViewController { get; internal set; }

        public static UIStoryboard Storyboard = UIStoryboard.FromName("Modales", null);
        public static UIStoryboard StoryboardCompletar = UIStoryboard.FromName("Main", null);
        partial void CanjearButton_TouchUpInside(UIButton sender)
        {
            this.CanjearButton.Tag = tag;
            var controller = GetVisibleViewController();

            var Modal = Storyboard.InstantiateViewController("CanjearBeneficioQR") as ModalImagenBeneficio;
            Modal.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            Modal.URLImagenBeneficioCode = ImagenSolicitar;
            Modal.DescripcionBeneficioString = Descripcion;

            if (AppDelegate.Auth.Usuario.RegistroCompleto)
            {
                ViewController.PresentViewController(Modal, true, null);
            }
            else
            {
                var okAlertController = UIAlertController.Create("Registro", "Completa tu registro para continuar", UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                var PerfilAction = UIAlertAction.Create("Completar Registro", UIAlertActionStyle.Default, (OK) =>
                {
                    ViewController.PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                });
                okAlertController.AddAction(okAction);
                okAlertController.AddAction(PerfilAction);
                okAlertController.PreferredAction = PerfilAction;
                //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (Oks) =>
                //{
                //    ViewController.PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                //   // controller.PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                //}));
                ViewController.PresentViewController(okAlertController, true, null);
            }

        }

        partial void ShareButton_TouchUpInside(UIButton sender)
        {
            var item = UIActivity.FromObject("Obtén el beneficio " + Descripcion + " descargando la aplicación: https://apps.apple.com/us/app/fresco-app/id1460400309?l=es&ls=1");
            var activityItems = new NSObject[] { item };
            UIActivity[] applicationActivities = null;

            var activityController = new UIActivityViewController(activityItems, applicationActivities);

            ViewController.PresentViewController(activityController, true, null);
        }

        partial void AgregarFavoritos_TouchUpInside(UIButton sender)
        {
            AppDelegate.Wallet.AgregarBeneficioWallet(IdSucursal, IDBeneficio);

            var alert = UIAlertController.Create("Agregado", "Se ha agregado a Favoritos", UIAlertControllerStyle.Alert);
            //var WalletAction = UIAlertAction.Create("Ir a Favoritos", UIAlertActionStyle.Default, (OK) =>
            //{
            //    this.Window?.RootViewController?.PerformSegue("IR_FAVORITOS", this.Window);

            //});
            var OkAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
            //   action.SetValueForKey(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), new NSString("image"));
            // alert.AddAction(action);
            alert.AddAction(OkAction);
            //alert.AddAction(WalletAction);
            alert.PreferredAction = OkAction;

            this.Window?.RootViewController?.PresentViewController(alert, true, () =>
            {
                AppDelegate.Beneficios.ObtenerBeneficioPorIdSucursal(IdSucursal);
            });
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
    }
}