using CoreGraphics;
using Foundation;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class BeneficiosTableViewController : UIViewController
    {
       // public Sucursal SucursalSeleccionada { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           // FavoritosButton.Hidden = true;
            //FavoritosButton.SemanticContentAttribute = UISemanticContentAttribute.ForceRightToLeft;
          //  FavoritosButton.ImageEdgeInsets =
          //      new UIEdgeInsets(top: 5, left: (FavoritosButton.Bounds.Width - 35), bottom: 5, right: -15);
         //   FavoritosButton.TitleEdgeInsets =
             //   new UIEdgeInsets(top: 0, left: -20, bottom: 0, right: (FavoritosButton.ImageView.Frame.Width));

            TableView.Source = new BeneficiosAdapter(AppDelegate.Beneficios,this);
           // BeneficiosAdapter.IdSucursal = SucursalSeleccionada.Id;
          //  title.Title = SucursalSeleccionada.Nombre;
         //   NavigationController.NavigationItem.Title = SucursalSeleccionada.Nombre;



        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //ANIMACION LOADING...
            UIActivityIndicatorView CargandoBeneficios;
            CargandoBeneficios = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            CargandoBeneficios.Frame = new CGRect(150, 150, 40, 40);
            CargandoBeneficios.Center = View.Center;
            View.AddSubview(CargandoBeneficios);
            CargandoBeneficios.BringSubviewToFront(View);
            //DEFAULT:
            var transform = CGAffineTransform.MakeScale(1.5f, 1.5f);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                transform = CGAffineTransform.MakeScale(3f, 3f);
            }

            CargandoBeneficios.Transform = transform;
            CargandoBeneficios.Color = UIColor.FromRGB(39, 170, 225);
            //
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(6,181,158);
            AppDelegate.Beneficios.Beneficios.CollectionChanged += Beneficios_CollectionChanged;
            if (AppDelegate.Beneficios.Beneficios.Count == 0)
            {
               // CargandoBeneficios.StartAnimating();
                //AppDelegate.Beneficios.ObtenerBeneficioPorIDSucursal(SucursalSeleccionada.Id);
                AppDelegate.Beneficios.ObtenerTodosLosBeneficios();
               // CargandoBeneficios.StopAnimating();
            }

            NavigationController.NavigationItem.Title = "Promociones y descuentos";
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            AppDelegate.Beneficios.Beneficios.CollectionChanged -= Beneficios_CollectionChanged;
        }

   
        //public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        //{

        //    if (segue.Identifier == "DetalleBeneficioSegue")
        //    {
        //        var controller = segue.DestinationViewController as DetalleBeneficiosViewController;
        //        var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);

        //        var item = AppDelegate.Beneficios.Beneficios[indexPath.Row];

        //        controller.SucursalID = this.SucursalSeleccionada.Id;
        //        controller.Sucursal = this.SucursalSeleccionada.Nombre;
        //        controller.Beneficios = item;
        //    }
        //}

        private void Beneficios_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TableView.ReloadData();
        }

        public BeneficiosTableViewController(IntPtr handle) : base(handle)
        {
        }
    }

    public class BeneficiosAdapter : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("BENEFICIO_CELL");
        private BeneficiosViewModel viewmodel;
        [Weak]
        BeneficiosTableViewController ViewController;
        public static string IdSucursal { get; set; }

        public BeneficiosAdapter(BeneficiosViewModel viewmodel, BeneficiosTableViewController ViewController)
        {
            this.viewmodel = viewmodel;
            this.ViewController = ViewController;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            BeneficioCell cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as BeneficioCell;
            var beneficio = viewmodel.Beneficios[indexPath.Row];
            
            cell.Label = beneficio.Descripcion;
            cell.Descripcion = beneficio.Descripcion;
            cell.Image = beneficio.ImgBeneficio;
            cell.tag = indexPath.Row;
            cell.ImagenSolicitar = beneficio.CadenaCodigo;
            cell.IDBeneficio = beneficio.IdBeneficio;
            cell.ViewController = ViewController;
            
            //cell.IdSucursal = IdSucursal;
            

            return cell;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (!AppDelegate.Auth.Usuario.RegistroCompleto)
            {
                var okAlertController = UIAlertController.Create("Registro", "Completa tu registro para continuar", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (Oks) =>
                {
                    ViewController.PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                    // controller.PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                }));
                ViewController.PresentViewController(okAlertController, true, null);
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section) => viewmodel.Beneficios.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }
}