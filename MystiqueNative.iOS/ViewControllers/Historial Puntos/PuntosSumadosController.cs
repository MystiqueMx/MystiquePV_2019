using Foundation;
using MystiqueNative.Models;
using MystiqueNative.ViewModels;
using System;
using System.Collections.ObjectModel;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class PuntosSumadosController : UIViewController
    {
        public PuntosSumadosController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TotalLabel.Text = "Total  " + string.Format("{0:#,##0.##}", HistorialViewModel.Instance.Sumados)+" pts";
            TableView.Source = new PuntosSumadosAdapter(HistorialViewModel.Instance.MovimientosSumados);
        }

        private class PuntosSumadosAdapter : UITableViewSource
        {
            private ObservableCollection<MovimientoCitypoints> movimientosSumados;

            public PuntosSumadosAdapter(ObservableCollection<MovimientoCitypoints> movimientosSumados)
            {
                this.movimientosSumados = movimientosSumados;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                PuntosSumadosCell cell = tableView.DequeueReusableCell("PUNTOSSUMADOS_CELL", indexPath) as PuntosSumadosCell;
                var historialsumado = movimientosSumados[indexPath.Row];
                //  cell.ticket = historialsumado.;
                cell.Fecha = historialsumado.FechaCompraConFormatoEspanyol;
                cell.MontoCompra = "$"+historialsumado.MontoAsInt.ToString();
                cell.Puntos = historialsumado.PuntosAsInt.ToString("#,##0") +" pts";
                cell.tag = indexPath.Row;
                cell.NoTicket = historialsumado.Folio;
                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section) => movimientosSumados.Count;
            public override nint NumberOfSections(UITableView tableView) => 1;
        }
    }
}