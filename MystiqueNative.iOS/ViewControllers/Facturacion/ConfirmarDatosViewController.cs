using Foundation;
using MystiqueNative.Models;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ConfirmarDatosViewController : UIViewController
    {
        #region DECLARACIONES
        internal string id;
        internal string razonsocial;
        internal string rfc;
        internal string correo;
        internal string codigopostal;
        internal string direccion;
        internal string cfdi;
        #endregion

        #region CONSTRUCTOR
        public ConfirmarDatosViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RFCLabel.Text = rfc;
            RazonSocialLabel.Text = razonsocial;
            EmailLabel.Text = correo;
            CodigoPostalLabel.Text = codigopostal;
            DireccionLabel.Text = direccion;
            CFDILabel.Text = cfdi;
            TotalLabel.Text = $"$ {ViewModels.FacturacionViewModel.Instance.TicketEscaneado.MontoCompra}";
            SucursalLabel.Text = ViewModels.FacturacionViewModel.Instance.TicketEscaneado.Sucursal;
        }
        #endregion

        #region View Did Appear
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModels.FacturacionViewModel.Instance.OnSolicitarFacturaFinished += Instance_OnSolicitarFacturaFinished;
        }


        #endregion

        #region View Will Appear
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        #endregion

        #region View Did Disappear
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion

        #endregion

        #region METODOS INTERNOS
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            if (segue.Identifier == "DETALLE_RECEPTOR_SEGUE")
            {
                var controller = segue.DestinationViewController as InformacionReceptorViewController;

                controller.id = id;
                controller.razonsocial = RazonSocialLabel.Text;
                controller.rfc = RFCLabel.Text;
                controller.correo = EmailLabel.Text;
                controller.codigopostal = CodigoPostalLabel.Text;
                controller.direccion = DireccionLabel.Text;
                controller.cfdi = CFDILabel.Text;
            }
        }
        #endregion

        #region EVENTOS
        partial void EditarButton_TouchUpInside(UIButton sender)
        {
            PerformSegue("DETALLE_RECEPTOR_SEGUE", this);
        }

        partial void ConfirmarButton_TouchUpInside(UIButton sender)
        {

            ReceptorFactura Datos = new ReceptorFactura();
            Datos.Id = id;
            Datos.RazonSocial = RazonSocialLabel.Text;
            Datos.Email = EmailLabel.Text;
            Datos.Rfc = RFCLabel.Text;
            Datos.CodigoPostal = CodigoPostalLabel.Text;
            Datos.Direccion = DireccionLabel.Text;
            Datos.UsoCFDI = CFDILabel.Text;

            ViewModels.FacturacionViewModel.Instance.SolicitarFactura(Datos);
        }
        private void Instance_OnSolicitarFacturaFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            if (e.Success)
            {
                var Alert = UIAlertController.Create("Nueva factura", e.Message, UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, ((OK) =>
                {
                    PerformSegue("MIS_FACTURAS_SEGUE", this);
                })));
                PresentViewController(Alert, true, null);

            }
            else
            {
                var Alert = UIAlertController.Create("Nueva factura", e.Message, UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(Alert, true, null);
            }
        }
        #endregion


    }
}