using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class FacturaConsumoCell : UITableViewCell
    {
        public FacturaConsumoCell(IntPtr handle) : base(handle)
        {
            //   AddGestureRecognizer(longPressGesture);
        }
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            this.FacturarConsumoView.Tag = tag;
            var longPressGesture = new UILongPressGestureRecognizer(LongPressMethod);
            var TapGesture = new UITapGestureRecognizer(TapPressMethod);
          
            FacturarConsumoView.AddGestureRecognizer(longPressGesture);
            FacturarConsumoView.AddGestureRecognizer(TapGesture);

        }

        private void TapPressMethod(UITapGestureRecognizer gestureRecognizer)
        {
            //UIView.Animate(3, () =>
            //{
            //    FacturarConsumoView.BackgroundColor = UIColor.FromRGB(100, 100, 100);
            //});
            if (gestureRecognizer.State == UIGestureRecognizerState.Ended)
            {
                FacturaSelected.PerformSegue("CONFIRMAR_DATOS_SEGUE", this);
            }
        }

        public nint tag;
        public string Nombre { get; internal set; }
        public string Correo { get; internal set; }
        public string nombre { get => Nombre; set { Nombre = value; NombreLabel.Text = value; } }
        public string correo { get => Correo; set { Correo = value; CorreoLabel.Text = value; } }

        public FacturarConsumoViewController FacturaSelected { get; internal set; }

        void LongPressMethod(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State == UIGestureRecognizerState.Began)
            {
                Console.Write("LongPress");
                var Alert = UIAlertController.Create("Factura Consumo", null, UIAlertControllerStyle.ActionSheet);
                Alert.AddAction(UIAlertAction.Create("Editar antes de solicitar", UIAlertActionStyle.Default, ((Edit) =>
                {
                    FacturaSelected.PerformSegue("DETALLE_RECEPTOR_SEGUE", this);
                })));
                Alert.AddAction(UIAlertAction.Create("Eliminar", UIAlertActionStyle.Destructive, null));
                Alert.AddAction(UIAlertAction.Create("Cancelar", UIAlertActionStyle.Cancel, null));
                FacturaSelected.PresentViewController(Alert, true, null);
            }

        }
    }
}