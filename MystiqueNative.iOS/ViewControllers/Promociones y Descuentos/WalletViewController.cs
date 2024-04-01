using CoreGraphics;
using Foundation;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class WalletViewController : UITableViewController
    {
        private NSTimer timer;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.Title = "Favoritos de " + AppDelegate.Auth.Usuario.Nombre;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(238, 39, 56);
            AppDelegate.Wallet.ObtenerBeneficiosWallet();
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            var label = new UILabel(new CGRect(0, 0, TableView.Bounds.Size.Width, TableView.Bounds.Size.Height));
            label.Lines = 2;
            nfloat centerX = View.Frame.Width / 2;
            nfloat centerY = View.Frame.Height / 2;

            TableView.Source = new WalletAdapter(AppDelegate.Wallet);
            AppDelegate.Wallet.BeneficiosWallet.CollectionChanged += Sucursales_CollectionChanged;

            if (string.IsNullOrEmpty(MystiqueApp.Comercio))
            {
                AppDelegate.Beneficios.ObtenerSucursalesPorIdComercio("4");
                AppDelegate.Wallet.ObtenerBeneficiosWallet();
            }
            else { AppDelegate.Beneficios.ObtenerSucursalesPorIdComercio(MystiqueApp.Comercio); }


            #region Label empty table
            this.timer = NSTimer.CreateRepeatingScheduledTimer(0.1, (_) =>
            {
                TableView.ReloadData();
                if (TableView.VisibleCells.Length == 0)
                {
                    label.Hidden = false;
                    label.Text = "No se ha agregado algún beneficio\n a Mis Favoritos";
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

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(238, 39, 56);
            AppDelegate.Wallet.ObtenerBeneficiosWallet();
            TableView.ReloadData();

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            

        }

        private void Sucursales_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           
            TableView.ReloadData();
        }

        public WalletViewController (IntPtr handle) : base (handle)
        {
        }
    }

/// <summary>
/// ////////////////////////////////////ADAPTER ////////////////////////////
/// </summary>
    public class WalletAdapter : UITableViewSource
    {
       

        static readonly NSString CELL_IDENTIFIER = new NSString("WALLET_CELL");
        private WalletViewModel viewmodel;
        private NSTimer timer;
        

        public WalletAdapter(WalletViewModel viewmodel)
        {
            this.viewmodel = viewmodel;
        } 

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            
            WalletBeneficioCell cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as WalletBeneficioCell;
            var Wallet = viewmodel.BeneficiosWallet[indexPath.Row];
            cell.Label = Wallet.Descripcion;


            cell.Label2 = Wallet.DiasRestantes+ " días" +" "+ Wallet.HorasRestantes + " hrs"+ "  ";
            cell.Image = Wallet.ImagenBeneficioUrl;
            if(!string.IsNullOrEmpty(Wallet.IdBeneficio) ){

                cell.IDBeneficio = Wallet.IdBeneficio;
                cell.tag = indexPath.Row;
                cell.ImagenSolicitar = Wallet.CodigoQRString;
                cell.Descripcion = Wallet.Descripcion;

            }
            return cell;


        }
        
        public override nint RowsInSection(UITableView tableview, nint section) => viewmodel.BeneficiosWallet.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }
}