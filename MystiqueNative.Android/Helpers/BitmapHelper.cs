using System;
using System.IO;
using Android.Graphics;
using Android.Support.Media;

namespace MystiqueNative.Droid.Helpers
{
    public static class BitmapHelper
    {
        private const int JpgCompressQuality = 70;
        public static Bitmap DecodeAndResize(string photo, int targetWidth)
        {
            var mBitmapOptions = new BitmapFactory.Options() { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(photo, mBitmapOptions);
            var srcWidth = mBitmapOptions.OutWidth;
            const int sampleSize = 4;

            var options = new BitmapFactory.Options()
            {
                InScaled = true,
                InSampleSize = sampleSize,
                InDensity = srcWidth,
                InTargetDensity = targetWidth * sampleSize,
            };

            return BitmapFactory.DecodeFile(photo, options);
        }
        public static Bitmap RotateImage(Bitmap img, int degree)
        {
            var matrix = new Matrix();
            matrix.PostRotate(degree);
            var rotatedImg = Bitmap.CreateBitmap(img, 0, 0, img.Width, img.Height, matrix, true);
            return rotatedImg;
        }
        public static Bitmap RotateImageBasedOnExif(this Bitmap img, string exifMetadata)
        {
            try
            {
                var exif = new ExifInterface(exifMetadata);
                var orientString = exif.GetAttribute(ExifInterface.TagOrientation);
                var orientation = orientString != null ? int.Parse(orientString) : ExifInterface.OrientationNormal;

                var rotationAngle = 0;
                switch (orientation)
                {
                    case ExifInterface.OrientationRotate90:
                        rotationAngle = 90;
                        break;
                    case ExifInterface.OrientationRotate180:
                        rotationAngle = 180;
                        break;
                    case ExifInterface.OrientationRotate270:
                        rotationAngle = 270;
                        break;
                    default:
                        break;
                }

                return rotationAngle == 0 ? img : BitmapHelper.RotateImage(img, rotationAngle);
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("~ BitmapHelper RotateImageBasedOnExif| Failed to read exif > "+ img);
                Console.WriteLine("~ BitmapHelper RotateImageBasedOnExif| Ex > "+ e.Message);
#endif
                return img;
            }
        }
        public static byte[] LoadBitmapToMemoryAsJpg(this Bitmap img)
        {
            byte[] image = null;
            using (var stream = new MemoryStream())
            {
                img.Compress(Bitmap.CompressFormat.Jpeg, JpgCompressQuality, stream);
                image = stream.ToArray();
            }
            return image;
        }
        public static Bitmap GrabBitmapFromUrl(string url)
        {

            var connection = (Java.Net.HttpURLConnection)new Java.Net.URL(url).OpenConnection();
            connection.SetRequestProperty("User-agent", "Mozilla/4.0");
            return BitmapFactory.DecodeStream(connection.InputStream);
        }
    }
}