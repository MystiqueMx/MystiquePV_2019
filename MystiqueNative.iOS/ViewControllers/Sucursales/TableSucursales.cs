using Foundation;
using System;
using UIKit;

using MystiqueNative.ViewModels;

namespace MystiqueNative.iOS
{
    public partial class TableSucursales : UITableViewController
    {
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

       
            TableView.Source = new SucursalesAdapter(AppDelegate.Beneficios);
            if (string.IsNullOrEmpty(MystiqueApp.Comercio))
            {
                AppDelegate.Beneficios.ObtenerSucursalesPorIdComercio("4");
            }
                else { AppDelegate.Beneficios.ObtenerSucursalesPorIdComercio(MystiqueApp.Comercio);  }
            
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            AppDelegate.Beneficios.Sucursales.CollectionChanged += Sucursales_CollectionChanged;
            if (string.IsNullOrEmpty(MystiqueApp.Comercio))
            {
                AppDelegate.Beneficios.ObtenerSucursalesPorIdComercio("4");
            }
            else { AppDelegate.Beneficios.ObtenerSucursalesPorIdComercio(MystiqueApp.Comercio); }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.Beneficios.Sucursales.CollectionChanged -= Sucursales_CollectionChanged;
        }


        private void Sucursales_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TableView.ReloadData();
        }

        public TableSucursales (IntPtr handle) : base (handle)
        {
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "SegueDetalleSucursal")
            {

                var controller = segue.DestinationViewController as BeneficiosTableViewController;
                var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);
                var item = AppDelegate.Beneficios.Sucursales[indexPath.Row];

                AppDelegate.Beneficios.Beneficios = new System.Collections.ObjectModel.ObservableCollection<Models.BeneficiosSucursal>();
                
              //  controller.SucursalSeleccionada = item;
            }
        }
    }

    public class SucursalesAdapter : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("SUCURSAL_CELL");
        [Weak]
        private BeneficiosViewModel viewmodel;

        public SucursalesAdapter(BeneficiosViewModel viewmodel)
        {
            this.viewmodel = viewmodel;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SucursalesCell cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as SucursalesCell;
            var sucursal = viewmodel.Sucursales[indexPath.Row];
           
            cell.Label = sucursal.Nombre;
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => viewmodel.Sucursales.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }
}