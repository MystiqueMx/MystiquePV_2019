using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ModalTest : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);

            //fonditojeje.Alpha = 0.75f;
            //ModalView.Alpha = 1f;

            // CHECKMARK
            //var CheckBox = new BEMCheckBox(new CoreGraphics.CGRect(0, 0, 30, 30));
            //CheckBox.BoxType = BEMBoxType.Square;
            //CheckBox.OnAnimationType = BEMAnimationType.Fade;
            //CheckBox.OffAnimationType = BEMAnimationType.Fade;

            //CheckBox.OnFillColor  = new UIColor(red: 0.50f, green: 0.74f, blue: 0.00f, alpha: 1.0f);
            //CheckBox.OnTintColor = new UIColor(red: 0.50f, green: 0.74f, blue: 0.00f, alpha: 1.0f);
            //CheckBox.OnCheckColor = UIColor.White;
            //checkboxView.AddSubview(CheckBox);

            //
        }
        public ModalTest (IntPtr handle) : base (handle)
        {
        }
        partial void UIButton102115_TouchUpInside(UIButton sender)
        {
            DismissModalViewController(true);
        }
    }
}