using Foundation;
using System;
using UIKit;
using MystiqueNative.ViewModels;
using MystiqueNative.Models;
using System.Collections.ObjectModel;
using CoreGraphics;

namespace MystiqueNative.iOS
{
    public partial class MisFacturasViewController : UITableViewController
    {

        #region DECLARACIONES
        UILabel label;
        UILabel Cargando;
        UIActivityIndicatorView activityindicator;
        #endregion

        #region CONSTRUCTOR
        public MisFacturasViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            #region Set empty label
            label = new UILabel(new CGRect(0, 0, TableView.Bounds.Size.Width, TableView.Bounds.Size.Height));
            label.Text = "No tienes facturas";
            label.TextColor = UIColor.Black.ColorWithAlpha(0.60f);
            label.TextAlignment = UITextAlignment.Center;
            label.AdjustsFontSizeToFitWidth = true;
            label.Font.WithSize(12);
            TableView.BackgroundView = label;
            label.Hidden = true;
            #endregion

            #region Activity Indicator

            //activityindicator = new UIActivityIndicatorView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width, UIScreen.MainScreen.Bounds.Size.Height));
            //activityindicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
            //activityindicator.TintColor = UIColor.Black.ColorWithAlpha(0.60f);
            //activityindicator.Color = UIColor.White;
            //activityindicator.BackgroundColor = UIColor.FromRGBA(20, 20, 20, 0);
            //activityindicator.Center = this.View.Center;
            //View.AddSubview(activityindicator);
            //activityindicator.Hidden = true;

            #endregion
            #region Loading Label

            //Cargando = new UILabel(new CGRect(0, -20, TableView.Bounds.Size.Width, TableView.Bounds.Size.Height));
            //Cargando.Hidden = false;
            //Cargando.Text = "Cargando...";
            //Cargando.TextColor = UIColor.Black.ColorWithAlpha(0.60f);
            //Cargando.TextAlignment = UITextAlignment.Center;
            //Cargando.AdjustsFontSizeToFitWidth = true;
            //Cargando.Font.WithSize(12);
            //View.AddSubview(Cargando);
            //Cargando.Hidden = true;
            #endregion
            TableView.Source = new MisFacturasAdapter(this, FacturacionViewModel.Instance.Facturas);

        }
        #endregion

        #region View Did Appear
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
           // activityindicator.Hidden = false;
          //  Cargando.Hidden = false;
            FacturacionViewModel.Instance.OnObtenerFacturasFinished += Instance_OnObtenerFacturasFinished;
            FacturacionViewModel.Instance.ObtenerFacturas();
        }

        private void Instance_OnObtenerFacturasFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            //activityindicator.Hidden = true;
           // Cargando.Hidden = true;
          //  label.Hidden = FacturacionViewModel.Instance.Facturas.Count > 0 ? true : false;
            TableView.ReloadData();

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
            //activityindicator.Hidden = true;
           // Cargando.Hidden = true;
            FacturacionViewModel.Instance.OnObtenerFacturasFinished -= Instance_OnObtenerFacturasFinished;
        }


        #endregion

        #endregion

        #region METODOS INTERNOS
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            if (segue.Identifier == "DETALLE_FACTURA_SEGUE")
            {
                var controller = segue.DestinationViewController as DetalleFacturaController;
                var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);

                var item = ViewModels.FacturacionViewModel.Instance.Facturas[indexPath.Row];
                
                controller.id = item.Id;
                controller.folio = item.Folio;
                controller.sucursal = item.Sucursal;
                controller.fecha = item.FechaCompraConFormatoEspanyol;
                controller.fecharegistro = item.FechaRegistro;
                controller.estatus = item.Estatus;
                controller.montocompra = item.MontoCompraConFormatoMoneda;

            }
        }
        #endregion

        #region EVENTOS

        #endregion
    }

    internal class MisFacturasAdapter : UITableViewSource
    {
        private readonly MisFacturasViewController misFacturasViewController;
        private readonly ObservableCollection<Factura> facturas;

        public MisFacturasAdapter(MisFacturasViewController misFacturasViewController, ObservableCollection<Factura> facturas)
        {
            this.misFacturasViewController = misFacturasViewController;
            this.facturas = facturas;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MisFacturasCell cell = tableView.DequeueReusableCell("MISFACTURAS_CELL", indexPath) as MisFacturasCell;
            var Factura = facturas[indexPath.Row];

            cell.Title = $"{Factura.Sucursal} - {Factura.Estatus}";
            cell.Id = $"Folio: {Factura.Folio}";
            cell.Fecha = $"Fecha: {Factura.FechaCompraConFormatoEspanyol}";
            cell.Total = $"Total: {Factura.MontoCompraConFormatoMoneda}";
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => facturas.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;


    }
}