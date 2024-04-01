using CoreGraphics;
using FFImageLoading;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ListaRecompensasCell : UITableViewCell
    {
        private string labelnombre;
        private string labelpuntos;
        private string image;
        public string IDCanjePuntos { get; set; }
        public nint tag;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            Disponibilidad.Hidden = true;
            NombreRecompensa.AdjustsFontSizeToFitWidth = true;
            Eliminar.TouchUpInside += delegate
            {

                //    this.Eliminar.Tag = tag;
                if (!AppDelegate.CityPoints.IsBusy)
                {
                    this.Hidden = true;
                    AppDelegate.CityPoints.EliminarRecompensa(IDCanjePuntos);
                }
                //AppDelegate.CityPoints.ObtenerRecompensasActivas();
                //AppDelegate.CityPoints.ObtenerEstadoCuenta();

            };
        }
        public string RecompensaQR { get; set; }

        public string LabelNombre { get => labelnombre; set { labelnombre = value; NombreRecompensa.Text = value; } }
        public string LabelPuntos { get => labelpuntos; set { labelpuntos = value; Estatus.Text = value; } }
        public string Image { get => image; set { image = value; ImageService.Instance.LoadUrl(value).DownSample(height: 200).Into(RecompensaImg); } }

        public RecompensasTableViewController ViewController { get; internal set; }

        public ListaRecompensasCell(IntPtr handle) : base(handle)
        {
        }
        public static UIStoryboard Storyboard = UIStoryboard.FromName("Recompensas", null);
        partial void SolicitarButton_TouchUpInside(UIButton sender)
        {
            this.SolicitarButton.Tag = tag;

            var Modal = Storyboard.InstantiateViewController("RecompensaQR") as ListaRecompensaModal;
            Modal.codigoQR = RecompensaQR;

            this.Window?.RootViewController?.PresentViewController(Modal, true, null);

            
        }
       
    }
}