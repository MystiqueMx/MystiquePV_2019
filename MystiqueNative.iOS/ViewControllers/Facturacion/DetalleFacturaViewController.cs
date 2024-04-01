using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class DetalleFacturaViewController : UIViewController
    {

        #region DECLARACIONES
        internal string id;
        internal string folio;
        internal string sucursal;
        internal string fecha;
        internal string fecharegistro;
        internal string estatus;
        internal string montocompra;
        #endregion

        #region CONSTRUCTOR
        public DetalleFacturaViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //FolioLabel.Text = folio;
            //SucursalLabel.Text = sucursal;
            //FechaLabel.Text = fecha;
            //EstatusLabel.Text = estatus;
            //TotalLabel.Text = montocompra;
        }
        #endregion

        #region View Did Appear
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
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

        #endregion

        #region EVENTOS
      
        #endregion

    }
}