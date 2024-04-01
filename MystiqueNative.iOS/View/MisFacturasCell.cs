using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MisFacturasCell : UITableViewCell
    {
        public MisFacturasCell(IntPtr handle) : base(handle)
        {
            
        }
        #region Declaraciones

        private string title;
        private string id;
        private string fecha;
        private string total;
        #endregion
        public string Title { get => title; set { title = value; TitleLabel.Text = value; } }
        public string Id { get => id; set { id = value; FolioLabel.Text = value; } }
        public string Fecha { get => fecha; set { fecha = value; FechaLabel.Text = value; } }
        public string Total { get => total; set { total = value; TotalLabel.Text = value; } }
    }
}