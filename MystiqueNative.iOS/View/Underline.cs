using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class Underline : UITextView
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var border = new CALayer();
            nfloat width = 1.5f;
            border.BorderColor = UIColor.FromRGBA(red: 0.39f, green: 0.39f, blue: 0.39f, alpha: 0.39f).CGColor;
            border.Frame = new CoreGraphics.CGRect(0, Frame.Size.Height - width, Frame.Size.Width, Frame.Size.Height);
            border.BorderWidth = width;
            Layer.AddSublayer(border);
            Layer.MasksToBounds = true;
        }

        private void TextChangedEvent(NSNotification notifcation)
        {
            var field = (UITextView)notifcation.Object;

            if (notifcation.Object != this) return;
            if (field.Text.ToLowerInvariant().Equals("add comment"))
            {
                field.Text = string.Empty;
                field.TextColor = UIColor.FromRGB(0, 0, 0);
            }
            else if (string.IsNullOrWhiteSpace(field.Text))
            {
                field.TextColor = UIColor.FromRGB(199, 199, 205);
                field.Text = "Comentario...";
            }
        }

        public void InitData()
        {
            //Set the styling of the comment box
            //Color will look similar to the placeholder color text
            this.Layer.BorderWidth = 1;
            this.Layer.BackgroundColor = new CGColor(255f, 255f, 255f);
            this.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            this.TextColor = UIColor.FromRGB(199, 199, 205);
            this.Layer.BorderWidth = 1.0f;
            this.Layer.CornerRadius = 8.0f;
            this.Layer.MasksToBounds = true;
            this.Text = "Add Comment";

            NSNotificationCenter.DefaultCenter.AddObserver(UITextView.TextDidBeginEditingNotification, TextChangedEvent);
            NSNotificationCenter.DefaultCenter.AddObserver(UITextView.TextDidEndEditingNotification, TextChangedEvent);

       
         
        }

        public Underline (IntPtr handle) : base (handle)
        {
        }
    }
}