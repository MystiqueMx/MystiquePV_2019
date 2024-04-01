using CoreGraphics;
using FFImageLoading;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class WalletBeneficioCell : UITableViewCell
    {
        private string imageBeneficio;
        private string labelDescripcion;
        private string labelCuenta;
        public  nint tag;
        public string ImagenSolicitar { get; set; }
        public string Descripcion { get; set; }
        public string IDBeneficio { get; set; }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            BeneficioDescripcion.AdjustsFontSizeToFitWidth = true;
            tiempo.AdjustsFontSizeToFitWidth = true;
            AppDelegate.Wallet.PropertyChanged += Wallet_PropertyChanged;
            
        }

        private void Wallet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
         
        }
        public string Label { get => labelDescripcion; set { labelDescripcion = value; BeneficioDescripcion.Text = value; } }
        public string Label2 { get => labelCuenta; set { labelCuenta = value; tiempo.Text = value; } }
        public string Image { get => imageBeneficio; set { imageBeneficio = value; ImageService.Instance.LoadUrl(value).DownSample(height: 200).Into(imagenBeneficio); } }


        public WalletBeneficioCell(IntPtr handle) : base(handle)
        {
        }
        public static UIStoryboard Storyboard = UIStoryboard.FromName("Modales", null);
        partial void SolicitarButton_TouchUpInside(UIButton sender)
        {
            this.SolicitarButton.Tag = tag;

            var Modal = Storyboard.InstantiateViewController("CanjearBeneficioQR") as ModalImagenBeneficio;
            Modal.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            Modal.URLImagenBeneficioCode = ImagenSolicitar;
            Modal.DescripcionBeneficioString = Descripcion;

            this.Window?.RootViewController?.PresentViewController(Modal, true, null);
     
        }
        
        partial void EliminarButton_TouchUpInside(UIButton sender)
        {
            this.EliminarButton.Tag = tag;
            AppDelegate.Wallet.EliminarBeneficioWallet(IDBeneficio);
            AppDelegate.Wallet.ObtenerBeneficiosWallet();
        }
     
    }
}