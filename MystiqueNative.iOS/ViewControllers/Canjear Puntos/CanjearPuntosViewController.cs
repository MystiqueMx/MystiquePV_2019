using CoreGraphics;
using Foundation;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class CanjearPuntosViewController : UIViewController
    {

        public UIActivityIndicatorView ActivityIndicator { get { return activityindicator; } }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            #region Activity Indicator

            ActivityIndicator.Hidden = true;
            activityindicator = new UIActivityIndicatorView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width, UIScreen.MainScreen.Bounds.Size.Height));
            activityindicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
            activityindicator.TintColor = UIColor.FromRGB(20, 20, 20);
            activityindicator.Color = UIColor.White;
            activityindicator.BackgroundColor = UIColor.FromRGBA(20, 20, 20, 150);
            activityindicator.Center = View.Center;
            // activityindicator.Center = new CGPoint(View.Center.X, View.Center.Y + 20);
            View.AddSubview(activityindicator);
            activityindicator.Hidden = true;

            #endregion

            TableView.Source = new RecompensasAdapter(AppDelegate.CityPoints, this);
            AutomaticallyAdjustsScrollViewInsets = false;
            TableView.SectionHeaderHeight = 0;
            AppDelegate.CityPoints.PropertyChanged += CityPoints_PropertyChanged;

        }

        private void CityPoints_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "CanjearStatus")
                {
                    if (AppDelegate.CityPoints.CanjearStatus)
                    {
                        //PuntosLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                        {

                        }
                    }
                }

                if (e.PropertyName == "ErrorStatus")
                {

                    if (!AppDelegate.CityPoints.ErrorStatus)
                    {
                        if (AppDelegate.CityPoints.EstadoCuenta != null)
                        {
                            if (!string.IsNullOrEmpty(AppDelegate.CityPoints.EstadoCuenta.Puntos))
                            {
                                PuntosLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                            }
                            else
                            {
                                PuntosLabel.Text = "0";
                            }
                        }
                    }
                    else
                    {
                        var okAlertController = UIAlertController.Create("Mi Saldo", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        PresentViewController(okAlertController, true, null);
                    }
                }

                if (e.PropertyName == "EliminarStatus")
                {

                    if (!AppDelegate.CityPoints.EliminarStatus)
                    {
                        if (!string.IsNullOrEmpty(AppDelegate.CityPoints.EstadoCuenta.Puntos))
                        {
                            //  PuntosLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
                        }
                        else
                        {
                            PuntosLabel.Text = "0";
                        }

                    }
                    else
                    {
                        var okAlertController = UIAlertController.Create("Mi Saldo", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        PresentViewController(okAlertController, true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            /*
            if (e.PropertyName == "ErrorStatus")

            { if(AppDelegate.CityPoints.EstadoCuenta != null)
                {

                    PuntosLabel.Text = AppDelegate.CityPoints.EstadoCuenta.Puntos;
                }
                
            }
            if(e.PropertyName == "CanjearStatus")
            {
                AppDelegate.CityPoints.ObtenerEstadoCuenta();
            } else 
                    if(!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    { 
                    var okAlertController = UIAlertController.Create("Mi Saldo", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(okAlertController, true, null);
                AppDelegate.CityPoints.ErrorMessage = string.Empty;
                    }
            */
        }

        private void Recompensas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            TableView.ReloadData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AppDelegate.CityPoints.ObtenerEstadoCuenta();
            if (AppDelegate.CityPoints.Recompensas.Count == 0)
            {
                AppDelegate.CityPoints.ObtenerRecompensas();
            }
            if (AppDelegate.CityPoints.EstadoCuenta != null)
            {
                //  PuntosLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
            }
            else
            {
                PuntosLabel.Text = "0";
            }

            AppDelegate.CityPoints.Recompensas.CollectionChanged += Recompensas_CollectionChanged;

            if (AppDelegate.CityPoints.EstadoCuenta == null)
            {
                AppDelegate.CityPoints.ObtenerEstadoCuenta();
            }
            if (AppDelegate.CityPoints.Recompensas.Count == 0)
            {
                AppDelegate.CityPoints.ObtenerRecompensas();
            }
            if (AppDelegate.CityPoints.EstadoCuenta != null)
            {
                //  PuntosLabel.Text = AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString();
            }
            else
            {
                PuntosLabel.Text = "0";
            }



            // AppDelegate.CityPoints.PropertyChanged += CityPoints_PropertyChanged;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
        }
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {

            if (segue.Identifier == "DETALLERECOMPENSA_SEGUE")
            {

                var controller = segue.DestinationViewController as DetalleCanjearVC;
                var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);
                var item = AppDelegate.CityPoints.Recompensas[indexPath.Row];

                // AppDelegate.CityPoints.Recompensas = new System.Collections.ObjectModel.ObservableCollection<Models.Recompensa>();

                controller.RecompensaSeleccionada = item;
            }
        }

        public override void PrepareForInterfaceBuilder()
        {
            base.PrepareForInterfaceBuilder();

        }
        public CanjearPuntosViewController(IntPtr handle) : base(handle)
        {
        }

        public void MostrarCodigo()
        {
            //ModalRecompensa Modal = Storyboard.InstantiateViewController("ModalRecompensaID") as ModalRecompensa;
            //Modal.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;

            //Modal.codigoQR =
            //AppDelegate.CityPoints.CodigoCanje.CodigoQR;
            //NavigationController.PresentModalViewController(Modal, true);
        }
    }


    internal class RecompensasAdapter : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("RECOMPENSAS_CELL");
        private CitypointsViewModel viewmodel;
        [Weak]
        CanjearPuntosViewController ViewController;

        public RecompensasAdapter(CitypointsViewModel viewmodel, CanjearPuntosViewController ViewController)
        {
            this.viewmodel = viewmodel;
            this.ViewController = ViewController;
        }
        //public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        //{
        //    return 0.0f;
        //}
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            RecompensasCell cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as RecompensasCell;
            var recompensa = viewmodel.Recompensas[indexPath.Row];

            cell.LabelNombre = recompensa.Nombre;
            cell.LabelPuntos = recompensa.Costo + " pts" + "  ";
            cell.Image = recompensa.ImgRecompensa;
            cell.tag = indexPath.Row;
            cell.Id = recompensa.Id;
            cell.RecompensaSeleccionada = recompensa;
            cell.ViewController = ViewController;
            return cell;

        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var recompensa = viewmodel.Recompensas[indexPath.Row];
            tableView.DeselectRow(indexPath, true);
        }

        public override nint RowsInSection(UITableView tableview, nint section) => viewmodel.Recompensas.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }
}