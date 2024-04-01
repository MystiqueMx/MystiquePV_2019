using Foundation;
using MystiqueNative.Models;
using System;
using System.Collections.ObjectModel;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class FacturarConsumoViewController : UIViewController
    {

        #region DECLARACIONES

        #endregion

        #region CONSTRUCTOR
        public FacturarConsumoViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var ConsumoAdapter = new FacturarConsumoAdapter(this, ViewModels.FacturacionViewModel.Instance.ReceptoresGuardados);
            TableView.Source = ConsumoAdapter;

            AgregarFacturarView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                PerformSegue("NUEVO_RECEPTOR_SEGUE", this);
            }));
            
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
            TableViewHeight.Constant = (ViewModels.FacturacionViewModel.Instance.ReceptoresGuardados.Count * 90);
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
                var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);

                var item = ViewModels.FacturacionViewModel.Instance.ReceptoresGuardados[indexPath.Row];

                controller.id = item.Id;
                //  controller.indexpath = int.Parse(indexPath.ToString());
                controller.razonsocial = item.RazonSocial;
                controller.rfc = item.Rfc;
                controller.correo = item.Email;
                controller.codigopostal = item.CodigoPostal;
                controller.direccion = item.Direccion;
                controller.cfdi = item.UsoCFDI;
            }
            if (segue.Identifier == "CONFIRMAR_DATOS_SEGUE")
            {
                var controller = segue.DestinationViewController as ConfirmarDatosViewController;
                var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);

                var item = ViewModels.FacturacionViewModel.Instance.ReceptoresGuardados[indexPath.Row];

                controller.id = item.Id;
                controller.razonsocial = item.RazonSocial;
                controller.rfc = item.Rfc;
                controller.correo = item.Email;
                controller.codigopostal = item.CodigoPostal;
                controller.direccion = item.Direccion;
                controller.cfdi = item.UsoCFDI;
            }
        }

        #endregion

        #region EVENTOS

        #endregion

    }

    internal class FacturarConsumoAdapter : UITableViewSource
    {
        private FacturarConsumoViewController facturarConsumoViewController;
        private ObservableCollection<ReceptorFactura> receptoresGuardados;

        public FacturarConsumoAdapter(FacturarConsumoViewController facturarConsumoViewController, ObservableCollection<ReceptorFactura> receptoresGuardados)
        {
            this.facturarConsumoViewController = facturarConsumoViewController;
            this.receptoresGuardados = receptoresGuardados;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            FacturaConsumoCell cell = tableView.DequeueReusableCell("FACTURASCONSUMO_CELL", indexPath) as FacturaConsumoCell;
            var ReceptoresGuardados = receptoresGuardados[indexPath.Row];

            cell.tag = indexPath.Row;
            cell.FacturaSelected = facturarConsumoViewController;
            cell.Nombre = ReceptoresGuardados.Rfc;
            cell.Correo = ReceptoresGuardados.Email;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => receptoresGuardados.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }
}