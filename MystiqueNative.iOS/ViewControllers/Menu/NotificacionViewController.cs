using CoreGraphics;
using Foundation;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class NotificacionViewController : UITableViewController
    {
        private NSTimer timer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.Source = new NotificacionesAdapter(AppDelegate.ObtenerNotificaciones);
       

            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            var label = new UILabel(new CGRect(0, 0, TableView.Bounds.Size.Width, TableView.Bounds.Size.Height));
            nfloat centerX = View.Frame.Width / 2;
            nfloat centerY = View.Frame.Height / 2;
            #region Label empty table
            this.timer = NSTimer.CreateRepeatingScheduledTimer(0.5, (_) =>
            {

                if (TableView.VisibleCells.Length == 0)
                {
                    label.Hidden = false;
                    label.Text = "No tienes notificaciones";
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
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AppDelegate.ObtenerNotificaciones.LimpiarNotificaciones();
            //ParentViewController.TabBarItem.BadgeValue = "4";
            //this.TabBarItem.BadgeValue = "5";
        }
        private void Notificaciones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TableView.ReloadData();
            if (AppDelegate.ObtenerNotificaciones.NotificacionesNuevas > 0)
            {
                //this.NavigationController.TabBarItem.BadgeValue = AppDelegate.ObtenerNotificaciones.NotificacionesNuevas.ToString();
               // this.NavigationController.TabBarItem.BadgeValue = "3";
            }
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AppDelegate.ObtenerNotificaciones.Notificaciones.CollectionChanged += Notificaciones_CollectionChanged;
            AppDelegate.ObtenerNotificaciones.ObtenerNotificaciones();
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.ObtenerNotificaciones.Notificaciones.CollectionChanged -= Notificaciones_CollectionChanged;
        }
        public NotificacionViewController(IntPtr handle) : base(handle)
        {
        }
    }

    public class NotificacionesAdapter : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("NOTIFICATION_CELL");
        private NotificacionesViewModel viewmodel;
        public NotificacionesAdapter(NotificacionesViewModel viewmodel)
        {
            this.viewmodel = viewmodel;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            CellNotificaciones cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as CellNotificaciones;
            var noti = viewmodel.Notificaciones[indexPath.Row];

            cell.Label = noti.Contenido;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => viewmodel.Notificaciones.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    
    }
}