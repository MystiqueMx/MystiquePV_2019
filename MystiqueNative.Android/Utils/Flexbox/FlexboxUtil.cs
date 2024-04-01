using Android.Content;
using Android.Content.Res;
using Android.Views;
using Com.Google.Android.Flexbox;

namespace MystiqueNative.Droid.Utils.Flexbox
{
    public static class FlexboxUtil
    {
        public static FlexboxLayout.LayoutParams GetLayoutParams(this Resources resources, int l, int t, int r, int b)
        {
            var dp = (int)resources.DisplayMetrics.Density;
            var layoutParams = new FlexboxLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.SetMargins(l * dp, t * dp, r * dp, b * dp);
            return layoutParams;
        }
    }
}