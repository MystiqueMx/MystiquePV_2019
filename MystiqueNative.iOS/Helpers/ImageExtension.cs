using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MystiqueNative.iOS.Helpers
{
    public static class ImageExtension
    {

        public static UIImage OrientationFix(this UIImage self)
        {
            if (self.Orientation == UIImageOrientation.Up)
                return self;

            CGAffineTransform transform = CGAffineTransform.MakeIdentity();
            switch (self.Orientation)
            {
                case UIImageOrientation.Down:
                case UIImageOrientation.DownMirrored:
                    transform = CGAffineTransform.Translate(transform, self.Size.Width, self.Size.Height);
                    transform = CGAffineTransform.Rotate(transform, (nfloat)Math.PI);
                    break;

                case UIImageOrientation.Left:
                case UIImageOrientation.LeftMirrored:
                    transform = CGAffineTransform.Translate(transform, self.Size.Width, 0);
                    transform = CGAffineTransform.Rotate(transform, (nfloat)Math.PI / 2);
                    break;

                case UIImageOrientation.Right:
                case UIImageOrientation.RightMirrored:
                    transform = CGAffineTransform.Translate(transform, 0, self.Size.Height);
                    transform = CGAffineTransform.Rotate(transform, (nfloat)(-Math.PI / 2));
                    break;

                default:
                    break;
            }

            switch (self.Orientation)
            {
                case UIImageOrientation.UpMirrored:
                case UIImageOrientation.DownMirrored:
                    transform = CGAffineTransform.Translate(transform, self.Size.Width, 0);
                    transform = CGAffineTransform.Scale(transform, -1, 1);
                    break;

                case UIImageOrientation.RightMirrored:
                case UIImageOrientation.LeftMirrored:
                    transform = CGAffineTransform.Translate(transform, self.Size.Height, 0);
                    transform = CGAffineTransform.Scale(transform, -1, 1);
                    break;

                default:
                    break;
            }

            var ctx = new CGBitmapContext(
                          null, (nint)self.Size.Width, (nint)self.Size.Height, self.CGImage.BitsPerComponent, self.CGImage.BytesPerRow, self.CGImage.ColorSpace, self.CGImage.BitmapInfo);
            ctx.ConcatCTM(transform);

            switch (self.Orientation)
            {
                case UIImageOrientation.Left:
                case UIImageOrientation.LeftMirrored:
                case UIImageOrientation.Right:
                case UIImageOrientation.RightMirrored:
                    ctx.DrawImage(new CGRect(0, 0, self.Size.Height, self.Size.Width), self.CGImage);
                    break;
                default:
                    ctx.DrawImage(new CGRect(0, 0, self.Size.Width, self.Size.Height), self.CGImage);
                    break;
            }

            var cgimg = ctx.ToImage();
            var img = new UIImage(cgimg);

            ctx.Dispose();
            ctx = null;
            cgimg.Dispose();
            cgimg = null;

            return img;
        }

        public static NSData ZipToJpeg(this UIImage self, int maxWidthPixel)
        {
            UIImage newImage = null;

            if (self.Size.Width <= maxWidthPixel)
            {
                newImage = self;
            }
            else
            {
                var sc = maxWidthPixel / self.Size.Width;
                var height = sc * self.Size.Height;

                UIGraphics.BeginImageContext(new CGSize(maxWidthPixel, height));
                self.Draw(new CGRect(0, 0, maxWidthPixel, height));
                newImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }

            var data = newImage.AsJPEG(0.5f);
            return data;
        }
    }
}