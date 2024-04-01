using Foundation;
using System;
using System.Threading.Tasks;
using UIKit;
using ZXing.Mobile;


namespace MystiqueNative.iOS
{
    public partial class ScanPointsViewController : UIViewController
    {
        string resultado;
        string QRresultado = null;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

        }

        public override void ViewDidAppear(bool animated)
        {

        }

        //public async Task<string> ScanAsync()
        //{
        //    //var scanner = new ZXing.Mobile.MobileBarcodeScanner();
        //    //var result = await scanner.Scan();

        //    //resultado = ("Scanned Barcode: " + result.Text);
        //    //Console.WriteLine(resultado);
        //    //return resultado;

        //}

        public ScanPointsViewController (IntPtr handle) : base (handle)
        {
        }
    }
}