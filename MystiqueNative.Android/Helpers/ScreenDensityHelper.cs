using Android.Content;
using Android.Util;

namespace MystiqueNative.Droid.Helpers
{
    public static class ScreenDensityHelper
    {
        public static int DipToPx(Context ctx, int dip) => TypedValue.ComplexToDimensionPixelSize(dip, ctx.Resources.DisplayMetrics);
    }
}