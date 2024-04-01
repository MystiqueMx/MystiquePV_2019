using CoreGraphics;
using FFImageLoading;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ToCircleImage : UIImageView
    {
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            Image = ToRounded(Image, 0f, 1f, 1f, 0d, null);
        }

        public static UIImage ToRounded(UIImage source, nfloat rad, double cropWidthRatio, double cropHeightRatio, double borderSize, string borderHexColor)

        {
            double sourceWidth = source.Size.Width;
            double sourceHeight = source.Size.Height;

            double desiredWidth = sourceWidth;
            double desiredHeight = sourceHeight;

            double desiredRatio = cropWidthRatio / cropHeightRatio;
            double currentRatio = sourceWidth / sourceHeight;

            if (currentRatio > desiredRatio)
                desiredWidth = (cropWidthRatio * sourceHeight / cropHeightRatio);
            else if (currentRatio < desiredRatio)
                desiredHeight = (cropHeightRatio * sourceWidth / cropWidthRatio);

            float cropX = (float)((sourceWidth - desiredWidth) / 2);
            float cropY = (float)((sourceHeight - desiredHeight) / 2);

            if (rad == 0)
                rad = (nfloat)(Math.Min(desiredWidth, desiredHeight) / 2);
            else
                rad = (nfloat)(rad * (desiredWidth + desiredHeight) / 2 / 500);

            UIGraphics.BeginImageContextWithOptions(new CGSize(desiredWidth, desiredHeight), false, (nfloat)0.0);

            try
            {
                using (var context = UIGraphics.GetCurrentContext())
                {
                    var clippedRect = new CGRect(0d, 0d, desiredWidth, desiredHeight);

                    context.BeginPath();

                    using (var path = UIBezierPath.FromRoundedRect(clippedRect, rad))
                    {
                        context.AddPath(path.CGPath);
                        context.Clip();
                    }

                    var drawRect = new CGRect(-cropX, -cropY, sourceWidth, sourceHeight);
                    source.Draw(drawRect);

                    if (borderSize > 0d)
                    {
                        borderSize = (borderSize * (desiredWidth + desiredHeight) / 2d / 1000d);
                        var borderRect = new CGRect((0d + borderSize / 2d), (0d + borderSize / 2d),
                            (desiredWidth - borderSize), (desiredHeight - borderSize));

                        context.BeginPath();

                        using (var path = UIBezierPath.FromRoundedRect(borderRect, rad))
                        {
                            context.SetStrokeColor(borderHexColor.ToUIColor().CGColor);
                            context.SetLineWidth((nfloat)borderSize);
                            context.AddPath(path.CGPath);
                            context.StrokePath();
                        }
                    }

                    var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();

                    return modifiedImage;
                }
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                UIGraphics.EndImageContext();
            }
        }
        public ToCircleImage (IntPtr handle) : base (handle)
        {
        }
    }
}