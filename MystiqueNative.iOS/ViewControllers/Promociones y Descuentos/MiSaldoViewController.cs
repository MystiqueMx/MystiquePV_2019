using Foundation;
using MystiqueNative.iOS;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MiSaldoViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            CanjeadosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
                PerformSegue("CANJEADOS_SEGUE", this);
            }));
            SumadosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
                PerformSegue("SUMADOS_SEGUE", this);
            }));


            //TableView.Source = new PointsHistorialAdapter(AppDelegate.CityPoints);
            //this.AutomaticallyAdjustsScrollViewInsets = false;

        }


        private void Instance_FinishLoadingHistorial(object sender, HistorialViewArgs e)
        {

            if (e.Success)
            {
                //Canjeados.Text = string.Format("{0:n0}", e.Canjeados);
                //Sumados.Text = string.Format("{0:n0}", e.Sumados);
                //PuntosActuales.Text = string.Format("{0:n0}", e.Actuales);

                Canjeados.Text = e.Canjeados;
                Sumados.Text = e.Sumados;
                PuntosActuales.Text = e.Actuales;

            }

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            HistorialViewModel.Instance.ObtenerHistorial();
            HistorialViewModel.Instance.FinishLoadingHistorial += Instance_FinishLoadingHistorial;
            PuntosActuales.Text = AppDelegate.Auth.Usuario.PuntosActualesAsInt.ToString() + " pts";
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //AppDelegate.CityPoints.Movimientos.CollectionChanged += Movimientos_CollectionChanged;

            //if (AppDelegate.CityPoints.EstadoCuenta != null)
            //{
            // CityPointsLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
            //}

            //AppDelegate.CityPoints.PropertyChanged += CityPoints_PropertyChanged;

            //AppDelegate.CityPoints.ObtenerMovimientosCitypoints();
            //AppDelegate.CityPoints.ObtenerEstadoCuenta();

            //if (AppDelegate.CityPoints.EstadoCuenta != null)
            //{
            //    CityPointsLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
            //} 

        }

        private void Movimientos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //TableView.ReloadData();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
        }

        private void CityPoints_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "ErrorStatus")
            //{

            //    if (AppDelegate.CityPoints.EstadoCuenta != null)
            //    {

            //        CityPointsLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
            //    }
            //    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
            //    {
            //        var alert = UIAlertController.Create("Mi Saldo", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
            //        var OKAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (ok)=> {
            //            NavigationController.PopViewController(true);
            //        });
            //        alert.AddAction(OKAction);
            //        alert.PreferredAction = OKAction;
            //        PresentViewController(alert, true, null);
            //        AppDelegate.CityPoints.ErrorMessage = string.Empty;
            //    }

            //}
        }


        public MiSaldoViewController(IntPtr handle) : base(handle)
        {
        }

    }


    internal class PointsHistorialAdapter : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("HISTORIAL_CELL");
        private CitypointsViewModel viewmodel;

        public PointsHistorialAdapter(CitypointsViewModel viewmodel)
        {
            this.viewmodel = viewmodel;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MiSaldoCell cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as MiSaldoCell;
            var historial = viewmodel.Movimientos[indexPath.Row];

            cell.LabelFecha = historial.FechaRegistroConFormatoEspanyol;
            cell.LabelPuntos = historial.Puntos;

            if (historial.IsUp)
            {
                cell.Image = UIImage.FromBundle("up");
            }
            else
            {
                cell.Image = UIImage.FromBundle("chevron-down");
            }
            return cell;

        }
        public override nint RowsInSection(UITableView tableview, nint section) => viewmodel.Movimientos.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }
}
