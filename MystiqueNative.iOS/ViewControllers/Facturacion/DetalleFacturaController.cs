using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class DetalleFacturaController : UIViewController
    {
        public DetalleFacturaController (IntPtr handle) : base (handle)
        {
        }
        internal string id;
        internal string folio;
        internal string sucursal;
        internal string fecha;
        internal string fecharegistro;
        internal string estatus;
        internal string montocompra;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            FolioLabel.Text = folio;
            SucursalLabel.Text = sucursal;
            FechaLabel.Text = fecha;
            EstatusLabel.Text = estatus;
            TotalLabel.Text = montocompra;
        }
        partial void ReenviarButton_TouchUpInside(UIButton sender)
        {
            //TODO REENVIAR FACTURA AL CORREO
            var Alert = UIAlertController.Create("Reenviar factura", "Se ha enviado la factura al siguiente correo: mail@mail.com", UIAlertControllerStyle.Alert);
            Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(Alert, true, null);
        }
    }
}