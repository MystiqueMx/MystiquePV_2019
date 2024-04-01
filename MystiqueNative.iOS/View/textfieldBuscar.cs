using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class textfieldBuscar : UITextField
    {
        
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            LeftViewMode = UITextFieldViewMode.Always;

            var SearchIcon = new UIImageView(new CGRect(15, 0, 20, 20));
            SearchIcon.Image = UIImage.FromBundle("buscar");

            UIView view = new UIView(new CGRect(0, 0, 55, 20));
            view.AddSubview(SearchIcon);

            LeftView = view;
        }
        public textfieldBuscar (IntPtr handle) : base (handle)
        {
        }
    }
}