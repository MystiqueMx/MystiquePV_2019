using Foundation;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;
using System;
using System.Collections.ObjectModel;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class PuntosCanjeadosController : UIViewController
    {
        public PuntosCanjeadosController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TotalLabel.Text = "Total  " + string.Format("{0:#,##0.##}", HistorialViewModel.Instance.Canjeados)+" pts";
            TableView.Source = new PuntosCanjeadosAdapter(HistorialViewModel.Instance.MovimientosCanjeados);
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (HistorialViewModel.Instance.MovimientosCanjeados.Count == 0)
            {
                SinCanjeLabel.Hidden = false;
            }
            else
            {
                SinCanjeLabel.Hidden = true;
            }
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

        }

    }

    internal class PuntosCanjeadosAdapter : UITableViewSource
    {
        private ObservableCollection<MovimientoCitypoints> movimientosCanjeados;

        public PuntosCanjeadosAdapter(ObservableCollection<MovimientoCitypoints> movimientosCanjeados)
        {
            this.movimientosCanjeados = movimientosCanjeados;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            PuntosCanjeadosCell cell = tableView.DequeueReusableCell("PUNTOSCANJEADOS_CELL", indexPath) as PuntosCanjeadosCell;
            var historialcanjeado = movimientosCanjeados[indexPath.Row];
            //  cell.ticket = historialsumado.;
            cell.Fecha = historialcanjeado.FechaRegistroConFormatoEspanyol;
            // cell.monto = historialsumado.monto;
            cell.Producto = historialcanjeado.Producto;
            cell.Puntos = historialcanjeado.PuntosAsInt.ToString("#,##0") +" pts";
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section) => movimientosCanjeados.Count;
        public override nint NumberOfSections(UITableView tableView) => 1;
    }
}