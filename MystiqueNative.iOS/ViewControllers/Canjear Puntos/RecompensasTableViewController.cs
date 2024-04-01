using CoreGraphics;
using FFImageLoading;
using Foundation;
using MystiqueNative.iOS.View;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class RecompensasTableViewController : UIViewController
    {
        private NSTimer timer;
        LoadingOverlay loadPop;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds, "Cargando...");
            TableView.Source = new ListaRecompensasAdapter(AppDelegate.CityPoints,this);
            //this.AutomaticallyAdjustsScrollViewInsets = false;
            AppDelegate.CityPoints.ObtenerRecompensasActivas();

            AppDelegate.CityPoints.RecompensasActivas.CollectionChanged += RecompensasActivas_CollectionChanged;

            var label = new UILabel(new CGRect(0, 0, TableView.Bounds.Size.Width, TableView.Bounds.Size.Height));
            nfloat centerX = View.Frame.Width / 2;
            nfloat centerY = View.Frame.Height / 2;
            
            #region Label empty table
            this.timer = NSTimer.CreateRepeatingScheduledTimer(0.5, (_) =>
            {

                if (TableView.VisibleCells.Length == 0)
                {
                    label.Hidden = false;
                    label.Lines = 2;
                    label.Text = "No cuentas con recompensas por \ncanjear";
                    label.TextColor = UIColor.Black.ColorWithAlpha(0.60f);

                    label.TextAlignment = UITextAlignment.Center;
                    label.AdjustsFontSizeToFitWidth = true;
                    label.Font.WithSize(12);

                }
                else
                {
                    label.Text = "";
                }

                TableView.BackgroundView = label;
            });
            #endregion

        }

        private void RecompensasActivas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TableView.ReloadData();
           
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Puntos.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
          
                if (AppDelegate.CityPoints.EstadoCuenta != null)
                {
                    if (string.IsNullOrEmpty(AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString()))
                    {
                        var alert = UIAlertController.Create("Recompensas", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        var OKAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (ok) =>
                        {
                            this.NavigationController.PopViewController(true);
                        });
                        alert.AddAction(OKAction);
                        alert.PreferredAction = OKAction;
                        PresentViewController(alert, true, null);
                        AppDelegate.CityPoints.ErrorMessage = string.Empty;
                    }
                    else
                    {
                        Puntos.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                    }
                }

                AppDelegate.CityPoints.PropertyChanged += CityPoints_PropertyChanged;

                AppDelegate.CityPoints.ObtenerMovimientosCitypoints();
                AppDelegate.CityPoints.ObtenerEstadoCuenta();

                if (AppDelegate.CityPoints.EstadoCuenta != null)
                {
                    Puntos.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                }

        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.CityPoints.ObtenerRecompensasActivas();
            AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
        }
        public RecompensasTableViewController (IntPtr handle) : base (handle)
        {
        }

        private void CityPoints_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanjearStatus")
            {
                if (AppDelegate.CityPoints.CanjearStatus)
                {
                    Puntos.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    {
                        var okAlertController = UIAlertController.Create("Canjear Puntos", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        PresentViewController(okAlertController, true, null);
                        AppDelegate.CityPoints.ErrorMessage = string.Empty;
                    }
                }
            }

            if (e.PropertyName == "EliminarStatus")
            {
                
                if (AppDelegate.CityPoints.EliminarStatus)
                {
                    AppDelegate.CityPoints.ObtenerRecompensasActivas();
                    AppDelegate.CityPoints.ObtenerEstadoCuenta();
                }
                else
                {
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    {
                        AppDelegate.CityPoints.ObtenerRecompensasActivas();
                        var okAlertController = UIAlertController.Create("Canjear Puntos", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        PresentViewController(okAlertController, true, null);
                        AppDelegate.CityPoints.ErrorMessage = string.Empty;
                    }
                }
            }
            if (e.PropertyName == "ErrorStatus")
            {

                if (AppDelegate.CityPoints.EstadoCuenta != null)
                {

                    Puntos.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                }

            }
        }
    }

    internal class ListaRecompensasAdapter : UITableViewSource
    {
        private CitypointsViewModel cityPoints;
        static readonly NSString CELL_IDENTIFIER = new NSString("LISTARECOMPENSAS_CELL");
        [Weak]
        RecompensasTableViewController ViewController;
        public ListaRecompensasAdapter(CitypointsViewModel cityPoints, RecompensasTableViewController ViewController)
        {
            this.cityPoints = cityPoints;
            this.ViewController = ViewController;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ListaRecompensasCell cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as ListaRecompensasCell;
            var recompensa = cityPoints.RecompensasActivas[indexPath.Row];
            cell.LabelNombre =recompensa.Descripcion;
            cell.LabelPuntos = recompensa.DiasRestantes+" días" + " "+ recompensa.HorasRestantes +  " hrs" ;
            cell.tag = indexPath.Row;
            cell.Image = recompensa.ImgRecompensa;
            cell.RecompensaQR = recompensa.FolioCanje;
            cell.IDCanjePuntos = recompensa.Id;
            cell.ViewController = ViewController;

            return cell;
        }
        public override nint RowsInSection(UITableView tableview, nint section) => cityPoints.RecompensasActivas.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }


}