using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class SucursalesMap : UIViewController
    {
       
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);

            BuscarInput.Layer.BorderWidth = 0.5f;
            BuscarInput.Layer.BorderColor = UIColor.FromRGBA(red: 0.78f, green: 0.78f, blue: 0.78f, alpha: 1.0f).CGColor;

            this.BuscarInput.ShouldReturn += (textField) => {

                BuscarInput.ResignFirstResponder();

                return true;
            };


        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //this.NavigationController.Title = "City Salads";
        }


        public SucursalesMap (IntPtr handle) : base (handle)
        {
        }
    }
}