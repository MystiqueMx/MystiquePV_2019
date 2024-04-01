using Foundation;
using MystiqueNative.ViewModels;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MenuPrincipalViewController : UIViewController
    {
        #region CONSTRUCTOR
        public MenuPrincipalViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region DECLARACIONES

        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            #region SET SEGUES MENU
            CapturarPuntosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (AppDelegate.Auth.Usuario.RegistroCompleto)
                {
                    PerformSegue("SUMAR_PUNTOS_SEGUE", this);
                }
                else
                {
                    var okAlertController = UIAlertController.Create("City Points", "Complete su registro para poder continuar", UIAlertControllerStyle.Alert);
                    var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                    var PerfilAction = UIAlertAction.Create("Completar Registro", UIAlertActionStyle.Default, (OK) =>
                    {
                        PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                    });
                    okAlertController.AddAction(okAction);
                    okAlertController.AddAction(PerfilAction);
                    okAlertController.PreferredAction = PerfilAction;
                    PresentViewController(okAlertController, true, null);
                }

            }));

            CanjearPuntosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                PerformSegue("CANJEAR_SEGUE", this);
            }));
            PromosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                PerformSegue("PROMOS_SEGUE", this);
            }));
            FacturacionView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                PerformSegue("FACTURA_SEGUE", this);
            }));

            #endregion


        }
        #endregion

        #region View Did Appear
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            AppDelegate.CityPoints.ObtenerEstadoCuenta();
            AppDelegate.CityPoints.OnEstadoCuentaFinished += CityPoints_OnEstadoCuentaFinished;
        }

        private void CityPoints_OnEstadoCuentaFinished(object sender, EstadoCuentaArgs e)
        {
            if (e.EstadoCuenta.Success)
            {
                MiSaldo.Text = e.EstadoCuenta.PuntosAsInt.ToString() + " pts";
            }
            else
            {
                AppDelegate.CityPoints.ObtenerEstadoCuenta();
            }
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
            AppDelegate.CityPoints.OnEstadoCuentaFinished -= CityPoints_OnEstadoCuentaFinished;
        }
        #endregion

        #endregion

        #region METODOS INTERNOS



        #endregion

        #region EVENTOS

        #endregion
    }
}